using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
namespace TCG.Editor.AssetImporter {
    public class TextureAssetImporter : AssetPostprocessor {
        private const string SpriteAtlasPath = "Assets/Art/2D/SpriteAtlas/";
        private static readonly string[] SetAtlasPathList = {
            "Assets/Art/2D/Atlas",
        };
        private static readonly string[] GenSpriteAtlasPathList = {
            "Assets/Art/2D/Atlas",
        };

        #region OnPreprocessTexture

        public void OnPreprocessTexture() {
            var textureImporter = assetImporter as TextureImporter;
            if (textureImporter == null) {
                return;
            }

            if (assetPath.StartsWith("Assets/Art/2D")) {
                textureImporter.mipmapEnabled = false;
                SetUISpriteTexture(assetPath, textureImporter);
            }
        }

        private static void SetUISpriteTexture(string assetPath, TextureImporter importer) {
            var index = Array.FindIndex(SetAtlasPathList, assetPath.StartsWith);
            if (index != -1) {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;

                var dirName = assetPath.Split('/');
                var tagName = dirName[dirName.Length - 2];
                importer.spritePackingTag = tagName;
                importer.wrapMode = TextureWrapMode.Clamp;
            } else if (assetPath.StartsWith("Assets/Art/2D/UITexture")) {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.wrapMode = assetPath.Contains("Repeat") ? TextureWrapMode.Repeat : TextureWrapMode.Clamp;
                importer.spritePackingTag = string.Empty;
                importer.npotScale = TextureImporterNPOTScale.None;
            } else if (assetPath.StartsWith("Assets/Art/2D/RawTexture") || assetPath.StartsWith("Assets/Art/2D/SpineAnimation")) {
                if (assetPath.Contains("Repeat")) {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.alphaIsTransparency = true;
                    importer.wrapMode = TextureWrapMode.Repeat ;
                    importer.spritePackingTag = string.Empty;
                    importer.npotScale = TextureImporterNPOTScale.None;
                } else {
                    importer.textureType = TextureImporterType.Default;
                    importer.alphaIsTransparency = true;
                    importer.spritePackingTag = string.Empty;
                }
                if (assetPath.StartsWith("Assets/Art/2D/SpineAnimation")) {
                    importer.alphaIsTransparency = false;
                    importer.sRGBTexture = true;
                }
            }
            SetTextureFormat(importer);
        }

        private static void SetTextureFormat(TextureImporter importer) {
            if (importer == null) {
                return;
            }
            StStandaloneTextureFormat(importer);
            SetAndroidTextureFormat(importer);
            SetIosTextureFormat(importer);
        }

        private static void StStandaloneTextureFormat(TextureImporter importer) {
            //var platformSetting = importer.GetPlatformTextureSettings("Standalone");
            //platformSetting.overridden = true;
            //platformSetting.format = TextureImporterFormat.ASTC_4x4;
            //importer.SetPlatformTextureSettings(platformSetting);
        }

        private static void SetAndroidTextureFormat(TextureImporter importer) {
            var platformSetting = importer.GetPlatformTextureSettings("Android");
            SetTextureImporterFormat(platformSetting, importer);
        }

        private static void SetIosTextureFormat(TextureImporter importer) {
            var platformSetting = importer.GetPlatformTextureSettings("iPhone");
            SetTextureImporterFormat(platformSetting, importer);
        }

        private static void SetTextureImporterFormat(TextureImporterPlatformSettings platformSetting, TextureImporter importer) {
            platformSetting.overridden = true;

            var filename = Path.GetFileNameWithoutExtension(importer.assetPath);
            if (filename.EndsWith("_n") || filename.EndsWith("_N")) {
                platformSetting.format = TextureImporterFormat.ASTC_4x4;
            } else {
                if (platformSetting.format < TextureImporterFormat.ASTC_4x4 ||
                    platformSetting.format > TextureImporterFormat.ASTC_12x12) {
                    platformSetting.format = importer.DoesSourceTextureHaveAlpha()
                        ? TextureImporterFormat.ASTC_6x6
                        : TextureImporterFormat.ASTC_6x6;
                }
            }
            importer.SetPlatformTextureSettings(platformSetting);
        }

        #endregion

        public static void OnActionOnPostprocess(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets) {
            foreach (var s in movedAssets) {
                if (s.StartsWith("Assets/Art/2D")) {
                    AssetDatabase.ImportAsset(s);
                }
            }

            var assets = new List<string>();
            if (importedAssets?.Length > 0) {
                assets.AddRange(importedAssets);
                CheckTextureFileValid(importedAssets);
            }
            if (deletedAssets?.Length > 0) {
                assets.AddRange(deletedAssets);
            }
            if (movedAssets.Length > 0) {
                assets.AddRange(movedAssets);
            }
            SetSpriteAtlas(assets);
        }

