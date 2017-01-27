using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PatternActor
{
    public int archetype = 0;
    public Vector3 position;
    public Quaternion rotation;
}

public class ND_EnemyTransform
{
    public int archetype;
    public Vector3 position;
    public Quaternion rotation;
}

public class ND_EnemyPattern : ScriptableObject 
{
    public List<PatternActor> pattern = new List<PatternActor>();

    /*public void saveAllEnemies()
    {
        GameObject[] dominosTrsfrm = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < dominosTrsfrm.Length; ++i) {
            Debug.Log(i);
        }
    }*/

    public void Init(List<ND_EnemyTransform> tempList)
    {
        for(int i = 0; i < tempList.Count; i++)
        {
            PatternActor temp = new PatternActor();
            temp.position = tempList[i].position;
            temp.rotation = tempList[i].rotation;
            temp.archetype = tempList[i].archetype;
            Debug.Log("Saved 1 more enemy in the pattern");
            pattern.Add(temp);
        }
    }
    public static ND_EnemyPattern CreateInstance(List<ND_EnemyTransform> tempList)
    {
        ND_EnemyPattern data = Object.FindObjectOfType<ND_EnemyPattern>();

        if(data == null)
            data = ScriptableObject.CreateInstance<ND_EnemyPattern>();

        data.Init(tempList);
        return data;
    }
}
