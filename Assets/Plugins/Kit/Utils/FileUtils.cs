using System;
using System.IO;
namespace Kit {
    public static class FileUtils {
        // ReSharper disable once UnusedMember.Global
        public static string GetFileExt(string fileName) {
            var formatFileName = fileName.Replace('\\', '/');
            var fileExt = "";

            var index = formatFileName.LastIndexOf('.');
            if (index > 0) {
                fileExt = formatFileName.Substring(index + 1, formatFileName.Length - index - 1);
            }

            return fileExt;
        }

        // ReSharper disable once UnusedMember.Global
        public static string GetParentFolder(string path) {
            var formatPath = path.Replace('\\', '/');

            if (formatPath.EndsWith("/")) {
                formatPath = formatPath.Substring(0, formatPath.Length - 1);
            }

            var index = formatPath.LastIndexOf('/');
            if (index > 0) {
                formatPath = formatPath.Substring(0, index);
            }

            return formatPath;
        }

        public static string ReadFileTxt(string path) {
            if (!File.Exists(path)) {
                return string.Empty;
            }

            var text = "";
            var sr = File.OpenText(path);
            while (!sr.EndOfStream) {
                text = sr.ReadToEnd();
            }
            sr.Close();

            return text;
        }

        public static void CheckFileAndCreateDirWhenNeeded(string filePath) {
            if (string.IsNullOrEmpty(filePath)) {
                return;
            }

            var fileInfo = new FileInfo(filePath);
            var dirInfo = fileInfo.Directory;
            if (dirInfo?.Exists == false) {
                Directory.CreateDirectory(dirInfo.FullName);
            }
        }

        public static void CheckDirAndCreateWhenNeeded(string folderPath) {
            if (string.IsNullOrEmpty(folderPath)) {
                return;
            }

            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static bool SafeFileExist(string filePath) {
            try {
                return File.Exists(filePath);
            } catch (Exception ex) {
                Log.LogError(
                    $"SafeFileExist failed! path = {filePath} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes) {
            try {
                if (string.IsNullOrEmpty(outFile)) {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile)) {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllBytes(outFile, outBytes);
                return true;
            } catch (Exception ex) {
                Log.LogError(
                    $"SafeWriteAllBytes failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllLines(string outFile, string[] outLines) {
            try {
                if (string.IsNullOrEmpty(outFile)) {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile)) {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllLines(outFile, outLines);
                return true;
            } catch (Exception ex) {
                Log.LogError($"SafeWriteAllLines failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeWriteAllText(string outFile, string text) {
            try {
                if (string.IsNullOrEmpty(outFile)) {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile)) {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllText(outFile, text);
                return true;
            } catch (Exception ex) {
                Log.LogError($"SafeWriteAllText failed! path = {outFile} with err = {ex.Message}");
                return false;
            }
        }

        public static byte[] SafeReadAllBytes(string inFile) {
            try {
                if (string.IsNullOrEmpty(inFile)) {
                    return null;
                }

                if (!File.Exists(inFile)) {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            } catch (Exception ex) {
                Log.LogError($"SafeReadAllBytes failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static string[] SafeReadAllLines(string inFile) {
            try {
                if (string.IsNullOrEmpty(inFile)) {
                    return null;
                }

                if (!File.Exists(inFile)) {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            } catch (Exception ex) {
                Log.LogError($"SafeReadAllLines failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        public static string SafeReadAllText(string inFile) {
            try {
                if (string.IsNullOrEmpty(inFile)) {
                    return null;
                }

                if (!File.Exists(inFile)) {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            } catch (Exception ex) {
                Log.LogError($"SafeReadAllText failed! path = {inFile} with err = {ex.Message}");
                return null;
            }
        }

        private static void DeleteDirectory(string dirPath)
        {
            var files = Directory.GetFiles(dirPath);
            var dirs = Directory.GetDirectories(dirPath);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }

        public static bool SafeClearDir(string folderPath) {
            try {
                if (string.IsNullOrEmpty(folderPath)) {
                    return true;
                }

                if (Directory.Exists(folderPath)) {
                    DeleteDirectory(folderPath);
                }
                Directory.CreateDirectory(folderPath);
                return true;
            } catch (Exception ex) {
                Log.LogError($"SafeClearDir failed! path = {folderPath} with err = {ex.Message}");
                return false;
            }
        }

        public static bool SafeDeleteDir(string folderPath) {
            try {
                if (string.IsNullOrEmpty(folderPath)) {
                    return true;
                }

                if (Directory.Exists(folderPath)) {
                    DeleteDirectory(folderPath);
                }
                return true;
            } catch (Exception ex) {
                Log.LogError($"SafeDeleteDir failed! path = {folderPath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeDeleteFile(string filePath) {
            try {
                if (string.IsNullOrEmpty(filePath)) {
                    return true;
                }

                if (!File.Exists(filePath)) {
                    return true;
                }
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            } catch (Exception ex) {
                Log.LogError($"SafeDeleteFile failed! path = {filePath} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeRenameFile(string sourceFileName, string destFileName) {
            try {
                if (string.IsNullOrEmpty(sourceFileName)) {
                    return false;
                }

                if (!File.Exists(sourceFileName)) {
                    return true;
                }
                SafeDeleteFile(destFileName);
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                File.Move(sourceFileName, destFileName);
                return true;
            } catch (Exception ex) {
                Log.LogError(
                    $"SafeRenameFile failed! path = {sourceFileName} with err: {ex.Message}");
                return false;
            }
        }

        public static bool SafeCopyFile(string fromFile, string toFile) {
            try {
                if (string.IsNullOrEmpty(fromFile)) {
                    return false;
                }

                if (!File.Exists(fromFile)) {
                    return false;
                }
                CheckFileAndCreateDirWhenNeeded(toFile);
                SafeDeleteFile(toFile);
                File.Copy(fromFile, toFile, true);
                return true;
            } catch (Exception ex) {
                Log.LogError(
                    $"SafeCopyFile failed! formFile = {fromFile}, toFile = {toFile}, with err = {ex.Message}");
                return false;
            }
        }
    }
}
