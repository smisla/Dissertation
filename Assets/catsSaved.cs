//using TMPro.Examples;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class CatsSaved : MonoBehaviour
//{

//    public GameObject catCounter;
//    public TextMeshProUGUI catCounterText;
//    public TextMeshProUGUI catDeadText;
//    public bool catDead;
//    public int totalCatsReached;
//    public GameObject spawner;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        catDeadText.gameObject.SetActive(false);
//        CatBehaviour catBehaviour = GetComponent<CatBehaviour>();
//        CatSpawner spawner = GetComponent<CatSpawner>();

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        catCounterText.text = "Cats Saved: " + spawner.totalCatsReached.ToString();
//        if (catBehaviour.catDead)
//        {
//            catDeadText.gameObject.SetActive(true);
//        }
//    }
//}
