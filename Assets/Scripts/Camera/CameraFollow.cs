using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 3f, -6f); // distance behind between kart and camera
    [SerializeField] private float _positionSmoothSpeed = 8f;
    [SerializeField] private float _rotationSmoothSpeed = 5f;

    // LateUpdate so the camera moves after the kart moves, this will avoid the jitter and lag, instead of following mid update
    private void LateUpdate()
    {
        
        FollowTarget();
        LookAtTarget();
    }
   
    private void FollowTarget()
    {
        // it follows the kart even if it turns.
        Vector3 cameraPosition = _target.position + _target.TransformDirection(_offset);
        transform.position = Vector3.Lerp(transform.position, cameraPosition, _positionSmoothSpeed * Time.deltaTime);
    }

    private void LookAtTarget()
    {
        // smoothly rotate to face the kart
        Quaternion cameraPosition = Quaternion.LookRotation( _target.position - transform.position);
        transform.rotation = Quaternion.Slerp( transform.rotation, cameraPosition, _rotationSmoothSpeed * Time.deltaTime);
    }
}


