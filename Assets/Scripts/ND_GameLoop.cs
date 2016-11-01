using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ND_GameLoop : MonoBehaviour {

    public static bool isSlowMo = false;
    private bool slowMoActive = false;
    public Button restartButton;

	// Use this for initialization
    void Start()
    {
        GameEventManager.GameOver += GameOver;
        Time.timeScale = 0;
        restartButton.gameObject.SetActive(true);
	}

    void Update()
    {
        Screen.SetResolution(375, 600, false); Screen.orientation = ScreenOrientation.Portrait;
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameEventManager.TriggerGameStart();
        }
        if (Input.GetKeyDown(KeyCode.A) && !slowMoActive)
        {
            slowMoActive = isSlowMo = true;
            GameEventManager.TriggerSlowMotionState_Begin();
        }
        else if (Input.GetKeyDown(KeyCode.A) && slowMoActive)
        {
            slowMoActive = isSlowMo = false;
            GameEventManager.TriggerSlowMotionState_End();
        }
	}
    public void Restart()
    {
        GameEventManager.TriggerPause();
        restartButton.gameObject.SetActive(false);
        GameEventManager.TriggerGameStart();
    }
    void GameOver()
    {
        if (this != null)
        {
            GameEventManager.TriggerPause();
            restartButton.gameObject.SetActive(true);
        }
    }
}

public static class GameEventManager {

	public delegate void GameEvent();

    public static event GameEvent SlowMotionState_Begin, SlowMotionState_End, DamagePlayer, GameOver, GameStart;
    public static bool GameSlowed = false;
    public static bool PauseState = true;

    public static void TriggerSlowMotionState_Begin()
    {
        if (SlowMotionState_Begin != null)
        {
            GameSlowed = true;
            SlowMotionState_Begin();
		}
	}
    public static void TriggerSlowMotionState_End()
    {
        if (SlowMotionState_End != null)
        {
            GameSlowed = false;
            SlowMotionState_End();
		}
	}
    public static void TriggerPlayerDamage()
    {
        if (DamagePlayer != null)
        {
            DamagePlayer();
        }
    }
    public static void TriggerGameOver()
    {
        if (GameOver != null)
        {
            GameOver();
        }
    }
    public static void TriggerGameStart()
    {
        if (GameStart != null)
        {
            GameStart();
        }
    }
    public static void TriggerPause()
    {
        if (PauseState)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        PauseState = !PauseState;
    }
}