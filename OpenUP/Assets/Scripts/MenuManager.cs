using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Update is called once per frame
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
