using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private SpawnPointInfo[] _spawnPoints;

    [System.Serializable]
    public struct SpawnPointInfo
    {
        public Vector3 SpawnPoint;
        [HideInInspector] public bool InUse;

        public SpawnPointInfo(Vector3 spawnPoint)
        {
            SpawnPoint = spawnPoint;
            InUse = false;
        }
    }
    private SpawnPointInfo[] _currentSpawnPoints;

    private Dictionary<string, SpawnPointInfo> _spawnPointDic = new Dictionary<string, SpawnPointInfo>();

    private void Awake()
    {
        _currentSpawnPoints = new SpawnPointInfo[_spawnPoints.Length];
        for(int i = 0; i < _spawnPoints.Length; i++)
        {
            _currentSpawnPoints[i] = new SpawnPointInfo(_spawnPoints[i].SpawnPoint);
        }
    }

    public Vector3 GetSpawnPoint(string IP)
    {
        if (!_spawnPointDic.ContainsKey(IP))
        {
            for(int i = 0; i < _currentSpawnPoints.Length; i++)
            {
                if (!_currentSpawnPoints[i].InUse)
                {
                    _currentSpawnPoints[i].InUse = true;
                    _spawnPointDic.Add(IP, _currentSpawnPoints[i]);
                    return _currentSpawnPoints[i].SpawnPoint;
                }
            }
        }
        Debug.LogError("FailToFindValidSpawnPoint");
        return Vector3.zero;
    }

    public void ClearSpawnPointUsage(string IP)
    {
        if(_spawnPointDic.TryGetValue(IP, out SpawnPointInfo value))
        {
            value.InUse = false;
            _spawnPointDic.Remove(IP);
        }

    }

}
