using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ViewController : MonoBehaviour
{
	public static ViewController Instance;
	public Image brickAmountFillerImage;

	[SerializeField] private TextMeshProUGUI levelText;
	[SerializeField] private TextMeshProUGUI resultText;
	[SerializeField] private GameObject joyStickCanvas;
	[SerializeField] private GameObject startGameCanvas;
	[SerializeField] private GameObject roundOverCanvas;

	[SerializeField] private Button startRoundButton;
	[SerializeField] private Button restartButton;
	[SerializeField] private Button nextRoundButton;

	private bool hasPlayerWonRound = false;

    private void Awake()
    {
        if (Instance== null)
        {
			Instance = GetComponent<ViewController>();

		}
    }

    void OnEnable()
	{
		GameManager.OnGameStateChanged += OnGameStateChanged;
        PlayerController.OnplayerWonRound += PlayerController_OnplayerWonRound;
		startRoundButton.onClick.AddListener(OnClickStartRound);
		restartButton.onClick.AddListener(OnClickRestartRound);
		nextRoundButton.onClick.AddListener(OnClickNextRound);
	}

    private void PlayerController_OnplayerWonRound(bool result)
    {
		hasPlayerWonRound = result;

	}

    void OnDisable()
	{

		GameManager.OnGameStateChanged -= OnGameStateChanged;
		PlayerController.OnplayerWonRound -= PlayerController_OnplayerWonRound;
		startRoundButton.onClick.RemoveListener(OnClickStartRound);
		restartButton.onClick.RemoveListener(OnClickRestartRound);
		nextRoundButton.onClick.RemoveListener(OnClickNextRound);
	}

	public void OnGameStateChanged(Gamestate inGamestate)
	{
		

		switch (inGamestate)
		{
			case Gamestate.LoadState:
				joyStickCanvas.SetActive(false);
				startGameCanvas.SetActive(true);
				roundOverCanvas.SetActive(false);
				hasPlayerWonRound = false;
				int levelNumber=PlayerPrefs.GetInt(ConstStrings.CURRENTLEVEL);
				levelNumber++;
				levelText.text = "LEVEL "+levelNumber.ToString();
				//Debug.LogError("game load state received");
				break;
			case Gamestate.RoundStart:
				joyStickCanvas.SetActive(true);
				startGameCanvas.SetActive(false);
				roundOverCanvas.SetActive(false);
				break;
			case Gamestate.RoundOverState:
				joyStickCanvas.SetActive(false);
				startGameCanvas.SetActive(false);
				roundOverCanvas.SetActive(true);
				if (hasPlayerWonRound)
				{
					Invoke(nameof(SetupRoundOver), 3);
                }
                else
				{
					SetupRoundOver();
				}
				break;
			case Gamestate.MetagameSate:
				break;
			case Gamestate.GameoverState:
				break;
			default:
				break;
		}
	}


    public void SetupRoundOver()
    {
        if (hasPlayerWonRound)
        {
			nextRoundButton.gameObject.SetActive(true);
			restartButton.gameObject.SetActive(false);
			resultText.text = "Level Cleared";

		}
        else
        {
			nextRoundButton.gameObject.SetActive(false);
			restartButton.gameObject.SetActive(true);
			resultText.text = "Level Failed";
		}
    }


	public void OnClickStartRound()
	{
		GameManager.Instance.ChangeGameState(Gamestate.RoundStart);
	}
	public void OnClickNextRound()
	{
		SceneManager.LoadScene(0);
		Debug.Log("reloading");
	}
	public void OnClickRestartRound()
	{
		
		SceneManager.LoadSceneAsync(0);
		Debug.Log("reloading");
	}
}
