using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Camera mainCamera;
    public LineRenderer laserLine;

    public float damage = 25f;
    public float range = 40f;
    public float fireCooldown = 0.3f;
    public float laserDuration = 0.12f;

    public int maxRicochets = 3;

    private float nextFireTime;
    private Coroutine laserCoroutine;
    private PlayerHealth playerHealth;
    private PlayerEnergy playerEnergy;

    [Header("Laser Colors")]
    public Color normalLaserColor = Color.cyan;
    public Color boostedLaserColor = new Color(0.7f, 0.2f, 1f);

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerEnergy = GetComponent<PlayerEnergy>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        laserLine.useWorldSpace = true;
        laserLine.enabled = false;
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead())
            return;

        AimAtMouse();

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireCooldown;
        }
    }

    void AimAtMouse()
    {
        Ray mouseRay = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        Plane groundPlane = new Plane(
            Vector3.up,
            transform.position
        );

        if (groundPlane.Raycast(mouseRay, out float distance))
        {
            Vector3 mousePoint = mouseRay.GetPoint(distance);

            Vector3 direction = mousePoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Shoot()
    {
        if (GameAudio.Instance != null)
            GameAudio.Instance.PlayLaser();

        bool boosted = playerEnergy != null &&
               playerEnergy.IsBoostActive;

        Color laserColor = boosted
            ? boostedLaserColor
            : normalLaserColor;

        laserLine.startColor = laserColor;
        laserLine.endColor = laserColor;

        laserLine.material.SetColor("_BaseColor", laserColor);

        laserLine.startWidth = boosted ? 0.14f : 0.08f;
        laserLine.endWidth = boosted ? 0.14f : 0.08f;

        Vector3 origin = transform.position
            + Vector3.up * 0.2f
            + transform.forward * 0.15f;

        Vector3 direction = transform.forward;

        float remainingRange = range;
        int ricochets = 0;

        int allowedRicochets = 0;

        if (playerEnergy != null && playerEnergy.IsBoostActive)
            allowedRicochets = maxRicochets;

        List<Vector3> points = new List<Vector3>();
        points.Add(origin);

        while (remainingRange > 0f)
        {
            if (Physics.Raycast(
                origin,
                direction,
                out RaycastHit hit,
                remainingRange))
            {
                points.Add(hit.point);
                remainingRange -= hit.distance;

                EnemyAI enemy =
                    hit.collider.GetComponentInParent<EnemyAI>();

                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    break;
                }

                if (!hit.collider.CompareTag("Reflective"))
                    break;

                if (ricochets >= allowedRicochets)
                    break;

                direction = Vector3.Reflect(
                    direction,
                    hit.normal
                ).normalized;

                origin = hit.point + direction * 0.01f;

                remainingRange -= 0.01f;
                ricochets++;
            }
            else
            {
                points.Add(origin + direction * remainingRange);
                break;
            }
        }

        laserLine.positionCount = points.Count;
        laserLine.SetPositions(points.ToArray());

        if (laserCoroutine != null)
            StopCoroutine(laserCoroutine);

        laserCoroutine = StartCoroutine(ShowLaser());
    }

    IEnumerator ShowLaser()
    {
        laserLine.enabled = true;

        yield return new WaitForSeconds(laserDuration);

        laserLine.enabled = false;
        laserCoroutine = null;
    }
}
