using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour {

    public bool Active = true;
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;
    private GameController controller;
    private SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        controller.RegisterEmitter(gameObject);
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Deactivate()
    {
        Active = false;
        spriteRenderer.sprite = InactiveSprite;
        controller.ActiveEmitterList.Remove(gameObject);
        controller.InactiveEmitterList.Add(gameObject);
    }
}
