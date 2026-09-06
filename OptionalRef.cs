using System;
using UnityEngine;

// Token: 0x02000B0F RID: 2831
[Serializable]
public class OptionalRef<T> where T : Object
{
	// Token: 0x170006DB RID: 1755
	// (get) Token: 0x06004872 RID: 18546 RVA: 0x00185CF5 File Offset: 0x00183EF5
	// (set) Token: 0x06004873 RID: 18547 RVA: 0x00185CFD File Offset: 0x00183EFD
	public bool enabled
	{
		get
		{
			return this._enabled;
		}
		set
		{
			this._enabled = value;
		}
	}

	// Token: 0x170006DC RID: 1756
	// (get) Token: 0x06004874 RID: 18548 RVA: 0x00185D08 File Offset: 0x00183F08
	// (set) Token: 0x06004875 RID: 18549 RVA: 0x00185D30 File Offset: 0x00183F30
	public T Value
	{
		get
		{
			if (this)
			{
				return this._target;
			}
			return default(T);
		}
		set
		{
			this._target = (value ? value : default(T));
		}
	}

	// Token: 0x06004876 RID: 18550 RVA: 0x00185D5C File Offset: 0x00183F5C
	public static implicit operator bool(OptionalRef<T> r)
	{
		if (r == null)
		{
			return false;
		}
		if (!r._enabled)
		{
			return false;
		}
		Object @object = r._target;
		return @object != null && @object;
	}

	// Token: 0x06004877 RID: 18551 RVA: 0x00185D90 File Offset: 0x00183F90
	public static implicit operator T(OptionalRef<T> r)
	{
		if (r == null)
		{
			return default(T);
		}
		if (!r._enabled)
		{
			return default(T);
		}
		Object @object = r._target;
		if (@object == null)
		{
			return default(T);
		}
		if (!@object)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x06004878 RID: 18552 RVA: 0x00185DF4 File Offset: 0x00183FF4
	public static implicit operator Object(OptionalRef<T> r)
	{
		if (r == null)
		{
			return null;
		}
		if (!r._enabled)
		{
			return null;
		}
		Object @object = r._target;
		if (@object == null)
		{
			return null;
		}
		if (!@object)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x04005B06 RID: 23302
	[SerializeField]
	private bool _enabled;

	// Token: 0x04005B07 RID: 23303
	[SerializeField]
	private T _target;
}
