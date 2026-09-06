using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DD1 RID: 3537
[AttributeUsage(AttributeTargets.Field)]
public class OnExitPlay_SetNew : OnExitPlay_Attribute
{
	// Token: 0x060056AF RID: 22191 RVA: 0x001C4650 File Offset: 0x001C2850
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
