using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MltJogador 
{
    public static Dictionary<string, PlayerData> clientes = new Dictionary<string, PlayerData>();
    public static string servidor="";
    public static string meuIp;
    public static UdpClient udpClient = new UdpClient(11000);
    public static List<string> CurrentIPsRequests = new List<string>();
    public static string CurrentScene; 

    public static DataTypes CurrentProcessingDataType;

    public enum DataTypes
    {
        SpawnPlayer,
        RemovePlayer,
        UpdateValues
    };

    [System.Serializable]
    public struct PlayerData
    {
        public GameObject Obj;
        //public SpawnPoints.SpawnPointInfo SpawnPointInfo;

        public PlayerData(GameObject player/*, SpawnPoints.SpawnPointInfo spawnPointInfo*/)
        {
            Obj = player;
            //SpawnPointInfo = spawnPointInfo;
        }
    }

    [System.Serializable]
    public struct LevelData
    {
        public string SceneToOpen;
    }

    //public class GameData
    //{
    //    public PlayerData PlayerData;
    //    public LevelData LevelData;
    //}

    [System.Serializable]
    public struct GameData
    {
        public string IP;
        public DataTypes DataType;
    }

    public static void ObterMeuIp()
    {
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            MltJogador.meuIp = endPoint.Address.ToString();
        }
    }
}
