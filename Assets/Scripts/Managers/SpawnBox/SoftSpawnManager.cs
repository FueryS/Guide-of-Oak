using UnityEngine;
using UnityEngine.SceneManagement;

public class SoftSpawnManager : MonoBehaviour
{
    public static SoftSpawnManager Instance_SoftSpawnManager;

    [Header("Settings")]
    public string playerTag = "Player";

    // Store the position as a Vector3, not a Transform
    [HideInInspector] public Vector3 savedSpawnPosition;
    [HideInInspector] public bool hasSavedPosition = false;

    GameObject[] softSpawns;

    private void Awake()
    {
        SetupSingleton();
    }

    //Ignore
    private void OnEnable() => SceneManager.sceneLoaded += RespawnPlayerAtCurrentSpawnSet;
    private void OnDisable() => SceneManager.sceneLoaded -= RespawnPlayerAtCurrentSpawnSet;


    #region prep utils

    //Setup the singleton pattern for this manager.
    //This ensures that only one instance of the SoftSpawnManager exists at any time,
    //and that it persists across scene loads.
    void SetupSingleton()
    {
        if (Instance_SoftSpawnManager != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance_SoftSpawnManager = this;
        DontDestroyOnLoad(gameObject);
    }

    //Respawn the player at the coordinates of the current spawn set when a new scene is loaded.
    void RespawnPlayerAtCurrentSpawnSet(Scene scene, LoadSceneMode mode)
    {
        if (hasSavedPosition)
        {
            //Find player on reload
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
            {
                // Diable CharacterController before moving the player to avoid physics issues, then re-enable it after.
                CharacterController cc = player.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                    Debug.Log($"<color=green> Character controller Disabled </color>");
                }

                player.transform.position = savedSpawnPosition;

                if (cc != null)
                {
                    cc.enabled = true;
                    Debug.Log($"<color=green> Character controller Enabled </color>");
                }

                Debug.Log("Player respawned at saved coordinates: " + savedSpawnPosition);

            }
        }
    }


    #endregion
}