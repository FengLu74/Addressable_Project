using System;
using System.Collections.Generic;
using Kit;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace TCG.Common {
    // ReSharper disable once ClassNeverInstantiated.Global
    [Serializable]
    public class GameSetting {
        // ReSharper disable once InconsistentNaming
        public bool log2file;
        // ReSharper disable once IdentifierTypo
        public bool openfilter;
    }

    [Serializable]
    public class NetServerList {
        public string name;
        public string host;
        public List<int> port;
        public string entry;
    }
    // ReSharper disable once ClassNeverInstantiated.Global
    [Serializable]
    public class NetSetting {
        public int activeServer;
        public List<NetServerList> serverList;
        public string dataManagerUrl;
        public string hotfixVersionUrl;
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    [Serializable]
    public class VersionSetting {
        public string version;
        public int build;
    }

    [Serializable]
    public class ServerSetting {
        public bool server;
        public string ip;
    }


    public static class Settings {
        private static ServerSetting _serverSetting;
        private static GameSetting _gameSetting;
        private static NetSetting _netSetting;
        private static VersionSetting _versionSetting;
        private static int _develop = -1;
        private static int _server = -1;

        [Serializable]
        private class DevSetting {
            public bool develop;
            public bool server;
        }

        public static bool IsServer() {
            if (_server != -1) {
                return _server == 1;
            }
            var setting = LoadSetting<ServerSetting>("server", false);
            if (setting == null) {
                _server = 0;
            } else {
                _server = setting.server ? 1 : 0;
            }
            return _server == 1;
        }

        public static bool IsDevelop() {
            if (_develop != -1) {
                return _develop == 1;
            }
            var setting = LoadSetting<DevSetting>("dev", false);
            if (setting == null) {
                _develop = 0;
            } else {
                _develop = setting.develop ? 1 : 0;
            }
            return _develop == 1;
        }

        public static ServerSetting GetServerSetting() {
            var isDevelop = IsDevelop();
            return _serverSetting ??= LoadSetting<ServerSetting>("server", !isDevelop);
        }

        public static GameSetting GetGameSetting() {
            var isDevelop = IsDevelop();
            return _gameSetting ??= LoadSetting<GameSetting>("game_config", !isDevelop);
        }
        public static NetSetting GetNetConfigSetting() {
            var isDevelop = IsDevelop();
            return _netSetting ??= LoadSetting<NetSetting>("net_config", !isDevelop);
        }
        public static VersionSetting GetStreamingAssetsVersionSetting() {
            var isDevelop = IsDevelop();
            return LoadSetting<VersionSetting>("version", !isDevelop);
        }
        public static VersionSetting GetPersistentVersionSetting() {
            var isDevelop = IsDevelop();
            return _versionSetting ??= LoadSetting<VersionSetting>("version", !isDevelop, "update");
        }
        // ReSharper disable once UnusedMember.Global
        public static bool SetVersionSetting(int build) {
            if (_versionSetting == null) {
                _versionSetting = GetStreamingAssetsVersionSetting();
            }
            return SetVersionSetting(_versionSetting.version, build);
        }
        public static bool SetVersionSetting(string version) {
            if (_versionSetting == null) {
                _versionSetting = GetStreamingAssetsVersionSetting();
            }
            return SetVersionSetting(version, _versionSetting.build);
        }

        private static bool SetVersionSetting(string version, int build) {
            if (_versionSetting == null) {
                _versionSetting = new VersionSetting();
            }
            _versionSetting.version = version;
            _versionSetting.build = build;
            var isDevelop = IsDevelop();
            return WriteSetting("version", _versionSetting, !isDevelop);
        }

        private static bool WriteSetting<T>(string fileName, T setting, bool decrypt = true) {
            try {
                var content = JsonUtility.ToJson(setting);
                if (decrypt) {
                    content = AESUtils.Encrypt(content);
                }
                SaveUpdateConfig(fileName, content);
                return true;
            } catch (Exception e) {
                Log.LogException(e);
                return false;
            }
        }

        private static T LoadSetting<T>(string fileName, bool decrypt = true, string parentDir = "ccg_config") {
            var content = LoadConfig(fileName, parentDir);
            if (content.IsNullOrEmpty()) {
                return default;
            }

            if (decrypt) {
                content = AESUtils.Decrypt(content);
            }

            T ret = default;
            try {
                ret = JsonUtility.FromJson<T>(content);
            } catch (Exception e) {
                Log.LogException(e);
            }
            return ret;
        }

        private static string LoadConfig(string fileName, string parentDir )
            => PersistentUtil.LoadPersistentTextFile($"{parentDir}/{fileName}.con");

        private static void SaveUpdateConfig(string fileName, string content)
            => PersistentUtil.SavePersistentTextFile($"update/{fileName}.con", content);
        public static void DeleteUpdateConfig(string fileName)
            => PersistentUtil.DeletePersistentTextFile($"update/{fileName}.con");

        public static AddressablesPlatform GetPlatformName() =>
            // ReSharper disable once RedundantAssignment
#pragma warning disable CS0618
            PlatformMappingService.GetPlatform();
#pragma warning restore CS0618

        public static string GetVersion() {
            if (_versionSetting == null) {
                GetPersistentVersionSetting();
            }
            if (_versionSetting == null) {
                _versionSetting = GetStreamingAssetsVersionSetting();
            }
            return _versionSetting.version;
        }

    }
}
