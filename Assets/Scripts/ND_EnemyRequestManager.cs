using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// LD Script
/// Spawns new ennemies every X seconds
/// Uses 1 timer per enemy type, updated in Update, affected by modifier
/// </summary>
public class ND_EnemyRequestManager : MonoBehaviour {

    public float basicApparitionFrequency = 1.0f;
    public float heavyApparitionFrequency = 5.0f;
    public float shieldedApparitionFrequency = 7.0f;
    public float bombApparitionFrequency = 7.0f;
    public bool shouldSpawnPattern = false;
    public List<ND_EnemyPattern> patternsToSpawn = new List<ND_EnemyPattern>();
    float timer = 0.0f;
    float timerHeavy = 0.0f;
    float timerShielded = 0.0f;
    float timerBomb = 0.0f;

    private float timeModifier = 1.0f; //SlowMotion reaction (affects spawn timers)

    void Start()
    {
        GameEventManager.SlowMotionState_Begin += SlowMoActivation;
        GameEventManager.SlowMotionState_End += SlowMoDEActivation;
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
    void spawnBasic()
    {
        ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType("Enemy", false).GetComponent<ND_Enemy>();
        tempEnemy.Activate();
    }
    
    void spawnHeavy()
    {
        ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType("EnemyHeavy", false).GetComponent<ND_Enemy>();
        tempEnemy.Activate();
    }
    void spawnShield()
    {
        ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType("EnemyShield", false).GetComponent<ND_Enemy>();
        tempEnemy.Activate();
    }
    void spawnBomb()
    {
        ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType("EnemyBomb", false).GetComponent<ND_Enemy>();
        tempEnemy.Activate();
    }
    void spawnPattern()
    {
        //Debug.Log("Assets/" + patternToSpawn.name + ".asset");
        for(int j = 0; j < patternsToSpawn.Count; j++)
        {
            ND_EnemyPattern dataLoaded = (ND_EnemyPattern)AssetDatabase.LoadAssetAtPath("Assets/" + patternsToSpawn[j].name + ".asset", typeof(ND_EnemyPattern));
            //Debug.Log(dataLoaded.pattern.Count.ToString());
            for (int i = 0; i < dataLoaded.pattern.Count; i++)
            {
                //Debug.Log(ND_EnemyFactory.instance.objectPrefabs[dataLoaded.pattern[i].archetype].name);

                ND_Enemy tempEnemy = ND_EnemyFactory.instance.GetObjectForType(ND_EnemyFactory.instance.objectPrefabs[dataLoaded.pattern[i].archetype].name, false).GetComponent<ND_Enemy>();
                tempEnemy.transform.position = dataLoaded.pattern[i].position;
                tempEnemy.Activate();
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        if (!GameEventManager.PauseState)
        {
            if (shouldSpawnPattern)
            {
                shouldSpawnPattern = false;
                spawnPattern();
            }
            //timeModifier = 1.0f;
            //if(ND_Enemy.input)
            //{
            //    timeModifier = ND_Enemy.m_fSlowModifierMax;
            //}
            timer += Time.deltaTime / timeModifier;
            timerHeavy += Time.deltaTime / timeModifier;
            timerShielded += Time.deltaTime / timeModifier;
            timerBomb += Time.deltaTime / timeModifier;

            if (timer >= (basicApparitionFrequency))
            {
                timer = 0.0f;
                spawnBasic();
            }
            if (timerHeavy >= (heavyApparitionFrequency))
            {
                timerHeavy = 0.0f;
                spawnHeavy();
            }
            if (timerShielded >= (shieldedApparitionFrequency))
            {
                timerShielded = 0.0f;
                spawnShield();
            }
            if (timerBomb >= (bombApparitionFrequency))
            {
                timerBomb = 0.0f;
                spawnBomb();
            }
        }
	}
}
