using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x0200109E RID: 4254
	public abstract class SceneBakeTask : MonoBehaviour
	{
		// Token: 0x17000A15 RID: 2581
		// (get) Token: 0x06006A00 RID: 27136 RVA: 0x002210C7 File Offset: 0x0021F2C7
		// (set) Token: 0x06006A01 RID: 27137 RVA: 0x002210CF File Offset: 0x0021F2CF
		public SceneBakeMode bakeMode
		{
			get
			{
				return this.m_bakeMode;
			}
			set
			{
				this.m_bakeMode = value;
			}
		}

		// Token: 0x17000A16 RID: 2582
		// (get) Token: 0x06006A02 RID: 27138 RVA: 0x002210D8 File Offset: 0x0021F2D8
		// (set) Token: 0x06006A03 RID: 27139 RVA: 0x002210E0 File Offset: 0x0021F2E0
		public virtual int callbackOrder
		{
			get
			{
				return this.m_callbackOrder;
			}
			set
			{
				this.m_callbackOrder = value;
			}
		}

		// Token: 0x17000A17 RID: 2583
		// (get) Token: 0x06006A04 RID: 27140 RVA: 0x002210E9 File Offset: 0x0021F2E9
		// (set) Token: 0x06006A05 RID: 27141 RVA: 0x002210F1 File Offset: 0x0021F2F1
		public bool runIfInactive
		{
			get
			{
				return this.m_runIfInactive;
			}
			set
			{
				this.m_runIfInactive = value;
			}
		}

		// Token: 0x06006A06 RID: 27142
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x06006A07 RID: 27143 RVA: 0x00002C2D File Offset: 0x00000E2D
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x040079A0 RID: 31136
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x040079A1 RID: 31137
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x040079A2 RID: 31138
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
