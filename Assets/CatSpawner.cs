using UnityEngine;
using System.Collections;

public class CatSpawner : MonoBehaviour
{
    public GameObject catPrefab;

    public Transform startPoint;
    public Transform middlePoint;
    public Transform endPoint;

    public Transform destination;

    public float spawnInterval = 5f;
    public int maxCats = 5; //later remove

    private int currentCatCount = 0; //later remove
    
    public bool isSpawning = false;

    public void Start()
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

    void SpawnCat()
    {
        GameObject newCat = Instantiate(catPrefab, startPoint.position, Quaternion.LookRotation(endPoint.position - startPoint.position));
        CatBehaviour catScript = newCat.GetComponent<CatBehaviour>();

        if (catScript != null)
        {
            catScript.SetTargetPositions(startPoint, middlePoint, endPoint);
        }

        currentCatCount++;
    }

    public void CatReachedDestination()
    {
        currentCatCount--;
    }
}
