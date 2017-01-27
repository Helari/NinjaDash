using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PatternTime
{
    public float timer;
    public List<PatternProba> patternProba = new List<PatternProba>();
}

[System.Serializable]
public class PatternProba
{
    public ND_EnemyPattern pattern;
    public int proba = 1;
}

public class ND_Level : ScriptableObject 
{
    public List<PatternTime> patternTime = new List<PatternTime>();

    public void Init(List<ND_EnemyTransform> tempList)
    {
        //for (int i = 0; i < tempList.Count; i++)
        //{
        //    PatternActor temp = new PatternActor();
        //    temp.position = tempList[i].position;
        //    temp.rotation = tempList[i].rotation;
        //    temp.archetype = tempList[i].archetype;
        //    Debug.Log("Saved 1 more enemy in the pattern");
        //    pattern.Add(temp);
        //}
    }
    public static ND_Level CreateInstance(/*List<ND_EnemyTransform> tempList*/)
    {
        ND_Level data = Object.FindObjectOfType<ND_Level>();

        if (data == null)
            data = ScriptableObject.CreateInstance<ND_Level>();

        //data.Init(tempList);
        return data;
    }

}
