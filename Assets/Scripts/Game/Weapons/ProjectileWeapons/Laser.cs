using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [SerializeField] 
    private LayerMask _hitMask;

    [SerializeField]
    private LineRenderer _lineRenderer;

    private void Awake()
    {
        if (_lineRenderer.positionCount != 2)
        {
            _lineRenderer.positionCount = 2;
        }    
    }

    public void UpdateLaser(Vector3 origin, Vector3 direction, float maxDistance = 50f)
    {
        _lineRenderer.SetPosition(0, origin);

        RaycastHit hit;
        Vector3 endPoint = origin + direction.normalized * maxDistance;
        if (Physics.Raycast(origin, direction, out hit, maxDistance, _hitMask))
        {
            endPoint = hit.point;
        }

        _lineRenderer.SetPosition(1, endPoint);
    }

    public void Hide()
    {
        _lineRenderer.enabled = false;
    }

    public void Show()
    {
        _lineRenderer.enabled = true;
    }
}