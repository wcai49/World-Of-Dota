using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


/* TODO:
    1. Debug: why after using right click to rotate player, then left click to play around with camera causes a screen shake, TERRIBLE.
    2. Optimize: player movement code clean.
 */

public class PlayerControl : MonoBehaviour
{
    // player movement
    public float moveSpeed = 5f;
    public Vector2 moveInput = new Vector2(0f, 0f);
            
    // camera control
    public Vector2 mouseDrag = new Vector2(0f, 0f);
    public float mouseScrollY;
    public float mouseRotateVelocity = 1.0f;
    public float camDistance;
    public float leftMouse = 0;
    public float rightMouse = 0;
    [SerializeField] float sensitivity = 8.0f;
    [SerializeField] float maxCameraDistance = 100f;
    [SerializeField] float minCameraDistance = 5f;
    
    // sign components
    public CharacterController characterController;
    Animator playerAnimator;
    PlayerInputAction playerInputAction;
    Camera playerCamera;
   
    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        // bind WASD movement
        playerInputAction.Player.Move.performed += moveValue => moveInput = moveValue.ReadValue<Vector2>();
        playerInputAction.Player.Move.canceled += moveValue => moveInput = moveValue.ReadValue<Vector2>();
        // bind mouse scroll       
        playerInputAction.Player.Zoom.performed += scrollValue => mouseScrollY = scrollValue.ReadValue<float>();
        // bind mouse movement
        playerInputAction.Player.Look.performed += dragValue => mouseDrag = dragValue.ReadValue<Vector2>();
        playerInputAction.Player.Look.canceled += dragValue => mouseDrag = dragValue.ReadValue<Vector2>();
        // bind mouse left click
        playerInputAction.Player.Select.performed += leftMouseDown => leftMouse = leftMouseDown.ReadValue<float>();
        playerInputAction.Player.Select.canceled += leftMouseDown => leftMouse = leftMouseDown.ReadValue<float>();
        // bind mouse right click
        playerInputAction.Player.RightSelect.performed += rightMouseDown => rightMouse = rightMouseDown.ReadValue<float>();
        playerInputAction.Player.RightSelect.canceled += rightMouseDown => rightMouse = rightMouseDown.ReadValue<float>();
    }

    #region - Enable / Disable -
    private void OnEnable()
    {
        playerInputAction.Enable();
    }

    private void OnDisable()
    {
        playerInputAction.Disable();
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // assign gameobjects
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        playerCamera = GetComponentInChildren<Camera>();

        updateCameraDistance();
    }

    // Update is called once per frame
    void Update()
    {
        if(moveInput.magnitude != 0)
        {
            // player is moving backwards
            if (moveInput.y < 0)
            {
                playerAnimator.Play("walkBack");
                float backwardSlow = 0.3f;
                characterController.Move(-transform.forward * moveSpeed * backwardSlow * Time.deltaTime);

            }
            // player strafe left or right
            else if (moveInput.x != 0)
            {
                if (moveInput.x < 0)
                {
                    playerAnimator.Play("Left_Strafe");
                    characterController.Move(-transform.right * moveSpeed  * Time.deltaTime);
                }
                else
                {
                    playerAnimator.Play("Right_Strafe");
                    characterController.Move(transform.right * moveSpeed * Time.deltaTime);
                }

            }
            // player move forward
            else
            {
                playerAnimator.Play("walkFront");
                characterController.Move(transform.forward * moveSpeed  * Time.deltaTime);
            }
        }
        else
        {
            playerAnimator.Play("idle");
        }

        // camera zoom
        if (mouseScrollY != 0)
        {
            float distance = Vector3.Distance(transform.position, playerCamera.transform.position);
            
            if((distance > minCameraDistance && distance < maxCameraDistance) || (mouseScrollY < 0 && distance < maxCameraDistance) || (mouseScrollY > 0 && distance > minCameraDistance))
            {
                playerCamera.transform.position = Vector3.MoveTowards(playerCamera.transform.position, transform.position, mouseScrollY * sensitivity * Time.deltaTime);
                updateCameraDistance();
            }
        }

        // camera rotate around player
        if (leftMouse == 1 && mouseDrag.magnitude != 0)
        {
            Vector3 camDirection = new Vector3(mouseDrag.x, mouseDrag.y, 0);

            playerCamera.transform.position = new Vector3();
            playerCamera.transform.Rotate(new Vector3(1, 0, 0), -camDirection.y * mouseRotateVelocity);
            playerCamera.transform.Rotate(new Vector3(0, 1, 0), camDirection.x * mouseRotateVelocity, Space.World);
            playerCamera.transform.Translate(new Vector3(0, 0, -camDistance));
        }

        // use right click hold to turn left right
        if (rightMouse == 1 && mouseDrag.magnitude != 0)
        {
            Vector3 camDirection = new Vector3(mouseDrag.x, mouseDrag.y, 0);
            transform.Rotate(new Vector3(0, 1, 0), camDirection.x * mouseRotateVelocity);
        }
    }    

    public void updateCameraDistance()
    {
        camDistance = Vector3.Distance(transform.position, playerCamera.transform.position);
    }
}
