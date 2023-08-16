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

public class ClienteIniciar : MonoBehaviour
{
    [SerializeField]
    TMP_InputField servidor, nome;
    [SerializeField]
    Button conectar, cancelar;
    string acao;


    // Start is called before the first frame update
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
        Byte[] sendBytes = Encoding.ASCII.GetBytes(nome.text + "/" + MltJogador.DataTypes.SpawnPlayer);
        MltJogador.udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
        conectar.interactable = false;
        cancelar.interactable = true;
    }
    public void Desconectar()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(servidor.text), 11000);
        //Byte[] sendBytes = Encoding.ASCII.GetBytes("REMOVER");
        Byte[] sendBytes = Encoding.ASCII.GetBytes(MltJogador.DataTypes.RemovePlayer.ToString());
        MltJogador.udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
        conectar.interactable = true;
        cancelar.interactable = false;
    }

    void ReceberDados()
    {
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        bool emLoop = true;
        //Blocks until a message returns on this socket from a remote host.
        while (emLoop)
        {
            Byte[] receiveBytes = MltJogador.udpClient.Receive(ref RemoteIpEndPoint);
            string returnData = Encoding.ASCII.GetString(receiveBytes);

            if(Enum.TryParse<MltJogador.DataTypes>(returnData, out MltJogador.DataTypes data))
            {
                MltJogador.CurrentProcessingDataType = data;
            }
            //if (returnData == "INICIAR")
            //{
            //    MltJogador.servidor = RemoteIpEndPoint.Address.ToString();
            //    emLoop = false;
            //    acao = "INICIAR";
            //}
        }
        
 
    }
}
