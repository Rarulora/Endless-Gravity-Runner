using System;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Gameplay, Pause, GameOver }

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
	private GameState startState = GameState.MainMenu;
	public GameState State { get; private set; }
	public static event Action<GameState> OnStateChanged;

	const string GAMESCENE = "Gameplay";
	const string MAINMENUSCENE = "MainMenu";

	public SaveData Data { get; private set; } = new SaveData();

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else { Destroy(gameObject); return; }

		DontDestroyOnLoad(Instance);

		SaveSystem.SyncInIfWebGL();

		Load();

		SetState(startState, force: true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (State == GameState.Gameplay) Pause();
			else if (State == GameState.Pause) Resume();
		}
	}

	public void ToMenu()
	{
		SceneManager.LoadScene(MAINMENUSCENE);
		SetState(GameState.MainMenu, true);
	}

	public void StartRun()
	{
		SceneManager.LoadScene(GAMESCENE);
		SetState(GameState.Gameplay);
	}

	public void Pause() => SetState(GameState.Pause);
	public void Resume() => SetState(GameState.Gameplay);
	public void GameOver()
	{
		int currentScore = 0;
		if (ScoreManager.Instance != null)
			currentScore = ScoreManager.Instance.Score;
		else
			currentScore = Data.LastScore;

		Data.LastScore = currentScore;
		if (currentScore > Data.HighScore)
			Data.HighScore = currentScore;

		Save();

		SetState(GameState.GameOver);
	}

	public void RestartRun()
	{
		SceneManager.LoadScene(GAMESCENE);
		SetState(GameState.Gameplay, true);
	}

	public void SetState(GameState next, bool force = false)
	{
		if (!force && State == next) return;
		State = next;
		ApplyTimeScale(next);
		OnStateChanged?.Invoke(State);
	}

	private void ApplyTimeScale(GameState s)
	{
		Time.timeScale = (s == GameState.Gameplay) ? 1f : 0f;
		if (s == GameState.GameOver)
			Time.timeScale = 0.01f;
	}

	public void Save()
	{
		if (ScoreManager.Instance != null)
		{
			Data.LastScore = ScoreManager.Instance.Score;
			Data.HighScore = Math.Max(Data.HighScore, ScoreManager.Instance.HighScore);
		}

		SaveSystem.Save(Data);
	}

	public void Load()
	{
		Data = SaveSystem.Load();
	}

	public void ResetAllSaves()
	{
		SaveSystem.DeleteAll();
		Data = new SaveData();
	}
}
