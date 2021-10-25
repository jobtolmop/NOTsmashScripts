using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ItemSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private int activeItems;
    [SerializeField] private List<ScriptableItem> scriptableItems;

    void Start()
    {
        StartCoroutine(spawnTimer());
    }

    void Update()
    {

    }

    private IEnumerator spawnTimer()
    {
        float duration = 10f;
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            //Debug.Log(normalizedTime);
            yield return null;
        }

        if (activeItems < 2 && PhotonNetwork.IsMasterClient)
        {
            activeItems++;

            GameObject temp = PhotonNetwork.Instantiate(itemPrefab.name, this.transform.position, Quaternion.identity);

            int id = temp.GetComponent<PhotonView>().ViewID;
            //Debug.Log(id);
            //Debug.Log(scriptableItems[0]);
            //Debug.Log(temp);
            Debug.Log(Random.RandomRange(0, scriptableItems.Count - 1));
            photonView.RPC("RPC_AssignWeapon", RpcTarget.All, id, Random.RandomRange(0, scriptableItems.Count - 1));
        }

        StartCoroutine(spawnTimer());
    }

    [PunRPC]
    void RPC_AssignWeapon(int id, int weaponId)
    {
        PhotonView.Find(id).gameObject.GetComponent<Item>().weapon = scriptableItems[weaponId];      
    }

    [PunRPC]
    void RPC_ThrowWeapon(int index, Vector2 pos, Vector2 dir, float mult)
    {
        GameObject temp = PhotonNetwork.Instantiate(itemPrefab.name, pos, Quaternion.identity);
        int id = temp.GetComponent<PhotonView>().ViewID;
        photonView.RPC("RPC_AssignWeapon", RpcTarget.All, id, index);
        temp.GetComponent<Rigidbody2D>().AddForce(dir * mult, ForceMode2D.Impulse);

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            foreach(GameObject item in GameObject.FindGameObjectsWithTag("Item"))
            {
                int index = scriptableItems.FindIndex(x => x == item.GetComponent<Item>().weapon);
                photonView.RPC("RPC_AssignWeapon", newPlayer, item.GetComponent<PhotonView>().ViewID, index);
            }
        }
    }

    public void ThrowItem(ScriptableItem item, Vector2 pos, Vector2 dir, float mult)
    {
        int index = scriptableItems.FindIndex(x => x == item);
        photonView.RPC("RPC_ThrowWeapon", RpcTarget.MasterClient, index, pos, dir, mult);
    }
}
