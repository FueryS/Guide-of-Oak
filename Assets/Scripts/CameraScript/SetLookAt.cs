using UnityEngine;

public class SetLookAt : MonoBehaviour
{
    //------- External variables ------------
    [SerializeField] Transform LA_pos;

    public bool look;
    private void Update()
    {
        if (LA_pos == null||!look) return;
        transform.LookAt(LA_pos);
    }
}
