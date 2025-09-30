using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BombDamage : MonoBehaviour
{
    public int damage = 1;
    public float knockbackForce = 6f;
    public float upForce = 2f;
    public float cooldown = 0.8f;
    public AudioClip impactClip;

    private float lastHitTime = -999f;

    void Reset()
    {
        gameObject.tag = "Bomb";
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (Time.time - lastHitTime < cooldown) return;
        lastHitTime = Time.time;

        if (GameManager.Instance != null)
            GameManager.Instance.DamagePlayer(damage);

        Rigidbody prb = collision.rigidbody;
        if (prb != null)
        {
            Vector3 dir = (collision.transform.position - transform.position).normalized;
            dir.y = 0f;
            Vector3 impulse = dir * knockbackForce + Vector3.up * upForce;
            // prb.AddForce(impulse, ForceMode.VelocityChange);
            prb.AddForce(impulse, ForceMode.Impulse);
        }
        
        // Play flash effect
        var flashComp = collision.transform.GetComponent<DamageFlash>();
        if (flashComp != null) flashComp.Flash();
        
        // Play sound
        if (impactClip != null)
            AudioSource.PlayClipAtPoint(impactClip, transform.position, 2.0f);
    }
}