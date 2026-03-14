using System;
using UnityEngine;
using UnityEngine.Audio;

public class AnimationEvents : MonoBehaviour
{
    GameObject attachHitbox; //Used for attack related stuff

    [Header("Audio Source")]
    public AudioSource audioSource;
    public AudioSource SwardSource;

    [Header("Audio Resources")]
    public AudioResource footStepResource;
    public AudioResource heavySwardResource;
    public AudioClip woosh;
    

    void Awake()
    {


        //------------------ Attack Animation ----------------------------------
        
        attachHitbox = GetComponentInParent<ValuesAssigner>().attackHitbox;

        //Set the hitbox to inactive at the start of the game to prevent unintended collisions.
        if (attachHitbox != null)
            attachHitbox.SetActive(false);
    }


    #region Attack Utils

    ///<Summery>
    /// These Methods will be called via the Y-Bot object through the animation events
    ///        - When attacking set the necessary objects on active state including the script that we will make in future for dealing damage
    ///        - This new scrip will also be applied to the hitbox game object
    ///        - Ensure everything is disabled on EndAttack to prevent any conflicts
    ///</Summery>
    
    public void PlayWoosh()
    {
        SwardSource.clip = woosh;
        SwardSource.Play();
    }
    public void PlayHeavySwardSound()
    {
        SwardSource.resource = heavySwardResource;
        SwardSource.Play();
    }

    public void InitiateAttack()
    {
        attachHitbox.SetActive(true);
    }

    public void EndAttack()
    {
        attachHitbox?.SetActive(false);
    }
    #endregion

    #region Defence Utils



    #endregion

    #region Audio Utils

    public void PlayFootSteps()
    {
        audioSource.resource = footStepResource;
        audioSource.Play();
    }

    #endregion

}

