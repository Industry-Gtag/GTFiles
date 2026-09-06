using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001340 RID: 4928
	public class GlowBugsInJar : MonoBehaviour
	{
		// Token: 0x06007B9B RID: 31643 RVA: 0x00286224 File Offset: 0x00284424
		private void OnEnable()
		{
			this.shakeStarted = false;
			this.UpdateGlow(0f);
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnShakeEvent;
			}
		}

		// Token: 0x06007B9C RID: 31644 RVA: 0x002862FC File Offset: 0x002844FC
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06007B9D RID: 31645 RVA: 0x0028634C File Offset: 0x0028454C
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "OnShakeEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args != null && args.Length == 1)
			{
				object obj = args[0];
				if (obj is bool)
				{
					bool flag = (bool)obj;
					if (flag)
					{
						this.ShakeStartLocal();
						return;
					}
					this.ShakeEndLocal();
					return;
				}
			}
		}

		// Token: 0x06007B9E RID: 31646 RVA: 0x002863AC File Offset: 0x002845AC
		public void HandleOnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { true });
			}
			this.ShakeStartLocal();
		}

		// Token: 0x06007B9F RID: 31647 RVA: 0x00286406 File Offset: 0x00284606
		private void ShakeStartLocal()
		{
			this.currentGlowAmount = 0f;
			this.shakeStarted = true;
			this.shakeTimer = 0f;
		}

		// Token: 0x06007BA0 RID: 31648 RVA: 0x00286428 File Offset: 0x00284628
		public void HandleOnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[] { false });
			}
			this.ShakeEndLocal();
		}

		// Token: 0x06007BA1 RID: 31649 RVA: 0x00286482 File Offset: 0x00284682
		private void ShakeEndLocal()
		{
			this.shakeStarted = false;
			this.shakeTimer = 0f;
		}

		// Token: 0x06007BA2 RID: 31650 RVA: 0x00286498 File Offset: 0x00284698
		public void Update()
		{
			if (this.shakeStarted)
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount < 1f)
				{
					this.currentGlowAmount += this.glowIncreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
					return;
				}
			}
			else
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount > 0f)
				{
					this.currentGlowAmount -= this.glowDecreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
				}
			}
		}

		// Token: 0x06007BA3 RID: 31651 RVA: 0x00286564 File Offset: 0x00284764
		private void UpdateGlow(float value)
		{
			if (this.renderers.Length != 0)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Material material = this.renderers[i].material;
					Color color = material.GetColor(this.shaderProperty);
					color.a = value;
					material.SetColor(this.shaderProperty, color);
					material.EnableKeyword("_EMISSION");
				}
			}
		}

		// Token: 0x04008D9E RID: 36254
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04008D9F RID: 36255
		[Space]
		[Tooltip("Time interval - every X seconds update the glow value")]
		[SerializeField]
		private float glowUpdateInterval = 2f;

		// Token: 0x04008DA0 RID: 36256
		[Tooltip("step increment - increase the glow value one step for N amount")]
		[SerializeField]
		private float glowIncreaseStepAmount = 0.1f;

		// Token: 0x04008DA1 RID: 36257
		[Tooltip("step decrement - decrease the glow value one step for N amount")]
		[SerializeField]
		private float glowDecreaseStepAmount = 0.2f;

		// Token: 0x04008DA2 RID: 36258
		[Space]
		[SerializeField]
		private string shaderProperty = "_EmissionColor";

		// Token: 0x04008DA3 RID: 36259
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04008DA4 RID: 36260
		private bool shakeStarted = true;

		// Token: 0x04008DA5 RID: 36261
		private static int EmissionColor;

		// Token: 0x04008DA6 RID: 36262
		private float currentGlowAmount;

		// Token: 0x04008DA7 RID: 36263
		private float shakeTimer;

		// Token: 0x04008DA8 RID: 36264
		private RubberDuckEvents _events;

		// Token: 0x04008DA9 RID: 36265
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
