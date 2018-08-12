using UnityEngine;

public enum Facing
{
    LEFT = 0,
    RIGHT = 1,
    UP = 2,
    DOWN = 3,
}

public class Player : MonoBehaviour
{

    public float MoveSpeed;
    public int CurrentAnimFrame = 0;
    public int MaxAnimFrame = 2;
    public float AnimTime = 0.05f;
    public Facing Facing = Facing.RIGHT;
    public Sprite[] AnimSpritesLR;
    public Sprite[] AnimSpritesD;
    public Sprite[] AnimSpritesU;

    public Vector2 LastMoveVector;

    public Transform[] ProjectileSpawnPoints;
    public GameObject ProjectilePrefab;

    public int CurrentBarrierCollisions;

    public float nextAnimFrame;
    private SpriteRenderer spriteRenderer;
    private GameController controller;
    new private Rigidbody2D rigidbody2D;

    // Use this for initialization
    void Start()
    {
        nextAnimFrame = Time.fixedTime + nextAnimFrame;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        controller = FindObjectOfType<GameController>();
    }

    private void Update()
    {
        UpdateFacing();
        SetSprite();

        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), Mathf.RoundToInt(transform.position.y));

        if (Input.GetButtonDown("Fire1"))
        {
            if (controller.CanShoot())
            {
                SpawnProjectiles();
            }
        }

        if (CurrentBarrierCollisions > 0)
        {
            controller.BarrierHit();
        }
    }

    private void SetSprite()
    {
        if (Facing == Facing.LEFT | Facing == Facing.RIGHT)
        {
            spriteRenderer.sprite = AnimSpritesLR[CurrentAnimFrame];
        }
        else if (Facing == Facing.UP)
        {
            spriteRenderer.sprite = AnimSpritesU[CurrentAnimFrame];
        }
        else if (Facing == Facing.DOWN)
        {
            spriteRenderer.sprite = AnimSpritesD[CurrentAnimFrame];
        }
    }

    private void UpdateFacing()
    {
        if (LastMoveVector.x < 0)
        {
            Facing = Facing.LEFT;
            spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (LastMoveVector.x > 0)
        {
            Facing = Facing.RIGHT;
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
            if (LastMoveVector.y > 0)
            {
                Facing = Facing.UP;
            }
            else if (LastMoveVector.y < 0)
                Facing = Facing.DOWN;
        }
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

        Vector2 newPosition = (Vector2)transform.position + (moveVector * MoveSpeed);
        rigidbody2D.MovePosition(newPosition);

        if (moveVector.magnitude > 0)
        {
            LastMoveVector = moveVector;
            if (Time.fixedTime >= nextAnimFrame)
            {
                CurrentAnimFrame++;
                nextAnimFrame = Time.fixedTime + AnimTime;
                if (CurrentAnimFrame == MaxAnimFrame)
                {
                    CurrentAnimFrame = 0;
                }
            }
        }
    }

    private void SpawnProjectiles()
    {
        Vector3 spawnPoint = ProjectileSpawnPoints[(int)Facing].position;
        Quaternion rotation = Quaternion.identity;
        switch (Facing)
        {
            case Facing.LEFT:
                rotation = Quaternion.Euler(0, 180, 0);
                break;
            case Facing.RIGHT:
                rotation = Quaternion.identity;
                break;
            case Facing.DOWN:
                rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Facing.UP:
                rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
        Vector2[] moveVectors = new Vector2[3];
        switch ((int)LastMoveVector.x) // oh god this is horrible
        {
            case -1:
                switch ((int)LastMoveVector.y)
                {
                    case -1:
                        moveVectors[0] = new Vector2(-1, -0.5f);
                        moveVectors[1] = new Vector2(-1, -1);
                        moveVectors[2] = new Vector2(-0.5f, -1);
                        break;
                    case 0:
                        moveVectors[0] = new Vector2(-1, 0.5f);
                        moveVectors[1] = new Vector2(-1, 0);
                        moveVectors[2] = new Vector2(-1, -0.5f);
                        break;
                    case 1:
                        moveVectors[0] = new Vector2(-1, 0.5f);
                        moveVectors[1] = new Vector2(-1, 1);
                        moveVectors[2] = new Vector2(-.5f, 1);
                        break;
                }
                break;
            case 0:
                switch ((int)LastMoveVector.y)
                {
                    case -1:
                        moveVectors[0] = new Vector2(-0.5f, -1);
                        moveVectors[1] = new Vector2(0, -1);
                        moveVectors[2] = new Vector2(0.5f, -1);
                        break;
                    case 0: // this should be the default spawn case
                        moveVectors[0] = new Vector2(1, 0.5f);
                        moveVectors[1] = new Vector2(1, 0);
                        moveVectors[2] = new Vector2(1, 0.5f);
                        break;
                    case 1:
                        moveVectors[0] = new Vector2(-0.5f, 1);
                        moveVectors[1] = new Vector2(0, 1);
                        moveVectors[2] = new Vector2(0.5f, 1);
                        break;
                }
                break;
            case 1:
                switch ((int)LastMoveVector.y)
                {
                    case -1:
                        moveVectors[0] = new Vector2(1, -0.5f);
                        moveVectors[1] = new Vector2(1, -1);
                        moveVectors[2] = new Vector2(0.5f, -1);
                        break;
                    case 0:
                        moveVectors[0] = new Vector2(1, 0.5f);
                        moveVectors[1] = new Vector2(1, 0);
                        moveVectors[2] = new Vector2(1, -0.5f);
                        break;
                    case 1:
                        moveVectors[0] = new Vector2(1, 0.5f);
                        moveVectors[1] = new Vector2(1, 1);
                        moveVectors[2] = new Vector2(0.5f, 1);
                        break;
                }
                break;
        }
        for (int i = 0; i < 3; i++)
        {
            GameObject projectile;
            projectile = Instantiate(ProjectilePrefab, spawnPoint, rotation);
            projectile.GetComponent<PlayerProjectile>().MoveVector = moveVectors[i];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Emitter")
        {
            collision.gameObject.GetComponent<Emitter>().Deactivate();
        } else if (collision.gameObject.tag == "PowerUp")
        {
            collision.gameObject.GetComponent<PowerUp>().Consume();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PhaseBarrier")
        {
            CurrentBarrierCollisions++;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PhaseBarrier")
        {
            CurrentBarrierCollisions--;
        }
    }
}
