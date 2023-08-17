using Mirror;
using UnityEngine;
namespace DedicatedServerKit {
    [RequireComponent(typeof(TelepathyTransport))]
    public class TelepathyTransportAdapter : TransportAdapter {
        private TelepathyTransport _transport;

        private void Awake() => _transport = GetComponent<TelepathyTransport>();

        public override void SetPort(ushort port) => _transport.port = port;
    }
}
