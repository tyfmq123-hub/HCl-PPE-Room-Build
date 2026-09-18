#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using HClTraining.App;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEngine;

public static class PpePortfolioDemoBuildProfileSetup
{
    public const string ProfileName = "PPE Portfolio Demo Win64";
    public const string ProfileAssetPath = "Assets/Settings/Build Profiles/PPE Portfolio Demo Win64.asset";
    public const string PlayerOutputPath = "Builds/PPE-Portfolio-Demo-Win64/HClTrainingPPEDemo.exe";
    const string BatchIntentPath = "Logs/ppe-demo-batch-intent.txt";

    static readonly string[] DemoSceneOrder =
    {
        SceneFlowController.PpeScenePath,
        SceneFlowController.AppScenePath,
        SceneFlowController.TitleScenePath,
        SceneFlowController.StartScenePath,
        SceneFlowController.PreEntryScenePath,
        SceneFlowController.LeakResponseScenePath,
        SceneFlowController.CompleteScenePath,
    };

    [MenuItem("Tools/App/Create PPE Portfolio Demo Build Profile")]
    public static void CreateOrUpdateFromMenu()
    {
        WriteBatchIntent(false, false);
        CreateOrUpdate();
    }

    public static void CreateOrUpdateFromBatch()
    {
        WriteBatchIntent(true, false);
        CreateOrUpdate();
    }

    public static void CreateProfileAndBuildPlayerFromBatch()
    {
        WriteBatchIntent(true, true);
        CreateOrUpdate();
    }

    static void WriteBatchIntent(bool exitWhenDone, bool buildPlayer)
    {
        Directory.CreateDirectory("Logs");
        File.WriteAllText(BatchIntentPath, (exitWhenDone ? "exit" : "keep") + " " + (buildPlayer ? "build" : "profile"));
    }

    static void ReadBatchIntent(out bool exitWhenDone, out bool buildPlayer)
    {
        exitWhenDone = false;
        buildPlayer = false;
        if (!File.Exists(BatchIntentPath))
            return;
        var text = File.ReadAllText(BatchIntentPath);
        exitWhenDone = text.IndexOf("exit", StringComparison.OrdinalIgnoreCase) >= 0;
        buildPlayer = text.IndexOf("build", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static void CreateOrUpdate()
    {
        if (!VerifyClassicBuildSettings())
        {
            Finish(1);
            return;
        }

        var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(ProfileAssetPath);
        if (profile == null)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:BuildProfile"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var candidate = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
                if (candidate != null && candidate.name == ProfileName)
                {
                    profile = candidate;
                    break;
                }
            }
        }

        if (profile == null)
        {
            Directory.CreateDirectory("Assets/Settings/Build Profiles");
            profile = ScriptableObject.CreateInstance<BuildProfile>();
            AssetDatabase.CreateAsset(profile, ProfileAssetPath);
            ConfigureWindowsPlatform(profile);
            Debug.Log("[PPE Demo Profile] Created " + ProfileAssetPath);
        }

        ApplyDemoSettings(profile);
        FinishAfterProfileReady(profile);
    }

    static void ConfigureWindowsPlatform(BuildProfile profile)
    {
        var so = new SerializedObject(profile);
        var logged = false;
        var iterator = so.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (!logged)
            {
                logged = true;
                Debug.Log("[PPE Demo Profile] Serialized properties start at " + iterator.propertyPath);
            }

            switch (iterator.propertyPath)
            {
                case "m_BuildTarget":
                case "buildTarget":
                    iterator.intValue = (int)BuildTarget.StandaloneWindows64;
                    break;
                case "m_Subtarget":
                case "subtarget":
                    iterator.intValue = (int)StandaloneBuildSubtarget.Player;
                    break;
                case "m_PlatformGuid":
                case "m_PlatformId":
                case "platformGuid":
                case "platformId":
                    if (iterator.propertyType == SerializedPropertyType.Hash128 ||
                        iterator.type == "GUID")
                    {
                        var guid = GetWindowsStandalonePlatformId();
                        TryAssignGuidProperty(iterator, guid);
                    }
                    break;
            }
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(profile);
    }

    static void TryAssignGuidProperty(SerializedProperty property, GUID guid)
    {
        var guidString = guid.ToString();
        if (property.FindPropertyRelative("m_Value") != null)
            property.FindPropertyRelative("m_Value").stringValue = guidString;
        else if (property.propertyType == SerializedPropertyType.String)
            property.stringValue = guidString;
        else
            Debug.LogWarning("[PPE Demo Profile] GUID property '" + property.propertyPath + "' type=" + property.type);
    }

