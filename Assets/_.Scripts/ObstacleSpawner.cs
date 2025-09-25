using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleSpawner : MonoBehaviour
{
	[SerializeField] private List<Obstacle> obstaclePrefabs = new List<Obstacle>();

	[Header("Y Spawn Aralýðý")]
	[SerializeField] private float yMin = -3f;
	[SerializeField] private float yMax = 3f;

	[Header("Y Grid (opsiyonel)")]
	[SerializeField] private bool useYGrid = true;
	[SerializeField] private float yStep = 0.5f;

	[Header("X Boþluk (kameraya göre)")]
	[SerializeField] private float minGap = 6f;
	[SerializeField] private float maxGap = 12f;

	[SerializeField] private float spawnAhead = 18f;

	[Header("Temizleme")]
	[SerializeField] private float despawnOffset = 15f;

	[Header("Pool")]
	[SerializeField] private int poolCountPerPrefab = 3;

	private readonly Dictionary<Obstacle, Queue<Obstacle>> pools = new();
	private readonly Dictionary<Obstacle, Obstacle> instanceToPrefab = new();

	private readonly List<Obstacle> activeObstacles = new();

	private float nextSpawnX;
	private float lastSpawnX;

	private Transform cameraTransform;

	private void Awake()
	{
		cameraTransform = Camera.main.transform;

		if (yMin > yMax) (yMin, yMax) = (yMax, yMin);

		foreach (var prefab in obstaclePrefabs)
		{
			if (!prefab) continue;

			var q = new Queue<Obstacle>();
			for (int i = 0; i < poolCountPerPrefab; i++)
			{
				var inst = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
				inst.gameObject.SetActive(false);
				q.Enqueue(inst);
				instanceToPrefab[inst] = prefab;
			}
			pools[prefab] = q;
		}

		if (obstaclePrefabs.Count == 0)
		{
			enabled = false;
			return;
		}

		lastSpawnX = cameraTransform.position.x;
		nextSpawnX = cameraTransform.position.x + spawnAhead;
	}

	private void Update()
	{
		float camX = cameraTransform.position.x;

		while (camX + spawnAhead >= nextSpawnX)
		{
			SpawnOne(nextSpawnX);
			float gap = Random.Range(minGap, maxGap);
			nextSpawnX = lastSpawnX + gap;
		}

		DespawnBehindCamera(camX);
	}

	void SpawnOne(float targetX)
	{
		if (obstaclePrefabs.Count == 0) return;

		var prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];
		var obstacle = GetFromPool(prefab);
		obstacle.gameObject.SetActive(true);

		float y;
		VerticalAnchor anchor;

		if (obstacle.SnapToYEdges)
		{
			bool snapTop = (Random.value < 0.5f);
			if (snapTop)
			{
				y = yMax;
				anchor = VerticalAnchor.TopCenter;
			}
			else
			{
				y = yMin;
				anchor = VerticalAnchor.BottomCenter;
			}
		}
		else
		{
			float rawY = Random.Range(yMin, yMax);

			if (useYGrid)
				rawY = SnapToStep(rawY, yStep);

			y = Mathf.Clamp(rawY, yMin, yMax);

			anchor = (y > 0f) ? VerticalAnchor.TopCenter : VerticalAnchor.BottomCenter;
		}

		obstacle.PlaceAligned(targetX, y, anchor);

		lastSpawnX = obstacle.GetRightmostX();
		activeObstacles.Add(obstacle);
	}


	Obstacle GetFromPool(Obstacle prefab)
	{
		var q = pools[prefab];
		if (q.Count > 0)
		{
			var inst = q.Dequeue();
			return inst;
		}

		var extra = Instantiate(prefab, Vector3.one * 99999f, Quaternion.identity, transform);
		extra.gameObject.SetActive(false);
		instanceToPrefab[extra] = prefab;
		return extra;
	}

	void ReturnToPool(Obstacle obs)
	{
		obs.gameObject.SetActive(false);

		if (instanceToPrefab.TryGetValue(obs, out var prefab) && pools.TryGetValue(prefab, out var q))
		{
			q.Enqueue(obs);
			return;
		}

		pools[obstaclePrefabs[0]].Enqueue(obs);
	}

	void DespawnBehindCamera(float camX)
	{
		float limitX = camX - despawnOffset;

		for (int i = activeObstacles.Count - 1; i >= 0; i--)
		{
			var obs = activeObstacles[i];
			if (!obs) { activeObstacles.RemoveAt(i); continue; }

			if (obs.GetRightmostX() < limitX)
			{
				activeObstacles.RemoveAt(i);
				ReturnToPool(obs);
			}
		}
	}

	private static float SnapToStep(float value, float step)
	{
		if (step <= Mathf.Epsilon) return value;
		return Mathf.Round(value / step) * step;
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		var cam = Camera.main;
		if (!cam) return;

		Transform camT = cam.transform;

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(new Vector3(camT.position.x + spawnAhead, yMin, 0),
						new Vector3(camT.position.x + spawnAhead, yMax, 0));

		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(new Vector3(camT.position.x - despawnOffset, yMin, 0),
						new Vector3(camT.position.x - despawnOffset, yMax, 0));
	}
#endif
}