        private static void SetSpriteAtlas(IEnumerable<string> list) {
            var refreshAtlas = new HashSet<string>();
            foreach (var nm in list) {
                var index = Array.FindIndex(GenSpriteAtlasPathList, p => nm.StartsWith(p));
                if (index == -1 || nm.Contains("SpriteAtlas")) {
                    continue;
                }

                var dirName = nm.Split('/');
                var tagName = dirName[dirName.Length - 2];

                refreshAtlas.Add(tagName);
            }
            if (refreshAtlas.Count == 0) {
                return;
            }

            var spriteAtlasPath = Application.dataPath + "/Art/2D/SpriteAtlas";
            if (!Directory.Exists(spriteAtlasPath)) {
                Directory.CreateDirectory(spriteAtlasPath);
            }

            var atlasList = CollectionAndResetSpriteAtlas(refreshAtlas);

#if UNITY_ANDROID
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.Android);
#elif UNITY_IOS
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.iOS);
#else
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.Android);
#endif
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static List<SpriteAtlas> CollectionAndResetSpriteAtlas(
            IEnumerable<string> refreshAtlas) {
            var atlasList = new List<SpriteAtlas>();
            var searchList = new List<string> { ".png", ".jpg", ".jpeg", ".tga" };
            var fileList = new List<FileInfo>();
            foreach (var atlasName in refreshAtlas) {
                DirectoryInfo dir = null;
                foreach (var atlasPath in GenSpriteAtlasPathList) {
                    var texturePath = atlasPath + "/" + atlasName;
                    dir = new DirectoryInfo(texturePath);
                    if (dir.Exists) {
                        break;
                    }
                }
                if (dir?.Exists != true) {
                    continue;
                }
                var spriteAtlas =
                    CollectionAndResetSpriteAtlasForOne(atlasName, ref fileList, searchList, dir,
                        SpriteAtlasPath);
                if (spriteAtlas != null) {
                    atlasList.Add(spriteAtlas);
                }
            }
            return atlasList;
        }

        private static SpriteAtlas CollectionAndResetSpriteAtlasForOne(string atlasName,
            ref List<FileInfo> fileList, List<string> searchList, DirectoryInfo dir,
            string dstSpriteAtlasPath) {
            var tempFiles = dir.GetFiles("*", SearchOption.AllDirectories);
            fileList.Clear();
            foreach (var file in tempFiles) {
                var ext = Path.GetExtension(file.FullName);
                if (searchList.Find(
                        o => o.Equals(ext.ToLower(), StringComparison.OrdinalIgnoreCase)) != null) {
                    fileList.Add(file);
                }
            }
            var files = fileList.ToArray();
            if (files.Length == 0) {
                return null;
            }

            var spriteAtlas =
                AssetDatabase.LoadAssetAtPath<SpriteAtlas>(dstSpriteAtlasPath + atlasName + ".spriteatlas");
            if (spriteAtlas == null) {
                spriteAtlas = new SpriteAtlas();
                AssetDatabase.CreateAsset(spriteAtlas, dstSpriteAtlasPath + atlasName + ".spriteatlas");
            } else {
                var objs = spriteAtlas.GetPackables();
                spriteAtlas.Remove(objs);
            }
            //spriteAtlas.SetIncludeInBuild(false);

            var packSetting = new SpriteAtlasPackingSettings {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 2
            };
            spriteAtlas.SetPackingSettings(packSetting);

            var textureSetting = new SpriteAtlasTextureSettings {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear
            };
            spriteAtlas.SetTextureSettings(textureSetting);

            var platformSetting = new TextureImporterPlatformSettings {
                maxTextureSize = 2048,
                format = TextureImporterFormat.Automatic,
                crunchedCompression = true,
                textureCompression = TextureImporterCompression.Compressed,
                compressionQuality = 50
            };
            spriteAtlas.SetPlatformSettings(platformSetting);

            var objArr = new Object[files.Length];
            for (var i = 0; i < files.Length; ++i) {
                var file = files[i];
                var assetFilePath =
                    file.FullName.Substring(file.FullName.IndexOf("Asset",
                        StringComparison.Ordinal));
                objArr[i] = AssetDatabase.LoadAssetAtPath<Sprite>(assetFilePath);
                if (objArr[i] == null) {
                    Debug.LogError("sprite 为空 " + assetFilePath);
                }
            }
            spriteAtlas.Add(objArr);

            return spriteAtlas;
        }

        private static void CheckTextureFileValid(IEnumerable<string> assetPaths) {
            var findPsd = false;
            foreach (var path in assetPaths) {
                if (!path.StartsWith("Assets/Art") ||
                    !path.EndsWith(".psd") && !path.EndsWith(".PSD")) {
                    continue;
                }
                Debug.LogError("非法图片：" + path);

                findPsd = true;
                AssetDatabase.DeleteAsset(path);
            }
            if (findPsd) {
                EditorUtility.DisplayDialog("资源错误", "不能导入PSD格式的图片，已删除！", "确定");
            }
        }

        [MenuItem("CustomTools/资源管理/资源检查/重建SpriteAtlas")]
        public static void CheckSpriteSpriteAtlas() {
            var spriteAtlasPath = Application.dataPath + "/Art/2D/SpriteAtlas";
            if (!Directory.Exists(spriteAtlasPath)) {
                Directory.CreateDirectory(spriteAtlasPath);
            }
            var hashAtlas = new HashSet<string>();

            foreach (var atlas in GenSpriteAtlasPathList)
            {
                var atlasPath = Application.dataPath + atlas.Replace("Assets", "");
                var dirs = Directory.GetDirectories(atlasPath);
                foreach (var dir in dirs) {
                    var dirName = dir.Substring(dir.Replace("\\", "/").LastIndexOf("/", StringComparison.Ordinal) + 1);
                    hashAtlas.Add(dirName);
                }
            }


            var atlasList = CollectionAndResetSpriteAtlas(hashAtlas);
            BuildPackAtlases(atlasList);
        }

        private static void BuildPackAtlases(List<SpriteAtlas> atlasList) {

#if UNITY_ANDROID
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.Android);
#elif UNITY_IOS
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.iOS);
#else
            SpriteAtlasUtility.PackAtlases(atlasList.ToArray(), BuildTarget.Android);
#endif
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
