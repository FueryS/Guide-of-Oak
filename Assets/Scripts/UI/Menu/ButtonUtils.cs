using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonUtils : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] Animator Pannel;

    public string OpenPannelTrigger = "open";
    public string ClosePannelTrigger = "close";
    public string trainingLevelName = "TrainingLevel";

    //List so it can be edited in the inspector, and so it can be serialized, but basicly a custom disctionary
    public List<LevelSerial> LevelSerials = new List<LevelSerial>();

    private void Awake()
    {
        if (Pannel == null)
        {
            Debug.LogError("Pannel is not assigned in the inspector.");
        }

    }

    public void OpenPannel()
    {
        Pannel.SetTrigger(OpenPannelTrigger);
    }
    public void ClosePannel() {
        Pannel.SetTrigger(ClosePannelTrigger);
    }
    public void NewGame()
    {
        GameLoader.nextScene = trainingLevelName;
        GameLoader.Instance.LoadNextScene();
    }
    public void LoadLevel(int levelIndex)
    {

        var entry = LevelSerials.FirstOrDefault(ls => ls.levelIndex == levelIndex);

        if (entry != null)
        {
            GameLoader.nextScene = entry.sceneName;
            GameLoader.Instance.LoadNextScene();
        }
        else
        {
            Debug.LogError($"Level index {levelIndex} not found in LevelSerials list.");
        }
    }
}



// Basicly a custom disctionary
[System.Serializable]
public class LevelSerial
{
    public int levelIndex;
    public string sceneName;
}
