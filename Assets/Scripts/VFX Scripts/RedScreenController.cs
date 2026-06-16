using UnityEngine;
using UnityEngine.UI;
public class RedScreenController : MonoBehaviour
{
    [SerializeField] private Image _redScreen;
    [SerializeField] private Image _textImage;
    [SerializeField] private Transform _kart;

    [SerializeField] private float _dangerStartDistance = 20f;
    [SerializeField] private float _dangerClosestDistance = 3f;

    [SerializeField] private float _maxRedIntensity = 0.7f;
    [SerializeField] private float _fadeSpeed = 5f;

    [SerializeField] private bool _pulseEnabled = true;
    [SerializeField] private float _pulseFrequency = 3f;
    [SerializeField] private float _pulseStrength = 0.1f;

    private Transform _boulder;
    private float _currentAlpha = 0f;
    private bool _boulderFound = false;

    private void Update()
    {
        UpdateRedScreen();
    }

    public void SetBoulder(Transform boulder)
    {
        _boulder = boulder;
        _boulderFound = true;
    }

    private void UpdateRedScreen()
    {
        if (!_boulderFound || _boulder == null)
        {
            SetRedAlpha(0f);
            SetTextAlpha(0f);
            return;
            
        }

        float distanceToBoulder = Vector3.Distance(_kart.position, _boulder.position);
        float targetIntensity = Mathf.InverseLerp( _dangerStartDistance, _dangerClosestDistance, distanceToBoulder);

        targetIntensity = Mathf.Clamp(targetIntensity, 0f, 1f) * _maxRedIntensity;

        if (_pulseEnabled && targetIntensity > 0.4f)
        {
            float pulse = Mathf.Sin(Time.time * _pulseFrequency) * _pulseStrength;
            targetIntensity += pulse;   
        }

        _currentAlpha = Mathf.Lerp(_currentAlpha, targetIntensity, _fadeSpeed * Time.deltaTime);

        SetRedAlpha(_currentAlpha);
        SetTextAlpha(_currentAlpha); 
    }

    private void SetRedAlpha(float alpha)
    {
        Color color = _redScreen.color;
        color.a = Mathf.Clamp01(alpha);
        _redScreen.color = color;

    }
    
    private void SetTextAlpha(float alpha)
    {
        Color color = _textImage.color;
        color.a = Mathf.Clamp01(alpha);
        _textImage.color = color;
    }

}
