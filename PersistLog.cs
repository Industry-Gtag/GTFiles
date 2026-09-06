using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000492 RID: 1170
public class PersistLog : MonoBehaviour
{
	// Token: 0x06001C74 RID: 7284 RVA: 0x0009A308 File Offset: 0x00098508
	private async void OnEnable()
	{
		this.earlyQ = new List<ValueTuple<double, string, string>>();
		Application.logMessageReceived += this.LogMessageEnqueue;
		while (GorillaComputer.instance == null)
		{
			await Task.Yield();
		}
		string text = Application.persistentDataPath + Path.DirectorySeparatorChar.ToString();
		string text2 = text + "gt.log";
		string text3 = text + "gt-old.log";
		string text4 = text + "gt-older.log";
		try
		{
			if (File.Exists(text3))
			{
				File.Copy(text3, text4, true);
			}
		}
		catch (IOException)
		{
		}
		try
		{
			if (File.Exists(text2))
			{
				File.Copy(text2, text3, true);
			}
		}
		catch (IOException)
		{
		}
		string text5 = text2;
		for (int i = 1; i <= 10; i++)
		{
			try
			{
				PersistLog.sr = File.CreateText(text5);
				break;
			}
			catch (IOException obj) when (i < 10)
			{
				text5 = text + "gt_" + (i + 1).ToString() + ".log";
			}
		}
		if (PersistLog.sr == null)
		{
			Debug.LogError("[PersistLog] Failed to create log file after 10 attempts.");
			Application.logMessageReceived -= this.LogMessageEnqueue;
		}
		else
		{
			PersistLog.sr.Write(string.Format("{0:U}\r\n\r\n                           MONKE WUZ HERE!\r\n               _______    /\r\n              /       \\\r\n             /  _____  \\\r\n            / / _   _ \\ \\\r\n           [ | (O) (O) | ]\r\n            | \\  . .  / |\r\n     _______|  | _._ |  |_______\r\n    /        \\  \\___/  /        \\\r\n\r\nApp Id:        {1}\r\nApp Ver:       {2}\r\nPlatform:      {3}\r\nSys Lang:      {4}\r\nGC Version:    {5}\r\nGC Build Code: {6}\r\nGC Build Date: {7}\r\n\r\n", new object[]
			{
				DateTime.Now,
				Application.identifier,
				Application.version,
				Application.platform,
				Application.systemLanguage,
				GorillaComputer.instance.version,
				GorillaComputer.instance.buildCode,
				GorillaComputer.instance.buildDate
			}));
			Application.logMessageReceived -= this.LogMessageEnqueue;
			foreach (ValueTuple<double, string, string> valueTuple in this.earlyQ)
			{
				double item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				string item3 = valueTuple.Item3;
				PersistLog.sr.Write(string.Format("T+{0} >> {1}\n==========================\n{2}\n\n", item, item2, item3));
			}
			PersistLog.sr.Flush();
			Application.logMessageReceived += this.LogMessageReceived;
		}
	}

	// Token: 0x06001C75 RID: 7285 RVA: 0x0009A33F File Offset: 0x0009853F
	private void OnDisable()
	{
		this.OnDestroy();
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x0009A347 File Offset: 0x00098547
	private void OnDestroy()
	{
		Application.logMessageReceived -= this.LogMessageEnqueue;
		Application.logMessageReceived -= this.LogMessageReceived;
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Close();
		PersistLog.sr = null;
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x0009A383 File Offset: 0x00098583
	private void LogMessageEnqueue(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			this.earlyQ.Add(new ValueTuple<double, string, string>(Time.realtimeSinceStartupAsDouble, msg, strace));
		}
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x0009A3A8 File Offset: 0x000985A8
	private void LogMessageReceived(string msg, string strace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
		{
			if (this.plog == msg + strace)
			{
				if (!this.dup)
				{
					PersistLog.sr.Write(string.Format("T+{0} >> Duplicate log entry... Supressing further\n\n", Time.realtimeSinceStartupAsDouble));
					PersistLog.sr.Flush();
					this.dup = true;
				}
			}
			else
			{
				PersistLog.sr.Write(string.Format("T+{0} >> {1}\n==========================\n{2}\n\n", Time.realtimeSinceStartupAsDouble, msg, strace));
				PersistLog.sr.Flush();
				this.dup = false;
			}
		}
		this.plog = msg + strace;
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x0009A44B File Offset: 0x0009864B
	public static void Log(string msg)
	{
		PersistLog.Log(LogType.Log, msg);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x0009A454 File Offset: 0x00098654
	public static void Log(LogType type, string msg)
	{
		msg = string.Format("T+{0} >[DEV MSG]> {1}\n\n", Time.realtimeSinceStartupAsDouble, msg);
		Debug.unityLogger.Log(type, msg);
		if (PersistLog.sr == null)
		{
			return;
		}
		PersistLog.sr.Write(msg);
		PersistLog.sr.Flush();
	}

	// Token: 0x04002693 RID: 9875
	private static StreamWriter sr;

	// Token: 0x04002694 RID: 9876
	private string plog;

	// Token: 0x04002695 RID: 9877
	private bool dup;

	// Token: 0x04002696 RID: 9878
	[TupleElementNames(new string[] { "time", "msg", "strace" })]
	private List<ValueTuple<double, string, string>> earlyQ;
}
