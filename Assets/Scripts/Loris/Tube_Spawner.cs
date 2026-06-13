using System.Collections.Generic;
using UnityEngine;

public class Tube_Spawner : MonoBehaviour
{
    float timer;

    Camera mainCamera;
    Queue<Tube_Controller> tubePool = new();
    List<Tube_Controller> allTubes = new();
    void Awake()
    {
        mainCamera = Camera.main;
        SetSpawnPos();
        CreatePool();
    }
    void Update()
    {
        timer += Time.deltaTime;
        while(timer > FB_Manager.instance.spawnRate)
        {
            SpawnTube();
            timer -= FB_Manager.instance.spawnRate;
        }
    }
    void SpawnTube()
    {
        if (tubePool.Count == 0)
        {
            return;
        }
        Tube_Controller tube = tubePool.Dequeue();

        float randomY = Random.Range(-FB_Manager.instance.heightOffset, FB_Manager.instance.heightOffset);
        tube.transform.position = transform.position + new Vector3(0, randomY);
        tube.gameObject.SetActive(true);
    }
    void SetSpawnPos()
    {
        float screenRight = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = screenRight + 0.5f;
        Vector3 pos = transform.position;
        pos.x = spawnX;

        transform.position = pos;
    }
    void CreatePool()
    {
        for (int i = 0; i < FB_Manager.instance.poolSize; i++)
        {
            GameObject tubeObj = Instantiate(FB_Manager.instance.tubePrefab, transform);
            tubeObj.SetActive(false);
            Tube_Controller tube = tubeObj.GetComponent<Tube_Controller>();
            tube.Initialize(this);

            tubePool.Enqueue(tube);
            allTubes.Add(tube);
        }
    }
    public void ReturnToPool(Tube_Controller tube)
    {
        tube.gameObject.SetActive(false);
        tubePool.Enqueue(tube);
    }
    public void ResetSpawner()
    {
        timer = 0;
        tubePool.Clear();
        foreach (Tube_Controller tube in allTubes)
        {
            tube.gameObject.SetActive(false);
            tubePool.Enqueue(tube);
        }
    }
}
