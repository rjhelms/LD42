using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour {

    private GameController controller;

	// Use this for initialization
	void Start () {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        controller.RegisterEmitter(gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
