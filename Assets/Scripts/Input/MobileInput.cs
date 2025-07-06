using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Touch = UnityEngine.Touch;

public class MobileInput : MonoSingleton<MobileInput>
{
    [SerializeField]
    private bool _isEnableMultiTouch = false;

    [SerializeField]
    private float _timeHoldDrag = 0.06f;

    [SerializeField]
    private float _minMagnitudeDirection = 5f;

    private readonly Dictionary<int, TouchData> _activeTouches = new Dictionary<int, TouchData>();
    private Camera _uiCamera;
    private IInputHandler _inputFilter;
    private string _joystickName = "Joystick";

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
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            int touchId = touch.fingerId;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsTouchOnLayer(touch.position))
                    {
                        HandleTouchDown(touch);
                    }
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
                if (IsTouchOnLayer(touch.position))
                {
                    HandleTouchDown(touch);
                }
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
        if (IsTouchOverUI(screenPosition))
        {
            return false;
        }

        return true;
    }

    private bool IsTouchOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name.Contains(_joystickName))
            {
                return false;
            }
        }

        return results.Count > 0;
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