using UnityEngine;
using UnityEngine.UI;

public class BrightnessControl : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;

    void Start()
    {
        // Загрузка сохраненного значения яркости
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 1.0f); // Значение по умолчанию - 1.0
        RenderSettings.ambientIntensity = savedBrightness;

        // Устанавливаем слайдер в сохраненное положение
        brightnessSlider.value = savedBrightness;

        // Подписываемся на событие изменения значения
        brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
    }

    private void ChangeBrightness(float value)
    {
        // Применяем яркость и сохраняем её
        RenderSettings.ambientIntensity = value;
        PlayerPrefs.SetFloat("Brightness", value);
    }

    void OnDestroy()
    {
        brightnessSlider.onValueChanged.RemoveListener(ChangeBrightness);
    }
}