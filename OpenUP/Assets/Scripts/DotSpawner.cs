using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotSpawner : MonoBehaviour
{
    public static DotSpawner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject dotPrefab;
    public List<GameObject> dots;
    [SerializeField] private float innerSpawnRad;
    [SerializeField] private float outerSpawnRad;
    [SerializeField] private int spawnAmn;
    [SerializeField] private float minDotDistance;

    void Start()
    {
        dots = new List<GameObject>();

        GameObject _FirstGo = Instantiate(dotPrefab, new Vector2(transform.position.x + Random.Range(innerSpawnRad, outerSpawnRad) * Mathf.Cos(Random.Range(0, 2 * Mathf.PI)), transform.position.y + Random.Range(innerSpawnRad, outerSpawnRad) * Mathf.Sin(Random.Range(0, 2 * Mathf.PI))), Quaternion.identity);
        dots.Add(_FirstGo);

        for (int i = 0; i < spawnAmn - 1; i++)
        {
            GameObject _Go = Instantiate(dotPrefab, NewSpawnPoint(), Quaternion.identity);
            dots.Add(_Go);
        }
    }

    void Update()
    {
        if (dots.Count < spawnAmn)
        {
            SpawnNewDot();
        }
    }

    void SpawnNewDot()
    {
        GameObject _Go = Instantiate(dotPrefab, NewSpawnPoint(), Quaternion.identity);
        dots.Add(_Go);
    }

    private Vector2 NewSpawnPoint()
    {
        bool tooClose = true;
        Vector2 newPos = Vector2.zero;

        while (tooClose)
        {
            float x = Random.Range(innerSpawnRad, outerSpawnRad);
            float phi = Random.Range(0, 2 * Mathf.PI);
            float dx = x * Mathf.Cos(phi);
            float dy = x * Mathf.Sin(phi);
            newPos = new Vector2(transform.position.x + dx, transform.position.y + dy);

            for (int i = 0; i < dots.Count; i++)
            {
                if (Vector2.Distance(new Vector2(dots[i].transform.position.x, dots[i].transform.position.y), newPos) > minDotDistance)
                { 
                    tooClose = false;
                }
            }
        }

        return newPos;
    }
    
}
