using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace History
{
    public class HistoryCache
    {
        public static Dictionary<string, (object asset, int stateIndex)> loadedAssets = new Dictionary<string, (object asset, int stateIndex)> ();

        public static T TryLoadObject<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("Попытка загрузить ресурс с пустым или нулевым ключом");
                return default(T);
            }

            Debug.Log($"Попытка загрузить ресурс: {key}");

            if (loadedAssets.ContainsKey(key))
            {
                object cachedResource = loadedAssets[key].asset;
                if (cachedResource is T cachedResult)
                    return cachedResult;
                
                Debug.LogWarning($"Кэшированный объект '{key}' не совпадает с ожидаемым типом: {typeof(T)}");
                return default(T);
            }

            object resource = Resources.Load(key);
            if (resource == null)
            {
                Debug.LogWarning($"Ресурс '{key}' не найден в папке Resources.");
                return default(T);
            }

            if (resource is T result)
            {
                loadedAssets[key] = (resource, 0); // Кэшируем загруженный ресурс
                return result;
            }

            Debug.LogWarning($"Ресурс '{key}' найден, но его тип не совпадает с ожидаемым: {typeof(T)}");
            return default(T);
        }


        public static TMP_FontAsset LoadFont(string key) => TryLoadObject<TMP_FontAsset>(key);
        public static AudioClip LoadAudio(string key) => TryLoadObject<AudioClip>(key);
        public static Texture2D LoadImage(string key) => TryLoadObject<Texture2D>(key);
    }
}