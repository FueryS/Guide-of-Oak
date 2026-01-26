using UnityEngine;

public class Sloperotater : MonoBehaviour
{
    [SerializeField] Transform Transform;
    [SerializeField] float xRotation = 10;
    [SerializeField] float zRotation = 0;
    float t = 0f;

    private void Start()
    {
        t = Time.time;
    }
    private void FixedUpdate()
    {
        if (Time.time - t < 1f) return;
        t = Time.time;
        CopyPlayerRotation();
    }
    void CopyPlayerRotation()
    {
        Vector3 targetEuler = Transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(xRotation, (targetEuler.y * -1), zRotation);
        //Debug.Log(targetEuler.y);
    }
}
