using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    #region modules
    public static GameLoader Instance;
    public Animator m_transition;
    #endregion

    // Shared data
    public static string nextScene;

    // Scene-specific
    private string _currentScene;

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
    }

    [ContextMenu("Reload Scene")]
    public void ReloadScene()
    {
        StartCoroutine(SceneReloadTransition());
    }

    [ContextMenu("Load Next Scene")]
    public void LoadNextScene()
    {
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
        m_transition.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(nextScene);
        nextScene = null;      
        m_transition.SetTrigger("FadeOut");
    }    
    
    IEnumerator SceneReloadTransition()
    {
        m_transition.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(_currentScene);
        nextScene = null;      
        m_transition.SetTrigger("FadeOut");
    }
}
