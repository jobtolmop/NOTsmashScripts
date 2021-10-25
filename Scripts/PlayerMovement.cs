using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    public GameManager gm;

    private bool resetKnockback = true;
    private bool hasKnockback = false;
    private int hitCount = 0;

    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Variables")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float maxMoveSpeed;
    [SerializeField] private float linearDrag;
    private float horizontalDirection;
    private bool changingDirection => (rb.velocity.x > 0f && horizontalDirection < 0f) || (rb.velocity.x < 0f && horizontalDirection > 0f);

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float airLinearDrag = 2.5f;
    [SerializeField] private float fallMultiplier = 8f;
    [SerializeField] private float lowJumpFallMultiplier = 5f;

    [Header("Ground Collision Variables")]
    [SerializeField] private float groundRaycastLength;
    [SerializeField] private Vector3 groundRaycastOffset;
    private bool onGround;
    private bool canJump => Input.GetButtonDown("Jump") && onGround;

    void Awake()
    {
        if (!photonView.IsMine)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<PlayerMovement>().enabled = false;
        }
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            horizontalDirection = GetInput().x;
            if (canJump)
            {
                Jump();
                Debug.Log("Jump");
            }

            if (transform.position.y <= -7)
            {
                Debug.Log("aaaaa");
                photonView.RPC("RPC_restartLevel", RpcTarget.All, gm.getSpawnLocations().position);
            }

            if (resetKnockback)
            {
                CheckCollisions();
            }
        }
    }

    private void FixedUpdate()
    {
        //CheckCollisions();
        MoveCharacter();
        ApplyLinearDrag();

        if (onGround)
        {
            ApplyLinearDrag();
        }
        else
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void MoveCharacter()
    {
        rb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(rb.velocity.x) > maxMoveSpeed && !hasKnockback)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxMoveSpeed, rb.velocity.y);
        }
    }

    private void ApplyLinearDrag()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f || changingDirection)
        {
            rb.drag = linearDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = airLinearDrag;
    }

    private void FallMultiplier()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckCollisions()
    {
        onGround = Physics2D.Raycast(transform.position + groundRaycastOffset, Vector2.down, groundRaycastLength, groundLayer);

        if (hasKnockback)
        {
            hasKnockback = false;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundRaycastOffset, transform.position + groundRaycastOffset + Vector3.down * groundRaycastLength);
    }

    public void Knockback(Vector2 direction, float mult)
    {
        //Debug.Log(direction);

        hasKnockback = true;
        StartCoroutine(disableCollisionCheck());
        hitCount++;
        direction += (Vector2.up * 0.25f);
        rb.AddForce((direction * (1 + (0.1f * hitCount))* mult), ForceMode2D.Impulse); 

    }

    public void ItemKnockback(Vector2 EnemyPos, float ItemMult = 1)
    {
        hasKnockback = true;
        StartCoroutine(disableCollisionCheck());
        Vector2 dir = ((Vector2)transform.position - EnemyPos).normalized;
        Debug.DrawRay(transform.position, dir * 5, Color.red);
        Vector2 SideForce = dir * 24;
        Vector2 UpwardForce = Vector2.up * 12;
        float KnockbackMult = 1 + (0.25f * hitCount);
        Vector2 Knockback = ((SideForce + UpwardForce) * KnockbackMult) * ItemMult;
        Debug.DrawRay(transform.position, Knockback, Color.yellow);
        hitCount++;
        rb.AddForce(Knockback, ForceMode2D.Impulse);
    }

    private IEnumerator disableCollisionCheck()
    {
        resetKnockback = false;
        yield return new WaitForSeconds(0.1f);
        resetKnockback = true;
    }

    [PunRPC]
    void RPC_restartLevel(Vector3 t)
    {       
        transform.position = t;       
    }
}
