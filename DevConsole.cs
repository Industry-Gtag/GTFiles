using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000317 RID: 791
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x170001FC RID: 508
	// (get) Token: 0x060013F2 RID: 5106 RVA: 0x0006C646 File Offset: 0x0006A846
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindAnyObjectByType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	// Token: 0x170001FD RID: 509
	// (get) Token: 0x060013F3 RID: 5107 RVA: 0x0006C664 File Offset: 0x0006A864
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x0006C670 File Offset: 0x0006A870
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x060013F5 RID: 5109 RVA: 0x00044B04 File Offset: 0x00042D04
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400188F RID: 6287
	private static DevConsole _instance;

	// Token: 0x04001890 RID: 6288
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04001891 RID: 6289
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001892 RID: 6290
	[SerializeField]
	private float maxHeight;

	// Token: 0x04001893 RID: 6291
	public static readonly string[] tracebackScrubbing = new string[] { "ExitGames.Client.Photon", "Photon.Realtime.LoadBalancingClient", "Photon.Pun.PhotonHandler" };

	// Token: 0x04001894 RID: 6292
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x04001895 RID: 6293
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x04001896 RID: 6294
	public int targetLogIndex = -1;

	// Token: 0x04001897 RID: 6295
	public int currentLogIndex;

	// Token: 0x04001898 RID: 6296
	public bool isMuted;

	// Token: 0x04001899 RID: 6297
	public float currentZoomLevel = 1f;

	// Token: 0x0400189A RID: 6298
	public List<GameObject> disableWhileActive;

	// Token: 0x0400189B RID: 6299
	public List<GameObject> enableWhileActive;

	// Token: 0x0400189C RID: 6300
	public int expandAmount = 20;

	// Token: 0x0400189D RID: 6301
	public int expandedMessageIndex = -1;

	// Token: 0x0400189E RID: 6302
	public bool canExpand = true;

	// Token: 0x0400189F RID: 6303
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x040018A0 RID: 6304
	public float lineStartHeight;

	// Token: 0x040018A1 RID: 6305
	public float textStartHeight;

	// Token: 0x040018A2 RID: 6306
	public float lineStartTextWidth;

	// Token: 0x040018A3 RID: 6307
	public double textScale = 0.5;

	// Token: 0x040018A4 RID: 6308
	public List<DevConsoleInstance> instances;

	// Token: 0x02000318 RID: 792
	[Serializable]
	public class LogEntry
	{
		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060013F8 RID: 5112 RVA: 0x0006C75A File Offset: 0x0006A95A
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		// Token: 0x060013F9 RID: 5113 RVA: 0x0006C788 File Offset: 0x0006A988
		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!DevConsole.tracebackScrubbing.Any((string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		// Token: 0x040018A5 RID: 6309
		private static int TotalIndex;

		// Token: 0x040018A6 RID: 6310
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x040018A7 RID: 6311
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x040018A8 RID: 6312
		public readonly string Trace;

		// Token: 0x040018A9 RID: 6313
		public bool forwarded;

		// Token: 0x040018AA RID: 6314
		public int repeatCount = 1;

		// Token: 0x040018AB RID: 6315
		public bool filtered;

		// Token: 0x040018AC RID: 6316
		public int index;
	}

	// Token: 0x0200031A RID: 794
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060013FC RID: 5116 RVA: 0x0006C842 File Offset: 0x0006AA42
		// (set) Token: 0x060013FD RID: 5117 RVA: 0x0006C84A File Offset: 0x0006AA4A
		public Type data { get; set; }

		// Token: 0x060013FE RID: 5118 RVA: 0x0006C854 File Offset: 0x0006AA54
		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		// Token: 0x040018AE RID: 6318
		public GorillaDevButton[] buttons;

		// Token: 0x040018AF RID: 6319
		public Text lineText;

		// Token: 0x040018B0 RID: 6320
		public RectTransform transform;

		// Token: 0x040018B1 RID: 6321
		public int targetMessage;

		// Token: 0x040018B2 RID: 6322
		public GorillaDevButton maximizeButton;

		// Token: 0x040018B3 RID: 6323
		public GorillaDevButton forwardButton;

		// Token: 0x040018B4 RID: 6324
		public SpriteRenderer backdrop;

		// Token: 0x040018B5 RID: 6325
		private bool expanded;

		// Token: 0x040018B6 RID: 6326
		public DevInspector inspector;
	}

	// Token: 0x0200031B RID: 795
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x060013FF RID: 5119 RVA: 0x0006C8D0 File Offset: 0x0006AAD0
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string text3 in array)
				{
					text2 = text2 + "    " + text3 + "\n";
				}
				string text4 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text4.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		// Token: 0x040018B8 RID: 6328
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x0200031C RID: 796
		[Serializable]
		public class Block
		{
			// Token: 0x06001401 RID: 5121 RVA: 0x0006CAA0 File Offset: 0x0006ACA0
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x040018B9 RID: 6329
			public string type;

			// Token: 0x040018BA RID: 6330
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x0200031D RID: 797
		[Serializable]
		public class TextBlock
		{
			// Token: 0x040018BB RID: 6331
			public string type;

			// Token: 0x040018BC RID: 6332
			public string text;
		}
	}
}
