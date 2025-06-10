using UnityEngine;
using UnityEngine.UIElements;

public class Movment_Basic : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float groundDrag = 5f;
    public float jumpForce = 7f;
    public float jumpCooldown = 0.25f;
    public float airMultiplier = 0.4f;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    bool grounded;
    public Rigidbody rb;
    public Transform cameraTransform;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public bool isOnGround = true;

    public Animator animator;
    private void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;

        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform not assigned in Movment_Basic script!");
            enabled = false;
        }
    }

    private void Update()
    {
        Debug.Log("readyToJump: " + readyToJump + "grounded: " + grounded);
        Debug.DrawRay(transform.position, Vector3.down * 0.2f, Color.green);
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && isOnGround==true)
        {
            Debug.Log("Jump");
            Jump();
            isOnGround=false;
        }
        if (horizontalInput != 0 || verticalInput != 0)
        {

            animator.SetFloat("Speed", 1);
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
        

    }

    private void MovePlayer()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f; 
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
        moveDirection.Normalize(); 


        if (grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

}