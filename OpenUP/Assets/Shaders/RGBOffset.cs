using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBOffset : MonoBehaviour
{
    public Material rgbOffsetMat;
    [SerializeField] private float offset;


    // Start is called before the first frame update
    void Start()
    {
        rgbOffsetMat.SetFloat("_RedAngle", Random.Range(0, 2*Mathf.PI));
        rgbOffsetMat.SetFloat("_GreenAngle", Random.Range(0, 2*Mathf.PI));
        rgbOffsetMat.SetFloat("_BlueAngle", Random.Range(0, 2*Mathf.PI));
    }

    // Update is called once per frame
    void Update()
    {
        rgbOffsetMat.SetFloat("_OffsetAmn", offset);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, rgbOffsetMat);
    }
}
