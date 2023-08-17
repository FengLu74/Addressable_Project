using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace Kit {
    public static class AESUtils {
        private static string _key = string.Empty;
        // 加密识别头（用来识别文件是否已经加密过）
        private const string AES_HEAD = "AESEncrypt";

        // ReSharper disable once InvalidXmlDocComment
        /// <summary>
        /// var bytes = Encoding.UTF8.GetBytes("ccg@latte&ss");
        /// var base64 = Convert.ToBase64String(bytes);
        /// bytes = Encoding.UTF8.GetBytes(base64);
        /// foreach (var b in bytes) {
        ///     Debug.Log($"{b:X2}");
        /// }
        /// </summary>
        internal static readonly byte[] RgbIv = {
            0x59, 0x32, 0x4E, 0x6E, 0x51, 0x47, 0x78, 0x68,
            0x64, 0x48, 0x52, 0x6C, 0x4A, 0x6E, 0x4E, 0x7A
        };

        // ReSharper disable once MemberCanBePrivate.Global
        public static void SetKey(string key) {
            if (string.IsNullOrEmpty(key)) {
                return;
            }
            var array = key.ToCharArray();
            var dest = new char[array.Length];
            for (var i = 0; i < array.Length; i++) {
                int x = array[i];
                x += 5;
                dest[array.Length - 1 - i] = (char)x;
            }
            _key = new string(dest);
        }

        // ReSharper disable once UnusedMember.Global
        public static string Decrypt(string encString) {
            var sEncryptedString = encString;

            // ReSharper disable once IdentifierTypo
            var myRijndael = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            };

            var key = Encoding.UTF8.GetBytes(_key);
            var iv = RgbIv; //Convert.FromBase64String(_iv);

            // ReSharper disable once IdentifierTypo
            var decryptor = myRijndael.CreateDecryptor(key, iv);
            var sEncrypted = Convert.FromBase64String(sEncryptedString);
            var fromEncrypt = new byte[sEncrypted.Length];
            var msDecrypt = new MemoryStream(sEncrypted);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return Encoding.UTF8.GetString(fromEncrypt).TrimEnd('\0');
        }

        // ReSharper disable once UnusedMember.Global
        public static byte[] Decrypt(byte[] data) {
            // ReSharper disable once IdentifierTypo
            var myRijndael = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            };

            var key = Encoding.UTF8.GetBytes(_key);
            var iv = RgbIv; //Convert.FromBase64String(_iv);

            // ReSharper disable once IdentifierTypo
            var decryptor = myRijndael.CreateDecryptor(key, iv);
            var fromEncrypt = new byte[data.Length];
            var msDecrypt = new MemoryStream(data);
            var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            return fromEncrypt;
        }

        // ReSharper disable once UnusedMember.Global
        public static string Encrypt(string rawString) {
            var sToEncrypt = rawString;
            var toEncrypt = Encoding.UTF8.GetBytes(sToEncrypt);
            var encrypted = Encrypt(toEncrypt);
            return Convert.ToBase64String(encrypted);
        }

        public static byte[] Encrypt(byte[] toEncrypt) {
            // ReSharper disable once IdentifierTypo
            var myRijndael = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            };

            var key = Encoding.UTF8.GetBytes(_key);
            var iv = RgbIv; //Convert.FromBase64String(_iv);

            // ReSharper disable once IdentifierTypo
            var encryptor = myRijndael.CreateEncryptor(key, iv);
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            return  msEncrypt.ToArray();
        }

        // ReSharper disable once UnusedMember.Global
        public static byte[] Encrypt(MemoryStream ms) {
            if (ms == null) {
                return null;
            }

            // ReSharper disable once IdentifierTypo
            var myRijndael = new RijndaelManaged {
                Padding = PaddingMode.Zeros,
                Mode = CipherMode.CBC,
                KeySize = 128,
                BlockSize = 128
            };

            var key = Encoding.UTF8.GetBytes(_key);
            var iv = RgbIv; //Convert.FromBase64String(_iv);

            // ReSharper disable once IdentifierTypo
            var encryptor = myRijndael.CreateEncryptor(key, iv);
            var msEncrypt = new MemoryStream();
            var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            var toEncrypt = ms.ToArray();
            csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
            csEncrypt.FlushFinalBlock();

            return msEncrypt.ToArray();
        }

        /// <summary>
        /// 文件加密，传入文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="offset">偏移量</param>
        public static void EncryptFileOffset(string path, ulong offset) {
            if (!FileUtils.SafeFileExist(path) || offset == 0) {
                return;
            }
            try {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                    fs.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    var headBuffer = new byte[offset];
                    fs.Write(headBuffer, 0, headBuffer.Length);
                    fs.Write(buffer, 0, buffer.Length);
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 文件加密，传入文件路径
        /// </summary>
        /// <param name="path"></param>
        public static void EncryptFile(string path) {
            if (!FileUtils.SafeFileExist(path)) {
                return;
            }
            try {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                    //读取字节头，判断是否已经加密过了
                    var headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    var headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AES_HEAD) {
#if UNITY_EDITOR
                        Debug.Log(path + "已经加密过了！");
#endif
                        return;
                    }
                    //加密并且写入字节头
                    fs.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.SetLength(0);
                    var headBuffer = Encoding.UTF8.GetBytes(AES_HEAD);
                    fs.Write(headBuffer, 0, headBuffer.Length);
                    var encBuffer = Encrypt(buffer);
                    fs.Write(encBuffer, 0, encBuffer.Length);
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 文件解密，传入文件路径，返回字节,路径不能是streamAssets
        /// </summary>
        /// <returns></returns>
        public static byte[] DecryptFile(string path) {
            if (!FileUtils.SafeFileExist(path)) {
                return null;
            }
            byte[] decBuffer = null;
            try {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    var headBuff = new byte[10];
                    fs.Read(headBuff, 0, headBuff.Length);
                    var headTag = Encoding.UTF8.GetString(headBuff);
                    if (headTag == AES_HEAD) {
                        var buffer = new byte[fs.Length - headBuff.Length];
                        fs.Read(buffer, 0, Convert.ToInt32(fs.Length - headBuff.Length));
                        decBuffer = Decrypt(buffer);
                    }
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }

            return decBuffer;
        }

        /// <summary>
        /// 文件解密，传入文件名，返回字节
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static byte[] DecryptStreamingFile(string fileName) {
            byte[] decBuffer = null;
            try {
                var bytesData = PersistentUtil.LoadPersistentBytesFile(fileName);
                var headBuff = new byte[10];
                Array.Copy(bytesData, headBuff, headBuff.Length);
                var headTag = Encoding.UTF8.GetString(headBuff);
                if (headTag == AES_HEAD) {
                    var buffer = new byte[bytesData.Length - headBuff.Length];
                    Array.Copy(bytesData,headBuff.Length, buffer, 0, buffer.Length);
                    decBuffer = Decrypt(buffer);
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return decBuffer;
        }

        /// <summary>
        /// 文件解密，传入文件路径，返回字节
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] DecryptStreamingFileByFullPath(string filePath) {
            byte[] decBuffer = null;
            try {
                var bytesData = PersistentUtil.LoadPersistentBytesFileByFullPath(filePath);
                var headBuff = new byte[10];
                Array.Copy(bytesData, headBuff, headBuff.Length);
                var headTag = Encoding.UTF8.GetString(headBuff);
                if (headTag == AES_HEAD) {
                    var buffer = new byte[bytesData.Length - headBuff.Length];
                    Array.Copy(bytesData,headBuff.Length, buffer, 0, buffer.Length);
                    decBuffer = Decrypt(buffer);
                }
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return decBuffer;
        }
    }
}
