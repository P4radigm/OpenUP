using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private AnimationCurve vgBackCurve;
    [SerializeField] private float vgBackTiming;

    [SerializeField] private float startTiming;
    [SerializeField] private float wordTiming;
    [SerializeField] private float spaceTiming;
    [SerializeField] private float enterTiming;

    [SerializeField] private GameObject YOU;
    [SerializeField] private GameObject WIN;
    [SerializeField] private GameObject OPENED;
    [SerializeField] private GameObject UP;

    private float startAmnVg;
    

    private Coroutine vgBackCoroutine;

    private VolumeProfile v;
    private Vignette vg;

    // Start is called before the first frame update
    void Start()
    {
        v = GameObject.FindGameObjectWithTag("PP").GetComponent<Volume>()?.profile;
        v.TryGet(out vg);

        startAmnVg = vg.intensity.value;
        FindOrAddKey(vgBackCurve, 0f, startAmnVg);
        //StartCoroutine(EndCredits(startTiming, wordTiming, spaceTiming, enterTiming));
        StartVgBack();
    }

    private IEnumerator vgBackIE()
    {
        float lerpTime = 0;

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / vgBackTiming;
            float evaluatedLerpTime = vgBackCurve.Evaluate(lerpTime);

            vg.intensity.value = evaluatedLerpTime;

            yield return null;
        }
        StartCoroutine(EndCredits(startTiming, wordTiming, spaceTiming, enterTiming));
        //blackFlame.SetActive(false);

        yield return null;
    }

    private void StartVgBack()
    {
        if (vgBackCoroutine != null) { StopCoroutine(vgBackCoroutine); }
        vgBackCoroutine = StartCoroutine(vgBackIE());
    }

    IEnumerator EndCredits(float start, float word, float space, float enter)
    {


        yield return new WaitForSeconds(start);
            YOU.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(word);
            YOU.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSeconds(space);
            WIN.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(word);
            WIN.GetComponent<TextMeshProUGUI>().enabled = false;

        yield return new WaitForSeconds(enter);

            YOU.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(word);
            YOU.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSeconds(space);
            OPENED.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(word);
            OPENED.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSeconds(space);
            UP.GetComponent<TextMeshProUGUI>().enabled = true;
        yield return new WaitForSeconds(word);
            UP.GetComponent<TextMeshProUGUI>().enabled = false;

        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(0);
    }

    static void FindOrAddKey(AnimationCurve curve, float time, float value)
    {
        for (int i = 0; i < curve.keys.Length; ++i)
        {
            if (curve.keys[i].time == time)
            {
                curve.RemoveKey(i);
                break;
            }
        }
        int key = curve.AddKey(time, value);
    }
}
