using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

// Token: 0x0200032C RID: 812
public static class GTFileLog
{
	// Token: 0x17000206 RID: 518
	// (get) Token: 0x0600141A RID: 5146 RVA: 0x0006CC64 File Offset: 0x0006AE64
	private static GTFileLog.FLogInstance Default
	{
		get
		{
			if (GTFileLog._default != null)
			{
				return GTFileLog._default;
			}
			object registryLock = GTFileLog._registryLock;
			GTFileLog.FLogInstance @default;
			lock (registryLock)
			{
				if (GTFileLog._default == null)
				{
					GTFileLog._default = new GTFileLog.FLogInstance("main");
				}
				@default = GTFileLog._default;
			}
			return @default;
		}
	}

	// Token: 0x0600141B RID: 5147 RVA: 0x0006CCC8 File Offset: 0x0006AEC8
	public static GTFileLog.FLogInstance GetLog(string name)
	{
		object registryLock = GTFileLog._registryLock;
		GTFileLog.FLogInstance flogInstance2;
		lock (registryLock)
		{
			GTFileLog.FLogInstance flogInstance;
			if (GTFileLog._instances.TryGetValue(name, out flogInstance))
			{
				flogInstance2 = flogInstance;
			}
			else
			{
				GTFileLog.FLogInstance flogInstance3 = new GTFileLog.FLogInstance(name);
				GTFileLog._instances[name] = flogInstance3;
				flogInstance2 = flogInstance3;
			}
		}
		return flogInstance2;
	}

