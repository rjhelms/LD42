using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhaseBarrierState
{
    IN,
    ACTIVE,
    OUT
}

public class PhaseBarrier : MonoBehaviour {

    public PhaseBarrierState State;
    public float InTime;
    public float OutTime;
    public float FlashTime;
    public GameObject ParentEmitter;
    public Sprite[] PhaseBarrierSprites;
    public float SpriteChangeTime = 0.3f;
    public float SpriteChangeChance = 0.2f;
    private float nextStateChangeTime;
    private float nextFlashTime;
    private float nextSpriteChangeTime;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private GameController controller;

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        rigidbody2D = GetComponent<Rigidbody2D>();
        nextStateChangeTime = Time.time + InTime;
        nextFlashTime = Time.time + FlashTime;

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
        RandomSprite();
        nextSpriteChangeTime = Time.time + SpriteChangeTime;

    }

    // Update is called once per frame
    void Update() {
        if (State != PhaseBarrierState.ACTIVE & Time.time > nextFlashTime)
        {
            nextFlashTime = Time.time + FlashTime;
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        if (controller.GameState == GameState.RUNNING)
        {
            if (Time.time > nextSpriteChangeTime)
            {
                nextSpriteChangeTime = Time.time + SpriteChangeTime;
                if (Random.value < SpriteChangeChance)
                {
                    RandomSprite();
                }
            }
            switch (State)
            {
                case PhaseBarrierState.IN:
                    if (Time.time > nextStateChangeTime)
                    {
                        spriteRenderer.enabled = true;
                        rigidbody2D.simulated = true;
                        State = PhaseBarrierState.ACTIVE;
                    }
                    break;
                case PhaseBarrierState.ACTIVE:
                    break;
                case PhaseBarrierState.OUT:
                    if (Time.time > nextStateChangeTime)
                    {
                        controller.PhaseBarrierList.Remove(gameObject);
                        Destroy(gameObject);
                    }
                    break;
            }
        }
	}

    public void Die()
    {
        State = PhaseBarrierState.OUT;
        nextFlashTime = Time.time + FlashTime;
        nextStateChangeTime = Time.time + OutTime;
        spriteRenderer.enabled = false;
        rigidbody2D.simulated = false;
    }

    private void RandomSprite()
    {
        spriteRenderer.sprite = PhaseBarrierSprites[Random.Range(0, PhaseBarrierSprites.Length)];
    }
}
