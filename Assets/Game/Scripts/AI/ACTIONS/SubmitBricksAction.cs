using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitBricksAction : GoapAction
{
    private BrickStackHandler brickStackHandler;

    private bool submitingBricks=false;
    private bool bricksSubmitted = false;

    private SubmitBricksAction()
    {
        AddPrecondition(ConstStrings.ENOUGHBRICKSCOLLECTED,true);
        AddPrecondition(ConstStrings.SUBMITTINGBRICKS,false);
        AddEffect(ConstStrings.SUBMITBRICK, true);
    }


    private void Awake()
    {
        brickStackHandler = GetComponentInChildren<BrickStackHandler>();
        MinDistanceFromTarget = 1;
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
       // Debug.Log("procedural condition  "+ brickStackHandler.EnoughBricksAvailable());
        return brickStackHandler.EnoughBricksAvailable();
    }

    public override bool IsDone()
    {
        //Debug.Log("Done submitting bricks "+ (!submitingBricks && bricksSubmitted));
        return !submitingBricks && bricksSubmitted;
    }

    public override bool Perform(GameObject agent)
    {
        //Debug.LogError("perform state called " + submitingBricks + " " + bricksSubmitted);
       //  if (!submitingBricks)
        // {
            submitingBricks = true;
           // Debug.Log("submitting bricks in perform");
            brickStackHandler.SubmitBricks(OnSubmittedBricks);

       // }
       
        return true;
    }

    public override bool RequiresInRange()
    {
        target = Map.Instance.GerBrickSubmissionPoint(brickStackHandler.BrickType);
        return true;
    }

    public override void Reset()
    {
        submitingBricks = false;
        bricksSubmitted = false;
       // Debug.LogError("reset function called "+ submitingBricks+" "+ bricksSubmitted);
    }


    public void OnSubmittedBricks()
    {
      //  Debug.Log("Bricks submitted by AI");
        submitingBricks = false;
        bricksSubmitted = true;
        brickStackHandler.StopSubmittingBricks();
    }
}
