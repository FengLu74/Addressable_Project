using Sirenix.OdinInspector;
using UnityEngine;
namespace DedicatedServerKit {

    [CreateAssetMenu(menuName = "Dedicated Server Kit/Client Config", fileName = "ClientConfig", order = 0)]
    public class ClientConfig : ScriptableObject {
        [LabelText("连接服务器的地址")]
        public string IpAddress;
        [LabelText("端口")]
        public int Port;
    }
}
