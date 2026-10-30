using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerEnergy : MonoBehaviour
{
    public int maxEnergy = 100;
    public int currentEnergy = 0;
    public int activationCost = 100;
    public float boostDuration = 10f;

    public TMP_Text energyText;

    public bool IsBoostActive { get; private set; }

    private Coroutine boostCoroutine;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        UpdateUI();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead())
            return;

        if (Input.GetKeyDown(KeyCode.E) &&
            currentEnergy >= activationCost &&
            !IsBoostActive)
        {
            ActivateBoost();
        }
    }

    public void AddEnergy(int amount)
    {
        if (amount <= 0)
            return;

        currentEnergy = Mathf.Min(
            currentEnergy + amount,
            maxEnergy
        );

        UpdateUI();
    }

    void ActivateBoost()
    {
        currentEnergy -= activationCost;
        IsBoostActive = true;

        UpdateUI();
        boostCoroutine = StartCoroutine(BoostTimer());
    }

    IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(boostDuration);

        IsBoostActive = false;
        boostCoroutine = null;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (energyText == null)
            return;

        if (IsBoostActive)
            energyText.text = "РИКОШЕТ АКТИВЕН!";
        else if (currentEnergy >= activationCost)
            energyText.text = "Энергия: 100/100 [E]";
        else
            energyText.text = "Энергия: " +
                currentEnergy + "/" + maxEnergy;
    }
}
