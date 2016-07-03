using UnityEngine;
using System.Collections;

public class ND_GameLoop : MonoBehaviour {

    public static bool isSlowMo = false;
    private bool slowMoActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	void Update () {
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
}

public static class GameEventManager {

	public delegate void GameEvent();

    public static event GameEvent SlowMotionState_Begin, SlowMotionState_End, DamagePlayer;
    public static bool GameSlowed = false;

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
        if(DamagePlayer != null)
        {
            DamagePlayer();
        }
    }
}