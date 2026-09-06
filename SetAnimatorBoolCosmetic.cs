using System;
using UnityEngine;

// Token: 0x020000BD RID: 189
public class SetAnimatorBoolCosmetic : MonoBehaviour
{
	// Token: 0x0600049A RID: 1178 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnAnimatorValueChanged()
	{
	}

	// Token: 0x0600049B RID: 1179 RVA: 0x00019FDA File Offset: 0x000181DA
	public void SetAnimatorBool(bool value)
	{
		if (this.bool1Hash == 0)
		{
			this.bool1Hash = Animator.StringToHash(this.boolParameterName);
		}
		this.animator.SetBool(this.bool1Hash, value);
	}

	// Token: 0x0600049C RID: 1180 RVA: 0x0001A007 File Offset: 0x00018207
	public void SetAnimatorBool2(bool value)
	{
		if (this.bool2Hash == 0)
		{
			this.bool2Hash = Animator.StringToHash(this.bool2ParameterName);
		}
		this.animator.SetBool(this.bool2Hash, value);
	}

	// Token: 0x0600049D RID: 1181 RVA: 0x0001A034 File Offset: 0x00018234
	public void SetAnimatorBool3(bool value)
	{
		if (this.bool3Hash == 0)
		{
			this.bool3Hash = Animator.StringToHash(this.bool3ParameterName);
		}
		this.animator.SetBool(this.bool3Hash, value);
	}

	// Token: 0x0600049E RID: 1182 RVA: 0x0001A061 File Offset: 0x00018261
	public void SetAnimatorBool4(bool value)
	{
		if (this.bool4Hash == 0)
		{
			this.bool4Hash = Animator.StringToHash(this.bool4ParameterName);
		}
		this.animator.SetBool(this.bool4Hash, value);
	}

	// Token: 0x0600049F RID: 1183 RVA: 0x0001A08E File Offset: 0x0001828E
	public void SetAnimatorBool5(bool value)
	{
		if (this.bool5Hash == 0)
		{
			this.bool5Hash = Animator.StringToHash(this.bool5ParameterName);
		}
		this.animator.SetBool(this.bool5Hash, value);
	}

	// Token: 0x060004A0 RID: 1184 RVA: 0x0001A0BB File Offset: 0x000182BB
	public void SetAnimatorInteger1(int value)
	{
		if (this.int1Hash == 0)
		{
			this.int1Hash = Animator.StringToHash(this.int1ParameterName);
		}
		this.animator.SetInteger(this.int1Hash, value);
	}

	// Token: 0x060004A1 RID: 1185 RVA: 0x0001A0E8 File Offset: 0x000182E8
	public void SetAnimatorInteger2(int value)
	{
		if (this.int2Hash == 0)
		{
			this.int2Hash = Animator.StringToHash(this.int2ParameterName);
		}
		this.animator.SetInteger(this.int2Hash, value);
	}

	// Token: 0x060004A2 RID: 1186 RVA: 0x0001A115 File Offset: 0x00018315
	public void SetAnimatorInteger3(int value)
	{
		if (this.int3Hash == 0)
		{
			this.int3Hash = Animator.StringToHash(this.int3ParameterName);
		}
		this.animator.SetInteger(this.int3Hash, value);
	}

	// Token: 0x060004A3 RID: 1187 RVA: 0x0001A142 File Offset: 0x00018342
	public void SetAnimatorInteger4(int value)
	{
		if (this.int4Hash == 0)
		{
			this.int4Hash = Animator.StringToHash(this.int4ParameterName);
		}
		this.animator.SetInteger(this.int4Hash, value);
	}

	// Token: 0x060004A4 RID: 1188 RVA: 0x0001A16F File Offset: 0x0001836F
	public void SetAnimatorFloat1(float value)
	{
		if (this.float1Hash == 0)
		{
			this.float1Hash = Animator.StringToHash(this.float1ParameterName);
		}
		this.animator.SetFloat(this.float1Hash, value);
	}

