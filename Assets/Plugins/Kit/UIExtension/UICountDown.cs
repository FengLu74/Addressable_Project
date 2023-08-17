using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
// ReSharper disable IdentifierTypo
namespace Kit {
    public class UICountDown : MonoBehaviour {

        //用来控制显示几位最大时间
        private enum FormatLimit {
            Dhms,
            Dhm,
            Dh,
            D,
        }

        private enum FormatEnum {
            Edhms,
            Edhm,
            Edh,
            Ed,
            Eh,
            Ehm,
            Ehms,
            Ems,
            Em,
            Es,
        }

        //用冒号还是用文字
        private enum FormatUnitEnum {
            Colon,
            Letter,
        }

        [SerializeField] private Text label;
        [SerializeField]
        [InfoBox("用来控制显示最大几位时间")]
        private FormatLimit formatLimit;
        [SerializeField]
        [ReadOnly]
        private FormatEnum formatEnum;
        [SerializeField]
        [InfoBox("用冒号或多语言")]
        private FormatUnitEnum formatUnitEnum;
        [SerializeField] private string dayLanguageKey = "Time_D";
        [SerializeField] private string hourLanguageKey = "Time_H";
        [SerializeField] private string minLanguageKey = "Time_M";
        [SerializeField] private string secLanguageKey = "Time_S";
        private long _targetTime;
        private long _span;
        private string _format = string.Empty;

        private string _day = "d";
        private string _hour = "h";
        private string _min = "m";
        private string _sec = "s";

        public Action actionCountDownEnd;
        public Func<string, string> actionLanguage;

        public long TargetTime {
            set {
                if (value <= GetTimestampToSeconds()) {
                    OnFinish();
                } else {
                    SetLanguage();
                    _span = 0;
                    _targetTime = value;
                    enabled = true;
                }
            }
        }

        private void Awake() {
            if (label == null) {
                label = GetComponent<Text>();
            }
            ResetFormatStr();
            if (label != null) {
                label.text = GetFormat(0);
            }
            enabled = false;
        }

#if UNITY_EDITOR
        private void OnValidate() {
            if (Application.isPlaying) {
                ResetFormatStr();
            }
        }
#endif

