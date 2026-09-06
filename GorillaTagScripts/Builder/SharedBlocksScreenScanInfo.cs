using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001071 RID: 4209
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x060068F1 RID: 26865 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnUpPressed()
		{
		}

		// Token: 0x060068F2 RID: 26866 RVA: 0x00002C2D File Offset: 0x00000E2D
		public override void OnDownPressed()
		{
		}

		// Token: 0x060068F3 RID: 26867 RVA: 0x0021C55C File Offset: 0x0021A75C
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed(false);
		}

		// Token: 0x060068F4 RID: 26868 RVA: 0x0021C56A File Offset: 0x0021A76A
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x060068F5 RID: 26869 RVA: 0x0021C578 File Offset: 0x0021A778
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x04007864 RID: 30820
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
