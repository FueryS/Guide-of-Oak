using UnityEngine;

public class BlockBehaviour : MonoBehaviour
{
    private Transform _target; // No longer public, we find it automatically
    public float reactDistance = 20f;

    private Vector3 _origin;

    private void Start()
    {
        _origin = transform.position;
        transform.position += new Vector3(0, 5, 0);

        // 1. Find the Player automatically
        // Ensure your Player object has the Tag "Player" in the Inspector!
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            _target = playerObj.transform;
        }
        else
        {
            Debug.LogWarning($"Block at {name} couldn't find an object with the 'Player' tag!");
        }
    }

    private void Update()
    {
        if (_target == null) return;

        // 2. Calculate distance from the block's origin to the player's current position
        float currentDistance = Vector3.Distance(_origin, _target.position);

        if (currentDistance > reactDistance)
        {
            Destroy(gameObject);
        }
    }
}