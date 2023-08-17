using UnityEngine;

namespace Kit
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Start() => DontDestroyOnLoad(gameObject);
    }
}