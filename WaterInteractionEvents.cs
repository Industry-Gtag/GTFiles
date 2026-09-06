using System;
using System.Collections.Generic;
using GorillaLocomotion.Swimming;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000CEF RID: 3311
public class WaterInteractionEvents : MonoBehaviour
{
	// Token: 0x060051F7 RID: 20983 RVA: 0x001B38F0 File Offset: 0x001B1AF0
	private void Update()
	{
		if (this.overlappingWaterVolumes.Count < 1)
		{
			if (this.inWater)
			{
				this.onExitWater.Invoke();
			}
			this.inWater = false;
			base.enabled = false;
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
		{
			WaterVolume.SurfaceQuery surfaceQuery;
			if (this.overlappingWaterVolumes[i] == null)
			{
				this.overlappingWaterVolumes.RemoveAt(i);
				i--;
			}
			else if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(this.waterContactSphere.transform.position, out surfaceQuery, false))
			{
				float num = Vector3.Dot(surfaceQuery.surfacePoint - this.waterContactSphere.transform.position, surfaceQuery.surfaceNormal);
				float num2 = Vector3.Dot(surfaceQuery.surfacePoint - surfaceQuery.surfaceNormal * surfaceQuery.maxDepth - base.transform.position, surfaceQuery.surfaceNormal);
				if (num > -this.waterContactSphere.radius && num2 < this.waterContactSphere.radius)
				{
					flag = true;
				}
			}
		}
		bool flag2 = this.inWater;
		this.inWater = flag;
		if (!flag2 && this.inWater)
		{
			this.onEnterWater.Invoke();
			return;
		}
		if (flag2 && !this.inWater)
		{
			this.onExitWater.Invoke();
		}
	}

	// Token: 0x060051F8 RID: 20984 RVA: 0x001B3A54 File Offset: 0x001B1C54
	protected void OnTriggerEnter(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && !this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Add(component);
			base.enabled = true;
		}
	}

	// Token: 0x060051F9 RID: 20985 RVA: 0x001B3A94 File Offset: 0x001B1C94
	protected void OnTriggerExit(Collider other)
	{
		WaterVolume component = other.GetComponent<WaterVolume>();
		if (component != null && this.overlappingWaterVolumes.Contains(component))
		{
			this.overlappingWaterVolumes.Remove(component);
		}
	}

	// Token: 0x0400644D RID: 25677
	public UnityEvent onEnterWater = new UnityEvent();

	// Token: 0x0400644E RID: 25678
	public UnityEvent onExitWater = new UnityEvent();

	// Token: 0x0400644F RID: 25679
	[SerializeField]
	private SphereCollider waterContactSphere;

	// Token: 0x04006450 RID: 25680
	private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

	// Token: 0x04006451 RID: 25681
	private bool inWater;
}
