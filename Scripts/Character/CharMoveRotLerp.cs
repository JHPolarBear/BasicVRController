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

        //vrtVal 값에 따라 현재 전진하는지 후진하는지를 판단한 후,
        //후진일 경우 캐릭터 스피드/4 에 해당하는 값을 대입하여 걷는 효과 부여
        if (vrtVal > 0)
        {
            speedFixed = speed;
        }
        else if (vrtVal < 0)
        {
            speedFixed = speed / 4;
        }

        //최종적으로 캐릭터가 이동해야할 방향 벡터를 계산
        Vector3 direction = transform.forward.normalized * speedFixed * vrtVal;

        //점프 버튼 입력 시 점프 횟수가 남아있는 경우에만 점프 실행
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            Jump();
            jumpCount--;
        }

        //캐릭터의 위치 변경
        characterController.Move(direction * Time.deltaTime);

        //Set animation
        //캐릭터의 속력이 증가하면 달리는 애니메이션 실행
        animator.SetFloat("Run", characterController.velocity.magnitude);

        //점프 카운트가 0인 경우 점프 애니메이션 실행
        //만약 다중 점프를 구현한다면 Animator에서 조건 변경 필요
        animator.SetInteger("Jump", jumpCount);

        //만약 VrtVal 값이 음수인 경우, 뛰는 모션 대신 걷는 모션 실행
        animator.SetInteger("Walk", (int)vrtVal);
    }

    void Jump()
    {
        //현재 위치에 Up 벡터에 점프 높이를 곱한만큼을 더한다
        Vector3 dir = transform.position + transform.up * jumpHeight;

        //Lerp 함수를 활용하여 점프 시작 시에는 높이 올라가다가
        //올라갈수록 증가하는 높이를 감소시켰다.
        transform.position = Vector3.Lerp(transform.position, dir, jumpPower * Time.deltaTime);
    }

    //캐릭터가 땅에 닿는 경우 점프 카운트를 1로 증가시킴
    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.tag == "Terrain")
        {
            jumpCount = 1;
        }
    }

}
