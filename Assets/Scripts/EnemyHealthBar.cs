using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Transform targetCamera;

    public void UpdateHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;

        float percent = current / max;

        if (percent <= 0.25f)
            fillImage.color = Color.red;
        else if (percent <= 0.5f)
            fillImage.color = Color.yellow;
        else
            fillImage.color = Color.green;
    }

    void LateUpdate()
    {
        if (targetCamera == null && Camera.main != null)
            targetCamera = Camera.main.transform;

        if (targetCamera != null)
            transform.rotation = targetCamera.rotation;
    }
}
