using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;

    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    float ySpeed;

    Quaternion targetRotation;

    CameraController camController;

    Animator animator;

    CharacterController characterController;

    private void Awake()
    {
        camController = Camera.main.GetComponent<CameraController> ();
        animator = GetComponent<Animator> ();
        characterController = GetComponent<CharacterController> ();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        var moveDirection = camController.PlanerRotation * moveInput;

        GroundCheck();
        Debug.Log(isGrounded);

        if(isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDirection * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDirection);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
    }

    private void LateUpdate()
    {
        Jump();
        Punch();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("isJumpingOver", true);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.V))
        {
            animator.SetBool("isJumping", true);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetBool("isJumpingSide", true);
        }
    }

    private void Punch()
    {
        if (isGrounded && Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isPunching", true);
        }
    }
}
