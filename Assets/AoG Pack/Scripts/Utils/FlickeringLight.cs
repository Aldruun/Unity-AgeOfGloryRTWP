using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private float _baseIntensity;
    private bool _flickering;

    private Light _lightSource;
    public float MaxIncrease;

    public float MaxReduction;
    public float RateDamping;
    public bool StopFlickering;
    public float Strength;

    public void Reset()
    {
        MaxReduction = 0.2f;
        MaxIncrease = 0.2f;
        RateDamping = 0.1f;
        Strength = 300;
    }

    public void Start()
    {
        _lightSource = GetComponent<Light>();
        if (_lightSource == null)
        {
            Debug.LogError("Flicker script must have a Light Component on the same GameObject.");
            return;
        }

        _baseIntensity = _lightSource.intensity;
        StartCoroutine(DoFlicker());
    }

    private void Update()
    {
        if (!StopFlickering && !_flickering) StartCoroutine(DoFlicker());
    }

    private IEnumerator DoFlicker()
    {
        _flickering = true;

        while (!StopFlickering)
        {
            _lightSource.intensity = Mathf.Lerp(_lightSource.intensity,
                Random.Range(_baseIntensity - MaxReduction, _baseIntensity + MaxIncrease), Strength * Time.deltaTime);
            yield return new WaitForSeconds(RateDamping);
        }

        _flickering = false;
    }
}