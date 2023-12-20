using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BrickCollector))]
public class FindBrickAction : GoapAction
{

    [SerializeField] private LayerMask brickLayerMask;



    private bool foundBrick = false;

    private BrickCollector brickCollector;
    private BrickStackHandler brickStackHandler;

    private const int MAXBRICKSEARCHRADIUS = 300;
    private void Awake()
    {
        brickCollector = GetComponent<BrickCollector>();
        brickStackHandler = GetComponentInChildren<BrickStackHandler>();
    }

    public FindBrickAction()
    {
        AddPrecondition(ConstStrings.BRICKINRANGE,false);

        
        AddEffect(ConstStrings.BRICKINRANGE,true);

    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        foundBrick = false;
        return true;
    }

    public override bool IsDone()
    {
        //Debug.Log("Found Brick");
       return foundBrick;
    }

    public override bool Perform(GameObject agent)
    {
        

        for (int brickSearchRadius = 30; brickSearchRadius < MAXBRICKSEARCHRADIUS; brickSearchRadius+=30)
        {
            Collider[] bricks = Physics.OverlapSphere(transform.position,brickSearchRadius,brickLayerMask);
            if (bricks.Length>0)
            {
                //Debug.Log("found Bricks"+bricks.Length);
                for (int i = 0; i < bricks.Length; i++)
                {
                    Brick b = bricks[i].GetComponent<Brick>();
                    if (b.BrickType== brickStackHandler.BrickType && b.IsOnground)
                    {
                        brickCollector.TargetBrick = b.gameObject;
                        foundBrick = true;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        foundBrick = false;
    }


   
}
