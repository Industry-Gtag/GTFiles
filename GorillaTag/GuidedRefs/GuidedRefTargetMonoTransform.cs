using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126C RID: 4716
	public class GuidedRefTargetMonoTransform : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000B9B RID: 2971
		// (get) Token: 0x06007748 RID: 30536 RVA: 0x0026AC24 File Offset: 0x00268E24
		// (set) Token: 0x06007749 RID: 30537 RVA: 0x0026AC2C File Offset: 0x00268E2C
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

		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x0600774A RID: 30538 RVA: 0x000874AD File Offset: 0x000856AD
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.transform;
			}
		}

		// Token: 0x0600774B RID: 30539 RVA: 0x0013274D File Offset: 0x0013094D
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600774C RID: 30540 RVA: 0x0026AC35 File Offset: 0x00268E35
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoTransform>(this, true);
		}

		// Token: 0x0600774D RID: 30541 RVA: 0x0026AC3E File Offset: 0x00268E3E
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoTransform>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x0600774F RID: 30543 RVA: 0x000874AD File Offset: 0x000856AD
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06007750 RID: 30544 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040086D0 RID: 34512
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
