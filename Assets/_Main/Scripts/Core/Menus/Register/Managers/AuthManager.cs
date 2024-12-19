using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue; // Используется для работы с диалогами (возможно, определенный вами класс)
using Mono.Data.Sqlite; // Библиотека для работы с SQLite базой данных
using System.Data; // Пространство имен для работы с IDataReader
using System.IO; // Для работы с файлами
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class AuthManager : MonoBehaviour
{
    public InputField loginInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button loginButton;
    public Text feedbackText;

    private string currentScene;
    private bool isReturning = false;
     private AsyncOperation async;

    public static GameManager instance;

    private Color fadeTransparency = new Color(0, 0, 0, .04f)
    {

    };
    public float fadeSpeed = .02f;
    

    private DatabaseHandler db;

    void Start()
    {
        db = new DatabaseHandler("VS.db");
        registerButton.onClick.AddListener(Register);
        loginButton.onClick.AddListener(Login);
    }

    void Update() {
        // Проверяем нажатие клавиши "Escape"
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ReturnToVisualNovel(); // Возвращаемся в сцену визуальной новеллы
        }
    }

    void Register()
    {
        string login = loginInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please fill in both fields.";
            return;
        }

        if (db.CheckIfUserExists(login))
        {
            feedbackText.text = "User already exists.";
        }
        else
        {
            // Регистрация нового пользователя с начальным количеством валюты (например, 100)
            db.AddUser(login, password, 100);
            feedbackText.text = "Registration successful!";
        }
    }

    void Login()
    {
        string login = loginInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please fill in both fields.";
            return;
        }

        if (db.ValidateUser(login, password))
        {
            feedbackText.text = "Login successful!";
            // Переход к следующей сцене или основному меню игры
        }
        else
        {
            feedbackText.text = "Invalid login or password.";
        }
    }

    public string CurrentSceneName {
        get {
            return currentScene;
        }
    }

    public void ReturnToVisualNovel() {
        if (isReturning) { // Если уже возвращаемся, выходим
            return;
        }

        if (CurrentSceneName != "Visual Novel") { // Если мы не в сцене "Visual Novel"
            StopAllCoroutines(); // Останавливаем все корутины
            LoadScene("Visual Novel"); // Загружаем сцену визуальной новеллы
            isReturning = true; // Устанавливаем флаг возврата
        }
    }

     public void LoadScene(string sceneName) {
        instance.StartCoroutine(Load(sceneName)); // Начинаем асинхронную загрузку сцены
         instance.StartCoroutine(FadeOut(instance.faderObj, instance.faderImg)); // Начинаем затемнение
    }

     IEnumerator Load(string sceneName) {
        async = SceneManager.LoadSceneAsync(sceneName); // Начинаем асинхронную загрузку
        async.allowSceneActivation = false; // Не активируем сцену сразу
        yield return async; // Ждем завершения загрузки
        isReturning = false; // Сбрасываем флаг возврата
    }
    IEnumerator FadeOut(GameObject faderObject, Image fader) {
        faderObject.SetActive(true); // Активируем объект затемнения
        while (fader.color.a < 1) { // Пока не достигнем полного затемнения
            fader.color += fadeTransparency; // Увеличиваем прозрачность
            yield return new WaitForSeconds(fadeSpeed);
        }
        ActivateScene(); // Активируем загруженную сцену
    }
    public void ActivateScene() 
	{
        async.allowSceneActivation = true; // Разрешаем смену сцены

		if (async != null) 
		{
			async.allowSceneActivation = true;
		} else 
		{
			Debug.LogError("Асинхронная загрузка не была инициализирована.");
		}
    }
}
