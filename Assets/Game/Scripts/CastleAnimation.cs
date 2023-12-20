using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleAnimation : MonoBehaviour
{
    [SerializeField] private int bricksCollected = 0;


    private Animator animator;
    [SerializeField] private int set1 = Animator.StringToHash("Set 1");
    [SerializeField] private int set2 = Animator.StringToHash("Set 2");
    [SerializeField] private int set3 = Animator.StringToHash("Set 3");
    [SerializeField] private int set4 = Animator.StringToHash("Set 4");


    private int CastleCompletePercent25 = 0;
    private int CastleCompletePercent50 = 0;
    private int CastleCompletePercent75 = 0;
    private int CastleCompletePercent100 = 0;

    private bool set1Played = false;
    private bool set2Played = false;
    private bool set3Played = false;
    private bool set4Played = false;

    private MeshRenderer[] meshRenderers;
    private void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

    }

    private void Start()
    {
        CastleCompletePercent25 = CalculatePercent(25, 100);
        CastleCompletePercent50 = CalculatePercent(50, 100);
        CastleCompletePercent75 = CalculatePercent(75, 100);
        CastleCompletePercent100 = 100;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
      //  SetmeshRenderers(false);

       
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SubmitBrick(5);
        }
    }
    public void SubmitBrick(int value)
    {
        bricksCollected += value;


        if (bricksCollected >= CastleCompletePercent25 )
        {
            if (!set1Played)
            {
                set1Played = true;
                
                animator.SetTrigger(set1);

            }
        }
         if (bricksCollected >= CastleCompletePercent50 && bricksCollected < CastleCompletePercent75)
        {
            if (!set2Played)
            {
                set2Played = true;
                animator.ResetTrigger(set1);
                animator.SetTrigger(set2);

            }
            
        }
         if (bricksCollected >= CastleCompletePercent75 && bricksCollected < CastleCompletePercent100)
        {
            if (!set3Played)
            {
                set3Played = true;
                animator.ResetTrigger(set2);
                animator.SetTrigger(set3);

            }
        }
         if (bricksCollected >= 100)
        {
            if (!set4Played)
            {
                set4Played = true;
                animator.ResetTrigger(set3);
                animator.SetTrigger(set4);

            }

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
}
