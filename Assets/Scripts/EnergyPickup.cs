using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    public int energyAmount = 25;
    public float rotationSpeed = 90f;

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
        PlayerEnergy playerEnergy =
            other.GetComponentInParent<PlayerEnergy>();

        if (playerEnergy == null)
            return;

        PlayerHealth health =
            playerEnergy.GetComponent<PlayerHealth>();

        if (health != null && health.IsDead())
            return;

        playerEnergy.AddEnergy(energyAmount);

        Destroy(gameObject);
    }
}
