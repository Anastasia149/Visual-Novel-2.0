using UnityEngine;

// Перечисление для определения типов звуковых эффектов
public enum Clip { Select, Swap, Clear };

public class SFXManager : MonoBehaviour {
    public static SFXManager instance; // Глобальный экземпляр SFXManager для доступа из других скриптов

    private AudioSource[] sfx; // Массив аудиокомпонентов для звуковых эффектов

    // Инициализация при запуске
    void Start() {
        instance = GetComponent<SFXManager>(); // Устанавливаем текущий экземпляр
        sfx = GetComponents<AudioSource>(); // Получаем все аудиокомпоненты, прикрепленные к объекту
    }

    // Воспроизводит звуковой эффект в зависимости от переданного типа
    public void PlaySFX(Clip audioClip) {
        sfx[(int)audioClip].Play(); // Проигрываем аудиоклип, соответствующий индексу в перечислении
    }
}

