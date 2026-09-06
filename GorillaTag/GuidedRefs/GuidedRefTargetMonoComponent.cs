using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126A RID: 4714
	public class GuidedRefTargetMonoComponent : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x06007736 RID: 30518 RVA: 0x0026ABC0 File Offset: 0x00268DC0
		// (set) Token: 0x06007737 RID: 30519 RVA: 0x0026ABC8 File Offset: 0x00268DC8
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

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x06007738 RID: 30520 RVA: 0x0026ABD1 File Offset: 0x00268DD1
		public Object GuidedRefTargetObject
		{
			get
			{
				return this.targetComponent;
			}
		}

		// Token: 0x06007739 RID: 30521 RVA: 0x0013274D File Offset: 0x0013094D
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x0600773A RID: 30522 RVA: 0x0026ABD9 File Offset: 0x00268DD9
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoComponent>(this, true);
		}

		// Token: 0x0600773B RID: 30523 RVA: 0x0026ABE2 File Offset: 0x00268DE2
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoComponent>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x0600773D RID: 30525 RVA: 0x000874AD File Offset: 0x000856AD
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x0600773E RID: 30526 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040086CD RID: 34509
		[SerializeField]
		private Component targetComponent;

		// Token: 0x040086CE RID: 34510
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
