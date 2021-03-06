﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayScript : MonoBehaviour
{
    public void StartSpawn()
    {
        MainGameScript.Instance.StartSpawn();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
        //SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoBack(string componentTarget) {
        SceneManager.LoadScene(componentTarget);
    }

    public void GoBackLevelSelect(string componentTarget)
    {
        PlayerPrefs.SetInt("LevelScrollValue", (int)GameObject.Find("Text").transform.position.x);
        SceneManager.LoadScene(componentTarget);
    }
}
