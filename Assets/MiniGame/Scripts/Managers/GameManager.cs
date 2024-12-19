using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    // Синглтон для доступа к единственному экземпляру GameManager
    public static GameManager instance;

    // Объект для затемнения экрана при смене сцен
    public GameObject faderObj;

    // Изображение затемнения
    public Image faderImg;

    // Флаг для проверки, завершена ли игра
    public bool gameOver = false;

    // Скорость затемнения экрана
    public float fadeSpeed = .02f;

    // Цвет, используемый для управления прозрачностью затемнения
    private Color fadeTransparency = new Color(0, 0, 0, .04f);

    // Название текущей сцены
    private string currentScene;

    // Операция асинхронной загрузки сцены
    private AsyncOperation async;

    private bool isReturning = false;

    void Awake() {
        // Проверяем, существует ли уже экземпляр GameManager
        if (instance == null) {
            // Сохраняем GameManager при смене сцен
            DontDestroyOnLoad(gameObject);

            // Устанавливаем текущий экземпляр
            instance = GetComponent<GameManager>();

            // Подписываемся на событие, вызываемое после загрузки сцены
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        } else {
            // Уничтожаем дублирующийся экземпляр
            Destroy(gameObject);
        }
    }

    void Update() {
        // Проверяем нажатие клавиши "Escape"
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ReturnToVisualNovel(); // Возвращаемся в сцену визуальной новеллы
        }
    }

    // Загружает сцену с указанным именем
    public void LoadScene(string sceneName) {
        instance.StartCoroutine(Load(sceneName)); // Начинаем асинхронную загрузку сцены
        instance.StartCoroutine(FadeOut(instance.faderObj, instance.faderImg)); // Начинаем затемнение
    }

	public void StartNewGame(GUIManager.Difficulty difficulty) {
        GUIManager.instance.SetDifficulty(difficulty);  // Устанавливаем сложность
        // Перезагружаем сцену или начинаем игру заново
        //ReloadScene();
    }

    // Перезагружает текущую сцену
    public void ReloadScene() {
        LoadScene(SceneManager.GetActiveScene().name); // Загружаем текущую сцену
    }

    // Вызывается, когда сцена загружена
    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        currentScene = scene.name; // Сохраняем имя текущей сцены
        instance.StartCoroutine(FadeIn(instance.faderObj, instance.faderImg)); // Запускаем осветление экрана
    }

    // Постепенно увеличивает прозрачность, чтобы затемнить экран
    IEnumerator FadeOut(GameObject faderObject, Image fader) {
        faderObject.SetActive(true); // Активируем объект затемнения
        while (fader.color.a < 1) { // Пока не достигнем полного затемнения
            fader.color += fadeTransparency; // Увеличиваем прозрачность
            yield return new WaitForSeconds(fadeSpeed);
        }
        ActivateScene(); // Активируем загруженную сцену
    }

    // Постепенно уменьшает прозрачность, чтобы осветлить экран
    IEnumerator FadeIn(GameObject faderObject, Image fader) {
        while (fader.color.a > 0) { // Пока не достигнем полной прозрачности
            fader.color -= fadeTransparency; // Уменьшаем прозрачность
            yield return new WaitForSeconds(fadeSpeed);
        }
        faderObject.SetActive(false); // Деактивируем объект затемнения
    }

    // Асинхронно загружает сцену с указанным именем
    IEnumerator Load(string sceneName) {
        async = SceneManager.LoadSceneAsync(sceneName); // Начинаем асинхронную загрузку
        async.allowSceneActivation = false; // Не активируем сцену сразу
        yield return async; // Ждем завершения загрузки
        isReturning = false; // Сбрасываем флаг возврата
    }

    // Активирует загруженную сцену
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

    // Возвращает имя текущей сцены
    public string CurrentSceneName {
        get {
            return currentScene;
        }
    }

    // Завершает игру и закрывает приложение
    public void ExitGame() {
        // Проверяем, если игра запущена как отдельное приложение
        #if UNITY_STANDALONE
            Application.Quit(); // Закрываем приложение
        #endif

        // Проверяем, если игра запущена в редакторе
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Останавливаем выполнение сцены
        #endif
    }

    // Флаг для предотвращения повторного возврата
    

    // Возвращает игрока в сцену визуальной новеллы
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
}
