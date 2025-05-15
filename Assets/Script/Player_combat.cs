using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_combat : MonoBehaviour
{

    public Animator animator;
    // Update is called once per frame
    void Start() {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            attack();
        }
    }

    void attack(){
        animator.SetTrigger("attack");
    }
}
