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
    private DataContainer _dataContainer;
    private Thread _thread;

    private void Awake()
    {
        MltJogador.ServerScript = this;
    }

    void Start()
    {
        if(MltJogador.servidor != MltJogador.ObterMeuIp()) Destroy(this);
        _dataContainer.GetComponent<DataContainer>();        
        _thread = new Thread(ReceberDados);
        _thread.Start();
    }

    void ReceberDados()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
            _dataContainer.CurrentPackageDataBeingProcessed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            ProcessData();            
        }
    }

    public void IniciarJogo()
    {
        DataPackage[] datas = MltJogador.Players.Values.ToArray();
        for (int i = 0; i < datas.Length; i++)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(datas[i].IP), 11000);
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
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.CurrentIPsRequests[i]), 11000);
            _memoryStream = new MemoryStream();
            //_dataContainer.CurrentPackageDataBeingProcessed = MltJogador.Players[MltJogador.CurrentIPsRequests[i]];
            _binaryFormatter.Serialize(_memoryStream, /*_dataContainer.CurrentPackageDataBeingProcessed*/MltJogador.Players[MltJogador.CurrentIPsRequests[i]]);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
        }
    }

    #region DataGenerators
    public void ProcessData()
    {
        switch (_dataContainer.CurrentPackageDataBeingProcessed.CurrentDataMode)
        {
            case DataPackage.DataState.SpawnPlayer:
                MltJogador.Players.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP, _dataContainer.CurrentPackageDataBeingProcessed);
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
                break;
            case DataPackage.DataState.RemovePlayer:
                MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].CurrentDataMode = DataPackage.DataState.RemovePlayer;
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
                SendRemovePlayerData();
                break;
            case DataPackage.DataState.UpdateValues:
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
                break;
            //case DataPackage.DataState.Neutral:
            //    break;
        }
    }
    #endregion
}
