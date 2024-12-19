using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DifficultySelectionManager : MonoBehaviour
{
    public GameObject difficultyCanvas;  // Канвас с кнопками
    public GameObject gameCanvas;        // Канвас с игрой (если у вас есть отдельный канвас для игры)
    public GameManager gameManager;      // Ссылка на ваш GameManager

    public void OnEasyButtonClicked()
    {
        gameManager.StartNewGame(GUIManager.Difficulty.Easy);
        GUIManager.instance.StartTimer();  // Запуск таймера
        SwitchToGameCanvas();
    }

    public void OnMediumButtonClicked()
    {
        gameManager.StartNewGame(GUIManager.Difficulty.Medium);
        GUIManager.instance.StartTimer();  // Запуск таймера
        SwitchToGameCanvas();
    }

    public void OnHardButtonClicked()
    {
        gameManager.StartNewGame(GUIManager.Difficulty.Hard);
        GUIManager.instance.StartTimer();  // Запуск таймера
        SwitchToGameCanvas();
    }

    // Метод для переключения канвасов
    private void SwitchToGameCanvas()
    {
        difficultyCanvas.SetActive(false); // Скрываем канвас с выбором сложности
        gameCanvas.SetActive(true);         // Показываем канвас игры
    }
}

