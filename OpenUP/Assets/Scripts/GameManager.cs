using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool escapeToQuitIsNeeded;
    [SerializeField] bool escapeToMenuIsNeeded;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && escapeToMenuIsNeeded)
        {
            if(SceneManager.GetActiveScene().buildIndex == 0 && escapeToQuitIsNeeded)
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
