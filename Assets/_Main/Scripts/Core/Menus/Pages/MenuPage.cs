using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPage : MonoBehaviour
{
    public enum PageType {  SaveAndLoad, Config, Help }
    public PageType pageType;

    private const string open = "Open";
    private const string close = "Close";
    public Animator anim;
    public virtual void Open()
    {
        anim.SetTrigger(open);
    }

    public virtual void Close(bool closeAllMenus = false)
    {
        anim.SetTrigger(close);

        if (closeAllMenus)
            VNMenuManager.instance.CloseRoot();

    }

}
