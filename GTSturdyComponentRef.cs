using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000311 RID: 785
[Serializable]
public struct GTSturdyComponentRef<T> where T : Component
{
	// Token: 0x170001FA RID: 506
	// (get) Token: 0x060013E3 RID: 5091 RVA: 0x0006C429 File Offset: 0x0006A629
	// (set) Token: 0x060013E4 RID: 5092 RVA: 0x0006C431 File Offset: 0x0006A631
	public Transform BaseXform
	{
		get
		{
			return this._baseXform;
		}
		set
		{
			this._baseXform = value;
		}
	}

	// Token: 0x170001FB RID: 507
	// (get) Token: 0x060013E5 RID: 5093 RVA: 0x0006C43C File Offset: 0x0006A63C
	// (set) Token: 0x060013E6 RID: 5094 RVA: 0x0006C4AB File Offset: 0x0006A6AB
	public T Value
	{
		get
		{
			if (!this._value)
			{
				return this._value;
			}
			if (string.IsNullOrEmpty(this._relativePath))
			{
				return default(T);
			}
			Transform transform;
			if (!this._baseXform.TryFindByPath(this._relativePath, out transform, false))
			{
				return default(T);
			}
			this._value = transform.GetComponent<T>();
			return this._value;
		}
		set
		{
			this._value = value;
			this._relativePath = ((!value) ? this._baseXform.GetRelativePath(value.transform) : string.Empty);
		}
	}

	// Token: 0x060013E7 RID: 5095 RVA: 0x0006C4E4 File Offset: 0x0006A6E4
	public static implicit operator T(GTSturdyComponentRef<T> sturdyRef)
	{
		return sturdyRef.Value;
	}

	// Token: 0x060013E8 RID: 5096 RVA: 0x0006C4F0 File Offset: 0x0006A6F0
	public static implicit operator GTSturdyComponentRef<T>(T component)
	{
		return new GTSturdyComponentRef<T>
		{
			Value = component
		};
	}

	// Token: 0x04001883 RID: 6275
	[SerializeField]
	private T _value;

	// Token: 0x04001884 RID: 6276
	[SerializeField]
	private string _relativePath;

	// Token: 0x04001885 RID: 6277
	[SerializeField]
	private Transform _baseXform;
}
