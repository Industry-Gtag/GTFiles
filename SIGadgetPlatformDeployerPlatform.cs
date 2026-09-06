using System;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class SIGadgetPlatformDeployerPlatform : MonoBehaviour, ISIGameDeployable
{
	// Token: 0x0600068F RID: 1679 RVA: 0x00024908 File Offset: 0x00022B08
	public void ApplyUpgrades(SIUpgradeSet upgrades)
	{
		bool flag = upgrades.Contains(SIUpgradeType.Platform_Duration);
		float num = (flag ? this.extendedDuration : this.defaultDuration);
		this.timeToDie = Time.time + num;
		this.extendedDurationFrame.SetActive(flag);
		this.checkBounds = new Bounds(this.activeCollider.center, this.activeCollider.size);
		Vector3 size = this.checkBounds.size;
		Vector3 lossyScale = this.activeCollider.transform.lossyScale;
		size.x *= lossyScale.x;
		size.y *= lossyScale.y;
		size.z *= lossyScale.z;
		this.checkBounds.size = size;
		this.checkOffset = this.activeCollider.transform.position;
		this.checkRot = this.activeCollider.transform.rotation;
		this.CheckHeadOverlap();
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x000249FC File Offset: 0x00022BFC
	public void CheckHeadOverlap()
	{
		if (this.activeCollider == null)
		{
			return;
		}
		Vector3 position = GorillaTagger.Instance.headCollider.transform.position;
		float num = GorillaTagger.Instance.headCollider.radius * GorillaTagger.Instance.headCollider.transform.lossyScale.x;
		Vector3 vector = Quaternion.Inverse(this.checkRot) * (position - this.checkOffset);
		if (Vector3.Magnitude(this.checkBounds.ClosestPoint(vector) - vector) < num)
		{
			this.isOverlappingHead = true;
			this.activeCollider.enabled = false;
			return;
		}
		this.isOverlappingHead = false;
		this.activeCollider.enabled = true;
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00024AB5 File Offset: 0x00022CB5
	private void LateUpdate()
	{
		if (Time.time > this.timeToDie)
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		if (this.isOverlappingHead)
		{
			this.CheckHeadOverlap();
		}
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00024AE3 File Offset: 0x00022CE3
	private void OnDisable()
	{
		Action onDisabled = this.OnDisabled;
		if (onDisabled != null)
		{
			onDisabled();
		}
		this.OnDisabled = null;
	}

	// Token: 0x040007FE RID: 2046
	[SerializeField]
	private GameObject extendedDurationFrame;

	// Token: 0x040007FF RID: 2047
	[SerializeField]
	private float defaultDuration = 10f;

	// Token: 0x04000800 RID: 2048
	[SerializeField]
	private float extendedDuration = 20f;

	// Token: 0x04000801 RID: 2049
	[SerializeField]
	private BoxCollider activeCollider;

	// Token: 0x04000802 RID: 2050
	private bool isOverlappingHead;

	// Token: 0x04000803 RID: 2051
	private float timeToDie = -1f;

	// Token: 0x04000804 RID: 2052
	private Bounds checkBounds;

	// Token: 0x04000805 RID: 2053
	private Vector3 checkOffset;

	// Token: 0x04000806 RID: 2054
	private Quaternion checkRot;

	// Token: 0x04000807 RID: 2055
	public Action OnDisabled;
}
