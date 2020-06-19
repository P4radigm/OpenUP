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
    [SerializeField] private float normalVgIntensity;

    [SerializeField] private AnimationCurve audioFadeCurve;
    [SerializeField] private float audioFadeDuration;

    [SerializeField] private float startTiming;
    [SerializeField] private float wordTiming;
    [SerializeField] private float spaceTiming;
    [SerializeField] private float enterTiming;

    [SerializeField] private GameObject YOU;
    [SerializeField] private GameObject WIN;
    [SerializeField] private GameObject OPENED;
    [SerializeField] private GameObject UP;    

    private Coroutine vgBackCoroutine;
    private Coroutine audioFadeRoutine;

    private VolumeProfile v;
    private Vignette vg;
    private AudioManager aM;

    void Start()
    {
        v = GameObject.FindGameObjectWithTag("PP").GetComponent<Volume>()?.profile;
        v.TryGet(out vg);

        StartVgBack();
        aM = AudioManager.instance;
    }

    private IEnumerator vgBackIE()
    {
        float _currentIntensity = vg.intensity.value;

        float lerpTime = 0;

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / vgBackTiming;
            float _evaluatedLerpTime = vgBackCurve.Evaluate(lerpTime);
            float _newIntensity = Mathf.Lerp(_currentIntensity, normalVgIntensity, _evaluatedLerpTime);

            vg.intensity.value = _newIntensity;

            yield return null;
        }
        StartCoroutine(EndCredits(startTiming, wordTiming, spaceTiming, enterTiming));

        yield return null;
    }

    private void StartVgBack()
    {
        if (vgBackCoroutine != null) { StopCoroutine(vgBackCoroutine); }
        vgBackCoroutine = StartCoroutine(vgBackIE());
    }

    IEnumerator EndCredits(float start, float word, float space, float enter)
    {


        yield return new WaitForSecondsRealtime(start);
            YOU.GetComponent<TextMeshProUGUI>().enabled = true;
            aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(word);
            YOU.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSecondsRealtime(space);
            WIN.GetComponent<TextMeshProUGUI>().enabled = true;
            aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(word);
            WIN.GetComponent<TextMeshProUGUI>().enabled = false;

        yield return new WaitForSecondsRealtime(enter);

            YOU.GetComponent<TextMeshProUGUI>().enabled = true;
            aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(word);
            YOU.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSecondsRealtime(space);
            OPENED.GetComponent<TextMeshProUGUI>().enabled = true;
            aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(word);
            OPENED.GetComponent<TextMeshProUGUI>().enabled = false;
        yield return new WaitForSecondsRealtime(space);
            UP.GetComponent<TextMeshProUGUI>().enabled = true;
            aM.Play("Beat2");
        yield return new WaitForSecondsRealtime(word);
            UP.GetComponent<TextMeshProUGUI>().enabled = false;

        yield return new WaitForSecondsRealtime(2);
        StartAudioFade("Music");
        yield return new WaitForSecondsRealtime(audioFadeDuration);
        SceneManager.LoadScene(0);
    }

    private void StartAudioFade(string name)
    {
        if(audioFadeRoutine != null) { return; }
        else { audioFadeRoutine = StartCoroutine(AudioFadeIE(name)); }
    }

    private IEnumerator AudioFadeIE(string n)
    {
        float lerpTime = 0;

        float _oldVolume = aM.GetVolume(n);

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / audioFadeDuration;
            float _evaluatedLerpTime = audioFadeCurve.Evaluate(lerpTime);

            float _newVolume = Mathf.Lerp(_oldVolume, 0f, _evaluatedLerpTime);

            aM.SetVolume("Music", _newVolume);

            yield return null;
        }

        audioFadeRoutine = null;

        yield return null;
    }
}
