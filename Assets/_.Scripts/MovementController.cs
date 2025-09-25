using UnityEngine;

public class MovementController : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] private float baseSpeed = 6f;
	[SerializeField] private float maxSpeed = 12f;
	[SerializeField] private float scoreForMaxSpeed = 100f;
	[SerializeField] private float accelPerSec = 10f;

	[Header("Sprint")]
	[SerializeField] private float sprintMultiplier = 1.5f;

	[Header("Gravity (parallel growth)")]
	[SerializeField] private float maxGravity = 14f;
	[SerializeField] private float gravityAccelPerSec = 10f;

	[Header("Gravity Flip")]
	[SerializeField] private LayerMask groundMask;
	[SerializeField] private float groundCheckDistance = 1f;
	[SerializeField] private float flipCooldown = 0.12f;
	[SerializeField] private bool flipVisualScale = true;

	private Rigidbody2D rb;
	private Collider2D col;
	private float lastFlipTime = -999f;

	private float baseGravity;
	private float currentSpeed;
	private float currentGravityMag;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		col = GetComponent<Collider2D>();

		currentSpeed = baseSpeed;
		baseGravity = Mathf.Abs(rb.gravityScale);
		currentGravityMag = Mathf.Max(baseGravity, 0.01f);
		rb.gravityScale = Mathf.Sign(rb.gravityScale == 0 ? 1 : rb.gravityScale) * currentGravityMag;
	}

	private void Update()
	{
		float score = (ScoreManager.Instance != null) ? ScoreManager.Instance.Score : 0f;
		float t = (scoreForMaxSpeed > 0f) ? Mathf.Clamp01(score / scoreForMaxSpeed) : 1f;

		float targetSpeed = Mathf.Lerp(baseSpeed, maxSpeed, t);
		float targetGravityMag = Mathf.Lerp(Mathf.Abs(baseGravity), maxGravity, t);

		currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelPerSec * Time.deltaTime);
		currentGravityMag = Mathf.MoveTowards(currentGravityMag, targetGravityMag, gravityAccelPerSec * Time.deltaTime);

		bool isSprinting = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1) || Input.GetKey(KeyCode.D);
		float sprintMul = isSprinting ? Mathf.Max(1f, sprintMultiplier) : 1f;

		float finalSpeed = Mathf.Min(currentSpeed * sprintMul, maxSpeed * Mathf.Max(1f, sprintMultiplier));
		rb.linearVelocity = new Vector2(finalSpeed, rb.linearVelocity.y);

		float sign = Mathf.Sign(rb.gravityScale == 0 ? 1 : rb.gravityScale);
		rb.gravityScale = sign * currentGravityMag;

		if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W)
			|| Input.GetKeyDown(KeyCode.S) || Input.GetMouseButtonDown(0))
		{
			TryFlipGravity();
		}
	}


	private void TryFlipGravity()
	{
		if (Time.time - lastFlipTime < flipCooldown) return;
		if (OnGround())
		{
			FlipGravity();
			lastFlipTime = Time.time;
		}
	}

	private bool OnGround()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, groundCheckDistance, groundMask);
		if (!hit) hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
		return hit;
	}

	private void FlipGravity()
	{
		rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
		float sign = Mathf.Sign(rb.gravityScale == 0 ? 1 : rb.gravityScale);
		rb.gravityScale = -sign * currentGravityMag;

		if (flipVisualScale)
		{
			var s = transform.localScale;
			s.y *= -1f;
			transform.localScale = s;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + groundCheckDistance * Vector3.up);
		Gizmos.DrawLine(transform.position, transform.position + groundCheckDistance * Vector3.down);
	}
}
