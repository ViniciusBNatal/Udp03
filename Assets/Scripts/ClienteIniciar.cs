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

public class ClienteIniciar : MonoBehaviour
{
    [SerializeField]
    TMP_InputField servidor, nome;
    [SerializeField]
    Button conectar, cancelar;    
    string acao;
    BinaryFormatter _binaryFormatter = new BinaryFormatter();
    MemoryStream _memoryStream;
    private DataContainer _dataContainer;

    private void Awake()
    {
        _dataContainer = GetComponent<DataContainer>();

    }

    void Start()
    {
        conectar.interactable = true;
        cancelar.interactable = false;
        Thread thread1 = new Thread(ReceberDados);
        thread1.Start();
    }

    // Update is called once per frame
    void Update()
    {
        switch(acao)
        {
            case "INICIAR":
                SceneManager.LoadScene("Jogo");
                break;
        }
    }

    public void Conectar()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(servidor.text), 11000);
        //Byte[] sendBytes = Encoding.ASCII.GetBytes(nome.text + "/" + MltJogador.DataTypes.SpawnPlayer);
        _memoryStream = new MemoryStream();
        DataPackage temp = GenerateNewPlayer();
        _dataContainer.CurrentPackageDataBeingUsed = temp;
        //_dataContainer.ClientPackage = temp;
        _binaryFormatter.Serialize(_memoryStream, _dataContainer.CurrentPackageDataBeingUsed);
        byte[] info = _memoryStream.ToArray();
        MltJogador.udpClient.Send(info, info.Length, ipEndPoint);

        conectar.interactable = false;
        cancelar.interactable = true;
    }
    public void Desconectar()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(servidor.text), 11000);
        //Byte[] sendBytes = Encoding.ASCII.GetBytes("REMOVER");
        //Byte[] sendBytes = Encoding.ASCII.GetBytes(MltJogador.DataTypes.RemovePlayer.ToString());
        _memoryStream = new MemoryStream();
        _dataContainer.CurrentPackageDataBeingUsed = MltJogador.clientes[MltJogador.ObterMeuIp()];
        _binaryFormatter.Serialize(_memoryStream, _dataContainer.CurrentPackageDataBeingUsed);
        byte[] info = _memoryStream.ToArray();
        MltJogador.udpClient.Send(info, info.Length, ipEndPoint);
        conectar.interactable = true;
        cancelar.interactable = false;
    }

    void ReceberDados()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        //Blocks until a message returns on this socket from a remote host.
        while (true)
        {
            _memoryStream = new MemoryStream(MltJogador.udpClient.Receive(ref RemoteIpEndPoint));
            _dataContainer.CurrentPackageDataBeingUsed = (DataPackage)_binaryFormatter.Deserialize(_memoryStream);
            ProcessData();
            //Byte[] receiveBytes = MltJogador.udpClient.Receive(ref RemoteIpEndPoint);
            //string returnData = Encoding.ASCII.GetString(receiveBytes);

            //if(Enum.TryParse<MltJogador.DataTypes>(returnData, out MltJogador.DataTypes data))
            //{
            //    MltJogador.CurrentProcessingDataType = data;
            //}
            //if (returnData == "INICIAR")
            //{
            //    MltJogador.servidor = RemoteIpEndPoint.Address.ToString();
            //    emLoop = false;
            //    acao = "INICIAR";
            //}
        }
        
 
    }

    private void ProcessData()
    {
        switch (_dataContainer.CurrentPackageDataBeingUsed.CurrentDataMode)
        {
            case DataPackage.DataState.SpawnPlayer:
                //_dataContainer.CurrentPackageBeingSend.SpawnLocation = _spawnScript.GetSpawnPoint(_dataContainer.CurrentPackageBeingSend.IP);
                MltJogador.clientes.Add(_dataContainer.CurrentPackageDataBeingUsed.IP, _dataContainer.CurrentPackageDataBeingUsed);
                MltJogador.CurrentIPsRequests.Add(_dataContainer.CurrentPackageDataBeingUsed.IP);
                break;
            case DataPackage.DataState.RemovePlayer:
                //quit aplication
                break;
            case DataPackage.DataState.UpdateValues:
                break;
            default:
                break;
        }
    }

    #region DataGenerators
    private DataPackage GenerateNewPlayer()
    {
        return new DataPackage(MltJogador.ObterMeuIp(), 
            MltJogador.PlayerPrefab, "Jogo", 
            Vector3.zero, 
            DataPackage.DataState.SpawnPlayer,
            Vector3.zero);
    }
    #endregion
}
