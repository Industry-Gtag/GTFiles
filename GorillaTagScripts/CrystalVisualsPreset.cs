using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F7C RID: 3964
	[CreateAssetMenu(fileName = "CrystalVisualsPreset", menuName = "ScriptableObjects/CrystalVisualsPreset", order = 0)]
	public class CrystalVisualsPreset : ScriptableObject
	{
		// Token: 0x0600624F RID: 25167 RVA: 0x001FA3B0 File Offset: 0x001F85B0
		public override int GetHashCode()
		{
			return new ValueTuple<CrystalVisualsPreset.VisualState, CrystalVisualsPreset.VisualState>(this.stateA, this.stateB).GetHashCode();
		}

		// Token: 0x06006250 RID: 25168 RVA: 0x00002C2D File Offset: 0x00000E2D
		[Conditional("UNITY_EDITOR")]
		private void Save()
		{
		}

		// Token: 0x04007117 RID: 28951
		public CrystalVisualsPreset.VisualState stateA;

		// Token: 0x04007118 RID: 28952
		public CrystalVisualsPreset.VisualState stateB;

		// Token: 0x02000F7D RID: 3965
		[Serializable]
		public struct VisualState
		{
			// Token: 0x06006252 RID: 25170 RVA: 0x001FA3DC File Offset: 0x001F85DC
			public override int GetHashCode()
			{
				int num = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.albedo);
				int num2 = CrystalVisualsPreset.VisualState.<GetHashCode>g__GetColorHash|2_0(this.emission);
				return new ValueTuple<int, int>(num, num2).GetHashCode();
			}

			// Token: 0x06006253 RID: 25171 RVA: 0x001FA414 File Offset: 0x001F8614
			[CompilerGenerated]
			internal static int <GetHashCode>g__GetColorHash|2_0(Color c)
			{
				return new ValueTuple<float, float, float>(c.r, c.g, c.b).GetHashCode();
			}

			// Token: 0x04007119 RID: 28953
			[ColorUsage(false, false)]
			public Color albedo;

			// Token: 0x0400711A RID: 28954
			[ColorUsage(false, false)]
			public Color emission;
		}
	}
}
