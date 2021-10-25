using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Item : MonoBehaviourPunCallbacks
{
    public ScriptableItem weapon;
    public bool doesKnockback = false;

    public ScriptableItem getWeapon()
    {
        return weapon;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (collision.transform.tag == "Player")
            {
                if (doesKnockback)
                {
                    photonView.RPC("RPC_KnocksBack", collision.transform.GetComponent<PhotonView>().Controller, collision.transform.GetComponent<PhotonView>().ViewID);
                    PhotonNetwork.Destroy(gameObject);
                }
            }
        }
    }

    [PunRPC]
    void RPC_EnableKnockback()
    {
        doesKnockback = true;
    }

    [PunRPC]
    void RPC_KnocksBack(int id)
    {
       PhotonView.Find(id).GetComponent<PlayerMovement>().ItemKnockback ((Vector2)transform.position, weapon.KnockbackMultiplier * 2);
    }
}