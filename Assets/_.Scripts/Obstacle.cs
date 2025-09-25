using UnityEngine;

public enum VerticalAnchor { TopCenter, BottomCenter }

[DisallowMultipleComponent]
public class Obstacle : MonoBehaviour
{
	[SerializeField] private bool flipVisualScale = false;
	[SerializeField] private float width = 1f;
	[SerializeField] private float height = 1f;

	[SerializeField] private bool autoFromBounds = true;
	[SerializeField] private bool includeColliders = true;

	[SerializeField] private bool snapToYEdges = false;



	private float initialZ;
	private float baseScaleYAbs;

	public float HalfWidth => width * 0.5f;
	public float HalfHeight => height * 0.5f;
	public float GetRightmostX() => transform.position.x + HalfWidth;
	public bool SnapToYEdges => snapToYEdges;

	

	public void PlaceAligned(float x, float y, VerticalAnchor anchor)
	{
		if (!gameObject.activeSelf) gameObject.SetActive(true);

		float posY = (anchor == VerticalAnchor.TopCenter)
			? (y - HalfHeight)
			: (y + HalfHeight);

		transform.position = new Vector3(x, posY, initialZ);

		if (flipVisualScale)
		{
			var s = transform.localScale;
			s.y = (anchor == VerticalAnchor.TopCenter) ? -baseScaleYAbs : +baseScaleYAbs;
			transform.localScale = s;
		}
	}

	private void Awake()
	{
		initialZ = transform.position.z;
		baseScaleYAbs = Mathf.Abs(transform.localScale.y);
		if (autoFromBounds) RecalculateSizeFromBounds();
	}

	private void OnValidate()
	{
		if (width < 0f) width = 0f;
		if (height < 0f) height = 0f;
		if (autoFromBounds) RecalculateSizeFromBounds();
	}

	public void RecalculateSizeFromBounds()
	{
		Collider2D collider = GetComponent<Collider2D>();
		if (collider != null)
		{
			height = collider.bounds.size.y;
			width = collider.bounds.size.x;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.25f);
		Gizmos.DrawWireCube(transform.position, new Vector3(Mathf.Max(width, 0.001f), Mathf.Max(height, 0.001f), 0.01f));
	}
#endif
}
