using System;

// Token: 0x02000328 RID: 808
public class ComponentMember
{
	// Token: 0x17000201 RID: 513
	// (get) Token: 0x06001410 RID: 5136 RVA: 0x0006CBB5 File Offset: 0x0006ADB5
	public string Name { get; }

	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001411 RID: 5137 RVA: 0x0006CBBD File Offset: 0x0006ADBD
	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	// Token: 0x17000203 RID: 515
	// (get) Token: 0x06001412 RID: 5138 RVA: 0x0006CBCA File Offset: 0x0006ADCA
	public bool IsStarred { get; }

	// Token: 0x17000204 RID: 516
	// (get) Token: 0x06001413 RID: 5139 RVA: 0x0006CBD2 File Offset: 0x0006ADD2
	public string Color { get; }

	// Token: 0x06001414 RID: 5140 RVA: 0x0006CBDA File Offset: 0x0006ADDA
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x040018F2 RID: 6386
	private Func<string> getValue;

	// Token: 0x040018F3 RID: 6387
	public string computedPrefix;

	// Token: 0x040018F4 RID: 6388
	public string computedSuffix;
}
