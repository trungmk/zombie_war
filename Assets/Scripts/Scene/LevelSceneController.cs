using Core;
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
            });

        _gameManager.StartGame();
    }

    public override void OnUnloaded()
    {
        MobileInput.Instance.SetInputFilter(null);
        UIManager.Instance.GetCache<InGamePanel>();
    }
}
