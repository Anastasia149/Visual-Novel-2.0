using UnityEngine;

public class LogoutManager : MonoBehaviour
{
    public GameObject registrationCanvas;

    public void Logout()
{
    // Выводим информацию в консоль для отладки
    Debug.Log("Logout called");

    // Сброс текущей сессии
    PlayerPrefs.DeleteKey("LoggedInEmail");
    PlayerPrefs.Save();

    // Выводим информацию о том, что ключ был удалён
    Debug.Log("LoggedInEmail key deleted");

    // Показываем окно регистрации
    registrationCanvas.SetActive(true);
}

}
