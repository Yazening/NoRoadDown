using UnityEngine;
using UnityEngine.VFX;

// Boost VFX Controller
public class VFXController : MonoBehaviour
{
    [SerializeField] private VisualEffect[] _boostVFX; // array of vfx's are on when boosting

    private CarController _carController;

    private void Awake()
    {
        _carController = GetComponent<CarController>();

        StopAll(_boostVFX); // nothing is playing on start
    }

    private void Update()
    {
        HandleVFX();
    }

    private void HandleVFX()
    {
        // seas the state of the boost if its boosting = boost vfx on, vice versa
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