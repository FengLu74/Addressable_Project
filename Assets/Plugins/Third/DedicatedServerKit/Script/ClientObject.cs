using System;
using Mirror;
using UnityEngine;
namespace DedicatedServerKit {
    public class ClientObject : MonoBehaviour {
        public ClientConfig Config;
        public NetworkManager NetworkManager;

        private void Awake() {
            var networkManager = FindObjectOfType<NetworkManager>();
            if (networkManager == null) {
                Instantiate(NetworkManager);
            }
        }
    }
}
