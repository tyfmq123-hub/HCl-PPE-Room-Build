#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using HClTraining.SharedXR;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Object = UnityEngine.Object;

/// <summary>
/// Headset-free tests of packing and final confirmation. Run in an empty EditMode
/// test scene so the scene-level singleton contracts cannot touch authored content.
/// </summary>
public sealed class ResponseBagPackingTests
{
    readonly List<GameObject> objects = new List<GameObject>();
    ResponseEquipmentManager equipment;
    ResponseBagPackingController packing;
    ResponseBagPrepareInteractable prepare;
    PPESequenceManager sequence;
    SessionEquipmentInventory inventory;
    XRInteractionManager interaction;
    XRSimpleInteractable bagInteractable;
    XRRayInteractor left;
    XRRayInteractor right;
    XRInteractionConfirmRouter router;
    PPESelectionManager selection;
    PPEEquipmentManager ppeEquipment;
    GameObject bag;
    ResponseBagPackableItem[] items;

    [SetUp]
    public void SetUp()
    {
        Assert.IsNull(ResponseEquipmentManager.Instance, "Use an empty EditMode test scene.");
        Assert.IsNull(PPESequenceManager.Instance, "Use an empty EditMode test scene.");
        Assert.IsNull(SessionEquipmentInventory.Instance, "Use an empty EditMode test scene.");

        inventory = NewObject("Test inventory").AddComponent<SessionEquipmentInventory>();
        Invoke(inventory, "Awake");
        sequence = NewObject("Test sequence").AddComponent<PPESequenceManager>();
        SetField(sequence, "showBlockedFeedback", false);
        Invoke(sequence, "Awake");
        interaction = NewObject("Test XRI manager").AddComponent<XRInteractionManager>();

        var host = NewObject("Test response manager");
        equipment = host.AddComponent<ResponseEquipmentManager>();
        Invoke(equipment, "Awake");
        packing = host.AddComponent<ResponseBagPackingController>();
        Invoke(packing, "Awake");

        bag = NewObject("Test response bag");
        bag.AddComponent<BoxCollider>();
        bagInteractable = bag.AddComponent<XRSimpleInteractable>();
        bagInteractable.interactionManager = interaction;
        interaction.RegisterInteractable((IXRInteractable)bagInteractable);
        equipment.SetResponseBagRoot(bag);
        prepare = bag.AddComponent<ResponseBagPrepareInteractable>();
        prepare.EditorWire(equipment, bag, bagInteractable);
        Invoke(prepare, "OnEnable");

        var trigger = NewObject("Test packing volume").AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = Vector3.one;
        SetField(packing, "packingTrigger", trigger);

        items = new[]
        {
            NewItem(ResponseEquipmentManager.IdGasDetector),
            NewItem(ResponseEquipmentManager.IdAbsorbentPad),
            NewItem(ResponseEquipmentManager.IdAbsorbentSock),
        };
        SetField(packing, "gasItem", items[0]);
        SetField(packing, "padItem", items[1]);
        SetField(packing, "sockItem", items[2]);

        left = NewInteractor("Test left ray", InteractorHandedness.Left);
        right = NewInteractor("Test right ray", InteractorHandedness.Right);
    }

