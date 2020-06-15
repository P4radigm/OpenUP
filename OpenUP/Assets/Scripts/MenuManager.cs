using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadMainScene(1);
        }
    }

    public void LoadMainScene(int sceneCount)
    {
        SceneManager.LoadScene(sceneCount);
    }
}
