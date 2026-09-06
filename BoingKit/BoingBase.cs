using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200141D RID: 5149
	public class BoingBase : MonoBehaviour
	{
		// Token: 0x17000C73 RID: 3187
		// (get) Token: 0x060081DE RID: 33246 RVA: 0x002A70CA File Offset: 0x002A52CA
		public Version CurrentVersion
		{
			get
			{
				return this.m_currentVersion;
			}
		}

		// Token: 0x17000C74 RID: 3188
		// (get) Token: 0x060081DF RID: 33247 RVA: 0x002A70D2 File Offset: 0x002A52D2
		public Version PreviousVersion
		{
			get
			{
				return this.m_previousVersion;
			}
		}

		// Token: 0x17000C75 RID: 3189
		// (get) Token: 0x060081E0 RID: 33248 RVA: 0x002A70DA File Offset: 0x002A52DA
		public Version InitialVersion
		{
			get
			{
				return this.m_initialVersion;
			}
		}

		// Token: 0x060081E1 RID: 33249 RVA: 0x002A70E2 File Offset: 0x002A52E2
		protected virtual void OnUpgrade(Version oldVersion, Version newVersion)
		{
			this.m_previousVersion = this.m_currentVersion;
			if (this.m_currentVersion.Revision < 33)
			{
				this.m_initialVersion = Version.Invalid;
				this.m_previousVersion = Version.Invalid;
			}
			this.m_currentVersion = newVersion;
		}

		// Token: 0x040092B4 RID: 37556
		[SerializeField]
		private Version m_currentVersion;

		// Token: 0x040092B5 RID: 37557
		[SerializeField]
		private Version m_previousVersion;

		// Token: 0x040092B6 RID: 37558
		[SerializeField]
		private Version m_initialVersion = BoingKit.Version;
	}
}
