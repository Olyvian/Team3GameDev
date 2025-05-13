using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement2 : MonoBehaviour
{
    public CharacterController2D controller2D;
    float horizontalMove = 0f;
    public float runspeed = 40f;
    bool Jump = false;
    public Animator animator;
    bool Crouch = false;
    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal")*runspeed;
        animator.SetFloat("Speed",Mathf.Abs(horizontalMove));
        if (Input.GetButtonDown("Jump"))
        {
            Jump = true;
        }

        if (Input.GetButtonDown("Crouch")){
            Crouch = true;
        }else if (Input.GetButtonUp("Crouch"))
        {
            Crouch = false;
        }
    }

    void FixedUpdate()
    {
        controller2D.Move(horizontalMove*Time.fixedDeltaTime, Crouch, Jump);
        Jump = false;
    }
}
