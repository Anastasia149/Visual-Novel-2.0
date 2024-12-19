using UnityEngine;
using TMPro; // Для работы с TextMeshPro
using System.Data;
using Mono.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections;

public class RegistrationManager : MonoBehaviour
{
    public TMP_InputField loginInput; // Поле ввода email
    public TMP_InputField passwordInput; // Поле ввода пароля
    public TMP_InputField confirmPasswordInput; // Поле для подтверждения пароля
    public TextMeshProUGUI feedbackText; // Текст для сообщений пользователю
    public GameObject registrationCanvas;
    public GameObject mainCanvas; // Ссылка на основной канвас после входа

    private string dbPath; // Путь к базе данных

    private void Start()
    {
        // Устанавливаем путь к базе данных
#if UNITY_EDITOR
        dbPath = "URI=file:" + Application.dataPath + "/Plugins/VS.db";
#elif UNITY_ANDROID || UNITY_IOS
        dbPath = "URI=file:" + Application.persistentDataPath + "/VS.db";
#endif

        Debug.Log("Путь к базе данных: " + dbPath);
        Debug.Log("Файл существует: " + System.IO.File.Exists(dbPath.Replace("URI=file:", "")));

        // Проверка существования базы данных
       if (!System.IO.File.Exists(dbPath.Replace("URI=file:", "")))
        {
            Debug.LogWarning("База данных не найдена. Создаю новую.");
            CreateDatabase();
        }
    }

    private void Update()
    {
        // Если нажата клавиша ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchToMainCanvas(); // Переключиться на основной канвас
        }
    }

    private void CreateDatabase()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS User (
                    ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Login TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    GameCurrency INTEGER DEFAULT 0
                );";
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void Register()
    {
        string login = loginInput.text.Trim();
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (!IsValidLogin(login))
        {
            feedbackText.text = "Введите корректный логин.";
            return;
        }

        if (password != confirmPassword)
        {
            feedbackText.text = "Пароли не совпадают.";
            return;
        }

        if (password.Length < 6)
        {
            feedbackText.text = "Пароль должен быть не менее 6 символов.";
            return;
        }

        if (CheckIfUserExists(login))
        {
            feedbackText.text = "Этот логин уже используется.";
            return;
        }

        AddUser(login, password, 0);
        feedbackText.text = "Регистрация успешна!";
    }

    public void Login()
    {
        string login = loginInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Введите логин и пароль.";
            return;
        }

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = "SELECT Password FROM User WHERE Login = @login";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                var result = command.ExecuteScalar();

                if (result == null)
                {
                    feedbackText.text = "Пользователь не найден.";
                    return;
                }

                string storedHashedPassword = result.ToString();
                string inputHashedPassword = HashPassword(password);

                if (storedHashedPassword == inputHashedPassword)
                {
                    feedbackText.text = "Вход выполнен успешно!";
                    Debug.Log($"Пользователь {login} успешно вошёл.");
                    StartCoroutine(SwitchCanvasAfterDelay());
                }
                else
                {
                    feedbackText.text = "Неверный пароль.";
                }
            }
        }
    }

    private bool IsValidLogin(string login)
    {
        if (string.IsNullOrEmpty(login)) return false;

        if (login.Length < 3 || login.Length > 50) return false;

        string validLoginPattern = "^[a-zA-Z0-9._-]+$";
        return System.Text.RegularExpressions.Regex.IsMatch(login, validLoginPattern);
    }

    private bool CheckIfUserExists(string login)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = "SELECT COUNT(1) FROM User WHERE Login = @login";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
        }
    }

    private void AddUser(string login, string password, int gameCurrency)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            string query = "INSERT INTO User (Login, Password, GameCurrency) VALUES (@login, @password, @gameCurrency)";
            using (var command = new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", HashPassword(password));
                command.Parameters.AddWithValue("@gameCurrency", gameCurrency);
                command.ExecuteNonQuery();
            }
        }
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private IEnumerator SwitchCanvasAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Задержка перед переключением канвасов

        SwitchToMainCanvas();
    }

    private void SwitchToMainCanvas()
    {
        if (registrationCanvas != null)
        {
            registrationCanvas.SetActive(false); // Скрыть канвас регистрации
        }

        if (mainCanvas != null)
        {
            mainCanvas.SetActive(true); // Показать основной канвас
        }
        else
        {
            Debug.LogWarning("MainCanvas не установлен.");
        }
    }
}
