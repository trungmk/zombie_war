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
        UIManager.Instance.Show<LoadingTransition>()
            .OnShowCompleted(view =>
            {
                LoadingTransition loadingPanel = view as LoadingTransition;
                if (loadingPanel != null)
                {
                    loadingPanel.OnTapToPlayClicked += () =>
                    {
                        UIManager.Instance.Hide<LoadingTransition>(isDisable: true, isDestroy: true);
                    };
                }
            });
    }

    private void Handle_OnLoadPoolsCompleted()
    {
        //LoadingPanel loadingPanel = UIManager.Instance.GetCache<LoadingPanel>();

        //if (loadingPanel != null)
        //{
        //    loadingPanel.StartToEnableButton();
        //    return;
        //}

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_LEVEL_1);
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f);

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_LEVEL_1);

        
    }
}
