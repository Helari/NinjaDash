using UnityEngine;
using System.Collections;
 using UnityEngine.UI;

public class ND_Timer : MonoBehaviour {

    public Text timerLabel;

    private float time;

    private float m_fSlowModifierTimer = 1.0f;         //Current speed variable
    public float m_fSlowModifierMaxTimer = 100.0f;    //The Speed and Time -SlowMotion ON- value, effect coefficient

    void Start() //Listen to SlowMo events and random positioning
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.GameStart += GameStart;
    }
    void GameStart()
    {
        if (this != null)
        {
            time = 0.0f;
            m_fSlowModifierTimer = 1.0f;
        }
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            m_fSlowModifierTimer = m_fSlowModifierMaxTimer;
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            m_fSlowModifierTimer = 1.0f;
        }
    }
    void Update()
    {
        time += Time.deltaTime / m_fSlowModifierTimer;

        var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
        var seconds = time % 60;//Use the euclidean division for the seconds.
        var fraction = (time * 100) % 100;

        //update the label value
        timerLabel.text = string.Format("{0:00} : {1:00} : {2:000}", minutes, seconds, fraction);
    }
}