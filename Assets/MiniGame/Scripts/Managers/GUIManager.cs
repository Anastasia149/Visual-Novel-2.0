using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour {
    public static GUIManager instance; // Глобальный экземпляр GUIManager для доступа из других скриптов

    public GameObject gameOverPanel; // Панель, отображаемая при завершении игры
    public Text yourScoreTxt; // Текст для отображения текущего счета игрока
    public Text highScoreTxt; // Текст для отображения лучшего результата (рекорда)

    public Text scoreTxt; // Текст для отображения текущего счета в процессе игры
    //public Text moveCounterTxt; // Текст для отображения оставшегося количества ходов

    public Text timerTxt;  // Новый текстовый элемент для таймера
    public Text difficultyTxt;  // Текст для отображения выбранного уровня сложности

    private int score; // Счет игры и оставшееся количество ходов
    private float timeRemaining;  // Оставшееся время
    private bool gameIsActive = true;
    private bool timerStarted = false;  // Флаг для начала отсчета таймера
    private int targetScore;  // Цель для достижения очков

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty;  // Уровень сложности

    void Awake() {
        instance = GetComponent<GUIManager>(); // Устанавливаем текущий экземпляр
        //moveCounter = 40; // Устанавливаем начальное количество ходов
        SetDifficulty(Difficulty.Easy);  // Начнем с легкого уровня
    }

     void Update() {
        if (gameIsActive && timerStarted) {  // Таймер запускается только, если он активирован
            // Обновляем таймер
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0) {
                timeRemaining = 0;
                GameOver();  // Завершаем игру, если время вышло
            }
            timerTxt.text = "Time: " + Mathf.Ceil(timeRemaining).ToString();

            // Проверка на достижение цели очков
            if (score >= targetScore) {
                GameOver();
            }
        }
    }

    public void SetDifficulty(Difficulty difficulty) {
        currentDifficulty = difficulty;

        // Устанавливаем цели в зависимости от сложности
        if (difficulty == Difficulty.Easy) {
            targetScore = 5000;
            timeRemaining = 120;  // 2 минуты на легком уровне
            difficultyTxt.text = "Difficulty: Easy";
        } else if (difficulty == Difficulty.Medium) {
            targetScore = 8000;
            timeRemaining = 90;  // 1.5 минуты на среднем уровне
            difficultyTxt.text = "Difficulty: Medium";
        } else if (difficulty == Difficulty.Hard) {
            targetScore = 100;
            timeRemaining = 10;  // 1 минута на сложном уровне
            difficultyTxt.text = "Difficulty: Hard";
        }
    }

     public void StartTimer() {
        timerStarted = true;  // Запуск таймера
    }

    // Отображает панель завершения игры
    public void GameOver() {
        GameManager.instance.gameOver = true; // Устанавливаем флаг завершения игры

        gameOverPanel.SetActive(true); // Показываем панель Game Over

        // Проверяем, превысил ли текущий счет рекордный
        if (score > PlayerPrefs.GetInt("HighScore")) {
            PlayerPrefs.SetInt("HighScore", score); // Сохраняем новый рекорд в PlayerPrefs
            highScoreTxt.text = "New High Score!"; // Текст для награды
        } else {
            highScoreTxt.text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString(); // Текст для награды без изменения рекорда
        }

        yourScoreTxt.text = score.ToString(); // Отображаем текущий счет

        // Определение награды на основе сложности
        if (currentDifficulty == Difficulty.Easy) {
            highScoreTxt.text += " - 10 алмазов";
        } else if (currentDifficulty == Difficulty.Medium) {
            highScoreTxt.text += " - 20 алмазов";
        } else {
            highScoreTxt.text += " - 30 алмазов";
        }
    }

    // Свойство для работы с текущим счетом
    public int Score 
    {
        get {
            return score; // Возвращаем текущий счет
        }

        set {
            score = value; // Устанавливаем новое значение счета
            scoreTxt.text = score.ToString(); // Обновляем отображение счета
        }        
    }

    // Свойство для работы с количеством оставшихся ходов
    // public int MoveCounter {
    //     get {
    //         return moveCounter; // Возвращаем текущее количество ходов
    //     }

    //     set {
    //         moveCounter = value; // Устанавливаем новое значение ходов
    //         if (moveCounter <= 0) { // Если ходы закончились
    //             moveCounter = 0; // Устанавливаем 0, чтобы избежать отрицательных значений
    //             StartCoroutine(WaitForShifting()); // Запускаем проверку окончания сдвигов
    //         }
    //         moveCounterTxt.text = moveCounter.ToString(); // Обновляем отображение количества ходов
    //     }
    // }

    // Ожидает завершения всех сдвигов перед завершением игры
    private IEnumerator WaitForShifting() {
        yield return new WaitUntil(() => !BoardManager.instance.IsShifting); // Ждет, пока сдвиги завершатся
        yield return new WaitForSeconds(.25f); // Небольшая задержка
        GameOver(); // Вызывает завершение игры
    }
}
