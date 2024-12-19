using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour {
    // Синглтон для доступа к экземпляру BoardManager из других классов
    public static BoardManager instance;

    // Список спрайтов, которые могут использоваться для заполнения плиток
    public List<Sprite> characters = new List<Sprite>();

    // Префаб плитки
    public GameObject tile;

    // Размеры доски
    public int xSize, ySize;

    // Двумерный массив для хранения всех плиток на доске
    private GameObject[,] tiles;

    // Свойство, указывающее, что в данный момент плитки смещаются
    public bool IsShifting { get; set; }

    void Start () {
        // Устанавливаем текущий объект в качестве экземпляра
        instance = GetComponent<BoardManager>();

        // Смещение между плитками
        Vector2 offset = new Vector2(10f, 10f);

        // Создаем игровую доску
        CreateBoard(offset.x, offset.y);
    }

    private void CreateBoard(float xOffset, float yOffset) {
        // Инициализируем массив плиток
        tiles = new GameObject[xSize, ySize];

        // Начальные координаты доски
        float startX = transform.position.x;
        float startY = transform.position.y;

        // Переменные для отслеживания предыдущих спрайтов (для избежания начальных совпадений)
        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        // Создаем плитки
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                // Создаем новую плитку и размещаем её на сцене
                GameObject newTile = Instantiate(tile, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), tile.transform.rotation);

                // Добавляем плитку в массив
                tiles[x, y] = newTile;

                // Устанавливаем родителем текущий объект (BoardManager)
                newTile.transform.parent = transform;

                // Добавляем BoxCollider2D для обработки кликов
                BoxCollider2D collider = newTile.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(xOffset, yOffset);
                collider.isTrigger = true;

                // Создаем список возможных спрайтов для этой плитки
                List<Sprite> possibleCharacters = new List<Sprite>(characters);

                // Убираем из списка спрайты, которые находятся слева и снизу, чтобы избежать совпадений
                possibleCharacters.Remove(previousLeft[y]);
                possibleCharacters.Remove(previousBelow);

                // Выбираем случайный спрайт из оставшихся
                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];

                // Назначаем спрайт плитке
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

                // Сохраняем текущий спрайт как предыдущий для следующей итерации
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public IEnumerator FindNullTiles() {
        // Ищем плитки с пустыми спрайтами (null)
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {
                    // Если нашли пустую плитку, смещаем плитки вниз
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break; // Прерываем, чтобы начать новый поиск
                }
            }
        }

        // После заполнения доски ищем новые совпадения
        for (int x = 0; x < xSize; x++) {
            for (int y = 0; y < ySize; y++) {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) {
        // Устанавливаем флаг, что плитки смещаются
        IsShifting = true;

        // Список всех SpriteRenderer для текущего столбца
        List<SpriteRenderer> renders = new List<SpriteRenderer>();

        // Считаем количество пустых плиток
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++) {
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null) {
                nullCount++;
            }
            renders.Add(render);
        }

        // Перемещаем спрайты вниз
        for (int i = 0; i < nullCount; i++) {
            // Увеличиваем счет за каждую убранную плитку
            GUIManager.instance.Score += 50;

            // Задержка для визуального эффекта
            yield return new WaitForSeconds(shiftDelay);

            // Смещаем спрайты вниз
            for (int k = 0; k < renders.Count - 1; k++) {
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }

        // Сбрасываем флаг после завершения смещения
        IsShifting = false;
    }

    private Sprite GetNewSprite(int x, int y) {
        // Создаем список возможных спрайтов
        List<Sprite> possibleCharacters = new List<Sprite>(characters);

        // Убираем из списка спрайты, которые находятся слева, справа и снизу
        if (x > 0) {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1) {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0) {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        // Возвращаем случайный спрайт из оставшихся
        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
}
