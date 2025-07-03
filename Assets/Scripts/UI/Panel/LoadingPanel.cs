using UnityEngine;
using Core;
using System;

public class LoadingPanel : PanelView
{
    public Action OnTapToPlayClicked;

    public void OnTapToPlay_Button()
    {
        OnTapToPlayClicked?.Invoke();
    }
}
