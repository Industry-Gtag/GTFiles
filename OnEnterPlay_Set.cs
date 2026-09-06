using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000DC7 RID: 3527
[AttributeUsage(AttributeTargets.Field)]
public class OnEnterPlay_Set : OnEnterPlay_Attribute
{
	// Token: 0x0600569A RID: 22170 RVA: 0x001C4409 File Offset: 0x001C2609
	public OnEnterPlay_Set(object value)
	{
		this.value = value;
	}

	// Token: 0x0600569B RID: 22171 RVA: 0x001C4418 File Offset: 0x001C2618
	public override void OnEnterPlay(FieldInfo field)
	{
		if (!field.IsStatic)
		{
			Debug.LogError(string.Format("Can't Set non-static field {0}.{1}", field.DeclaringType, field.Name));
			return;
		}
		if (field.FieldType == typeof(ushort))
		{
			field.SetValue(null, Convert.ToUInt16(this.value));
			return;
		}
		field.SetValue(null, this.value);
	}

	// Token: 0x04006795 RID: 26517
	private object value;
}
