using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001359 RID: 4953
	[RequireComponent(typeof(NetworkedRandomProvider))]
	public class RandomWeightedOutput : MonoBehaviour
	{
		// Token: 0x06007C29 RID: 31785 RVA: 0x00288D4F File Offset: 0x00286F4F
		private void Awake()
		{
			if (this.networkProvider == null)
			{
				this.networkProvider = base.GetComponentInParent<NetworkedRandomProvider>();
			}
		}

		// Token: 0x06007C2A RID: 31786 RVA: 0x00288D6C File Offset: 0x00286F6C
		public void PickNextRandom()
		{
			int deterministicPickIndex = this.GetDeterministicPickIndex();
			if (deterministicPickIndex >= 0)
			{
				UnityEvent onPick = this.outputs[deterministicPickIndex].onPick;
				if (onPick != null)
				{
					onPick.Invoke();
				}
				UnityEvent<int> unityEvent = this.onAnyPick;
				if (unityEvent != null)
				{
					unityEvent.Invoke(deterministicPickIndex);
				}
				if (this.debugLog)
				{
					Debug.Log(string.Format("[RandomWeightedOutput] Picked '{0}' (idx={1})", this.outputs[deterministicPickIndex].name, deterministicPickIndex));
				}
			}
		}

		// Token: 0x06007C2B RID: 31787 RVA: 0x00288DE0 File Offset: 0x00286FE0
		private int GetDeterministicPickIndex()
		{
			if (this.networkProvider == null)
			{
				return -1;
			}
			List<int> list = new List<int>(this.outputs.Count);
			for (int i = 0; i < this.outputs.Count; i++)
			{
				RandomWeightedOutput.WeightedOutput weightedOutput = this.outputs[i];
				if (weightedOutput != null && weightedOutput.enabled && weightedOutput.weight > 0f)
				{
					list.Add(i);
				}
			}
			if (list.Count == 0)
			{
				return -1;
			}
			double num = 0.0;
			foreach (int num2 in list)
			{
				num += (double)this.outputs[num2].weight;
			}
			if (num <= 0.0)
			{
				return list[0];
			}
			double num3 = (double)this.networkProvider.GetSelectedAsFloat() * num;
			double num4 = 0.0;
			for (int j = 0; j < list.Count; j++)
			{
				int num5 = list[j];
				num4 += (double)this.outputs[num5].weight;
				if (num3 < num4)
				{
					return num5;
				}
			}
			List<int> list2 = list;
			return list2[list2.Count - 1];
		}

		// Token: 0x04008E61 RID: 36449
		[Header("Network Provider")]
		[Tooltip("For best result, pick Float01 or Double01 as the output mode in your NetworkedRandomProvider")]
		[SerializeField]
		private NetworkedRandomProvider networkProvider;

		// Token: 0x04008E62 RID: 36450
		[Header("Weighted Outputs")]
		[SerializeField]
		private List<RandomWeightedOutput.WeightedOutput> outputs = new List<RandomWeightedOutput.WeightedOutput>();

		// Token: 0x04008E63 RID: 36451
		[Header("Event")]
		[SerializeField]
		public UnityEvent<int> onAnyPick = new UnityEvent<int>();

		// Token: 0x04008E64 RID: 36452
		[SerializeField]
		private bool debugLog;

		// Token: 0x0200135A RID: 4954
		[Serializable]
		public class WeightedOutput
		{
			// Token: 0x04008E65 RID: 36453
			[SerializeField]
			public string name = "Event";

			// Token: 0x04008E66 RID: 36454
			[SerializeField]
			[Range(0f, 100f)]
			public float weight = 1f;

			// Token: 0x04008E67 RID: 36455
			[SerializeField]
			public bool enabled = true;

			// Token: 0x04008E68 RID: 36456
			[SerializeField]
			public UnityEvent onPick = new UnityEvent();
		}
	}
}
