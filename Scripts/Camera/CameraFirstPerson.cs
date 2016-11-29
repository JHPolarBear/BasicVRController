using UnityEngine;
using System.Collections;

public class CameraFirstPerson : MonoBehaviour {

    //player to chase
    public GameObject Player;

    Vector3 offset;

    //Value for rotation
    float yaw;
    float pitch;

    void Start()
    {
        //offset between player character and main camera
        offset = transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CameraRotation();
    }

    //Update Camer position
    void LateUpdate()
    {
        //항상 캐릭터 눈 위치에 존재하도록 재지정
        transform.position = Player.transform.position + offset;
    }

    //Google VR SDK는 디바이스(머리)의 움직임을 Mouse X 및 Mouse Y에 적용함 (Unity 기준)
    void CameraRotation()
    {
        //Mouse X의 매 프레임 변화량을 받아서 기존의 이동량에 더해서 x축 회전에 활용
        //즉 -45~45도를 벗어나는 값이 들어올 경우, 최대,최소값을 -45, 45로 고정
        yaw = Mathf.Clamp(yaw + Input.GetAxis("Mouse X"), -55f, 55f);

        //아래 위로
        //Mouse Y의 매 프레임 변화량을 받아서 기존에 이동량에 더해 y축 회전에 활용
        pitch = Mathf.Clamp(pitch + Input.GetAxis("Mouse Y"), -45f, 45f);


        //변화량을 Quaternion으로 변환하여 카메라 회전 값에 적용
        transform.localRotation = Quaternion.Euler(new Vector3(-pitch, yaw, 0f));
    }
}
