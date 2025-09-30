using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player & Gameplay")]
    public Transform player;
    public int startLives = 3;
    public float fallYDeath = -10f;

    [Header("UI")]
    public TMP_Text scoreTextTMP;
    public TMP_Text livesTextTMP;

    private int score;
    private int lives;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        lives = startLives;
        score = 0;
        RefreshUI();
    }

    void Update()
    {
        if (player != null && player.position.y < fallYDeath)
        {
            DamagePlayer(999); //drop from the ground, get dead directly
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        RefreshUI();
    }

    public void DamagePlayer(int amount)
    {
        lives -= amount;
        if (lives <= 0)
        {
            lives = 0;
            RefreshUI();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        RefreshUI();
    }

    private void RefreshUI()
    {
        string s = $"Score: {score}";
        string l = $"Lives: {lives}";
        if (scoreTextTMP != null) scoreTextTMP.text = s;
        if (livesTextTMP != null) livesTextTMP.text = l;
    }
}