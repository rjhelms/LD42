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
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Deactivate()
    {
        if (Active)
        {
            Active = false;
            spriteRenderer.sprite = InactiveSprite;
            controller.DeactivateEmitter(gameObject);
        }
    }
}
