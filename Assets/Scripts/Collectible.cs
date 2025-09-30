using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectible : MonoBehaviour
{
    public int scoreValue = 1;
    public AudioClip pickupClip; // coin collection audio
    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
        gameObject.tag = "Collectible";
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        if (pickupClip != null)
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, 1.0f);

        Destroy(gameObject);
    }
}