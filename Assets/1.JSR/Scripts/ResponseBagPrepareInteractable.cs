using HClTraining.SharedXR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Ray-select the world response bag (PPE outline sticky), then confirm preparation via
/// <see cref="XRInteractionConfirmRouter"/> (or XR Activate with the same guards).
/// Not wearable PPE — does not route through PPEEquipmentManager.Equip.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(XRSimpleInteractable))]
public class ResponseBagPrepareInteractable : MonoBehaviour
{
    static readonly string DefaultDisplayName =
        "\uD654\uD559\uBB3C\uC9C8 \uB204\uCD9C \uB300\uC751\uAC00\uBC29";

    [Header("References")]
    [SerializeField] ResponseEquipmentManager equipmentManager;
    [SerializeField] GameObject responseBagRoot;
    [SerializeField] XRBaseInteractable interactable;
    [SerializeField] PPEOutlineHighlight outlineHighlight;

    [Header("Display")]
    [SerializeField] string displayName = DefaultDisplayName;

    [Header("Prepare")]
    [SerializeField] AudioSource completionAudio;

    [Header("Confirm Timing")]
    [SerializeField]
    [Min(0f)]
    float confirmDelay = 0.3f;

    [Tooltip("When true, this component does not subscribe to Grip — XRInteractionConfirmRouter owns confirm.")]
    [SerializeField] bool confirmInputOwnedByRouter = true;

