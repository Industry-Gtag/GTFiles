using System;
using UnityEngine;

namespace GorillaTag.GuidedRefs
{
	// Token: 0x0200126B RID: 4715
	public class GuidedRefTargetMonoGameObject : MonoBehaviour, IGuidedRefTargetMono, IGuidedRefMonoBehaviour, IGuidedRefObject
	{
		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x0600773F RID: 30527 RVA: 0x0026ABF6 File Offset: 0x00268DF6
		// (set) Token: 0x06007740 RID: 30528 RVA: 0x0026ABFE File Offset: 0x00268DFE
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

		// Token: 0x17000B9A RID: 2970
		// (get) Token: 0x06007741 RID: 30529 RVA: 0x000066D3 File Offset: 0x000048D3
		public Object GuidedRefTargetObject
		{
			get
			{
				return base.gameObject;
			}
		}

		// Token: 0x06007742 RID: 30530 RVA: 0x0013274D File Offset: 0x0013094D
		protected void Awake()
		{
			((IGuidedRefObject)this).GuidedRefInitialize();
		}

		// Token: 0x06007743 RID: 30531 RVA: 0x0026AC07 File Offset: 0x00268E07
		protected void OnDestroy()
		{
			GuidedRefHub.UnregisterTarget<GuidedRefTargetMonoGameObject>(this, true);
		}

		// Token: 0x06007744 RID: 30532 RVA: 0x0026AC10 File Offset: 0x00268E10
		void IGuidedRefObject.GuidedRefInitialize()
		{
			GuidedRefHub.RegisterTarget<GuidedRefTargetMonoGameObject>(this, this.guidedRefTargetInfo.hubIds, this);
		}

		// Token: 0x06007746 RID: 30534 RVA: 0x000874AD File Offset: 0x000856AD
		Transform IGuidedRefMonoBehaviour.get_transform()
		{
			return base.transform;
		}

		// Token: 0x06007747 RID: 30535 RVA: 0x00019405 File Offset: 0x00017605
		int IGuidedRefObject.GetInstanceID()
		{
			return base.GetInstanceID();
		}

		// Token: 0x040086CF RID: 34511
		[SerializeField]
		private GuidedRefBasicTargetInfo guidedRefTargetInfo;
	}
}
