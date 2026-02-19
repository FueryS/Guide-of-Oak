using System.Collections;
using UnityEngine;

public class KillPlayerInBlueHell : MonoBehaviour
{
    #region Objects
    [SerializeField]GameObject Player;
    #endregion

    #region Inspector

    public float blueHellStartsAt = -5;
    public float blueHellDamage = 30;
    public float blueHellDamagePeriod = 0.3f;

    #endregion

    private bool _isDamaging = false; // The Gate

    private void Start()
    {
        if (Player == null)
            Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player object not found in the scene. Please ensure there is a GameObject with the tag 'Player'.");
        }
    }



    

    private void Update()
    {
        // Only start damage if we aren't already damaging and player is too low
        if (Player.transform.position.y < blueHellStartsAt && !_isDamaging)
        {
            StartCoroutine(DoDamage());
        }
    }

    IEnumerator DoDamage()
    {
        _isDamaging = true; // Close the gate

        // Check if player has Stats component to avoid errors
        Stats playerStats = Player.GetComponent<Stats>();
        if (playerStats != null)
        {
            playerStats.Damage(blueHellDamage, true);
        }

        yield return new WaitForSeconds(blueHellDamagePeriod);

        _isDamaging = false; // Open the gate for the next hit
    }

}
