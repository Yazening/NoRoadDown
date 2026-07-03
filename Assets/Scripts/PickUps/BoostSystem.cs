using UnityEngine;
public class BoostSystem : MonoBehaviour
{
    public bool _hasBoost { get; private set; }
    private CarController _carController;
    private void Awake()
    {
        _carController = GetComponent<CarController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Boost")) return;
        if (_hasBoost) return;
        CollectBoost(other.gameObject);
    }

    private void CollectBoost(GameObject pickupObject)
    {
        _hasBoost = true;
        _carController.GiveBoost();
        pickupObject.SetActive(false);
        StartCoroutine(RespawnPickup(pickupObject));
        Debug.Log("Boost pickedup");
    }
    private System.Collections.IEnumerator RespawnPickup(GameObject pickupObject)
    {
        float timer = 0f;
        while (timer < 8f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (pickupObject != null)
        {
            pickupObject.SetActive(true);
            Debug.Log("Boost respawned");
        }
    }
    public void UseBoost()
    {
        _hasBoost = false;
    }
    public void DebugGiveBoost()
    {
        _hasBoost = true;
        _carController.GiveBoost();
    }
}