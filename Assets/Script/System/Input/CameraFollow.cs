using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform follow;
    [SerializeField] float smooth;

    [Header("Rotate")]
    [SerializeField] float rotateSpeed;

    [Header("Zoom")]
    [SerializeField] float maxOffsetY;
    [SerializeField] float minOffsetY;
    [SerializeField] float offsetY;
    [SerializeField] float zoomSpeed;

    [Header("Compass")]
    [SerializeField] GameObject needleCompassUI;
    [SerializeField] GameObject needleCompassMap;

    private InputSetting _input;
    private float angle;
    private float offsetX;
    private float offsetZ;

    private void Awake()
    {
        offsetY = maxOffsetY;
        angle = 180f;
    }

    private void Start()
    {
        _input = InputSetting.instance;
    }

    private void LateUpdate()
    {
        Follow();
        RotateCamera();
        ZoomCamera();
    }

    private void RotateCamera()
    {
        if (_input.rotateLeft)
        {
            angle += Time.deltaTime * rotateSpeed;
        }
        if (_input.rotateRight)
        {
            angle -= Time.deltaTime * rotateSpeed;
        }
        offsetX = (offsetY / 2f) * Mathf.Sqrt(2) * Mathf.Sin(angle);
        offsetZ = (offsetY / 2f) * Mathf.Sqrt(2) * Mathf.Cos(angle);
        needleCompassUI.transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.y);
        needleCompassMap.transform.eulerAngles = new Vector3(0, 0, -transform.eulerAngles.y);
    }

    private void ZoomCamera()
    {
        if (_input.zoom != 0)
        {
            if (_input.zoom == 1)
            {
                offsetY += Time.deltaTime * zoomSpeed;
                if (offsetY >= maxOffsetY)
                    offsetY = maxOffsetY;
            }
            if (_input.zoom == -1)
            {
                offsetY -= Time.deltaTime * zoomSpeed;
                if (offsetY <= minOffsetY)
                    offsetY = minOffsetY;
            }
        }
    }

    public void Follow()
    {
        transform.position = Vector3.Lerp(transform.position, follow.position + new Vector3(offsetX, offsetY, offsetZ), smooth);
        transform.LookAt(follow);
    }
}
