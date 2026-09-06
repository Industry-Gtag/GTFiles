using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class ChestObjectHysteresis : MonoBehaviour, ISpawnable
{
	// Token: 0x1700038D RID: 909
	// (get) Token: 0x060020DB RID: 8411 RVA: 0x000B071B File Offset: 0x000AE91B
	// (set) Token: 0x060020DC RID: 8412 RVA: 0x000B0723 File Offset: 0x000AE923
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x1700038E RID: 910
	// (get) Token: 0x060020DD RID: 8413 RVA: 0x000B072C File Offset: 0x000AE92C
	// (set) Token: 0x060020DE RID: 8414 RVA: 0x000B0734 File Offset: 0x000AE934
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060020DF RID: 8415 RVA: 0x000B0740 File Offset: 0x000AE940
	void ISpawnable.OnSpawn(VRRig rig)
	{
		if (!this.angleFollower && (string.IsNullOrEmpty(this.angleFollower_path) || base.transform.TryFindByPath(this.angleFollower_path, out this.angleFollower, false)))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"ChestObjectHysteresis: DEACTIVATING! Could not find `angleFollower` using path: \"",
				this.angleFollower_path,
				"\". For component at: \"",
				this.GetComponentPath(int.MaxValue),
				"\""
			}), this);
			base.gameObject.SetActive(false);
			return;
		}
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000B07CE File Offset: 0x000AE9CE
	private void Start()
	{
		this.lastAngleQuat = base.transform.rotation;
		this.currentAngleQuat = base.transform.rotation;
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000B07F2 File Offset: 0x000AE9F2
	private void OnEnable()
	{
		ChestObjectHysteresisManager.RegisterCH(this);
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000B07FA File Offset: 0x000AE9FA
	private void OnDisable()
	{
		ChestObjectHysteresisManager.UnregisterCH(this);
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000B0804 File Offset: 0x000AEA04
	public void InvokeUpdate()
	{
		this.currentAngleQuat = this.angleFollower.rotation;
		this.angleBetween = Quaternion.Angle(this.currentAngleQuat, this.lastAngleQuat);
		if (this.angleBetween > this.angleHysteresis)
		{
			base.transform.rotation = Quaternion.Slerp(this.currentAngleQuat, this.lastAngleQuat, this.angleHysteresis / this.angleBetween);
			this.lastAngleQuat = base.transform.rotation;
		}
		base.transform.rotation = this.lastAngleQuat;
	}

	// Token: 0x04002B9F RID: 11167
	public float angleHysteresis;

	// Token: 0x04002BA0 RID: 11168
	public float angleBetween;

	// Token: 0x04002BA1 RID: 11169
	public Transform angleFollower;

	// Token: 0x04002BA2 RID: 11170
	[Delayed]
	public string angleFollower_path;

	// Token: 0x04002BA3 RID: 11171
	private Quaternion lastAngleQuat;

	// Token: 0x04002BA4 RID: 11172
	private Quaternion currentAngleQuat;
}
