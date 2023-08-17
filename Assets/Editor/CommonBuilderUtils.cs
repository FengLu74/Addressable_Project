using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace TCG.Editor {
    public static class CommonBuilderUtils {
        public static void CollectFiles(string dirPath, ref List<string> files, string search, string exclude = "") {
            if (!Directory.Exists(dirPath)) {
                return;
            }
            var ds = Directory.GetDirectories(dirPath);
            var fs = Directory.GetFiles(dirPath, search);
            if (fs.Length > 0) {
                if (!string.IsNullOrEmpty(exclude)) {
                    for (var i = fs.Length - 1; i >= 0; i--) {
                        if (string.IsNullOrEmpty(fs[i])) {
                            continue;
                        }
                        if (fs[i].Contains(exclude)) {
                            fs[i] = string.Empty;
                        }
                    }
                }
                foreach (var t in fs) {
                    if (string.IsNullOrEmpty(t)) {
                        continue;
                    }

                    var file = t.Replace("\\", "/");
                    if (!string.IsNullOrEmpty(file) && !files.Contains(file)) {
                        files.Add(file);
                    }
                }
            }

            if (ds.Length <= 0) {
                return;
            }
            foreach (var path in ds) {
                CollectFiles(path.Replace("\\", "/"), ref files, search, exclude);
            }
        }

        public static void CollectFiles(string dirPath, ref List<string> files, List<string> searchList, string exclude = "") {
            if (!Directory.Exists(dirPath)) {
                return;
            }
            var ds = Directory.GetDirectories(dirPath);
            var fs = Directory.GetFiles(dirPath, "*.*");
            if (fs.Length > 0) {
                if (!string.IsNullOrEmpty(exclude)) {
                    for (var i = fs.Length - 1; i >= 0; i--) {
                        if (string.IsNullOrEmpty(fs[i])) {
                            continue;
                        }
                        if (fs[i].Contains(exclude)) {
                            fs[i] = string.Empty;
                        }
                    }
                }
                foreach (var t in fs) {
                    if (string.IsNullOrEmpty(t)) {
                        continue;
                    }

                    var file = t.Replace("\\", "/");
                    if (string.IsNullOrEmpty(file) || files.Contains(file)) {
                        continue;
                    }
                    var ext = Path.GetExtension(file);
                    if (searchList.Find(o =>
                            o.Equals(ext.ToLower(), StringComparison.OrdinalIgnoreCase)) != null) {
                        files.Add(file);
                    }
                }
            }

            if (ds.Length <= 0) {
                return;
            }
            foreach (var path in ds) {
                CollectFiles(path.Replace("\\", "/"), ref files, searchList, exclude);
            }
        }

        public static void FindFile(string path, string searchPattern, List<string> files) {
            path += "/";
            path = path.Replace("\\", "/");
            var dir = new DirectoryInfo(path);
            try {
                foreach (var fi in dir.GetFiles(searchPattern)) {
                    var strPath = fi.ToString();
                    strPath = strPath.Replace("\\", "/");
                    files.Add(strPath);
                }

                foreach (var di in dir.GetDirectories()) {
                    FindFile(di.ToString(), searchPattern, files);
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
            }
        }
    }
}
