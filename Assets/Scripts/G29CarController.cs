using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G29CarController : MonoBehaviour
{
    private WheelCollider[] wheelColliders;
    private GameObject[] wheels;
    public GameObject SpeedPoint;
    public GameObject RpmPoint;
    private Rigidbody rb;
    private Vector3 m_LastPosition;
    public float m_Speed;

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
    public float Rpm;
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
        SteerVehicle();
        WheelPosWithCollider();
        InputLogitech();

        for (int i = 0; i < wheels.Length; i++)
        {
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

        m_Speed = GetSpeed();
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
    }
    void Update()
    {
        SpeedPoint.transform.localRotation = Quaternion.Euler(0, 0, 45 + -m_Speed);
        RpmPoint.transform.localRotation = Quaternion.Euler(0, 0, 45 + -Rpm);
    }
    void WheelPosWithCollider()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation); //wheelCollider의  월드 포지션과 월드 로테이션 값을 받아옴
            wheels[i].transform.position = wheelPosition;
            wheels[i].transform.rotation = wheelRotation;
        }

        Rpm = wheelColliders[3].rpm * 350/1000;
    }

    // 애커만 조향
    void SteerVehicle()
    {
        if (ro > 0)
        {
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
        }
        else if (ro < 0)
        {
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * ro;
        }
        else
        {
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

    private float GetSpeed()
    {
        float speed = (((transform.position - m_LastPosition).magnitude) / Time.deltaTime);
        m_LastPosition = transform.position;

        
        return speed * 10;
    }
}