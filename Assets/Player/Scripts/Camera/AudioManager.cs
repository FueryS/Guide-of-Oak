using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Audios
    [Header("AudioSources")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource SFXPlayer;

    [Header("Audios")]
    public AudioClip HappyChildHoods;

    [Header("Audio SFX")]
    public AudioClip[] BolderBreakWall;
    public AudioResource FootSteps;

    #endregion

    private void Start()
    {
        if (BGMSource != null)
        {
            BGMSource.clip = HappyChildHoods;
            BGMSource.Play();
        }
    }

    #region Bolder Break Wall


    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F)) PlayFootStep();
    }


    public void PlayBolderBreakWall()
    {
        int n = BolderBreakWall.Length;
        n = Random.Range(0, n);
        SFXSource.clip = BolderBreakWall[n];
        SFXSource.Play();


    }
    public void PlayBolderBreakWall(int n)
    {
        SFXSource.clip = BolderBreakWall[n];
        SFXSource.Play();
    }
    #endregion


    #region FootStep

    public void PlayFootStep()
    {
        // In 2026, use 'resource' x`instead of 'clip' for random containers
        SFXPlayer.resource = FootSteps;
        SFXPlayer.Play();
        //  Debug.Log("Played");
    }

    #endregion

}
