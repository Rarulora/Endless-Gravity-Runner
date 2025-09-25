using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
	[Header("Panels")]
	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject gameOverPanel;

	[Header("GameOver Texts")]
	[SerializeField] private TMP_Text scoreText;

	private void OnEnable()
	{
		if (GameManager.Instance != null)
		{
			GameManager.OnStateChanged += HandleState;
			HandleState(GameManager.Instance.State);
		}
		else
		{
			SetPause(false);
			SetGameOver(false);
		}
	}

	private void OnDisable()
	{
		if (GameManager.Instance != null)
			GameManager.OnStateChanged -= HandleState;
	}

	private void HandleState(GameState state)
	{
		SetPause(state == GameState.Pause);

		bool showGO = (state == GameState.GameOver);
		SetGameOver(showGO);

		if (showGO && scoreText)
		{
			var gm = GameManager.Instance;
			int currentScore =
				(ScoreManager.Instance != null) ? ScoreManager.Instance.Score
												: gm.Data.LastScore;

			int highScore = gm.Data.HighScore;

			scoreText.text =
				$"<color=grey><b>Score: </b></color>{currentScore}\n\n" +
				$"<color=grey><b>High Score: </b></color>{highScore}";
		}
	}

	private void SetPause(bool on) { if (pausePanel) pausePanel.SetActive(on); }
	private void SetGameOver(bool on) { if (gameOverPanel) gameOverPanel.SetActive(on); }

	public void OnResumeButton() { if (GameManager.Instance) GameManager.Instance.Resume(); }
	public void OnRestartButton() { if (GameManager.Instance) GameManager.Instance.RestartRun(); }
	public void OnMenuButton() { if (GameManager.Instance) GameManager.Instance.ToMenu(); }
	public void OnPauseButton() { if (GameManager.Instance) GameManager.Instance.Pause(); }
}
