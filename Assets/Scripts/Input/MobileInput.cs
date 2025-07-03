using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.Touch;

public class MobileInput : MonoSingleton<MobileInput>
{
    [SerializeField]
    private bool _isEnableMultiTouch = false;

    [Header("LayerMask to filter touchable objects")]
    [SerializeField]
    private LayerMask _inputLayerMask;

    [SerializeField]
    private float _timeHoldDrag = 0.06f;

    [SerializeField]
    private float _minMagnitudeDirection = 5f;

    private readonly Dictionary<int, TouchData> _activeTouches = new Dictionary<int, TouchData>();
    private Camera _uiCamera;
    private IInputHandler _inputFilter;

    private struct TouchData
    {
        public int TouchId;
        public Vector2 StartPosition;
        public Vector2 LastPosition;
        public float HoldTime;
        public bool IsDragging;
    }

    private void Start()
    {
#if UNITY_EDITOR
        TouchSimulation.Enable();
#endif
        Input.multiTouchEnabled = _isEnableMultiTouch;
        _uiCamera = CameraManager.Instance.UICamera.Camera;
    }

    public void SetInputFilter(IInputHandler inputFilter)
    {
        _inputFilter = inputFilter;
    }

    private void Update()
    {
        if (_isEnableMultiTouch)
        {
            HandleMultiTouch();
        }
        else
        {
            HandleSingleTouch();
        }

        ClearTouchs();
    }

    private void ClearTouchs()
    {
        if (Input.touchCount <= 0)
        {
            if (_activeTouches.Count > 0)
            {
                // End all active touches
                foreach (var touchData in _activeTouches.Values)
                {
                    HandleTouchUp(touchData.TouchId, touchData.LastPosition);
                }
                _activeTouches.Clear();
            }

            return;
        }
    }

    private void HandleMultiTouch()
    {
        // Process all active touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int touchId = touch.fingerId;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchDown(touch);
                    break;

                case TouchPhase.Moved:
                    if (_activeTouches.ContainsKey(touchId))
                    {
                        HandleTouchMove(touch);
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (_activeTouches.ContainsKey(touchId))
                    {
                        HandleTouchUp(touchId, touch.position);
                        _activeTouches.Remove(touchId);
                    }
                    break;
            }
        }

        List<int> touchIds = new List<int>(_activeTouches.Keys);
        foreach (int touchId in touchIds)
        {
            TouchData touchData = _activeTouches[touchId];
            touchData.HoldTime += Time.deltaTime;
            _activeTouches[touchId] = touchData;
        }
    }

    private void HandleSingleTouch()
    {
        if (Input.touchCount <= 0)
        {
            if (_activeTouches.Count > 0)
            {
                foreach (var touchData in _activeTouches.Values)
                {
                    HandleTouchUp(touchData.TouchId, touchData.LastPosition);
                }

                _activeTouches.Clear();
            }

            return;
        }

        Touch touch = Input.GetTouch(0);
        int touchId = touch.fingerId;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                HandleTouchDown(touch);

                break;

            case TouchPhase.Moved:
                if (_activeTouches.ContainsKey(touchId))
                {
                    HandleTouchMove(touch);
                }    

                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (_activeTouches.ContainsKey(touchId))
                {
                    HandleTouchUp(touchId, touch.position);
                    _activeTouches.Remove(touchId);
                }

                break;
        }

        if (_activeTouches.ContainsKey(touchId))
        {
            TouchData touchData = _activeTouches[touchId];
            touchData.HoldTime += Time.deltaTime;
            _activeTouches[touchId] = touchData;
        }
    }

    private bool IsTouchOnLayer(Vector2 screenPosition)
    {
        if (_uiCamera == null)
        {
            Debug.LogWarning("UI Camera is not set. Cannot check touch layer.");
            return false;
        }

        Vector2 worldPoint = _uiCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D ray = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, _inputLayerMask);
        return ray.collider != null;
    }

    private void HandleTouchMove(Touch touch)
    {
        int touchId = touch.fingerId;
        if (!_activeTouches.ContainsKey(touchId))
        {
            return;
        }

        TouchData touchData = _activeTouches[touchId];
        touchData.LastPosition = touch.position;

        if (touchData.HoldTime > _timeHoldDrag)
        {
            Vector2 direction = touch.position - touchData.StartPosition;
            bool isInputMoving = direction.sqrMagnitude >= _minMagnitudeDirection;

            if (isInputMoving)
            {
                if (!touchData.IsDragging)
                {
                    touchData.IsDragging = true;
                }

                if (_inputFilter != null)
                {
                    _inputFilter.OnUserDrag(new InputData(touchId, touch.position));
                }

                touchData.StartPosition = touch.position;
            }
        }

        _activeTouches[touchId] = touchData;
    }

    private void HandleTouchDown(Touch touch)
    {
        int touchId = touch.fingerId;

        TouchData touchData = new TouchData
        {
            StartPosition = touch.position,
            LastPosition = touch.position,
            HoldTime = 0f,
            IsDragging = false
        };

        _activeTouches[touchId] = touchData;

        if (_inputFilter != null)
        {
            _inputFilter.OnUserPress(new InputData(touchId, touch.position));
        }
    }

    private void HandleTouchUp(int touchId, Vector2 position)
    {
        if (_inputFilter != null)
        {
            _inputFilter.OnUserRelease(new InputData(touchId, position));
        }
    }
}