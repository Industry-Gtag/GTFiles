using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200125E RID: 4702
	public abstract class BaseGuidedRefTargetMono : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x06007707 RID: 30471 RVA: 0x0013274D File Offset: 0x0013094D
		protected virtual void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06007708 RID: 30472 RVA: 0x002699C6 File Offset: 0x00267BC6
		protected virtual void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<BaseGuidedRefTargetMono>(this, true);
		}

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x06007709 RID: 30473 RVA: 0x002699CF File Offset: 0x00267BCF
		// (set) Token: 0x0600770A RID: 30474 RVA: 0x002699D7 File Offset: 0x00267BD7
		GuidedRefBasicTargetInfo IGuidedRefTargetMono.GRefTargetInfo
		{
			get
			{
				return this.guidedRefTargetInfo;
			}
			set
			{
				this.guidedRefTargetInfo = value;
			}
		}

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x0600770B RID: 30475 RVA: 0x0008409E File Offset: 0x0008229E
		Object IGuidedRefTargetMono.GuidedRefTargetObject
		{
			get
			{
				return this;
			}
		}

		// Token: 0x0600770C RID: 30476 RVA: 0x002699E0 File Offset: 0x00267BE0
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<BaseGuidedRefTargetMono>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x0600770E RID: 30478 RVA: 0x000874AD File Offset: 0x000856AD
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600770F RID: 30479 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040086B3 RID: 34483
		public GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
