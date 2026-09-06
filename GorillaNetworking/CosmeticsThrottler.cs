using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010D4 RID: 4308
	public class CosmeticsThrottler : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006BA6 RID: 27558 RVA: 0x0022B194 File Offset: 0x00229394
		private void Awake()
		{
			this._cosmeticSlots = 16;
			VRRig[] allRigs = VRRigCache.Instance.GetAllRigs();
			this._rigHelpers = new GorillaRigHelper[allRigs.Length];
			for (int i = 0; i < allRigs.Length; i++)
			{
				this._rigHelpers[i] = new GorillaRigHelper
				{
					rig = allRigs[i],
					state = CosmeticsThrottler.RigDrawState.Startup,
					sqrDistance = 9999f,
					prevSqrDistance = 9999f
				};
			}
			RoomSystem.JoinedRoomEvent += new Action(this.UpdatePlayerCount);
			RoomSystem.LeftRoomEvent += new Action(this.UpdatePlayerCount);
		}

		// Token: 0x06006BA7 RID: 27559 RVA: 0x0022B248 File Offset: 0x00229448
		private void UpdatePlayerCount()
		{
			int num = NetworkSystem.Instance.AllNetPlayers.Length;
			if (num < this.ThrottlePlayerCountThreshold && this.lastPlayerCount >= this.ThrottlePlayerCountThreshold)
			{
				this.EnableAllRenderers();
			}
			this.lastPlayerCount = num;
		}

		// Token: 0x06006BA8 RID: 27560 RVA: 0x000DF973 File Offset: 0x000DDB73
		private void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}

		// Token: 0x06006BA9 RID: 27561 RVA: 0x000DF97B File Offset: 0x000DDB7B
		private void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}

		// Token: 0x06006BAA RID: 27562 RVA: 0x0022B288 File Offset: 0x00229488
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(base.transform.position, this.DrawAllDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.MaxDrawDistance);
		}

		// Token: 0x06006BAB RID: 27563 RVA: 0x0022B2D8 File Offset: 0x002294D8
		public void SliceUpdate()
		{
			if (this.lastPlayerCount < this.ThrottlePlayerCountThreshold)
			{
				return;
			}
			if (this.mainCamera == null)
			{
				this.mainCamera = Camera.main;
				return;
			}
			Vector3 position = base.transform.position;
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this._rigHelpers[i].prevSqrDistance = this._rigHelpers[i].sqrDistance;
				if (!this._rigHelpers[i].rig.isActiveAndEnabled || this._rigHelpers[i].rig.isLocal)
				{
					this._rigHelpers[i].sqrDistance = 9999f;
				}
				else
				{
					Vector3 position2 = this._rigHelpers[i].rig.transform.position;
					if (this.mainCamera.WorldToScreenPoint(position2).z <= 0f)
					{
						this._rigHelpers[i].sqrDistance = 9999f;
					}
					else
					{
						float sqrMagnitude = (position2 - position).sqrMagnitude;
						this._rigHelpers[i].sqrDistance = sqrMagnitude;
					}
				}
			}
			Array.Sort<GorillaRigHelper>(this._rigHelpers);
			float num = this.DrawAllDistance * this.DrawAllDistance;
			float num2 = this.MaxDrawDistance * this.MaxDrawDistance;
			for (int j = 0; j < this._rigHelpers.Length; j++)
			{
				if (this._rigHelpers[j].sqrDistance >= 9999f)
				{
					this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
				}
				else
				{
					if (this.DrawOnPlayerCount)
					{
						if (j < this.DrawAllCount)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
							goto IL_0293;
						}
						if (j >= this.DrawMaxCount)
						{
							this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
							goto IL_0293;
						}
					}
					if (this._rigHelpers[j].sqrDistance <= num)
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.All);
					}
					else if (this._rigHelpers[j].sqrDistance <= num2)
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Partial);
					}
					else
					{
						this._rigHelpers[j] = this.UpdateRigState(this._rigHelpers[j], CosmeticsThrottler.RigDrawState.Min);
					}
				}
				IL_0293:;
			}
		}

		// Token: 0x06006BAC RID: 27564 RVA: 0x0022B590 File Offset: 0x00229790
		private GorillaRigHelper UpdateRigState(GorillaRigHelper helper, CosmeticsThrottler.RigDrawState newState)
		{
			CosmeticsThrottler.RigDrawState state = helper.state;
			if (newState == state)
			{
				return helper;
			}
			switch (newState)
			{
			case CosmeticsThrottler.RigDrawState.All:
				if (state != CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRig(helper.rig, true);
					helper.rig.ToggleMatParticles(true);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Partial:
				if (state <= CosmeticsThrottler.RigDrawState.All)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, false, true);
					helper.rig.ToggleMatParticles(false);
				}
				else if (state == CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRigForSlots(helper.rig, true, false);
				}
				break;
			case CosmeticsThrottler.RigDrawState.Min:
				if (state != CosmeticsThrottler.RigDrawState.Min)
				{
					this.ToggleRenderersOnRig(helper.rig, false);
					helper.rig.ToggleMatParticles(false);
				}
				break;
			}
			helper.state = newState;
			return helper;
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x0022B638 File Offset: 0x00229838
		private void ToggleRenderersOnRig(VRRig rig, bool toggle)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleRenderers(toggle);
					cosmeticItemInstance.ToggleParticles(toggle);
				}
			}
		}

		// Token: 0x06006BAE RID: 27566 RVA: 0x0022B690 File Offset: 0x00229890
		private void ToggleRenderersOnRigForSlots(VRRig rig, bool toggle, bool includesSlots = true)
		{
			CosmeticsController.CosmeticSet cosmeticSet = rig.cosmeticSet;
			int num = cosmeticSet.items.Length;
			for (int i = 0; i < num; i++)
			{
				CosmeticItemInstance cosmeticItemInstance = rig.cosmeticsObjectRegistry.Cosmetic(cosmeticSet.items[i].displayName);
				if (cosmeticItemInstance != null)
				{
					cosmeticItemInstance.ToggleParticles(toggle);
					if (this.ContainsSlot(cosmeticItemInstance.ActiveSlot) == includesSlots)
					{
						cosmeticItemInstance.ToggleRenderers(toggle);
					}
				}
			}
		}

		// Token: 0x06006BAF RID: 27567 RVA: 0x0022B6F8 File Offset: 0x002298F8
		private bool ContainsSlot(CosmeticsController.CosmeticSlots slot)
		{
			for (int i = 0; i < this.ToggleSlots.Length; i++)
			{
				if (this.ToggleSlots[i] == slot)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06006BB0 RID: 27568 RVA: 0x0022B728 File Offset: 0x00229928
		public void EnableAllRenderers()
		{
			for (int i = 0; i < this._rigHelpers.Length; i++)
			{
				this.ToggleRenderersOnRig(this._rigHelpers[i].rig, true);
			}
		}

		// Token: 0x04007B1B RID: 31515
		public float DrawAllDistance = 5f;

		// Token: 0x04007B1C RID: 31516
		public float MaxDrawDistance = 10f;

		// Token: 0x04007B1D RID: 31517
		public bool DrawOnPlayerCount = true;

		// Token: 0x04007B1E RID: 31518
		public int DrawAllCount = 6;

		// Token: 0x04007B1F RID: 31519
		public int DrawMaxCount = 14;

		// Token: 0x04007B20 RID: 31520
		public int ThrottlePlayerCountThreshold = 11;

		// Token: 0x04007B21 RID: 31521
		private int lastPlayerCount;

		// Token: 0x04007B22 RID: 31522
		public CosmeticsController.CosmeticSlots[] ToggleSlots;

		// Token: 0x04007B23 RID: 31523
		[SerializeField]
		private GorillaRigHelper[] _rigHelpers;

		// Token: 0x04007B24 RID: 31524
		private int _cosmeticSlots;

		// Token: 0x04007B25 RID: 31525
		private float _update;

		// Token: 0x04007B26 RID: 31526
		private Camera mainCamera;

		// Token: 0x020010D5 RID: 4309
		public enum RigDrawState
		{
			// Token: 0x04007B28 RID: 31528
			All,
			// Token: 0x04007B29 RID: 31529
			Partial,
			// Token: 0x04007B2A RID: 31530
			Min,
			// Token: 0x04007B2B RID: 31531
			Startup = -1
		}
	}
}
