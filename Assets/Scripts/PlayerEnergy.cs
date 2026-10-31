using System.Collections;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Energy")]
    public int maxEnergy = 100;
    public int currentEnergy = 0;
    public int activationCost = 100;

    [Header("Boost")]
    public float boostDuration = 10f;

    public bool IsBoostActive { get; private set; }

    private Coroutine boostCoroutine;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
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
    }

    void ActivateBoost()
    {
        currentEnergy -= activationCost;
        IsBoostActive = true;

        boostCoroutine = StartCoroutine(BoostTimer());
    }

    IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(boostDuration);

        IsBoostActive = false;
        boostCoroutine = null;
    }
}
