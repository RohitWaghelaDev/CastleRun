
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickCollector : MonoBehaviour, IGoap,IPlayer
{

	public float AgentMoveSpeed = 20;
	
	public BrickType brickType;

	private List<GameObject> enemiesInRange = new List<GameObject>();
	private int enemyLayerID;
	private BrickStackHandler brickStackHandler;

	public GameObject TargetBrick; //{ get; set;}

	private bool castleComplete=false;

	private bool inSafeZone;
	private int safeZoneLayerID;
	private int playerLayerID;
	public BrickStackHandler BrickStackHandler => brickStackHandler;
	BrickType IPlayer.BrickType => brickType;
	public bool CanMove { get; set; }
	public bool InFallState { get; set; }
	//BrickType IPlayer.BrickType => brickStackHandler.BrickType;

	private Animator animator;
	private int idleHash = Animator.StringToHash("Idle");
	private int runHash = Animator.StringToHash("Run");
	private int fallHash = Animator.StringToHash("Fall");
	private int movementSpeedHash = Animator.StringToHash("MovementSpeed");

	#region UnityFunctions

	private void OnEnable()
    {
        Castle.OnCastleComplete += OnCastleComplete;
		GameManager.OnGameStateChanged += OnGameStateChanged;
		GetComponent<GoapAgent>().enabled = false;


	}



	void Start()
	{
		if (animator == null)
		{
			animator = GetComponent<Animator>();
		}

		brickStackHandler = GetComponentInChildren<BrickStackHandler>();
		enemyLayerID = LayerMask.NameToLayer("Player");
		safeZoneLayerID = LayerMask.NameToLayer("SafeZone");
		playerLayerID = LayerMask.NameToLayer("Player");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == enemyLayerID)
		{
			enemiesInRange.Add(other.gameObject);
		}
		else if (other.gameObject.layer == safeZoneLayerID)
		{
			inSafeZone = true;
		}

		if (other.gameObject.layer == playerLayerID)
		{
			IPlayer enemyPlayer = other.gameObject.GetComponent<IPlayer>();
			if (enemyPlayer != null && !enemyPlayer.IsInSafeZone())
			{
				if (enemyPlayer.BrickStackHandler.BrickStackCount < brickStackHandler.BrickStackCount)
				{
					enemyPlayer.BrickStackHandler.OnHitWithEnemy();
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == enemyLayerID)
		{
			enemiesInRange.Remove(other.gameObject);
        }
        else if (other.gameObject.layer == safeZoneLayerID)
        {
			inSafeZone = false;
        }
	}

	private void OnDisable()
	{
		Castle.OnCastleComplete -= OnCastleComplete;
		GameManager.OnGameStateChanged -= OnGameStateChanged;

	}

	#endregion

	#region IPlayer
	public bool IsInSafeZone() { return inSafeZone; }

	public void OnGameStateChanged(Gamestate gamestate)
	{
        switch (gamestate)
        {
            case Gamestate.LoadState:
				GetComponent<GoapAgent>().enabled = false;
				CanMove = false;
                break;
            case Gamestate.RoundStart:
				GetComponent<GoapAgent>().enabled = true;
				CanMove = true;
				break;
            case Gamestate.RoundOverState:
				GetComponent<GoapAgent>().enabled = false;
				AgentMoveSpeed = 0;
				//gameObject.SetActive(false);
				break;
            case Gamestate.MetagameSate:
                break;
            case Gamestate.GameoverState:
                break;
            default:
                break;
        }
    }

	public void ToFallState()
	{
		InFallState = true;
		CanMove = false;
		animator.SetTrigger(fallHash);

	}
	public void ToIdleState()
	{
		InFallState = false;
		CanMove = true;
		AgentMoveSpeed = 20;
		animator.SetTrigger(idleHash);

	}
	#endregion

	private void OnCastleComplete(BrickType inBrickType)
	{
		if (brickType == inBrickType)
		{
			castleComplete = true;
			GameManager.Instance.ChangeGameState(Gamestate.RoundOverState);
		}
	}

	public bool AttackTargetAvailable()
	{
        foreach (var item in enemiesInRange)
        {
            if (item.GetComponentInChildren<BrickStackHandler>().BrickStackCount<brickStackHandler.BrickStackCount)
            {
				return true;
            }
        }
		return false;
	}

	public bool AttackerAvailable()
	{
		foreach (var item in enemiesInRange)
		{
			if (item.GetComponentInChildren<BrickStackHandler>().BrickStackCount > brickStackHandler.BrickStackCount)
			{
				return true;
			}
		}
		return false;
	}

	public bool TargetBrickAvailable() { return TargetBrick != null; }

	public GameObject GetAttackTarget()
	{
		foreach (var item in enemiesInRange)
		{
			if (item.GetComponentInChildren<BrickStackHandler>().BrickStackCount < brickStackHandler.BrickStackCount)
			{
				return item;
			}
		}
		return null;
	}

	public GameObject GetAttacker()
	{
		foreach (var item in enemiesInRange)
		{
			if (item.GetComponentInChildren<BrickStackHandler>().BrickStackCount > brickStackHandler.BrickStackCount)
			{
				return item;
			}
		}
		return null;
	}

	

	#region IGOAP
	/**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 */
	public HashSet<KeyValuePair<string, object>> getWorldState()
	{
		HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

		//worldData.Add(new KeyValuePair<string, object>(ConstStrings.ATTACKTARGETINRANGE, AttackTargetAvailable()));
		//worldData.Add(new KeyValuePair<string, object>(ConstStrings.ATTACKERINRANGER, AttackerAvailable()));
		worldData.Add(new KeyValuePair<string, object>(ConstStrings.ENOUGHBRICKSCOLLECTED, (brickStackHandler.EnoughBricksAvailable())));
		worldData.Add(new KeyValuePair<string, object>(ConstStrings.BRICKINRANGE, TargetBrickAvailable()));
		worldData.Add(new KeyValuePair<string, object>(ConstStrings.SUBMITTINGBRICKS, brickStackHandler.submittingBricks));

		return worldData;
	}
	
	/**
	 * Implement in subclasses
	 */
	public  HashSet<KeyValuePair<string, object>> createGoalState()
	{
		/*HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>
		{

			new KeyValuePair<string,object>(ConstStrings.COLLECTBRICK,true),
			//new KeyValuePair<string, object>(ConstStrings.SUBMITBRICK,true)
          
        };*/

		HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        if (brickStackHandler.EnoughBricksAvailable())
        {
			goal.Add(new KeyValuePair<string, object>(ConstStrings.SUBMITBRICK,true)); 
        }
        else 
        {
			goal.Add(new KeyValuePair<string, object>(ConstStrings.COLLECTBRICK, true));
		}



		return goal;
	}


	public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
	{
		// Not handling this here since we are making sure our goals will always succeed.
		// But normally you want to make sure the world state has changed before running
		// the same goal again, or else it will just fail.
	}

	public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
	{
		// Yay we found a plan for our goal
		Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
	}

	public void ActionsFinished()
	{
		// Everything is done, we completed our actions for this gool. Hooray!
		Debug.Log("<color=blue>Actions completed</color>");
	}

	public void PlanAborted(GoapAction aborter)
	{
		// An action bailed out of the plan. State has been reset to plan again.
		// Take note of what happened and make sure if you run the same goal again
		// that it can succeed.
		Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
	}

	public bool MoveAgent(GoapAction nextAction)
	{
		if (!CanMove || InFallState)
		{
			AgentMoveSpeed = 0;
			animator.SetFloat(movementSpeedHash, AgentMoveSpeed);
		}
		if (castleComplete)
        {
			AgentMoveSpeed = 0;
			Debug.Log("making agent speed zero");
			animator.SetFloat(movementSpeedHash, AgentMoveSpeed);

		}

		animator.SetFloat(movementSpeedHash, AgentMoveSpeed);
		// move towards the NextAction's target
		float step = AgentMoveSpeed * Time.deltaTime;
		gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);
		transform.LookAt(nextAction.target.transform);

		//if (gameObject.transform.position.Equals(nextAction.target.transform.position))
		if ((transform.position - nextAction.target.transform.position).magnitude < nextAction.MinDistanceFromTarget)

		{
			// we are at the target location, we are done
			nextAction.SetInRange(true);
			return true;
		}
		else
			return false;
	}


    #endregion
}
