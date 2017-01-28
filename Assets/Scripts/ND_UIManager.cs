using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ND_UIManager : MonoBehaviour
{
    public Button cancelDashButton;
    public Button restartButton;

    void Start()
    {
        GameEventManager.GameOver += GameOver;
        GameEventManager.Victory += Victory;
        GameEventManager.GameStart += GameStart;
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        restartButton.gameObject.SetActive(true);
        cancelDashButton.gameObject.SetActive(false);
    }

    void Victory()
    {
        if (this != null)
        {
            restartButton.gameObject.GetComponentInChildren<Text>().text = "Victory !";
            restartButton.gameObject.SetActive(true);
            cancelDashButton.gameObject.SetActive(false);
        }
    }
    void GameOver()
    {
        if (this != null)
        {
            restartButton.gameObject.GetComponentInChildren<Text>().text = "Fail !";
            restartButton.gameObject.SetActive(true);
            cancelDashButton.gameObject.SetActive(false);
        }
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            cancelDashButton.gameObject.SetActive(true);
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            cancelDashButton.gameObject.SetActive(false);
        }
    }
    void GameStart()
    {
        if (this != null)
        {
            restartButton.gameObject.SetActive(false);
        }
    }
    public void switchPauseState()
    {
        GameEventManager.TriggerPause();
    }
}
