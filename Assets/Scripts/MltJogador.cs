using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MltJogador
{
    public static Dictionary<string, DataPackage> clientes = new Dictionary<string, DataPackage>();
    public static string servidor = "";
    public static string meuIp;
    public static UdpClient udpClient = new UdpClient(11000);
    public static List<string> CurrentIPsRequests = new List<string>();
    public static string CurrentScene;
    public static GameObject PlayerPrefab => Resources.Load<GameObject>("Player");
    public static string ObterMeuIp()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            return endPoint.Address.ToString();
        }
    }
}
