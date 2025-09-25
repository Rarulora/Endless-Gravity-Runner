using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform target;

	[Header("Smoothing")]
	[Tooltip("Daha k���k de�er = daha s�k� takip")]
	[SerializeField] private float smoothTime = 0.15f;

	[Tooltip("Karakterin �n�n� biraz g�ster (sabit bak�� �telemesi)")]
	[SerializeField] private float lookAheadX = 0f;

	[Header("S�n�rlar (opsiyonel)")]
	[SerializeField] private bool useClamp = false;
	[SerializeField] private float minX = -1000f;
	[SerializeField] private float maxX = 1000f;

	private float xVel;
	private float initialY;
	private float initialZ;
	private float offsetX;

	private void Awake()
	{
		if (target == null)
			target = GameObject.FindWithTag("Player").transform;

		initialY = transform.position.y;
		initialZ = transform.position.z;
		offsetX = transform.position.x - target.position.x;
	}

	private void LateUpdate()
	{
		if (!target) return;

		// �stenen X = hedef + ba�lang�� ofseti + iste�e ba�l� �teleme
		float desiredX = target.position.x + offsetX + lookAheadX;

		if (useClamp)
			desiredX = Mathf.Clamp(desiredX, minX, maxX);

		float newX = Mathf.SmoothDamp(transform.position.x, desiredX, ref xVel, smoothTime);

		// Sadece X'i g�ncelle, Y ve Z sabit kals�n
		transform.position = new Vector3(newX, initialY, initialZ);
	}

}
