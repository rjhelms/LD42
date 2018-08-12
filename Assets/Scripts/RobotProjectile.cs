using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotProjectile : MonoBehaviour
{
    public Vector2 MoveVector;
    public int MoveSpeed = 2;
    public float TimeToLive = 2f;

    new private Rigidbody2D rigidbody2D;
    private float dieTime;
    private GameController controller;

    // Use this for initialization
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        dieTime = Time.time + TimeToLive;
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= dieTime)
        {
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if (controller.GameState == GameState.RUNNING)
        {
            rigidbody2D.MovePosition(transform.position + (Vector3)MoveVector * MoveSpeed);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PhaseBarrier" | collision.gameObject.tag == "Terrain")
        {
            Destroy(gameObject);
        } else if (collision.gameObject.tag == "Player")
        {
            controller.RobotHit();
            Destroy(gameObject);
        }
    }
}
