using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrWheels : MonoBehaviour
{
    //차체 중심에 위치한 Rigidbody를 가져온다.
    public Rigidbody rb;

    [Header("Suspension")]
    public float restLength; //스프링의 기본상태에서의 길이
    public float springTravel; //스프링의 수축,팽창 범위
    public float springStiffness; //스프링의 강성
    public float damperStiffness; // 댐퍼의 강성

    private float minLength; //스프링의 최대길이
    private float maxLength; //스프링의 최소길이
    private float lastLength; //이전 프레임 업데이트에서의 스프링의 길이
    private float springLength; //현재 프레임에서의 스프링 길이
    private float springVelocity; //스프링의 속력
    private float springForce; //스프링의 힘
    private float damperForce; //댐퍼의 힘


    private Vector3 suspensionForce;

    [Header("Wheel")]
    public float wheelRadius;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    //물리학이기 때문에 FixedUpdate
    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            lastLength = springLength; //이전 프레임의 스프링 길이 저장.
            springLength = hit.distance - wheelRadius; //스프링의 길이 Raycast 길이에서 바퀴의 반지름을 뺀 부분
            springLength = Mathf.Clamp(springLength, minLength, maxLength); //스프링 길이를 최소 최대값을 통해 범위내에 있도록 한정하는것.
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            // 스프링의 속력, 이전길이에서 현재길이를 뺐을때
            // 음수 -> 스프링이 수축
            // 양수 -> 스프링이 이완
            // 이전-현재 값을 Time.fixedDeltaTime으로 나누어  값을 얻어주어 이전 프레임 대비 현재 스프링이 '얼마나' 수축되고 이완되었는지 값을 저장한다.
            springForce = -springStiffness * (springLength - restLength);
            //Fspring =−k*x
            //스프링의 힘을 구하는 공식이다. k는 강성을 나타내는 상수로 springStiffness이다.
            //x는 스프링의 기본 길이(restLength)에서 얼마만큼 수축 / 이완했는지에 대한 길이이므로
            //Current Spring Length 에서 RestLength 를 빼면 구할 수 있다.
            damperForce = damperStiffness * springVelocity;
            //댐퍼는 서스펜션 내 스프링이 수축과 이완을 반복하는 과정에서 스프링 운동을 '완화'하는 역할을 한다.
            //댐퍼가 없다면 차체가 계속해서 흔들리기만 하기 때문에 댐퍼를 통해 해당 스프링의 반동을 감쇠시켜주어야한다.
            //댐퍼의 크기가 클수록 크게 감쇠하여 스프링의 운동이 적어지기 때문에 딱딱하고 너무 작으면 출렁인다.
            suspensionForce = (springForce + damperForce) * transform.up;

            rb.AddForceAtPosition(suspensionForce, hit.point);
        }
    }

}
