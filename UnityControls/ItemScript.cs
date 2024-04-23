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
        if (item != null && collision != null)
        {
            ItemScript scr;
            if (collision.gameObject.transform.parent == null)
            {
                if (collision.gameObject.TryGetComponent<ItemScript>(out scr) && GCon.game.Items.ContainsKey(scr.item.Id))
                    item.OnCollisionEnter(GCon.game.Items[scr.item.Id]);
                else
                    item.OnCollisionEnter(new Block());
            }
            else
            {
                scr = collision.gameObject.GetComponentInParent<ItemScript>();
                if (scr != null && GCon.game.Items.ContainsKey(scr.item.Id))
                    item.OnCollisionEnter(GCon.game.Items[scr.item.Id]);
                else
                    item.OnCollisionEnter(new Block());
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (item != null && collision != null)
        {
            ItemScript scr;
            if (collision.gameObject.transform.parent == null)
            {
                if (collision.gameObject.TryGetComponent<ItemScript>(out scr) && GCon.game.Items.ContainsKey(scr.item.Id))
                    item.OnCollisionLeave(GCon.game.Items[scr.item.Id]);
                else
                    item.OnCollisionLeave(new Block());
            }
            else
            {
                scr = collision.gameObject.GetComponentInParent<ItemScript>();
                if (scr != null && GCon.game.Items.ContainsKey(scr.item.Id))
                    item.OnCollisionLeave(GCon.game.Items[scr.item.Id]);
                else
                    item.OnCollisionLeave(new Block());
            }
        }
    }
}
