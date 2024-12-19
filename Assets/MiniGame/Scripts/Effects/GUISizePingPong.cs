using UnityEngine;
using System.Collections;

// Обеспечивает, что компонент RectTransform обязательно присутствует на объекте
[RequireComponent(typeof(RectTransform))]
public class GUISizePingPong : MonoBehaviour {

    // Минимальный масштаб, до которого будет уменьшаться объект
    public Vector3 minScale = new Vector3(.5f, .5f, .5f);

    // Задержка между изменениями размера
    public float changeDelay = .03f;

    // Ссылка на компонент RectTransform
    RectTransform rectTransform;

    // Инициализация скрипта
    void Start() {
        rectTransform = GetComponent<RectTransform>(); // Получаем RectTransform объекта
        StartCoroutine(PingPongSize()); // Запускаем корутину для изменения размера
    }

    // Корутина для изменения размера объекта
    IEnumerator PingPongSize() {
        Vector3 startingScale = rectTransform.localScale; // Исходный масштаб объекта
        Vector3 currentScale = startingScale; // Текущий масштаб, который будет изменяться

        // Величина изменения масштаба за одну итерацию
        Vector3 changeScale = new Vector3(.01f, .01f, .01f);
        bool shrink = true; // Флаг для определения направления изменения (уменьшение/увеличение)

        // Бесконечный цикл изменения размера
        while (true) {
            if (shrink) {
                // Уменьшение размера
                currentScale -= changeScale;
                if (currentScale.x <= minScale.x) {
                    shrink = false; // Переходим к увеличению размера
                }
            } else {
                // Увеличение размера
                currentScale += changeScale;
                if (currentScale.x >= 1) { // Возвращаемся к исходному масштабу
                    shrink = true; // Переходим к уменьшению размера
                }
            }
            rectTransform.localScale = currentScale; // Применяем измененный масштаб
            yield return new WaitForSeconds(changeDelay); // Ждем перед следующей итерацией
        }
    }
}