    bool bagSelected;
    bool preparationInProgress;
    float selectedAtTime = -999f;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName) ? DefaultDisplayName : displayName;

    public bool IsBagSelected => IsStickySelected();
    public bool PreparationInProgress => preparationInProgress;

    void Awake()
    {
        ResolveRefs();
    }

    void OnEnable()
    {
        ResolveRefs();
        WireInteractable(true);
    }

    void OnDisable()
    {
        WireInteractable(false);
        if (!preparationInProgress)
            bagSelected = false;
    }

    void OnDestroy()
    {
        WireInteractable(false);
    }

    public void SetConfirmInputOwnedByRouter(bool owned)
    {
        confirmInputOwnedByRouter = owned;
    }

    void ResolveRefs()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (outlineHighlight == null)
            outlineHighlight = GetComponent<PPEOutlineHighlight>();

        if (responseBagRoot == null)
            responseBagRoot = gameObject;

        if (equipmentManager == null)
        {
            equipmentManager = ResponseEquipmentManager.Instance != null
                ? ResponseEquipmentManager.Instance
                : FindAnyObjectByType<ResponseEquipmentManager>();
        }
    }

    void WireInteractable(bool subscribe)
    {
        if (interactable == null)
            return;

        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
        interactable.activated.RemoveListener(OnActivated);

        if (!subscribe)
            return;

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);

        // Grip confirmation is owned by XRInteractionConfirmRouter — skip Activate to avoid
        // double prepare when grip also maps to XR Activate.
        if (!confirmInputOwnedByRouter)
            interactable.activated.AddListener(OnActivated);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (equipmentManager != null && equipmentManager.IsResponseBagPrepared())
            return;

        bagSelected = true;
        selectedAtTime = Time.unscaledTime;

        // Packing owns when the bag becomes selectable. The first Grip after all
        // three items have been packed is the explicit "take the bag" action.
        var packing = ResponseBagPackingController.Instance;
        if (packing != null && CanConfirmPackingGrip(ToSharedHand(args.interactorObject), args.interactorObject))
            BeginPreparation();
    }

    void OnSelectExited(SelectExitEventArgs _)
    {
        // Sticky like PPE: keep bagSelected after ray Toggle release/deselect.
        // Cleared when outline selection changes, prepare completes, or OnDisable.
        if (preparationInProgress)
            return;

        if (outlineHighlight != null && outlineHighlight.IsSelected)
            return;

        // No outline: allow sticky bagSelected until prepare / disable.
    }

    void OnActivated(ActivateEventArgs args)
    {
        // Legacy Activate ownership still receives an explicit XRI event. In the
        // packing flow that exact interactor must target this bag, even when its
        // handedness is unavailable; another hand's hover cannot authorize it.
        if (ResponseBagPackingController.Instance != null)
        {
            if (args == null || args.interactorObject == null ||
                !CanConfirmPackingGrip(ToSharedHand(args.interactorObject), args.interactorObject))
                return;
            BeginPreparation();
            return;
        }

        if (!CanConfirmPreparation())
            return;

        TryPrepareFromConfirmRouter();
    }

    /// <summary>
    /// Shared guard for Grip router, XR Activate, UI, and context-menu confirmation.
    /// </summary>
    public bool CanConfirmPreparation()
    {
        ResolveRefs();

        if (preparationInProgress)
            return false;

        if (!IsStickySelected())
            return false;

        if (Time.unscaledTime - GetEffectiveSelectedAtTime() < confirmDelay)
            return false;

        if (equipmentManager == null || !equipmentManager)
            return false;

        if (equipmentManager.IsResponseBagPrepared())
            return false;

        var root = responseBagRoot != null ? responseBagRoot : gameObject;
        if (root == null || !root.activeInHierarchy)
            return false;

        if (interactable == null)
            return false;

        // In the packing flow, an old sticky selection cannot authorize a later
        // Grip elsewhere. Keep the legacy public confirmation entry guarded too.
        if (ResponseBagPackingController.Instance != null && !CanConfirmPackingGrip(null))
            return false;

        if (!interactable.transform.IsChildOf(root.transform)
            && interactable.transform != root.transform)
            return false;

        return true;
    }

    /// <summary>Confirm entry used by <see cref="XRInteractionConfirmRouter"/> and Activate.</summary>
    public bool TryPrepareFromConfirmRouter()
    {
        if (!CanConfirmPreparation())
            return false;

        return BeginPreparation();
    }

    /// <summary>
    /// Final packing action: one Grip while the hand/near-far interactor is already
    /// hovering the enabled bag takes it immediately. This intentionally bypasses
    /// the generic PPE two-press sticky-selection delay.
    /// </summary>
    public bool TryPrepareFromPackingGripHover()
    {
        return TryPrepareFromPackingGripHover(null);
    }

    /// <summary>Uses the currently targeting interactor from the actual input hand.</summary>
    public bool TryPrepareFromPackingGripHover(SharedXRHand? inputHand)
    {
        ResolveRefs();

        if (!CanConfirmPackingGrip(inputHand))
            return false;

        return BeginPreparation();
    }

    bool CanConfirmPackingGrip(SharedXRHand? inputHand, IXRInteractor inputInteractor = null)
    {
        if (!isActiveAndEnabled || preparationInProgress ||
            interactable == null || !interactable.isActiveAndEnabled)
            return false;

        var packing = ResponseBagPackingController.Instance;
        if (packing == null || !packing.AllPacked)
            return false;

        if (equipmentManager == null || equipmentManager.IsResponseBagPrepared())
            return false;

        var root = responseBagRoot != null ? responseBagRoot : gameObject;
        if (root == null || !root.activeInHierarchy || equipmentManager.ResponseBagRoot != root ||
            (interactable.transform != root.transform && !interactable.transform.IsChildOf(root.transform)))
            return false;

        // Hover works for both Ray and Near/Far without depending on XRI callback
        // order. Selection is explicit interaction too. Never use sticky outline
        // state or a far ray's attach point as evidence of a current target.
        for (var i = 0; i < interactable.interactorsHovering.Count; i++)
            if (MatchesInputHand(interactable.interactorsHovering[i], inputHand, inputInteractor))
                return true;
        for (var i = 0; i < interactable.interactorsSelecting.Count; i++)
            if (MatchesInputHand(interactable.interactorsSelecting[i], inputHand, inputInteractor))
                return true;
        return false;
    }

    static bool MatchesInputHand(IXRInteractor interactor, SharedXRHand? inputHand, IXRInteractor inputInteractor)
    {
        if (interactor == null || interactor.transform == null ||
            !interactor.transform.gameObject.activeInHierarchy ||
            (interactor is Behaviour behaviour && !behaviour.isActiveAndEnabled))
            return false;
        if (inputInteractor != null && !ReferenceEquals(interactor, inputInteractor))
            return false;

        // Existing no-argument API and simulator inputs may not identify a hand;
        // those still require a live bag target. Known physical input must match.
        return !inputHand.HasValue || ToSharedHand(interactor) == inputHand;
    }

    static SharedXRHand? ToSharedHand(IXRInteractor interactor)
    {
        if (interactor == null)
            return null;
        switch (interactor.handedness)
        {
            case InteractorHandedness.Left: return SharedXRHand.Left;
            case InteractorHandedness.Right: return SharedXRHand.Right;
            default: return null;
        }
    }

    bool IsStickySelected()
    {
        if (outlineHighlight != null && outlineHighlight.IsSelected)
            return true;

        return bagSelected;
    }

    float GetEffectiveSelectedAtTime()
    {
        if (outlineHighlight != null
            && outlineHighlight.IsSelected
            && PPESelectionManager.Instance != null
            && PPESelectionManager.Instance.CurrentSelectedPPE == outlineHighlight)
        {
            return PPESelectionManager.Instance.SelectedAtTime;
        }

        return selectedAtTime;
    }

    bool BeginPreparation()
    {
        if (preparationInProgress)
            return false;

        ResolveRefs();
        preparationInProgress = true;

        try
        {
            if (equipmentManager == null)
                return false;

            return equipmentManager.TryCompleteResponseBagPreparation();
        }
        finally
        {
            preparationInProgress = false;
            bagSelected = false;
        }
    }

    public void EditorWire(
        ResponseEquipmentManager manager,
        GameObject bagRoot,
        XRBaseInteractable xrInteractable)
    {
        equipmentManager = manager;
        responseBagRoot = bagRoot != null ? bagRoot : gameObject;
        interactable = xrInteractable != null ? xrInteractable : GetComponent<XRBaseInteractable>();
        outlineHighlight = GetComponent<PPEOutlineHighlight>();
        displayName = DefaultDisplayName;
        confirmInputOwnedByRouter = true;
    }

#if UNITY_EDITOR
    [ContextMenu("Test Prepare (Guarded)")]
    void ContextTestPrepare()
    {
        bagSelected = true;
        selectedAtTime = Time.unscaledTime - confirmDelay;
        if (outlineHighlight != null)
            outlineHighlight.SetSelected(true);
        TryPrepareFromConfirmRouter();
    }
#endif
}
