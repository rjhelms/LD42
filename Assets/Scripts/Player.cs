using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveSpeed;
    public float rotateSpeed;
    private CharacterController controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        float moveHoriz = Input.GetAxis("Horizontal");
        float moveVert = Input.GetAxis("Vertical");
        float moveRotate = Input.GetAxis("Rotate");
        Debug.Log("Horiz: " + moveHoriz);
        Debug.Log("Vert: " + moveVert);
        transform.Rotate(0, moveRotate * rotateSpeed * Time.deltaTime, 0);
        Vector3 moveVector = transform.TransformDirection(moveHoriz, 0, moveVert);
        controller.Move(moveVector * moveSpeed * Time.deltaTime);
	}
}
