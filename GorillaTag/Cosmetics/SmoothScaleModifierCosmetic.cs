using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200135C RID: 4956
	public class SmoothScaleModifierCosmetic : MonoBehaviour
	{
		// Token: 0x06007C3C RID: 31804 RVA: 0x00289815 File Offset: 0x00287A15
		private void Awake()
		{
			this.initialScale = this.objectPrefab.transform.localScale;
		}

		// Token: 0x06007C3D RID: 31805 RVA: 0x0028982D File Offset: 0x00287A2D
		private void OnEnable()
		{
			this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
		}

		// Token: 0x06007C3E RID: 31806 RVA: 0x00289838 File Offset: 0x00287A38
		private void Update()
		{
			switch (this.currentState)
			{
			case SmoothScaleModifierCosmetic.State.None:
			case SmoothScaleModifierCosmetic.State.Scaled:
				break;
			case SmoothScaleModifierCosmetic.State.Reset:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.initialScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.initialScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.initialScale;
					if (this.onReset != null)
					{
						this.onReset.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.None);
					return;
				}
				break;
			case SmoothScaleModifierCosmetic.State.Scaling:
				this.SmoothScale(this.objectPrefab.transform.localScale, this.targetScale);
				if (Vector3.Distance(this.objectPrefab.transform.localScale, this.targetScale) < 0.01f)
				{
					this.objectPrefab.transform.localScale = this.targetScale;
					if (this.onScaled != null)
					{
						this.onScaled.Invoke();
					}
					this.UpdateState(SmoothScaleModifierCosmetic.State.Scaled);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06007C3F RID: 31807 RVA: 0x00289943 File Offset: 0x00287B43
		private void SmoothScale(Vector3 initial, Vector3 target)
		{
			this.objectPrefab.transform.localScale = Vector3.MoveTowards(initial, target, this.speed * Time.deltaTime);
		}

		// Token: 0x06007C40 RID: 31808 RVA: 0x00289968 File Offset: 0x00287B68
		private void UpdateState(SmoothScaleModifierCosmetic.State newState)
		{
			this.currentState = newState;
		}

		// Token: 0x06007C41 RID: 31809 RVA: 0x00289971 File Offset: 0x00287B71
		public void TriggerScale()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Scaled)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Scaling);
			}
		}

		// Token: 0x06007C42 RID: 31810 RVA: 0x00289983 File Offset: 0x00287B83
		public void TriggerReset()
		{
			if (this.currentState != SmoothScaleModifierCosmetic.State.Reset)
			{
				this.UpdateState(SmoothScaleModifierCosmetic.State.Reset);
			}
		}

		// Token: 0x04008E90 RID: 36496
		[Tooltip("The GameObject to scale up or down. This should reference the cosmetic mesh or object you want to visually modify.")]
		[SerializeField]
		private GameObject objectPrefab;

		// Token: 0x04008E91 RID: 36497
		[Tooltip("The target scale applied when scaling is triggered.")]
		[SerializeField]
		private Vector3 targetScale = new Vector3(2f, 2f, 2f);

		// Token: 0x04008E92 RID: 36498
		[Tooltip("Speed at which the object scales toward its target or initial size")]
		[SerializeField]
		private float speed = 2f;

		// Token: 0x04008E93 RID: 36499
		[Tooltip("Invoked once when the object reaches the target scale.")]
		public UnityEvent onScaled;

		// Token: 0x04008E94 RID: 36500
		[Tooltip("Invoked once when the object returns to its initial scale.")]
		public UnityEvent onReset;

		// Token: 0x04008E95 RID: 36501
		private SmoothScaleModifierCosmetic.State currentState;

		// Token: 0x04008E96 RID: 36502
		private Vector3 initialScale;

		// Token: 0x0200135D RID: 4957
		private enum State
		{
			// Token: 0x04008E98 RID: 36504
			None,
			// Token: 0x04008E99 RID: 36505
			Reset,
			// Token: 0x04008E9A RID: 36506
			Scaling,
			// Token: 0x04008E9B RID: 36507
			Scaled
		}
	}
}
