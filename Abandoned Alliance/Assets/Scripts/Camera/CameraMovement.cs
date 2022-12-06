using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] 
    private Camera _cam;

    [SerializeField]
    private SpriteRenderer _mapRenderer;

    private float _mapMinX, _mapMinY, _mapMaxX, _mapMaxY;
    private Vector3 _dragOrigin;

    private void Awake()
    {
        _mapMinX = _mapRenderer.transform.position.x - _mapRenderer.bounds.size.x / 2f;
        _mapMaxX = _mapRenderer.transform.position.x + _mapRenderer.bounds.size.x / 2f;

        _mapMinY = _mapRenderer.transform.position.y - _mapRenderer.bounds.size.y / 2f;
        _mapMaxY = _mapRenderer.transform.position.y + _mapRenderer.bounds.size.y / 2f;
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
            _dragOrigin = _cam.ScreenToWorldPoint(Input.mousePosition);
        }
        //calculate distance btween origin and new pos if the mouse button still held down.  
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = _dragOrigin - _cam.ScreenToWorldPoint(Input.mousePosition);
            //move cam by that distance
            _cam.transform.position = ClampCam(_cam.transform.position + difference);
        }
    }


    //Limit cam's movement so the cam does not move outside of the map
    private Vector3 ClampCam(Vector3 targetPos)
    {
        //takeing the cam width and height
        float camHeight = _cam.orthographicSize;
        float camWidth = _cam.orthographicSize * _cam.aspect;

        float minX = _mapMinX + camWidth;
        float maxX = _mapMaxX - camWidth;
        float minY = _mapMinY + camHeight;
        float maxY = _mapMaxY - camHeight;

        //clamp the X and Y position of the clamp
        float newX = Mathf.Clamp(targetPos.x, minX, maxX);
        float newY = Mathf.Clamp(targetPos.y, minY, maxY);

        return new Vector3(newX, newY, targetPos.z);
    }

}
