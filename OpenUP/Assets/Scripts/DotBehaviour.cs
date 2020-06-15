using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DotBehaviour : MonoBehaviour
{
    public Transform target;
    public float speed;
    [SerializeField] private float velocityThreshold;
    [SerializeField] private float saturation;
    [SerializeField] private float brightness;
    [SerializeField] private float despawnRange;
    [SerializeField] private ShowThis OpenUp;
    [SerializeField] private VolumeProfile v;
    [SerializeField] private float linDotSpeedIncrease;
    private Vignette vg;
    private DotSpawner dotSpawner;

    void Start()
    {
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("PP"));
        v = GameObject.FindGameObjectWithTag("PP").GetComponent<Volume>()?.profile;
        v.TryGet(out vg);
        //vg.intensity.value = 0.09f;
        OpenUp = GameObject.FindGameObjectWithTag("OpenUP").GetComponent<ShowThis>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        dotSpawner = DotSpawner.instance;
        Recolour(saturation, brightness);
    }

    void Update()
    {
        //aim at player
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //move
        transform.position += transform.right * Time.deltaTime * speed;

        //destroy when too far
        if (Vector2.Distance(this.transform.position, target.position) > despawnRange)
        {
            dotSpawner.dots.Remove(this.gameObject);
            Destroy(this.gameObject);
        }
    }

    void Recolour(float sat, float br)
    {
        SpriteRenderer SpriteRen = GetComponent<SpriteRenderer>();

        SpriteRen.color = Color.HSVToRGB(Random.Range(0, 360), sat, br);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Shield")
        {
            if(Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.y) > velocityThreshold)
            {
                vg.intensity.value += 0.01f;
                OpenUp.StartShow(0.3f);
                dotSpawner.dots.Remove(this.gameObject);
                for (int i = 0; i < dotSpawner.dots.Count; i++)
                {
                    dotSpawner.dots[i].GetComponent<DotBehaviour>().speed += linDotSpeedIncrease;
                }

                Destroy(this.gameObject);
            }
            else
            {
                collision.gameObject.SetActive(false);
                dotSpawner.dots.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
        }

        if (collision.gameObject.tag == "Player")
        {
            if (Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.x) + Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.y) < velocityThreshold)
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                vg.intensity.value += 0.01f;
                OpenUp.StartShow(0.3f);
                //collision.gameObject.SetActive(false);
                dotSpawner.dots.Remove(this.gameObject);
                for (int i = 0; i < dotSpawner.dots.Count; i++)
                {
                    dotSpawner.dots[i].GetComponent<DotBehaviour>().speed += linDotSpeedIncrease;
                }

                Destroy(this.gameObject);
            }
        }
    }
}
