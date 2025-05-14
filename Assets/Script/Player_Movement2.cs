using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement2 : MonoBehaviour
{
    public CharacterController2D controller2D;
    float horizontalMove = 0f;
    public float runspeed = 40f;
    public Rigidbody2D rb;
    bool Jump = false;
    public Animator animator;
    bool Crouch = false;
    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal")*runspeed;
        animator.SetFloat("yVelocity",rb.velocity.y);
        animator.SetFloat("Speed",Mathf.Abs(horizontalMove));
        if(rb.velocity.y < -0.1){
        animator.SetBool("isGround",false);
        }
        if (Input.GetButtonDown("Jump"))
        {
            Jump = true;
            animator.SetBool("isJumping",true);
        }

        if (Input.GetButtonDown("Crouch")){
            Crouch = true;
        }else if (Input.GetButtonUp("Crouch"))
        {
            Crouch = false;
        }
    }
    public void onLanding(){
        animator.SetBool("isJumping",false);
        animator.SetBool("isGround",true);
    }

    void FixedUpdate()
    {
        controller2D.Move(horizontalMove*Time.fixedDeltaTime, Crouch, Jump);
        Jump = false;
    }
}
