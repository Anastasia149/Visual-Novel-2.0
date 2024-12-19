using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject registrationCanvas; // Ссылка на Canvas регистрации
    public GameObject SettingsMenu; // Ссылка на главное меню (SettingsMenu)

    // Метод для открытия окна регистрации
    public void OpenRegistrationCanvas()
    {
        if (SettingsMenu != null)
        {
            SettingsMenu.SetActive(false); // Закрываем главное меню
        }
        
        registrationCanvas.SetActive(true); // Открываем окно регистрации
    }

    // Метод для закрытия окна регистрации и возврата к меню
    public void CloseRegistrationCanvas()
    {
        registrationCanvas.SetActive(false); // Закрываем окно регистрации

        if (SettingsMenu != null)
        {
            SettingsMenu.SetActive(true); // Открываем главное меню
        }
    }
}
