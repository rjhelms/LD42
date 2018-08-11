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

    private float nextStateChangeTime;
    private float nextFlashTime;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        rigidbody2D = GetComponent<Rigidbody2D>();
        nextStateChangeTime = Time.time + InTime;
        nextFlashTime = Time.time + FlashTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (State != PhaseBarrierState.ACTIVE & Time.time > nextFlashTime)
        {
            nextFlashTime = Time.time + FlashTime;
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        switch (State)
        {
            case PhaseBarrierState.IN:
                if (Time.time > nextStateChangeTime)
                {
                    spriteRenderer.enabled = true;
                    rigidbody2D.simulated = true;
                }
                break;
            case PhaseBarrierState.ACTIVE:
                break;
            case PhaseBarrierState.OUT:
                break;
        }
	}
}
