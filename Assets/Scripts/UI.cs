
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("Models")]
    public PlayerHealth playerHealth;
    public PlayerEnergy playerEnergy;
    public WaveManager waveManager;

    [Header("View")]
    public GameUIView view;

    private bool gameFinished;

    void Start()
    {
        gameFinished = false;

        view.HideResult();
        RefreshUI();
    }

    void Update()
    {
        RefreshUI();

        if (gameFinished || playerHealth == null)
            return;

        if (playerHealth.IsDead())
        {
            view.ShowResult(false);
            gameFinished = true;
        }
        else if (waveManager != null &&
                 waveManager.GameWon)
        {
            view.ShowResult(true);
            gameFinished = true;
        }
    }

    void RefreshUI()
    {
        if (view == null)
            return;

        if (playerHealth != null)
        {
            view.SetHealth(
                playerHealth.currentHealth,
                playerHealth.maxHealth
            );
        }

        if (waveManager != null)
        {
            view.SetEnemies(waveManager.enemiesAlive);
            view.SetWave(waveManager.currentWave);
        }

        if (playerEnergy != null)
        {
            view.SetEnergy(
                playerEnergy.currentEnergy,
                playerEnergy.maxEnergy,
                playerEnergy.activationCost,
                playerEnergy.IsBoostActive
            );
        }
    }
}
