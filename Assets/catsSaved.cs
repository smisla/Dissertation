using TMPro.Examples;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatsSaved : MonoBehaviour
{

    public GameObject catSad;
    public GameObject catHappy;
    public TextMeshProUGUI catCounterText;
    public TextMeshProUGUI catDeadText;
    public bool catDead;
    public int deadCat;
    public int totalCatsSaved;
    public GameObject spawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalCatsSaved = PlayerPrefs.GetInt("TotalCatsSaved");
        deadCat = PlayerPrefs.GetInt("DeadCat");
        catDeadText.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        catCounterText?.SetText($"Cats Saved: \n{totalCatsSaved}");
        if (deadCat == 1f)
        {
            catDeadText.gameObject.SetActive(true);
            catSad.gameObject.SetActive(true);
            catHappy.gameObject.SetActive(false);
        }
        //if (catBehaviour.catDead)
        //{
        //    catDeadText.gameObject.SetActive(true);
        //}
    }
}
