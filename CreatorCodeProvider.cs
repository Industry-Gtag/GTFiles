using System;
using Cosmetics;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class CreatorCodeProvider : MonoBehaviour, ICreatorCodeProvider, IBuildValidation
{
	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000111 RID: 273 RVA: 0x0000665F File Offset: 0x0000485F
	string ICreatorCodeProvider.TerminalId
	{
		get
		{
			return this.nexusCreatorCode.GroupId.Code + this.nexusCreatorCode.Code;
		}
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00006681 File Offset: 0x00004881
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.nexusCreatorCode == null)
		{
			Debug.LogError("The CreatorCodeProvider component on " + base.name + " must be assigned a nexusCreatorCode.");
			return false;
		}
		return true;
	}

	// Token: 0x06000113 RID: 275 RVA: 0x000066AE File Offset: 0x000048AE
	void ICreatorCodeProvider.GetCreatorCode(out string code, out NexusGroupId[] groups)
	{
		code = this.nexusCreatorCode.Code;
		groups = new NexusGroupId[] { this.nexusCreatorCode.GroupId };
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x06000114 RID: 276 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject ICreatorCodeProvider.GameObject
	{
		get
		{
			return base.gameObject;
		}
	}

	// Token: 0x0400011E RID: 286
	[SerializeField]
	private NexusCreatorCode nexusCreatorCode;
}
