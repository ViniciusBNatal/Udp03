using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscolhaPapel : MonoBehaviour
{
    public void Cliente()
    {
        SceneManager.LoadScene("Cliente");
    }
    public void Servidor()
    {
        SceneManager.LoadScene("Servidor");
    }
}
