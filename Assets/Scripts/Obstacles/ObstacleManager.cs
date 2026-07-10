using UnityEngine;
public class ObstacleManager : MonoBehaviour
{
    public enum ObstacleType
    {
        Static, Moving, Falling
    }

    [SerializeField] private ObstacleType _obstacleType;

    [Header("Obstacle Settings")]
    [SerializeField] private float _bounceForce = 8f;
    [SerializeField] private float _recoveryDelay = 0.3f;

    [Header("Moving Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _moveDistance = 4f;
    [SerializeField] private float _rotationSpeed = 50f;

    [Header("Falling Settings")]
    [SerializeField] private float _warningDuration = 1.5f;
    [SerializeField] private GameObject _shadowIndicator;
    [SerializeField] private float _fallForce = 30f;
    [SerializeField] private Vector3 _shadowMinScale;
    [SerializeField] private Vector3 _shadowMaxScale;

    [Header("Effects")]
    [SerializeField] private GameObject _VFXHitImpact;
    [SerializeField] private GameObject _VFXFallingImpact;
    private MeshRenderer _meshRenderer;

    private Vector3 _startPosition;
    private float _moveDirection = 1f;
    private bool _isFalling = false;
    private Rigidbody _rb;

    private void Start()
    {
        _startPosition = transform.position;

        if (_obstacleType == ObstacleType.Falling)
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb != null) _rb.isKinematic = true;
            if (_shadowIndicator != null) _shadowIndicator.SetActive(false);
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
            if (_meshRenderer != null) _meshRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (_obstacleType == ObstacleType.Moving)
            MoveObstacle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_obstacleType != ObstacleType.Falling) return;
        if (!other.CompareTag("Player")) return;
        if (_isFalling) return;

        StartCoroutine(TriggerFall());
    }

    public System.Collections.IEnumerator TriggerFall()
    {
        _isFalling = true;

        if (_meshRenderer != null)
            _meshRenderer.enabled = true;

        if (_shadowIndicator != null)
        {
            _shadowIndicator.SetActive(true);
            _shadowIndicator.transform.localScale = _shadowMinScale;
        }

        yield return new WaitForSeconds(_warningDuration);

        float startHeight = transform.position.y;
        float groundHeight = GetHeightObstacleGround();
        float totalFallDistance = startHeight - groundHeight;

        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.AddForce(Vector3.down * _fallForce, ForceMode.Impulse);
        }

        while (transform.position.y > groundHeight + 0.1f)
        {
            float currentFallDistance = startHeight - transform.position.y;
            float progress = currentFallDistance / totalFallDistance;
            progress = Mathf.Clamp01(progress);

            if (_shadowIndicator != null)
            {
                _shadowIndicator.transform.localScale = Vector3.Lerp(
                    _shadowMinScale,
                    _shadowMaxScale,
                    progress
                );
            }

            yield return null;
        }

        if (_shadowIndicator != null)
            _shadowIndicator.SetActive(false);

        yield return new WaitForSeconds(3f);
        ResetFallingRock();
    }

    private float GetHeightObstacleGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            return hit.point.y;
        }
        return transform.position.y - 10f;
    }

    private void MoveObstacle()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        if (distanceFromStart >= _moveDistance)
            _moveDirection = -1f;
        else if (distanceFromStart <= -_moveDistance)
            _moveDirection = 1f;

        transform.Translate(Vector3.right * _moveDirection * _moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, -_moveDirection * _moveSpeed * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void ResetFallingRock()
    {
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector3.zero;
        }

        if (_meshRenderer != null) _meshRenderer.enabled = false;

        transform.position = _startPosition;
        _isFalling = false;
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Rigidbody kartRb = other.gameObject.GetComponent<Rigidbody>();
            CarController carController = other.gameObject.GetComponent<CarController>();

            if (kartRb != null && carController != null)
            {
                if (_VFXHitImpact != null)
                {
                    ContactPoint contact = other.GetContact(0);
                    Debug.Log($"Spawning VFX at: {contact.point}");
                    GameObject vfx = Instantiate(_VFXHitImpact, contact.point, Quaternion.identity);
                    Destroy(vfx, 1f);
                }

                ContactPoint contactPoint = other.GetContact(0);
                Vector3 surfaceNormal = contactPoint.normal;

                surfaceNormal.y = 0f;
                surfaceNormal.Normalize();

                Vector3 incomingVelocity = kartRb.velocity;
                incomingVelocity.y = 0f;
                Vector3 reflectedDirection = Vector3.Reflect(incomingVelocity.normalized, surfaceNormal);

                StartCoroutine(BounceAndRecover(kartRb, carController, reflectedDirection));
            }
        }

        if (_obstacleType == ObstacleType.Falling && !other.gameObject.CompareTag("Player"))
        {
            if (_VFXFallingImpact != null)
            {
                ContactPoint contact = other.GetContact(0);
                GameObject vfx = Instantiate(_VFXFallingImpact, contact.point, Quaternion.identity);
                vfx.transform.position += Vector3.up * 0.5f;
                Destroy(vfx, 1.5f);
            }

            StartCoroutine(DelayedReset());
        }
    }

    private System.Collections.IEnumerator DelayedReset()
    {
        yield return new WaitForSeconds(0.5f);
        ResetFallingRock();
    }

    private System.Collections.IEnumerator BounceAndRecover(
    Rigidbody kartRb,
    CarController carController,
    Vector3 reflectedDirection)
    {
        carController.enabled = false;

        yield return new WaitForFixedUpdate();

        kartRb.constraints = RigidbodyConstraints.FreezeRotation;

        kartRb.velocity = Vector3.zero;
        kartRb.angularVelocity = Vector3.zero;

        Vector3 bounceVelocity = reflectedDirection * _bounceForce;
        bounceVelocity.y = 0f;
        kartRb.velocity = bounceVelocity;

        yield return new WaitForSeconds(_recoveryDelay);

        kartRb.velocity = Vector3.zero;
        kartRb.constraints = RigidbodyConstraints.FreezeRotationY;

        carController.enabled = true;
    }
}