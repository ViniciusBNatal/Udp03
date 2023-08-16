using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataContainer : MonoBehaviour
{
    public DataPackage CurrentPackageDataBeingUsed;
    //public DataPackage ClientPackage;
}
public class DataPackage
{
    public string IP;
    public GameObject PlayerObject;
    public string CurrentScene;
    public Vector3 PlayerDirection;
    public Vector3 SpawnLocation;
    public DataState CurrentDataMode;
    public enum DataState
    {
        SpawnPlayer,
        RemovePlayer,
        UpdateValues,
        Neutral
    };    
    public DataPackage(string ip, GameObject gobj, string sceneName, Vector3 direction, DataState dataState, Vector3 spawnPoint)
    {
        IP = ip;
        PlayerObject = gobj;
        CurrentScene = sceneName;
        PlayerDirection = direction;
        CurrentDataMode = dataState;
        SpawnLocation = spawnPoint;
    }
}
