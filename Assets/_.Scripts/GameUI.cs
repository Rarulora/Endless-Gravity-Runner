using UnityEngine;
using TMPro; // TextMeshPro kullanýyoruz

public class GameUI : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject gameOverPanel;

	[Header("GameOver Texts")]
	[SerializeField] private TMP_Text scoreText;



	private void OnEnable()
	{
		GameManager.OnStateChanged += HandleState;
		HandleState(GameManager.Instance.State);
		SetPause(false); 
		SetGameOver(false);
	}

	private void OnDisable()
	{
		GameManager.OnStateChanged -= HandleState;
	}

	private void HandleState(GameState state)
	{
		SetPause(state == GameState.Pause);

		bool showGO = (state == GameState.GameOver);
		SetGameOver(showGO);

		if (showGO)
		{
			if (scoreText) scoreText.text = $"<color=grey><b>Score: </b></color>{ScoreManager.Instance.Score}\n\n" +
					$"<color=grey><b>High Score: </b></color>{ScoreManager.Instance.HighScore}";

		}
	}

	private void SetPause(bool on)
	{
		if (pausePanel) pausePanel.SetActive(on);
	}

	private void SetGameOver(bool on)
	{
		if (gameOverPanel) gameOverPanel.SetActive(on);
	}

	public void OnResumeButton() { GameManager.Instance.Resume(); }
	public void OnRestartButton() { GameManager.Instance.RestartRun(); } 
	public void OnMenuButton() { GameManager.Instance.ToMenu(); }
	public void OnPauseButton() { GameManager.Instance.Pause(); }
}
