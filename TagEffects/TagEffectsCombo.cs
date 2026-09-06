using System;

namespace TagEffects
{
	// Token: 0x0200117F RID: 4479
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x0600704D RID: 28749 RVA: 0x0024373C File Offset: 0x0024193C
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x0600704E RID: 28750 RVA: 0x00243797 File Offset: 0x00241997
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x0600704F RID: 28751 RVA: 0x002437A5 File Offset: 0x002419A5
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x04008049 RID: 32841
		public TagEffectPack inputA;

		// Token: 0x0400804A RID: 32842
		public TagEffectPack inputB;
	}
}
