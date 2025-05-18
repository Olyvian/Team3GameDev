using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    public int attackDamage = 40;
    public LayerMask enemyLayers;
    public float knockbackForce = 10f; // Strength of the knockback
    public float knockbackDuration = 0.2f; // How long the enemy might be stunned/unable to act (optional)

    // It's good to have a reference to the origin of the attack for direction
    // This could be the player's transform or a specific point on the player
    public Transform attackOriginTransform;

    private List<Collider2D> hitEnemiesThisSwing;

    void OnEnable()
    {
        if (hitEnemiesThisSwing == null)
        {
            hitEnemiesThisSwing = new List<Collider2D>();
        }
        hitEnemiesThisSwing.Clear();

        // If attackOriginTransform is not set, try to get the root transform (likely the player)
        if (attackOriginTransform == null)
        {
            attackOriginTransform = transform.root; // Assumes hitbox is child of player
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Enemy_Health enemyHealth = other.GetComponent<Enemy_Health>();
        if (enemyHealth != null)
        {
            if (attackOriginTransform == null) // Ensure attackOriginTransform is set
            {
                attackOriginTransform = transform.root; // Or log an error if it must be manually set
                if (attackOriginTransform == null) Debug.LogError("Attack Origin Transform not set and could not be found!"); return;
            }

            Vector2 knockbackDir = (other.transform.position - attackOriginTransform.position).normalized;

            // Call the modified TakeDamage method on the enemy
            enemyHealth.TakeDamage(attackDamage, knockbackDir, knockbackForce, knockbackDuration);
        }
        if ((enemyLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            if (hitEnemiesThisSwing.Contains(other))
            {
                return;
            }
            hitEnemiesThisSwing.Add(other);

            Debug.Log("Weapon hit: " + other.name);
            if (enemyHealth != null)
            {
                // Apply damage first
                Vector2 knockbackDir = (other.transform.position - attackOriginTransform.position).normalized;

                // Call the modified TakeDamage method on the enemy
                enemyHealth.TakeDamage(attackDamage, knockbackDir, knockbackForce, knockbackDuration);

                // --- Apply Knockback ---
                Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
                if (enemyRb != null && attackOriginTransform != null)
                {
                    // Ensure the Rigidbody is Dynamic to receive forces properly
                    if (enemyRb.bodyType == RigidbodyType2D.Dynamic)
                    {
                        // Calculate direction away from the attack origin
                        Vector2 knockbackDirection = (other.transform.position - attackOriginTransform.position).normalized;

                        // Apply the force
                        enemyRb.velocity = Vector2.zero; // Optional: Stop current enemy movement before knockback
                        enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                        // Optional: If you have an EnemyAI script, you might want to tell it to pause
                        // EnemyAi enemyAI = other.GetComponent<EnemyAi>(); // Assuming you have an EnemyAI script
                        // if (enemyAI != null)
                        // {
                        //     enemyAI.Stun(knockbackDuration); // A new method you'd create in EnemyAI
                        // }
                    }
                    else
                    {
                        Debug.LogWarning(other.name + " is not Dynamic. Cannot apply physics-based knockback. Current type: " + enemyRb.bodyType);
                    }
                }
                else
                {
                    if (enemyRb == null) Debug.LogWarning(other.name + " is missing a Rigidbody2D for knockback.");
                    if (attackOriginTransform == null) Debug.LogWarning("Attack Origin Transform not set on WeaponHitbox for knockback direction.");
                }
            }
        }
    }
    
}