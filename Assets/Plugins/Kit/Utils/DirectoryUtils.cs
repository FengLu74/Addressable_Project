using System;
using System.IO;
using UnityEngine;

namespace Kit
{
    public static class DirectoryUtils
    {
        public static string GetOS()
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            string os;
#if UNITY_IPHONE
	        os = "ios";
#elif UNITY_STANDALONE_WIN
            os = "win";
#else
            os = "android";
#endif
            return os;
        }

        // ReSharper disable once UnusedMember.Global
        public static bool PrepareDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(dirPath);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't create directory " + dirPath + ". Exception is " + e);
                // Don't return false at here. Exceptions can be disabled in the unity optimization option.
            }

            return Directory.Exists(dirPath);
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetWWWConfigFilePath(string file)
        {
            if (IsPersistentFileExist("config/" + file))
            {
                return GetBundleFileRootForDownloadWWW() + "config/" + file;
            }

            return GetBundleFileRootForWWW() + "config/" + file;
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetNewWWWConfigFilePath(string file) =>
            GetBundleFileRootForDownloadWWW() + "config/" + file;

        // ReSharper disable once UnusedMember.Global
        public static void DeleteHotUpdateFileDir()
        {
            var dir = GetFilePersistentPath("");
            DeleteFolderEx(dir);
        }

        /// <summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void DeleteFolder(string dir)
        {
            foreach (var d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    var fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }

                    File.Delete(d); //直接删除其中的文件
                }
                else
                {
                    var d1 = new DirectoryInfo(d);

                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName); ////递归删除子文件夹
                    }

                    var dirs = d1.GetDirectories();
                    if (dirs.Length != 0)
                    {
                        foreach (var t in dirs)
                        {
                            DeleteFolder(t.FullName);
                            Directory.Delete(t.FullName);
                        }
                    }

                    Directory.Delete(d);
                }
            }
        }

        /// <summary>
        /// 删除文件夹及其内容
        /// </summary>
        /// <param name="dir"></param>
        // ReSharper disable once MemberCanBePrivate.Global
        public static void DeleteFolderEx(string dir)
        {
            if (Directory.Exists(dir) == false)
            {
                return;
            }

            foreach (var d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    var fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }

                    File.Delete(d); //直接删除其中的文件
                }
                else
                {
                    DeleteFolder(d); ////递归删除子文件夹
                    Directory.Delete(d);
                }
            }

            Directory.Delete(dir);
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetOldWWWConfigFilePath(string file) => GetBundleFileRootForWWW() + "config/" + file;

        // Separate each platform's path at root, to avoid confuse them at editor
        public static string GetFilePersistentPath(string assetPath)
        {
            if (_persistentPath == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _persistentPath = Application.persistentDataPath + "/assetbundle/";
#elif UNITY_ANDROID
				_persistentPath = Application.persistentDataPath + "/assetbundle/";
#elif UNITY_IPHONE
				_persistentPath = Application.persistentDataPath + "/assetbundle/";
#endif
            }

            return _persistentPath + assetPath;
        }

        private static string _persistentPath;

        // Separate each platform's path at root, to avoid confuse them at editor
        // ReSharper disable once UnusedMember.Global
        public static string GetBaseFilePersistentPath(string assetPath)
        {
            if (_persistentPathWithoutAssetBundle == null)
            {
#if UNITY_EDITOR
                _persistentPathWithoutAssetBundle = Application.dataPath + "/../";
#elif UNITY_STANDALONE
	            _persistentPathWithoutAssetBundle = Application.persistentDataPath + "/";
#elif UNITY_ANDROID
				_persistentPathWithoutAssetBundle = Application.persistentDataPath + "/";
#elif UNITY_IPHONE
				_persistentPathWithoutAssetBundle = Application.persistentDataPath + "/";
#endif
            }

            return _persistentPathWithoutAssetBundle + assetPath;
        }

        private static string _persistentPathWithoutAssetBundle;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static string GetFileStreamingAssetsPath(string assetPath)
        {
            if (_streamingAssetsPath == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _streamingAssetsPath = Application.streamingAssetsPath + "/assetbundle/";
#elif UNITY_ANDROID
				_streamingAssetsPath = Application.streamingAssetsPath + "/assetbundle/";
#elif UNITY_IPHONE
				_streamingAssetsPath = Application.streamingAssetsPath + "/assetbundle/";
#endif
            }

            return _streamingAssetsPath + assetPath;
        }

        private static string _streamingAssetsPath;

        // ReSharper disable once UnusedMember.Global
        public static string GetBaseFileStreamingAssetsPath(string assetPath)
        {
            if (_baseStreamingAssetsPath == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _baseStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
				_baseStreamingAssetsPath = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
				_baseStreamingAssetsPath = Application.streamingAssetsPath + "/";
#endif
            }

            return _baseStreamingAssetsPath + assetPath;
        }

        private static string _baseStreamingAssetsPath;

        public static string GetStreamingAssetsRootForWWW()
        {
            if (_streamingAssetsRootForWWW == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _streamingAssetsRootForWWW = "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
				_streamingAssetsRootForWWW = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
				_streamingAssetsRootForWWW = "file://"+Application.streamingAssetsPath+"/";
#endif
            }

            return _streamingAssetsRootForWWW;
        }

        private static string _streamingAssetsRootForWWW;

        /// <summary>
        /// 获取bundle的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetBundleFileRootForWWW()
        {
            if (_bundleRootForWWW == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _bundleRootForWWW = "file://" + Application.streamingAssetsPath + "/assetbundle/";
#elif UNITY_ANDROID
				_bundleRootForWWW = Application.streamingAssetsPath + "/assetbundle/";
#elif UNITY_IPHONE
				_bundleRootForWWW = "file://"+Application.streamingAssetsPath+"/assetbundle/";
#endif
            }

            return _bundleRootForWWW;
        }

        private static string _bundleRootForWWW;

        public static string GetBundleRootForAssetBundleLoader()
        {
            if (_bundleRootForAssetBundleLoader == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _bundleRootForAssetBundleLoader = Application.streamingAssetsPath + "/assetbundle/";
#elif UNITY_ANDROID
				_bundleRootForAssetBundleLoader = Application.dataPath + "!assets/assetbundle/";
#elif UNITY_IPHONE
				_bundleRootForAssetBundleLoader = Application.streamingAssetsPath + "/assetbundle/";
#endif
            }

            return _bundleRootForAssetBundleLoader;
        }

        private static string _bundleRootForAssetBundleLoader;

        /// <summary>
        /// 是否Persistent中存在某个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsPersistentFileExist(string path)
        {
            var fullPath = GetFilePersistentPath(path);
            return File.Exists(fullPath);
        }


        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetBundleFileRootForDownloadWWW()
        {
            if (_bundleRootForDownloadWWW == null)
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                _bundleRootForDownloadWWW = "file://" + Application.persistentDataPath + "/assetbundle/";
#elif UNITY_ANDROID
				_bundleRootForDownloadWWW = "file://" + Application.persistentDataPath + "/assetbundle/";
#elif UNITY_IPHONE
				_bundleRootForDownloadWWW = "file://" + Application.persistentDataPath + "/assetbundle/";
#endif
            }

            return _bundleRootForDownloadWWW;
        }

        private static string _bundleRootForDownloadWWW;


        // ReSharper disable once UnusedMember.Global
        public static void Init()
        {
            // 把所有路径都初始化一遍，在START或者AWAKE中调用，有时候在协程中调用有可能会无法初始化

            GetBundleFileRootForDownloadWWW();

            GetBundleFileRootForWWW();
            GetFileStreamingAssetsPath("");
            GetFilePersistentPath("");
        }
    }
}