using System;

// Token: 0x02000B3F RID: 2879
[Serializable]
public class ErrorContent
{
	// Token: 0x17000709 RID: 1801
	// (get) Token: 0x0600498A RID: 18826 RVA: 0x0018869B File Offset: 0x0018689B
	// (set) Token: 0x0600498B RID: 18827 RVA: 0x001886A3 File Offset: 0x001868A3
	public string Message { get; set; }

	// Token: 0x1700070A RID: 1802
	// (get) Token: 0x0600498C RID: 18828 RVA: 0x001886AC File Offset: 0x001868AC
	// (set) Token: 0x0600498D RID: 18829 RVA: 0x001886B4 File Offset: 0x001868B4
	public string Error { get; set; }

	// Token: 0x0600498E RID: 18830 RVA: 0x001886BD File Offset: 0x001868BD
	public override string ToString()
	{
		return "Error: " + this.Error + ", Message: " + this.Message;
	}
}
