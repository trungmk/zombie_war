using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DissolveMesh : MonoBehaviour
{
    [SerializeField]
    private Texture2D _dissolveTexture;

    [SerializeField]
    public Color _dissolveColor = Color.red;

    [SerializeField]
    public float _delayBeforeStart = 2f;

    [SerializeField]
    public float _dissolveTime = 2f;

    private Material _material;

    private float dissolveProgress = 0.3f;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
    }

    private IEnumerator<float> DissolveAfterDelay()
    {
        yield return Timing.WaitForSeconds(_delayBeforeStart);

        _material = GetComponent<Renderer>().material;
        _material.shader = Shader.Find("Custom/Dissolve");
        _material.SetTexture("_DissolveTex", _dissolveTexture);
        _material.SetColor("_DissolveColor", _dissolveColor);

        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime / _dissolveTime;
            _material.SetFloat("_DissolveThreshold", dissolveProgress);
            yield return Timing.WaitForOneFrame;
        }
    }

    public void StartToDissolve()
    {
        _material.SetFloat("_DissolveThreshold", 0);
        Timing.RunCoroutine(DissolveAfterDelay());
    }

    public void ResetValues()
    {
        if (_material != null)
        {
            _material.SetFloat("_DissolveThreshold", 0);
        }
    }
}