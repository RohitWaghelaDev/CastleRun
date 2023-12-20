using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,IPlayer
{
	public delegate void PlayerWon(bool result);
	public static event PlayerWon OnplayerWonRound;


	private float m_ForwardMoveSpeed=20;
	private float m_JoystickYaxisValue;
	private float m_JoystickXaxisValue;
	private float m_JoystickMagnitude;

	private float m_MoveSpeedValue;

	private Rigidbody m_Rigidbody;
	private Gamestate gamestate;

	private bool inSafeZone;
	private bool canRotate;
	private int safeZoneLayerID;
	private int playerLayerID;
	private BrickStackHandler brickStackHandler;

	BrickType IPlayer.BrickType => BrickType.BlueBrick;
	public bool CanMove { get; set; }
	public bool InFallState { get; set; }
	
	public BrickStackHandler BrickStackHandler => brickStackHandler;

	private Animator animator;
	private int idleHash = Animator.StringToHash("Idle");
	private int runHash = Animator.StringToHash("Run");
	private int fallHash = Animator.StringToHash("Fall");
	private int movementSpeedHash = Animator.StringToHash("MovementSpeed");

	
    #region unityFunctions

    void OnEnable()
	{
		GameManager.OnGameStateChanged += OnGameStateChanged;
		Castle.OnCastleComplete += OnCastleComplete;
	}

	void Start()
	{
        

		if (m_Rigidbody == null)
		{
			m_Rigidbody = GetComponent<Rigidbody>();
		}

        if (animator==null)
        {
			animator = GetComponent<Animator>();
        }

		m_Rigidbody.centerOfMass = new Vector3(0, -0.9f, 0);
		brickStackHandler = GetComponentInChildren<BrickStackHandler>();
		safeZoneLayerID = LayerMask.NameToLayer("SafeZone");
		playerLayerID = LayerMask.NameToLayer("Player");
	}




	void Update()
	{
		if (!CanMove || InFallState)
		{
			return;
		}
		MoveRB();
		TurnRB();

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == safeZoneLayerID && other.gameObject.GetComponent<SafeZone>().brickType == BrickType.BlueBrick)
		{
			inSafeZone = true;
			brickStackHandler.SubmitBricks(OnSubmittedBricks);
			
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == safeZoneLayerID && other.gameObject.GetComponent<SafeZone>().brickType== BrickType.BlueBrick)
		{
			inSafeZone = false;
			brickStackHandler.StopSubmittingBricks();
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

	/*
    private void OnCollisionEnter(Collision collision)
    {
		Debug.Log("7777777 collision"+ collision.gameObject.name);
        if (collision.gameObject.layer==playerLayerID)
        {
			IPlayer enemyPlayer = collision.gameObject.GetComponent<IPlayer>();
            if (enemyPlayer!=null && !enemyPlayer.IsInSafeZone())
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
	*/

    void OnDisable()
	{

		GameManager.OnGameStateChanged -= OnGameStateChanged;
		Castle.OnCastleComplete -= OnCastleComplete;
	}

	#endregion

	public void MoveRB()
	{

		m_JoystickMagnitude = VirtualJoystick.VerticalMagnitude();

		m_MoveSpeedValue = m_JoystickMagnitude * m_ForwardMoveSpeed;
		animator.SetFloat(movementSpeedHash, m_MoveSpeedValue);
		//Debug.Log("movement speed " + m_MoveSpeedValue);
		canRotate = m_MoveSpeedValue > 1 ? true : false;
		if (m_MoveSpeedValue >= 0.1f)
		{
			Vector3 movement = transform.forward * m_MoveSpeedValue * Time.deltaTime;
			m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        }
       
		
	}

	public void TurnRB()
	{
        if (!canRotate)
        {
			return;
        }
	
		m_JoystickYaxisValue = VirtualJoystick.Vertical();
		m_JoystickXaxisValue = VirtualJoystick.Horizontal();

		Quaternion newrot = Quaternion.Euler(m_Rigidbody.rotation.x, Mathf.Atan2(m_JoystickXaxisValue, m_JoystickYaxisValue) * Mathf.Rad2Deg, m_Rigidbody.rotation.z);

		//below is  old way for rotating tank
		//tank.tankRigidBody.rotation = Quaternion.Slerp (tank.tankRigidBody.rotation, newrot, tank.rotSmoothValue* Time.deltaTime );
		//new way
		m_Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, newrot, 7));
	}


	void RoundOver()
	{

		m_JoystickYaxisValue = 0;
		m_JoystickXaxisValue = 0;
		m_JoystickMagnitude = 0;
	}


	private void OnCastleComplete(BrickType inBrickType)
	{
		if (inBrickType == BrickType.BlueBrick)
		{
			
			OnplayerWonRound?.Invoke(true);
			GameManager.Instance.ChangeGameState(Gamestate.RoundOverState);
			int currenrLevel = PlayerPrefs.GetInt(ConstStrings.CURRENTLEVEL);
			currenrLevel ++;
			PlayerPrefs.SetInt(ConstStrings.CURRENTLEVEL,currenrLevel);
		}
		else
        {
			OnplayerWonRound?.Invoke(false);
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
				CanMove = false;
				break;
			case Gamestate.RoundStart:
				CanMove = true;
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
		animator.SetTrigger(idleHash);
		animator.SetFloat(movementSpeedHash, 0);
	}

	#endregion


	public void OnSubmittedBricks()
	{
		
	}
}
