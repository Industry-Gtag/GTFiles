using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DCF RID: 3535
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_SetNull : OnExitPlay_Attribute
{
	// Token: 0x060056AB RID: 22187 RVA: 0x001C43D3 File Offset: 0x001C25D3
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
