using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System.IO;

public class ServidorIniciar : MonoBehaviour
{
    public const int PORT = 11000;
    [SerializeField]
    Button iniciar;
    [SerializeField]
    TextMeshProUGUI listaClientes;
    BinaryFormatter _binaryFormatter = new BinaryFormatter();
    MemoryStream _memoryStream;
    private DataContainer _dataContainer;

    // Start is called before the first frame update
    void Start()
    {
        if(MltJogador.servidor != MltJogador.ObterMeuIp()) Destroy(this);
        _dataContainer.GetComponent<DataContainer>();
        listaClientes.text = "";
        Thread thread1 = new Thread(ReceberDados);
        thread1.Start();
        //MltJogador.ObterMeuIp();
        //MltJogador.servidor = MltJogador.ObterMeuIp();        
    }

    // Update is called once per frame
    void Update()
    {
        PreencheLista();
        //UpdatePlayers();
    }

    void ReceberDados()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            //Byte[] receiveBytes = MltJogador.udpClient.Receive(ref RemoteIpEndPoint);
            _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
            _dataContainer.CurrentPackageDataBeingUsed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            ProcessData();
            //string returnData = Encoding.ASCII.GetString(receiveBytes);
            //try
            //{
            //    //if (returnData != "REMOVER")
            //    //{
            //    //    _currentIP = RemoteIpEndPoint.Address.ToString();
            //    //    //GenerateNewPlayer(RemoteIpEndPoint.Address.ToString());
            //    //}
            //    //else
            //    //{
            //    //    //RemovePlayer(RemoteIpEndPoint.Address.ToString());
            //    //}
            //    string ip = RemoteIpEndPoint.Address.ToString();
            //    if (!MltJogador.CurrentIPsRequests.Contains(ip))
            //    {
            //        MltJogador.CurrentIPsRequests.Add(ip);
            //    }
            //}
            //catch (Exception e)
            //{
            //    Debug.LogException(e);
            //}
        }
    }

    //private void GenerateNewPlayer(string IP)
    //{
    //    Vector3 temp = _spawnScript.GetSpawnPoint(IP);
    //    GameObject player = Instantiate(_playerPrefab, temp, Quaternion.identity);
    //    //for testing
    //    float randomVal = UnityEngine.Random.Range(1, 100);
    //    player.GetComponent<Transform>().localScale = new Vector3(randomVal, randomVal, randomVal);
    //    MltJogador.clientes.Add(IP, new MltJogador.PlayerData(player));
    //}

    //private void RemovePlayer(string IP)
    //{
    //    if (MltJogador.clientes.ContainsKey(IP))
    //    {
    //        _spawnScript.ClearSpawnPointUsage(IP);
    //        Destroy(MltJogador.clientes[IP].Obj);
    //        MltJogador.clientes.Remove(IP);
    //    }
    //    else
    //    {
    //        Debug.LogError("Trying to delete a player that doesnt exist");
    //    }
    //}

    //private void UpdatePlayers()
    //{
    //    if (_canStart)
    //    {
    //        for (int i = 0; i < _currentIPsRequests.Count; i++)
    //        {
    //            if (MltJogador.clientes.ContainsKey(_currentIPsRequests[i]))
    //            {
    //                RemovePlayer(_currentIPsRequests[i]);
    //            }
    //            else
    //            {
    //                GenerateNewPlayer(_currentIPsRequests[i]);
    //            }
    //            _currentIPsRequests.Remove(_currentIPsRequests[i]);
    //        }                  
    //    }
    //}

    public void IniciarJogo()
    {
        DataPackage[] datas = MltJogador.clientes.Values.ToArray();
        for (int i = 0; i < datas.Length; i++)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(datas[i].IP), 11000);
            for (int a = 0; i < datas.Length; a++)
            {
                _memoryStream = new MemoryStream();
                _dataContainer.CurrentPackageDataBeingUsed = datas[a];
                _binaryFormatter.Serialize(_memoryStream, _dataContainer.CurrentPackageDataBeingUsed);
                byte[] info = _memoryStream.ToArray();
                MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
            }
            //Byte[] sendBytes = Encoding.ASCII.GetBytes("INICIAR");
            //Byte[] sendBytes = Encoding.ASCII.GetBytes(MltJogador.DataTypes.SpawnPlayer.ToString());
            //MltJogador.udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
        }
        //SceneManager.LoadScene("Jogo");
    }

    void PreencheLista()
    {
        string str = "";
        for (int i = 0; i < MltJogador.CurrentIPsRequests.Count; i++)
        {
            str = MltJogador.CurrentIPsRequests[i];
            //str += MltJogador.clientes.Keys.ElementAt(i) + " " + MltJogador.clientes.Values.ElementAt(i) + "\n";
        }
        listaClientes.text = str;
    }

    #region DataGenerators
    private void ProcessData()
    {
        switch (_dataContainer.CurrentPackageDataBeingUsed.CurrentDataMode)
        {
            case DataPackage.DataState.SpawnPlayer:
                MltJogador.clientes.Add(_dataContainer.CurrentPackageDataBeingUsed.IP, _dataContainer.CurrentPackageDataBeingUsed);
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingUsed.IP);
                break;
            case DataPackage.DataState.RemovePlayer:
                break;
            case DataPackage.DataState.UpdateValues:
                break;
            default:
                break;
        }
    }
    #endregion
}
