using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnSet : MonoBehaviour
{
    #region inspector
    public string player = "Player";
    public string SpawnManagerName = "SpawnManagerName";
    public string ManagerTag = "Manager";
    #endregion

    #region private
    Transform _playerTransform;
    #endregion

    #region module
    SoftSpawnManager SoftSpawnManager_m;
    #endregion


    void Start()
    {
        FindManager();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(player))
        {
            // Tell the manager the COORDINATES to remember
            SoftSpawnManager.Instance_SoftSpawnManager.savedSpawnPosition = transform.position;
            SoftSpawnManager.Instance_SoftSpawnManager.hasSavedPosition = true;

            Debug.Log("Checkpoint saved!");
        }
    }

    #region Prep Utils
   
    void FindManager()
    {
        SoftSpawnManager_m = SoftSpawnManager.Instance_SoftSpawnManager;
        
    }

    #endregion
}
