using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerControls : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float forceMultiplier = 1f;

    [Header("Game Feel")]
    [SerializeField] private float timeSlow = 0.5f;
    [SerializeField] private float gravity = 0.3f;
    [SerializeField] private bool VignetteFollowsPlayer;

    [Header("Player Feedback")]
    [SerializeField] private GameObject blackFlame;
    [SerializeField] private Material blackFlameMat;
    [SerializeField] private float cooldownDuration;
    [SerializeField] private AnimationCurve cooldownCurve;
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private float showDurationDelay;

    [Header("Audio")]
    [SerializeField] private float pitchDownDuration;
    [SerializeField] private AnimationCurve pitchDownCurve;
    [SerializeField] private float pitchUpDuration;
    [SerializeField] private AnimationCurve pitchUpCurve;

    private Transform playerTransform;
    private Camera mainCam;
    private Rigidbody2D playerRigidbody2D;

    //Movement
    private Vector2 moveDirection;
    private Vector2 beginPointPos;
    private Vector2 endPointPos;
    private float moveDirectionDistance;
    private float moveDirectionAngle = 0;
    private bool beginPhaseMouse = true;

    //Game Feel
    private float fixedDeltaTime;

    //Visuals
    private VolumeProfile v;
    private Vignette vg;
    private Coroutine FlameCooldownRoutine;
    private Coroutine ShowFlameRoutine;
    private float VPControl = 0;
    private bool flameCooldownStarted = true;

    //Audio
    private AudioManager aM;
    private Coroutine pitchDownRoutine;
    private Coroutine pitchUpRoutine;



    private void Awake()
    {
        //Make sure timscale = 1
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    void Start()
    {
        //Make cross compatible with PC and mobile
        Input.simulateMouseWithTouches = true;

        mainCam = FindObjectOfType<Camera>();
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        playerTransform = GetComponent<Transform>();
        aM = AudioManager.instance;

        //Get access to postprocessing vignette
        v = GameObject.FindGameObjectWithTag("PP").GetComponent<Volume>()?.profile;
        v.TryGet(out vg);

        pitchDownRoutine = null;
        pitchUpRoutine = null;
    }

    void Update()
    {
        blackFlameMat.SetFloat("_VPControl", VPControl);

        moveDirection = endPointPos - beginPointPos;
        moveDirection.Normalize();

        moveDirectionDistance = Mathf.Clamp(Vector2.Distance(endPointPos, beginPointPos), 0f, maxDistance);
       
        if (Input.GetMouseButton(0))
        {
            VPControl = 0;
            if (FlameCooldownRoutine != null) { StopCoroutine(FlameCooldownRoutine); }
            blackFlameMat.SetFloat("_OpacityControl", 1);
            Time.timeScale = timeSlow;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            StartPitchDown(0.5f);


            if (beginPhaseMouse == true)
            {
                StartShowFlame();
                beginPointPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                beginPhaseMouse = false;
            }

            endPointPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            moveDirectionAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            moveDirectionAngle *= -1;
            moveDirectionAngle -= 225f;

            blackFlame.GetComponent<Transform>().localEulerAngles = new Vector3(0, 0, moveDirectionAngle);
            Debug.DrawLine(beginPointPos, endPointPos, Color.green);
            VPControl = moveDirectionDistance;

        }
        else
        { 
            if (beginPhaseMouse == false)
            {
                flameCooldownStarted = false;
                playerRigidbody2D.gravityScale = gravity;
                playerRigidbody2D.velocity = Vector2.zero;
                playerRigidbody2D.AddForce(moveDirection * moveDirectionDistance * forceMultiplier);
                Time.timeScale = 1f;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }
            beginPhaseMouse = true;

            StartPitchUp(1f);

            if (!flameCooldownStarted)
            {
                StartFlameCooldown();
                flameCooldownStarted = true;
            }
        }

        if (VignetteFollowsPlayer)
        {
            vg.center.value = new Vector2(mainCam.WorldToScreenPoint(playerTransform.position).x/mainCam.scaledPixelWidth, mainCam.WorldToScreenPoint(playerTransform.position).y / mainCam.scaledPixelHeight);
        }
    }

    private void StartFlameCooldown()
    {
        if (FlameCooldownRoutine != null) { StopCoroutine(FlameCooldownRoutine); }
        FlameCooldownRoutine = StartCoroutine(IEFlameCooldown());
    }

    private IEnumerator IEFlameCooldown()
    {
        float lerpTime = 0;

        float _oldPower = blackFlameMat.GetFloat("_VPControl");

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / cooldownDuration;
            float _evaluatedLerpTimePower = cooldownCurve.Evaluate(lerpTime);
            float _evaluatedLerpTimeOpacity = opacityCurve.Evaluate(lerpTime);

            float _newVPControl = Mathf.Lerp(_oldPower, 0f, _evaluatedLerpTimePower);
            float _newOpacity = Mathf.Lerp(1, 0, _evaluatedLerpTimeOpacity);

            blackFlameMat.SetFloat("_VPControl", _newVPControl);
            blackFlameMat.SetFloat("_OpacityControl", _newOpacity);

            yield return null;
        }

        blackFlame.SetActive(false);

        FlameCooldownRoutine = null;

        yield return null;
    }

    private void StartShowFlame()
    {
        if (ShowFlameRoutine != null) { StopCoroutine(ShowFlameRoutine); }
        ShowFlameRoutine = StartCoroutine(IEShowFlame());
    }

    private IEnumerator IEShowFlame()
    {
        yield return new WaitForSecondsRealtime(showDurationDelay);
        blackFlame.SetActive(true);

        yield return null;
    }

    private void StartPitchDown(float newPitch)
    {
        if(pitchDownRoutine != null) 
        { 
            return; 
        }
        else
        {
            pitchDownRoutine = StartCoroutine(PitchDownIE(newPitch));
        }
    }

    private IEnumerator PitchDownIE(float nP)
    {
        float lerpTime = 0;
        Debug.Log("Ayy");
        float _oldPitch = aM.GetPitch("Music");

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / pitchDownDuration;
            float _evaluatedLerpTime = pitchDownCurve.Evaluate(lerpTime);

            float _newPitch = Mathf.Lerp(_oldPitch, nP, _evaluatedLerpTime);

            aM.SetPitch("Music", _newPitch);

            yield return null;
        }

        pitchDownRoutine = null;

        yield return null;
    }

    private void StartPitchUp(float newPitch)
    {
        if (pitchUpRoutine != null)
        {
            return;
        }
        else
        {
            pitchUpRoutine = StartCoroutine(PitchUpIE(newPitch));
        }
    }

    private IEnumerator PitchUpIE(float nP)
    {
        float lerpTime = 0;

        float _oldPitch = aM.GetPitch("Music");

        while (lerpTime < 1)
        {
            lerpTime += Time.deltaTime / pitchUpDuration;
            float _evaluatedLerpTime = pitchUpCurve.Evaluate(lerpTime);

            float _newPitch = Mathf.Lerp(_oldPitch, nP, _evaluatedLerpTime);

            aM.SetPitch("Music", _newPitch);

            yield return null;
        }

        pitchUpRoutine = null;

        yield return null;
    }
}
