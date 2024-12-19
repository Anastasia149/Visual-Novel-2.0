using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    [SerializeField] private Text diamondText;
    private int currentDiamonds; // Текущее количество алмазов

    public void UpdateMoneyDisplay(int newAmount)
    {
        currentDiamonds = newAmount;
        if (diamondText != null)
        {
            diamondText.text = $"Алмазы: {currentDiamonds}";
        }
        else
        {
            Debug.LogWarning("Текст для отображения алмазов не назначен!");
        }
    }
}
