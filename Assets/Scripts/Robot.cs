using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour {

    public Facing Facing;
    public Sprite[] DirectionSprites;

    public int HitPoints = 1;
    public int ScoreValue = 200;
    public int MoveSpeed = 1;
    public GameObject Projectile;

    private Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private GameController controller;
    private int CurrentCollisions;

	// Use this for initialization
	void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = FindObjectOfType<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 
                                         Mathf.RoundToInt(transform.position.y));
        spriteRenderer.sprite = DirectionSprites[(int)Facing];
	}

    void FixedUpdate()
    {
        if (CurrentCollisions > 0)
        {
            switch (Facing)
            {
                case Facing.LEFT:
                    Facing = Facing.RIGHT;
                    break;
                case Facing.RIGHT:
                    Facing = Facing.LEFT;
                    break;
                case Facing.UP:
                    Facing = Facing.DOWN;
                    break;
                case Facing.DOWN:
                    Facing = Facing.UP;
                    break;
            }
        }
        Vector2 moveVector = Vector2.zero;
        switch (Facing)
        {
            case Facing.LEFT:
                moveVector = new Vector2(-1, 0);
                break;
            case Facing.RIGHT:
                moveVector = new Vector2(1, 0);
                break;
            case Facing.UP:
                moveVector = new Vector2(0, 1);
                break;
            case Facing.DOWN:
                moveVector = new Vector2(0, -1);
                break;
        }
        moveVector *= MoveSpeed;
        Vector3 newPosition = transform.position + (Vector3)moveVector;
        rigidbody2D.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Terrain") | (collision.gameObject.tag == "PhaseBarrier") | (collision.gameObject.tag == "Enemy"))
        {
            switch (Facing)
            {
                case Facing.LEFT:
                    Facing = Facing.RIGHT;
                    break;
                case Facing.RIGHT:
                    Facing = Facing.LEFT;
                    break;
                case Facing.UP:
                    Facing = Facing.DOWN;
                    break;
                case Facing.DOWN:
                    Facing = Facing.UP;
                    break;
            }
        }
    }
}
