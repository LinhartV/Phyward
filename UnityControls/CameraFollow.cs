using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//let camera follow target
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private RenderTexture _cachedRenderTexture;

    /* Vector3 initialVectorBottomLeft;
     Vector3 initialVectorTopRight;

     Vector3 UpdatedVectorBottomLeft;
     Vector3 UpdatedVectorTopRight;*/


    private void Start()
    {
    }

    private void Update()
    {
        if (target != null)
        {
            /*UpdatedVectorBottomLeft = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 10)) - target.position;
            UpdatedVectorTopRight = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10)) - target.position;
            if (initialVectorBottomLeft == null)
            {
                initialVectorBottomLeft = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 10)) - target.position;
                initialVectorTopRight = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10)) - target.position;
            }
            if ((initialVectorBottomLeft != UpdatedVectorBottomLeft) || (initialVectorTopRight != UpdatedVectorTopRight))
            {
                initialVectorBottomLeft = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(0, 0, 10));
                initialVectorTopRight = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 10));
                Debug.Log("Screen Size has changed");
                ToolsUI.OnResize();
            }*/
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
    private void OnRenderImage(RenderTexture src, RenderTexture destination)
    {
        if (_cachedRenderTexture == null)
        {
            _cachedRenderTexture = new RenderTexture(src.width, src.height, src.depth);
        }
        if (GCon.freezeCamera)
        {
            Graphics.Blit(_cachedRenderTexture, destination);
        }
        else
        {
            Graphics.CopyTexture(src, _cachedRenderTexture);
            Graphics.Blit(src, destination);
        }
    }
}

