using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BallAgent : Agent
{
    private Rigidbody ballRigid;            // 볼 리지드바디
    public Transform pivotTransform;    // 훈련 유닛의 기준 위치
    public Transform target;                // 목표의 위치

    public float moveForce = 10f;        // 이동시키는 힘
    private bool targetEaten = false;    // 타겟을 잡았는지 여부
    private bool dead = false;             // 플로어에서 벗어났는지 여부

    private void Awake()
    {
        ballRigid = GetComponent<Rigidbody>();
    }
    
    // 에이전트가 타겟을 잡았을 경우 타겟을 리셋시키는 함수.
    void ResetTarget()
    {
        targetEaten = false;
        // x와 z가 -5 ~ 5 사이에 임의의 좌표를 생성한다.
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        // 타겟의 위치는 기준 위치에서 임의의 좌표를 더한 랜덤한 곳에 위치한다.
        target.position = randomPos + pivotTransform.position;
    }

    // 에이전트가 죽었다가 다시 시작할 때 리셋시키는 함수
    public override void AgentReset()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0.5f, Random.Range(-5f, 5f));
        transform.position = randomPos + pivotTransform.position;

        dead = false;                                     // 사망 상태 초기화
        ballRigid.velocity = Vector3.zero;          // 플레이어 속도 초기화

        ResetTarget();                                    // 타겟 초기화
    }

    // 에이전트가 수집하는 데이터 x, z 좌표에 대한 위치 및 속도
    public override void CollectObservations()
    {
        // 타겟과의 거리 계산
        Vector3 distanceToTarget = target.position - transform.position;

        // 정규화 된 값( -5~+5 -> -1~+1 압축하면 정규화 = 퍼포먼스가 올라감).
        AddVectorObs(Mathf.Clamp(distanceToTarget.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(distanceToTarget.z / 5f, -1f, 1f));

        // 훈련 유닛 기준 에이전트 위치
        Vector3 realtivePos = transform.position - pivotTransform.position;

        // 정규화된 값
        AddVectorObs(Mathf.Clamp(realtivePos.x / 5f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(realtivePos.z / 5f, -1f, 1f));

        // 에이전트의 속도를 정규화한 값
        AddVectorObs(Mathf.Clamp(ballRigid.velocity.x / 10f, -1f, 1f));
        AddVectorObs(Mathf.Clamp(ballRigid.velocity.z / 10f, -1f, 1f));
    }

    // 브레인이 에이전트에게 지시를 내리는 함수
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.001f);  // 아무런 액션을 하지 않는 것을 방지 하기 위한 벌점

        float horizontalInput = vectorAction[0];
        float verticalInput = vectorAction[1];

        ballRigid.AddForce(horizontalInput * moveForce, 0f, verticalInput * moveForce);

        if (targetEaten)
        {
            AddReward(1.0f);
            ResetTarget();
        }
        else if (dead)
        {
            AddReward(-1.0f);
            Done();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("dead"))
            dead = true;
        else if (other.CompareTag("goal"))
            targetEaten = true;
    }
}
