using UnityEngine;

// handles three obstacles types, static moving, and falling
public class ObstacleManager : MonoBehaviour
{

   
    public enum ObstacleType
    {
        Static, Moving, Falling
    }

    // selects type of obstacle here
    [SerializeField] private ObstacleType _obstacleType;

    [Header("Obstacle Settings")]
    [SerializeField] private float _bounceForce = 8f; // how hard the kart bounces off obstacles
    [SerializeField] private float _recoveryDelay = 0.3f; // _time taken for kart to regain control after hit by obstacle

    [Header("Moving Settings")]
    [SerializeField] private float _moveSpeed = 3f; // move speed of moving obstales side to siide 
    [SerializeField] private float _moveDistance = 4f; // how far it moves
    [SerializeField] private float _rotationSpeed = 50f; // How fast it rotates

    [Header("Falling Settings")]
    [SerializeField] private float _warningDuration = 1.5f; // time taken for shadow to show
    [SerializeField] private GameObject _shadowIndicator; // shadow indicator
    [SerializeField] private float _fallForce = 30f; // speed of falling
    [SerializeField] private Vector3 _shadowMinScale; // shadow size at very start
    [SerializeField] private Vector3 _shadowMaxScale; // shadow largest sizze

    [Header("Effects")]
    [SerializeField] private GameObject _VFXHitImpact;
    [SerializeField] private GameObject _VFXFallingImpact;
   
    // private only used in this script
    private MeshRenderer _meshRenderer;
    private Vector3 _startPosition;
    private float _moveDirection = 1f;
    private bool _isFalling = false;
    private Rigidbody _rb;

    private void Start()
    {
        _startPosition = transform.position;

        // only falling obstacles use rigidbody and mesh off and on 
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

    // shows the order of falling: warning, drop, shadow appears, reset
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

        // measures fall distance before releasing all physics
        float startHeight = transform.position.y;
        float groundHeight = GetHeightObstacleGround();
        float totalFallDistance = startHeight - groundHeight;

        // push rock downward
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.AddForce(Vector3.down * _fallForce, ForceMode.Impulse);
        }

        //increase size of shadow in real time with fall of the boulder
        while (transform.position.y > groundHeight + 0.1f)
        {
            float currentFallDistance = startHeight - transform.position.y;
            float progress = currentFallDistance / totalFallDistance;
            progress = Mathf.Clamp01(progress);

            if (_shadowIndicator != null)
            {
                _shadowIndicator.transform.localScale = Vector3.Lerp(_shadowMinScale, _shadowMaxScale, progress);
            }

            yield return null;
        }

        // rock fully fell, hides shadow 
        if (_shadowIndicator != null)
            _shadowIndicator.SetActive(false);

        yield return new WaitForSeconds(3f);
        ResetFallingRock();
    }

    //shoots a ray downward from the falling rock to measure distance between rock and ground
    private float GetHeightObstacleGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
        {
            return hit.point.y;
        }
        return transform.position.y - 10f;
    }

    // moves moving obstacles left and right and changes direction at each specfic lengths
    private void MoveObstacle()
    {
        float distanceFromStart = transform.position.x - _startPosition.x;

        if (distanceFromStart >= _moveDistance)
            _moveDirection = -1f;
        else if (distanceFromStart <= -_moveDistance)
            _moveDirection = 1f;

        transform.Translate(Vector3.right * _moveDirection * _moveSpeed * Time.deltaTime, Space.World);
        
        // rotates in the dirction of movemnt so it looks logical
        transform.Rotate(0f, 0f, -_moveDirection * _moveSpeed * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    // resets the falling rock back its starting position and hides it again
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

                // spawns vfx at contact point
                if (_VFXHitImpact != null)
                {
                    ContactPoint contact = other.GetContact(0);
                    Debug.Log($"Spawning VFX at: {contact.point}");
                    GameObject vfx = Instantiate(_VFXHitImpact, contact.point, Quaternion.identity);
                    Destroy(vfx, 1f);
                }

                // using contact normal for perfect bounce direction
                ContactPoint contactPoint = other.GetContact(0);
                Vector3 surfaceNormal = contactPoint.normal;

                surfaceNormal.y = 0f;
                surfaceNormal.Normalize();

                // reflects the kart's velocity off the surface
                Vector3 incomingVelocity = kartRb.velocity;
                incomingVelocity.y = 0f;
                Vector3 reflectedDirection = Vector3.Reflect(incomingVelocity.normalized, surfaceNormal);

                StartCoroutine(BounceAndRecover(kartRb, carController, reflectedDirection));
            }
        }

        // show vfx of falling rock when lands at point of contact
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

    // handles the bounce effct when kart hits an obstacle and freezes rotation during the bounce
    private System.Collections.IEnumerator BounceAndRecover(Rigidbody kartRb, CarController carController, Vector3 reflectedDirection)
    {
        carController.enabled = false;

        //
        yield return new WaitForFixedUpdate();

        kartRb.constraints = RigidbodyConstraints.FreezeRotation;

        // locks rotation during bounce
        kartRb.velocity = Vector3.zero;
        kartRb.angularVelocity = Vector3.zero;

        // sets the velocity for a accurate bounce
        Vector3 bounceVelocity = reflectedDirection * _bounceForce; bounceVelocity.y = 0f;
        kartRb.velocity = bounceVelocity;

        yield return new WaitForSeconds(_recoveryDelay);

        kartRb.velocity = Vector3.zero;
        kartRb.constraints = RigidbodyConstraints.FreezeRotationY;

        carController.enabled = true;
    }
}