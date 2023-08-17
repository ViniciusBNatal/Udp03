using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerScriptInstaler : MonoBehaviour
{
    private void Awake()
    {
        if(MltJogador.servidor == MltJogador.ObterMeuIp()) this.gameObject.AddComponent<ServidorIniciar>();
    }
}
