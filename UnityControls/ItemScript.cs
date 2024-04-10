using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public Item item;

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        item.OnCollisionEnter(GCon.game.Items[collision.gameObject.GetInstanceID()]);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        item.OnCollisionLeave(GCon.game.Items[collision.gameObject.GetInstanceID()]);
    }*/
    private void FixedUpdate()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int collisionId = collision.gameObject.GetComponent<ItemScript>().item.Id;
        if (GCon.game.Items.ContainsKey(collisionId))
            item.OnCollisionEnter(GCon.game.Items[collisionId]);
        else
            item.OnCollisionEnter(new Block());

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        int collisionId = collision.gameObject.GetComponent<ItemScript>().item.Id;
        if (GCon.game.Items.ContainsKey(collisionId))
            item.OnCollisionLeave(GCon.game.Items[collisionId]);
        else
            item.OnCollisionLeave(new Block());
    }
}
