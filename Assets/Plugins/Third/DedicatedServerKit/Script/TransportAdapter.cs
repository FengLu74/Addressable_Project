using UnityEngine;
namespace DedicatedServerKit {
    public abstract class TransportAdapter : MonoBehaviour
    {
        /// <summary>
        /// 设置端口
        /// </summary>
        /// <param name="port">The port number.</param>
        public abstract void SetPort(ushort port);
    }
}
