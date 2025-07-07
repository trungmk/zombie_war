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

                inGamePanel.OnUseGrenadeClicked = WeaponManager.Instance.Handle_EventUseGrenade;
                inGamePanel.OnSwapWeaponClicked = WeaponManager.Instance.Handle_EventSwapWeapon;

                Player player = _gameManager.Player;
                inGamePanel.SetupPlayerHealthBar(player);
            });

        _gameManager.StartGame((player) =>
        {
            SetupHealthBarForPlayer(player).Forget();
        });
    }

    private async UniTaskVoid SetupHealthBarForPlayer(Player player)
    {
        while(UIManager.Instance.GetCache<InGamePanel>() == null)
        {
            await UniTask.Yield();
        }

        InGamePanel inGamePanel = UIManager.Instance.GetCache<InGamePanel>();
        inGamePanel.SetupPlayerHealthBar(player);
    }

    public override void OnUnloaded()
    {
        MobileInput.Instance.SetInputFilter(null);
        UIManager.Instance.GetCache<InGamePanel>();
    }
}
