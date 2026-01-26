using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] Transform p;
    [SerializeField] Vector3 offset;

    private void Update()
    {
        transform.position = p.position+offset;
    }
}
