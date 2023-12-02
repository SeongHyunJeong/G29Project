using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FunctionTest : MonoBehaviour
{
    public int Score = 100; // 점수
    public int deadScore = 70; // 70점 이하 탈락
    public G29CarController controller;
    public float elapsedTime;  // 경과 시간

    void Start()
    {
        elapsedTime = 0.0f;
    }

    void Update()
    {
        //if (controller.left)
        //{
        //    print("좌측 방향지시등");
        //}
        //if (controller.right)
        //{
        //    print("우측 방향지시등");
        //}
        //if (controller.Night)
        //{
        //    print("라이트");
        //}
        //if (controller.Head)
        //{
        //    print("상향등");
        //}
    }
}
