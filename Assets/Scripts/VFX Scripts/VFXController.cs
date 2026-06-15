using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _boostVFX;

    private CarController _carController;

    private void Awake()
    {
        _carController = GetComponent<CarController>();

        StopAll(_boostVFX);
    }

    private void Update()
    {
        HandleVFX();
    }

    private void HandleVFX()
    {
        if (_carController.CurrentState == KartState.Boosting)
        {
            PlayAll(_boostVFX);
        }
        else
        {
            StopAll(_boostVFX);
        }
    }

    private void PlayAll(VisualEffect[] vfxArray)
    {
        foreach (VisualEffect vfx in vfxArray)
        {
            if (vfx != null) vfx.Play();
        }
    }

    private void StopAll(VisualEffect[] vfxArray)
    {
        foreach (VisualEffect vfx in vfxArray)
        {
            if (vfx != null) vfx.Stop();
        }
    }
}