    [TearDown]
    public void TearDown()
    {
        // These fixtures initialize scene singletons explicitly in EditMode;
        // clear them explicitly too, independent of editor callback delivery.
        if (packing != null)
            Invoke(packing, "OnDestroy");
        if (router != null)
            Invoke(router, "OnDestroy");
        if (selection != null)
            Invoke(selection, "OnDestroy");
        if (ppeEquipment != null)
            Invoke(ppeEquipment, "OnDestroy");
        if (equipment != null)
            Invoke(equipment, "OnDestroy");
        if (sequence != null)
            Invoke(sequence, "OnDestroy");
        if (inventory != null)
            Invoke(inventory, "OnDestroy");
        for (var i = objects.Count - 1; i >= 0; i--)
            if (objects[i] != null)
                Object.DestroyImmediate(objects[i]);
        objects.Clear();
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void AllPacked_RequiresEachOfTheThreeItems(int missingIndex)
    {
        for (var i = 0; i < items.Length; i++)
            if (i != missingIndex)
                Pack(items[i]);

        Assert.IsFalse(packing.AllPacked);
        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void ReleaseInsideBag_StoresEachItemOnce_AndWaitsForFinalGrip()
    {
        AdvanceToBag();
        PackAll();
        foreach (var item in items)
        {
            packing.NotifyItemReleased(item);
            Assert.AreEqual(1, inventory.GetStoredCount(item.ItemId));
        }

        Assert.IsTrue(packing.AllPacked);
        Assert.IsTrue(equipment.AreAllResponseBagItemsInspected());
        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Assert.IsTrue(bag.activeSelf);
        Assert.AreEqual(PPESequenceStep.ResponseBag, sequence.CurrentRequiredStep);
    }

    [Test]
    public void SharedSelect_WithoutLiveBagTarget_DoesNotPrepare()
    {
        ReadyBag();
        SharedInput(SharedXRHand.Right, SharedXRControl.Select);

        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Assert.IsTrue(bag.activeSelf);
        Assert.AreEqual(PPESequenceStep.ResponseBag, sequence.CurrentRequiredStep);
    }

    [Test]
    public void SharedSelect_FromOtherHand_DoesNotPrepare()
    {
        ReadyBag();
        Hover(left);
        SharedInput(SharedXRHand.Right, SharedXRControl.Select);

        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void SharedSelect_FromHoveringHand_PreparesExactlyOnce()
    {
        ReadyBag();
        Hover(right);
        var completed = 0;
        equipment.ResponseBagPreparedSucceeded += () => completed++;

        SharedInput(SharedXRHand.Right, SharedXRControl.Select);
        SharedInput(SharedXRHand.Right, SharedXRControl.Select);

        Assert.IsTrue(equipment.IsResponseBagPrepared());
        Assert.IsFalse(bag.activeSelf);
        Assert.AreEqual(1, completed);
        Assert.AreEqual(PPESequenceStep.WasteContainer, sequence.CurrentRequiredStep);
    }

    [Test]
    public void SharedTrigger_WhileHovering_DoesNotPrepare()
    {
        ReadyBag();
        Hover(right);
        SharedInput(SharedXRHand.Right, SharedXRControl.Trigger);

        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void PackingGrip_WithMissingItem_DoesNotPrepare()
    {
        AdvanceToBag();
        Pack(items[0]);
        Pack(items[1]);
        Hover(right);

        Assert.IsFalse(prepare.TryPrepareFromPackingGripHover(SharedXRHand.Right));
        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void PackingGrip_DuringDonning_DoesNotSkipSequence()
    {
        PackAll();
        Hover(right);

        Assert.IsFalse(prepare.TryPrepareFromPackingGripHover(SharedXRHand.Right));
        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Assert.AreEqual(PPETrainingPhase.Donning, sequence.CurrentPhase);
    }

    [Test]
    public void StickySelection_AfterHoverExit_DoesNotPrepare()
    {
        ReadyBag();
        Hover(right);
        SetStickySelection();
        interaction.HoverExit((IXRHoverInteractor)right, bagInteractable);

        Assert.IsFalse(prepare.CanConfirmPreparation());
        Assert.IsFalse(prepare.TryPrepareFromConfirmRouter());
        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void StandaloneRouter_OtherHandCannotUseStickySelectionBypass()
    {
        ReadyBag();
        Hover(left);
        SetStickySelection();
        router = NewObject("Test confirm router").AddComponent<XRInteractionConfirmRouter>();
        router.EditorWire(prepare, null);
        var handType = typeof(XRInteractionConfirmRouter).GetNestedType("ControllerHand", BindingFlags.NonPublic);

        Invoke(router, "RouteConfirm", Enum.Parse(handType, "Right"));

        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Invoke(router, "RouteConfirm", Enum.Parse(handType, "Left"));
        Assert.IsTrue(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void LegacyWearableFallback_CannotEquipStickyPackedBagElsewhere()
    {
        ReadyBag();
        var item = bag.AddComponent<PPEEquipable>();
        item.EditorApplyIdentity(PPEEquipmentManager.IdResponseBag, "Test bag");
        SetLegacySelection(item);
        SetStickySelection();

        Assert.IsTrue(selection.HasValidEquipSelection(0f), "Regression setup must be eligible for the legacy fallback.");
        RouteLegacyConfirm("Right");

        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Assert.IsFalse(ppeEquipment.IsEquipped(PPEEquipmentManager.IdResponseBag));
        Assert.IsTrue(bag.activeSelf);
    }

    [Test]
    public void LegacyWearableFallback_StillEquipsAnAllowedNonBagItem()
    {
        SetField(sequence, "donningOrder", new List<PPESequenceStep> { PPESequenceStep.ProtectiveSuit });
        var itemRoot = NewObject("Test suit");
        itemRoot.AddComponent<XRSimpleInteractable>();
        var item = itemRoot.AddComponent<PPEEquipable>();
        item.EditorApplyIdentity(PPEEquipmentManager.IdChemicalSuit, "Test suit");
        SetLegacySelection(item);

        RouteLegacyConfirm("Right");

        Assert.IsTrue(ppeEquipment.IsEquipped(PPEEquipmentManager.IdChemicalSuit));
        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void LegacyActivate_RequiresTheEventInteractorToTargetTheBag()
    {
        ReadyBag();
        Hover(left);
        prepare.SetConfirmInputOwnedByRouter(false);
        Invoke(prepare, "OnEnable");

        bagInteractable.activated.Invoke(new ActivateEventArgs
        {
            interactorObject = right,
            interactableObject = bagInteractable,
        });
        Assert.IsFalse(equipment.IsResponseBagPrepared());

        bagInteractable.activated.Invoke(new ActivateEventArgs
        {
            interactorObject = left,
            interactableObject = bagInteractable,
        });
        Assert.IsTrue(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void ExplicitXriSelect_ConfirmsFullyPackedBag()
    {
        ReadyBag();
        interaction.SelectEnter((IXRSelectInteractor)right, bagInteractable);

        Assert.IsTrue(equipment.IsResponseBagPrepared());
        Assert.AreEqual(PPESequenceStep.WasteContainer, sequence.CurrentRequiredStep);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void DisabledCallbackOrInteractable_RejectsFinalGrip(bool disableCallback)
    {
        ReadyBag();
        Hover(right);
        if (disableCallback)
            prepare.enabled = false;
        else
            bagInteractable.enabled = false;

        SharedInput(SharedXRHand.Right, SharedXRControl.Select);

        Assert.IsFalse(equipment.IsResponseBagPrepared());
    }

    [Test]
    public void ResetResponseBag_ClearsPreparedContentsAndStaleTriggerContacts()
    {
        // This minimal fixture verifies logical inspection/contact cleanup. Desk
        // layout, item home poses, and the authored scene reset are device checks.
        ReadyBag();
        Hover(right);
        Assert.IsTrue(prepare.TryPrepareFromPackingGripHover(SharedXRHand.Right));

        equipment.ResetResponseBag();

        Assert.IsFalse(equipment.IsResponseBagPrepared());
        Assert.IsFalse(equipment.AreAllResponseBagItemsInspected());
        Assert.IsTrue(bag.activeSelf);
        var contacts = GetField<HashSet<Collider>>(packing, "insideTrigger");
        Assert.AreEqual(0, contacts.Count);
        Hover(right);
        Assert.IsFalse(prepare.TryPrepareFromPackingGripHover(SharedXRHand.Right));
    }

    [Test]
    public void LegacyNoArgumentApi_StillRequiresLiveTarget()
    {
        ReadyBag();
        Assert.IsFalse(prepare.TryPrepareFromPackingGripHover());
        Hover(left);
        Assert.IsTrue(prepare.TryPrepareFromPackingGripHover());
    }

    GameObject NewObject(string name)
    {
        var go = new GameObject(name);
        objects.Add(go);
        return go;
    }

    ResponseBagPackableItem NewItem(string itemId)
    {
        var go = NewObject("Test " + itemId);
        go.AddComponent<BoxCollider>();
        var item = go.AddComponent<ResponseBagPackableItem>();
        item.EditorWire(itemId, packing);
        return item;
    }

    XRRayInteractor NewInteractor(string name, InteractorHandedness hand)
    {
        var interactor = NewObject(name).AddComponent<XRRayInteractor>();
        // EditMode AddComponent does not run XRBaseInteractor.Awake, which
        // normally creates this attach point. Select cancellation restores it.
        if (interactor.attachTransform == null)
        {
            var attach = new GameObject(name + " Attach").transform;
            attach.SetParent(interactor.transform, false);
            interactor.attachTransform = attach;
        }
        interactor.handedness = hand;
        interactor.interactionManager = interaction;
        interaction.RegisterInteractor((IXRInteractor)interactor);
        return interactor;
    }

    void Pack(ResponseBagPackableItem item)
    {
        packing.NotifyTriggerEnter(item.GetComponent<Collider>());
        packing.NotifyItemReleased(item);
    }

    void PackAll()
    {
        foreach (var item in items)
            Pack(item);
    }

    void AdvanceToBag()
    {
        while (sequence.CurrentPhase == PPETrainingPhase.Donning)
        {
            Assert.IsTrue(sequence.CurrentRequiredStep.HasValue);
            Assert.IsTrue(sequence.CompleteEquip(sequence.CurrentRequiredStep.Value));
        }
    }

    void ReadyBag()
    {
        AdvanceToBag();
        PackAll();
    }

    void Hover(XRRayInteractor interactor)
    {
        interaction.HoverEnter((IXRHoverInteractor)interactor, bagInteractable);
    }

    void SetStickySelection()
    {
        SetField(prepare, "bagSelected", true);
        SetField(prepare, "selectedAtTime", -1000f);
    }

    void SetLegacySelection(PPEEquipable item)
    {
        ppeEquipment = NewObject("Test PPE equipment").AddComponent<PPEEquipmentManager>();
        Invoke(ppeEquipment, "Awake");
        selection = NewObject("Test sticky selection").AddComponent<PPESelectionManager>();
        var highlight = item.gameObject.AddComponent<PPEOutlineHighlight>();
        SetField(selection, "currentSelectedPPE", highlight);
        SetField(selection, "selectedAtTime", -1000f);
        router = NewObject("Test legacy router").AddComponent<XRInteractionConfirmRouter>();
        router.EditorWire(prepare, selection);
        SetField(router, "allowLegacyWearableEquipFallback", true);
    }

    void RouteLegacyConfirm(string hand)
    {
        var handType = typeof(XRInteractionConfirmRouter).GetNestedType("ControllerHand", BindingFlags.NonPublic);
        Invoke(router, "RouteConfirm", Enum.Parse(handType, hand));
    }

    void SharedInput(SharedXRHand hand, SharedXRControl control)
    {
        Invoke(packing, "OnSharedInputPerformed", hand, control);
    }

    static void SetField(object target, string name, object value)
    {
        var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, name);
        field.SetValue(target, value);
    }

    static T GetField<T>(object target, string name)
    {
        var field = target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(field, name);
        return (T)field.GetValue(target);
    }

    static void Invoke(object target, string name, params object[] args)
    {
        var method = target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, name);
        method.Invoke(target, args);
    }
}
#endif
