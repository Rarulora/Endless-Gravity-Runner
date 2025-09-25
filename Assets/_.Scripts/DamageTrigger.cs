using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
	const string PLAYER = "Player";
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag(PLAYER))
		{
			GameManager.Instance.GameOver();
		}
	}
}
