using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackOpponentAction : GoapAction
{
    private float chaseDuration;
    private float startTime;

    private BrickCollector brickCollector;
    

    private void Start()
    {
        brickCollector = GetComponent<BrickCollector>();
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        target = brickCollector.GetAttackTarget();
        return target != null;
    }

    public override bool IsDone()
    {
        return Time.time - startTime > chaseDuration;
    }

    public override bool Perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > chaseDuration)
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
