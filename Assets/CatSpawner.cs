using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class CatSpawner : MonoBehaviour
{
    public GameObject catPrefab;
    public Material[] catMaterials;

    public Transform startPoint;
    public Transform climbPoint;
    public Transform slowDownPoint;
    public Transform middlePoint;
    public Transform endPoint;
    public Transform destination;
    public CatAudioController controller;

    public PlankDetector plankDetector;

    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    public bool isGameRunning = true;


    public TextMeshProUGUI catCounterText;

    private List<GameObject> activeCats = new List<GameObject>();
    public int totalCatsReached = 0;


    public float spawnInterval = 5f;
    public int maxCats = 5; //later remove

    private int currentCatCount = 0; //later remove
    
    public bool isSpawning = false;
    public Transform playerTransform;

    public void StartSpawning()
    {

        //StartCoroutine(SpawnCats());
        totalCatsReached = 0;
        if (!isSpawning)
        {
            plankDetector.StartPlankMonitoring();
            isSpawning = true;
            StartCoroutine(SpawnCats());
        }
    }

    private IEnumerator SpawnCats()
    {
        while (isGameRunning)
        { 
            if (activeCats.Count == 0 || Vector3.Distance(activeCats[activeCats.Count - 1].transform.position, endPoint.position) < 1.5f)
            {
                GameObject newCat = SpawnCat();
                activeCats.Add(newCat);

                float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                yield return null;
            }
        }

        //while (true)
        //{
        //    if (currentCatCount < maxCats)
        //    {
        //        SpawnCat();
        //    }
        //    yield return new WaitForSeconds(spawnInterval);
        //}
        //while (isSpawning)
        //{
        //    GameObject newCat = Instantiate(catPrefab, spawnPoint.position, Quaternion.identity);
        //    //CatBehaviour catScript = newCat.GetComponent<CatBehaviour>();
        //    catScript.destination = destination;

        //    yield return new WaitForSeconds(spawnInterval);
        //}
    }

    void AssignMaterial(GameObject cat)
    {
        if (catMaterials.Length == 0) return;

        Material chosenMaterial = catMaterials[Random.Range(0, catMaterials.Length)];

        SkinnedMeshRenderer catRenderer = cat.GetComponentInChildren<SkinnedMeshRenderer>();

        if (catRenderer != null)
        {
            catRenderer.material = new Material(chosenMaterial);

        }
        else
        {
            Debug.LogError("No SkinnedMeshRenderer found in " + cat.name);
        }
    }

    public void StopSpawning()
    {
        Debug.Log("StopSpawning() called.");
        isGameRunning = false;

        PlayerPrefs.SetInt("TotalCatsSaved", totalCatsReached);

        //foreach (GameObject cat in activeCats)
        //{
        //    if (cat != null)
        //    {
        //        CatBehaviour cb = cat.GetComponent<CatBehaviour>();
        //        if (cb != null)
        //        {
        //            cb.OnGameStopped();
        //            continue;
        //        }

        //        CatAIController ai = cat.GetComponent<CatAIController>();
        //        if (ai != null)
        //        {
        //            ai.OnGameStopped();
        //        }
        //    }
        //}
    }

    public void CatReachedDestination(GameObject cat)
    {
        if (activeCats.Contains(cat))
        {
            activeCats.Remove(cat);
        }
        totalCatsReached++;
        UpdateCounter();
    }

    void UpdateCounter()
    {
        if (catCounterText != null)
        {
            Debug.Log("Updating text to: Cats Saved: " + totalCatsReached);
            catCounterText.text = "Cats Saved: " + totalCatsReached.ToString();
        }
    }
    void AssignSize(GameObject cat)
    {
        float randomHeight = Random.Range(0.8f, 1.2f);
        cat.transform.localScale = new Vector3(randomHeight, randomHeight, randomHeight);
    }

    GameObject SpawnCat()
    {
        GameObject newCat = Instantiate(catPrefab, startPoint.position, Quaternion.LookRotation(endPoint.position - startPoint.position));
        newCat.GetComponent<CatAIController>().playerTransform = playerTransform;

        CatAudioController controller = newCat.GetComponent<CatAudioController>();
        if (controller != null)
        {
            controller.StartRandomMeow();
        }

        NavMeshAgent newAgent = newCat.GetComponent<NavMeshAgent>();
        CatBehaviour catScript = newCat.GetComponent<CatBehaviour>();

        //AssignSize(newCat);
        AssignMaterial(newCat);

        if (catScript != null)
        {
            //catScript.Init(this);
            catScript.SetTargetPositions(startPoint, climbPoint, slowDownPoint, middlePoint, endPoint);
            //catScript.spawner = this;
        }
        else
        {
            Debug.LogWarning("Spawned cat is missing behaviour script");
        }
        currentCatCount++;
        return newCat;
    }
}
