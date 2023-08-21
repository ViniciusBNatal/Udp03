using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataContainer : MonoBehaviour
{
    [HideInInspector] public DataPackage CurrentPackageDataBeingProcessed;
}
[System.Serializable]
public class DataPackage
{
    public string IP;
    //public GameObject PlayerObject;
    //public PlayerMovment PlayerMovment;
    public string CurrentScene;
    public float[] PlayerDirection = new float[3];
    public float[] SpawnLocation = new float[3];
    public DataState CurrentDataMode;
    public enum DataState
    {
        SpawnPlayer,
        RemovePlayer,
        UpdateValues
        //Neutral
    };    
    public DataPackage(string ip, /*GameObject gobj,*/ string sceneName, float[] direction, DataState dataState/*/*, PlayerMovment playerMovment*//*, Vector3 spawnPoint*/)
    {
        IP = ip;
        //PlayerObject = gobj;
        CurrentScene = sceneName;
        PlayerDirection = direction;
        CurrentDataMode = dataState;
        //SpawnLocation = spawnPoint;
        //PlayerMovment = playerMovment;
    }
}
