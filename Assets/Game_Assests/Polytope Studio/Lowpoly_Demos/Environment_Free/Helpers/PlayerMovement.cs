using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform playerObj;   // The visual representation of the player
    public Transform cameraTransform; // Assign your main camera's Transform here
    public float rotationSpeed = 10f;

    private bool isMoving = false;
    private Vector3 currentMoveDirection = Vector3.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure the cameraTransform is assigned
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform not assigned in PlayerMovement script!");
            enabled = false;
        }
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (inputDirection != Vector3.zero)
        {
            isMoving = true;
            currentMoveDirection = inputDirection;
            RotateBasedOnInput(currentMoveDirection);
        }
        else
        {
            isMoving = false;
        }
    }

    private void RotateBasedOnInput(Vector3 moveDir)
    {
        if (cameraTransform != null && moveDir != Vector3.zero)
        {
            // Determine the target rotation based on input relative to camera
            Vector3 cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();
            Vector3 cameraRight = cameraTransform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();

            Vector3 targetForward = (cameraForward * moveDir.z + cameraRight * moveDir.x).normalized;

            if (targetForward != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetForward);

                // Smoothly interpolate towards the target rotation
                playerObj.rotation = Quaternion.Slerp(playerObj.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}