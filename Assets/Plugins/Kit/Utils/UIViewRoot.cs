using UnityEngine;

namespace Kit
{
    public class UIViewRoot  : MonoBehaviour {
        private static UIViewRoot instance = null;
        private void Start() {
            if (instance != null) {
                Destroy(gameObject);
            }
            else {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}
