using UnityEngine;
using System.Collections.Generic;
// In EnemyAI.cs
public class EnemyAi : MonoBehaviour {
private bool isStunned = false;
private float stunTimer = 0f;
public void Stun(float duration)
{
    isStunned = true;
    stunTimer = duration;
    // Potentially stop NavMeshAgent or other movement components here
}

void Update() // Or FixedUpdate for physics related movement
{
    if (isStunned)
    {
        stunTimer -= Time.deltaTime;
        if (stunTimer <= 0)
        {
            isStunned = false;
            // Resume AI behavior
        }
        return; // Don't execute normal AI logic while stunned
    }
}


    // ... your normal AI movement and attack logic ...
}