#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CsvReader
{
    public class CsvPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (Path.GetExtension(assetPath) != ".csv")
                {
                    continue;
                }

                if (CsvConfig.Instance.csvPath.Equals(string.Empty))
                {
                    throw new ArgumentException("Csv path can't be null");
                }

                if (assetPath.IndexOf(CsvConfig.Instance.csvPath, StringComparison.Ordinal) == -1)
                {
                    continue;
                }

                var classInformation = CsvDataController.Instance.GetClassInfo(assetPath) ??
                                       throw new ArgumentNullException(
                                           $"Can't find csv config: Asset Path[{assetPath}]");

                foreach (var csvInfo in classInformation.csvInformations)
                {
                    var collectionType = CsvUtils.GetType(csvInfo.className) ??
                                         throw new ArgumentNullException(
                                             $"Class name is null: Class Name[{csvInfo.className}], Asset Path[{assetPath}]");

                    if (CsvConfig.Instance.scriptableObjectPath.Equals(string.Empty))
                    {
                        throw new ArgumentException("ScriptableObject path can't be null");
                    }

                    if (!csvInfo.usingFolder)
                    {
                        string nameAsset = $"{csvInfo.className}.asset";

                        string assetFile = $"{CsvConfig.Instance.scriptableObjectPath}/{nameAsset}";
                        var gm = AssetDatabase.LoadAssetAtPath(assetFile, collectionType);
                        if (gm == null)
                        {
                            gm = ScriptableObject.CreateInstance(collectionType);
                            AssetDatabase.CreateAsset(gm, assetFile);
                        }

                        if (csvInfo.csvFile != null)
                        {
                            var field = gm.GetType().GetField(csvInfo.fieldSetValue,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                            var dataType = CsvUtils.GetElementTypeFromFieldInfo(field);

                            var result = Reader.Deserialize(dataType, csvInfo.csvFile.text);

                            if (field != null)
                            {
                                field.SetValue(gm, result);

                                if (csvInfo.isConvert)
                                {
                                    var method = gm.GetType().GetMethod(csvInfo.convertMethod);
                                    method?.Invoke(gm, null);
                                    if (field.IsPrivate) field.SetValue(gm, null);
                                }
                            }

                            EditorUtility.SetDirty(gm);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        // Get all files in the same patterns.
                        var directoryInfo = new DirectoryInfo(csvInfo.csvFolderPath);
                        FileInfo[] files = directoryInfo.GetFiles("*.csv");

                        if (csvInfo.separateScriptableObject)
                        {
                            foreach (var file in files)
                            {
                                var relativeFilePath = Path.Combine(csvInfo.csvFolderPath,file.Name);
                                var distinctPart = file.Name.Replace(csvInfo.fileStartWith, "");
                                var convertedFilePath = relativeFilePath.Replace("\\", "/");
                                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(convertedFilePath);

                                if (textAsset)
                                {
                                    string nameAsset = $"{csvInfo.className}_{distinctPart}.asset";
                                    string assetFile = $"{CsvConfig.Instance.scriptableObjectPath}/{nameAsset}";

                                    var gm = AssetDatabase.LoadAssetAtPath(assetFile, collectionType);
                                    if (gm == null)
                                    {
                                        gm = ScriptableObject.CreateInstance(collectionType);
                                        AssetDatabase.CreateAsset(gm, assetFile);
                                    }

                                    var field = gm.GetType().GetField(csvInfo.fieldSetValue,
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                                    var dataType = CsvUtils.GetElementTypeFromFieldInfo(field);

                                    var result = Reader.Deserialize(dataType, textAsset.text);

                                    if (field != null)
                                    {
                                        field.SetValue(gm, result);
                                        if (csvInfo.isConvert)
                                        {
                                            var method = gm.GetType().GetMethod(csvInfo.convertMethod);
                                            method?.Invoke(gm, null);
                                            if (field.IsPrivate) field.SetValue(gm, null);
                                        }
                                    }

                                    EditorUtility.SetDirty(gm);
                                    AssetDatabase.SaveAssets();
                                }
                            }
                        }
                        else
                        {
                            // Get ScriptableObject Info
                            string nameAsset = $"{csvInfo.className}.asset";
                            string assetFile = $"{CsvConfig.Instance.scriptableObjectPath}/{nameAsset}";
                            var gm = AssetDatabase.LoadAssetAtPath(assetFile, collectionType);
                            if (gm == null)
                            {
                                gm = ScriptableObject.CreateInstance(collectionType);
                                AssetDatabase.CreateAsset(gm, assetFile);
                            }

                            var field = gm.GetType().GetField(csvInfo.fieldSetValue,
                                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            var dataType = CsvUtils.GetElementTypeFromFieldInfo(field);
                            // ===================

                            Type genericListType = typeof(List<>).MakeGenericType(dataType);
                            var resultList = (IList)Activator.CreateInstance(genericListType);
                            if (field != null)
                            {
                                foreach (var file in files)
                                {
                                    var relativeFilePath = Path.Combine(csvInfo.csvFolderPath,file.Name);
                                    var convertedFilePath = relativeFilePath.Replace("\\", "/");
                                    var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(convertedFilePath);

                                    if (textAsset)
                                    {
                                        var result = Reader.Deserialize(dataType, textAsset.text) as Array;
                                        foreach (var item in result)
                                            resultList.Add(item);
                                    }
                                }

                                Array resultArray = Array.CreateInstance(dataType, resultList.Count);
                                resultList.CopyTo(resultArray, 0);
                                field.SetValue(gm, resultArray);

                                if (csvInfo.isConvert)
                                {
                                    var method = gm.GetType().GetMethod(csvInfo.convertMethod);
                                    method?.Invoke(gm, null);
                                    if (field.IsPrivate) field.SetValue(gm, null);
                                }
                            }

                            EditorUtility.SetDirty(gm);
                            AssetDatabase.SaveAssets();
                        }
                    }
                }

                Debug.Log("Reimport Asset: " + assetPath);
            }
        }
    }
}
#endif