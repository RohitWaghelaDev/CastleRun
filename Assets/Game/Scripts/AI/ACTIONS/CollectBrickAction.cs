using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BrickCollector))]
public class CollectBrickAction : GoapAction
{
    private BrickCollector brickCollector;
    private BrickStackHandler brickStackHandler;
    


    private void Awake()
    {
        brickCollector = GetComponent<BrickCollector>();
        brickStackHandler = GetComponentInChildren<BrickStackHandler>();
        MinDistanceFromTarget = 12;
    }

    public CollectBrickAction()
    {

        AddPrecondition(ConstStrings.BRICKINRANGE,true);
        //AddPrecondition(ConstStrings.ENOUGHBRICKSCOLLECTED,false);

       AddEffect(ConstStrings.COLLECTBRICK,true);

    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        
        return true;
    }

    public override bool IsDone()
    {
        return true;
        //return brickStackHandler.BrickStackCount >= Map.Instance.minBrickRequredForSubmission ;
       
    }

    public override bool Perform(GameObject agent)
    {

        brickCollector.TargetBrick = null;
        target = null;
        return true;
    }

    public override bool RequiresInRange()
    {
        target = brickCollector.TargetBrick;
        return true;
    }

    public override void Reset()
    {
        brickCollector.TargetBrick = null;
        target = null; 
    }
}
