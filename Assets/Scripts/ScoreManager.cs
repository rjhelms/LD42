﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager> {

    protected ScoreManager() { }

    public int HitPoints = 24;
    public int MaxHitPoints = 24;

}