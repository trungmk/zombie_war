using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField]
    private LayerMask _hitMask = -1;

    [SerializeField]
    private float _maxDistance = 50f;

    [SerializeField] 
    private bool _isVisible = true;

    [SerializeField] 
    private float _laserWidth = 0.02f;

    [SerializeField] 
    private Material _laserMaterial;

    private LineRenderer _lineRenderer;
    private static readonly RaycastHit[] _raycastHits = new RaycastHit[1];

    public bool IsVisible => _isVisible;
    public Vector3 EndPoint { get; private set; }
    public bool IsHittingTarget { get; private set; }

    private void Awake()
    {
        InitializeLineRenderer();
    }

    private void Start()
    {
        SetVisibility(_isVisible);
    }

    private void InitializeLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null)
        {
            Debug.LogError($"LineRenderer not found on {gameObject.name}");
            return;
        }

        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.sortingOrder = 10;

        if (_laserMaterial != null)
        {
            _lineRenderer.material = _laserMaterial;
        }
    }

    public void UpdateLaser(Vector3 origin, Vector3 direction, float customMaxDistance = 0)
    {
        if (_lineRenderer == null)
        {
            return;
        }

        float maxDist = customMaxDistance == 0 ? _maxDistance: customMaxDistance;
        //Vector3 newDir = new Vector3(direction.x, origin.y, direction.z);
        _lineRenderer.SetPosition(0, origin);
        Vector3 endPoint = origin + direction.normalized * maxDist;
        IsHittingTarget = false;

        int hitCount = Physics.RaycastNonAlloc(origin, direction, _raycastHits, maxDist, _hitMask);
        if (hitCount > 0)
        {
            RaycastHit hit = _raycastHits[0];
            endPoint = hit.point;
            endPoint.y = origin.y; 
            IsHittingTarget = true;
        }

        _lineRenderer.SetPosition(1, endPoint);
        EndPoint = endPoint;
    }

    public void SetVisibility(bool isVisible)
    {
        _isVisible = isVisible;
        if (_lineRenderer != null)
        {
            _lineRenderer.enabled = isVisible;
        }
    }

    public void Hide()
    {
        SetVisibility(false);
    }

    public void Show()
    {
        SetVisibility(true);
    }

    public void SetWidth(float width)
    {
        _laserWidth = width;
        if (_lineRenderer != null)
        {
            _lineRenderer.startWidth = width;
            _lineRenderer.endWidth = width;
        }
    }

    public void SetMaxDistance(float distance)
    {
        _maxDistance = Mathf.Max(0.1f, distance);
    }

    public void SetHitMask(LayerMask mask)
    {
        _hitMask = mask;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!_isVisible)
        {
            return;
        }   

        Gizmos.color = Color.red;
        if (_lineRenderer != null && _lineRenderer.positionCount >= 2)
        {
            Vector3 start = _lineRenderer.GetPosition(0);
            Vector3 end = _lineRenderer.GetPosition(1);
            Gizmos.DrawLine(start, end);

            if (IsHittingTarget)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(end, 0.1f);
            }
        }
    }
#endif
}