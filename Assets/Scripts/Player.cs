using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float moveSpeed;
    public int currentAnimFrame = 0;
    public int maxAnimFrame = 2;
    public float animTime = 0.05f;

    public Sprite[] animSprites;

    public Vector2 lastMoveVector;

    public float nextAnimFrame;
    private SpriteRenderer spriteRenderer;
    new private Rigidbody2D rigidbody2D;

    // Use this for initialization
    void Start()
    {
        nextAnimFrame = Time.fixedTime + nextAnimFrame;
        lastMoveVector = new Vector2(1, 0);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        spriteRenderer.sprite = animSprites[currentAnimFrame];
        if (lastMoveVector.x < 0) spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        if (lastMoveVector.x > 0) spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
    }

    void FixedUpdate()
    {
        Vector2 moveVector = new Vector2(0, 0);

        if (Input.GetAxis("Horizontal") > 0)
        {
            moveVector += new Vector2(1, 0);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            moveVector += new Vector2(-1, 0);
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            moveVector += new Vector2(0, 1);
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            moveVector += new Vector2(0, -1);
        }

        Vector2 newPosition = (Vector2)transform.position + (moveVector * moveSpeed);
        rigidbody2D.MovePosition(newPosition);

        if (moveVector.magnitude > 0)
        {
            lastMoveVector = moveVector;
            if (Time.fixedTime >= nextAnimFrame)
            {
                currentAnimFrame++;
                nextAnimFrame = Time.fixedTime + animTime;
                if (currentAnimFrame == maxAnimFrame)
                {
                    currentAnimFrame = 0;
                }
            }
        }
    }
}
