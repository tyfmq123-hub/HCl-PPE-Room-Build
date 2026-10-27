#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

/// <summary>Headset-free regression tests for the authoritative PPE state gates.</summary>
public sealed class PPEPortfolioSequenceTests
{
    GameObject root;
    PPESequenceManager sequence;
    PPEEquipmentManager equipment;
    PPELoadoutData loadout;

    [SetUp]
    public void SetUp()
    {
        Assert.IsNull(PPESequenceManager.Instance, "Run in an empty EditMode test scene.");
        Assert.IsNull(PPEEquipmentManager.Instance, "Run in an empty EditMode test scene.");
        root = new GameObject(nameof(PPEPortfolioSequenceTests));
        equipment = root.AddComponent<PPEEquipmentManager>();
        sequence = root.AddComponent<PPESequenceManager>();
        if (PPEEquipmentManager.Instance != equipment)
            Invoke(equipment, "Awake");
        if (PPESequenceManager.Instance != sequence)
            Invoke(sequence, "Awake");
        SetField(sequence, "showBlockedFeedback", false);
    }

    [TearDown]
    public void TearDown()
    {
        if (loadout != null)
            Invoke(loadout, "OnDestroy");
        if (sequence != null)
            Invoke(sequence, "OnDestroy");
        if (equipment != null)
            Invoke(equipment, "OnDestroy");
        if (root != null)
            Object.DestroyImmediate(root);
    }

    [Test]
    public void DefaultOrder_SeparatesDonningFromPreparation_AndCompletesOnce()
    {
        var completionCount = 0;
        sequence.OnPPESequenceCompleted += () => completionCount++;
        var donning = new[]
        {
            PPESequenceStep.InnerGloves, PPESequenceStep.ProtectiveSuit,
            PPESequenceStep.ChemicalBoots, PPESequenceStep.Respirator,
            PPESequenceStep.FaceProtection, PPESequenceStep.ProtectiveGloves,
        };
        foreach (var step in donning)
        {
            Assert.AreEqual(PPETrainingPhase.Donning, sequence.CurrentPhase);
            Assert.AreEqual(step, sequence.CurrentRequiredStep);
            Assert.IsTrue(sequence.CanEquip(step));
            Assert.IsTrue(sequence.CompleteEquip(step));
        }

        Assert.IsTrue(sequence.IsDonningComplete);
        Assert.IsFalse(sequence.IsSequenceComplete);
        Assert.AreEqual(PPETrainingPhase.EquipmentPreparation, sequence.CurrentPhase);
        Assert.AreEqual(PPESequenceStep.ResponseBag, sequence.CurrentRequiredStep);
        Assert.IsFalse(sequence.CompleteEquip(PPESequenceStep.ResponseBag));
        sequence.NotifyBagContentsChecked();
        Assert.IsTrue(sequence.CompleteEquip(PPESequenceStep.ResponseBag));
        Assert.AreEqual(PPESequenceStep.WasteContainer, sequence.CurrentRequiredStep);
        Assert.IsTrue(sequence.CompleteEquip(PPESequenceStep.WasteContainer));
        Assert.AreEqual(PPETrainingPhase.Complete, sequence.CurrentPhase);
        Assert.IsTrue(sequence.IsSequenceComplete);
        Assert.IsNull(sequence.CurrentRequiredStep);
        Assert.AreEqual(1, completionCount);
        Assert.IsFalse(sequence.CompleteEquip(PPESequenceStep.WasteContainer));
        Assert.AreEqual(1, completionCount);
    }

    [TestCase(PPESequenceStep.ProtectiveSuit)]
    [TestCase(PPESequenceStep.ResponseBag)]
    [TestCase(PPESequenceStep.WasteContainer)]
    public void WrongOrder_DoesNotAdvanceOrComplete(PPESequenceStep attempted)
    {
        Assert.IsFalse(sequence.CanEquip(attempted));
        Assert.IsFalse(sequence.CompleteEquip(attempted));
        Assert.AreEqual(PPESequenceStep.InnerGloves, sequence.CurrentRequiredStep);
        Assert.IsFalse(sequence.IsStepCompleted(attempted));
    }

    [Test]
    public void DuplicateCompletion_DoesNotSkipTheNextStep()
    {
        Assert.IsTrue(sequence.CompleteEquip(PPESequenceStep.InnerGloves));
        Assert.IsFalse(sequence.CompleteEquip(PPESequenceStep.InnerGloves));
        Assert.IsFalse(sequence.CanEquip(PPESequenceStep.InnerGloves));
        Assert.AreEqual(PPESequenceStep.ProtectiveSuit, sequence.CurrentRequiredStep);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("UnknownEquipment")]
    public void InvalidItemId_IsRejected(string itemId)
    {
        Assert.IsFalse(sequence.CanEquip(itemId));
        Assert.IsFalse(sequence.CompleteEquip(itemId));
        Assert.AreEqual(PPESequenceStep.InnerGloves, sequence.CurrentRequiredStep);
    }

    [Test]
    public void ContentsInspection_AloneDoesNotSkipDonning_AndResetClearsIt()
    {
        sequence.NotifyBagContentsChecked();
        sequence.NotifyBagContentsChecked();
        Assert.IsTrue(sequence.BagContentsChecked);
        Assert.IsFalse(sequence.CanEquip(PPESequenceStep.ResponseBag));
        Assert.AreEqual(PPESequenceStep.InnerGloves, sequence.CurrentRequiredStep);
        sequence.ResetSequence();
        Assert.IsFalse(sequence.BagContentsChecked);
        Assert.IsFalse(sequence.IsSequenceComplete);
    }

