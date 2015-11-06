using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildManager
{
    public static void BuildExe()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);

        PlayerSettings.productName = "游戏谷";
        PlayerSettings.bundleVersion = "七雄无双";

        FileUtil.DeleteFileOrDirectory("release/Exe");
        Directory.CreateDirectory("release/Exe");

        var res = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(item=>item.path).ToArray(), "release/Exe/7x.exe", BuildTarget.StandaloneWindows,
            BuildOptions.None);
        if (res.Length > 0)
        {
            throw new Exception("BuildPlayer failure: " + res);
        }

        Debug.Log("Package exe ends.");
    }
}
