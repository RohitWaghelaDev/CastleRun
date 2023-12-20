using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MinMax
{
    public int min;
    public int max;

}

public class Map : MonoBehaviour
{
    public static Map Instance;

    public Transform bluePlayerSP, redPlayerSP, greenPlayerSP, yellowPlayerSP;

    public int CastleBrickCount = 100;
    public List<Castle> castles = new List<Castle>();

    [SerializeField] private int minBrickRequredForSubmissionPercentage;
    [SerializeField]private MinMax xRange;
    [SerializeField]private MinMax zRange;
    [SerializeField] private Transform[] fleeTransforms;

    public MinMax XRange { get => xRange; set => xRange = value; }
    public MinMax ZRange { get => zRange; set => zRange = value; }

    public int minBrickRequredForSubmission;
    [HideInInspector] public int oneBrickValue = 1;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = GetComponent<Map>();
        }
        minBrickRequredForSubmission = (CastleBrickCount * minBrickRequredForSubmissionPercentage )/ 100;
        Debug.Log("Min brick required for submission "+minBrickRequredForSubmission);
    }

    public GameObject GerBrickSubmissionPoint(BrickType brickType)
    {
       GameObject submissionPoint = null;
        switch (brickType)
        {
            case BrickType.BlueBrick:
                submissionPoint = bluePlayerSP.gameObject;
                break;
            case BrickType.RedBrick:
                submissionPoint = redPlayerSP.gameObject;
                break;
            case BrickType.GreenBrick:
                submissionPoint = greenPlayerSP.gameObject;
                break;
            case BrickType.YellowBrick:
                submissionPoint = yellowPlayerSP.gameObject;
                break;
            case BrickType.GreyBrick:
                break;
            default:
                break;
        }

        return submissionPoint;
    }


    public GameObject GetFleePosition()
    {

        int i = Random.Range(0,fleeTransforms.Length);
        return fleeTransforms[i].gameObject;
    }

    public Vector3 GetSpawnPosition(BrickType brickType)
    {
        Vector3 spawnPosition = Vector3.zero;
        switch (brickType)
        {
            case BrickType.BlueBrick:
                spawnPosition = bluePlayerSP.position;
                break;
            case BrickType.RedBrick:
                spawnPosition = redPlayerSP.position;
                break;
            case BrickType.GreenBrick:
                spawnPosition = greenPlayerSP.position;
                break;
            case BrickType.YellowBrick:
                spawnPosition = yellowPlayerSP.position;
                break;
            case BrickType.GreyBrick:
                break;
            default:
                break;
        }

        return spawnPosition;
    }


    public void SubmitBricks(BrickType inBrickType,int value)
    {
        foreach (var castle in castles)
        {
            if (castle.brickType == inBrickType)
            {
                castle.SubmitBrick(value);
            }
        }
    }

    public Vector3 GetCastlePosition(BrickType inBrickType)
    {
        Vector3 castlePosition = new Vector3(0,0,0);
        foreach (var castle in castles)
        {
            if (castle.brickType == inBrickType)
            {
                castlePosition= castle.gameObject.transform.position;
            }
        }
        return castlePosition;
    }
}
