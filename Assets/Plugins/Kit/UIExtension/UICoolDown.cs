using System;
using UnityEngine;
using UnityEngine.UI;
namespace Kit {

    public class UICoolDown : MonoBehaviour {
        [SerializeField] private Image imgCoolDown;
        [SerializeField] private Text textCoolDown;
        private float _coolDownTime;
        private float _curTime;
        private float _refreshTime;

        public Action ActionCoolDownEnd;
        public Action OnTickPerSec;

        public bool Pause { get; set; }
        public bool End => _curTime > _coolDownTime;

        // ReSharper disable once MemberCanBePrivate.Global
        public float CoolDownTime {
            set {
                _curTime = 0f;
                _refreshTime = 0f;
                _coolDownTime = value;
                if (_coolDownTime > 0f) {
                    enabled = true;
                }

                if (imgCoolDown == null && textCoolDown == null) {
                    enabled = false;
                } else {
                    if (imgCoolDown != null) {
                        if (!imgCoolDown.isActiveAndEnabled) {
                            imgCoolDown.enabled = true;
                        }
                        imgCoolDown.fillAmount = 1f;
                    }
                    if (textCoolDown != null) {
                        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                        textCoolDown.text = _coolDownTime.ToString();
                    }
                }
            }
        }

        private void Awake() {
            if (imgCoolDown == null) {
                imgCoolDown = GetComponent<Image>();
            }
            if (imgCoolDown != null) {
                imgCoolDown.fillAmount = 1f;
            }

            enabled = false;
        }

        private void Update() {
            if (Pause || End) {
                return;
            }
            _curTime += Time.deltaTime;
            if (_coolDownTime <= _curTime) {
                OnFinish();
                return;
            }
            var fill = _curTime / _coolDownTime;
            if (imgCoolDown != null) {
                imgCoolDown.fillAmount = 1f - Mathf.Clamp01(fill);
            }
            if (textCoolDown == null) {
                return;
            }
            if (!(_curTime - _refreshTime >= 1)) {
                return;
            }
            textCoolDown.text = Mathf.RoundToInt(_coolDownTime - _curTime).ToString();
            _refreshTime = _curTime;
            OnTickPerSec?.Invoke();
        }

        private void OnFinish() {
            if (imgCoolDown != null) {
                if (imgCoolDown.isActiveAndEnabled) {
                    imgCoolDown.enabled = false;
                }
                imgCoolDown.fillAmount = 0f;
            }
            if (textCoolDown != null) {
                textCoolDown.text = "0";
            }

            _curTime = 0f;
            enabled = false;
            _coolDownTime = 0f;
            _refreshTime = 0f;
            Pause = false;
            OnTickPerSec = null;
            ActionCoolDownEnd?.Invoke();
        }
    }
}
