using UnityEngine;
using TMPro;

public class MenuUI : MonoBehaviour
{
	[Header("UI Refs")]
	[SerializeField] private TMP_Text highScoreText;

	private void OnEnable()
	{
		RefreshHighScore();
		if (GameManager.Instance != null)
			GameManager.OnStateChanged += HandleState;
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
			GameManager.OnStateChanged -= HandleState;
	}

	private void HandleState(GameState s)
	{
		if (s == GameState.MainMenu)
			RefreshHighScore();
	}

	private void RefreshHighScore()
	{
		if (!highScoreText) return;

		if (GameManager.Instance == null)
		{
			highScoreText.text =
				"<color=grey><b>High Score: </b></color>-\n\n" +
				"<color=grey><b>Last Score: </b></color>-";
			return;
		}

		var data = GameManager.Instance.Data;
		highScoreText.text =
			$"<color=grey><b>High Score: </b></color>{data.HighScore}\n\n" +
			$"<color=grey><b>Last Score: </b></color>{data.LastScore}";
	}

	public void OnPlayButton()
	{
		if (GameManager.Instance) GameManager.Instance.StartRun();
	}
	public void OnResetSavesButton()
	{
		GameManager.Instance.ResetAllSaves();
		RefreshHighScore();
	}
}
