using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000D98 RID: 3480
public static class ComponentUtils
{
	// Token: 0x06005596 RID: 21910 RVA: 0x001BFD50 File Offset: 0x001BDF50
	public static T EnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return default(T);
		}
		if (target.AsNull<T>() != null)
		{
			return target;
		}
		return target = ctx.GetComponent<T>();
	}

	// Token: 0x06005597 RID: 21911 RVA: 0x001BFDA3 File Offset: 0x001BDFA3
	public static bool TryEnsureComponent<T>(this Component ctx, ref T target) where T : Component
	{
		if (ctx.AsNull<Component>() == null)
		{
			return false;
		}
		if (target.AsNull<T>() != null)
		{
			return true;
		}
		target = ctx.GetComponent<T>();
		return true;
	}

	// Token: 0x06005598 RID: 21912 RVA: 0x001BFDDC File Offset: 0x001BDFDC
	public static T AddComponent<T>(this Component c) where T : Component
	{
		return c.gameObject.AddComponent<T>();
	}

	// Token: 0x06005599 RID: 21913 RVA: 0x001BFDE9 File Offset: 0x001BDFE9
	public static void GetOrAddComponent<T>(this Component c, out T result) where T : Component
	{
		if (!c.TryGetComponent<T>(out result))
		{
			result = c.gameObject.AddComponent<T>();
		}
	}

	// Token: 0x0600559A RID: 21914 RVA: 0x001BFE05 File Offset: 0x001BE005
	public static bool GetComponentAndSetFieldIfNullElseLogAndDisable<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "Disabling.", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (c.GetComponentAndSetFieldIfNullElseLog(ref fieldRef, fieldName, fieldTypeName, msgSuffix, caller))
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x0600559B RID: 21915 RVA: 0x001BFE20 File Offset: 0x001BE020
	public static bool GetComponentAndSetFieldIfNullElseLog<T>(this Behaviour c, ref T fieldRef, string fieldName, string fieldTypeName, string msgSuffix = "", [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Component
	{
		if (fieldRef != null)
		{
			return true;
		}
		fieldRef = c.GetComponent<T>();
		if (fieldRef != null)
		{
			return true;
		}
		Debug.LogError(string.Concat(new string[] { caller, ": Could not find ", fieldTypeName, " \"", fieldName, "\" on \"", c.name, "\". ", msgSuffix }), c);
		return false;
	}

	// Token: 0x0600559C RID: 21916 RVA: 0x001BFEB1 File Offset: 0x001BE0B1
	public static bool DisableIfNull<T>(this Behaviour c, T fieldRef, string fieldName, string fieldTypeName, [CallerMemberName] string caller = "__UNKNOWN_CALLER__") where T : Object
	{
		if (fieldRef != null)
		{
			return true;
		}
		c.enabled = false;
		return false;
	}

	// Token: 0x0600559D RID: 21917 RVA: 0x001BFECB File Offset: 0x001BE0CB
	public static Hash128 ComputeStaticHash128(Component c, string k)
	{
		return ComponentUtils.ComputeStaticHash128(c, StaticHash.Compute(k));
	}

	// Token: 0x0600559E RID: 21918 RVA: 0x001BFEDC File Offset: 0x001BE0DC
	public static Hash128 ComputeStaticHash128(Component c, int k = 0)
	{
		if (c == null)
		{
			return default(Hash128);
		}
		Transform transform = c.transform;
		Component[] components = c.gameObject.GetComponents(typeof(Component));
		uint[] array = ComponentUtils.kHashBits;
		int siblingIndex = transform.GetSiblingIndex();
		int num = components.Length;
		int num2 = 0;
		while (num2 < num && c != components[num2])
		{
			num2++;
		}
		int num3 = StaticHash.Compute(k + 2, 1);
		int num4 = StaticHash.Compute(siblingIndex + 4, num3);
		int num5 = StaticHash.Compute(num + 8, num4);
		int num6 = StaticHash.Compute(num2 + 16, num5);
		array[0] = (uint)num3;
		array[1] = (uint)num4;
		array[2] = (uint)num5;
		array[3] = (uint)num6;
		SRand srand = new SRand(StaticHash.Compute(num3, num4, num5, num6));
		srand.Shuffle<uint>(array);
		Hash128 hash = new Hash128(array[0], array[1], array[2], array[3]);
		Hash128 hash2 = Hash128.Compute(c.GetType().FullName);
		Hash128 hash3 = TransformUtils.ComputePathHash(transform);
		Hash128 hash4 = transform.localToWorldMatrix.QuantizedHash128();
		HashUtilities.AppendHash(ref hash2, ref hash);
		HashUtilities.AppendHash(ref hash3, ref hash);
		HashUtilities.AppendHash(ref hash4, ref hash);
		return hash;
	}

	// Token: 0x040066FF RID: 26367
	private static readonly uint[] kHashBits = new uint[4];
}