        private void ResetFormatStr() {
            switch (formatEnum) {
            case FormatEnum.Edhms:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}:{2}:{3}",
                    FormatUnitEnum.Letter => $"{{0}}{_day}{{1}}{_hour}{{2}}{_min}{{3}}{_sec}",
                    _ => _format
                };
                break;
            case FormatEnum.Edhm:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}:{2}",
                    FormatUnitEnum.Letter => $"{{0}}{_day}{{1}}{_hour}{{2}}{_min}",
                    _ => _format
                };
                break;
            case FormatEnum.Edh:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}",
                    FormatUnitEnum.Letter => $"{{0}}{_day}{{1}}{_hour}",
                    _ => _format
                };
                break;
            case FormatEnum.Ed:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}",
                    FormatUnitEnum.Letter => $"{{0}}{_day}",
                    _ => _format
                };
                break;
            case FormatEnum.Eh:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}",
                    FormatUnitEnum.Letter => $"{{0}}{_hour}",
                    _ => _format
                };
                break;
            case FormatEnum.Ehm:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}",
                    FormatUnitEnum.Letter => $"{{0}}{_hour}{{1}}{_min}",
                    _ => _format
                };
                break;
            case FormatEnum.Ehms:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}:{2}",
                    FormatUnitEnum.Letter => $"{{0}}{_hour}{{1}}{_min}{{2}}{_sec}",
                    _ => _format
                };
                break;
            case FormatEnum.Ems:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}:{1}",
                    FormatUnitEnum.Letter => $"{{0}}{_min}{{1}}{_sec}",
                    _ => _format
                };
                break;
            case FormatEnum.Em:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}",
                    FormatUnitEnum.Letter => $"{{0}}{_min}",
                    _ => _format
                };
                break;
            case FormatEnum.Es:
                _format = formatUnitEnum switch {
                    FormatUnitEnum.Colon => "{0}",
                    FormatUnitEnum.Letter => $"{{0}}{_sec}",
                    _ => _format
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeLimit(TimeSpan ts) {
            switch (formatLimit) {
                case FormatLimit.Dhms:
                    if (ts.Days > 0) {
                        formatEnum = FormatEnum.Edhms;
                    }
                    else if (ts.Hours > 0) {
                        formatEnum = FormatEnum.Ehms;
                    }
                    else if (ts.Minutes > 0) {
                        formatEnum = FormatEnum.Ems;
                    } else {
                        formatEnum = FormatEnum.Es;
                    }
                    break;
                case FormatLimit.Dhm:
                    if (ts.Days > 0) {
                        formatEnum = FormatEnum.Edhm;
                    }
                    else if (ts.Hours > 0) {
                        formatEnum = FormatEnum.Ehms;
                    }
                    else if (ts.Minutes > 0) {
                        formatEnum = FormatEnum.Ems;
                    } else {
                        formatEnum = FormatEnum.Es;
                    }
                    break;
                case FormatLimit.Dh:
                    if (ts.Days > 0) {
                        formatEnum = FormatEnum.Edh;
                    }
                    else if (ts.Hours > 0) {
                        formatEnum = FormatEnum.Ehm;
                    }
                    else if (ts.Minutes > 0) {
                        formatEnum = FormatEnum.Ems;
                    } else {
                        formatEnum = FormatEnum.Es;
                    }
                    break;
                case FormatLimit.D:
                    if (ts.Days > 0) {
                        formatEnum = FormatEnum.Ed;
                    }
                    else if (ts.Hours > 0) {
                        formatEnum = FormatEnum.Eh;
                    }
                    else if (ts.Minutes > 0) {
                        formatEnum = FormatEnum.Em;
                    } else {
                        formatEnum = FormatEnum.Es;
                    }
                    break;
            }
        }

        private void ChangeFormat(TimeSpan ts) {
            ChangeLimit(ts);
            ResetFormatStr();
        }

        private void SetLanguage() {
            if (actionLanguage == null) {
                return;
            }
            _day = SetLanguage("d", dayLanguageKey);
            _hour = SetLanguage("h", hourLanguageKey);
            _min = SetLanguage("m", minLanguageKey);
            _sec = SetLanguage("s", secLanguageKey);
        }

        private string SetLanguage(string defaultValue, string languageKey) {
            if (string.IsNullOrEmpty(languageKey)) {
                return defaultValue;
            }
            var str = actionLanguage(languageKey);
            return !string.IsNullOrEmpty(str) ? str : defaultValue;
        }

        private string GetFormat(long t) {
            var ts = TimeSpan.FromSeconds(t);
            ChangeFormat(ts);
            string str;
            switch (formatEnum) {
            case FormatEnum.Edhms:
                str = string.Format(_format, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
                break;
            case FormatEnum.Edhm:
                str = string.Format(_format, ts.Days, ts.Hours, ts.Minutes);
                break;
            case FormatEnum.Edh:
                str = string.Format(_format, ts.Days, ts.Hours);
                break;
            case FormatEnum.Ed:
                str = string.Format(_format, ts.Days);
                break;
            case FormatEnum.Eh:
                str = string.Format(_format, ts.Hours + ts.Days * 24);
                break;
            case FormatEnum.Ehm:
                str = string.Format(_format, ts.Hours + ts.Days * 24, ts.Minutes);
                break;
            case FormatEnum.Ehms:
                str = string.Format(_format, ts.Hours + ts.Days * 24, ts.Minutes, ts.Seconds);
                break;
            case FormatEnum.Ems:
                str = string.Format(_format, ts.Minutes, ts.Seconds);
                break;
            case FormatEnum.Em:
                str = string.Format(_format, ts.Minutes);
                break;
            case FormatEnum.Es:
                str = string.Format(_format, ts.Seconds);
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }
            str = str.TrimStart(' ').TrimEnd(' ');
            return str;
        }

        private void Update() {
            if (_targetTime <= GetTimestampToSeconds()) {
                OnFinish();
                return;
            }

            var sp = _targetTime - GetTimestampToSeconds();
            if (sp == _span) {
                return;
            }
            label.text = GetFormat(sp);
            _span = sp;

        }

        private void OnFinish() {
            if (label.IsNotNull()) {
                label.text = GetFormat(0);
            }
            enabled = false;

            actionCountDownEnd?.Invoke();
        }

        public void Release() => enabled = false;

        //不安全的时间，需要拿服务器时间
        private static long GetTimestampToSeconds() {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}
