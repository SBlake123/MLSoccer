using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    public enum Team
    {
        BLUE = 0, RED
    }

    public Team team;

    private BehaviorParameters bps;
    private Rigidbody rb;

    // 이동속도, 킥파워
    public float moveSpeed = 1.0f;
    public float kickForce = 800.0f;

    //플레이어의 초기 위치
    public Vector3 initPosBlue = new Vector3(-5.5f, 0.5f, 0.0f);
    public Vector3 initPosRed = new Vector3(+5.5f, 0.5f, 0.0f);

    //플레이어의 초기 회전값
    public Quaternion initRotBlue = Quaternion.Euler(Vector3.up * 90);
    public Quaternion initRotRed = Quaternion.Euler(Vector3.up * -90);

    //플레이어의 색상 변경할 머티리얼
    public Material[] materials;

    public override void Initialize()
    {
        bps = GetComponent<BehaviorParameters>();
        bps.TeamId = (int)team;

        rb = GetComponent<Rigidbody>();
        rb.mass = 10.0f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.constraints = RigidbodyConstraints.FreezePositionY
                        | RigidbodyConstraints.FreezeRotationX
                        | RigidbodyConstraints.FreezeRotationZ;

        GetComponent<Renderer>().material = materials[(int)team];
        MaxStep = 10000;
    }

    public override void OnEpisodeBegin()
    {
        // 플레이어의 초기화
        InitPlayer();

        // 물리엔진 초기화
        rb.velocity = rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        //print($"[0]={action[0]}, [1]={action[1]}, [2]={action[2]}");

        // 이동벡터
        Vector3 dir = Vector3.zero;
        // 회전벡터
        Vector3 rot = Vector3.zero;

        int forward = action[0];
        int right = action[1];
        int rotate = action[2];

        switch (forward)
        {
            case 1: dir = transform.forward; break;
            case 2: dir = -transform.forward; break;
        }

        switch (right)
        {
            case 1: dir = -transform.right; break; //왼쪽
            case 2: dir = transform.right; break;  //오른쪽
        }

        switch (rotate)
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }

        transform.Rotate(rot, Time.fixedDeltaTime * 100.0f);
        rb.AddForce(dir * moveSpeed, ForceMode.VelocityChange);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 이산값으로 파라메터 정의
        var actions = actionsOut.DiscreteActions;
        // 파라메터 초기화
        actions.Clear();

        // Branch[0]
        // Branch Size = 3
        // 정지, 전진, 후진 (0, 1, 2) 이동 => 키보드 Non, W, S
        if (Input.GetKey(KeyCode.W)) actions[0] = 1;
        if (Input.GetKey(KeyCode.S)) actions[0] = 2;

        // Branch[1]
        // 정지, 좌, 우 (0, 1, 2) 이동 => 키보드 Non, Q, E
        if (Input.GetKey(KeyCode.Q)) actions[1] = 1;
        if (Input.GetKey(KeyCode.E)) actions[1] = 2;

        // Branch[2]
        // 정지, 좌, 우 (0, 1, 2) 회전 => 키보드 Non, A, D
        if (Input.GetKey(KeyCode.A)) actions[2] = 1;
        if (Input.GetKey(KeyCode.D)) actions[2] = 2;

    }

    public void InitPlayer()
    {
        transform.localPosition = (team == Team.BLUE) ? initPosBlue : initPosRed;
        transform.localRotation = (team == Team.BLUE) ? initRotBlue : initRotRed;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("ball"))
        {
            // 볼 터치시 + 리워드
            AddReward(0.2f);
            // 볼과 에이전트의 충돌지점(ContactPoint)를 활용해 방향벡터를 계산
            Vector3 shootDir = coll.GetContact(0).point - transform.position;
            coll.gameObject.GetComponent<Rigidbody>().AddForce(shootDir.normalized * kickForce);
        }
    }
}
