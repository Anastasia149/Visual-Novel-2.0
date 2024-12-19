using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {
    // Цвет выделенной плитки
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);

    // Ссылка на ранее выделенную плитку
    private static Tile previousSelected = null;

    // Ссылка на SpriteRenderer текущей плитки
    private SpriteRenderer render;

    // Флаг, обозначающий, выбрана ли текущая плитка
    private bool isSelected = false;

    // Массив направлений для поиска соседних плиток
    private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    void Awake() {
        // Инициализация ссылки на SpriteRenderer
        render = GetComponent<SpriteRenderer>();
    }

    private void Select() {
        // Выделяем плитку
        isSelected = true;
        render.color = selectedColor;

        // Устанавливаем текущую плитку как ранее выбранную
        previousSelected = gameObject.GetComponent<Tile>();

        // Проигрываем звук выбора
        SFXManager.instance.PlaySFX(Clip.Select);
    }

    private void Deselect() {
        // Снимаем выделение с плитки
        isSelected = false;
        render.color = Color.white;

        // Сбрасываем ранее выбранную плитку
        previousSelected = null;
    }

    void OnMouseDown() {
        // Условия, при которых плитка не может быть выбрана
        if (render.sprite == null || BoardManager.instance.IsShifting) {
            return;
        }

        if (isSelected) { // Если плитка уже выбрана
            Deselect();
        } else {
            if (previousSelected == null) { // Если это первая выбранная плитка
                Select();
            } else {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) { // Если ранее выбранная плитка — соседняя
                    SwapSprite(previousSelected.render);
                    previousSelected.ClearAllMatches();
                    previousSelected.Deselect();
                    ClearAllMatches();
                } else {
                    // Если плитка не соседняя, снимаем выделение с предыдущей
                    previousSelected.GetComponent<Tile>().Deselect();
                    Select();
                }
            }
        }
    }

    public void SwapSprite(SpriteRenderer render2) {
        // Обмен спрайтов между текущей и соседней плиткой
        if (render.sprite == render2.sprite) {
            return; // Ничего не делаем, если спрайты одинаковые
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;

        // Проигрываем звук обмена
        SFXManager.instance.PlaySFX(Clip.Swap);

        // Уменьшаем счетчик ходов
        //GUIManager.instance.MoveCounter--; 
    }

    private GameObject GetAdjacent(Vector2 castDir) {
        // Возвращает соседнюю плитку в указанном направлении
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null) {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles() {
        // Получаем все соседние плитки
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++) {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    private List<GameObject> FindMatch(Vector2 castDir) {
        // Находит все совпадающие плитки в указанном направлении
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) {
            matchingTiles.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths) {
        // Очищает совпадения в указанных направлениях
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++) {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2) { // Если совпадений 2 или больше
            for (int i = 0; i < matchingTiles.Count; i++) {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    private bool matchFound = false;

    public void ClearAllMatches() {
        // Удаляет все совпадения для текущей плитки
        if (render.sprite == null)
            return;

        // Проверяем совпадения по горизонтали и вертикали
        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (matchFound) {
            // Если есть совпадения, очищаем текущую плитку
            render.sprite = null;
            matchFound = false;

            // Останавливаем и запускаем процесс поиска пустых плиток
            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());

            // Проигрываем звук удаления
            SFXManager.instance.PlaySFX(Clip.Clear);
        }
    }
}
