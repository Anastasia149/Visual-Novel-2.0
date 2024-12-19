using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class CharText : Character
    {
        public CharText(string name, CharacterConfigData config) : base(name, config, prefab: null)
        {
            Debug.Log($"Created Text Character: '{name}'");
        }
    }
}