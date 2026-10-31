using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIView : MonoBehaviour
{
    [Header("Health")]
    public Slider healthBar;
    public TMP_Text healthText;

    [Header("Waves")]
    public TMP_Text enemiesText;
    public TMP_Text waveText;

    [Header("Energy")]
    public TMP_Text energyText;

    [Header("Result")]
    public TMP_Text resultText;

    public void SetHealth(float current, float max)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = max;
            healthBar.value = current;
        }

        if (healthText != null)
        {
            healthText.text = "HP: " +
                current.ToString("0") + " / " +
                max.ToString("0");
        }
    }

    public void SetEnemies(int count)
    {
        if (enemiesText != null)
            enemiesText.text = "Враги: " + count;
    }

    public void SetWave(int wave)
    {
        if (waveText != null)
            waveText.text = "Волна: " + wave;
    }

    public void SetEnergy(
        int current,
        int max,
        int activationCost,
        bool boosted)
    {
        if (energyText == null)
            return;

        if (boosted)
        {
            energyText.text = "РИКОШЕТ АКТИВЕН!";
        }
        else if (current >= activationCost)
        {
            energyText.text = "Энергия: " +
                current + "/" + max + " [E]";
        }
        else
        {
            energyText.text = "Энергия: " +
                current + "/" + max;
        }
    }

    public void HideResult()
    {
        if (resultText != null)
            resultText.gameObject.SetActive(false);
    }

    public void ShowResult(bool victory)
    {
        if (resultText == null)
            return;

        resultText.gameObject.SetActive(true);

        resultText.text = victory
            ? "ПОБЕДА"
            : "ПОРАЖЕНИЕ";

        resultText.color = victory
            ? new Color32(34, 197, 94, 255)
            : new Color32(239, 68, 68, 255);
    }
}
