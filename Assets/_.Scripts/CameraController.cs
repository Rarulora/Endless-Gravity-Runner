using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("Target")]
	[SerializeField] private Transform target;

	[Header("Smoothing")]
	[Tooltip("Daha küçük deðer = daha sýký takip")]
	[SerializeField] private float smoothTime = 0.15f;

	[Tooltip("Karakterin önünü biraz göster (sabit bakýþ ötelemesi)")]
	[SerializeField] private float lookAheadX = 0f;

	[Header("Sýnýrlar (opsiyonel)")]
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

		// Ýstenen X = hedef + baþlangýç ofseti + isteðe baðlý öteleme
		float desiredX = target.position.x + offsetX + lookAheadX;

		if (useClamp)
			desiredX = Mathf.Clamp(desiredX, minX, maxX);

		float newX = Mathf.SmoothDamp(transform.position.x, desiredX, ref xVel, smoothTime);

		// Sadece X'i güncelle, Y ve Z sabit kalsýn
		transform.position = new Vector3(newX, initialY, initialZ);
	}

}
