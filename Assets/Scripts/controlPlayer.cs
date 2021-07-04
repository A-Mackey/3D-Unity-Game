using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlPlayer : MonoBehaviour
{
    //Variables
    [SerializeField] Transform playerCamera = null;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float walkSpeed = 3.5f;
    [SerializeField][Range(0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField][Range(0f, 0.5f)] float mouseSmoothTime = 0.5f;
    [SerializeField] int numberOfJumps = 1;

    //Force Variables
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10f;

    //Toggleables
    [SerializeField] bool lockCursor = true;
    [SerializeField] bool moveInAir = true;

    //Used for math
    float cameraPitch = 0f;
    float velocityY = 0f;
    CharacterController controller = null;
    
    //For Jumping
    int currentJumps = 0;
    bool isJumping = false;

    //Vectors used for camera & movement smoothing
    Vector2 currentDir = Vector2.zero;
    Vector2 currentDirVelocity = Vector2.zero;

    Vector2 currentMouseDelta = Vector2.zero;
    Vector2 currentMouseDeltaVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if(lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
    }

    void UpdateMouseLook() {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraPitch -= currentMouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        playerCamera.localEulerAngles = Vector3.right * cameraPitch;

        transform.Rotate(Vector3.up * targetMouseDelta.x * mouseSensitivity);
    }

    void UpdateMovement() {
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        //Check if on the ground
        if (controller.isGrounded) {
            velocityY = 0.0f;
            currentJumps = 0;
        }
        //Increase accelleration downwards
        velocityY += gravity * Time.deltaTime;

        //Jumping
        if (Input.GetButtonDown("Jump") && (controller.isGrounded || (currentJumps < numberOfJumps))) {
            velocityY += jumpForce;
            currentJumps++;
            print("Jump: " + currentJumps);
        }

        //Player's current velocity
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * walkSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);
    }
}
