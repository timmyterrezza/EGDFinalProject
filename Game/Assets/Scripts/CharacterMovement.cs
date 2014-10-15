﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class CharacterMovement : MonoBehaviour
{
    public CameraControl cameraControl;

    [SerializeField] [Range(0.0f, 20.0f)] private float walkSpeed = 5.0f;
    [SerializeField] [Range(0.0f, 540.0f)] private float degRotPerSec = 200.0f;
    [SerializeField] [Range(0.0f, 100.0f)] private float gravity = 30.0f;
    // Speed is calculated to reach this height.
    [SerializeField] [Range(0.0f, 10.0f)] private float jumpHeight = 2.0f;
    // Extra time to become grounded before jumping.
    [SerializeField] [Range(0.0f, 0.5f)] private float jumpTimeout = 0.1f;
    public bool playerMoving;

    private CharacterController controller;
    private CollisionFlags collisionFlags;
    private float verticalSpeed;
    private float lastJumpTime;

    // Animation.
    private Animator animator;
    private int speedHash;
    private int strafeHash;

    private float originalGravity;
    private float originalJumpHeight;

    private bool grounded {
        get { return (collisionFlags & CollisionFlags.CollidedBelow) != 0; }
    }
    private float jumpVerticalSpeed {
        get { return Mathf.Sqrt(2 * jumpHeight * gravity); }
    }

    void Awake() {
        if (!cameraControl) {
            Debug.LogWarning("No camera control set on CharacterMovement!");
        }

        playerMoving = false;
        controller = gameObject.GetComponent<CharacterController>();
        verticalSpeed = 0.0f;
        lastJumpTime = -1000.0f;

        animator = gameObject.GetComponentInChildren<Animator>();
        speedHash = Animator.StringToHash("Speed");
        strafeHash = Animator.StringToHash("Strafe");

        originalGravity = gravity;
        originalJumpHeight = jumpHeight;
    }

    void Start() {

    }

    void Update() {
        bool inputReceived = false;
        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Vertical")) >= 0.01 ||
            Mathf.Abs(Input.GetAxisRaw("Strafe")) >= 0.01) {
            inputReceived = true;
        }

        if (inputReceived) {
            if (playerMoving) {
                // Rotate with horizontal input.
                float inputRotation = Input.GetAxis("Horizontal") *
                                      degRotPerSec * Time.deltaTime;
                Vector3 rotationEulerAngles = new Vector3(0.0f, inputRotation, 0.0f);
                transform.Rotate(rotationEulerAngles);
            } else {
                // Rotate to match the camera.
                float cameraRotation = cameraControl.cameraTransform.eulerAngles.y;
                Vector3 newEulerAngles = transform.eulerAngles;
                newEulerAngles.y = cameraRotation;
                transform.eulerAngles = newEulerAngles;
            }
        }

        Vector3 inputVector = transform.forward * Input.GetAxis("Vertical") +
                              transform.right * Input.GetAxis("Strafe");
        if (animator) {
            animator.SetFloat(speedHash, Mathf.Abs(Input.GetAxis("Vertical")));
            animator.SetFloat(strafeHash, Input.GetAxis("Strafe"));
        }

        ApplyGravity();
        ApplyJump();

        Vector3 motion = inputVector * walkSpeed + Vector3.up * verticalSpeed;
        motion *= Time.deltaTime;
        collisionFlags = controller.Move(motion);

        playerMoving = inputReceived;
    }

    void ApplyGravity() {
        if (grounded) {
            verticalSpeed = 0.0f;
        } else {
            verticalSpeed -= gravity * Time.deltaTime;
        }
    }

    void ApplyJump() {
        if (Input.GetButtonDown("Jump")) {
            lastJumpTime = Time.time;
        }
        // Timeout lets you trigger a jump slightly before landing.
        if (grounded && Time.time < lastJumpTime + jumpTimeout) {
            verticalSpeed = jumpVerticalSpeed;
        }
    }

    public void SetJumpHeight(float height) {
        jumpHeight = height;
    }

    public void ResetJumpHeight() {
        jumpHeight = originalJumpHeight;
    }

    public void SetGravity(float newGravity) {
        gravity = newGravity;
    }

    public void ResetGravity() {
        gravity = originalGravity;
    }
}