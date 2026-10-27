// Validation-only Editor probe. Keep this source outside Assets in the original
// checkout. Copy into the disposable validation project's Editor folder only.
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UnityPpeValidationProbe
{
    [Serializable]
    public sealed class CaseRecord
    {
        public string fullName;
        public string runState;
        public string skipReason;
    }

    [Serializable]
    public sealed class ProbeReport
    {
        public string editorVersion;
        public string projectPath;
        public string completedAtUtc;
        public string scenePath = "Assets/Scenes/04_PPE.unity";
        public int sceneGameObjects;
        public int sceneComponents;
        public int missingScriptCount;
        public List<string> missingScriptObjects = new List<string>();
        public List<string> brokenReferences = new List<string>();
        public List<string> scanErrors = new List<string>();
        public int totalDiscoveredLeaves;
        public int sequenceCases;
        public int gloveCases;
        public int bagCases;
        public List<CaseRecord> newCases = new List<CaseRecord>();
        public string discoveryError;
        public string[] tintExpectedRgba;
        public string[] tintActualRgba;
        public float tintMaximumChannelDelta;
        public string tintDiagnosticError;
    }

    static readonly ProbeReport report = new ProbeReport();
    static TestRunnerApi api;
    static DateTime started;
    static bool finished;

    public static void Run()
    {
        report.editorVersion = Application.unityVersion;
        report.projectPath = Directory.GetCurrentDirectory();
        if (Application.unityVersion != "6000.4.8f1")
            throw new InvalidOperationException("Exact Unity 6000.4.8f1 required.");

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        DiagnoseTintRoundTrip();

        var scene = default(Scene);
        try
        {
            scene = EditorSceneManager.OpenScene(report.scenePath, OpenSceneMode.Additive);
            foreach (var root in scene.GetRootGameObjects())
            foreach (var item in root.GetComponentsInChildren<Transform>(true))
            {
                var go = item.gameObject;
                report.sceneGameObjects++;
                var missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
                report.missingScriptCount += missing;
                if (missing != 0)
                    report.missingScriptObjects.Add(HierarchyPath(item) + ": " + missing);
                foreach (var component in go.GetComponents<Component>())
                {
                    if (component == null)
                        continue;
                    report.sceneComponents++;
                    try
                    {
                        using (var serialized = new SerializedObject(component))
                        {
                            var property = serialized.GetIterator();
                            while (property.Next(true))
                            {
                                if (property.propertyType == SerializedPropertyType.ObjectReference &&
                                    property.objectReferenceValue == null &&
                                    property.objectReferenceEntityIdValue != EntityId.None)
                                    report.brokenReferences.Add(HierarchyPath(item) + " / " +
                                        component.GetType().Name + "." + property.propertyPath);
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        report.scanErrors.Add(HierarchyPath(item) + " / " +
                            component.GetType().Name + ": " + error.Message);
                    }
                }
            }
        }
        catch (Exception error)
        {
            report.scanErrors.Add(error.ToString());
        }
        finally
        {
            if (scene.IsValid() && scene.isLoaded)
                EditorSceneManager.CloseScene(scene, true); // Never save the scene.
        }

        started = DateTime.UtcNow;
        EditorApplication.update += Watchdog;
        api = ScriptableObject.CreateInstance<TestRunnerApi>();
        try
        {
            api.RetrieveTestList(TestMode.EditMode, tree =>
            {
                Walk(tree);
                Finish();
            });
        }
        catch (Exception error)
        {
            report.discoveryError = error.ToString();
            Finish();
        }
    }

    static void Walk(ITestAdaptor node)
    {
        if (node.IsSuite)
        {
            foreach (var child in node.Children)
                Walk(child);
            return;
        }
        report.totalDiscoveredLeaves++;
        var name = node.FullName ?? string.Empty;
        var relevant = false;
        if (name.StartsWith("PPEPortfolioSequenceTests.", StringComparison.Ordinal))
        { report.sequenceCases++; relevant = true; }
        if (name.StartsWith("PPEGlovePairStateTests.", StringComparison.Ordinal))
        { report.gloveCases++; relevant = true; }
        if (name.StartsWith("ResponseBagPackingTests.", StringComparison.Ordinal))
        { report.bagCases++; relevant = true; }
        if (relevant)
            report.newCases.Add(new CaseRecord
            {
                fullName = name,
                runState = node.RunState.ToString(),
                skipReason = node.SkipReason,
            });
    }

    static void DiagnoseTintRoundTrip()
    {
        GameObject hand = null;
        try
        {
            hand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            PPEInnerGloveHandTint.ApplyToHand(hand, true);
            var propertyBlock = new MaterialPropertyBlock();
            hand.GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            var expected = PPEInnerGloveHandTint.WornHandColor;
            var actual = propertyBlock.GetColor("_BaseColor");
            report.tintExpectedRgba = Channels(expected);
            report.tintActualRgba = Channels(actual);
            report.tintMaximumChannelDelta = Mathf.Max(
                Mathf.Abs(expected.r - actual.r), Mathf.Abs(expected.g - actual.g),
                Mathf.Abs(expected.b - actual.b), Mathf.Abs(expected.a - actual.a));
        }
        catch (Exception error)
        {
            report.tintDiagnosticError = error.ToString();
        }
        finally
        {
            if (hand != null)
                UnityEngine.Object.DestroyImmediate(hand);
        }
    }

    static string[] Channels(Color color)
    {
        return new[] { color.r.ToString("R"), color.g.ToString("R"),
                       color.b.ToString("R"), color.a.ToString("R") };
    }

    static void Watchdog()
    {
        if ((DateTime.UtcNow - started).TotalSeconds < 180)
            return;
        report.discoveryError = "Test list retrieval exceeded 180 seconds.";
        Finish();
    }

    static void Finish()
    {
        if (finished)
            return;
        finished = true;
        EditorApplication.update -= Watchdog;
        report.completedAtUtc = DateTime.UtcNow.ToString("o");
        var output = Environment.GetEnvironmentVariable("PPE_VALIDATION_PROBE_OUTPUT");
        if (string.IsNullOrEmpty(output))
            output = Path.Combine(Directory.GetCurrentDirectory(), "Temp/PpeValidationProbe.json");
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        File.WriteAllText(output, JsonUtility.ToJson(report, true));
        Debug.Log("[PPE Validation Probe] " + output + " NewCases=" + report.newCases.Count +
                  " MissingScripts=" + report.missingScriptCount +
                  " BrokenReferences=" + report.brokenReferences.Count);
        var valid = report.sequenceCases == 13 && report.gloveCases == 17 && report.bagCases == 20 &&
                    report.missingScriptCount == 0 && report.brokenReferences.Count == 0 &&
                    report.scanErrors.Count == 0 && string.IsNullOrEmpty(report.discoveryError);
        EditorApplication.Exit(valid ? 0 : 3);
    }

    static string HierarchyPath(Transform item)
    {
        var path = item.name;
        while (item.parent != null)
        {
            item = item.parent;
            path = item.name + "/" + path;
        }
        return path;
    }
}
#endif
