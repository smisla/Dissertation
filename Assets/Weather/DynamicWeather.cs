using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWeather : MonoBehaviour
{
    public WeatherStates weatherState;

    public enum WeatherStates
    {
        Pick,
        Sunny,
        Rain,
        Storm,
        Mist
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WeatherFSM()
    {
        while (true)
        {
            switch (weatherState)
            {
                case WeatherStates.Pick:
                    Pick();
                    break;
                case WeatherStates.Sunny:
                    Sunny();
                    break;
                case WeatherStates.Rain:
                    Rain();
                    break;
                case WeatherStates.Storm:
                    Storm();
                    break;
                case WeatherStates.Mist:
                    Mist();
                    break;
            }
            yield return null;
        }
    }
    void Pick()
    {

    }

    void Sunny()
    {

    }
    void Rain()
    {

    }
    void Storm()
    {

    }
    void Mist()
    {

    }
}
