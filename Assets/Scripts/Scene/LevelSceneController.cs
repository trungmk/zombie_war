using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelSceneController : SceneController
{
    [SerializeField]
    private JoyStickHandler _joyStickHandler;

    [SerializeField]
    private GameManager _gameManager;

    private readonly InputHandler _inputHandler = new InputHandler();

    public override void OnLoaded()
    {
        MobileInput.Instance.SetInputFilter(_inputHandler);
        _inputHandler.RegisterTouchTarget(_joyStickHandler);
        UIManager.Instance.Show<InGamePanel>(_joyStickHandler)
            .OnShowCompleted(view =>
            {
                InGamePanel inGamePanel = view as InGamePanel;
                _inputHandler.RegisterTouchTarget(inGamePanel);
                inGamePanel.OnSwapWeaponClicked = WeaponManager.Instance.Handle_EventSwapWeapon;;
            });

        _gameManager.StartGame((player) =>
        {
            SetupUI(player).Forget();
        });
    }

    private async UniTaskVoid SetupUI(Player player)
    {
        while(UIManager.Instance.GetCache<InGamePanel>() == null)
        {
            await UniTask.Yield();
        }

        InGamePanel inGamePanel = UIManager.Instance.GetCache<InGamePanel>();
        inGamePanel.SetupPlayerHealthBar(player);
        inGamePanel.OnUseGrenadeClicked = player.GetComponent<PlayerInputHandler>().OnGrenadeButton;

        LoadingTransition loadingPanel = UIManager.Instance.GetCache<LoadingTransition>();
        if (loadingPanel != null)
        {
            loadingPanel.StartToEnableButton();
            return;
        }
    }

    public override void OnUnloaded()
    {
        MobileInput.Instance.SetInputFilter(null);
        UIManager.Instance.GetCache<InGamePanel>();
    }
}
