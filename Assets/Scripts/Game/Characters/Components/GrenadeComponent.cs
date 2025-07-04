using UnityEngine;
using System.Collections;

public class GrenadeComponent : MonoBehaviour
{
    [Header("Grenade Settings")]
    [SerializeField] private GrenadeData _currentGrenadeData;
    [SerializeField] private Transform _throwPoint;
    [SerializeField] private LayerMask _damageableLayers = -1;

    [Header("UI Indicator")]
    [SerializeField] private LineRenderer _trajectoryLine;
    [SerializeField] private int _trajectoryPoints = 30;

    // Events
    public System.Action<int> OnGrenadeCountChanged;
    public System.Action OnGrenadeThrown;

    private AudioSource _audioSource;
    private bool _isAiming = false;

    //private void Awake()
    //{
    //    _audioSource = GetComponent<AudioSource>();

    //    if (_trajectoryLine != null)
    //    {
    //        _trajectoryLine.enabled = false;
    //    }
    //}

    //public void StartAiming(Vector2 aimDirection)
    //{
    //    if (!CanThrowGrenade()) return;

    //    _isAiming = true;
    //    if (_trajectoryLine != null)
    //    {
    //        _trajectoryLine.enabled = true;
    //        UpdateTrajectoryPreview(aimDirection);
    //    }
    //}

    //public void UpdateAiming(Vector2 aimDirection)
    //{
    //    if (!_isAiming) return;

    //    if (_trajectoryLine != null)
    //    {
    //        UpdateTrajectoryPreview(aimDirection);
    //    }
    //}

    //public void ThrowGrenade(Vector2 aimDirection)
    //{
    //    if (!CanThrowGrenade()) return;

    //    _isAiming = false;
    //    if (_trajectoryLine != null)
    //    {
    //        _trajectoryLine.enabled = false;
    //    }

    //    // Use grenade from inventory
    //    if (_playerStats != null && !_playerStats.UseGrenade())
    //    {
    //        return;
    //    }

    //    // Calculate throw direction and force
    //    Vector3 throwDirection = new Vector3(aimDirection.x, 0.3f, aimDirection.y).normalized;
    //    Vector3 throwPosition = _throwPoint != null ? _throwPoint.position : transform.position + Vector3.up;

    //    // Create grenade projectile
    //    GameObject grenadeObj = CreateGrenadeProjectile(throwPosition, throwDirection);

    //    // Play pin pull sound
    //    if (_audioSource && _currentGrenadeData.pinPullSound)
    //    {
    //        _audioSource.PlayOneShot(_currentGrenadeData.pinPullSound);
    //    }

    //    OnGrenadeThrown?.Invoke();
    //    OnGrenadeCountChanged?.Invoke(_playerStats.BaseData.grenades);
    //}

    //private GameObject CreateGrenadeProjectile(Vector3 position, Vector3 direction)
    //{
    //    // Create grenade GameObject
    //    GameObject grenadeObj = new GameObject("Grenade");
    //    grenadeObj.transform.position = position;

    //    // Add components
    //    var rigidbody = grenadeObj.AddComponent<Rigidbody>();
    //    var collider = grenadeObj.AddComponent<SphereCollider>();
    //    collider.radius = 0.1f;

    //    // Add grenade projectile component
    //    var projectile = grenadeObj.AddComponent<GrenadeProjectile>();
    //    projectile.Initialize(_currentGrenadeData, _damageableLayers);

    //    // Apply throw force
    //    Vector3 force = direction * _currentGrenadeData.throwForce;
    //    rigidbody.AddForce(force, ForceMode.VelocityChange);
    //    rigidbody.AddTorque(Random.insideUnitSphere * 5f, ForceMode.VelocityChange);

    //    return grenadeObj;
    //}

    //private void UpdateTrajectoryPreview(Vector2 aimDirection)
    //{
    //    Vector3 throwDirection = new Vector3(aimDirection.x, 0.3f, aimDirection.y).normalized;
    //    Vector3 startPosition = _throwPoint != null ? _throwPoint.position : transform.position + Vector3.up;
    //    Vector3 velocity = throwDirection * _currentGrenadeData.throwForce;

    //    Vector3[] points = CalculateTrajectory(startPosition, velocity, _trajectoryPoints);

    //    _trajectoryLine.positionCount = points.Length;
    //    _trajectoryLine.SetPositions(points);
    //}

    //private Vector3[] CalculateTrajectory(Vector3 startPos, Vector3 velocity, int steps)
    //{
    //    Vector3[] points = new Vector3[steps];
    //    float timeStep = 0.1f;

    //    for (int i = 0; i < steps; i++)
    //    {
    //        float time = i * timeStep;
    //        points[i] = startPos + velocity * time + 0.5f * Physics.gravity * time * time;

    //        // Stop trajectory if it hits the ground
    //        if (points[i].y <= 0.1f)
    //        {
    //            points[i].y = 0.1f;
    //            System.Array.Resize(ref points, i + 1);
    //            break;
    //        }
    //    }

    //    return points;
    //}

    //public bool CanThrowGrenade()
    //{
    //    return _currentGrenadeData != null &&
    //           _playerStats != null &&
    //           _playerStats.BaseData.grenades > 0;
    //}

    //public void SetGrenadeData(GrenadeData grenadeData)
    //{
    //    _currentGrenadeData = grenadeData;
    //}

    //public int GetGrenadeCount()
    //{
    //    return _playerStats?.BaseData.grenades ?? 0;
    //}

    //public void StopAiming()
    //{
    //    _isAiming = false;
    //    if (_trajectoryLine != null)
    //    {
    //        _trajectoryLine.enabled = false;
    //    }
    //}
}