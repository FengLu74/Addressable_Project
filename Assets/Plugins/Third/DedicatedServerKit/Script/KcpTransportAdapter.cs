using UnityEngine;
using kcp2k;

namespace DedicatedServerKit {

    [RequireComponent(typeof(KcpTransport))]
    public class KcpTransportAdapter : TransportAdapter {
        private KcpTransport _transport;

        private void Awake() => _transport = GetComponent<KcpTransport>();

        public override void SetPort(ushort port) => _transport.Port = port;
    }
}
