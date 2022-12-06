using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _mapRenderer;

    [SerializeField]
    private Camera _cam;

    private float _mapMinX, _mapMinY, _mapMaxX, _mapMaxY;

    public float panSpeed = 20f;
    public float panBorderThickness = 10f;


    private void Awake()
    {
        _mapMinX = _mapRenderer.transform.position.x - _mapRenderer.bounds.size.x / 2f;
        _mapMaxX = _mapRenderer.transform.position.x + _mapRenderer.bounds.size.x / 2f;

        _mapMinY = _mapRenderer.transform.position.y - _mapRenderer.bounds.size.y / 2f;
        _mapMaxY = _mapRenderer.transform.position.y + _mapRenderer.bounds.size.y / 2f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        transform.position = ClampCam(pos);
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
