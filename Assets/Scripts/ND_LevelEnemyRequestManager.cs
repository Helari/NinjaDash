using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// LD Script
/// Spawns ND_EnemyPattern at level specific times
/// </summary>
public class ND_LevelEnemyRequestManager : MonoBehaviour {

    float timer;
    
    private ND_Level levelData;
    int levelID;
    int levelStep;

    public static int enemyCount;
    bool bLevelsDone;

    public static float timeModifier = 1.0f; //SlowMotion reaction (affects spawn timers)

    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
        GameEventManager.GameStart += GameStart;
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
    void GameStart()
    {
        if(this != null)
        {
            timer = 0.0f;
            levelID = 0;
            bLevelsDone = false;
            enemyCount = 0;
            levelData = Resources.Load("Level " + levelID, typeof(ND_Level)) as ND_Level;
            levelStep = 0;
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (!GameEventManager.PauseState)
        {
            timer += Time.deltaTime / timeModifier;

            if (!bLevelsDone && levelData != null && timer >= levelData.patternTime[levelStep].timer)
            {
                int ID = 0;
                int rand = Random.Range(0, 100);
                int currentProba = levelData.patternTime[levelStep].patternProba[ID].proba;
                //Debug.Log("Rand=" + rand.ToString());
                //Debug.Log("currentProba=" + currentProba.ToString());
                //Debug.Log("ID=0");
                while (rand > currentProba && ID < levelData.patternTime[levelStep].patternProba.Count-1)
                {
                    ID++;
                    currentProba += levelData.patternTime[levelStep].patternProba[ID].proba;
                    //Debug.Log("currentProba=" + currentProba.ToString());
                    //Debug.Log("ID=" + ID.ToString());

                }
                string name = levelData.patternTime[levelStep].patternProba[ID].pattern.name;

                //int patternIdToSpawn = Random.Range(0, levelData.patternTime[levelStep].patternProba.Count-1);
                //Debug.Log("Pattern:" + name);
                ND_EnemyPattern dataLoaded;

               // dataLoaded = (ND_EnemyPattern)Resources.Load("" + name + "", typeof(ND_Level));
                dataLoaded = Resources.Load(name, typeof(ND_EnemyPattern)) as ND_EnemyPattern;
                //Debug.Log(dataLoaded.ToString());
                #if UNITY_EDITOR
                //dataLoaded = (ND_EnemyPattern)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/" + name + ".asset", typeof(ND_EnemyPattern));
                #endif
                for (int i = 0; i < dataLoaded.pattern.Count; i++)
                {
                    ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType(ND_EnemyFactory.instance.objectPrefabs[dataLoaded.pattern[i].archetype].name, false).GetComponent<ND_Enemy>();
                    tempEnemy.transform.position = dataLoaded.pattern[i].position;
                    tempEnemy.Activate();
                    enemyCount++;
                }
                levelStep++;
                if (levelStep >= levelData.patternTime.Count)
                {
                    timer = 0;
                    levelStep = 0; //WIN CONDITION
                    levelID++;

                    levelData = Resources.Load("Level " + levelID, typeof(ND_Level)) as ND_Level;
                    if(levelData == null)
                    {
                        bLevelsDone = true;
                    }
                }
                //Debug.Log(levelStep.ToString());
            }
            if(bLevelsDone && enemyCount<=0)
            {
                GameEventManager.TriggerVictory();
            }
            //Debug.Log(bLevelsDone);
            //Debug.Log(enemyCount);
        }
	}
}
