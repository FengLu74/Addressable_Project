using UnityEngine;
using UnityEngine.Networking;
using System.IO;
namespace Kit {
    public class PersistentUtil {
        #region Persistent

        /// <summary>
        ///
        /// </summary>
        /// <param name="filePath">完整路径</param>
        /// <param name="showError"></param>
        /// <returns></returns>
        public static byte[] LoadConfigDataFileByFullPath(string filePath, bool showError = true) {
            var www = UnityWebRequest.Get(filePath);
            www.SendWebRequest();
#if UNITY_IOS
		    System.Threading.Thread.Sleep (100);
#endif
            while (!www.isDone)
            {
                System.Threading.Thread.Sleep(0);
            }

            if (www.error != null)
            {
                if (showError) {
                    Debug.LogError(www.error);
                }
                return null;
            }
            // var bytes = www.bytes;
            var bytes = www.downloadHandler.data;
            var wwwLen = bytes.Length;
            if (wwwLen <= 0)
            {
                return null;
            }

            var buffer = new byte[wwwLen];
            bytes.CopyTo(buffer, 0);
            www.Dispose();
            return buffer;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static byte[] LoadConfigDataFile(string fileName, bool showError = true)
        {
            string path;
            if (Application.isEditor) {
                path = Application.streamingAssetsPath + "/" + fileName;
                return FileUtils.SafeReadAllBytes(path);
            }
            path = DirectoryUtils.GetStreamingAssetsRootForWWW() + fileName;
            return LoadConfigDataFileByFullPath(path, showError);
        }

        public static byte[] LoadPersistentBytesFileByFullPath(string filePath) {
            byte[] bytesData;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!FileUtils.SafeFileExist(filePath)) {
                bytesData = LoadConfigDataFileByFullPath(filePath, false); //try load at streamingAssets
            } else {
                bytesData = FileUtils.SafeReadAllBytes(filePath);
            }
            return bytesData;
        }

        public static byte[] LoadPersistentBytesFile(string fileName) {
            var filePath = DirectoryUtils.GetBaseFilePersistentPath(fileName);
            byte[] bytesData;
            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (!FileUtils.SafeFileExist(filePath)) {
                bytesData = LoadConfigDataFile(fileName, false); //try load at streamingAssets
            } else {
                bytesData = FileUtils.SafeReadAllBytes(filePath);
            }
            return bytesData;
        }

        public static string LoadPersistentTextFile(string filename) {
            var bytesData = LoadPersistentBytesFile(filename);
            if (bytesData == null) {
                return string.Empty;
            }
            var content = System.Text.Encoding.UTF8.GetString(bytesData);
            return content;
        }

        public static void SavePersistentTextFile(string filename, string content) {
            var filePath = DirectoryUtils.GetBaseFilePersistentPath(filename);
            var directPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directPath)) {
                if (!DirectoryUtils.PrepareDirectory(directPath)) {
                    Debug.LogError("Can not create folder: " + directPath);
                    return;
                }
            }
            var bytes = System.Text.Encoding.UTF8.GetBytes(content);

            FileUtils.SafeDeleteFile(filePath);
            FileUtils.SafeWriteAllBytes(filePath, bytes);
        }

        public static void DeletePersistentTextFile(string filename) {
            var filePath = DirectoryUtils.GetBaseFilePersistentPath(filename);
            var directPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directPath)) {
                return;
            }
            FileUtils.SafeDeleteFile(filePath);
        }

        #endregion
    }
}
