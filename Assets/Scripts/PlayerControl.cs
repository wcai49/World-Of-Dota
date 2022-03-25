using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float idleTime = 10f;
    public float moveSpeed = 5f;
    public Vector2 moveInput = new Vector2(0f, 0f);
    private float idleTimer;
    public CharacterController characterController;
    Animator playerAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        idleTimer = idleTime;
    }

    // Update is called once per frame
    void Update()
    {
        // if the player idled too long , animator will play looking around animation instead.
        idleTimer -= Time.deltaTime;
        if(idleTimer < 0){
            playerAnimator.SetTrigger("IdleLong");
            idleTimer = idleTime;
        }

        Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if(direction.magnitude >= 0.1f){
            playerAnimator.SetBool("isMoving", true);
            
            characterController.Move(direction * moveSpeed * Time.deltaTime);
        }
        else
        {
            playerAnimator.SetBool("isMoving", false);
        }
    }

    public void playerMove(InputAction.CallbackContext context) {
        if(context.performed){
            moveInput = (context.ReadValue<Vector2>());
        }
        if(context.canceled)
        {
            moveInput = new Vector2(0f,0f);
        }
    }

    
}
