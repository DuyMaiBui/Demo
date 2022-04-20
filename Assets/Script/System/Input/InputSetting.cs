using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputSetting : MonoBehaviour
{
    public static InputSetting instance;

    [Header("Player")]
    public Vector2 move;
    public bool jump;
    public bool sprint;
    public bool rotateRight;
    public bool rotateLeft;
    public bool interract;

    [Header("Mouse")]
    public Vector2 position;
    public float zoom;
    public bool leftClick;
    public bool rightClick;

    [Header("UI")]
    public bool openTab;
    public bool openUI;
    public bool openMenu;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }
        openUI = false;
    }

    //
    public void OnMove(InputValue value)
    {
        if (!openUI)
            move = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!openUI)
            jump = value.isPressed;
    }

    public void OnInterract(InputValue value)
    {
        if (!openUI)
            interract = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        if (!openUI)
            sprint = value.isPressed;
    }

    public void OnRotateRight(InputValue value)
    {
        if (!openUI)
            rotateRight = value.isPressed;
    }

    public void OnRotateLeft(InputValue value)
    {
        if (!openUI)
            rotateLeft = value.isPressed;
    }

    public void OnLeftClick(InputValue value)
    {
        if (!openUI)
            leftClick = value.isPressed;
    }

    public void OnRightClick(InputValue value)
    {
        if (openUI)
            rightClick = value.isPressed;
    }

    public void OnPosition(InputValue value)
    {
        position = value.Get<Vector2>();
    }

    public void OnOpenTab(InputValue value)
    {
        openTab = value.isPressed;
    }

    public void OnMenu(InputValue value)
    {
        openMenu = value.isPressed;
    }

    public void OnZoom(InputValue value)
    {
        if (!openUI)
            zoom = value.Get<float>();
    }
}
