using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    protected ScoreManager() { }

    public int HitPoints = 24;
    public int MaxHitPoints = 24;
    public int Lives = 3;
    public int Level = 1;
    public int Score = 0;

    public void Reset()
    {
        Continue();
        Level = 1;
    }

    public void Continue()
    {
        Score = 0;
        Lives = 3;
        MaxHitPoints = 24;
        HitPoints = 24;
    }
}
