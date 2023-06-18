#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;

namespace ZBase.UnityScreenNavigator.Core
{
    partial class UnityScreenNavigatorSettings
    {
        [MenuItem("Assets/Create/Screen Navigator/Screen Navigator Settings", priority = -1)]
        private static void Create()
        {
            var asset = PlayerSettings.GetPreloadedAssets().OfType<UnityScreenNavigatorSettings>().FirstOrDefault();
            if (asset)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                throw new InvalidOperationException($"{nameof(UnityScreenNavigatorSettings)} already exists at {path}");
            }

            var assetPath = EditorUtility.SaveFilePanelInProject($"Save {nameof(UnityScreenNavigatorSettings)}",
                nameof(UnityScreenNavigatorSettings),
                "asset", "", "Assets");

            if (string.IsNullOrEmpty(assetPath))
            {
                // Return if canceled.
                return;
            }

            // Create folders if needed.
            var folderPath = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var instance = CreateInstance<UnityScreenNavigatorSettings>();
            AssetDatabase.CreateAsset(instance, assetPath);
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Add(instance);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
