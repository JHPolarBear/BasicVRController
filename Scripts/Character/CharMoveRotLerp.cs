using UnityEngine;
using System.Collections;

public class CharMoveRotLerp : MonoBehaviour {

    // Character's moving speed
    public float speed = 20f;
    // Variable to differ forward/backward walking speed
    float speedFixed;
    // Rotation speed
    public float rotationSpeed = 180f;

    /***Related to jump function***/

    //Possible jump count
    public int jumpCount = 1;
    //Jump power
    public float jumpPower = 2f;
    //Maximum jump height
    public float jumpHeight = 2f;

    //Character Controller component
    CharacterController characterController;

    //Animator
    Animator animator;

    // Use this for initialization
    void Start()
    {
        //get character controller and animater component from the object
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate smoothly according to A(left) and S(right) key
        // Multiply roatation speed to left/right around object's up vector
        transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);

        //Variable to keep "vertical" input value
        float vrtVal = 0f;

        //save current vertical input value into vrtVal 
        vrtVal = Input.GetAxis("Vertical");

        //Judge whether the object move forward or backward using vrtVal
        //If it moves backwards, reduce object speed into 1/4
        if (vrtVal > 0)
        {
            speedFixed = speed;
        }
        else if (vrtVal < 0)
        {
            speedFixed = speed / 4;
        }

        //Calculate direction vector for character to move
        Vector3 direction = transform.forward.normalized * speedFixed * vrtVal;

        //Only jump if jumpcount > 0
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            Jump();
            jumpCount--;
        }

        //move the character
        characterController.Move(direction * Time.deltaTime);

        //Set animation
        //Running animation value
        animator.SetFloat("Run", characterController.velocity.magnitude);

        //activate jump animation only when jumpcount equals 0
        //It need to be fixed if we implement muti-time jump later
        animator.SetInteger("Jump", jumpCount);

        //Walking animation when VrtVal is negative, which mease walk backward 
        animator.SetInteger("Walk", (int)vrtVal);
    }

    void Jump()
    {
        //multiply jump height and up vector, then add to current position
        Vector3 dir = transform.position + transform.up * jumpHeight;

        //Using Lerp function to implement jump affected by gravity
        transform.position = Vector3.Lerp(transform.position, dir, jumpPower * Time.deltaTime);
    }

    //add 1 to jumpcount when object touches the ground after jump
    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.tag == "Terrain")
        {
            jumpCount = 1;
        }
    }

}
