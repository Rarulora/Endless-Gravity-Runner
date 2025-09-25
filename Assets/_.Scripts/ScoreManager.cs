using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    [SerializeField] private TMP_Text scoreText;
    int score;
	public int Score => score;
	public int HighScore => PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);

	const string HIGHSCORE_KEY = "HighScore";

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
	}
	private void Start()
	{
		UpdateUI();
	}
	public void AddScore(int amount)
    {
		if (GameManager.Instance.State != GameState.Gameplay) return;
        score += amount;
        UpdateUI();
    }
	void HandleState(GameState s)
	{
		if (s == GameState.GameOver)
		{
			int hs = PlayerPrefs.GetInt(HIGHSCORE_KEY, 0);
			if (score > hs)
				PlayerPrefs.SetInt(HIGHSCORE_KEY, hs);
		}
	}
	private void OnEnable()
	{
		GameManager.OnStateChanged += HandleState;
	}
	private void OnDisable()
	{
		GameManager.OnStateChanged -= HandleState;
	}

	void UpdateUI() => scoreText.text = score.ToString();
}
