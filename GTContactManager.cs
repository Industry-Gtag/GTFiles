using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class GTContactManager : MonoBehaviour
{
	// Token: 0x06001464 RID: 5220 RVA: 0x00002C2D File Offset: 0x00000E2D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void InitializeOnLoad()
	{
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x0006DC78 File Offset: 0x0006BE78
	private static GTContactPoint[] InitContactPoints(int count)
	{
		GTContactPoint[] array = new GTContactPoint[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = new GTContactPoint();
		}
		return array;
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x0006DCA4 File Offset: 0x0006BEA4
	public static void RaiseContact(Vector3 point, Vector3 normal)
	{
		if (GTContactManager.gNextFree == -1)
		{
			return;
		}
		float time = GTShaderGlobals.Time;
		GTContactPoint gtcontactPoint = GTContactManager._gContactPoints[GTContactManager.gNextFree];
		gtcontactPoint.contactPoint = point;
		gtcontactPoint.radius = 0.04f;
		gtcontactPoint.counterVelocity = normal;
		gtcontactPoint.timestamp = time;
		gtcontactPoint.lifetime = 2f;
		gtcontactPoint.color = GTContactManager.gRND.NextColor();
		gtcontactPoint.free = 0U;
	}

	// Token: 0x06001467 RID: 5223 RVA: 0x0006DD0C File Offset: 0x0006BF0C
	public static void ProcessContacts()
	{
		Matrix4x4[] shaderData = GTContactManager.ShaderData;
		GTContactPoint[] gContactPoints = GTContactManager._gContactPoints;
		int frame = GTShaderGlobals.Frame;
		for (int i = 0; i < 32; i++)
		{
			GTContactManager.Transfer(ref gContactPoints[i].data, ref shaderData[i]);
		}
	}

	// Token: 0x06001468 RID: 5224 RVA: 0x0006DD4C File Offset: 0x0006BF4C
	private static void Transfer(ref Matrix4x4 from, ref Matrix4x4 to)
	{
		to.m00 = from.m00;
		to.m01 = from.m01;
		to.m02 = from.m02;
		to.m03 = from.m03;
		to.m10 = from.m10;
		to.m11 = from.m11;
		to.m12 = from.m12;
		to.m13 = from.m13;
		to.m20 = from.m20;
		to.m21 = from.m21;
		to.m22 = from.m22;
		to.m23 = from.m23;
		to.m30 = from.m30;
		to.m31 = from.m31;
		to.m32 = from.m32;
		to.m33 = from.m33;
	}

	// Token: 0x0400193C RID: 6460
	public const int MAX_CONTACTS = 32;

	// Token: 0x0400193D RID: 6461
	public static Matrix4x4[] ShaderData = new Matrix4x4[32];

	// Token: 0x0400193E RID: 6462
	private static GTContactPoint[] _gContactPoints = GTContactManager.InitContactPoints(32);

	// Token: 0x0400193F RID: 6463
	private static int gNextFree = 0;

	// Token: 0x04001940 RID: 6464
	private static SRand gRND = new SRand(DateTime.UtcNow);
}
