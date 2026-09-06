using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001301 RID: 4865
	public class CloserCosmetic : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000BD2 RID: 3026
		// (get) Token: 0x06007A1E RID: 31262 RVA: 0x0027D764 File Offset: 0x0027B964
		// (set) Token: 0x06007A1F RID: 31263 RVA: 0x0027D76C File Offset: 0x0027B96C
		public bool TickRunning { get; set; }

		// Token: 0x06007A20 RID: 31264 RVA: 0x0027D778 File Offset: 0x0027B978
		private void OnEnable()
		{
			TickSystem<object>.AddCallbackTarget(this);
			this.localRotA = this.sideA.transform.localRotation;
			this.localRotB = this.sideB.transform.localRotation;
			this.fingerValue = 0f;
			this.UpdateState(CloserCosmetic.State.Opening);
		}

		// Token: 0x06007A21 RID: 31265 RVA: 0x00041CFB File Offset: 0x0003FEFB
		private void OnDisable()
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}

		// Token: 0x06007A22 RID: 31266 RVA: 0x0027D7CC File Offset: 0x0027B9CC
		public void Tick()
		{
			switch (this.currentState)
			{
			case CloserCosmetic.State.Closing:
				this.Closing();
				return;
			case CloserCosmetic.State.Opening:
				this.Opening();
				break;
			case CloserCosmetic.State.None:
				break;
			default:
				return;
			}
		}

		// Token: 0x06007A23 RID: 31267 RVA: 0x0027D800 File Offset: 0x0027BA00
		public void Close(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Closing);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06007A24 RID: 31268 RVA: 0x0027D810 File Offset: 0x0027BA10
		public void Open(bool leftHand, float fingerFlexValue)
		{
			this.UpdateState(CloserCosmetic.State.Opening);
			this.fingerValue = fingerFlexValue;
		}

		// Token: 0x06007A25 RID: 31269 RVA: 0x0027D820 File Offset: 0x0027BA20
		private void Closing()
		{
			float num = (this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f);
			Quaternion quaternion = Quaternion.Euler(this.maxRotationB);
			Quaternion quaternion2 = Quaternion.Slerp(this.localRotB, quaternion, num);
			this.sideB.transform.localRotation = quaternion2;
			Quaternion quaternion3 = Quaternion.Euler(this.maxRotationA);
			Quaternion quaternion4 = Quaternion.Slerp(this.localRotA, quaternion3, num);
			this.sideA.transform.localRotation = quaternion4;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion2) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion4) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06007A26 RID: 31270 RVA: 0x0027D8E4 File Offset: 0x0027BAE4
		private void Opening()
		{
			float num = (this.useFingerFlexValueAsStrength ? Mathf.Clamp01(this.fingerValue) : 1f);
			Quaternion quaternion = Quaternion.Slerp(this.sideB.transform.localRotation, this.localRotB, num);
			this.sideB.transform.localRotation = quaternion;
			Quaternion quaternion2 = Quaternion.Slerp(this.sideA.transform.localRotation, this.localRotA, num);
			this.sideA.transform.localRotation = quaternion2;
			if (Quaternion.Angle(this.sideB.transform.localRotation, quaternion) < 0.1f && Quaternion.Angle(this.sideA.transform.localRotation, quaternion2) < 0.1f)
			{
				this.UpdateState(CloserCosmetic.State.None);
			}
		}

		// Token: 0x06007A27 RID: 31271 RVA: 0x0027D9A9 File Offset: 0x0027BBA9
		private void UpdateState(CloserCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04008B89 RID: 35721
		[SerializeField]
		private GameObject sideA;

		// Token: 0x04008B8A RID: 35722
		[SerializeField]
		private GameObject sideB;

		// Token: 0x04008B8B RID: 35723
		[SerializeField]
		private Vector3 maxRotationA;

		// Token: 0x04008B8C RID: 35724
		[SerializeField]
		private Vector3 maxRotationB;

		// Token: 0x04008B8D RID: 35725
		[SerializeField]
		private bool useFingerFlexValueAsStrength;

		// Token: 0x04008B8E RID: 35726
		private Quaternion localRotA;

		// Token: 0x04008B8F RID: 35727
		private Quaternion localRotB;

		// Token: 0x04008B90 RID: 35728
		private CloserCosmetic.State currentState;

		// Token: 0x04008B91 RID: 35729
		private float fingerValue;

		// Token: 0x02001302 RID: 4866
		private enum State
		{
			// Token: 0x04008B94 RID: 35732
			Closing,
			// Token: 0x04008B95 RID: 35733
			Opening,
			// Token: 0x04008B96 RID: 35734
			None
		}
	}
}
