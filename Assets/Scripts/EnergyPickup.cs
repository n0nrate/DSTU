using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [Header("Energy")]
    public int energyAmount = 25;

    [Header("Visual")]
    public float rotationSpeed = 90f;

    private bool isCollected = false;

    void Update()
    {
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.World
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected)
            return;

        PlayerEnergy playerEnergy =
            other.GetComponentInParent<PlayerEnergy>();

        if (playerEnergy == null)
            return;

        PlayerHealth health =
            playerEnergy.GetComponent<PlayerHealth>();

        if (health != null && health.IsDead())
            return;

        isCollected = true;

        playerEnergy.AddEnergy(energyAmount);

        if (GameAudio.Instance != null)
        {
            GameAudio.Instance.PlayEnergyPickup();
        }

        Destroy(gameObject);
    }
}
