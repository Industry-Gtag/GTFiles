using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02001184 RID: 4484
	public class TagEffectTester : MonoBehaviour, IHandEffectsTrigger
	{
		// Token: 0x17000ACD RID: 2765
		// (get) Token: 0x0600705C RID: 28764 RVA: 0x00243863 File Offset: 0x00241A63
		public bool Static
		{
			get
			{
				return this.isStatic;
			}
		}

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x0600705D RID: 28765 RVA: 0x0024386B File Offset: 0x00241A6B
		public IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x0600705E RID: 28766 RVA: 0x00243873 File Offset: 0x00241A73
		public Transform Transform { get; }

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x0600705F RID: 28767 RVA: 0x00036275 File Offset: 0x00034475
		public VRRig Rig
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06007060 RID: 28768 RVA: 0x0024387B File Offset: 0x00241A7B
		public bool FingersDown { get; }

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06007061 RID: 28769 RVA: 0x00243883 File Offset: 0x00241A83
		public bool FingersUp { get; }

		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06007062 RID: 28770 RVA: 0x0024388B File Offset: 0x00241A8B
		public Vector3 Velocity { get; }

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06007063 RID: 28771 RVA: 0x00243893 File Offset: 0x00241A93
		// (set) Token: 0x06007064 RID: 28772 RVA: 0x0024389B File Offset: 0x00241A9B
		public Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06007065 RID: 28773 RVA: 0x002438A4 File Offset: 0x00241AA4
		public bool RightHand { get; }

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06007066 RID: 28774 RVA: 0x002438AC File Offset: 0x00241AAC
		public float Magnitude { get; }

		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x06007067 RID: 28775 RVA: 0x002438B4 File Offset: 0x00241AB4
		public TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x06007068 RID: 28776 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnTriggerEntered(IHandEffectsTrigger other)
		{
		}

		// Token: 0x06007069 RID: 28777 RVA: 0x00002076 File Offset: 0x00000276
		public bool InTriggerZone(IHandEffectsTrigger t)
		{
			return false;
		}

		// Token: 0x04008054 RID: 32852
		[SerializeField]
		private bool isStatic = true;
	}
}
