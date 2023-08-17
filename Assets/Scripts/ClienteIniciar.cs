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
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.servidor), 11000);
            string[] IPs = MltJogador.Players.Keys.ToArray();
            for (int i = 0; i < MltJogador.Players.Count; i++)
            {
                if(IPs[i] != MltJogador.servidor)
                {
                    _memoryStream = new MemoryStream();
                    //_dataContainer.CurrentPackageDataBeingProcessed = MltJogador.Players[MltJogador.ObterMeuIp()];
                    MltJogador.Players[IPs[i]].CurrentDataMode = DataPackage.DataState.RemovePlayer;
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
            MltJogador.Players[MltJogador.ObterMeuIp()] = GenerateRemovePlayerPackage();
            _binaryFormatter.Serialize(_memoryStream, /*_dataContainer.CurrentPackageDataBeingProcessed*/MltJogador.Players[MltJogador.ObterMeuIp()]);
            byte[] info = _memoryStream.ToArray();
            MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
            MltJogador.servidor = null;
        }
    }

    public void MovmentDataCollection(Vector3 velocity)
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MltJogador.servidor), 11000);
        _memoryStream = new MemoryStream();
        _dataContainer.CurrentPackageDataBeingProcessed = GenerateMovmentPackage(velocity);
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
                MltJogador.Players.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP, _dataContainer.CurrentPackageDataBeingProcessed);
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
                break;
            case DataPackage.DataState.RemovePlayer:
                //quit aplication
                MltJogador.Players[_dataContainer.CurrentPackageDataBeingProcessed.IP].CurrentDataMode = DataPackage.DataState.RemovePlayer;
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingProcessed.IP);
                break;
            case DataPackage.DataState.UpdateValues:
                break;
                //case DataPackage.DataState.Neutral:
                //    break;
        }
    }

    #region DataGenerators
    private DataPackage GenerateSpawnPlayerPackage()
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            MltJogador.PlayerPrefab, "Jogo",
            Vector3.zero,
            DataPackage.DataState.SpawnPlayer
            /*Vector3.zero*/);
    }

    private DataPackage GenerateRemovePlayerPackage()
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            MltJogador.PlayerPrefab, "Entrada",
            Vector3.zero,
            DataPackage.DataState.RemovePlayer);
    }

    private DataPackage GenerateMovmentPackage(Vector3 direction)
    {
        return new DataPackage(MltJogador.ObterMeuIp(),
            MltJogador.PlayerPrefab, MltJogador.Players[MltJogador.ObterMeuIp()].CurrentScene,
            direction,
            DataPackage.DataState.UpdateValues
            /*Vector3.zero*/);
    }
    #endregion
}