	// Token: 0x060004A5 RID: 1189 RVA: 0x0001A19C File Offset: 0x0001839C
	public void SetAnimatorFloat2(float value)
	{
		if (this.float2Hash == 0)
		{
			this.float2Hash = Animator.StringToHash(this.float2ParameterName);
		}
		this.animator.SetFloat(this.float2Hash, value);
	}

	// Token: 0x060004A6 RID: 1190 RVA: 0x0001A1C9 File Offset: 0x000183C9
	public void SetAnimatorFloat3(float value)
	{
		if (this.float3Hash == 0)
		{
			this.float3Hash = Animator.StringToHash(this.float3ParameterName);
		}
		this.animator.SetFloat(this.float3Hash, value);
	}

	// Token: 0x060004A7 RID: 1191 RVA: 0x0001A1F6 File Offset: 0x000183F6
	public void SetAnimatorFloat4(float value)
	{
		if (this.float4Hash == 0)
		{
			this.float4Hash = Animator.StringToHash(this.float4ParameterName);
		}
		this.animator.SetFloat(this.float4Hash, value);
	}

	// Token: 0x060004A8 RID: 1192 RVA: 0x0001A223 File Offset: 0x00018423
	public void SetAnimatorTrigger(string triggerName)
	{
		this.animator.SetTrigger(triggerName);
	}

	// Token: 0x060004A9 RID: 1193 RVA: 0x0001A231 File Offset: 0x00018431
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x040004F6 RID: 1270
	[SerializeField]
	private Animator animator;

	// Token: 0x040004F7 RID: 1271
	[SerializeField]
	private string boolParameterName;

	// Token: 0x040004F8 RID: 1272
	[SerializeField]
	private string bool2ParameterName;

	// Token: 0x040004F9 RID: 1273
	[SerializeField]
	private string bool3ParameterName;

	// Token: 0x040004FA RID: 1274
	[SerializeField]
	private string bool4ParameterName;

	// Token: 0x040004FB RID: 1275
	[SerializeField]
	private string bool5ParameterName;

	// Token: 0x040004FC RID: 1276
	[SerializeField]
	private string int1ParameterName;

	// Token: 0x040004FD RID: 1277
	[SerializeField]
	private string int2ParameterName;

	// Token: 0x040004FE RID: 1278
	[SerializeField]
	private string int3ParameterName;

	// Token: 0x040004FF RID: 1279
	[SerializeField]
	private string int4ParameterName;

	// Token: 0x04000500 RID: 1280
	[SerializeField]
	private string float1ParameterName;

	// Token: 0x04000501 RID: 1281
	[SerializeField]
	private string float2ParameterName;

	// Token: 0x04000502 RID: 1282
	[SerializeField]
	private string float3ParameterName;

	// Token: 0x04000503 RID: 1283
	[SerializeField]
	private string float4ParameterName;

	// Token: 0x04000504 RID: 1284
	private int bool1Hash;

	// Token: 0x04000505 RID: 1285
	private int bool2Hash;

	// Token: 0x04000506 RID: 1286
	private int bool3Hash;

	// Token: 0x04000507 RID: 1287
	private int bool4Hash;

	// Token: 0x04000508 RID: 1288
	private int bool5Hash;

	// Token: 0x04000509 RID: 1289
	private const int MAX_BOOLS = 5;

	// Token: 0x0400050A RID: 1290
	private int int1Hash;

	// Token: 0x0400050B RID: 1291
	private int int2Hash;

	// Token: 0x0400050C RID: 1292
	private int int3Hash;

	// Token: 0x0400050D RID: 1293
	private int int4Hash;

	// Token: 0x0400050E RID: 1294
	private const int MAX_INTS = 4;

	// Token: 0x0400050F RID: 1295
	private int float1Hash;

	// Token: 0x04000510 RID: 1296
	private int float2Hash;

	// Token: 0x04000511 RID: 1297
	private int float3Hash;

	// Token: 0x04000512 RID: 1298
	private int float4Hash;

	// Token: 0x04000513 RID: 1299
	private const int MAX_FLOATS = 4;
}
