    using UnityEngine;

    public class BoulderSpawner : MonoBehaviour
    {
    [SerializeField] private GameObject _boulder;
    [SerializeField] private Transform _spawnPoint;
    private bool _hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _hasSpawned) return;
        _hasSpawned = true;
        SpawnBoulder(other.transform);
    }

    private void SpawnBoulder(Transform kart)
    {
        GameObject clonedBoulder = Instantiate(_boulder, _spawnPoint.position, transform.rotation);

        Rigidbody boulderRb = clonedBoulder.GetComponent<Rigidbody>();
        if (boulderRb != null)
        {
            boulderRb.AddForce(
                Vector3.forward * 10f + Vector3.down * 5f, ForceMode.Impulse);
        }

        BoulderHitKart hitScript = clonedBoulder.GetComponent<BoulderHitKart>();
        if (hitScript != null)
        {
            hitScript.SetKart(kart);
        }

        RedScreenController vignette = FindObjectOfType<RedScreenController>();
        if (vignette != null)
        {
            vignette.SetBoulder(clonedBoulder.transform);
        }

    }

    }
