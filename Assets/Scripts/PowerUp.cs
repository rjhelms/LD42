using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public int HealthUp;
    public int MaxHealthUp;
    public int MaxCannonUp;
    public int LivesUp;
    public int Score;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Consume()
    {
        ScoreManager.Instance.HitPoints += HealthUp;
        ScoreManager.Instance.MaxHitPoints += MaxHealthUp;
        if (ScoreManager.Instance.HitPoints > ScoreManager.Instance.MaxHitPoints)
        {
            ScoreManager.Instance.HitPoints = ScoreManager.Instance.MaxHitPoints;
        }
        ScoreManager.Instance.MaxCannonPower += MaxCannonUp;
        ScoreManager.Instance.Lives += LivesUp;
        ScoreManager.Instance.Score += Score;
        Destroy(gameObject);
    }
}
