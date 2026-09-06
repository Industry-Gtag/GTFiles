using System;

// Token: 0x02000E15 RID: 3605
public class InvalidType : ProxyType
{
	// Token: 0x17000863 RID: 2147
	// (get) Token: 0x06005835 RID: 22581 RVA: 0x001CB201 File Offset: 0x001C9401
	public override string Name
	{
		get
		{
			return this._self.Name;
		}
	}

	// Token: 0x17000864 RID: 2148
	// (get) Token: 0x06005836 RID: 22582 RVA: 0x001CB20E File Offset: 0x001C940E
	public override string FullName
	{
		get
		{
			return this._self.FullName;
		}
	}

	// Token: 0x17000865 RID: 2149
	// (get) Token: 0x06005837 RID: 22583 RVA: 0x001CB21B File Offset: 0x001C941B
	public override string AssemblyQualifiedName
	{
		get
		{
			return this._self.AssemblyQualifiedName;
		}
	}

	// Token: 0x0400687E RID: 26750
	private Type _self = typeof(InvalidType);
}
