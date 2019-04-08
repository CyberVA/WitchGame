using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed;
    private Vector2 velocity;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    void Update()
    {
        //Basic Movement - right now this isnt using any of the custom input stuff. Because it also only partially exists...
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        velocity = moveInput.normalized * speed;

        //Okay so this is where i test the new movement- this is gonna look REALLY WEIRD at first and i'll be polishing it from there
        //also make this into a switch later you fu--
        //if(Input.GetKeyDown(InputManager.IM.Up))
        //{
        //    Debug.Log("h but up");
        //}

        //if (Input.GetKeyDown(InputManager.IM.Down))
        //{
        //    Debug.Log("h but down");
        //}

        //if (Input.GetKeyDown(InputManager.IM.Left))
        //{
        //    Debug.Log("h but left");
        //}

        //if (Input.GetKeyDown(InputManager.IM.Right))
        //{
        //    Debug.Log("h but right");
        //}
    }
}
