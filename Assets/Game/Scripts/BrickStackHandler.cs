using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public delegate void OnSubmittedBricks();


public class BrickStackHandler : MonoBehaviour
{
    [SerializeField] private float brickDisableDelay = 1f;
    

    private GameObject localPuffParticleSystem;
    private int layerID;
    private BrickType brickType;

    public float BrickYPos { get; set; }
    public int TotalCollectedBricks;// { get; private set; }
    public int BrickStackCount { get; set; }
    public bool submittingBricks { get; private set; }
    private BrickStackHandler brickStackHandler;

    public List<GameObject>bricksInStack =new List<GameObject>();

    public BrickType BrickType { get => brickType; private set => brickType = value; }

    private OnSubmittedBricks OnSubmittedBricks;

    private IEnumerator submitBricksCR;

    private bool canCollectBricks = true;
    private int brickCollectionwaittime = 3;

    private IPlayer player;

    private void Awake()
    {
        brickStackHandler = GetComponent<BrickStackHandler>();
    }

    void Start()
    {
        layerID = LayerMask.NameToLayer("Brick");
        BrickYPos = transform.position.y;

        int brickStackLayer = LayerMask.NameToLayer("BrickStack");
        gameObject.layer = brickStackLayer;

        brickStackHandler = GetComponent<BrickStackHandler>();

        TotalCollectedBricks = 0;
        BrickStackCount = 0;
        submitBricksCR = SubmitBricksCoroutine();
        player = GetComponentInParent<IPlayer>();
        brickType = player.BrickType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer==layerID && canCollectBricks)
        {
            Brick b = other.gameObject.GetComponent<Brick>();
            if (b!=null && b.IsOnground && (b.BrickType==brickType || b.BrickType==BrickType.GreyBrick))
            {
                //b.transform.SetParent(transform);
                b.SetupBrick(brickType);
                b.MoveBrick(brickStackHandler);
                bricksInStack.Add(b.gameObject);
            }
        }
    }
   
    public Vector3 GetBrickPosition()
    {
        return new Vector3(transform.position.x,BrickYPos,transform.position.z);
        
    }
    public float GetBrickYPosition()
    {
        return  BrickYPos;

    }
    public bool EnoughBricksAvailable()
    {
       /* if (BrickStackCount >= Map.Instance.minBrickRequredForSubmission || BrickStackCount >= (Map.Instance.CastleBrickCount - TotalCollectedBricks))
        {
            Debug.Log("Enough brick available " + BrickStackCount.ToString() + " "+TotalCollectedBricks+" " + Map.Instance.minBrickRequredForSubmission + " " + Map.Instance.CastleBrickCount);
            Debug.Log((Map.Instance.CastleBrickCount - TotalCollectedBricks).ToString());
        }*/
        return BrickStackCount >= Map.Instance.minBrickRequredForSubmission || BrickStackCount >= (Map.Instance.CastleBrickCount - TotalCollectedBricks);
    }




    public void SubmitBricks(OnSubmittedBricks onSubmittedBricks)
    {
        if (submittingBricks || bricksInStack.Count<=0)
        {
            onSubmittedBricks();
            Debug.Log("already submitted bricks ");
            return;
        }
        
        OnSubmittedBricks = onSubmittedBricks;
        submitBricksCR = SubmitBricksCoroutine();
        StartCoroutine(submitBricksCR);
     

    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnHitWithEnemy();
        }
    }
    public void OnHitWithEnemy()
    {
        Debug.Log("****** called on hit on  "+gameObject.transform.parent.name);

       
        if (bricksInStack.Count<=0)
        {
            return;
        }
        GetComponent<Collider>().enabled = false;
        canCollectBricks = false;
        player.ToFallState();
        
        for (int i = bricksInStack.Count-1; i >= 0; i--)
        {
            if (bricksInStack[i].GetComponent<Brick>().BrickType!=brickType)
            {
                Debug.Log("##### not same brick type ");
                continue;
            }
            //  Debug.Log(bricksInStack.Count+"  "+i);
            GameObject brick = bricksInStack[i];
            brick.transform.parent = null;
            Brick b = brick.GetComponent<Brick>();
            b.SetupBrick(BrickType.GreyBrick);
            b.Rigidbody.useGravity = true;
            b.Rigidbody.isKinematic = false;

            int maxForce=30;
            int minForce = 10;
            int x = Random.Range(0,2);
            int xForce = x==0? Random.Range(-maxForce, -maxForce) : Random.Range(minForce, maxForce);
            int yForce = Random.Range(minForce, maxForce);
            int z = Random.Range(0, 2);
            int zForce = z == 0 ? Random.Range(-minForce, -maxForce) : Random.Range(minForce, maxForce);
            Vector3 force = new(xForce, yForce, zForce);
            b.Rigidbody.AddForce(force, ForceMode.Impulse);
            bricksInStack.RemoveAt(i);


            //Debug.Log(force);
        }
        bricksInStack = new List<GameObject>();
        BrickStackCount = 0;
        BrickYPos = transform.position.y;
        
        StartCoroutine(SetupBrickCollectionStatus());
    }
  
    IEnumerator SubmitBricksCoroutine()
    {
        submittingBricks = true;
        //transform.GetChild(0).gameObject.SetActive(true);
        for (int i = bricksInStack.Count - 1; i >= 0; i--)
        {
            GameObject brick = bricksInStack[i];
            //brick.GetComponent<TrailRenderer>().enabled = true;
            brick.gameObject.transform.parent = null;
            Vector3 targetPosition = Map.Instance.GetCastlePosition(brickType);
            brick.transform.DOMove(targetPosition, 1f).OnComplete(()=> brick.SetActive(false));
            Map.Instance.SubmitBricks(brickType,Map.Instance.oneBrickValue);
            TotalCollectedBricks++;
            BrickStackCount--;
            BrickYPos -= bricksInStack[i].transform.localScale.y;
            bricksInStack.RemoveAt(i);

            yield return brickDisableDelay;
        }
       
        ClearBrickStack();
        submittingBricks = false;
        OnSubmittedBricks?.Invoke();
       // transform.GetChild(0).gameObject.SetActive(false);



        if (TotalCollectedBricks>= Map.Instance.CastleBrickCount)
        {
            GoapAgent goapAgent = GetComponentInParent<GoapAgent>();
            if (goapAgent!=null)
            {
                GetComponentInParent<GoapAgent>().enabled = false;

            }
        }

        
    }


    public void ClearBrickStack()
    {
        for (int i = bricksInStack.Count - 1; i >= 0; i--)
        {

            if (!bricksInStack[i].activeInHierarchy)
            {
                bricksInStack.RemoveAt(i);

            }

        }
    }
    public void StopSubmittingBricks()
    {
       // transform.GetChild(0).gameObject.SetActive(false);
        submittingBricks = false;
        if (submitBricksCR!=null)
        {
            StopCoroutine(submitBricksCR);

        }
        ClearBrickStack();
        submitBricksCR = null;
    }
    IEnumerator SetupBrickCollectionStatus()
    {
        yield return new WaitForSeconds(brickCollectionwaittime);
       
        player.ToIdleState();
        yield return new WaitForSeconds(1);
        canCollectBricks = true;
        GetComponent<Collider>().enabled = true;
    }
}
