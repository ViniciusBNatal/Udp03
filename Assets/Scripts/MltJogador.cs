using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MltJogador
{
    public static Dictionary<string, InGameData> Players = new Dictionary<string, InGameData>();
    public static string servidor = "";
    private static string meuIp;
    public static UdpClient udpClient = new UdpClient(11000);
    public static List<string> CurrentIPsRequests = new List<string>();
    public static string CurrentScene;
    public static ClienteIniciar ClientScript;
    public static ServidorIniciar ServerScript;
    public static GameObject PlayerPrefab => Resources.Load<GameObject>("Player");

    public class InGameData
    {
        public DataPackage DataPackage;
        public GameObject PlayerObj;

        public InGameData(DataPackage package, GameObject gameObject)
        {
            DataPackage = package;
            PlayerObj = gameObject;
        }
    }
    public static string ObterMeuIp()
    {
        if (string.IsNullOrEmpty(meuIp))
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                meuIp = endPoint.Address.ToString();
                return meuIp;
            }
        }
        else
        {
            return meuIp;
        }
    }
}
