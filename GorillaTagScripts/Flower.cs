using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F83 RID: 3971
	public class Flower : MonoBehaviour
	{
		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06006281 RID: 25217 RVA: 0x001FB253 File Offset: 0x001F9453
		// (set) Token: 0x06006282 RID: 25218 RVA: 0x001FB25B File Offset: 0x001F945B
		public bool IsWatered { get; private set; }

		// Token: 0x06006283 RID: 25219 RVA: 0x001FB264 File Offset: 0x001F9464
		private void Awake()
		{
			this.shouldUpdateVisuals = true;
			this.anim = base.GetComponent<Animator>();
			this.timer = base.GetComponent<GorillaTimer>();
			this.perchPoint = base.GetComponent<BeePerchPoint>();
			this.timer.onTimerStopped.AddListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
			this.currentState = Flower.FlowerState.None;
			this.wateredFx = this.wateredFx.GetComponent<ParticleSystem>();
			this.IsWatered = false;
			this.meshRenderer = base.GetComponent<SkinnedMeshRenderer>();
			this.meshRenderer.enabled = false;
			this.anim.enabled = false;
		}

		// Token: 0x06006284 RID: 25220 RVA: 0x001FB2FB File Offset: 0x001F94FB
		private void OnDestroy()
		{
			this.timer.onTimerStopped.RemoveListener(new UnityAction<GorillaTimer>(this.HandleOnFlowerTimerEnded));
		}

		// Token: 0x06006285 RID: 25221 RVA: 0x001FB31C File Offset: 0x001F951C
		public void WaterFlower(bool isWatered = false)
		{
			this.IsWatered = isWatered;
			switch (this.currentState)
			{
			case Flower.FlowerState.None:
				this.UpdateFlowerState(Flower.FlowerState.Healthy, false, true);
				return;
			case Flower.FlowerState.Healthy:
				if (!isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, false, true);
					return;
				}
				break;
			case Flower.FlowerState.Middle:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Healthy, true, true);
					return;
				}
				this.UpdateFlowerState(Flower.FlowerState.Wilted, false, true);
				return;
			case Flower.FlowerState.Wilted:
				if (isWatered)
				{
					this.UpdateFlowerState(Flower.FlowerState.Middle, true, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006286 RID: 25222 RVA: 0x001FB38C File Offset: 0x001F958C
		public void UpdateFlowerState(Flower.FlowerState newState, bool isWatered = false, bool updateVisual = true)
		{
			if (FlowersManager.Instance.IsMine)
			{
				this.timer.RestartTimer();
			}
			this.ChangeState(newState);
			if (this.perchPoint)
			{
				this.perchPoint.enabled = this.currentState == Flower.FlowerState.Healthy;
			}
			if (updateVisual)
			{
				this.LocalUpdateFlowers(newState, isWatered);
			}
		}

		// Token: 0x06006287 RID: 25223 RVA: 0x001FB3E4 File Offset: 0x001F95E4
		private void LocalUpdateFlowers(Flower.FlowerState state, bool isWatered = false)
		{
			GameObject[] array = this.meshStates;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
			if (!this.shouldUpdateVisuals)
			{
				this.meshStates[(int)this.currentState].SetActive(true);
				return;
			}
			if (isWatered && this.wateredFx)
			{
				this.wateredFx.Play();
			}
			this.meshRenderer.enabled = true;
			this.anim.enabled = true;
			switch (state)
			{
			case Flower.FlowerState.Healthy:
				this.anim.SetTrigger(Flower.middle_to_healthy);
				return;
			case Flower.FlowerState.Middle:
				if (this.lastState == Flower.FlowerState.Wilted)
				{
					this.anim.SetTrigger(Flower.wilted_to_middle);
					return;
				}
				this.anim.SetTrigger(Flower.healthy_to_middle);
				return;
			case Flower.FlowerState.Wilted:
				this.anim.SetTrigger(Flower.middle_to_wilted);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006288 RID: 25224 RVA: 0x001FB4BD File Offset: 0x001F96BD
		private void HandleOnFlowerTimerEnded(GorillaTimer _timer)
		{
			if (!FlowersManager.Instance.IsMine)
			{
				return;
			}
			if (this.timer == _timer)
			{
				this.WaterFlower(false);
			}
		}

		// Token: 0x06006289 RID: 25225 RVA: 0x001FB4E1 File Offset: 0x001F96E1
		private void ChangeState(Flower.FlowerState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x001FB4F6 File Offset: 0x001F96F6
		public Flower.FlowerState GetCurrentState()
		{
			return this.currentState;
		}

		// Token: 0x0600628B RID: 25227 RVA: 0x001FB500 File Offset: 0x001F9700
		public void OnAnimationIsDone(int state)
		{
			if (this.meshRenderer.enabled)
			{
				for (int i = 0; i < this.meshStates.Length; i++)
				{
					bool flag = i == (int)this.currentState;
					this.meshStates[i].SetActive(flag);
				}
				this.anim.enabled = false;
				this.meshRenderer.enabled = false;
			}
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x001FB55D File Offset: 0x001F975D
		public void UpdateVisuals(bool enable)
		{
			this.shouldUpdateVisuals = enable;
			this.meshStatesGameObject.SetActive(enable);
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x001FB574 File Offset: 0x001F9774
		public void AnimCatch()
		{
			if (this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				this.OnAnimationIsDone(0);
			}
		}

		// Token: 0x04007140 RID: 28992
		private Animator anim;

		// Token: 0x04007141 RID: 28993
		private SkinnedMeshRenderer meshRenderer;

		// Token: 0x04007142 RID: 28994
		[HideInInspector]
		public GorillaTimer timer;

		// Token: 0x04007143 RID: 28995
		private BeePerchPoint perchPoint;

		// Token: 0x04007144 RID: 28996
		public ParticleSystem wateredFx;

		// Token: 0x04007145 RID: 28997
		public ParticleSystem sparkleFx;

		// Token: 0x04007146 RID: 28998
		public GameObject meshStatesGameObject;

		// Token: 0x04007147 RID: 28999
		public GameObject[] meshStates;

		// Token: 0x04007148 RID: 29000
		private static readonly int healthy_to_middle = Animator.StringToHash("healthy_to_middle");

		// Token: 0x04007149 RID: 29001
		private static readonly int middle_to_healthy = Animator.StringToHash("middle_to_healthy");

		// Token: 0x0400714A RID: 29002
		private static readonly int wilted_to_middle = Animator.StringToHash("wilted_to_middle");

		// Token: 0x0400714B RID: 29003
		private static readonly int middle_to_wilted = Animator.StringToHash("middle_to_wilted");

		// Token: 0x0400714C RID: 29004
		private Flower.FlowerState currentState;

		// Token: 0x0400714D RID: 29005
		private string id;

		// Token: 0x0400714E RID: 29006
		private bool shouldUpdateVisuals;

		// Token: 0x0400714F RID: 29007
		private Flower.FlowerState lastState;

		// Token: 0x02000F84 RID: 3972
		public enum FlowerState
		{
			// Token: 0x04007152 RID: 29010
			None = -1,
			// Token: 0x04007153 RID: 29011
			Healthy,
			// Token: 0x04007154 RID: 29012
			Middle,
			// Token: 0x04007155 RID: 29013
			Wilted
		}
	}
}
