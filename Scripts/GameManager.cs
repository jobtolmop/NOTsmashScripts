using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;
    public List<GameObject> spawnPoints;

    void Start()
    {
        int index = System.Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);
        GameObject item = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[index].transform.position, Quaternion.identity);
        item.GetComponent<PlayerMovement>().gm = this;
    }

    public Transform getSpawnLocations()
    {
        int index = System.Array.IndexOf(PhotonNetwork.PlayerList, PhotonNetwork.LocalPlayer);
        return spawnPoints[index].transform;  
    }


    void Update()
    {

    }
}
