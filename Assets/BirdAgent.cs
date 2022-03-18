using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.Events;

public class BirdAgent : Agent
{
    private Rigidbody2D rBody;
    public float jumpForce = 90f;
    private bool isJumpInputDown;

    public UnityAction onEndEpisode;
    public UnityAction<EnvironmentParameters> onEpisodeBegin;

    EnvironmentParameters resetParams;

    public override void Initialize()
    {
        this.rBody = this.GetComponent<Rigidbody2D>();
        resetParams = Academy.Instance.EnvironmentParameters;
    }

    public override void OnEpisodeBegin()
    {
        //Debug.Log("[BirdAgent] OnEpisodeBegin");

        this.transform.position = this.transform.parent.position + Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        this.Jump();

        this.onEpisodeBegin(resetParams);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.position.y);
        sensor.AddObservation(this.rBody.velocity.y);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 1) {
            this.Jump();
        }

        if (this.transform.position.y > 50f || this.transform.position.y < -50f)
        {
            this.AddReward(-0.1f);
            onEndEpisode();
            EndEpisode();
        }

        this.AddReward(1f / this.MaxStep); //0.0002

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpInputDown = true;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        var discreteActionOut = actionsOut.DiscreteActions;
        discreteActionOut[0] = 0;
        if (isJumpInputDown)
        {
            discreteActionOut[0] = 1;
            this.isJumpInputDown = false;
        }
    }

    public void Jump()
    {
        this.rBody.velocity = Vector3.up * this.jumpForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Pipe"))
        {
            Debug.Log("-0.1");
            AddReward(-0.1f);
            onEndEpisode();
            EndEpisode();
        }
    }
}
