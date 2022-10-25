using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using TMPro;

public class BallCtrl : MonoBehaviour
{
    public Agent[] players;

    private Rigidbody rb;

    [SerializeField]
    private int blueScore;
    [SerializeField]
    private int redScore;
    public TextMeshProUGUI redScoreText;
    public TextMeshProUGUI blueScoreText;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void InitBall()
    {
        transform.localPosition = new Vector3(0, 2.0f, 0);
        rb.velocity = rb.angularVelocity = Vector3.zero;
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BLUE_GOAL"))
        {
            // RED +1, BLUE -1
            players[0].AddReward(-1.0f);
            players[1].AddReward(-1.0f);
            players[2].AddReward(-0.3f);
            players[3].AddReward(+1.0f);
            players[4].AddReward(+1.0f);
            players[5].AddReward(+0.3f);

            players[0].EndEpisode();
            players[1].EndEpisode();
            players[2].EndEpisode();
            players[3].EndEpisode();
            players[4].EndEpisode();
            players[5].EndEpisode();


            InitBall();

            ++redScore;
            redScoreText.text = redScore.ToString();
        }

        if (coll.collider.CompareTag("RED_GOAL"))
        {
            // RED -1, BLUE +1
            players[0].AddReward(+1.0f);
            players[1].AddReward(+1.0f);
            players[2].AddReward(+0.3f);
            players[3].AddReward(-1.0f);
            players[4].AddReward(-1.0f);
            players[5].AddReward(-0.3f);

            players[0].EndEpisode();
            players[1].EndEpisode();
            players[2].EndEpisode();
            players[3].EndEpisode();
            players[4].EndEpisode();
            players[5].EndEpisode();


            InitBall();

            ++blueScore;
            blueScoreText.text = blueScore.ToString();
        }
    }
}
