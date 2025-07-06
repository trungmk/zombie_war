using Core;
using UnityEngine;
using MEC;
using System.Collections;
using System.Collections.Generic;

public class BootingSceneController : SceneController
{
    public override void OnLoaded()
    {
        ObjectPooling.Instance.Init(null);
        UIManager.Instance.Show<LoadingPanel>();
        Timing.RunCoroutine(ChangeScene());
    }

    private IEnumerator<float> ChangeScene()
    {
        yield return Timing.WaitForSeconds(2f);

        CoreSceneManager.Instance.ChangeScene(ContextNameGenerated.CONTEXT_LEVEL_1);

        UIManager.Instance.Hide<LoadingPanel>(isDisable: true, isDestroy: true);
    }
}
