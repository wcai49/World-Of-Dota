using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    // player movement
    public float moveSpeed = 5f;
    public Vector2 moveInput = new Vector2(0f, 0f);
    public CharacterController characterController;
    Animator playerAnimator;
    PlayerInputAction playerInputAction;
    Camera playerCamera;
    //player camera control
    public float mouseScrollY;
    public float mouseRotateVelocity = 1.0f;
    public float camDistance;
    public Vector2 mouseDrag = new Vector2(0f, 0f);
    [SerializeField] float sensitivity = 8.0f;
    [SerializeField] float maxCameraDistance = 100f;
    [SerializeField] float minCameraDistance = 5f;
    public float leftMouse = 0;
    public Vector2 mousePosition = new Vector2(0f, 0f);
    private Vector3 previousCamPosition;

    // player attack
    private void Awake()
    {
        playerInputAction = new PlayerInputAction();

        playerInputAction.Player.Move.performed += moveValue => moveInput = moveValue.ReadValue<Vector2>();
        playerInputAction.Player.Move.canceled += moveValue => moveInput = moveValue.ReadValue<Vector2>();
       
        playerInputAction.Player.Zoom.performed += scrollValue => mouseScrollY = scrollValue.ReadValue<float>();
        
        playerInputAction.Player.Look.performed += dragValue => mouseDrag = dragValue.ReadValue<Vector2>();
        playerInputAction.Player.Look.canceled += dragValue => mouseDrag = dragValue.ReadValue<Vector2>();


        playerInputAction.Player.Fire.performed += leftMouseDown => leftMouse = leftMouseDown.ReadValue<float>();
        playerInputAction.Player.Fire.canceled += leftMouseDown => leftMouse = leftMouseDown.ReadValue<float>();

        playerInputAction.Player.MousePosition.performed += mouseCurrent => mousePosition = mouseCurrent.ReadValue<Vector2>();

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
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            if (direction.magnitude >= 0.1f)
            {
                float constant = 1.0f;
                // player is moving backwards
                if (moveInput.y < 0)
                {
                    playerAnimator.Play("walkBack");
                    constant = 0.3f;
                }
                // player strafe left or right
                else if (moveInput.x != 0)
                {
                    if (moveInput.x < 0)
                    {
                        playerAnimator.Play("Left_Strafe");
                    }
                    else
                    {
                        playerAnimator.Play("Right_Strafe");
                    }

                }
                // player move forward
                else
                {
                    playerAnimator.Play("walkFront");
                }

                characterController.Move(direction * moveSpeed * constant * Time.deltaTime);
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

            if ((mouseScrollY < 0 && distance > maxCameraDistance) || (mouseScrollY > 0 && distance < minCameraDistance))
            {

            }
            else
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

        // player attack
    }    

    public void updateCameraDistance()
    {
        camDistance = Vector3.Distance(transform.position, playerCamera.transform.position);
    }
}
