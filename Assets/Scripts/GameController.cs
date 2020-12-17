using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public SpawnPoints spawnPoints;
    public CinemachineVirtualCamera cinemachineCamera;
    public GameObject prefab;

    void Start()
    {
        var pos = spawnPoints.GetRandomPoint().position;
        var obj = PhotonNetwork.Instantiate(prefab.name, pos, Quaternion.identity);
        obj.AddComponent<PlayerMovement>();

        cinemachineCamera.Follow = obj.transform;
        cinemachineCamera.LookAt = obj.transform;
    }
}
