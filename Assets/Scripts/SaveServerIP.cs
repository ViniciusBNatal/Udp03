using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveServerIP : MonoBehaviour
{
    private void Awake()
    {
        MltJogador.servidor = MltJogador.ObterMeuIp();
    }
}
