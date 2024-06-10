using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIScript : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public UIItem item;
    private bool isHovered = false;
    private bool isClicked = false;


    /*private void OnMouseEnter()
    {
    }
    private void OnMouseExit()
    {
    }
    private void OnMouseDown()
    {
    }
    private void OnMouseUp()
    {
    }*/

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isClicked)
        {
            item?.OnMouseDownDefault();
            isClicked = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovered)
        {
            item?.OnMouseEnterDefault();
            isHovered = true;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isHovered)
        {
            item?.OnMouseExitDefault();
            isHovered= false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isClicked)
        {
            item?.OnMouseUpDefault();
            isClicked = false;
        }
    }
}
