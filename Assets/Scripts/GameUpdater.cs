using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(SpawnPoints))]
public class GameUpdater : MonoBehaviour
{
    private SpawnPoints _spawnScript;
    private string _sceneAfterLoad;
    private bool _sceneIsLoading;

    private void Awake()
    {
        _spawnScript = GetComponent<SpawnPoints>();
    }

    void Update()
    {
        UpdateGame();
        UpdatePlayers();
    }

    private void UpdatePlayers()
    {
        if (!_sceneIsLoading && MltJogador.CurrentScene == "Jogo")
        {
            for (int i = 0; i < MltJogador.CurrentIPsRequests.Count; i++)
            {
                if (MltJogador.Players.ContainsKey(MltJogador.CurrentIPsRequests[i]))
                {
                    switch (MltJogador.Players[MltJogador.CurrentIPsRequests[i]].CurrentDataMode)
                    {
                        case DataPackage.DataState.SpawnPlayer:
                            GenerateNewPlayer(MltJogador.CurrentIPsRequests[i]);
                            break;
                        case DataPackage.DataState.RemovePlayer:
                            RemovePlayer(MltJogador.CurrentIPsRequests[i]);
                            break;
                        case DataPackage.DataState.UpdateValues:
                            UpdatePlayerMovment(MltJogador.CurrentIPsRequests[i]);
                            break;
                            //case DataPackage.DataState.Neutral:
                            //    break;
                    }
                    MltJogador.CurrentIPsRequests.Remove(MltJogador.CurrentIPsRequests[i]);
                    //MltJogador.Players[MltJogador.CurrentIPsRequests[i]].CurrentDataMode = DataPackage.DataState.Neutral;
                }
            }
        }
    }

    private void UpdateGame()
    {
        if (MltJogador.Players.ContainsKey(MltJogador.ObterMeuIp()) && !string.IsNullOrEmpty(MltJogador.Players[MltJogador.ObterMeuIp()].CurrentScene) && MltJogador.CurrentScene != MltJogador.Players[MltJogador.ObterMeuIp()].CurrentScene)
        {
            if (MltJogador.Players[MltJogador.ObterMeuIp()].CurrentDataMode == DataPackage.DataState.SpawnPlayer)
            {
                _sceneIsLoading = true;
                _sceneAfterLoad = MltJogador.Players[MltJogador.ObterMeuIp()].CurrentScene;
                AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneAfterLoad, LoadSceneMode.Single);
                operation.completed += OnSceneLoad;
            }
            else if (MltJogador.Players[MltJogador.ObterMeuIp()].CurrentDataMode == DataPackage.DataState.RemovePlayer)
            {
                _sceneAfterLoad = MltJogador.Players[MltJogador.ObterMeuIp()].CurrentScene;
                MltJogador.Players.Clear();
                MltJogador.servidor = null;
                MltJogador.CurrentIPsRequests.Clear();
                _sceneIsLoading = true;
                AsyncOperation operation = SceneManager.LoadSceneAsync(_sceneAfterLoad, LoadSceneMode.Single);
                operation.completed += OnSceneLoad;
            }
        }
    }

    private void OnSceneLoad(AsyncOperation operation)
    {
        MltJogador.CurrentScene = _sceneAfterLoad;
        _sceneIsLoading = false;
    }

    private void GenerateNewPlayer(string IP)
    {
        Vector3 spawnPoint = _spawnScript.GetSpawnPoint(IP);
        GameObject player = Instantiate(MltJogador.PlayerPrefab, spawnPoint, Quaternion.identity);
        player.GetComponent<MeshRenderer>().material.color = GenerateColorByIP(IP);
        //for testing
        //float randomVal = UnityEngine.Random.Range(1, 5);
        //player.GetComponent<Transform>().localScale = new Vector3(randomVal, randomVal, randomVal);
        MltJogador.Players[IP].PlayerObject = player;
        MltJogador.Players[IP].PlayerMovment = player.GetComponent<PlayerMovment>();
        if (IP == MltJogador.ObterMeuIp()) MltJogador.Players[IP].PlayerMovment.SetBeingControledLocaly(true);
        //MltJogador.Players[IP].SpawnLocation = spawnPoint;
    }

    private Color GenerateColorByIP(string IP)
    {
        char[] chars = IP.ToCharArray();
        string thirdPartText = null;
        int.TryParse((chars[0] + chars[1] + chars[2]).ToString(), out int firstPart);
        int.TryParse((chars[4] + chars[5] + chars[6]).ToString(), out int secondPart);
        for (int i = 7; i < chars.Length; i++)
        {
            string temp = chars[i].ToString();
            if (int.TryParse(temp, out int num))
            {
                thirdPartText += temp;
                if (thirdPartText.Length == 3) break;
            }
        }
        int.TryParse(thirdPartText, out int thirdPart);

        return new Color(firstPart - thirdPart, secondPart + thirdPart, (firstPart + secondPart + thirdPart) / 3);
    }

    private void RemovePlayer(string IP)
    {
        if (IP != MltJogador.ObterMeuIp())
        {
            _spawnScript.ClearSpawnPointUsage(IP);
            Destroy(MltJogador.Players[IP].PlayerObject);
            MltJogador.Players.Remove(IP);
        }
        else
        {
            Debug.LogError("Trying to delete a player that doesnt exist");
        }
    }

    private void UpdatePlayerMovment(string IP)
    {
        MltJogador.Players[IP].PlayerMovment.SetCurrentDirection(MltJogador.Players[IP].PlayerDirection);
    }
}
