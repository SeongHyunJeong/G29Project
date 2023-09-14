using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    private WheelCollider[] wheelColliders;
    private GameObject[] wheels;
    private Rigidbody rb;

    public float radius = 1f;
    public float ro = 0; // 핸들 회전을 따라서 바퀴 움직임을 조정하는 변수
    public int speed = 0;
    public float power;
    public float rot;
    public bool isBreak;
    private float wheelBase; //앞 뒤 바퀴 사이의 거리(m단위)
    private float rearTrack; //좌 우 바튀 사이의 거리(m단위)
    public float turnRadius; //회저 반지럼(m단위)
    // Start is called before the first frame update
    void Start()
    {
        wheelColliders = new WheelCollider[4];
        wheels = new GameObject[4];
        rb = GetComponentInParent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1, 0); //무게중심을 y축 방향으로 낮춤
        power = 100f;
        rot = 45f;

        wheels[0] = GameObject.FindGameObjectWithTag("FRWheel");
        wheels[1] = GameObject.FindGameObjectWithTag("FLWheel");
        wheels[2] = GameObject.FindGameObjectWithTag("RRWheel");
        wheels[3] = GameObject.FindGameObjectWithTag("RLWheel");
        wheelColliders[0] = GameObject.Find("WheelHubFrontRight").GetComponent<WheelCollider>();
        wheelColliders[1] = GameObject.Find("WheelHubFrontLeft").GetComponent<WheelCollider>();
        wheelColliders[2] = GameObject.Find("WheelHubRearRight").GetComponent<WheelCollider>();
        wheelColliders[3] = GameObject.Find("WheelHubRearLeft").GetComponent<WheelCollider>();

        

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].transform.position = wheels[i].transform.position;
        }
        wheelBase = Mathf.Abs(wheelColliders[1].transform.position.z - wheelColliders[3].transform.position.z);
        rearTrack = Mathf.Abs(wheelColliders[2].transform.position.x - wheelColliders[3].transform.position.x);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            // for문을 통해서 휠콜라이더 전체를 Vertical 입력에 따라서 power만큼의 힘으로 움직이게한다.
            wheelColliders[i].motorTorque = Input.GetAxis("Vertical") * power;
        }

        SteerVehicle(); //애커만 조향
        WheelPosWithCollider();  //바퀴의 위치와 로테이션 값을 항상 같게 만들어줌

       
    }
    void Update()
    {
        //LogitechGSDK.DIJOYSTATE2ENGINES rec;
        //rec = LogitechGSDK.LogiGetStateUnity(0);
        //ro = rec.lX / 819;
        //transform.rotation = Quaternion.Euler(0, ro, 0);
        //Debug.Log(speed);
    }

    void WheelPosWithCollider()
    {
        //값을 받아오는 용도의 변수
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation); //wheelCollider의  월드 포지션과 월드 로테이션 값을 받아옴
            wheels[i].transform.position = wheelPosition;
            wheels[i].transform.rotation = wheelRotation;
        }
    }
    

    void SteerVehicle()
    {
        //steerAngle = 바퀴의 조향 각도
        // 애커만 조향
        //공식 Rad2Deg * Atan(wheelBase in meter / (turnRadius in meters + (rearTrack in meters / 2to get center)) * steerInput  left
        // Rad2Deg* Atan(wheelBase in meter / (turnRadius in meters - (rearTrack in meters / 2to get center)) *steerInput  right
        
        //바퀴 오른쪽으로 회전
        if (Input.GetAxis("Horizontal") > 0)
        {   // rear tracks size is set to 1.5f          wheel base has been set to 2.55f
            //Rad2Deg -> 각도를 라디안에서 degree(도)로 변환
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * Input.GetAxis("Horizontal");
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * Input.GetAxis("Horizontal");
            Debug.Log(wheelColliders[0].steerAngle);
        }
        //바퀴 왼쪽으로 회전
        else if (Input.GetAxis("Horizontal") < 0)
        {
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * Input.GetAxis("Horizontal");
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * Input.GetAxis("Horizontal");
        }
        else
        {
            wheelColliders[0].steerAngle = 0;
            wheelColliders[1].steerAngle = 0;
        }
    }
}
