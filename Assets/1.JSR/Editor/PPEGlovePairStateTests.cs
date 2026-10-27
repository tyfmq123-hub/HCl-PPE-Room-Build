#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Isolated pair-state regressions. No live controller or headset is required.
/// Reflection is limited to fixture setup of serialized state/singletons;
/// assertions exercise the existing public completion/reset APIs.
/// </summary>
public sealed class PPEGlovePairStateTests
{
    GameObject root;
    PPEEquipmentManager manager;
    PPESequenceManager sequence;
    PPEEquipmentManager previousManager;
    PPESequenceManager previousSequence;
    PPEGlovePairCoordinator pair;
    PPEEquipable logicalItem;
    PPEGlovePairMember left;
    PPEGlovePairMember right;

    [SetUp]
    public void SetUp()
    {
        previousManager = PPEEquipmentManager.Instance;
        previousSequence = PPESequenceManager.Instance;
        root = new GameObject(nameof(PPEGlovePairStateTests));
        // Avoid XR/target Awake setup: these tests control only pair bookkeeping.
        root.SetActive(false);
        manager = Child("Equipment").AddComponent<PPEEquipmentManager>();
        sequence = Child("Sequence").AddComponent<PPESequenceManager>();
        SetSingleton(typeof(PPEEquipmentManager), manager);
        SetSingleton(typeof(PPESequenceManager), sequence);
        SetField(sequence, "donningOrder", new List<PPESequenceStep>
        {
            PPESequenceStep.ProtectiveGloves,
        });
        SetField(sequence, "equipmentPreparationOrder", new List<PPESequenceStep>
        {
            PPESequenceStep.ResponseBag, PPESequenceStep.WasteContainer,
        });
        SetField(sequence, "showBlockedFeedback", false);

        var pairRoot = Child("LogicalGloves");
        logicalItem = pairRoot.AddComponent<PPEEquipable>();
        logicalItem.EditorApplyIdentity(PPEEquipmentManager.IdChemicalGloves, "Test gloves");
        logicalItem.EditorSetDeactivateOnEquip(false);
        pair = pairRoot.AddComponent<PPEGlovePairCoordinator>();
        SetField(pair, "logicalItem", logicalItem);
        SetField(pair, "cachedEquipmentManager", manager);
        left = Member("Left", PPEGrabHand.Left);
        right = Member("Right", PPEGrabHand.Right);
        pair.EditorAssignMembers(left, right);
    }

    [TearDown]
    public void TearDown()
    {
        if (root != null)
            Object.DestroyImmediate(root);
        SetSingleton(typeof(PPEEquipmentManager), previousManager);
        SetSingleton(typeof(PPESequenceManager), previousSequence);
    }

