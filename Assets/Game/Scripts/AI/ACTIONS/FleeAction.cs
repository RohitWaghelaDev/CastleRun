using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeAction : GoapAction
{
    private float fleeDuration;
    private float startTime;

    private BrickCollector brickCollector;


    private void Start()
    {
        brickCollector = GetComponent<BrickCollector>();
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = Map.Instance.GetFleePosition();
        return target != null;
    }

    public override bool IsDone()
    {
        return Time.time - startTime > fleeDuration;
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > fleeDuration)
        {
            return false;
        }
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        startTime = 0.0f;
    }
}