    static void ApplyDemoSettings(BuildProfile profile)
    {
        profile.overrideGlobalScenes = true;
        var scenes = new EditorBuildSettingsScene[DemoSceneOrder.Length];
        for (var i = 0; i < DemoSceneOrder.Length; i++)
            scenes[i] = new EditorBuildSettingsScene(DemoSceneOrder[i], true);
        profile.scenes = scenes;
        profile.scriptingDefines = new[] { PpePortfolioDemo.ScriptingDefine };
        var so = new SerializedObject(profile);
        var hasDefines = so.FindProperty("m_HasScriptingDefines");
        if (hasDefines != null)
            hasDefines.boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[PPE Demo Profile] Applied at " + AssetDatabase.GetAssetPath(profile) +
                  " firstScene=" + DemoSceneOrder[0] +
                  " define=" + PpePortfolioDemo.ScriptingDefine);
    }

    static bool VerifyClassicBuildSettings()
    {
        if (EditorBuildSettings.scenes.Length == 0 ||
            EditorBuildSettings.scenes[0].path != SceneFlowController.AppScenePath ||
            !EditorBuildSettings.scenes[0].enabled)
        {
            Debug.LogError("[PPE Demo Profile] Classic EditorBuildSettings no longer starts at 01_App.");
            return false;
        }

        return true;
    }

    static void FinishAfterProfileReady(BuildProfile profile)
    {
        if (!VerifyClassicBuildSettings())
        {
            Finish(1);
            return;
        }

        ReadBatchIntent(out _, out var buildPlayerWhenReady);
        if (!buildPlayerWhenReady)
        {
            Finish(0);
            return;
        }

        try
        {
            var output = Path.GetFullPath(PlayerOutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(output) ?? "Builds");
            var report = BuildWithProfile(profile, output);
            if (report == null)
            {
                Debug.LogError("[PPE Demo Profile] BuildPipeline returned no report.");
                Finish(1);
                return;
            }

            Debug.Log("[PPE Demo Profile] Player build result=" + report.summary.result + " output=" + output);
            Finish(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
        }
        catch (Exception exception)
        {
            Debug.LogError("[PPE Demo Profile] Player build failed: " + exception);
            Finish(1);
        }
    }

    static UnityEditor.Build.Reporting.BuildReport BuildWithProfile(BuildProfile profile, string outputPath)
    {
        BuildProfile.SetActiveBuildProfile(profile);
        var optionsType = typeof(BuildPipeline).Assembly.GetType("UnityEditor.BuildPlayerWithProfileOptions");
        if (optionsType != null)
        {
            var options = Activator.CreateInstance(optionsType);
            SetMember(optionsType, options, "buildProfile", profile);
            SetMember(optionsType, options, "locationPathName", outputPath);
            SetMember(optionsType, options, "options", BuildOptions.None);
            var method = typeof(BuildPipeline).GetMethod("BuildPlayer", new[] { optionsType });
            if (method != null)
                return (UnityEditor.Build.Reporting.BuildReport)method.Invoke(null, new[] { options });
        }

        BuildProfile.SetActiveBuildProfile(profile);
        return BuildPipeline.BuildPlayer(DemoSceneOrder, outputPath, BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    static void SetMember(Type type, object instance, string name, object value)
    {
        var field = type.GetField(name, BindingFlags.Public | BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(instance, value);
            return;
        }

        var property = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(instance, value);
    }

    static GUID GetWindowsStandalonePlatformId()
    {
        var discovery = typeof(BuildPipeline).Assembly.GetType("UnityEditor.BuildTargetDiscovery");
        var fromTarget = discovery?.GetMethod(
            "GetGUIDFromBuildTarget",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(BuildTarget) },
            null);
        if (fromTarget != null)
            return (GUID)fromTarget.Invoke(null, new object[] { BuildTarget.StandaloneWindows64 });

        var named = discovery?.GetMethod(
            "GetGUIDFromBuildTarget",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(NamedBuildTarget), typeof(BuildTarget) },
            null);
        if (named != null)
            return (GUID)named.Invoke(null, new object[] { NamedBuildTarget.Standalone, BuildTarget.StandaloneWindows64 });

        var util = typeof(BuildProfile).Assembly.GetType("UnityEditor.Build.Profile.BuildProfileModuleUtil");
        var getId = util?.GetMethod(
            "GetPlatformId",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            null,
            new[] { typeof(BuildTarget), typeof(StandaloneBuildSubtarget) },
            null);
        if (getId != null)
            return (GUID)getId.Invoke(null, new object[] { BuildTarget.StandaloneWindows64, StandaloneBuildSubtarget.Player });

        throw new InvalidOperationException("Windows Standalone platform GUID를 찾지 못했습니다.");
    }

    static void Finish(int exitCode)
    {
        ReadBatchIntent(out var exitEditorWhenDone, out _);
        if (File.Exists(BatchIntentPath))
            File.Delete(BatchIntentPath);
        if (!exitEditorWhenDone)
            return;
        EditorApplication.Exit(exitCode);
    }
}
#endif
