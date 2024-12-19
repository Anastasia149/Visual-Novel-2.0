using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class buttonstuffs : MonoBehaviour
{
    public AudioSource mySounds;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public void HoverSound()
    {
        mySounds.PlayOneShot(hoverSound);
    }

    public void ClickSound()
    {
        mySounds.PlayOneShot(clickSound);
    }
}
