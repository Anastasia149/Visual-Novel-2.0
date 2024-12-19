using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Testing
{
    public class VariableStats : MonoBehaviour
    {
        //[SerializeField] public string stat;
        //[SerializeField] public int var_int = 0;
        //[SerializeField] private TextMeshProUGUI statText;

        public int var_dance = 0;
        public int var_guh = 0;
        public int var_swag = 0;
        //public string Charisma;
        public int var_char = 0;
        public float var_flt = 0;
        public bool var_bool = false;
        public string var_str = "";

        /*public VariableStats(string stat, int value)
         {
             this.stat = stat;
             this.var_int = value;

         }*/

        void Start()
        {
            VariableStore.CreateDatabase("DB_Links");

            VariableStore.CreateDatabase("Stats");
            VariableStore.CreateVariable("Stats.Charisma", var_char, () => var_char, value => var_char = value);
            VariableStore.CreateVariable("Stats.Danceoffskills", var_dance, () => var_dance, value => var_dance = value);
            VariableStore.CreateVariable("Stats.guh", var_guh, () => var_guh, value => var_guh = value);
            VariableStore.CreateVariable("Stats.Swagginess", var_swag, () => var_swag, value => var_swag = value);

            //VariableStore.CreateVariable("DB_Links.L_int", var_int, () => var_int, value => var_int = value);
            //VariableStore.CreateVariable("DB_Links.L_flt", var_flt, () => var_flt, value => var_flt = value);
            //VariableStore.CreateVariable("DB_Links.L_bool", var_bool, () => var_bool, value => var_bool = value);
            //VariableStore.CreateVariable("DB_Links.L_str", var_str, () => var_str, value => var_str = value);


            VariableStore.CreateDatabase("DB_Numbers");
            VariableStore.CreateDatabase("DB_Booleans");

            VariableStore.CreateVariable("DB_Numbers.num1", 1);
            VariableStore.CreateVariable("DB_Numbers.num5", 5);
            VariableStore.CreateVariable("DB_Booleans.lightIsOn", true);
            VariableStore.CreateVariable("DB_Numbers.float1", 7.5f);
            VariableStore.CreateVariable("str1", "hello");
            VariableStore.CreateVariable("str2", "world");

            VariableStore.PrintAllDatabases();

            VariableStore.PrintAllVariables();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                VariableStore.PrintAllVariables();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                string variable = "DB_Numbers.num1";
                VariableStore.TryGetValue(variable, out object v);
                VariableStore.TrySetValue(variable, (int)v + 5);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                VariableStore.TryGetValue("DB_Numbers.num1", out object num1);
                VariableStore.TryGetValue("DB_Numbers.num5", out object num2);

                Debug.Log($"num1 + num2 = {(int)num1 + (int)num2}");
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                VariableStore.TryGetValue("Stats.Charisma", out object charisma);
                VariableStore.TrySetValue("Stats.Charisma", (int)charisma + 1);

                VariableStore.TryGetValue("Stats.Danceoffskills", out object dance);
                VariableStore.TrySetValue("Stats.Danceoffskills", (int)dance + 4);

            }
        }
    }
}