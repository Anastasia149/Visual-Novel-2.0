using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Characters;

namespace Testing
{
    public class AudioTesting : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(Running());
        }

        Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        IEnumerator Running()
        {
            AudioManager.instance.PlayTrack("Audio/Music/Fly Me To The moon", volumeCap: 0.9f);

            yield return new WaitForSeconds(3);

            AudioManager.instance.PlayTrack("Audio/Music/space", volumeCap: 0.6f);

            yield return null;
        }
    }
}
