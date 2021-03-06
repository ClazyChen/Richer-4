﻿using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using System.Text;

public class PNetworkConfig {
    /// <summary>
    /// 通信监听线程的交互间隔时间
    /// </summary>
    public const int ListenerInterval = 10;
    /// <summary>
    /// TCP通信时用于句首标记
    /// </summary>
    public const char MessageStartFlag = '^';
    /// <summary>
    /// TCP通信时用于句末标记
    /// </summary>
    public const char MessageEndFlag = '$';
    /// <summary>
    /// TCP通信时每次收发包的大小
    /// </summary>
    public const int MaxBufferSizeNetwork = 1024;
    /// <summary>
    /// 最大允许的昵称长度
    /// </summary>
    public const int MaxNicknameLength = 8;

    /// <summary>
    /// 服务器用于通信的端口号
    /// </summary>
    public const int ServerPort = 9012;

    /// <summary>
    /// 默认网络传输延迟
    /// </summary>
    public const float DefaultNetworkDeltaTime = 0.05f;

    private static string GetIPAddressList() {
        string IPv4Address = string.Empty;
        try {
            string hostName = Dns.GetHostName();
            IPAddress[] addressArray = Dns.GetHostAddresses(hostName);
            foreach (IPAddress address in addressArray) {
                if (address.AddressFamily == AddressFamily.InterNetwork) {
                    if (!IPv4Address.Equals(string.Empty)) {
                        IPv4Address += ";";
                    }
                    IPv4Address += address.ToString();
                }
            }
        } catch (Exception e) {
            PLogger.Log("获取IP失败");
            PLogger.Log(e.ToString());
        }
        return IPv4Address;
    }

    /// <summary>
    /// 当前主机的IPv4地址
    /// </summary>
    public static IPAddress IP {
        get {
            if (PUIManager.GetUI<PChooseMapUI>().IPAddressInputField.text.Length > 0) {
                try {
                    IPAddress Ans = IPAddress.Parse(PUIManager.GetUI<PChooseMapUI>().IPAddressInputField.text);
                    return Ans;
                } catch (Exception) {
                    // 输入不符合规定，改为自动填入
                }
            }
            string addressList = GetIPAddressList();
            if (addressList.Contains(";")) {
                string[] addresses = addressList.Split(';');
                return IPAddress.Parse(addresses[addresses.Length - 1]);
            }
            return IPAddress.Parse(addressList);
        }
    }

    /// <summary>
    /// 默认的昵称
    /// </summary>
    public static string DefaultNickname {
        get {
            return PSystem.UserManager.Nickname;
        }
    }
}