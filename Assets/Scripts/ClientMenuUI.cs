using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClientMenuUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField servidor, nome;
    [SerializeField]
    Button conectar, cancelar;
    [SerializeField] private ClienteIniciar _clientScript;

    private void Awake()
    {
        conectar.interactable = true;
        cancelar.interactable = false;
    }

    public void Conect()
    {
        _clientScript.Conectar(servidor.text);
        conectar.interactable = false;
        cancelar.interactable = true;
    }

    public void Disconect()
    {
        _clientScript.Desconectar(servidor.text);
        conectar.interactable = true;
        cancelar.interactable = false;
    }

    public void ConectForServer()
    {
        _clientScript.Conectar(MltJogador.servidor);
    }
}
