using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Knockback : MonoBehaviourPunCallbacks
{
    [SerializeField] private float distance = 2f;
    [SerializeField] private ScriptableItem savedItem;
    [SerializeField] Vector3 point;
    [SerializeField] Vector2 dir;
   

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.layer = 11;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetAxisRaw("AttackHorizontal") != 0 || Input.GetAxisRaw("AttackVertical") != 0)
            {
                float horizontal = Input.GetAxisRaw("AttackHorizontal");
                float vertical = Input.GetAxisRaw("AttackVertical");

                Debug.DrawRay(transform.position, new Vector2(horizontal, vertical).normalized * distance, Color.red);

                int layermask = ~(LayerMask.GetMask("Player"));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(horizontal, vertical).normalized, distance, layermask);

                if (hit.collider != null)
                {
                    //Debug.Log(hit.transform.name);

                    if (hit.transform.tag == "Player")
                    {
                        float knockbackMult = savedItem ? savedItem.KnockbackMultiplier : 1;
                        photonView.RPC("RPC_Knockback", hit.transform.GetComponent<PhotonView>().Controller, hit.transform.GetComponent<PhotonView>().ViewID, new Vector2(horizontal, vertical), knockbackMult);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && savedItem)
            {
                point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dir = ((Vector2)point - (Vector2)transform.position).normalized;

                GameObject.FindGameObjectWithTag("ItemSpawner").GetComponent<ItemSpawner>().ThrowItem(savedItem, new Vector2(transform.position.x, transform.position.y) + (dir * 1), dir, 10);
                savedItem = null;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Item" && !collision.transform.GetComponent<Item>().doesKnockback)
        {
            savedItem = collision.transform.GetComponent<Item>().getWeapon();
            photonView.RPC("RPC_DestroyItem", RpcTarget.MasterClient, collision.transform.GetComponent<PhotonView>().ViewID);
        }
    }

    //knockback on F button | arrow button?

    [PunRPC]
    void RPC_Knockback(int id, Vector2 direction, float mult)
    {
        PhotonView.Find(id).GetComponent<PlayerMovement>().Knockback(direction, mult);
    }

    [PunRPC]
    void RPC_DestroyItem(int id)
    {
        GameObject itemObject = PhotonView.Find(id).gameObject;
        PhotonNetwork.Destroy(itemObject);
    }
}
