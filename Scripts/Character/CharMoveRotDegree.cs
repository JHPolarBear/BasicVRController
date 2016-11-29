using UnityEngine;
using System.Collections;

public class CharMoveRotDegree : MonoBehaviour {

    public float speed = 20f;
    float speedFixed;
    public float rotationSpeed = 180f;

    //Jump 관련 변수
    //점프 가능 횟수를 저장(횟수로 지정함으로서 이단 점프 기능 구현 가능)
    public int jumpCount = 1;
    //점프 높이값을 지정
    public float jumpPower = 2f;
    //점프 시 증가할 높이값
    public float jumpHeight = 1f;
    //점프 시 도달할 높이 값
    float jumpPos = 0f;

    CharacterController characterController;
    Animator animator;

    // Use this for initialization
    void Start()
    {
        //오브젝트에 할당되어 있는 캐릭터 컨트롤러와 애니메이터를 받아온다
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터가 다음 프레임에 이동해야할 벡터를 저장한다
        Vector3 direction = Vector3.zero;

        //"Horiaontal" 버튼의 입력 값
        float hrzVal = 0f;
        //"Vertical" 버튼의 입력 값
        float vrtVal = 0f;

        //캐릭터 회전 관련 변수
        int turnHorz = 0;
        int turnVert = 0;

        //"Verticla"과 "Horizontal" 버튼의 입력 값이 따라 
        //direction 벡터에 이동할 방향 벡터를 더하고, 전후좌우에 따른 turn_XXX 값을 부여
        //turn_XXX값은 다음에 캐릭터의 회전방향에 사용된다
        if ((hrzVal = Input.GetAxis("Horizontal")) > 0)
        {
            turnHorz = 1;
            direction += new Vector3(hrzVal, 0, 0);
        }
        else if ((hrzVal = Input.GetAxis("Horizontal")) < 0)
        {
            turnHorz = -1;
            direction += new Vector3(hrzVal, 0, 0);
        }

        if ((vrtVal = Input.GetAxis("Vertical")) > 0)
        {
            turnVert = 1;
            direction += new Vector3(0, 0, vrtVal);
        }
        else if ((vrtVal = Input.GetAxis("Vertical")) < 0)
        {
            turnVert = -1;
            direction += new Vector3(0, 0, vrtVal);
        }

        // // 캐릭터 이동시 회전 제어 // //
        //캐릭터가 전진하면서 좌우로 이동하는 경우 45도 회전,
        //좌우로만 움직이는 경우에는 90도 회전을 적용
        if (turnVert > 0)
        {
            //전진 및 후진 방향에 따라 캐릭터 속도를 변화
            //turnVert가 0보다 큰 경우 전진 하는것으로 판단하고 속도 유지
            speedFixed = speed;

            if (turnHorz > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 45f, 0));
            else if (turnHorz < 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, -45f, 0));
            else
                transform.rotation = Quaternion.Euler(new Vector3(0, 0f, 0));
        }
        if (turnVert < 0)
        {
            //turnVert가 0보다 작은 경우 후진 하는것으로 판단하고 속도를 1/4로 감소
            speedFixed = speed / 4;

            if (turnHorz > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, -45f, 0));
            else if (turnHorz < 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 45f, 0));
            else
                transform.rotation = Quaternion.Euler(new Vector3(0, 0f, 0));
        }
        else if (turnVert == 0)
        {
            //turnVert가 0인 경우 좌우 직선 운동 하는 것으로 판단하고 속도 유지
            speedFixed = speed;

            if (turnHorz > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 90f, 0));
            else if (turnHorz < 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, -90f, 0));
        }

        //점프 버튼 입력 시 점프 횟수가 남아있는 경우에만 점프 실행
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            Jump();
            jumpCount--;
        }

        //설정된 방향 벡터 dir에 정해진 속도를 곱한 후 캐릭터 위치 변경 실행
        characterController.Move(direction * Time.deltaTime * speedFixed);

        //Set animation
        //캐릭터의 속력이 증가하면 달리는 애니메이션 실행
        animator.SetFloat("Run", characterController.velocity.magnitude);

        //점프 카운트가 0인 경우 점프 애니메이션 실행
        //만약 다중 점프를 구현한다면 Animator에서 조건 변경 필요
        animator.SetInteger("Jump", jumpCount);

        //만약 VrtVal 값이 음수인 경우, 뛰는 모션 대신 걷는 모션 실행
        animator.SetInteger("Walk", turnVert);
    }

    void Jump()
    {
        Vector3 dir = transform.position + transform.up * jumpHeight;

        //Lerp 함수를 활용하여 점프 시작 시에는 높이 올라가다가
        //올라갈수록 증가하는 높이를 감소시켰다.
        transform.position = Vector3.Lerp(transform.position, dir, jumpPower * Time.deltaTime);
    }

    //캐릭터가 땅에 닿는 경우 점프 카운트를 1로 증가
    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.gameObject.tag == "Terrain")
        {
            jumpCount = 1;
        }
    }
}
