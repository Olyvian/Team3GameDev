using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public Rigidbody2D rb; // Assign in Inspector or get in Start/Awake
    public Collider2D collider2d;
    public Animator animator;
    // You might also want particle effects/sound for being hit
    // public GameObject hitEffectPrefab;
    // public AudioClip hitSound;

    void Awake()
    {
        currentHealth = maxHealth;
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " is missing a Rigidbody2D, knockback will not work!");
        }
        else if (rb.bodyType != RigidbodyType2D.Dynamic)
        {
            Debug.LogWarning(gameObject.name + "'s Rigidbody2D is not set to Dynamic. Physics-based knockback may not work as expected. Current type: " + rb.bodyType);
        }
    }

    // Modified TakeDamage to accept knockback parameters
    public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce, float knockbackDuration = 0.2f)
    {
        animator.SetTrigger("isHit");
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage. Health: " + currentHealth);

        // Optional: Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        // Optional: AudioSource.PlayClipAtPoint(hitSound, transform.position);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Apply Knockback if alive and Rigidbody is present and Dynamic
            if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            {
                rb.velocity = Vector2.zero; // Optional: Stop current movement
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                // Optional: Trigger stun in an AI component
                EnemyAi enemyAI = GetComponent<EnemyAi>();
                if (enemyAI != null)
                {
                    enemyAI.Stun(knockbackDuration);
                }
            }
        }
    }

    void Die()
    {
        Debug.Log(transform.name + " died.");
        animator.SetBool("isDead", true);
    }
    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}