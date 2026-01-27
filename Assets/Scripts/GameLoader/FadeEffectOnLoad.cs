using UnityEngine;
using UnityEngine.Rendering;

public class FadeEffectOnLoad : MonoBehaviour
{
    #region Tool
    [Tooltip("How long the fade in animation should be")]
    public float fadeInDuration = 1.0f;
    [Tooltip("How long the fade out animation should be")]
    public float fadeOutDuration = 1.0f;
    [Tooltip("What should be the starting opacity")]
    [Range(0,1)]
    public float startOpacity = 1.0f;
    #endregion

    #region modules
    Renderer m_Renderer;
    #endregion

    #region private
    bool _opeque;
    float _opacity;
    #endregion

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
        _opacity = startOpacity;
    }

    public void FadeInAnimation()
    {
        m_Renderer.material.SetFloat("Alpha", _opacity);
    }


}
