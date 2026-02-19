using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance;
    public Animator m_transition;

    [Tooltip("Set this variable to the name of the next scene to load.")]
    public static string nextScene;

    private string _currentScene;
    private bool _isTransitioning = false; // Prevents the reload loop

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        m_transition = GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _currentScene = SceneManager.GetActiveScene().name;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;
        _isTransitioning = false; // Reset the gate when the scene is ready
    }

    [ContextMenu("Reload Scene")]
    public void ReloadScene()
    {
        // If we are already loading, ignore any new requests
        if (_isTransitioning) return;
        StartCoroutine(SceneReloadTransition());
    }

    [ContextMenu("Load Next Scene")]
    public void LoadNextScene()
    {
        if (_isTransitioning) return;

        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(SceneTransition());
        }
        else
        {
            Debug.LogWarning("Next scene is not set!");
        }
    }

    IEnumerator SceneTransition()
    {
        _isTransitioning = true;
        m_transition.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
        nextScene = null;
        m_transition.SetTrigger("FadeOut");
    }

    IEnumerator SceneReloadTransition()
    {
        _isTransitioning = true;
        m_transition.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(_currentScene);
        // _isTransitioning is reset in OnSceneLoaded
        m_transition.SetTrigger("FadeOut");
    }
}