

// Randomizer for text scripts - works but doesn't save



/*using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Randomizer : MonoBehaviour
{
    public int random;
    public static int tries = 0;

    public static bool firstTry = true;
    public bool selected = false;

    public GameObject Convo1;
    public GameObject Convo2;
    public GameObject Convo3;
    public GameObject End;


    [SerializeField] public static bool Convo1Check = true;
    [SerializeField] public static bool Convo2Check = true;
    [SerializeField] public static bool Convo3Check = true;

    public static List<int> listOfOptions = new List<int> { 1, 2, 3 };

    public static List<int> backup = new List<int> { 1, 2, 3 };


    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        /*PlayerPrefs.SetInt("firstTry", (firstTry ? 1 : 0));
        PlayerPrefs.Save();

        for (int i = 0; i < PlayerPrefs.GetInt("random_count"); i++)
        {
            PlayerPrefs.SetInt("options" + 4, listOfOptions[i]);
            PlayerPrefs.Save();
            int something = PlayerPrefs.GetInt("options" + i);
            Debug.Log($"{something}");
            listOfOptions[i] = PlayerPrefs.GetInt("options" + 0);
        }


        //var random = new int[] { 1, 7, 3 };
        if (firstTry == true)
        {
            int randomIndex = Random.Range(0, listOfOptions.Count);
            int selectedItem = listOfOptions[randomIndex];
            listOfOptions.RemoveAt(randomIndex);
            Debug.Log($"{selectedItem} {listOfOptions.Count}");
            random = selectedItem;
            firstTry = false;
            
        }

        else
        {
            int random_count = PlayerPrefs.GetInt("random_count");
            int randomIndex = Random.Range(0, random_count);
            int selectedItem = listOfOptions[randomIndex];
            listOfOptions.RemoveAt(randomIndex);
            Debug.Log($"ya mama{selectedItem} {random_count}");
            random = selectedItem;
        }
        PlayerPrefs.SetInt("random_count", listOfOptions.Count);
        PlayerPrefs.Save();

        for (int i = 0; i < PlayerPrefs.GetInt("random_count"); i++)
        {
            PlayerPrefs.SetInt("options" + i, 0);
            PlayerPrefs.Save();
        }

        // overwrite listOfOptions.Count with random count
        // something with uh selecteditem

        int randomIndex = Random.Range(0, listOfOptions.Count);
        int selectedItem = listOfOptions[randomIndex];
        random = selectedItem;
        Debug.Log($"{selectedItem}");

    }

    // Update is called once per frame
    void Update()
    {

        if (random == 1 && Convo1Check == true)
        {
            selected = true;
            Convo1.SetActive(true);
            Convo1Check = false;
            Debug.Log($"{Convo1Check}");
            tries = tries + 1;
        }
        if (random == 2 && Convo2Check == true)
        {
            selected = true;
            Convo2.SetActive(true);
            Convo2Check = false;
            Debug.Log($"{Convo2Check}");
            tries = tries + 1;
        }
        if (random == 3 && Convo3Check == true)
        {
            selected = true;
            Convo3.SetActive(true);
            Convo3Check = false;
            Debug.Log($"{Convo3Check}");
            tries = tries + 1;
        }
        if (Convo1Check == false && Convo2Check == false && Convo3Check == false)
        {
            selected = true;
            End.SetActive(true);
        }

        else if (selected == false)
        {
            for (int i = 0; i < listOfOptions.Count; i++)
            {
                int randomIndex2 = Random.Range(0, listOfOptions.Count);
                int selectedItem2 = listOfOptions[randomIndex2];
                random = selectedItem2;
                Debug.Log($"{selectedItem2}");
            }
        }

        if (Input.GetKeyDown("p"))
        {
            Convo2Check = true; 
            Convo3Check = true;
            Convo1Check = true;
        }

    }
}
*/