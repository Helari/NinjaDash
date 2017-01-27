using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// LD Script
/// Spawns ND_EnemyPattern at level specific times
/// </summary>
public class ND_LevelEnemyRequestManager : MonoBehaviour {

    float timer = 0.0f;
    
    public ND_Level levelData;
    int levelStep = 0;

    private float timeModifier = 1.0f; //SlowMotion reaction (affects spawn timers)

    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        levelData = (ND_Level)AssetDatabase.LoadAssetAtPath("Assets/Level.asset", typeof(ND_Level));
        levelStep = 0;
    }
    void SlowMoActivation()
    {
        if (this != null)
        {
            //Gets the SlowMoModifier
            timeModifier = ND_Enemy.m_fSlowModifierMax;
        }
    }
    void SlowMoDEActivation()
    {
        if (this != null)
        {
            timeModifier = 1.0f;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (!GameEventManager.PauseState)
        {
            timer += Time.deltaTime / timeModifier;

            if(timer >= levelData.patternTime[levelStep].timer)
            {
                int ID = 0;
                int rand = Random.Range(0, 100);
                int currentProba = levelData.patternTime[levelStep].patternProba[ID].proba;
                Debug.Log("Rand=" + rand.ToString());
                Debug.Log("currentProba=" + currentProba.ToString());
                Debug.Log("ID=0");
                while (rand > currentProba && ID < levelData.patternTime[levelStep].patternProba.Count-1)
                {
                    ID++;
                    currentProba += levelData.patternTime[levelStep].patternProba[ID].proba;
                    Debug.Log("currentProba=" + currentProba.ToString());
                    Debug.Log("ID=" + ID.ToString());

                }
                string name = levelData.patternTime[levelStep].patternProba[ID].pattern.name;

                //int patternIdToSpawn = Random.Range(0, levelData.patternTime[levelStep].patternProba.Count-1);
                Debug.Log("Pattern:" + name);
                ND_EnemyPattern dataLoaded = (ND_EnemyPattern)AssetDatabase.LoadAssetAtPath("Assets/" + name + ".asset", typeof(ND_EnemyPattern));

                for (int i = 0; i < dataLoaded.pattern.Count; i++)
                {
                    ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType(ND_EnemyFactory.instance.objectPrefabs[dataLoaded.pattern[i].archetype].name, false).GetComponent<ND_Enemy>();
                    tempEnemy.transform.position = dataLoaded.pattern[i].position; Debug.Log(tempEnemy.name);
                    tempEnemy.Activate();
                }
                levelStep++;
                if (levelStep >= levelData.patternTime.Count)
                {
                    timer = 0;
                    levelStep = 0; //WIN CONDITION
                }
                //Debug.Log(levelStep.ToString());
            }
        }
	}
}
