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
    //[SerializeField] private AnimationCurve showFlameCurve;

    //private float showDuration;
    private Transform playerTransform;
    private Camera mainCam;
    //private LineRenderer directionVisual;
    private Rigidbody2D playerRigidbody2D;
    private Vector3 playerPosition;
    private Vector2 moveDirection;
    private Vector2 beginPointPos;
    private Vector2 endPointPos;
    //private bool slowDown = false;
    private bool beginPhaseMouse = true;
    private float moveDirectionDistance;
    private float fixedDeltaTime;
    private float moveDirectionAngle = 0;
    //private float prevMoveDirectionAngle = 0;

    private bool flameCooldownStarted = true;
    private float VPControl = 0;

    private VolumeProfile v;
    private Vignette vg;

    private Coroutine FlameCooldownRoutine;
    private Coroutine ShowFlameRoutine;

    private void Awake()
    {
        //Screen.SetResolution(2160, 1080, 0, 60);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        this.fixedDeltaTime = Time.fixedDeltaTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCam = FindObjectOfType<Camera>();
        v = GameObject.FindGameObjectWithTag("PP").GetComponent<Volume>()?.profile;
        v.TryGet(out vg);
        //cooldownDuration = cooldownDuration * 0.01f;
        Input.simulateMouseWithTouches = true;
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        //directionVisual = GetComponent<LineRenderer>();
        playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        blackFlameMat.SetFloat("_VPControl", VPControl);
        playerPosition = playerTransform.position;
        //directionVisual.SetPosition(0, playerPosition);

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

            if (beginPhaseMouse == true)
            {
                StartShowFlame();
                beginPointPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                beginPhaseMouse = false;
            }

            //Vector2 aha = new Vector2(playerPosition.x, playerPosition.y) + (moveDirection * moveDirectionDistance);
            endPointPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //float moveDirectionAngle = Vector2.Angle(Vector2.up, moveDirection);
            //if (moveDirectionAngle < 0.0f) moveDirectionAngle = 360f - moveDirectionAngle;
            moveDirectionAngle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
            moveDirectionAngle *= -1;
            moveDirectionAngle -= 225f;

            //if(blackFlame.GetComponent<Transform>().localEulerAngles.z == prevMoveDirectionAngle) blackFlame.SetActive(false);
            //else blackFlame.SetActive(true);

            blackFlame.GetComponent<Transform>().localEulerAngles = new Vector3(0, 0, moveDirectionAngle);
            //directionVisual.SetPosition(1, aha);
            Debug.DrawLine(beginPointPos, endPointPos, Color.green);
            //Debug.Log("Begin - " + beginPointPos);
            //Debug.Log("End - " + endPointPos);
            VPControl = moveDirectionDistance;

        }
        else
        { 
            if (beginPhaseMouse == false)
            {
                flameCooldownStarted = false;
                //prevMoveDirectionAngle = moveDirectionAngle; 
                playerRigidbody2D.gravityScale = gravity;
                playerRigidbody2D.velocity = Vector2.zero;
                playerRigidbody2D.AddForce(moveDirection * moveDirectionDistance * forceMultiplier);
                //slow down time/game feel vignette idk dat soort shit
                Time.timeScale = 1f;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }
            //directionVisual.SetPosition(1, playerPosition);
            beginPhaseMouse = true;

            if (!flameCooldownStarted)
            {
                StartFlameCooldown();
                flameCooldownStarted = true;
            }
        }

        //if (!Input.GetMouseButton(0))
        //{
        //    if (!flameCooldownStarted) 
        //    {
        //        StartFlameCooldown(); 
        //        flameCooldownStarted = true;
        //    } 
        //    //blackFlame.SetActive(false);
        //    //directionVisual.SetPosition(1, playerPosition);
        //}

        if (VignetteFollowsPlayer)
        {
            //Debug.Log(Screen.currentResolution);
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
        //float lerpTime = 0;

        //while (lerpTime < 1)
        //{
        //    lerpTime += Time.deltaTime / showDuration;
        //    float evaluatedLerpTime = showFlameCurve.Evaluate(lerpTime);

        //    blackFlameMat.SetFloat("_OpacityControl", evaluatedLerpTime);

        //    yield return null;
        //}

        yield return null;
    }
}
