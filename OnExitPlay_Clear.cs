using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DD2 RID: 3538
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_Clear : OnExitPlay_Attribute
{
	// Token: 0x060056B1 RID: 22193 RVA: 0x001C46A8 File Offset: 0x001C28A8
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		field.FieldType.GetMethod("Clear").Invoke(field.GetValue(null), new object[0]);
	}
}
