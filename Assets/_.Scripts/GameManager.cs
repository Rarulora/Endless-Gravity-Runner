using System;
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

	private void Awake()
	{
		if(Instance == null)
			Instance = this;
		else
		{
			Destroy(this);
			return;
		}
		DontDestroyOnLoad(Instance);

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
	public void GameOver() => SetState(GameState.GameOver);
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
}