    [Test]
    public void EquipmentManager_RejectsWrongOrder_ThenRegistersCorrectItemOnce()
    {
        UseSingleItemDonningOrder();
        var suit = CreateItem(PPEEquipmentManager.IdChemicalSuit);
        var respirator = CreateItem(PPEEquipmentManager.IdFaceRespirator);
        var eventCount = 0;
        equipment.EquipmentSucceeded += _ => eventCount++;

        Assert.IsFalse(equipment.Equip(respirator));
        Assert.IsFalse(respirator.IsEquipped);
        Assert.IsFalse(equipment.IsEquipped(respirator.ItemId));
        Assert.IsTrue(equipment.Equip(suit));
        Assert.IsTrue(suit.IsEquipped);
        Assert.IsTrue(equipment.IsEquipped(suit.ItemId));
        Assert.IsFalse(equipment.Equip(suit));
        Assert.AreEqual(1, eventCount);
        var ids = new List<string>();
        equipment.CopyEquippedIdsTo(ids);
        CollectionAssert.AreEqual(new[] { suit.ItemId }, ids);
    }

    [Test]
    public void RoomReset_ClearsItemManagerSequenceAndPreparationState()
    {
        UseSingleItemDonningOrder();
        var suit = CreateItem(PPEEquipmentManager.IdChemicalSuit);
        Assert.IsTrue(equipment.Equip(suit));
        equipment.SetPrepared(PPEEquipmentManager.IdWasteContainer, true);
        sequence.NotifyBagContentsChecked();

        sequence.ResetPpeRoomTraining();

        Assert.IsFalse(suit.IsEquipped);
        Assert.IsTrue(suit.gameObject.activeSelf);
        Assert.IsFalse(equipment.IsEquipped(suit.ItemId));
        Assert.IsFalse(equipment.IsPrepared(PPEEquipmentManager.IdWasteContainer));
        Assert.IsFalse(sequence.BagContentsChecked);
        Assert.IsFalse(sequence.IsSequenceComplete);
        Assert.AreEqual(PPETrainingPhase.Donning, sequence.CurrentPhase);
        Assert.AreEqual(PPESequenceStep.ProtectiveSuit, sequence.CurrentRequiredStep);
    }

    [Test]
    public void PreparationChange_IsIdempotent_AndNotReportedAsWearable()
    {
        var eventCount = 0;
        equipment.PreparationChanged += (_, __) => eventCount++;
        Assert.IsTrue(equipment.SetPrepared(PPEEquipmentManager.IdWasteContainer, true));
        Assert.IsFalse(equipment.SetPrepared(PPEEquipmentManager.IdWasteContainer, true));
        Assert.IsFalse(equipment.IsEquipped(PPEEquipmentManager.IdWasteContainer));
        Assert.IsTrue(equipment.SetPrepared(PPEEquipmentManager.IdWasteContainer, false));
        Assert.AreEqual(2, eventCount);
    }

    [Test]
    public void RecordedPartialLoadout_RemainsAuthoritative_WhenRoomManagersAreMissing()
    {
        // Other EditMode fixtures can leave a destroyed Unity wrapper in the
        // static slot. Match Unity's null semantics while rejecting live data.
        Assert.IsTrue(PPELoadoutData.Instance == null, "Run in an empty EditMode test scene.");
        var host = new GameObject("RecordedLoadout");
        host.transform.SetParent(root.transform);
        var data = host.AddComponent<PPELoadoutData>();
        loadout = data;
        if (PPELoadoutData.Instance != data)
            Invoke(data, "Awake");
        data.ReplaceWithSnapshot(new PPELoadoutSnapshot
        {
            source = PPELoadoutSource.PpeSceneResult,
            equippedItemIds = new List<string> { PPEEquipmentManager.IdInnerGloves },
            preparedItemIds = new List<string>(),
            sequenceComplete = false,
        });
        Invoke(sequence, "OnDestroy");
        Invoke(equipment, "OnDestroy");
        Object.DestroyImmediate(sequence);
        Object.DestroyImmediate(equipment);
        data.CaptureFromManagers();
        Assert.AreEqual(PPELoadoutSource.PpeSceneResult, data.Source);
        Assert.IsTrue(data.HasResolvedLoadout);
        Assert.IsFalse(PPELoadoutPresets.ShouldApplyFieldReadyPreset(data.Source));
        Assert.IsTrue(data.IsEquipped(PPEEquipmentManager.IdInnerGloves));
        Assert.IsFalse(data.IsSequenceComplete);
        Invoke(data, "OnDestroy");
    }

    void UseSingleItemDonningOrder()
    {
        SetField(sequence, "donningOrder", new List<PPESequenceStep> { PPESequenceStep.ProtectiveSuit });
        sequence.ResetSequence();
    }

    PPEEquipable CreateItem(string id)
    {
        var host = new GameObject(id);
        host.transform.SetParent(root.transform);
        var item = host.AddComponent<PPEEquipable>();
        item.EditorApplyIdentity(id, id);
        item.EditorSetDeactivateOnEquip(false);
        return item;
    }

    static void SetField(object target, string name, object value)
    {
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    }

    static void Invoke(object target, string name)
    {
        target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
    }
}
#endif
