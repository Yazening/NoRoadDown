using UnityEngine;
public class ObstacleManager : MonoBehaviour
{
    public enum ObstacleType
    {
        Static,
        Moving,
        Falling
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
    [SerializeField] private float _fallDelay = 2f;
    [SerializeField] private float _warningDuration = 1.5f;
    [SerializeField] private GameObject _shadowIndicator;
    [SerializeField] private float _fallForce = 30f;

    [Header("Falling Settings")]
    [SerializeField] private GameObject _VFXHitImpact; 

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

            if (_rb != null)
            {
                _rb.isKinematic = true;
            }

            if (_shadowIndicator != null)
            {
                _shadowIndicator.SetActive(false);
            }

            StartCoroutine(FallCycle());
        }
    }

    private void Update()
    {
        if (_obstacleType == ObstacleType.Moving)
        {
            MoveObstacle();
        }
    }

    private void MoveObstacle()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        if (distanceFromStart >= _moveDistance)
        {
            _moveDirection = -1f;
        }
        else if (distanceFromStart <= -_moveDistance)
        {
            _moveDirection = 1f;
        }

        transform.Translate(Vector3.right * _moveDirection * _moveSpeed * Time.deltaTime, Space.World);
        transform.Rotate(0f, 0f, -_moveDirection * _moveSpeed * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private System.Collections.IEnumerator FallCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fallDelay);

            if (_shadowIndicator != null)
            {
                _shadowIndicator.SetActive(true);
            }

            yield return new WaitForSeconds(_warningDuration);

            if (_shadowIndicator != null)
            {
                _shadowIndicator.SetActive(false);
            }

            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.AddForce(Vector3.down * _fallForce, ForceMode.Impulse);
            }

            _isFalling = true;

            yield return new WaitForSeconds(3f);

            ResetFallingRock();
        }
    }

    private void ResetFallingRock()
    {
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.velocity = Vector3.zero;
        }

        transform.position = _startPosition;
        _isFalling = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        Rigidbody kartRb = other.gameObject.GetComponent<Rigidbody>();
        CarController carController = other.gameObject.GetComponent<CarController>();

        if (kartRb != null && carController != null)
        {
            if (_VFXHitImpact != null)
            {
                ContactPoint contact = other.GetContact(0);
                GameObject vfx = Instantiate(_VFXHitImpact, contact.point, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            Vector3 bounceDirection = other.transform.position - transform.position;
            bounceDirection.y = 0f;
            bounceDirection.Normalize();

            kartRb.velocity = Vector3.zero;
            kartRb.AddForce(bounceDirection * _bounceForce, ForceMode.Impulse);

            StartCoroutine(BounceAndRecover(kartRb, carController));
        }
    }

    private System.Collections.IEnumerator BounceAndRecover( Rigidbody kartRb,  CarController carController)
    {
        carController.enabled = false;
        yield return new WaitForSeconds(_recoveryDelay);
        carController.enabled = true;
    }
}
