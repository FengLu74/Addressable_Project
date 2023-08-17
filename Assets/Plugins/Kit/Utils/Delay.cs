using System;
using UnityEngine;
namespace Kit {
    public class Delay : MonoBehaviour {
        public float delay;
        public Action onAction;

        private float _lastTime;
        private bool _turnOn;
        public bool TurnOn {
            // ReSharper disable once UnusedMember.Global
            get => _turnOn;
            set {
                if (_turnOn == value) {
                    return;
                }
                _turnOn = value;
                if (enabled != value) {
                    enabled = value;
                }
                _lastTime = Time.time;
            }
        }

        private void Update() {
            if (Time.time - _lastTime <= delay) {
                return;
            }
            onAction?.Invoke();
            TurnOn = false;
        }
    }
}
