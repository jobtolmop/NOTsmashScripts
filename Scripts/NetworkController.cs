using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{

    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        Connect();
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("We are now connected to the " + PhotonNetwork.CloudRegion + " server!");
    //}

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public void Play()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a room and failed");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("joined a room!");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer);
    }


    void Update()
    {

    }


}
