using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float idleTime = 10f;

    private float idleTimer;
    Animator playerAnimator;
    // Start is called before the first frame update
    void Start()
    {
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
    }
}
