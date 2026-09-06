using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200121F RID: 4639
	[Serializable]
	public class CoolDownHelper
	{
		// Token: 0x06007599 RID: 30105 RVA: 0x00264977 File Offset: 0x00262B77
		public CoolDownHelper()
		{
			this.coolDown = 1f;
			this.checkTime = 0f;
		}

		// Token: 0x0600759A RID: 30106 RVA: 0x00264995 File Offset: 0x00262B95
		public CoolDownHelper(float cd)
		{
			this.coolDown = cd;
			this.checkTime = 0f;
		}

		// Token: 0x0600759B RID: 30107 RVA: 0x002649B0 File Offset: 0x00262BB0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool CheckCooldown()
		{
			float unscaledTime = Time.unscaledTime;
			if (unscaledTime < this.checkTime)
			{
				return false;
			}
			this.OnCheckPass();
			this.checkTime = unscaledTime + this.coolDown;
			return true;
		}

		// Token: 0x0600759C RID: 30108 RVA: 0x002649E3 File Offset: 0x00262BE3
		public virtual void Start()
		{
			this.checkTime = Time.unscaledTime + this.coolDown;
		}

		// Token: 0x0600759D RID: 30109 RVA: 0x002649F7 File Offset: 0x00262BF7
		public virtual void Stop()
		{
			this.checkTime = float.MaxValue;
		}

		// Token: 0x0600759E RID: 30110 RVA: 0x00002C2D File Offset: 0x00000E2D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public virtual void OnCheckPass()
		{
		}

		// Token: 0x04008586 RID: 34182
		public float coolDown;

		// Token: 0x04008587 RID: 34183
		[NonSerialized]
		public float checkTime;
	}
}
