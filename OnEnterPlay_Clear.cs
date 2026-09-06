using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DC9 RID: 3529
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_Clear : OnEnterPlay_Attribute
{
	// Token: 0x0600569E RID: 22174 RVA: 0x001C44E0 File Offset: 0x001C26E0
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Clear non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		MethodInfo method = field.FieldType.GetMethod("Clear");
		object value = field.GetValue(null);
		if (value != null)
		{
			method.Invoke(value, new object[0]);
		}
	}
}
