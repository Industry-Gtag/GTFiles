using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012DE RID: 4830
	public class AOEReceiver : MonoBehaviour
	{
		// Token: 0x0600790A RID: 30986 RVA: 0x002773FB File Offset: 0x002755FB
		public void ReceiveAOE(in AOEReceiver.AOEContext AOEContext)
		{
			if (!this.enabledForAOE)
			{
				return;
			}
			AOEContextEvent onAOEReceived = this.OnAOEReceived;
			if (onAOEReceived == null)
			{
				return;
			}
			onAOEReceived.Invoke(AOEContext);
		}

		// Token: 0x04008A06 RID: 35334
		public AOEContextEvent OnAOEReceived;

		// Token: 0x04008A07 RID: 35335
		[Tooltip("Quick toggle to disable receiving without disabling the GameObject.")]
		[SerializeField]
		private bool enabledForAOE = true;

		// Token: 0x020012DF RID: 4831
		[Serializable]
		public struct AOEContext
		{
			// Token: 0x04008A08 RID: 35336
			public Vector3 origin;

			// Token: 0x04008A09 RID: 35337
			public float radius;

			// Token: 0x04008A0A RID: 35338
			public GameObject instigator;

			// Token: 0x04008A0B RID: 35339
			public float baseStrength;

			// Token: 0x04008A0C RID: 35340
			public float finalStrength;

			// Token: 0x04008A0D RID: 35341
			public float distance;

			// Token: 0x04008A0E RID: 35342
			public float normalizedDistance;
		}
	}
}
