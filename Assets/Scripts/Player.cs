using UnityEngine;

public enum Facing
{
    LEFT = 0,
    RIGHT = 1,
}

public class Player : MonoBehaviour
{

    public float MoveSpeed;
    public int CurrentAnimFrame = 0;
    public int MaxAnimFrame = 2;
    public float AnimTime = 0.05f;
    public Facing Facing = Facing.RIGHT;
    public Sprite[] AnimSprites;

    public Vector2 LastMoveVector;

    public Transform[] ProjectileSpawnPoints;
    public GameObject ProjectilePrefab;

    public float nextAnimFrame;
    private SpriteRenderer spriteRenderer;
    new private Rigidbody2D rigidbody2D;

    // Use this for initialization
    void Start()
    {
        nextAnimFrame = Time.fixedTime + nextAnimFrame;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        spriteRenderer.sprite = AnimSprites[CurrentAnimFrame];
        if (LastMoveVector.x < 0)
        {
            Facing = Facing.LEFT;
            spriteRenderer.transform.localScale = new Vector3(-1, 1, 1);
        }
        if (LastMoveVector.x > 0)
        {
            Facing = Facing.RIGHT;
            spriteRenderer.transform.localScale = new Vector3(1, 1, 1);
        }
        transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);

        if (Input.GetButtonDown("Fire1"))
        {
            GameObject projectile;
            int direction = 1;
            if (Facing == Facing.LEFT)
            {
                direction = -1;
            }
            projectile = GameObject.Instantiate(ProjectilePrefab, ProjectileSpawnPoints[(int)Facing].position, Quaternion.identity);
            projectile.GetComponent<PlayerProjectile>().MoveVector = new Vector2(direction, -1);
            projectile.transform.localScale = new Vector3(direction, 1, 1);
            projectile = GameObject.Instantiate(ProjectilePrefab, ProjectileSpawnPoints[(int)Facing].position, Quaternion.identity);
            projectile.GetComponent<PlayerProjectile>().MoveVector = new Vector2(direction, 0);
            projectile.transform.localScale = new Vector3(direction, 1, 1);
            projectile = GameObject.Instantiate(ProjectilePrefab, ProjectileSpawnPoints[(int)Facing].position, Quaternion.identity);
            projectile.GetComponent<PlayerProjectile>().MoveVector = new Vector2(direction, 1);
            projectile.transform.localScale = new Vector3(direction, 1, 1);
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
}
