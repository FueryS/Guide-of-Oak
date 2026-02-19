using UnityEngine;
using TMPro; // Required for TMP_Text

public class TMPProDistanceFader : MonoBehaviour
{
    #region Variables: Target Settings
    [Header("Targeting")]
    [Tooltip("The exact Name of Object2.")]
    [SerializeField] private string targetName = "Player";

    [Tooltip("The Tag of Object2 to verify the correct object is found.")]
    [SerializeField] private string targetTag = "Player";

    private Transform _targetTransform;
    private TMP_Text _tmpText; // Universal: Works for both UI and World TMP
    #endregion

    #region Variables: Fading Logic
    [Header("Fade Settings")]
    [Tooltip("Maximum opacity allowed (0 to 1).")]
    [Range(0f, 1f)]
    [SerializeField] private float maxOpacity = 1f;

    [Tooltip("Distance greater than the fadeStartDistance where the text is ivisible")]
    [SerializeField] private float invisibleDistance = 6f;

    [Tooltip("Distance where the text is fully visible.")]
    [SerializeField] private float fadeStartDistance = 4f;

    [Tooltip("Distance where the text becomes fully invisible.")]
    [SerializeField] private float fadeEndDistance = 1f;

    [Tooltip("X-axis: 0 (FadeEnd) to 1 (FadeStart). Y-axis: Opacity.")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
    #endregion

    #region Variables: Visibility Logic
    [Header("Occlusion")]
    [Tooltip("Instantly hide text if the player is behind the object's forward face.")]
    [SerializeField] private bool hideIfBehind = true;
    #endregion

    private void Start()
    {

        if (fadeEndDistance > fadeStartDistance || fadeStartDistance > invisibleDistance || fadeEndDistance > invisibleDistance)
        {
            Debug.LogError(
                $"[Fader] Invalid distance settings! Ensure: fadeEndDistance < fadeStartDistance < invisibleDistance\n" +
                $"Current Settings: fadeEndDistance={fadeEndDistance}, fadeStartDistance={fadeStartDistance}, invisibleDistance={invisibleDistance}"
            );
            Destroy(gameObject);
        }   

        // Get the component (Works for TextMeshPro or TextMeshProUGUI)
        _tmpText = GetComponent<TMP_Text>();

        // Custom search: Finds the object by name AND confirms the tag
        GameObject foundObj = GameObject.Find(targetName);
        if (foundObj != null && foundObj.CompareTag(targetTag))
        {
            _targetTransform = foundObj.transform;
        }
        else
        {
            Debug.LogError($"[Fader] Could not find '{targetName}' with tag '{targetTag}'!");
        }
    }

    private void Update()
    {
        if (_targetTransform == null || _tmpText == null) return;

        float distance = Vector3.Distance(transform.position, _targetTransform.position);
        float finalAlpha = 0f;


        // 1. Check if "Behind" using Dot Product
        bool isBehind = false;
        if (hideIfBehind)
        {
            Vector3 toTarget = (_targetTransform.position - transform.position).normalized;
            // Negative dot means the target is in the opposite direction of 'Forward'
            if (Vector3.Dot(transform.forward, toTarget) > 0) isBehind = true;
        }

        // 2. Calculate Alpha if not behind
        if (!isBehind && distance < invisibleDistance)
        {

            // Normalize distance to a 0-1 value
            float t = Mathf.InverseLerp(fadeEndDistance, fadeStartDistance, distance);
            // Evaluate against your custom curve
            finalAlpha = fadeCurve.Evaluate(t) * maxOpacity;
        }

        // Apply to the TMP component
        Color c = _tmpText.color;
        _tmpText.color = new Color(c.r, c.g, c.b, finalAlpha);
    }
}
