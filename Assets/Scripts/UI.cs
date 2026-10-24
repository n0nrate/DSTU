using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public Slider healthBar;
    public TMP_Text healthText;
    public TMP_Text enemiesText;
    public TMP_Text resultText;
    public WaveManager waveManager;
    public TMP_Text waveText;

    private bool gameFinished = false;

    void Start()
    {
        healthBar.maxValue = playerHealth.maxHealth;
        resultText.gameObject.SetActive(false);
    }

    void Update()
    {
        healthBar.value = playerHealth.currentHealth;

        healthText.text = "HP: " +
            playerHealth.currentHealth.ToString("0") +
            " / " +
            playerHealth.maxHealth.ToString("0");

        int enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        enemiesText.text = "Враги: " + enemies;

        if (!gameFinished)
        {
            if (playerHealth.IsDead())
            {
                FinishGame("ПОРАЖЕНИЕ");
            }
            else if (waveManager != null && waveManager.GameWon)
            {
                FinishGame("ПОБЕДА");
            }

        if (waveManager != null && waveText != null)
        {
           waveText.text = "Волна: " + waveManager.currentWave;
        }
        }
    }

    
    void FinishGame(string message)
    {
        gameFinished = true;

        resultText.gameObject.SetActive(true);
        resultText.text = message;

        if (playerHealth.IsDead())
        {
            resultText.color = new Color32(239, 68, 68, 255);
        }
        else
        {
            resultText.color = new Color32(34, 197, 94, 255);
        }
    }
}
