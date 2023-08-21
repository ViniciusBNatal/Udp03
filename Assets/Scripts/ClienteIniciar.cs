using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

[RequireComponent(typeof(DataContainer))]
public class ClienteIniciar : MonoBehaviour
{
    BinaryFormatter _binaryFormatter = new BinaryFormatter();
    MemoryStream _memoryStream;
    private DataContainer _dataContainer;
    private Thread _thread;

    private void Awake()
    {
        _dataContainer = GetComponent<DataContainer>();
        MltJogador.ClientScript = this;
    }

    void Start()
    {
        _thread = new Thread(ReceberDados);
        _thread.Start();
    }

    public void Conectar(string serverIP)
    {
        //if this machine is the Host, will go directly to precessing data, no need to send data
        if (MltJogador.servidor == MltJogador.ObterMeuIp())
        {
            DataPackage temp = GenerateSpawnPlayerPackage();
            ServidorIniciar servidor = FindObjectOfType<ServidorIniciar>();
            servidor.GetComponent<DataContainer>().CurrentPackageDataBeingProcessed = temp;
            servidor.ProcessData();
        }
        else
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), 11000);
            _memoryStream = new MemoryStream();
            DataPackage temp = GenerateSpawnPlayerPackage();
            //_dataContainer.CurrentPackageDataBeingProcessed = temp;
            _binaryFormatter.Serialize(_memoryStream, /*_dataContainer.CurrentPackageDataBeingProcessed*/temp);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
            MltJogador.servidor = serverIP;
        }

    }
    public void Desconectar(string serverIP)
    {
        if (MltJogador.servidor == MltJogador.ObterMeuIp())
        {
            string[] IPs = MltJogador.Players.Keys.ToArray();
            for (int i = 0; i < MltJogador.Players.Count; i++)
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IPs[i]), 11000);
                if(IPs[i] != MltJogador.servidor)
                {
                    _memoryStream = new MemoryStream();
                    //_dataContainer.CurrentPackageDataBeingProcessed = MltJogador.Players[MltJogador.ObterMeuIp()];
                    MltJogador.Players[IPs[i]].DataPackage.CurrentDataMode = DataPackage.DataState.RemovePlayer;
                    _binaryFormatter.Serialize(_memoryStream, /*_dataContainer.CurrentPackageDataBeingProcessed*/MltJogador.Players[IPs[i]]);
                    byte[] info = _memoryStream.ToArray();
                    MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
                }
            }
        }
        else
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), 11000);
            _memoryStream = new MemoryStream();
            //_dataContainer.CurrentPackageDataBeingProcessed = MltJogador.Players[MltJogador.ObterMeuIp()];
            MltJogador.Players[MltJogador.ObterMeuIp()].DataPackage = GenerateRemovePlayerPackage();
            _binaryFormatter.Serialize(_memoryStream, /*_dataContainer.CurrentPackageDataBeingProcessed*/MltJogador.Players[MltJogador.ObterMeuIp()]);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
            MltJogador.servidor = null;
        }
    }

    public void MovmentDataCollection(float[] direction)
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.servidor), 11000);
        _memoryStream = new MemoryStream();
        _dataContainer.CurrentPackageDataBeingProcessed = GenerateMovmentPackage(direction);
        _binaryFormatter.Serialize(_memoryStream, _dataContainer.CurrentPackageDataBeingProcessed);
        byte[] info = _memoryStream.ToArray();
        MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
    }

    void ReceberDados()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //Blocks until a message returns on this socket from a remote host.
        while (true)
        {
            _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
            _dataContainer.CurrentPackageDataBeingProcessed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            ProcessData();
        }
    }

    private void ProcessData()
    {
        switch (_dataContainer.CurrentPackageDataBeingProcessed.CurrentDataMode)
        {
            case DataPackage.DataState.SpawnPlayer:
                MltJogador.Players.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP, new MltJogador.InGameData(_dataContainer.CurrentPackageDataBeingProcessed, null));
                break;
            case DataPackage.DataState.RemovePlayer:
                MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].DataPackage.CurrentDataMode = DataPackage.DataState.RemovePlayer;
                break;
            case DataPackage.DataState.UpdateValues:
                MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].DataPackage.PlayerDirection = _dataContainer.CurrentPackageDataBeingProcessed.PlayerDirection;                
                break;
                //case DataPackage.DataState.Neutral:
                //    break;
        }
        MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
    }

    #region DataGenerators
    private DataPackage GenerateSpawnPlayerPackage()
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            "Jogo",
            null,
            DataPackage.DataState.SpawnPlayer,
            null
            /*Vector3.zero*/);
    }

    private DataPackage GenerateRemovePlayerPackage()
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            "Entrada",
            null,
            DataPackage.DataState.RemovePlayer,
            null);
    }

    private DataPackage GenerateMovmentPackage(float[] direction)
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            MltJogador.Players[MltJogador.ObterMeuIp()].DataPackage.CurrentScene,
            direction,
            DataPackage.DataState.UpdateValues,
            null
            /*Vector3.zero*/);
    }
    #endregion
}
