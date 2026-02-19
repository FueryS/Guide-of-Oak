using UnityEngine;
using TMPro;
using Unity.AppUI.UI;

[RequireComponent(typeof(SphereCollider))]
public class ResizerUI : MonoBehaviour
{
    #region public
    public Transform cameraTransform;
    public Transform playerTransform;
    public float minScale = 0.6f;
    public float maxScale = 1.2f;
    public float maxDistance = 5f;

    [Tooltip("Enable to auto set max scale based on min scale and max distance")]
    public bool autoResize;
    #endregion 


    #region private
    string playerTag = "Player";
    bool _execute = false;
    TMP_Text _tmp;
    Transform _parent;

    Vector3 _orignalPos;
    #endregion

    private void Start()
    {
        playerTag = playerTransform.tag;
        _tmp = GetComponent<TMP_Text>();

        _parent = GetComponentInParent<Transform>();

        if (_tmp == null)
        {
            Debug.LogError("No TMP_Text component found on " + gameObject.name);
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        SphereCollider collider = GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = maxDistance;

        _orignalPos = transform.localPosition; //record orignal position for scaling issue that I was facing
        _orignalPos.y -= minScale; //remove the min scale componenet as it will be added later anyway

        if (autoResize ) maxScale=minScale*maxDistance;

    }

    private void LateUpdate()
    {
        if (!_execute || cameraTransform == null || _tmp == null || playerTransform == null) 
        {
            _tmp.alpha = 0f;
            return; 
        }

        // Face the camera

        Vector3 direction = transform.position - cameraTransform.position;
        transform.rotation = Quaternion.LookRotation(direction);


        // Scale changes based on distance
        float distancePlayerSelf = Vector3.Distance(transform.position, playerTransform.position);
        float normalisedDistance = Mathf.Clamp01(distancePlayerSelf / maxDistance);
        float scale = Mathf.Lerp(minScale, maxScale, normalisedDistance);
        transform.localScale = Vector3.one * scale;

        //Change Local postion based on current scale
        transform.localPosition =new Vector3(_orignalPos.x,( _orignalPos.y+scale),_orignalPos.z);

        // Alpha fades as distance increases (uses normalizedDistance)
        float alpha = 1f-(normalisedDistance);
        _tmp.alpha = alpha;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _execute = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            _execute = false;
            if (_tmp != null)
            {
                _tmp.alpha = 0f;
            }

        }
    }
}