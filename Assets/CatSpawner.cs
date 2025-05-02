using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using TMPro;

public class CatSpawner : MonoBehaviour
{
    public GameObject catPrefab;

    public Material[] catMaterials;

    public Transform startPoint;
    public Transform slowDownPoint;
    public Transform middlePoint;
    public Transform endPoint;

    public Transform destination;

    public float spawnInterval = 5f;
    public int maxCats = 5; //later remove

    private int currentCatCount = 0; //later remove
    
    public bool isSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnCats());

        //if (!isSpawning)
        //{
        //    isSpawning = true;
        //    StartCoroutine(SpawnCats());
        //}
    }

    private IEnumerator SpawnCats()
    {
        while (true)
        {
            if (currentCatCount < maxCats)
            {
                SpawnCat();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
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

    void AssignSize(GameObject cat)
    {
        float randomHeight = Random.Range(0.8f, 1.2f);
        cat.transform.localScale = new Vector3(randomHeight, randomHeight, randomHeight);
    }

    void SpawnCat()
    {
        GameObject newCat = Instantiate(catPrefab, startPoint.position, Quaternion.LookRotation(endPoint.position - startPoint.position));
        NavMeshAgent newAgent = newCat.GetComponent<NavMeshAgent>();
        CatBehaviour catScript = newCat.GetComponent<CatBehaviour>();
        AssignSize(newCat);
        AssignMaterial(newCat);

        if (catScript != null)
        {
            catScript.SetTargetPositions(startPoint, slowDownPoint, middlePoint, endPoint);
        }

        currentCatCount++;
    }

    public void CatReachedDestination()
    {
        currentCatCount--;
    }
}
