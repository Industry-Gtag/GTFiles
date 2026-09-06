using System;
using UnityEngine;

// Token: 0x020007C5 RID: 1989
public class GRMeterEnergy : MonoBehaviour
{
	// Token: 0x060032BE RID: 12990 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void Awake()
	{
	}

	// Token: 0x060032BF RID: 12991 RVA: 0x001161DC File Offset: 0x001143DC
	public void Refresh()
	{
		float num = 0f;
		if (this.tool != null && this.tool.GetEnergyMax() > 0)
		{
			num = (float)this.tool.energy / (float)this.tool.GetEnergyMax();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		GRMeterEnergy.MeterType meterType = this.meterType;
		if (meterType == GRMeterEnergy.MeterType.Linear || meterType != GRMeterEnergy.MeterType.Radial)
		{
			this.meter.localScale = new Vector3(1f, num, 1f);
			return;
		}
		float num2 = Mathf.Lerp(this.angularRange.x, this.angularRange.y, num);
		Vector3 zero = Vector3.zero;
		zero[this.rotationAxis] = num2;
		this.meter.localRotation = Quaternion.Euler(zero);
	}

	// Token: 0x040041CA RID: 16842
	public GRTool tool;

	// Token: 0x040041CB RID: 16843
	public Transform meter;

	// Token: 0x040041CC RID: 16844
	public Transform chargePoint;

	// Token: 0x040041CD RID: 16845
	public GRMeterEnergy.MeterType meterType;

	// Token: 0x040041CE RID: 16846
	public Vector2 angularRange = new Vector2(-45f, 45f);

	// Token: 0x040041CF RID: 16847
	[Range(0f, 2f)]
	public int rotationAxis;

	// Token: 0x020007C6 RID: 1990
	public enum MeterType
	{
		// Token: 0x040041D1 RID: 16849
		Linear,
		// Token: 0x040041D2 RID: 16850
		Radial
	}
}
