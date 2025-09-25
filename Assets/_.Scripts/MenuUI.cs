using UnityEngine;
using TMPro; // TextMeshPro

public class MenuUI : MonoBehaviour
{
	[Header("UI Refs")]
	[SerializeField] private TMP_Text highScoreText;

	const string HIGHSCORE_KEY = "HighScore";

	private void OnEnable()
	{
		RefreshHighScore();
		GameManager.OnStateChanged += HandleState;
	}

	private void OnDisable()
	{
		GameManager.OnStateChanged -= HandleState;
	}

	private void HandleState(GameState s)
	{
		if (s == GameState.MainMenu)
			RefreshHighScore();
	}

	private void RefreshHighScore()
	{
		int hs = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
		highScoreText.text = $"<color=grey><b>High Score: </b></color>{hs}";
	}

	public void OnPlayButton()
	{
		GameManager.Instance.StartRun();
	}
}
