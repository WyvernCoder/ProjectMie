using UnityEditor;
using System.IO;
using NUnit.Framework;

public class AB_Builder
{
    static string dir = "AssetBundles";

    [MenuItem("Assets/AB BuildTool/For Windows")]
    static void BuildAllAssetBundles_Win()
    {
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_Windows" + Path.GetExtension(file.Name)));
        }
        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/For Android")]
    static void BuildAllAssetBundles_Android()
    {
        string dir = "AssetBundles";
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_Android" + Path.GetExtension(file.Name)));
        }
        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/For iOS")]
    static void BuildAllAssetBundles_iOS()
    {
        string dir = "AssetBundles";
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_iOS" + Path.GetExtension(file.Name)));
        }
        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/For WebGL")]
    static void BuildAllAssetBundles_WebGL()
    {
        string dir = "AssetBundles";
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_WebGl" + Path.GetExtension(file.Name)));
        }
        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/For Mobile")]
    static void BuildAllAssetBundles_Mobile()
    {
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_Android" + Path.GetExtension(file.Name)));
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            if (file.Name.Contains("_Android")) continue;
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_iOS" + Path.GetExtension(file.Name)));
        }

        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/For Windows And Android")]
    static void BuildAllAssetBundles_WA()
    {
        CleanDir();
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_Windows" + Path.GetExtension(file.Name)));
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            if (file.Name.Contains("_Windows")) continue;
            file.MoveTo(Path.Combine(dir, Path.GetFileNameWithoutExtension(file.Name) + "_Android" + Path.GetExtension(file.Name)));
        }

        CleanJunk();
    }

    [MenuItem("Assets/AB BuildTool/Clear ALL AB Name")]
    static void ClearAllAssetBundleName()
    {
        foreach (var item in AssetDatabase.GetAllAssetBundleNames()) AssetDatabase.RemoveAssetBundleName(item, true);
    }

    static void CleanDir()
    {
        if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles()) file.Delete();
    }

    static void CleanJunk()
    {
        if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
        foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
        {
            if (file.Name.Contains("manifest") || file.Name.Contains("AssetBundles")) file.Delete();
        }
    }
}
