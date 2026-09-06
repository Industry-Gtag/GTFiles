using System;
using UnityEngine;

namespace TagEffects
{
	// Token: 0x02001178 RID: 4472
	public interface IHandEffectsTrigger
	{
		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x06007028 RID: 28712
		IHandEffectsTrigger.Mode EffectMode { get; }

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x06007029 RID: 28713
		Transform Transform { get; }

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x0600702A RID: 28714
		VRRig Rig { get; }

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x0600702B RID: 28715
		bool FingersDown { get; }

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x0600702C RID: 28716
		bool FingersUp { get; }

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x0600702D RID: 28717
		Vector3 Velocity { get; }

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x0600702E RID: 28718
		// (set) Token: 0x0600702F RID: 28719
		Action<IHandEffectsTrigger.Mode> OnTrigger { get; set; }

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x06007030 RID: 28720
		bool RightHand { get; }

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x06007031 RID: 28721
		TagEffectPack CosmeticEffectPack { get; }

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x06007032 RID: 28722
		bool Static { get; }

		// Token: 0x06007033 RID: 28723
		void OnTriggerEntered(IHandEffectsTrigger other);

		// Token: 0x06007034 RID: 28724
		bool InTriggerZone(IHandEffectsTrigger t);

		// Token: 0x02001179 RID: 4473
		public enum Mode
		{
			// Token: 0x04008023 RID: 32803
			HighFive,
			// Token: 0x04008024 RID: 32804
			FistBump,
			// Token: 0x04008025 RID: 32805
			Tag3P,
			// Token: 0x04008026 RID: 32806
			Tag1P,
			// Token: 0x04008027 RID: 32807
			HighFive_And_FistBump
		}
	}
}
