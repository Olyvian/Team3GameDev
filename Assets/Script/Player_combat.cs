using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    public Animator animator;
    public GameObject weaponHitboxObject; // Assign your "WeaponHitbox" GameObject here

    public float attackRate = 2f; // Attacks per second
    private float nextAttackTime = 0f;

    void Start()
    {
        if (weaponHitboxObject != null)
        {
            weaponHitboxObject.SetActive(false); // Ensure it's disabled at start
        }
        else
        {
            Debug.LogError("WeaponHitbox GameObject not assigned to PlayerAttackWithHitbox script!");
        }
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) // Or your preferred attack button
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // 1. Play attack animation
        animator.SetTrigger("attack"); // "Attack" should be a Trigger parameter in your Animator
        // The animation itself will call EnableHitbox() and DisableHitbox() via events
    }

    // --- Animation Event Functions ---
    // These functions will be called by events in your attack animation clip.

    public void EnableAttackHitbox()
    {
        if (weaponHitboxObject != null)
        {
            weaponHitboxObject.SetActive(true);
            // The OnEnable() in WeaponHitbox.cs will clear the list of hit enemies.
        }
    }

    public void DisableAttackHitbox()
    {
        
            weaponHitboxObject.SetActive(false);
        
    }
}