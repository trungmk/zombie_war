using Core;
using UnityEngine;
using MEC;
using System.Collections;
using System.Collections.Generic;
using System;

public class BootingSceneController : SceneController
{
    public override void OnLoaded()
    {
        ObjectPooling.Instance.OnLoadPoolsCompleted = Handle_OnLoadPoolsCompleted;
        ObjectPooling.Instance.Init(null);
        UIManager.Instance.Show<LoadingPanel>()
            .OnShowCompleted(view =>
            {
                LoadingPanel loadingPanel = view as LoadingPanel;
                if (loadingPanel != null)
                {
                    loadingPanel.OnTapToPlayClicked += () =>
                    {
                        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_LEVEL_1);
                        UIManager.Instance.Hide<LoadingPanel>(isDisable: true, isDestroy: true);
                    };
                }
            });
    }

    private void Handle_OnLoadPoolsCompleted()
    {
        LoadingPanel loadingPanel = UIManager.Instance.GetCache<LoadingPanel>();

        if (loadingPanel != null)
        {
            loadingPanel.StartToEnableButton();
            return;
        }
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f);

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_LEVEL_1);

        
    }
}
