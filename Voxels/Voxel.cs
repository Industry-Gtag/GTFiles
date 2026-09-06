using System;

namespace Voxels
{
	// Token: 0x020013BD RID: 5053
	[Serializable]
	public struct Voxel
	{
		// Token: 0x06007E3C RID: 32316 RVA: 0x00295D6E File Offset: 0x00293F6E
		public Voxel(byte material, byte density)
		{
			this.Material = material;
			this.Density = density;
		}

		// Token: 0x040090E2 RID: 37090
		public byte Material;

		// Token: 0x040090E3 RID: 37091
		public byte Density;
	}
}
