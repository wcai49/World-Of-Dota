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
    [SerializeField] float cameraDistance;
    [SerializeField] float sensitivity = 10.0f;
    [SerializeField] float maxCameraDistance = 100f;
    [SerializeField] float minCameraDistance = 5f;

    // player attack
    private void Awake()
    {
        playerInputAction = new PlayerInputAction();

        playerInputAction.Player.Move.performed += moveValue => moveInput = moveValue.ReadValue<Vector2>();
        playerInputAction.Player.Move.canceled += moveValue => moveInput = moveValue.ReadValue<Vector2>();
       
        playerInputAction.Player.Zoom.performed += scrollValue => mouseScrollY = scrollValue.ReadValue<float>();
        
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
            }

        }

        // player attack
    }    
}
