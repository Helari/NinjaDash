using UnityEngine;
using System.Collections;
using UnityEngine.UI;
 
public class FPSDisplayScript : MonoBehaviour {
    float timeA;
    public int fps;
    public int lastFPS;
    public Text textFPS;
    public Text textFPS2;
    float deltaTime = 0.0f;
    // Use this for initialization
    void Start () {
        timeA = Time.timeSinceLevelLoad;
        DontDestroyOnLoad (this);
    }
   
    // Update is called once per frame
    void Update () {
        //Debug.Log(Time.timeSinceLevelLoad+" "+timeA);
        if(Time.timeSinceLevelLoad  - timeA <= 1)
        {
            fps++;
        }
        else
        {
            lastFPS = fps + 1;
            timeA = Time.timeSinceLevelLoad;
            fps = 0;
        }
        textFPS.text = lastFPS.ToString();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        float fps2 = 1.0f / deltaTime;
        string text2 = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps2);
        textFPS2.text = text2;
    }
}