using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour
{
	public BrickType brickType;
	private Gamestate gamestate;

	public bool CanMove { get; set; }
	private bool inSafeZone;
	private int safeZoneLayerID;
	private int playerLayerID;
	private BrickStackHandler brickStackHandler;
	public BrickStackHandler BrickStackHandler => brickStackHandler;
	//BrickType IPlayer.BrickType => brickType;
	public bool InFallState { get; set; }
	// Start is called before the first frame update
	void Start()
    {
		brickStackHandler = GetComponentInChildren<BrickStackHandler>();
		safeZoneLayerID = LayerMask.NameToLayer("SafeZone");
		playerLayerID = LayerMask.NameToLayer("Player");
	}

    


	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == safeZoneLayerID && other.gameObject.GetComponent<SafeZone>().brickType == BrickType.BlueBrick)
		{
			inSafeZone = true;
			//brickStackHandler.SubmitBricks(OnSubmittedBricks);

		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == safeZoneLayerID && other.gameObject.GetComponent<SafeZone>().brickType == BrickType.BlueBrick)
		{
			inSafeZone = false;
			//brickStackHandler.StopSubmittingBricks();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("7777777 collision" + collision.gameObject.name);
		if (collision.gameObject.layer == playerLayerID)
		{
			IPlayer enemyPlayer = collision.gameObject.GetComponent<IPlayer>();
			if (enemyPlayer != null && !enemyPlayer.IsInSafeZone())
			{
				if (enemyPlayer.BrickStackHandler.BrickStackCount < brickStackHandler.BrickStackCount)
				{
					enemyPlayer.BrickStackHandler.OnHitWithEnemy();
				}
				else
				{
					brickStackHandler.OnHitWithEnemy();
				}
			}
		}
	}

	#region IPlayer

	public bool IsInSafeZone() { return inSafeZone; }

	public void OnGameStateChanged(Gamestate inGamestate)
	{
		gamestate = inGamestate;

		switch (gamestate)
		{
			case Gamestate.LoadState:
				break;
			case Gamestate.RoundStart:
				break;
			case Gamestate.RoundOverState:
				gameObject.SetActive(false);
				break;
			case Gamestate.MetagameSate:
				break;
			case Gamestate.GameoverState:
				break;
			default:
				break;
		}
	}


	#endregion

}
