using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G29CarController : MonoBehaviour
{
    private WheelCollider[] wheelColliders;
    private GameObject[] wheels;
    private Rigidbody rb;

    [Header("Padel")]
    public float power; //속력
    private float accPower; //엑셀범위
    private int intAccP;
    private int isAcc;

    public float breakPower; //브레이크 범위값
    private float wheelBase; //앞 뒤 바퀴 사이의 거리(m단위)
    private float rearTrack; //좌 우 바튀 사이의 거리(m단위)
    public float turnRadius; //회전 반지름(m단위)

    [Header("Handle")]
    private float ro; //바퀴 회전값
    private float isro;

    public bool isBreak;
    // Start is called before the first frame update
    void Start()
    {
        Init();
        breakPower = 0;
        isBreak = false;
    }

    private void FixedUpdate()
    {
        SteerVehicle(); //애커만 조향
        WheelPosWithCollider();  //바퀴의 위치와 로테이션 값을 항상 같게 만들어줌
        InputLogitech();
        //브레이크 기능(자동차 브레이크시 4개의 바퀴에 모두 적용되어야함)
        for (int i = 0; i < wheels.Length; i++)
        {
            // for문을 통해서 휠콜라이더 전체를 엑셀 입력에 따라서 power만큼의 힘으로 움직이게 한다.
            wheelColliders[i].motorTorque = isAcc * power;
        }

        if (breakPower > 32767)
        {
            isBreak = true;
        }
        else
        {
            isBreak = false;
        }

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.brakeTorque = 3 * breakPower;
        }
        print(ro);
    }

    void InputLogitech()
    {
        LogitechGSDK.DIJOYSTATE2ENGINES rec;
        rec = LogitechGSDK.LogiGetStateUnity(0);
        ro = rec.lX / 32767f;

        accPower = Mathf.Abs(rec.lY - 32767);
        intAccP = (int)accPower / 10000;
        if (rec.lY != 0)
        {
            for (int i = 0; i < 128; i++)
            {
                if (rec.rgbButtons[i] == 128)
                {
                    if (i == 14) isAcc = 1;
                    else if (i == 15) isAcc = -1;
                }

            }
        }
        if (rec.lRz != 0)
        {
            breakPower = Mathf.Abs(rec.lRz - 32767);
        }
        //Button status :

    }

    //바퀴 mesh와 collider의 위치를 맞춰줌
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

    // 애커만 조향
    void SteerVehicle()
    {
        //steerAngle = 바퀴의 조향 각도

        //공식 Rad2Deg * Atan(wheelBase in meter / (turnRadius in meters + (rearTrack in meters / 2to get center)) * steerInput  left
        // Rad2Deg* Atan(wheelBase in meter / (turnRadius in meters - (rearTrack in meters / 2to get center)) *steerInput  right
        //transform.rotation = Quaternion.Euler(0, ro, 0); 
        //바퀴 오른쪽으로 회전
        if (ro > 0)
        {
            //Debug.Log("우회전");
            //Rad2Deg -> 각도를 라디안에서 degree(도)로 변환
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
        }
        //바퀴 왼쪽으로 회전
        else if (ro < 0)
        {
            //Debug.Log("좌회전");
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
            //Debug.Log(wheelColliders[0].steerAngle);
        }
        else
        {
            //Debug.Log("직진");
            wheelColliders[0].steerAngle = 0;
            wheelColliders[1].steerAngle = 0;
        }
        switch (intAccP)
        {
            case 0:
                power = 0;
                break;
            case 1:
                power = 100;
                break;
            case 2:
                power = 150;
                break;
            case 3:
                power = 200;
                break;
            case 4:
                power = 250;
                break;
            case 5:
                power = 300;
                break;
            case 6:
                power = 350;
                break;
        }
    }

    //초기화
    private void Init()
    {
        wheelColliders = new WheelCollider[4];
        wheels = new GameObject[4];
        rb = GetComponentInParent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -1, 0); //무게중심을 y축 방향으로 낮춤
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

        turnRadius = 3;
        wheelBase = wheelColliders[1].transform.position.z - wheelColliders[3].transform.position.z;
        rearTrack = wheelColliders[2].transform.position.x - wheelColliders[3].transform.position.x;
    }
}