	// Token: 0x0600141C RID: 5148 RVA: 0x0006CD30 File Offset: 0x0006AF30
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void Log(string msg)
	{
		GTFileLog.Default.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x0600141D RID: 5149 RVA: 0x0006CD47 File Offset: 0x0006AF47
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarning(string msg)
	{
		GTFileLog.Default.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x0600141E RID: 5150 RVA: 0x0006CD5E File Offset: 0x0006AF5E
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogError(string msg)
	{
		GTFileLog.Default.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
	}

	// Token: 0x0600141F RID: 5151 RVA: 0x0006CD75 File Offset: 0x0006AF75
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("LOG", msg);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0006CD87 File Offset: 0x0006AF87
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogWarningNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("WARN", msg);
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0006CD99 File Offset: 0x0006AF99
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void LogErrorNoTrace(string msg)
	{
		GTFileLog.Default.WriteEntryNoTrace("ERR", msg);
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0006CDAC File Offset: 0x0006AFAC
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLog(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("LOG", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("LOG", msg);
				}
			}
		}
		Debug.Log("[GT/FLog] " + msg);
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0006CE6C File Offset: 0x0006B06C
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogWarning(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("WARN", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("WARN", msg);
				}
			}
		}
		Debug.LogWarning("[GT/FLog] " + msg);
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0006CF2C File Offset: 0x0006B12C
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	public static void CLogError(string msg)
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null && GTFileLog._default.IsActive)
			{
				GTFileLog._default.WriteEntryNoTrace("ERR", msg);
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				if (flogInstance.IsActive)
				{
					flogInstance.WriteEntryNoTrace("ERR", msg);
				}
			}
		}
		Debug.LogError("[GT/FLog] " + msg);
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0006CFEC File Offset: 0x0006B1EC
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void Reset()
	{
		object registryLock = GTFileLog._registryLock;
		lock (registryLock)
		{
			if (GTFileLog._default != null)
			{
				GTFileLog._default.Close();
			}
			foreach (GTFileLog.FLogInstance flogInstance in GTFileLog._instances.Values)
			{
				flogInstance.Close();
			}
		}
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0006D078 File Offset: 0x0006B278
	private static void OnUnityLogMessage(string condition, string stackTrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (GTFileLog._inCallback)
		{
			return;
		}
		GTFileLog._inCallback = true;
		try
		{
			string text = ((type == LogType.Exception) ? "EXCEPTION" : ((type == LogType.Assert) ? "ASSERT" : "UNITY_ERR"));
			GTFileLog.Default.WriteEntry(text, condition, stackTrace);
		}
		finally
		{
			GTFileLog._inCallback = false;
		}
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0006D0E4 File Offset: 0x0006B2E4
	internal static string GetTimestamp()
	{
		if (!(NetworkSystem.Instance != null))
		{
			return Mathf.FloorToInt(Time.realtimeSinceStartup * 1000f).ToString() + "u";
		}
		return NetworkSystem.Instance.ServerTimestamp.ToString();
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0006D134 File Offset: 0x0006B334
	internal static string ExtractFirstExternalCaller(string stackTrace)
	{
		if (string.IsNullOrEmpty(stackTrace))
		{
			return "(unknown)";
		}
		int num;
		for (int i = 0; i < stackTrace.Length; i = num + 1)
		{
			num = stackTrace.IndexOf('\n', i);
			if (num < 0)
			{
				num = stackTrace.Length;
			}
			int num2 = num - i;
			if (num2 > 0 && stackTrace.IndexOf("GTFileLog", i, Math.Min(num2, 60), StringComparison.Ordinal) < 0)
			{
				return stackTrace.Substring(i, num2).Trim();
			}
		}
		return "(unknown)";
	}

	// Token: 0x04001903 RID: 6403
	private static readonly object _registryLock = new object();

	// Token: 0x04001904 RID: 6404
	private static Dictionary<string, GTFileLog.FLogInstance> _instances = new Dictionary<string, GTFileLog.FLogInstance>();

	// Token: 0x04001905 RID: 6405
	private static GTFileLog.FLogInstance _default;

	// Token: 0x04001906 RID: 6406
	[ThreadStatic]
	private static bool _inCallback;

	// Token: 0x0200032D RID: 813
	public sealed class FLogInstance
	{
		// Token: 0x0600142A RID: 5162 RVA: 0x0006D1C3 File Offset: 0x0006B3C3
		internal FLogInstance(string prefix)
		{
			this._prefix = prefix;
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x0600142B RID: 5163 RVA: 0x0006D1E0 File Offset: 0x0006B3E0
		internal bool IsActive
		{
			get
			{
				object @lock = this._lock;
				bool flag2;
				lock (@lock)
				{
					flag2 = this._writer != null;
				}
				return flag2;
			}
		}

		// Token: 0x0600142C RID: 5164 RVA: 0x0006D228 File Offset: 0x0006B428
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void Log(string msg)
		{
			this.WriteEntry("LOG", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x0600142D RID: 5165 RVA: 0x0006D23B File Offset: 0x0006B43B
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarning(string msg)
		{
			this.WriteEntry("WARN", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x0600142E RID: 5166 RVA: 0x0006D24E File Offset: 0x0006B44E
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogError(string msg)
		{
			this.WriteEntry("ERR", msg, StackTraceUtility.ExtractStackTrace());
		}

		// Token: 0x0600142F RID: 5167 RVA: 0x0006D261 File Offset: 0x0006B461
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogNoTrace(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
		}

		// Token: 0x06001430 RID: 5168 RVA: 0x0006D26F File Offset: 0x0006B46F
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogWarningNoTrace(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
		}

		// Token: 0x06001431 RID: 5169 RVA: 0x0006D27D File Offset: 0x0006B47D
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void LogErrorNoTrace(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
		}

		// Token: 0x06001432 RID: 5170 RVA: 0x0006D28B File Offset: 0x0006B48B
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLog(string msg)
		{
			this.WriteEntryNoTrace("LOG", msg);
			Debug.Log("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x06001433 RID: 5171 RVA: 0x0006D2B4 File Offset: 0x0006B4B4
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogWarning(string msg)
		{
			this.WriteEntryNoTrace("WARN", msg);
			Debug.LogWarning("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x06001434 RID: 5172 RVA: 0x0006D2DD File Offset: 0x0006B4DD
		[Conditional("BETA")]
		[Conditional("UNITY_EDITOR")]
		public void CLogError(string msg)
		{
			this.WriteEntryNoTrace("ERR", msg);
			Debug.LogError("[GT/FLog:" + this._prefix + "] " + msg);
		}

		// Token: 0x06001435 RID: 5173 RVA: 0x0006D308 File Offset: 0x0006B508
		internal void WriteEntryNoTrace(string level, string msg)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(null);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[] { "[", timestamp, "] [", level, "] ", msg }));
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		// Token: 0x06001436 RID: 5174 RVA: 0x0006D3D4 File Offset: 0x0006B5D4
		internal void WriteEntry(string level, string msg, string trace)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			object @lock = this._lock;
			lock (@lock)
			{
				this.EnsureWriter(trace);
				if (this._writer != null)
				{
					try
					{
						string timestamp = GTFileLog.GetTimestamp();
						this._writer.WriteLine(string.Concat(new string[] { "[", timestamp, "] [", level, "] ", msg, "\n- - - -" }));
						this._writer.WriteLine(trace);
						this._writer.WriteLine("");
					}
					catch (Exception ex)
					{
						Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Write failed: " + ex.Message);
						this.CloseWriter();
					}
				}
			}
		}

		// Token: 0x06001437 RID: 5175 RVA: 0x0006D4C4 File Offset: 0x0006B6C4
		private void EnsureWriter(string callerTrace)
		{
			if (this._writer != null || this._failed)
			{
				return;
			}
			if (ApplicationQuittingState.IsQuitting)
			{
				this._failed = true;
				return;
			}
			try
			{
				string persistentDataPath = Application.persistentDataPath;
				Directory.CreateDirectory(persistentDataPath);
				string text = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
				string text2 = "flog_" + this._prefix + "_" + text;
				string text3 = Path.Combine(persistentDataPath, text2 + ".log");
				for (int i = 1; i <= 10; i++)
				{
					try
					{
						this._writer = new StreamWriter(text3, true)
						{
							AutoFlush = true
						};
						break;
					}
					catch (IOException obj) when (i < 10)
					{
						text3 = Path.Combine(persistentDataPath, text2 + "_" + (i + 1).ToString() + ".log");
					}
				}
				if (this._writer == null)
				{
					throw new IOException("All 10 log file attempts failed due to sharing violations.");
				}
				this._writer.WriteLine(string.Format("--- {0} log started {1:u} ---", this._prefix, DateTime.UtcNow));
				this._writer.WriteLine("--- playerName: " + PlayerPrefs.GetString("playerName", "(unset)") + " ---");
				string text4 = ((callerTrace != null) ? GTFileLog.ExtractFirstExternalCaller(callerTrace) : "(no-trace)");
				Debug.Log(string.Concat(new string[] { "<color=orange><b>[GT/GTFileLog:", this._prefix, "]</b> Writing to \"", text3, "\". First caller: ", text4, "</color>" }));
				GTFileLog.FLogInstance.PruneOldFlogFiles(persistentDataPath);
			}
			catch (Exception ex)
			{
				this._failed = true;
				Debug.LogError("[GT/GTFileLog:" + this._prefix + "] Failed to create log file: " + ex.Message);
			}
		}

		// Token: 0x06001438 RID: 5176 RVA: 0x0006D6C0 File Offset: 0x0006B8C0
		private static void PruneOldFlogFiles(string dir)
		{
			try
			{
				string[] files = Directory.GetFiles(dir, "flog_*.log");
				if (files.Length > 10)
				{
					Array.Sort<string>(files, (string a, string b) => File.GetLastWriteTimeUtc(a).CompareTo(File.GetLastWriteTimeUtc(b)));
					int num = files.Length - 10;
					for (int i = 0; i < num; i++)
					{
						try
						{
							File.Delete(files[i]);
						}
						catch
						{
						}
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06001439 RID: 5177 RVA: 0x0006D74C File Offset: 0x0006B94C
		private void CloseWriter()
		{
			try
			{
				StreamWriter writer = this._writer;
				if (writer != null)
				{
					writer.Flush();
				}
				StreamWriter writer2 = this._writer;
				if (writer2 != null)
				{
					writer2.Dispose();
				}
			}
			catch
			{
			}
			this._writer = null;
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x0006D798 File Offset: 0x0006B998
		internal void Close()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				this.CloseWriter();
				this._failed = false;
			}
		}

		// Token: 0x04001907 RID: 6407
		private StreamWriter _writer;

		// Token: 0x04001908 RID: 6408
		private bool _failed;

		// Token: 0x04001909 RID: 6409
		private readonly object _lock = new object();

		// Token: 0x0400190A RID: 6410
		private readonly string _prefix;

		// Token: 0x0400190B RID: 6411
		private const string FilePrefix = "flog_";

		// Token: 0x0400190C RID: 6412
		private const int MaxFlogFiles = 10;
	}
}
