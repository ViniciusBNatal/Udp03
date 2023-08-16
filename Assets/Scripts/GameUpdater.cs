using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpawnPoints))]
public class GameUpdater : MonoBehaviour
{
    private SpawnPoints _spawnScript;
    [SerializeField] GameObject _playerPrefab;
    private string _currentScene;
    private bool _sceneLoaded;

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
        if (_sceneLoaded)
        {
            for (int i = 0; i < MltJogador.CurrentIPsRequests.Count; i++)
            {
                if (MltJogador.clientes.ContainsKey(MltJogador.CurrentIPsRequests[i]))
                {
                    RemovePlayer(MltJogador.CurrentIPsRequests[i]);
                }
                else
                {
                    GenerateNewPlayer(MltJogador.CurrentIPsRequests[i]);
                }
                MltJogador.CurrentIPsRequests.Remove(MltJogador.CurrentIPsRequests[i]);
            }
        }
    }

    private void UpdateGame()
    {
        if(MltJogador.CurrentProcessingDataType == MltJogador.DataTypes.SpawnPlayer)
        {
           AsyncOperation operation = SceneManager.LoadSceneAsync("Jogo", LoadSceneMode.Single);
            operation.completed += OnSceneLoad;
        }
        //if(!string.IsNullOrEmpty(MltJogador.CurrentScene) && _currentScene != MltJogador.CurrentScene)
        //{
        //    _currentScene = MltJogador.CurrentScene;
        //    SceneManager.LoadScene(_currentScene);
        //}
    }

    private void OnSceneLoad(AsyncOperation operation)
    {
        _sceneLoaded = true;
    }

    private void GenerateNewPlayer(string IP)
    {
        Vector3 temp = _spawnScript.GetSpawnPoint(IP);
        GameObject player = Instantiate(_playerPrefab, temp, Quaternion.identity);
        //for testing
        float randomVal = UnityEngine.Random.Range(1, 100);
        player.GetComponent<Transform>().localScale = new Vector3(randomVal, randomVal, randomVal);
        MltJogador.clientes.Add(IP, new MltJogador.PlayerData(player));
        MltJogador.CurrentProcessingDataType = MltJogador.DataTypes.UpdateValues;
    }

    private void RemovePlayer(string IP)
    {
        if (MltJogador.clientes.ContainsKey(IP))
        {
            _spawnScript.ClearSpawnPointUsage(IP);
            Destroy(MltJogador.clientes[IP].Obj);
            MltJogador.clientes.Remove(IP);
        }
        else
        {
            Debug.LogError("Trying to delete a player that doesnt exist");
        }
        MltJogador.CurrentProcessingDataType = MltJogador.DataTypes.UpdateValues;
    }
}
