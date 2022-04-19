using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -10f;

    [Space(10)]
    [Range(0, 0.3f)]
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float jumpTimeout = 0.5f;
    [SerializeField] private float fallTimeout = 0.15f;

    [Space(10)]
    [Range(0, 0.5f)]
    [SerializeField] private float changeSpeedRate;

    [Header("Player Ground")]
    [SerializeField] private bool grounded = true;
    [SerializeField] private float groundOffset;
    [SerializeField] private float groundRadius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Energy")]
    [SerializeField] private float sprintEnergy;
    [SerializeField] private float chargeEnergy;

    [SerializeField] private bool gizmos;

    // Player
    private float speed;
    private float targetRotation;
    private float rotationVelocity;
    private float verticalVelocity;

    // Timeout deltatime
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // Animation
    float speedAnim = 0f;

    // Ground
    private Vector3 groundOffsetVector;

    private CharacterController _controller;
    private Camera _mainCamera;
    private InputSetting _input;


    private void Awake()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _controller = GetComponent<CharacterController>();
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;

    }

    private void OnEnable()
    {
        EventDispatcher.OutOfEnergyEvent += StopSprint;
    }

    private void OnDisable()
    {
        EventDispatcher.OutOfEnergyEvent -= StopSprint;
    }

    private void Start()
    {
        _input = InputSetting.instance;
    }

    private void Update()
    {
        GroundedCheck();
        JumpAndGravity();
        Movement();

        MoveAnimation();
    }

    private void StopSprint()
    {
        _input.sprint = false;
    }

    private void Movement()
    {
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;
        if (_input.move == Vector2.zero)
        {
            targetSpeed = 0f;
        }
        float currentHorizonSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizonSpeed < targetSpeed - speedOffset || currentHorizonSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizonSpeed, targetSpeed * _input.move.magnitude, Time.deltaTime * acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
            speed = targetSpeed;

        if (_input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(_input.move.x, _input.move.y) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
        Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
    }

    private void GroundedCheck()
    {
        groundOffsetVector = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(groundOffsetVector, groundRadius, groundLayer);
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (_input.jump && jumpTimeoutDelta <= 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }
        }

        if (verticalVelocity < 50f)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void MoveAnimation()
    {
        if (_input.move == Vector2.zero)
        {
            EventDispatcher.ChangedEnergy(chargeEnergy, true);
            speedAnim = Mathf.Lerp(speedAnim, 0f, changeSpeedRate * 2f);
        }
        else
        {
            if (_input.sprint)
            {
                speedAnim = Mathf.Lerp(speedAnim, 2f, changeSpeedRate);
                EventDispatcher.ChangedEnergy(sprintEnergy, false);
            }
            else
            {
                EventDispatcher.ChangedEnergy(chargeEnergy, true);
                speedAnim = Mathf.Lerp(speedAnim, 1f, changeSpeedRate);
            }
        }
        EventDispatcher.Move(speedAnim);
    }

    private void OnDrawGizmos()
    {
        if (gizmos)
        {
            Color trueColor = Color.red;
            Color falseColor = Color.green;

            if (grounded)
                Gizmos.color = trueColor;
            else
                Gizmos.color = falseColor;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), groundRadius);
        }
    }
}
