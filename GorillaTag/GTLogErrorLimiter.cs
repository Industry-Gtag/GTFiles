using System;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Text;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x020011F9 RID: 4601
	public class GTLogErrorLimiter
	{
		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x060074BD RID: 29885 RVA: 0x0025E94C File Offset: 0x0025CB4C
		// (set) Token: 0x060074BE RID: 29886 RVA: 0x0025E954 File Offset: 0x0025CB54
		public string baseMessage
		{
			get
			{
				return this._baseMessage;
			}
			set
			{
				this._baseMessage = value ?? "__NULL__";
			}
		}

		// Token: 0x060074BF RID: 29887 RVA: 0x0025E966 File Offset: 0x0025CB66
		public GTLogErrorLimiter(string baseMessage, int countdown = 10, string occurrencesJoinString = "\n- ")
		{
			this.baseMessage = baseMessage;
			this.countdown = countdown;
			this.sb = ZString.CreateStringBuilder();
			this.sb.Append(this.baseMessage);
			this.occurrencesJoinString = occurrencesJoinString;
		}

		// Token: 0x060074C0 RID: 29888 RVA: 0x0025E9A0 File Offset: 0x0025CBA0
		public void Log(string subMessage = "", Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.countdown < 0)
			{
				return;
			}
			if (this.countdown == 0)
			{
				this.sb.Insert(0, "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n");
			}
			this.sb.Append(subMessage ?? "__NULL__");
			this.sb.Append("\n\nError origin - Caller: ");
			this.sb.Append(caller ?? "__NULL__");
			this.sb.Append(", Line: ");
			this.sb.Append(line);
			this.sb.Append("File: ");
			this.sb.Append(sourceFilePath ?? "__NULL__");
			Debug.LogError(this.sb.ToString(), context);
			this.sb.Clear();
			this.sb.Append(this.baseMessage);
			this.countdown--;
			this.occurrenceCount = 0;
		}

		// Token: 0x060074C1 RID: 29889 RVA: 0x0025EA95 File Offset: 0x0025CC95
		public void Log(Object obj, Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (!obj)
			{
				this.Log("__NULL__", context, caller, sourceFilePath, line);
				return;
			}
			this.Log(obj.ToString(), null, "Log", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\GTLogErrorLimiter.cs", 137);
		}

		// Token: 0x060074C2 RID: 29890 RVA: 0x0025EACD File Offset: 0x0025CCCD
		public void AddOccurrence(string s)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append(s);
		}

		// Token: 0x060074C3 RID: 29891 RVA: 0x0025EB03 File Offset: 0x0025CD03
		public void AddOccurrence(StringBuilder stringBuilder)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append<StringBuilder>(stringBuilder);
		}

		// Token: 0x060074C4 RID: 29892 RVA: 0x0025EB3C File Offset: 0x0025CD3C
		public void AddOccurence(GameObject gObj)
		{
			this.occurrenceCount++;
			if (gObj == null)
			{
				this.AddOccurrence("__NULL__");
				return;
			}
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Q(gObj.GetPath());
		}

		// Token: 0x060074C5 RID: 29893 RVA: 0x0025EB98 File Offset: 0x0025CD98
		public void LogOccurrences(Component component = null, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.Log("\"" + component.GetComponentPath(int.MaxValue) + "\"", obj, caller, sourceFilePath, line);
		}

		// Token: 0x060074C6 RID: 29894 RVA: 0x0025EBF8 File Offset: 0x0025CDF8
		public void LogOccurrences(Utf16ValueStringBuilder subMessage, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.sb.Append<Utf16ValueStringBuilder>(subMessage);
			this.Log("", obj, caller, sourceFilePath, line);
		}

		// Token: 0x0400846D RID: 33901
		private const string __NULL__ = "__NULL__";

		// Token: 0x0400846E RID: 33902
		public int countdown;

		// Token: 0x0400846F RID: 33903
		public int occurrenceCount;

		// Token: 0x04008470 RID: 33904
		public string occurrencesJoinString;

		// Token: 0x04008471 RID: 33905
		private string _baseMessage;

		// Token: 0x04008472 RID: 33906
		public Utf16ValueStringBuilder sb;

		// Token: 0x04008473 RID: 33907
		private const string k_lastMsgHeader = "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n";
	}
}
