using System;
using System.IO;
using UnityEngine;
#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

[Serializable]
public class SaveData
{
	public int HighScore;
	public int LastScore;
	public string Version = "1.0.0";
	public long LastUpdatedUnix;
}

public static class SaveSystem
{
	private const string FileName = "save.json";
	private static string FullPath => Path.Combine(Application.persistentDataPath, FileName);

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void GR_SyncFS_In();
    [DllImport("__Internal")] private static extern void GR_SyncFS_Out();
#endif

	public static void SyncInIfWebGL()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        GR_SyncFS_In();
#endif
	}

	/// Call after writing files to push memory to IndexedDB.
	public static void SyncOutIfWebGL()
	{
#if UNITY_WEBGL && !UNITY_EDITOR
        GR_SyncFS_Out();
#endif
	}

	public static void Save(SaveData data)
	{
		try
		{
			data.LastUpdatedUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

			Directory.CreateDirectory(Application.persistentDataPath);

			string json = JsonUtility.ToJson(data, prettyPrint: false);
			File.WriteAllText(FullPath, json);

			SyncOutIfWebGL();
		}
		catch (Exception e)
		{
			Debug.LogError($"[SaveSystem] Save error: {e}");
		}
	}

	public static SaveData Load()
	{
		try
		{
			if (!File.Exists(FullPath))
			{
				return new SaveData();
			}

			string json = File.ReadAllText(FullPath);
			var data = JsonUtility.FromJson<SaveData>(json);
			return data ?? new SaveData();
		}
		catch (Exception e)
		{
			Debug.LogError($"[SaveSystem] Load error: {e}");
			return new SaveData();
		}
	}

	public static void DeleteAll()
	{
		try
		{
			if (File.Exists(FullPath))
			{
				File.Delete(FullPath);
				SyncOutIfWebGL();
			}
		}
		catch (Exception e)
		{
			Debug.LogError($"[SaveSystem] Delete error: {e}");
		}
	}
}