    [TestCase(PPEGrabHand.Left)]
    [TestCase(PPEGrabHand.Right)]
    public void FirstSide_DoesNotCompleteLogicalEquipment(PPEGrabHand side)
    {
        var member = side == PPEGrabHand.Left ? left : right;
        Assert.That(pair.NotifySideEquipped(member, true), Is.True);
        Assert.That(pair.LeftEquipped, Is.EqualTo(side == PPEGrabHand.Left));
        Assert.That(pair.RightEquipped, Is.EqualTo(side == PPEGrabHand.Right));
        Assert.That(member.gameObject.activeSelf, Is.False);
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.False);
        Assert.That(sequence.CurrentRequiredStep, Is.EqualTo(PPESequenceStep.ProtectiveGloves));
    }

    [TestCase(PPEGrabHand.Left)]
    [TestCase(PPEGrabHand.Right)]
    public void BothSides_RegisterOneLogicalItemAndAdvanceOnce(PPEGrabHand firstSide)
    {
        var succeeded = 0;
        manager.EquipmentSucceeded += _ => succeeded++;
        var first = firstSide == PPEGrabHand.Left ? left : right;
        var second = firstSide == PPEGrabHand.Left ? right : left;
        Assert.That(pair.NotifySideEquipped(first, true), Is.True);
        Assert.That(pair.NotifySideEquipped(second, true), Is.True);
        Assert.That(pair.BothEquipped, Is.True);
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.True);
        Assert.That(logicalItem.IsEquipped, Is.True);
        Assert.That(succeeded, Is.EqualTo(1));
        Assert.That(sequence.CurrentRequiredStep, Is.EqualTo(PPESequenceStep.ResponseBag));
        Assert.That(pair.NotifySideEquipped(first, true), Is.False);
        Assert.That(pair.NotifySideEquipped(second, true), Is.False);
        Assert.That(succeeded, Is.EqualTo(1));
    }

    [Test]
    public void DuplicatePartialSide_DoesNotCountAsBothSides()
    {
        Assert.That(pair.NotifySideEquipped(left, true), Is.True);
        Assert.That(pair.NotifySideEquipped(left, true), Is.False);
        Assert.That(pair.LeftEquipped, Is.True);
        Assert.That(pair.RightEquipped, Is.False);
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.False);
    }

    [Test]
    public void WrongSequence_PreservesBothPhysicalMembers()
    {
        SetField(sequence, "donningOrder", new List<PPESequenceStep>
        {
            PPESequenceStep.InnerGloves, PPESequenceStep.ProtectiveGloves,
        });
        Assert.That(pair.NotifySideEquipped(left, true), Is.False);
        Assert.That(pair.NotifyInnerGloveSideFromRelease(right), Is.False);
        Assert.That(pair.LeftEquipped, Is.False);
        Assert.That(pair.RightEquipped, Is.False);
        Assert.That(left.gameObject.activeSelf, Is.True);
        Assert.That(right.gameObject.activeSelf, Is.True);
    }

    [TestCase(false, PPEGrabHand.Left)]
    [TestCase(false, PPEGrabHand.Right)]
    [TestCase(true, PPEGrabHand.Left)]
    [TestCase(true, PPEGrabHand.Right)]
    public void MissingManager_RollsBackSecondSideForRetry(bool releasePath, PPEGrabHand secondSide)
    {
        var first = secondSide == PPEGrabHand.Left ? right : left;
        var second = secondSide == PPEGrabHand.Left ? left : right;
        Assert.That(pair.NotifySideEquipped(first, true), Is.True);
        Object.DestroyImmediate(manager.gameObject);
        SetSingleton(typeof(PPEEquipmentManager), null);
        Assert.That(Object.FindAnyObjectByType<PPEEquipmentManager>(), Is.Null,
            "This isolated test requires no other active equipment manager.");

        Assert.That(CompleteSecond(releasePath, second), Is.False);
        AssertRolledBackSecondSide(secondSide);

        manager = Child("ReplacementEquipment").AddComponent<PPEEquipmentManager>();
        SetSingleton(typeof(PPEEquipmentManager), manager);
        SetField(pair, "cachedEquipmentManager", manager);
        Assert.That(pair.NotifySideEquipped(second, true), Is.True);
        Assert.That(pair.BothEquipped, Is.True);
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.True);
    }

    [TestCase(false, PPEGrabHand.Left)]
    [TestCase(false, PPEGrabHand.Right)]
    [TestCase(true, PPEGrabHand.Left)]
    [TestCase(true, PPEGrabHand.Right)]
    public void RejectedLogicalEquip_RollsBackOnlySecondSideAndCanRetry(bool releasePath, PPEGrabHand secondSide)
    {
        var first = secondSide == PPEGrabHand.Left ? right : left;
        var second = secondSide == PPEGrabHand.Left ? left : right;
        var grab = second.gameObject.AddComponent<PPEHandGrabbable>();
        Assert.That(pair.NotifySideEquipped(first, true), Is.True);
        // A real manager failure: local serialized state is already marked,
        // while the manager has no equipment registration for this item.
        logicalItem.MarkAsEquipped();
        Assert.That(CompleteSecond(releasePath, second), Is.False);
        AssertRolledBackSecondSide(secondSide);
        Assert.That(grab.enabled, Is.True, "The restored side must remain grabbable.");
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.False);
        Assert.That(sequence.CurrentRequiredStep, Is.EqualTo(PPESequenceStep.ProtectiveGloves));

        logicalItem.ResetEquipState();
        Assert.That(pair.NotifySideEquipped(second, true), Is.True);
        Assert.That(pair.BothEquipped, Is.True);
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.True);
    }

    [Test]
    public void ResetAfterPartialCompletion_RestoresBothSidesAndAllowsNewAttempt()
    {
        Assert.That(pair.NotifySideEquipped(right, true), Is.True);
        pair.ResetPair();
        Assert.That(pair.LeftEquipped, Is.False);
        Assert.That(pair.RightEquipped, Is.False);
        Assert.That(left.gameObject.activeSelf, Is.True);
        Assert.That(right.gameObject.activeSelf, Is.True);
        Assert.That(pair.DebugCompleteSideCalled, Is.False);
        Assert.That(pair.NotifySideEquipped(right, true), Is.True);
    }

    [Test]
    public void MemberFromAnotherPair_IsRejectedWithoutMutation()
    {
        var foreign = Child("ForeignMember").AddComponent<PPEGlovePairMember>();
        foreign.Configure(null, PPEGrabHand.Left, foreign.transform, null);
        Assert.That(pair.NotifySideEquipped(foreign, true), Is.False);
        Assert.That(pair.NotifyInnerGloveSideFromRelease(foreign), Is.False);
        Assert.That(pair.LeftEquipped, Is.False);
        Assert.That(foreign.gameObject.activeSelf, Is.True);
    }

    [Test]
    public void PairRelease_PropagatesRegistrationFailureToStoredState()
    {
        logicalItem.EditorApplyIdentity(PPEEquipmentManager.IdChemicalBoots, "Test boots");
        SetField(sequence, "donningOrder", new List<PPESequenceStep>
        {
            PPESequenceStep.ChemicalBoots,
        });
        pair.EditorApplyPairSettings(PPEPairTargetKind.BodyLowerLeg, false, 0.12f, 40f, 0f);
        left.Configure(pair, PPEGrabHand.Left, left.transform, null);
        right.Configure(pair, PPEGrabHand.Right, right.transform, null);
        // Lower-leg targets explicitly support a foot-anchor fallback, so this
        // exercises the actual release API without pretending a headset tracks.
        var target = Child("RightLowerLeg").AddComponent<PPELowerLegTargetFollower>();
        target.Configure(PPEGrabHand.Right, target.transform, 0f);
        SetField(pair, "leftTrackedTarget", target.transform);
        SetField(pair, "rightTrackedTarget", target.transform);
        right.transform.position = target.transform.position;
        var grab = right.gameObject.AddComponent<PPEHandGrabbable>();
        var stored = right.gameObject.AddComponent<PPEStoredDisplayState>();
        Invoke(stored, "Awake");
        Assert.That(pair.NotifySideEquipped(left, true), Is.True);
        logicalItem.MarkAsEquipped();

        Assert.That(PPEReleaseEquip.TryEquipOnRelease(grab, PPEGrabHand.Left), Is.False);
        AssertRolledBackSecondSide();
        Assert.That(stored.State, Is.EqualTo(PPEStoredDisplayState.StorageState.Stored));
        Assert.That(manager.IsEquipped(logicalItem.ItemId), Is.False);
    }

    bool CompleteSecond(bool releasePath, PPEGlovePairMember member)
    {
        return releasePath
            ? pair.NotifyInnerGloveSideFromRelease(member)
            : pair.NotifySideEquipped(member, true);
    }

    void AssertRolledBackSecondSide(PPEGrabHand rejectedSide = PPEGrabHand.Right)
    {
        Assert.That(pair.LeftEquipped, Is.EqualTo(rejectedSide != PPEGrabHand.Left));
        Assert.That(pair.RightEquipped, Is.EqualTo(rejectedSide != PPEGrabHand.Right));
        Assert.That(pair.BothEquipped, Is.False);
        Assert.That(left.gameObject.activeSelf, Is.EqualTo(rejectedSide == PPEGrabHand.Left));
        Assert.That(right.gameObject.activeSelf, Is.EqualTo(rejectedSide == PPEGrabHand.Right));
        Assert.That(pair.DebugCompleteSideSucceeded, Is.False);
    }

    GameObject Child(string name)
    {
        var child = new GameObject(name);
        child.transform.SetParent(root.transform, false);
        return child;
    }

    PPEGlovePairMember Member(string name, PPEGrabHand side)
    {
        var child = Child(name);
        child.transform.SetParent(pair.transform, false);
        var member = child.AddComponent<PPEGlovePairMember>();
        member.Configure(pair, side, member.transform, null);
        return member;
    }

    static void SetField(object target, string name, object value)
    {
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(target, value);
    }

    static void SetSingleton(System.Type type, object value)
    {
        type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            .GetSetMethod(true).Invoke(null, new[] { value });
    }

    static void Invoke(object target, string name)
    {
        target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(target, null);
    }
}
#endif
