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

[RequireComponent(typeof(DataContainer))]
public class ServidorIniciar : MonoBehaviour
{
    public const int PORT = 11000;
    BinaryFormatter _binaryFormatter = new BinaryFormatter();
    MemoryStream _memoryStream;
    //private DataContainer _dataContainer;
    private Thread _thread;
    private const float _sendDataFrequency = .02f;
    private float _timeSinceLastDataSend;
    private DataPackage _currentDataPackage;

    private void Awake()
    {
        MltJogador.ServerScript = this;
    }

    void Start()
    {
        if (MltJogador.servidor != MltJogador.ObterMeuIp()) Destroy(this);
        //_dataContainer = GetComponent<DataContainer>();
        _thread = new Thread(ReceberDados);
        _thread.Start();
    }

    private void Update()
    {
        _timeSinceLastDataSend += Time.deltaTime;
        if (_timeSinceLastDataSend >= _sendDataFrequency)
        {
            SendChangePlayerMovment();
            SendRemovePlayerData();
        }
    }

    void ReceberDados()
    {
        //IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //while (true)
        //{
        //    _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
        //    _dataContainer.CurrentPackageDataBeingProcessed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
        //    ProcessData();
        //}
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //Blocks until a message returns on this socket from a remote host.
        while (true)
        {
            _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
            //_dataContainer.CurrentPackageDataBeingProcessed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            DataPackage package = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            if(!MltJogador.Players.ContainsKey(package.IP) || MltJogador.Players[package.IP].DataPackage != package)
            {
                _currentDataPackage = package;
                ProcessData();
            }

        }
    }

    public void SendStartGame()
    {
        MltJogador.InGameData[] temp = MltJogador.Players.Values.ToArray();
        //DataPackage[] datas = MltJogador.Players.Values.Select(x => x.DataPackage).ToArray();
        DataPackage[] datas = new DataPackage[temp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            datas[i] = temp[i].DataPackage;
            datas[i].CurrentScene = "Jogo";
        }
        for (int i = 0; i < datas.Length; i++)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(datas[i].IP), PORT);
            for (int a = 0; i < datas.Length; a++)
            {
                _memoryStream = new MemoryStream();
                //_dataContainer.CurrentPackageDataBeingProcessed = datas[a];
                _binaryFormatter.Serialize(_memoryStream, datas[a]/*_dataContainer.CurrentPackageDataBeingProcessed*/);
                byte[] info = _memoryStream.ToArray();
                MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
            }
        }
    }

    private void SendRemovePlayerData()
    {
        for (int i = 0; i < MltJogador.CurrentIPsRequests.Count; i++)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.CurrentIPsRequests[i]), PORT);
            _memoryStream = new MemoryStream();
            _binaryFormatter.Serialize(_memoryStream, MltJogador.Players[MltJogador.CurrentIPsRequests[i]].DataPackage);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
        }
    }

    private void SendChangePlayerMovment()
    {
        for (int i = 0; i < MltJogador.CurrentIPsRequests.Count; i++)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.CurrentIPsRequests[i]), PORT);
            _memoryStream = new MemoryStream();
            _binaryFormatter.Serialize(_memoryStream, MltJogador.Players[MltJogador.CurrentIPsRequests[i]].DataPackage);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
        }
    }

    public void SetCurrentDataPackage(DataPackage data)
    {
        _currentDataPackage = data;
    }

    #region DataGenerators
    public void ProcessData()
    {
        switch (_currentDataPackage.CurrentDataMode)
        {
            case DataPackage.DataState.SpawnPlayer:
                /*if (!MltJogador.Players.ContainsKey(_dataContainer.CurrentPackageDataBeingProcessed.IP))*/
                if (!MltJogador.Players.ContainsKey(_currentDataPackage.IP)) MltJogador.Players.Add(_currentDataPackage.IP, new MltJogador.InGameData(_currentDataPackage, null, null));
                break;
            case DataPackage.DataState.RemovePlayer:
                MltJogador.Players[_currentDataPackage.IP].DataPackage.CurrentDataMode = DataPackage.DataState.RemovePlayer;
                //SendRemovePlayerData();
                break;
            case DataPackage.DataState.UpdateValues:
                MltJogador.Players[_currentDataPackage.IP].DataPackage.PlayerDirection = _currentDataPackage.PlayerDirection;
                break;
                //case DataPackage.DataState.Neutral:
                //    break;
        }
        MltJogador.CurrentIPsRequests.Add(_currentDataPackage.IP);
        //switch (_dataContainer.CurrentPackageDataBeingProcessed.CurrentDataMode)
        //{
        //    case DataPackage.DataState.SpawnPlayer:
        //        /*if (!MltJogador.Players.ContainsKey(_dataContainer.CurrentPackageDataBeingProcessed.IP))*/ MltJogador.Players.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP, new MltJogador.InGameData(_dataContainer.CurrentPackageDataBeingProcessed, null, null));
        //        break;
        //    case DataPackage.DataState.RemovePlayer:
        //        MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].DataPackage.CurrentDataMode = DataPackage.DataState.RemovePlayer;
        //        //SendRemovePlayerData();
        //        break;
        //    case DataPackage.DataState.UpdateValues:
        //        MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].DataPackage.PlayerDirection = _dataContainer.CurrentPackageDataBeingProcessed.PlayerDirection;
        //        break;
        //        //case DataPackage.DataState.Neutral:
        //        //    break;
        //}
        //MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
    }
    #endregion
}
