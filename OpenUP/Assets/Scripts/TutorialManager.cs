﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float startTiming;
    [SerializeField] private float wordTiming;
    [SerializeField] private float spaceTiming;

    [SerializeField] private GameObject SWIPE;
    [SerializeField] private GameObject TO;
    [SerializeField] private GameObject MOVE;

    void Start()
    {
        StartCoroutine(Tutorial(startTiming, wordTiming, spaceTiming));
    }

    IEnumerator Tutorial(float start, float word, float space)
    {
        yield return new WaitForSecondsRealtime(start);
            SWIPE.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSecondsRealtime(word);
            SWIPE.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSecondsRealtime(space);
            TO.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSecondsRealtime(word);
            TO.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSecondsRealtime(space);
            MOVE.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSecondsRealtime(word);
            MOVE.GetComponent<TextMeshProUGUI>().enabled = false;

        this.gameObject.SetActive(false);
    }
}
