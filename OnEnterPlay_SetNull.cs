using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DC6 RID: 3526
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_SetNull : OnEnterPlay_Attribute
{
	// Token: 0x06005698 RID: 22168 RVA: 0x001C43D3 File Offset: 0x001C25D3
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNull non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.SetValue(null, null);
	}
}
