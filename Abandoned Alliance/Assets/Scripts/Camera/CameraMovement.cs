using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] 
    private Camera cam;

    [SerializeField]
    private SpriteRenderer mapRenderer;

    private float mapMinX, mapMinY, mapMaxX, mapMaxY;
    private Vector3 dragOrigin;

    private void Awake()
    {
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    private void LateUpdate()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        //save mouse pos in world space and when drag starts
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate distance btween origin and new pos if the mouse button still held down.  
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            //move cam by that distance
            cam.transform.position = ClampCam(cam.transform.position + difference);
        }
    }


    //Limit cam's movement so the cam does not move outside of the map
    private Vector3 ClampCam(Vector3 targetPos)
    {
        //takeing the cam width and height
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        //clamp the X and Y position of the clamp
        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(newX, newY, targetPos.z);
    }

}
