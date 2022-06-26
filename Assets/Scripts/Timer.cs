using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    
    public float timer;
    public Text timeText;

    void Update()
    {
        timer = GameManager.instance.time;
        timer += Time.deltaTime;
        timeText.text = "" + timer.ToString("f0");
        GameManager.instance.time = timer;




        // float t = Time.time - startTime;
        // string minutes = ((int) t/ 60).ToString();
        // string seconds = (t%60).ToString();

        // timeText.text = minutes + ":" + seconds;
    }
}
