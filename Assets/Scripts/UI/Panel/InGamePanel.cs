using Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InGamePanel : PanelView, ITouchTarget
{
    [SerializeField]
    private RectTransform _rect;

    [SerializeField]
    private JoystickUI _leftJoystick;

    [SerializeField]
    private JoystickUI _righttJoystick;

    [SerializeField]
    private RectTransform _leftJoystickOriginPosition;

    [SerializeField]
    private RectTransform _rightJoystickOriginPosition;

    [Header("Health Bars")]
    [SerializeField]
    private HealthBarUI _playerHealthBar;

    [SerializeField]
    private HealthBarUI _enemyHealthBarPrefab;

    [SerializeField]
    private RectTransform _enemyHealthBarContainer;

    [SerializeField]
    private Vector3 _enemyHealthBarOffset = new Vector3(0, 2f, 0);

    private JoyStickHandler _joyStickHandler;
    private Camera _worldCamera;
    private Camera _uiCamera;
    private Player _player;

    public Action OnSwapWeaponClicked { get; set; }

    public Action OnUseGrenadeClicked { get; set; }

    public bool IsActive => gameObject.activeSelf;

    private readonly List<HealthBarUI> _enemyHealthBarCache = new List<HealthBarUI>();
    private readonly Dictionary<Enemy, HealthBarUI> _activeEnemyHealthBars = new Dictionary<Enemy, HealthBarUI>();
    private readonly Dictionary<Enemy, System.Action<int, int>> _enemyHealthCallbacks = new Dictionary<Enemy, System.Action<int, int>>();

    private const int PRELOAD_COUNT = 50;

    // Singleton instance for easy access
    public static InGamePanel Instance { get; private set; }

    protected override void OnPanelShowed(params object[] args)
    {
        Instance = this;

        _joyStickHandler = args[0] as JoyStickHandler;
        if (_joyStickHandler == null)
        {
            Debug.LogError("JoyStickHandler is null. Cannot setup InGamePanel.");
            return;
        }

        _worldCamera = CameraManager.Instance.MainCamera;
        _uiCamera = CameraManager.Instance.UICamera.Camera;

        _leftJoystick.transform.position = _leftJoystickOriginPosition.position;
        _righttJoystick.transform.position = _rightJoystickOriginPosition.position;
        _leftJoystick.Setup(_joyStickHandler, Joystick.Left);
        _righttJoystick.Setup(_joyStickHandler, Joystick.Right);

        PreloadEnemyHealthBars();
    }

    protected override void OnPanelHided(params object[] args)
    {
        ClearAllEnemyHealthBars();

        if (_player != null && _player.Health != null)
        {
            _player.Health.OnHealthChanged -= OnPlayerHealthChanged;
        }

        Instance = null;
    }

    private void Update()
    {
        UpdateEnemyHealthBarPositions();
    }

    public void SetupPlayerHealthBar(Player  player)
    {
        _player = player;
        if (_player == null)
        {
            Debug.LogError("Player not found!");
            return;
        }

        if (_playerHealthBar == null)
        {
            Debug.LogError("PlayerHealthBar UI component not assigned!");
            return;
        }

        if (_player.Health == null)
        {
            Debug.LogError("Player HealthComponent not found!");
            return;
        }

        float maxHealth = (float)_player.PlayerData.MaxHealth;
        _playerHealthBar.Initialize(maxHealth);

        _player.Health.OnHealthChanged += OnPlayerHealthChanged;

        float currentHealth = (float)_player.Health.CurrentHealth;
        OnPlayerHealthChanged((int)currentHealth, (int)maxHealth);

        Debug.Log($"Player health bar initialized - Current: {currentHealth}, Max: {maxHealth}");
    }

    private void OnPlayerHealthChanged(int currentHealth, int maxHealth)
    {
        if (_playerHealthBar != null)
        {
            // Convert int to float for HealthBarUI
            _playerHealthBar.UpdateHealth((float)currentHealth, (float)maxHealth);
            Debug.Log($"Player health updated - Current: {currentHealth}, Max: {maxHealth}");
        }
    }

    // Public method to test player health bar
    [ContextMenu("Test Player Damage")]
    public void TestPlayerDamage()
    {
        if (_player != null)
        {
            _player.TakeDamage(20);
        }
    }

    [ContextMenu("Test Player Heal")]
    public void TestPlayerHeal()
    {
        if (_player != null && _player.Health != null)
        {
            _player.Health.Heal(30);
        }
    }

    private void PreloadEnemyHealthBars()
    {
        for (int i = 0; i < PRELOAD_COUNT; i++)
        {
            HealthBarUI healthBar = CreateEnemyHealthBar();
            healthBar.SetVisibility(false);
            _enemyHealthBarCache.Add(healthBar);
        }
    }

    private HealthBarUI CreateEnemyHealthBar()
    {
        GameObject healthBarObj = Instantiate(_enemyHealthBarPrefab.gameObject, _enemyHealthBarContainer);
        HealthBarUI healthBar = healthBarObj.GetComponent<HealthBarUI>();
        return healthBar;
    }

    public HealthBarUI GetEnemyHealthBar(Enemy enemy)
    {
        if (_activeEnemyHealthBars.ContainsKey(enemy))
        {
            return _activeEnemyHealthBars[enemy];
        }

        HealthBarUI healthBar = GetFromCache();
        if (healthBar == null)
        {
            healthBar = CreateEnemyHealthBar();
        }

        // Initialize with enemy max health
        float enemyMaxHealth = (float)enemy.EnemyData.MaxHealth;
        healthBar.Initialize(enemyMaxHealth);

        // Store callback reference for proper cleanup
        System.Action<int, int> healthCallback = (current, max) =>
        {
            healthBar.UpdateHealth((float)current, (float)max);
        };
        _enemyHealthCallbacks[enemy] = healthCallback;

        enemy.HealthComponent.OnHealthChanged += healthCallback;
        enemy.OnEnemyDied += ReturnEnemyHealthBarToCache;

        _activeEnemyHealthBars[enemy] = healthBar;
        UpdateEnemyHealthBarPosition(enemy, healthBar);

        // Set initial health
        float currentHealth = (float)enemy.HealthComponent.CurrentHealth;
        healthBar.UpdateHealth(currentHealth, enemyMaxHealth);
        healthBar.SetVisibility(true);

        return healthBar;
    }

    private HealthBarUI GetFromCache()
    {
        if (_enemyHealthBarCache.Count > 0)
        {
            HealthBarUI healthBar = _enemyHealthBarCache[0];
            _enemyHealthBarCache.RemoveAt(0);
            return healthBar;
        }
        return null;
    }

    private void ReturnEnemyHealthBarToCache(Enemy enemy)
    {
        if (_activeEnemyHealthBars.TryGetValue(enemy, out HealthBarUI healthBar))
        {
            _activeEnemyHealthBars.Remove(enemy);

            // Unsubscribe from events to prevent memory leaks
            if (_enemyHealthCallbacks.TryGetValue(enemy, out var callback))
            {
                enemy.HealthComponent.OnHealthChanged -= callback;
                _enemyHealthCallbacks.Remove(enemy);
            }
            enemy.OnEnemyDied -= ReturnEnemyHealthBarToCache;

            healthBar.SetVisibility(false);
            healthBar.transform.SetAsLastSibling();

            _enemyHealthBarCache.Add(healthBar);
        }
    }

    private void UpdateEnemyHealthBarPositions()
    {
        foreach (var kvp in _activeEnemyHealthBars.ToArray())
        {
            Enemy enemy = kvp.Key;
            HealthBarUI healthBar = kvp.Value;

            if (enemy != null && healthBar != null)
            {
                UpdateEnemyHealthBarPosition(enemy, healthBar);
            }
            else if (enemy == null)
            {
                ReturnEnemyHealthBarToCache(enemy);
            }
        }
    }

    private void UpdateEnemyHealthBarPosition(Enemy enemy, HealthBarUI healthBar)
    {
        Vector3 worldPosition = enemy.transform.position + _enemyHealthBarOffset;
        Vector2 screenPosition = WorldToUIPosition(worldPosition);

        RectTransform healthBarRect = healthBar.GetComponent<RectTransform>();
        healthBarRect.anchoredPosition = screenPosition;

        bool isVisible = IsEnemyVisible(enemy.transform.position);
        healthBar.SetVisibility(isVisible);
    }

    private bool IsEnemyVisible(Vector3 worldPosition)
    {
        if (_worldCamera == null) return false;

        Vector3 viewportPoint = _worldCamera.WorldToViewportPoint(worldPosition);
        return viewportPoint.z > 0 &&
               viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1;
    }

    private void ClearAllEnemyHealthBars()
    {
        foreach (var kvp in _activeEnemyHealthBars.ToArray())
        {
            ReturnEnemyHealthBarToCache(kvp.Key);
        }
    }

    private Vector2 WorldToUIPosition(Vector3 worldPosition)
    {
        if (_worldCamera == null || _uiCamera == null)
        {
            return Vector2.zero;
        }

        Vector3 screenPosition = _worldCamera.WorldToScreenPoint(worldPosition);

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rect,
            screenPosition,
            _uiCamera,
            out localPosition
        );

        return localPosition;
    }

    public HealthBarUI RegisterEnemy(Enemy enemy)
    {
        if (enemy != null && !_activeEnemyHealthBars.ContainsKey(enemy))
        {
            return GetEnemyHealthBar(enemy);
        }
        return null;
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            ReturnEnemyHealthBarToCache(enemy);
        }
    }

    public void SetEnemyHealthBarOffset(Vector3 offset)
    {
        _enemyHealthBarOffset = offset;
    }

    public void TouchedDown(InputData inputData)
    {
        Joystick joystick = RectUtil.GetJoystickFromTouchPosition(inputData.TouchPosition, 0.5f);
        Vector2 worldPos = _uiCamera.ScreenToWorldPoint(inputData.TouchPosition);
        Vector2 localTouchPos = _rect.InverseTransformPoint(worldPos);

        switch (joystick)
        {
            case Joystick.Left:
                _leftJoystick.transform.localPosition = localTouchPos;
                break;
            case Joystick.Right:
                _righttJoystick.transform.localPosition = localTouchPos;
                break;
            default:
                Debug.LogWarning("Unknown joystick type for touch position: " + inputData.TouchPosition);
                break;
        }
    }

    public void Dragged(InputData inputData)
    {
    }

    public void TouchedUp(InputData inputData)
    {
        _leftJoystick.transform.localPosition = _leftJoystickOriginPosition.localPosition;
        _righttJoystick.transform.localPosition = _rightJoystickOriginPosition.localPosition;
    }

    public void SwapWeaponButton()
    {
        OnSwapWeaponClicked?.Invoke();
    }

    public void UseGrenadeButton()
    {
        OnUseGrenadeClicked?.Invoke();
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}