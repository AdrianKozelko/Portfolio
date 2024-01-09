using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public Rigidbody2D RB;
    public BoxCollider2D groundChecker;
    public Death death;

    public float moveSpeed;
    public float maxSpeed;
    public float moveAcceleration;
    public float moveDeceleration;
    public float changeSpeed;
    public float turnSpeed;
    public float jumpForce;
    public float wallFloat;
    public float offset;
    public float floorFloat;
    public float downOffset;
    public float upOffset;
    public float gravityCap;
    public float jumpBuffer;
    public float jumpBufferTimer;
    public float jumpVelocity;
    public float coyoteTime;
    public float jumpRelease;

    public float minDeath;
    public float maxDeath;
    

    public float wallJumpForce;

    public float numberOfSwaps;
    
    public int numberOfJumps; 
    public int cameraMiddle;
    

    private float AF;
    private float CS;
    private float SG;

    public bool goRight;
    public bool goLeft;
    public bool rightDecel;
    public bool leftDecel;

    public bool isGrounded;
    public bool canJump;
    public bool wallRight;
    public bool wallLeft;
    public bool floorDown;
    public bool floorUp;
    public bool startCoyoteTimer;
    public bool canRelease;
    public bool gravityChanged1;
    public bool gravityChanged2;
    public bool jumped;
    public bool gravitySwaped = false;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        //Gets float values from Inspector so they can't be changed
        SG = Mathf.Sqrt(RB.gravityScale);
        AF = moveAcceleration;
        CS = moveAcceleration / maxSpeed;
        jumpForce = jumpForce * SG;

        death = GameObject.FindGameObjectWithTag("Death").GetComponent<Death>();
    }

    void Update()
    {
        //Movement


        //Changes moveSpeed by moveAcceleration depending on direction
        if(Input.GetKey(KeyCode.D))
        {
            if(goRight == true)
            {
                if(wallRight == false)
                {
                    moveSpeed = Mathf.MoveTowards(moveSpeed, maxSpeed,moveAcceleration * Time.deltaTime);
                }
            }
        }
         if(Input.GetKey(KeyCode.A))
        {
            if(goLeft == true)
            {
                if(wallLeft == false)
                {
                    moveSpeed = Mathf.MoveTowards(moveSpeed, -maxSpeed, moveAcceleration * Time.deltaTime);
                }
            }
        }

        //Switches Directions
        if (Input.GetKeyDown(KeyCode.D))
        {
            goRight = true;
            goLeft = false;

            rightDecel = true;
            leftDecel = false;

            moveAcceleration = AF;

            if (moveSpeed <= 0)
            {
                changeSpeed = -moveSpeed;
                moveAcceleration += changeSpeed * CS * turnSpeed;
            }
            else
            {
                changeSpeed = moveSpeed;
                moveAcceleration += changeSpeed * CS * turnSpeed;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            goLeft = true;
            goRight = false;

            leftDecel = true;
            rightDecel = false;

            moveAcceleration = AF;

            if (moveSpeed <= 0)
            {
                changeSpeed = -moveSpeed;
                moveAcceleration += changeSpeed * CS * turnSpeed;
            }
            else
            {
                changeSpeed = moveSpeed;
                moveAcceleration += changeSpeed * CS * turnSpeed;
            }
        }

        //Decelerates moveSpeed depending on direction
        if (Input.GetKeyUp(KeyCode.D))
        {            
            goLeft = true;

            if (goRight == true)
            {
                moveAcceleration = AF;

                if (moveSpeed <= 0)
                {
                    changeSpeed = -moveSpeed;
                    moveAcceleration += changeSpeed * CS * turnSpeed;
                }
                else
                {
                    changeSpeed = moveSpeed;
                    moveAcceleration += changeSpeed * CS * turnSpeed;
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            goRight = true;

            if (goLeft == true)
            {
                moveAcceleration = AF;

                if (moveSpeed <= 0)
                {
                    changeSpeed = -moveSpeed;
                    moveAcceleration += changeSpeed * CS * turnSpeed;
                }
                else
                {
                    changeSpeed = moveSpeed;
                    moveAcceleration += changeSpeed * CS * turnSpeed;
                }
            }
        }
        //Movement ends////////////////////////////////////////////////////////////////////////////////////////////////////


        //Jumping

        if(startCoyoteTimer == true)
        {
            coyoteTime += Time.deltaTime;
        }

        jumpVelocity = RB.velocity.y;

        if(jumpVelocity <= 5 && isGrounded == false && jumped == true)
        {
            if(gravityChanged1 == true)
            {
            RB.gravityScale = RB.gravityScale / 1.5f;
            gravityChanged1 = false;
            }
        }
        if(jumpVelocity <= -5 && isGrounded == false && jumped == true)
        {
            if(gravityChanged2 == true)
            {
            RB.gravityScale = RB.gravityScale * 1.5f; 
            gravityChanged2 = false;
            }
        }

        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferTimer = 0;

            if(canRelease == true)
            {
            jumpRelease = 9f;
            }

            if(coyoteTime <= 0.1f && numberOfJumps < 1)
            {
                canJump = true;
            }
        }

        if(Input.GetButton("Jump"))
        {
            jumpBufferTimer += Time.deltaTime;

            if(canRelease == true)
            {
            jumpRelease = Mathf.MoveTowards(jumpRelease,4,12 * Time.deltaTime);
            }

            if(canJump == true)
            {
            Jump();
            jumped = true;
            }
        }
        
        if(Input.GetButtonUp("Jump"))
        {
            if(gravitySwaped == false)
            {
            RB.gravityScale = jumpRelease;
            }
            else
            {
                RB.gravityScale = -jumpRelease;
            }
            canRelease = false;
        }
        //Jumping ends////////////////////////////////////////////////////////////////////////////////////////////////////


        //Collision Detection

        Vector2 rightDirection = Vector2.right;
        Vector2 leftDirection = Vector2.left;
        Vector2 downDirection = Vector2.down;
        Vector2 upDirection = Vector2.up;

        Vector2 position = transform.position;

        float wallDistance = wallFloat;
        float floorDistance = floorFloat;

        //Casts 2 Raycasts for each corner of the player in every direction
        RaycastHit2D RaycastRight1 = Physics2D.Raycast(position + new Vector2(0,offset), rightDirection, wallDistance, groundLayer);
        RaycastHit2D RaycastRight2 = Physics2D.Raycast(position + new Vector2(0,-offset), rightDirection, wallDistance, groundLayer);

        RaycastHit2D RaycastLeft1 = Physics2D.Raycast(position + new Vector2(0,offset), leftDirection, wallDistance, groundLayer);
        RaycastHit2D RaycastLeft2 = Physics2D.Raycast(position + new Vector2(0,-offset), leftDirection, wallDistance, groundLayer);

        RaycastHit2D RaycastDown1 = Physics2D.Raycast(position + new Vector2(downOffset,0), downDirection, floorDistance, groundLayer);
        RaycastHit2D RaycastDown2 = Physics2D.Raycast(position + new Vector2(-downOffset,0), downDirection, floorDistance, groundLayer);

        RaycastHit2D RaycastUp1 = Physics2D.Raycast(position + new Vector2(upOffset,0), upDirection, floorDistance, groundLayer);
        RaycastHit2D RaycastUp2 = Physics2D.Raycast(position + new Vector2(-upOffset,0), upDirection, floorDistance, groundLayer);

        //Checks for walls
        if (RaycastRight1.collider != null || RaycastRight2.collider != null)
{
 Debug.DrawRay(position, rightDirection, Color.green);
 wallRight = true;
}
else
{ 
 Debug.DrawRay(position, rightDirection, Color.red);
 wallRight = false;
}

if(RaycastLeft1.collider != null || RaycastLeft2.collider != null)
{
 Debug.DrawRay(position, leftDirection, Color.green);
 wallLeft = true;
}
else
{ 
 Debug.DrawRay(position, leftDirection, Color.red);
 wallLeft = false;
}

        //Checks for ground
        if (RaycastDown1.collider != null || RaycastDown2.collider != null)
        {
            Debug.DrawRay(position, downDirection, Color.green);
            floorDown = true;
        }
        else
        {
            Debug.DrawRay(position, downDirection, Color.red);
            floorDown = false;
        }

         if (RaycastUp1.collider != null || RaycastUp2.collider != null)
        {
            Debug.DrawRay(position, upDirection, Color.green);
            floorUp = true;
        }
        else
        {
            Debug.DrawRay(position, upDirection, Color.red);
            floorUp = false;
        }

        //Stops movement if against wall
        if (goRight == true && wallRight == true) //&& floorUp == false)
{
    moveSpeed = 0;
}
if(goLeft == true && wallLeft == true) //&& floorUp == false)
{
    moveSpeed = 0;
}

        //Changes the raycast position if airborn
        if (isGrounded == false && floorDown == false)
        {
            offset = 0.5f;
        }
        else
        {
            offset = 0;
        }
        //Collision Detection ends////////////////////////////////////////////////////////////////////////////////////////////////////


        //Gravity Swap
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            numberOfSwaps += 1;

            cameraMiddle += 1;

            if(numberOfSwaps == 1)
            {
                if(gravitySwaped == false)
                {
                    RB.gravityScale = 6;
                }
                else
                {
                    RB.gravityScale = -6;
                }
                
                groundChecker.offset = new Vector2(0,groundChecker.offset.y * -1);

                RB.gravityScale = -RB.gravityScale;
            }       
        }

        //Checks if the player has swapped gravity
        if(RB.gravityScale < 0)
        {
            gravitySwaped = true;
        }
        else
        {
            gravitySwaped = false;
        }

        //Gravity Swap ends////////////////////////////////////////////////////////////////////////////////////////////////////

        //Resets Scene (Dying)

        if(transform.position.y < minDeath || transform.position.y > maxDeath)
        {
            death.RunDeath();
        }
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        RB.velocity = new Vector2(moveSpeed,RB.velocity.y);

         if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
        }
        else
        {
            moveSpeed = Mathf.MoveTowards(moveSpeed, 0, moveDeceleration * Time.deltaTime); 
        }
    }
    
    //Ground Detection
       private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //Lets player swap gravity again
            if(numberOfSwaps >= 1)
            {
            numberOfSwaps = 0;
            }

            //Resets values now that player is grounded
            numberOfJumps = 0;
            isGrounded = true;
            canJump = true;
            startCoyoteTimer = false;
            coyoteTime = 0;
            canRelease = true;
            gravityChanged1 = true;
            gravityChanged2 = true;
            jumped = false;
            cameraMiddle = 0;

            if(gravitySwaped == false)
            {
            RB.gravityScale = 4;
            }
        }
    }

    //Ground leaving detection
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
            canJump = false;
            startCoyoteTimer = true;
        }
    }

    private void Jump()
    {
        //Jumps if player is grounded
        if(canJump == true && jumpBufferTimer <= jumpBuffer)
        {
            jumpRelease = 9f;
            numberOfJumps += 1;
            canJump = false;

            //Switched jump direction if the player has swapped gravity
            if(gravitySwaped == false)
            {
            RB.gravityScale = 4;
            RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
            RB.gravityScale = -4;
            RB.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
            }
        }
    }
}

