using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010EC RID: 4332
	public class GorillaTextManager : MonoBehaviourPostTick
	{
		// Token: 0x06006CB3 RID: 27827 RVA: 0x0023251E File Offset: 0x0023071E
		public static void RegisterText(GorillaText text)
		{
			if (GorillaTextManager.instance == null)
			{
				GorillaTextManager.CreateManager();
			}
			if (!GorillaTextManager.instance.gorillaTexts.Contains(text))
			{
				GorillaTextManager.instance.gorillaTexts.Add(text);
			}
		}

		// Token: 0x06006CB4 RID: 27828 RVA: 0x00232554 File Offset: 0x00230754
		private void Awake()
		{
			if (GorillaTextManager.instance != null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			GorillaTextManager.instance = this;
		}

		// Token: 0x06006CB5 RID: 27829 RVA: 0x00232578 File Offset: 0x00230778
		public override void PostTick()
		{
			for (int i = 0; i < this.gorillaTexts.Count; i++)
			{
				this.gorillaTexts[i].InvokeIfUpdated();
			}
		}

		// Token: 0x06006CB6 RID: 27830 RVA: 0x002325AC File Offset: 0x002307AC
		public static void CreateManager()
		{
			GorillaTextManager gorillaTextManager = new GameObject("GorillaTextManager").AddComponent<GorillaTextManager>();
			gorillaTextManager.gorillaTexts = new List<GorillaText>();
			GorillaTextManager.instance = gorillaTextManager;
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gorillaTextManager);
			}
		}

		// Token: 0x04007D11 RID: 32017
		public static GorillaTextManager instance;

		// Token: 0x04007D12 RID: 32018
		public List<GorillaText> gorillaTexts = new List<GorillaText>();
	}
}
