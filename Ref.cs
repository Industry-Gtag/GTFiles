using System;
using UnityEngine;

// Token: 0x020009DD RID: 2525
[Serializable]
public class Ref<T> where T : class
{
	// Token: 0x17000616 RID: 1558
	// (get) Token: 0x060040C7 RID: 16583 RVA: 0x00158E33 File Offset: 0x00157033
	// (set) Token: 0x060040C8 RID: 16584 RVA: 0x00158E3B File Offset: 0x0015703B
	public T AsT
	{
		get
		{
			return this;
		}
		set
		{
			this._target = value as Object;
		}
	}

	// Token: 0x060040C9 RID: 16585 RVA: 0x00158E50 File Offset: 0x00157050
	public static implicit operator bool(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		return @object != null && @object != null;
	}

	// Token: 0x060040CA RID: 16586 RVA: 0x00158E78 File Offset: 0x00157078
	public static implicit operator T(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		if (@object == null)
		{
			return default(T);
		}
		if (@object == null)
		{
			return default(T);
		}
		return @object as T;
	}

	// Token: 0x060040CB RID: 16587 RVA: 0x00158EC0 File Offset: 0x001570C0
	public static implicit operator Object(Ref<T> r)
	{
		Object @object = ((r != null) ? r._target : null);
		if (@object == null)
		{
			return null;
		}
		if (@object == null)
		{
			return null;
		}
		return @object;
	}

	// Token: 0x0400514C RID: 20812
	[SerializeField]
	private Object _target;
}
