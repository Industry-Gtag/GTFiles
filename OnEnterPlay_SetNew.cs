using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DC8 RID: 3528
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_SetNew : OnEnterPlay_Attribute
{
	// Token: 0x0600569C RID: 22172 RVA: 0x001C4488 File Offset: 0x001C2688
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't SetNew non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		object obj = field.FieldType.GetConstructor(new Type[0]).Invoke(new object[0]);
		field.SetValue(null, obj);
	}
}
