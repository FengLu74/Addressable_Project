using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

namespace Kit {
    public class Ip {
        public enum ADDRESSFAM {
            IPv4,
            IPv6
        }
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="Addfam">要获取的IP类型</param>
        /// <returns></returns>
        public static string GetIP(ADDRESSFAM Addfam) {
            if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6) {
                return null;
            }
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
                NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

                if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) &&
                    item.OperationalStatus == OperationalStatus.Up)
#endif
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties()
                                 .UnicastAddresses) {
                        //IPv4
                        if (Addfam == ADDRESSFAM.IPv4) {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
                                output = ip.Address.ToString();
                                Debug.Log("IPv4 IP:" + output);
                                return output;
                            }
                        }
                        //IPv6
                        else if (Addfam == ADDRESSFAM.IPv6) {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6) {
                                output = ip.Address.ToString();
                                Debug.Log("IPv6 IP:" + output);
                                return output;
                            }
                        }
                    }
                }
            }
            return output;
        }
    }
}
