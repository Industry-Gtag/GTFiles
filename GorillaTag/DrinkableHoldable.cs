using System;
using emotitron.Compression;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200120A RID: 4618
	public class DrinkableHoldable : TransferrableObject
	{
		// Token: 0x06007505 RID: 29957 RVA: 0x0025FE84 File Offset: 0x0025E084
		internal override void OnEnable()
		{
			base.OnEnable();
			base.enabled = this.containerLiquid != null;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.sipSoundCooldown, this.containerLiquid.fillAmount, this.coolingDown);
			this.myByteArray = new byte[32];
		}

		// Token: 0x06007506 RID: 29958 RVA: 0x0025FED8 File Offset: 0x0025E0D8
		protected override void LateUpdateLocal()
		{
			if (!this.containerLiquid.isActiveAndEnabled || !GorillaParent.hasInstance || !GorillaComputer.hasInstance)
			{
				base.LateUpdateLocal();
				return;
			}
			float num = (float)((GorillaComputer.instance.startupMillis + (long)Time.realtimeSinceStartup * 1000L) % 259200000L) / 1000f;
			if (Mathf.Abs(num - this.lastTimeSipSoundPlayed) > 129600f)
			{
				this.lastTimeSipSoundPlayed = num;
			}
			float num2 = this.sipRadius * this.sipRadius;
			bool flag = (GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
			if (!flag)
			{
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (!rig.isOfflineVRRig)
					{
						if (flag || rig.head == null)
						{
							break;
						}
						if (rig.head.rigTarget.IsNull())
						{
							break;
						}
						flag = (rig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
					}
				}
			}
			if (flag)
			{
				this.containerLiquid.fillAmount = Mathf.Clamp01(this.containerLiquid.fillAmount - this.sipRate * Time.deltaTime);
				if (num > this.lastTimeSipSoundPlayed + this.sipSoundCooldown)
				{
					if (!this.wasSipping)
					{
						this.lastTimeSipSoundPlayed = num;
						this.coolingDown = true;
					}
				}
				else
				{
					this.coolingDown = false;
				}
			}
			this.wasSipping = flag;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.lastTimeSipSoundPlayed, this.containerLiquid.fillAmount, this.coolingDown);
			base.LateUpdateLocal();
		}

		// Token: 0x06007507 RID: 29959 RVA: 0x002600D0 File Offset: 0x0025E2D0
		protected override void LateUpdateReplicated()
		{
			base.LateUpdateReplicated();
			int itemState = (int)this.itemState;
			this.UnpackValuesNonstatic(in itemState, out this.lastTimeSipSoundPlayed, out this.containerLiquid.fillAmount, out this.coolingDown);
		}

		// Token: 0x06007508 RID: 29960 RVA: 0x00260109 File Offset: 0x0025E309
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (this.coolingDown && !this.wasCoolingDown)
			{
				this.sipSoundBankPlayer.Play();
			}
			this.wasCoolingDown = this.coolingDown;
		}

		// Token: 0x06007509 RID: 29961 RVA: 0x00260138 File Offset: 0x0025E338
		private static int PackValues(float cooldownStartTime, float fillAmount, bool coolingDown)
		{
			byte[] array = new byte[32];
			int num = 0;
			array.WriteBool(coolingDown, ref num);
			array.Write((ulong)((double)cooldownStartTime * 100.0), ref num, 25);
			array.Write((ulong)((double)fillAmount * 63.0), ref num, 6);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x0600750A RID: 29962 RVA: 0x0026018C File Offset: 0x0025E38C
		private void UnpackValuesNonstatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			DrinkableHoldable.GetBytes(packed, ref this.myByteArray);
			int num = 0;
			coolingDown = this.myByteArray.ReadBool(ref num);
			cooldownStartTime = (float)(this.myByteArray.Read(ref num, 25) / 100.0);
			fillAmount = this.myByteArray.Read(ref num, 6) / 63f;
		}

		// Token: 0x0600750B RID: 29963 RVA: 0x002601F0 File Offset: 0x0025E3F0
		public static void GetBytes(int value, ref byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)((value >> 8 * i) & 255);
			}
		}

		// Token: 0x0600750C RID: 29964 RVA: 0x00260220 File Offset: 0x0025E420
		private static void UnpackValuesStatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			byte[] bytes = BitConverter.GetBytes(packed);
			int num = 0;
			coolingDown = bytes.ReadBool(ref num);
			cooldownStartTime = (float)(bytes.Read(ref num, 25) / 100.0);
			fillAmount = bytes.Read(ref num, 6) / 63f;
		}

		// Token: 0x040084B9 RID: 33977
		[AssignInCorePrefab]
		public ContainerLiquid containerLiquid;

		// Token: 0x040084BA RID: 33978
		[AssignInCorePrefab]
		[SoundBankInfo]
		public SoundBankPlayer sipSoundBankPlayer;

		// Token: 0x040084BB RID: 33979
		[AssignInCorePrefab]
		public float sipRate = 0.1f;

		// Token: 0x040084BC RID: 33980
		[AssignInCorePrefab]
		public float sipSoundCooldown = 0.5f;

		// Token: 0x040084BD RID: 33981
		[AssignInCorePrefab]
		public Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x040084BE RID: 33982
		[AssignInCorePrefab]
		public float sipRadius = 0.15f;

		// Token: 0x040084BF RID: 33983
		private float lastTimeSipSoundPlayed;

		// Token: 0x040084C0 RID: 33984
		private bool wasSipping;

		// Token: 0x040084C1 RID: 33985
		private bool coolingDown;

		// Token: 0x040084C2 RID: 33986
		private bool wasCoolingDown;

		// Token: 0x040084C3 RID: 33987
		private byte[] myByteArray;
	}
}
