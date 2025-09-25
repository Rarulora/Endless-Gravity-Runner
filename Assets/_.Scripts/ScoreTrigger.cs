using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
	[SerializeField] int ScoreAmount = 1;
    bool isTriggered = false;

	const string PLAYER = "Player";
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(PLAYER) && !isTriggered)
		{
			isTriggered = true;
			ScoreManager.Instance.AddScore(ScoreAmount);
		}
	}

	private void OnEnable()
	{
		isTriggered = false;	
	}
}
