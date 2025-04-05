using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartCalibration()
    {
        SceneManager.LoadScene("CalibrationScene");
    }
}
