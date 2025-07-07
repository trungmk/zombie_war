using Core;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private LevelManager _levelManager;

    [SerializeField]
    private JoyStickHandler _joystickHandler;

    [SerializeField]
    private Player _playerControllerPrefab;

    [SerializeField]
    private Transform _playerSpawnPosition;

    [SerializeField]
    private Transform _playerHolder;

    private CinemachineCamera _cinemachineCamera;

    public Player Player { get; private set; }

    private void Awake()
    {
        _levelManager.OnLevelCompleted += OnLevelCompleted;
        _levelManager.OnLevelFailed += OnLevelFailed;
        _levelManager.OnWaveStarted += OnWaveStarted;
        _cinemachineCamera = CameraManager.Instance.CinemachineCamera;
    }

    public void StartGame(Action<Player> callback)
    {
        WeaponManager.Instance.Init();

        Player playerGO = Instantiate(_playerControllerPrefab);
        playerGO.transform.position = _playerSpawnPosition.position;
        playerGO.transform.rotation = Quaternion.identity;
        playerGO.transform.SetParent(_playerHolder);

        SetupCamera(playerGO.transform);

        PlayerInputHandler playerInputHandler = playerGO.GetComponent<PlayerInputHandler>();

        if (playerInputHandler != null)
        {
            _joystickHandler.RegisterHandleJoyStickDirection(Joystick.Left, playerInputHandler.LeftJoyStick);
            _joystickHandler.RegisterHandleJoyStickDirection(Joystick.Right, playerInputHandler.RightJoyStick);
        }

        Player = playerGO;

        if (callback != null)
        {
            callback(playerGO);
        }
    }

    private void OnWaveStarted(int obj)
    {
        
    }

    private void OnLevelFailed()
    {
        
    }

    private void OnLevelCompleted()
    {
        
    }

    private void SetupCamera(Transform playerTransform)
    {
        if (_cinemachineCamera == null)
        {
            Debug.LogError("CinemachineCamera not assigned!");
            return;
        }

        _cinemachineCamera.Follow = playerTransform;
        _cinemachineCamera.LookAt = playerTransform;
    }
}
