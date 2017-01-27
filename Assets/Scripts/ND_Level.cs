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

    public static ND_Level CreateInstance()
    {
        ND_Level data = Object.FindObjectOfType<ND_Level>();

        if (data == null)
            data = ScriptableObject.CreateInstance<ND_Level>();

        return data;
    }

}
