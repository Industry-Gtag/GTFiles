using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F8E RID: 3982
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x060062FF RID: 25343 RVA: 0x001FDDF7 File Offset: 0x001FBFF7
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x06006300 RID: 25344 RVA: 0x001FDDFE File Offset: 0x001FBFFE
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x06006301 RID: 25345 RVA: 0x001FDE14 File Offset: 0x001FC014
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
				where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
				select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x040071B2 RID: 29106
		public Material SharedBase;

		// Token: 0x040071B3 RID: 29107
		public Texture2D CrystalAlbedo;

		// Token: 0x040071B4 RID: 29108
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x040071B5 RID: 29109
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x040071B6 RID: 29110
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x040071B7 RID: 29111
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x040071B8 RID: 29112
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x040071B9 RID: 29113
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x040071BA RID: 29114
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x040071BB RID: 29115
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x040071BC RID: 29116
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x040071BD RID: 29117
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x040071BE RID: 29118
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x040071BF RID: 29119
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x040071C0 RID: 29120
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x040071C1 RID: 29121
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x040071C2 RID: 29122
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x02000F8F RID: 3983
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x040071C3 RID: 29123
			public Material keyMaterial;

			// Token: 0x040071C4 RID: 29124
			public CrystalVisualsPreset visualPreset;

			// Token: 0x040071C5 RID: 29125
			[Space]
			public int low;

			// Token: 0x040071C6 RID: 29126
			public int mid;

			// Token: 0x040071C7 RID: 29127
			public int high;
		}
	}
}
