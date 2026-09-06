using System;
using System.Globalization;
using System.Reflection;

// Token: 0x02000E16 RID: 3606
public class ProxyType : Type
{
	// Token: 0x06005839 RID: 22585 RVA: 0x001CB240 File Offset: 0x001C9440
	public ProxyType()
	{
	}

	// Token: 0x0600583A RID: 22586 RVA: 0x001CB258 File Offset: 0x001C9458
	public ProxyType(string typeName)
	{
		this._typeName = typeName;
	}

	// Token: 0x17000866 RID: 2150
	// (get) Token: 0x0600583B RID: 22587 RVA: 0x001CB277 File Offset: 0x001C9477
	public override string Name
	{
		get
		{
			return this._typeName;
		}
	}

	// Token: 0x17000867 RID: 2151
	// (get) Token: 0x0600583C RID: 22588 RVA: 0x001CB27F File Offset: 0x001C947F
	public override string FullName
	{
		get
		{
			return ProxyType.kPrefix + this._typeName;
		}
	}

	// Token: 0x0600583D RID: 22589 RVA: 0x001CB294 File Offset: 0x001C9494
	public static ProxyType Parse(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			throw new ArgumentNullException("input");
		}
		input = input.Trim();
		if (!input.Contains(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (!input.StartsWith(ProxyType.kPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return ProxyType.kInvalidType;
		}
		if (input.Contains(','))
		{
			input = input.Split(',', StringSplitOptions.None)[0];
		}
		string text = input.Split('.', StringSplitOptions.None)[1].Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			return ProxyType.kInvalidType;
		}
		return new ProxyType(text);
	}

	// Token: 0x0600583E RID: 22590 RVA: 0x001CB320 File Offset: 0x001C9520
	public override string ToString()
	{
		return base.ToString() + "." + this._typeName;
	}

	// Token: 0x0600583F RID: 22591 RVA: 0x001CB338 File Offset: 0x001C9538
	public override object[] GetCustomAttributes(bool inherit)
	{
		return this._self.GetCustomAttributes(inherit);
	}

	// Token: 0x06005840 RID: 22592 RVA: 0x001CB346 File Offset: 0x001C9546
	public override object[] GetCustomAttributes(Type attributeType, bool inherit)
	{
		return this._self.GetCustomAttributes(attributeType, inherit);
	}

	// Token: 0x06005841 RID: 22593 RVA: 0x001CB355 File Offset: 0x001C9555
	public override bool IsDefined(Type attributeType, bool inherit)
	{
		return this._self.IsDefined(attributeType, inherit);
	}

	// Token: 0x17000868 RID: 2152
	// (get) Token: 0x06005842 RID: 22594 RVA: 0x001CB364 File Offset: 0x001C9564
	public override Module Module
	{
		get
		{
			return this._self.Module;
		}
	}

	// Token: 0x17000869 RID: 2153
	// (get) Token: 0x06005843 RID: 22595 RVA: 0x001CB371 File Offset: 0x001C9571
	public override string Namespace
	{
		get
		{
			return this._self.Namespace;
		}
	}

	// Token: 0x06005844 RID: 22596 RVA: 0x00002076 File Offset: 0x00000276
	protected override TypeAttributes GetAttributeFlagsImpl()
	{
		return TypeAttributes.NotPublic;
	}

	// Token: 0x06005845 RID: 22597 RVA: 0x00036275 File Offset: 0x00034475
	protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x06005846 RID: 22598 RVA: 0x001CB37E File Offset: 0x001C957E
	public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
	{
		return this._self.GetConstructors(bindingAttr);
	}

	// Token: 0x06005847 RID: 22599 RVA: 0x001CB38C File Offset: 0x001C958C
	public override Type GetElementType()
	{
		return this._self.GetElementType();
	}

	// Token: 0x06005848 RID: 22600 RVA: 0x001CB399 File Offset: 0x001C9599
	public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
	{
		return this._self.GetEvent(name, bindingAttr);
	}

	// Token: 0x06005849 RID: 22601 RVA: 0x001CB3A8 File Offset: 0x001C95A8
	public override EventInfo[] GetEvents(BindingFlags bindingAttr)
	{
		return this._self.GetEvents(bindingAttr);
	}

	// Token: 0x0600584A RID: 22602 RVA: 0x001CB3B6 File Offset: 0x001C95B6
	public override FieldInfo GetField(string name, BindingFlags bindingAttr)
	{
		return this._self.GetField(name, bindingAttr);
	}

	// Token: 0x0600584B RID: 22603 RVA: 0x001CB3C5 File Offset: 0x001C95C5
	public override FieldInfo[] GetFields(BindingFlags bindingAttr)
	{
		return this._self.GetFields(bindingAttr);
	}

	// Token: 0x0600584C RID: 22604 RVA: 0x001CB3D3 File Offset: 0x001C95D3
	public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
	{
		return this._self.GetMembers(bindingAttr);
	}

	// Token: 0x0600584D RID: 22605 RVA: 0x00036275 File Offset: 0x00034475
	protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x0600584E RID: 22606 RVA: 0x001CB3E1 File Offset: 0x001C95E1
	public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
	{
		return this._self.GetMethods(bindingAttr);
	}

	// Token: 0x0600584F RID: 22607 RVA: 0x001CB3EF File Offset: 0x001C95EF
	public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
	{
		return this._self.GetProperties(bindingAttr);
	}

	// Token: 0x06005850 RID: 22608 RVA: 0x001CB400 File Offset: 0x001C9600
	public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
	{
		return this._self.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
	}

	// Token: 0x1700086A RID: 2154
	// (get) Token: 0x06005851 RID: 22609 RVA: 0x001CB425 File Offset: 0x001C9625
	public override Type UnderlyingSystemType
	{
		get
		{
			return this._self.UnderlyingSystemType;
		}
	}

	// Token: 0x06005852 RID: 22610 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsArrayImpl()
	{
		return false;
	}

	// Token: 0x06005853 RID: 22611 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsByRefImpl()
	{
		return false;
	}

	// Token: 0x06005854 RID: 22612 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsCOMObjectImpl()
	{
		return false;
	}

	// Token: 0x06005855 RID: 22613 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPointerImpl()
	{
		return false;
	}

	// Token: 0x06005856 RID: 22614 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool IsPrimitiveImpl()
	{
		return false;
	}

	// Token: 0x1700086B RID: 2155
	// (get) Token: 0x06005857 RID: 22615 RVA: 0x001CB432 File Offset: 0x001C9632
	public override Assembly Assembly
	{
		get
		{
			return this._self.Assembly;
		}
	}

	// Token: 0x1700086C RID: 2156
	// (get) Token: 0x06005858 RID: 22616 RVA: 0x001CB43F File Offset: 0x001C963F
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName.Replace("ProxyType", this.FullName);
		}
	}

	// Token: 0x1700086D RID: 2157
	// (get) Token: 0x06005859 RID: 22617 RVA: 0x001CB45C File Offset: 0x001C965C
	public override Type BaseType
	{
		get
		{
			return this._self.BaseType;
		}
	}

	// Token: 0x1700086E RID: 2158
	// (get) Token: 0x0600585A RID: 22618 RVA: 0x001CB469 File Offset: 0x001C9669
	public override Guid GUID
	{
		get
		{
			return this._self.GUID;
		}
	}

	// Token: 0x0600585B RID: 22619 RVA: 0x00036275 File Offset: 0x00034475
	protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
	{
		return null;
	}

	// Token: 0x0600585C RID: 22620 RVA: 0x00002076 File Offset: 0x00000276
	protected override bool HasElementTypeImpl()
	{
		return false;
	}

	// Token: 0x0600585D RID: 22621 RVA: 0x001CB476 File Offset: 0x001C9676
	public override Type GetNestedType(string name, BindingFlags bindingAttr)
	{
		return this._self.GetNestedType(name, bindingAttr);
	}

	// Token: 0x0600585E RID: 22622 RVA: 0x001CB485 File Offset: 0x001C9685
	public override Type[] GetNestedTypes(BindingFlags bindingAttr)
	{
		return this._self.GetNestedTypes(bindingAttr);
	}

	// Token: 0x0600585F RID: 22623 RVA: 0x001CB493 File Offset: 0x001C9693
	public override Type GetInterface(string name, bool ignoreCase)
	{
		return this._self.GetInterface(name, ignoreCase);
	}

	// Token: 0x06005860 RID: 22624 RVA: 0x001CB4A2 File Offset: 0x001C96A2
	public override Type[] GetInterfaces()
	{
		return this._self.GetInterfaces();
	}

	// Token: 0x0400687F RID: 26751
	private Type _self = typeof(ProxyType);

	// Token: 0x04006880 RID: 26752
	private readonly string _typeName;

	// Token: 0x04006881 RID: 26753
	private static readonly string kPrefix = "ProxyType.";

	// Token: 0x04006882 RID: 26754
	private static InvalidType kInvalidType = new InvalidType();
}
