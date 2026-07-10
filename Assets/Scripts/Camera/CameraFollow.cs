using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 3f, -6f);
    [SerializeField] private float _positionSmoothSpeed = 8f;
    [SerializeField] private float _rotationSmoothSpeed = 5f;

    private void LateUpdate()
    {
        FollowTarget();
        LookAtTarget();
    }
   
    private void FollowTarget()
    {
        Vector3 cameraPosition = _target.position + _target.TransformDirection(_offset);
        transform.position = Vector3.Lerp(transform.position, cameraPosition, _positionSmoothSpeed * Time.deltaTime);
    }

    private void LookAtTarget()
    {
        Quaternion cameraPosition = Quaternion.LookRotation( _target.position - transform.position);
        transform.rotation = Quaternion.Slerp( transform.rotation, cameraPosition, _rotationSmoothSpeed * Time.deltaTime);
    }
}


