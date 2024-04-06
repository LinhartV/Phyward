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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GCon.game.Items.ContainsKey(collision.gameObject.GetInstanceID()))
            item.OnCollisionEnter(GCon.game.Items[collision.gameObject.GetInstanceID()]);
        else
            item.OnCollisionEnter(new Block());

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GCon.game.Items.ContainsKey(collision.gameObject.GetInstanceID()))
            item.OnCollisionLeave(GCon.game.Items[collision.gameObject.GetInstanceID()]);
        else
            item.OnCollisionLeave(new Block());
    }
}
