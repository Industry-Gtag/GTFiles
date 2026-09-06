using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02001070 RID: 4208
	public class SharedBlocksScreen : MonoBehaviour
	{
		// Token: 0x060068E8 RID: 26856 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnUpPressed()
		{
		}

		// Token: 0x060068E9 RID: 26857 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnDownPressed()
		{
		}

		// Token: 0x060068EA RID: 26858 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnSelectPressed()
		{
		}

		// Token: 0x060068EB RID: 26859 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnDeletePressed()
		{
		}

		// Token: 0x060068EC RID: 26860 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnNumberPressed(int number)
		{
		}

		// Token: 0x060068ED RID: 26861 RVA: 0x00002C2D File Offset: 0x00000E2D
		public virtual void OnLetterPressed(string letter)
		{
		}

		// Token: 0x060068EE RID: 26862 RVA: 0x0021C526 File Offset: 0x0021A726
		public virtual void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x060068EF RID: 26863 RVA: 0x0021C541 File Offset: 0x0021A741
		public virtual void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x04007862 RID: 30818
		public SharedBlocksTerminal.ScreenType screenType;

		// Token: 0x04007863 RID: 30819
		public SharedBlocksTerminal terminal;
	}
}
