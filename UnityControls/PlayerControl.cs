using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for controlling player
/// </summary>
public class PlayerControl : MonoBehaviour
{
    public LayerMask solidLayer;
    public Rigidbody2D rb;
    Vector2 finalMovement;
    public float raycastOffset = 0.55f;
    void Start()
    {

    }

    void Update()
    {
        //Movement control
        finalMovement = Vector2.zero;
        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && Physics2D.Raycast(transform.position, Vector2.left, raycastOffset, solidLayer).collider == null)
        {
            finalMovement.x = -1;
        }
        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Physics2D.Raycast(transform.position, Vector2.right, raycastOffset, solidLayer).collider == null)
        {
            finalMovement.x = 1;
        }
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Physics2D.Raycast(transform.position, Vector2.up, raycastOffset, solidLayer).collider == null)
        {
            finalMovement.y = 1;
        }
        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Physics2D.Raycast(transform.position, Vector2.down, raycastOffset, solidLayer).collider == null)
        {
            finalMovement.y = -1;
        }
        //Rotate to face pointer
        /*var mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        this.GetComponentInChildren<SpriteRenderer>().transform.up = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);*/
        //Shooting
        if (Input.GetMouseButton(0))
        {
            GCon.game.Player.Weapon.Fire();
        }
    }
    /*private void FixedUpdate()
    {
        if (finalMovement != Vector2.zero)
        {
            finalMovement = finalMovement.normalized;
            rb.MovePosition(rb.position + finalMovement * movementSpeed * Time.fixedDeltaTime);
        }
    }*/

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {

        }
    }*/
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }*/
    /*private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null && collision.collider.tag == "Ground")
        {
            collidedWithWall = false;
            Debug.Log("not touching");
        }
    }*/
}
