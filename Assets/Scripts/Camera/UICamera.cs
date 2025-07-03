using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField]
    private Camera _uiCamera;

    [Header("Reference resolution (width:height)")]
    [SerializeField] 
    private float _referenceWidth = 1080f;

    [SerializeField] 
    private float _referenceHeight = 1920f;

    [Header("Reference orthographic size (for vertical or horizontal)")]
    [SerializeField] 
    private float _referenceOrthoSizeVertical = 10.5f;

    [SerializeField] 
    private float _referenceOrthoSizeHorizontal = 10.5f;

    [Header("Orientation priority")]
    [SerializeField] 
    private bool isVertical;

    [SerializeField]
    private bool isHorizontal = true;

    public bool IsScaleByVertical { get; private set; } = false;

    public bool IsScaleByHorizontal { get; private set; } = false;

    public Camera Camera => _uiCamera;

    public void Setup()
    {
        AdjustCamera();
    }

    private void AdjustCamera()
    {
        float targetAspect = _referenceWidth / _referenceHeight;
        float windowAspect = (float)Screen.width / Screen.height;
        if (isVertical)
        {
            if (windowAspect > targetAspect)
            {
                _uiCamera.orthographicSize = _referenceOrthoSizeVertical;
                IsScaleByVertical = false;
            }
            else
            {
                float scaleFactor = targetAspect / windowAspect;
                _uiCamera.orthographicSize = _referenceOrthoSizeVertical * scaleFactor;
                IsScaleByVertical = true;
            }
        }
        else if (isHorizontal)
        {
            float reverseTargetAspect = _referenceHeight / _referenceWidth;
            float windowAspectInv = (float)Screen.height / Screen.width;
            if (windowAspectInv > reverseTargetAspect)
            {
                _uiCamera.orthographicSize = _referenceOrthoSizeHorizontal;
                IsScaleByHorizontal = false;
            }
            else
            {
                float scaleFactor = reverseTargetAspect / windowAspectInv;
                _uiCamera.orthographicSize = _referenceOrthoSizeHorizontal * scaleFactor;
                IsScaleByHorizontal = true;
            }
        }
    }
}
