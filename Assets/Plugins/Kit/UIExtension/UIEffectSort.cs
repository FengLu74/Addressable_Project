using UnityEngine;
namespace Kit {
    public class UIEffectSort : MonoBehaviour {
#pragma warning disable 649
        [SerializeField] private int orderAdditive;
#pragma warning restore 649
        private Canvas _canvas;
        private Renderer[] _renderers;

        private bool _initRenderer;
        private bool _initCanvas;
        //private int _oldBaseOrder;

#if UNITY_EDITOR
        private void OnValidate() => SetOrder();
#endif
        // ReSharper disable once MemberCanBePrivate.Global
        public void Init() {
            if (!_initCanvas) {
                _initCanvas = true;

                _canvas = GetComponentInParent<Canvas>();
                if (_canvas == null) {
                    enabled = false;
                    return;
                }
            }

            // ReSharper disable once InvertIf
            if (!_initRenderer) {
                _initRenderer = true;
                _renderers = GetComponentsInChildren<Renderer>(true);
            }
        }

        public void SetOrder() {
            Init();

            if (_canvas == null) {
                return;
            }

            var newBaseOrder = _canvas.sortingOrder + orderAdditive;
            foreach (var rd in _renderers) {
                rd.sortingOrder = newBaseOrder;// + (rd.sortingOrder - _oldBaseOrder);
            }
            //_oldBaseOrder = newBaseOrder;
        }

        // ReSharper disable once UnusedMember.Global
        public void SetOrder(Canvas canvas) {
            _canvas = canvas;
            _initCanvas = true;
            SetOrder();
        }

        public void SetOrder(int additive) {
            orderAdditive = additive;
            SetOrder();
        }

    }
}
