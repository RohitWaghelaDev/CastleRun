using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{

    public delegate void CastleComplete(BrickType brickType);
    public static event CastleComplete OnCastleComplete;


    public BrickType brickType;

    [SerializeField]private int bricksCollected = 0;
    [SerializeField] private ParticleSystem confetti;

    private Animator animator;
    [SerializeField] private int set1 = Animator.StringToHash("Set 1");
    [SerializeField] private int set2 = Animator.StringToHash("Set 2");
    [SerializeField] private int set3 = Animator.StringToHash("Set 3");
    [SerializeField] private int set4 = Animator.StringToHash("Set 4");


    private int CastleCompletePercent25=0;
    private int CastleCompletePercent50 = 0;
    private int CastleCompletePercent75 = 0;
    private int CastleCompletePercent100 = 0;

    private MeshRenderer[] meshRenderers;
    private void OnEnable()
    {
        if (animator==null)
        {
            animator = GetComponent<Animator>();
        }

    }

    private void Start()
    {
        CastleCompletePercent25 = CalculatePercent(25, Map.Instance.CastleBrickCount);
        CastleCompletePercent50 = CalculatePercent(50, Map.Instance.CastleBrickCount);
        CastleCompletePercent75 = CalculatePercent(75, Map.Instance.CastleBrickCount);
        CastleCompletePercent100 = Map.Instance.CastleBrickCount;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
       // SetmeshRenderers(false);

        ViewController.Instance.brickAmountFillerImage.fillAmount = 0;
    }
    public void SubmitBrick(int value)
    {
        bricksCollected+=value;
        if (brickType==BrickType.BlueBrick)
        {
            ViewController.Instance.brickAmountFillerImage.fillAmount = (float)bricksCollected / (float)Map.Instance.CastleBrickCount;

        }

        if (bricksCollected>= CastleCompletePercent25 && bricksCollected < CastleCompletePercent50)
        {
            //SetmeshRenderers(true);
            animator.SetTrigger(set1);
        }
        else if (bricksCollected >= CastleCompletePercent50 && bricksCollected < CastleCompletePercent75)
        {
            animator.SetTrigger(set2);
        }
        else if (bricksCollected >= CastleCompletePercent75 && bricksCollected < CastleCompletePercent100)
        {
            animator.SetTrigger(set3);
        }
        else if(bricksCollected >= Map.Instance.CastleBrickCount)
        {
            animator.SetTrigger(set4);
            Debug.Log("7777 "+bricksCollected+" "+Map.Instance.CastleBrickCount);
            OnCastleComplete(brickType);
            Invoke(nameof(EnableConfetti),2);
        }
        
    }

    private int CalculatePercent(int percent, int amount)
    {
        return (int)((amount * percent) / 100);
    }

    private void SetmeshRenderers(bool status)
    {

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].enabled = status;
        }
    }

    private void EnableConfetti()
    {
        confetti.gameObject.SetActive(true);
        confetti.Play();
    }
}
