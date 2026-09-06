using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F4C RID: 3916
	public class WhackAMoleManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006034 RID: 24628 RVA: 0x001E85D2 File Offset: 0x001E67D2
		private void Awake()
		{
			WhackAMoleManager.instance = this;
			this.allGames.Clear();
		}

		// Token: 0x06006035 RID: 24629 RVA: 0x00019260 File Offset: 0x00017460
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006036 RID: 24630 RVA: 0x00019269 File Offset: 0x00017469
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006037 RID: 24631 RVA: 0x001E85E8 File Offset: 0x001E67E8
		public void SliceUpdate()
		{
			foreach (WhackAMole whackAMole in this.allGames)
			{
				whackAMole.InvokeUpdate();
			}
		}

		// Token: 0x06006038 RID: 24632 RVA: 0x001E8638 File Offset: 0x001E6838
		private void OnDestroy()
		{
			WhackAMoleManager.instance = null;
		}

		// Token: 0x06006039 RID: 24633 RVA: 0x001E8640 File Offset: 0x001E6840
		public void Register(WhackAMole whackAMole)
		{
			this.allGames.Add(whackAMole);
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x001E864F File Offset: 0x001E684F
		public void Unregister(WhackAMole whackAMole)
		{
			this.allGames.Remove(whackAMole);
		}

		// Token: 0x04006ED8 RID: 28376
		public static WhackAMoleManager instance;

		// Token: 0x04006ED9 RID: 28377
		public HashSet<WhackAMole> allGames = new HashSet<WhackAMole>();
	}
}
