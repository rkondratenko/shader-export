using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CreateAssetBundles
{
	[MenuItem ("Assets/Build Shader Bundle")]
	static void BuildAllAssetBundles ()
	{
		// Bring up save panel
		string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
		string directory = Path.GetDirectoryName(path);
		string bundleName = Path.GetFileNameWithoutExtension(path);
		string extension = Path.GetExtension(path);

		var opts = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;

		KeyValuePair<BuildTarget, string>[] platforms = {
			new KeyValuePair<BuildTarget, string>(BuildTarget.StandaloneWindows, "-windows"),
			new KeyValuePair<BuildTarget, string>(BuildTarget.StandaloneOSX, "-macosx"),
			new KeyValuePair<BuildTarget, string>(BuildTarget.StandaloneLinux64, "-linux")
		};
		Object[] selection = Selection.GetFiltered(typeof(Shader), SelectionMode.Assets);
		AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
		buildMap[0].assetNames = selection.Select(x => AssetDatabase.GetAssetPath(x)).ToArray();
		// Stripping platform names from the file name
		foreach (var platform in platforms)
		{
			bundleName = Regex.Replace(bundleName,$"{platform.Value}$","");
		}
		Debug.Log($"Bundle: {bundleName}");
		foreach (var item in buildMap[0].assetNames)
		{
			Debug.Log($"Item: {item}");
		}
		foreach (var platform in platforms)
		{
			buildMap[0].assetBundleName = $"{bundleName}{platform.Value}{extension}";
			BuildPipeline.BuildAssetBundles($"{directory}", buildMap, opts, platform.Key);
		}
	}
}
