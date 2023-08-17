using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerMenuUI : MonoBehaviour
{
    [SerializeField]
    Button iniciar;
    [SerializeField]
    TextMeshProUGUI listaClientes;
    [SerializeField] private ServidorIniciar _serverScript;

    private void Awake()
    {
        listaClientes.text = "";
    }

    private void Update()
    {
        PreencheLista();
    }

    public void StartGame()
    {
        _serverScript.IniciarJogo();
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
}
