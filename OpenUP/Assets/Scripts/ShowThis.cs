using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowThis : MonoBehaviour
{
    [SerializeField] private GameObject OPEN;
    [SerializeField] private GameObject UP;
    [SerializeField] private GameObject Panel;

    [SerializeField] private GameObject[] white;
    [SerializeField] private GameObject[] black;

    private GameObject player;

    private Coroutine ShowRoutine;

    private AudioManager aM;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aM = AudioManager.instance;
    }


    public void StartShow(float showTime)
    {
        if (ShowRoutine != null) { ResetText(); StopCoroutine(ShowRoutine); }
        ShowRoutine = StartCoroutine(ShowIE(showTime));
    }

    private void ResetText()
    {
        OPEN.GetComponent<TextMeshProUGUI>().enabled = false;
        UP.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    public IEnumerator ShowIE(float showTime)
    {
        for (int i = 0; i < white.Length; i++)
        {
            white[i].GetComponent<SpriteRenderer>().color = Color.black;
        }

        for (int i = 0; i < black.Length; i++)
        {
            black[i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        Camera.main.backgroundColor = Color.black;

        for (int i = 0; i < player.GetComponent<DotSpawner>().dots.Count; i++)
        {
            player.GetComponent<DotSpawner>().dots[i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        Panel.GetComponent<Image>().enabled = true;
        OPEN.GetComponent<TextMeshProUGUI>().enabled = true;
        aM.Play("Beat1");
        yield return new WaitForSecondsRealtime(showTime);
        OPEN.GetComponent<TextMeshProUGUI>().enabled = false;
        UP.GetComponent<TextMeshProUGUI>().enabled = true;
        aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(showTime);
        UP.GetComponent<TextMeshProUGUI>().enabled = false;
        Panel.GetComponent<Image>().enabled = false;
        //aM.Stop("C3");
        //aM.Stop("A2_E3");

        for (int i = 0; i < white.Length; i++)
        {
            white[i].GetComponent<SpriteRenderer>().color = Color.white;
        }

        for (int i = 0; i < black.Length; i++)
        {
            black[i].GetComponent<SpriteRenderer>().color = Color.black;
        }

        for (int i = 0; i < player.GetComponent<DotSpawner>().dots.Count; i++)
        {
            player.GetComponent<DotSpawner>().dots[i].GetComponent<SpriteRenderer>().color = Color.black;
        }

        Camera.main.backgroundColor = Color.white;
    }
}
