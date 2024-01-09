using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.U2D.Path;
using UnityEngine;

public class Camera : MonoBehaviour
{
    //Gets player and it's Rigidbody
    public Rigidbody2D RB;
    public GameObject player;
    public Movement movement;

    //Camera Floats
    public float smoothSpeed;
    private float smoothPosition;
    private float xOffset;
    public float changeSpeed;
    public float cameraDistance;
    private float earlyPlayer;
    private float latePlayer;

    public float playerY;
    public float cameraY;
    public float yOffset;
    public float ySpeed;
    public float fallingSpeed;
    private float YO;
    private float YS;

    public float minYPos;
    public float maxYPos;

    
    

    //Direction Booleans
    private bool goRight;
    private bool goLeft;
    private bool stopLeft;
    private bool stopRight;

    public bool grounded;
    public bool staticX;
    public bool staticY;
    private bool SY;
    



    void Start()
    {
        movement = GameObject.FindGameObjectWithTag("Player").GetComponent<Movement>();

        YO = yOffset;
        YS = ySpeed;
        SY = staticY;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks if the player is stopped
        earlyPlayer = player.transform.position.x;

        if (earlyPlayer == latePlayer)
        {
            stopRight = true;
            stopLeft = true;
        }
        else
        {
            stopRight = false;
            stopLeft = false;
        }
    }

    void FixedUpdate()
    {
        //Changes smoothPosition with a lerp and offset

        smoothPosition = Mathf.Lerp(transform.position.x,RB.position.x + xOffset,smoothSpeed * Time.deltaTime);

        cameraY = Mathf.Lerp(cameraY, playerY, ySpeed * Time.deltaTime);
        
    }
    void LateUpdate()
    {
        //Moves camera based on smoothPosition

        if(staticX == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y,-10);
        }
        else
        {
            transform.position = new Vector3(smoothPosition,transform.position.y,-10);
        }

        if(staticY == true || SY == true) 
        {
            transform.position = new Vector3(transform.position.x,transform.position.y,-10);
        }
        else
        {
            transform.position = new Vector3(transform.position.x,cameraY,-10);
        }

        latePlayer = player.transform.position.x;
        
        //Moves offset based on plaer movement
            if (Input.GetKey(KeyCode.D))
        {
            if(stopRight == false && goRight == true)
            {
                xOffset = Mathf.Lerp(xOffset, cameraDistance, changeSpeed * Time.deltaTime);
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if(stopLeft == false && goLeft == true)
            {
                xOffset = Mathf.Lerp(xOffset, -cameraDistance, changeSpeed * Time.deltaTime);
            }
        }

        //Switches Directions
        if (Input.GetKeyDown(KeyCode.D))
        {
            goRight = true;
            goLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            goLeft = true;
            goRight = false;
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            goLeft = true;
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            goRight = true;
        }

        //Vertical Movement

        if(movement.isGrounded == true)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if(grounded == true)
        {
        playerY = player.transform.position.y + yOffset;
        }

        if(movement.gravitySwaped == true && movement.floorUp == true)
        {
            yOffset = -YO;
        }
        
        if(movement.gravitySwaped == false && movement.floorDown == true)
        {
            yOffset = YO;
        }

        if(movement.jumped == false && grounded == false || movement.RB.velocity.y <= -18 || movement.RB.velocity.y >= 18)
        {
            playerY = player.transform.position.y + yOffset;
            ySpeed = fallingSpeed;
        }
        else
        {
            ySpeed = YS;
        }

        if(cameraY < minYPos || cameraY > maxYPos)
        {
            staticY = true;
        }
        else
        {
            staticY = false;
        }

    }
}
