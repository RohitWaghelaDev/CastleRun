using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public enum BrickType : int {BlueBrick,RedBrick,GreenBrick,YellowBrick, GreyBrick}

public class Brick : MonoBehaviour
{
    [SerializeField]
    private float speed = 10F;
    [SerializeField]
    private Color32 blueColor, redcolor, greenColor, yellowColor, greyColor;

    public BrickType BrickType;// { get; private set; }
    private Renderer brickRenderer;

    public bool IsPlacedOnStack;// { get; private set; }
    public bool IsOnground;// { get; private set; }
    public Rigidbody Rigidbody { get; set; }
    private float startTime;
    private float journeyLength;
    [SerializeField]private int groundLayerID;
    private BrickStackHandler brickStackHandler;
    private TrailRenderer trailRenderer;
    private Vector3 brickRotation = new Vector3(0,90,0);
    private Vector3 startingScale = new Vector3(0.4f,0.2f,0.6f);

    private void OnEnable()
    {
        Vector3 endScale = new(3f, 0.7f, 1.5f);
        transform.localScale = startingScale;
        transform.DOScale(endScale,0.1f);
        transform.rotation = Quaternion.identity;
        IsOnground = false;
        IsPlacedOnStack = false;
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        trailRenderer.enabled = true;
    }

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        brickRenderer = GetComponent<Renderer>();
        Rigidbody = GetComponent<Rigidbody>();
        groundLayerID = LayerMask.NameToLayer("Ground");
    }


    private void LateUpdate()
    {
        if (!IsPlacedOnStack && brickStackHandler!=null)
        {
            
            IsOnground = false;
            journeyLength = Vector3.Distance(transform.position, brickStackHandler.GetBrickPosition());
            if (journeyLength <= 10f)
            {
                PlaceBrick();
                return;
            }
            float distCovered = (Time.time - startTime) * speed;
            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / journeyLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(transform.position, brickStackHandler.GetBrickPosition(), fractionOfJourney);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("colliding with " + collision.gameObject.name);
        if (collision.gameObject.layer == groundLayerID)
        {
           // Debug.Log("***** collidong with ground");
            IsOnground = true;
            IsPlacedOnStack = false;
            Rigidbody.useGravity = false;
            Rigidbody.isKinematic = true;
            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
            }
            trailRenderer.enabled = true;
        }
    }

    public void SetupBrick(BrickType brickType)
    {
        IsOnground = true;
        switch (brickType)
        {
            case BrickType.BlueBrick:
                BrickType = BrickType.BlueBrick;
                SetupBrickColor(blueColor);
                break;
            case BrickType.RedBrick:
                BrickType = BrickType.RedBrick;
                SetupBrickColor(redcolor);
                break;
            case BrickType.GreenBrick:
                BrickType = BrickType.GreenBrick;
                SetupBrickColor(greenColor);
                break;
            case BrickType.YellowBrick:
                BrickType = BrickType.YellowBrick;
                SetupBrickColor(yellowColor);
                break;
            case BrickType.GreyBrick:
                BrickType = BrickType.GreyBrick;
                SetupBrickColor(greyColor);
                transform.SetParent(null);
                IsOnground = false;
                IsPlacedOnStack = false;
                
                break;
            default:
                break;
        }
    }

    private void SetupBrickColor(Color32 c)
    {
        if (brickRenderer == null)
        {
            brickRenderer = GetComponent<Renderer>();
        }
        if (trailRenderer==null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
        }
        //trailRenderer.startColor = c;
        brickRenderer.material.color = c;
    }

    public void MoveBrick(BrickStackHandler inBrickStackHandler)
    {
        BrickSpawner.Instance.EmptyOccupiedPosition(transform.position);
        brickStackHandler = inBrickStackHandler;
        /*if (BrickType==BrickType.GreyBrick)
        {
            SetupBrick(brickStackHandler.BrickType);
        }*/
        startTime = Time.time;
        IsPlacedOnStack = false;
        IsOnground = false;
    }
    private void PlaceBrick()
    {
        IsOnground = false;
        IsPlacedOnStack = true;
        transform.SetParent(brickStackHandler.transform);
       // transform.position = brickStackHandler.GetBrickPosition();
        float brickyPosition = brickStackHandler.GetBrickYPosition();
        Vector3 pos = new Vector3(0,brickyPosition,0);
        transform.localPosition = pos;
        transform.localScale=new Vector3(3,0.7f,1.5f);
        transform.localRotation = brickStackHandler.transform.localRotation; ;
        //transform.localRotation = Quaternion.identity ;

        brickStackHandler.BrickYPos += transform.localScale.y;
        //Debug.Log("topbrick "+ brickStackHandler.BrickYPos);
        brickStackHandler.BrickStackCount++;
        brickStackHandler = null;
        trailRenderer.enabled = false;
    }
   
   
}
