using System;
using GorillaExtensions;
using Photon.Pun;

// Token: 0x02000081 RID: 129
public struct CritterAppearance
{
	// Token: 0x06000327 RID: 807 RVA: 0x0001338B File Offset: 0x0001158B
	public CritterAppearance(string hatName, float size = 1f)
	{
		this.hatName = hatName;
		this.size = size;
	}

	// Token: 0x06000328 RID: 808 RVA: 0x0001339C File Offset: 0x0001159C
	public object[] WriteToRPCData()
	{
		object[] array = new object[] { this.hatName, this.size };
		if (this.hatName == null)
		{
			array[0] = string.Empty;
		}
		if (this.size != 0f)
		{
			array[1] = this.size;
		}
		return array;
	}

	// Token: 0x06000329 RID: 809 RVA: 0x000133F3 File Offset: 0x000115F3
	public static int DataLength()
	{
		return 2;
	}

	// Token: 0x0600032A RID: 810 RVA: 0x000133F8 File Offset: 0x000115F8
	public static bool ValidateData(object[] data)
	{
		float num;
		return data != null && data.Length == CritterAppearance.DataLength() && CrittersManager.ValidateDataType<float>(data[1], out num) && num >= 0f && !float.IsNaN(num) && !float.IsInfinity(num);
	}

	// Token: 0x0600032B RID: 811 RVA: 0x00013440 File Offset: 0x00011640
	public static CritterAppearance ReadFromRPCData(object[] data)
	{
		string text;
		if (!CrittersManager.ValidateDataType<string>(data[0], out text))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		float num;
		if (!CrittersManager.ValidateDataType<float>(data[1], out num))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		return new CritterAppearance((string)data[0], num.GetFinite());
	}

	// Token: 0x0600032C RID: 812 RVA: 0x00013498 File Offset: 0x00011698
	public static CritterAppearance ReadFromPhotonStream(PhotonStream data)
	{
		string text = (string)data.ReceiveNext();
		float num = (float)data.ReceiveNext();
		return new CritterAppearance(text, num);
	}

	// Token: 0x0600032D RID: 813 RVA: 0x000134C2 File Offset: 0x000116C2
	public override string ToString()
	{
		return string.Format("Size: {0} Hat: {1}", this.size, this.hatName);
	}

	// Token: 0x040003C2 RID: 962
	public float size;

	// Token: 0x040003C3 RID: 963
	public string hatName;
}
