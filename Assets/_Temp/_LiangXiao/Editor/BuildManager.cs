using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildManager
{
    [MenuItem("Build/Exe")]
    public static void BuildExe()
    {
        Debug.Log("[BUILD] Switch to exe.");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        PlayerSettings.companyName = "游戏谷";
        PlayerSettings.productName = "七雄无双";

        Debug.Log("[BUILD] Clear folder.");
        FileUtil.DeleteFileOrDirectory("release/Exe");
        Directory.CreateDirectory("release/Exe");

        Debug.Log("[BUILD] Start building.");

        var d = DateTime.Now;
        var str = d.Month + d.Day + "_" + d.Hour + d.Minute + ".exe";

        var res = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(item => item.path).ToArray(), "release/Exe/" + str, BuildTarget.StandaloneWindows,
            BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("[BUILD] Build exe ends.");
    }

    [MenuItem("Build/Ios")]
    public static void BuildIos()
    {
        Debug.Log("[BUILD] Switch to iOS.");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);

        PlayerSettings.companyName = "游戏谷";
        PlayerSettings.productName = "七雄无双";

        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_6_0;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;

        Debug.Log("[BUILD] Clear folder.");
        FileUtil.DeleteFileOrDirectory("release/iOS");
        Directory.CreateDirectory("release/iOS");

        Debug.Log("[BUILD] Start building.");
        var res = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(item => item.path).ToArray(), "release/iOS", BuildTarget.iOS,
    BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("[BUILD] Build iOS ends.");
    }

    [MenuItem("Build/Android")]
    public static void BuildAndroid()
    {
        Debug.Log("[BUILD] Switch to Android.");
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);

        PlayerSettings.companyName = "游戏谷";
        PlayerSettings.productName = "七雄无双";

        Debug.Log("[BUILD] Clear folder.");
        FileUtil.DeleteFileOrDirectory("release/Android");
        Directory.CreateDirectory("release/Android");

        Debug.Log("[BUILD] Start building.");

        var d = DateTime.Now;
        var str = d.Month + d.Day + "_" + d.Hour + d.Minute + ".apk";

        var res = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(item => item.path).ToArray(), "release/Android/" + str, BuildTarget.Android,
    BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("[BUILD] Build Android ends.");
    }
}
