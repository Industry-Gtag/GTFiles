using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class DoNotShip : MonoBehaviour, IBuildValidation
{
	// Token: 0x060003E4 RID: 996 RVA: 0x00017634 File Offset: 0x00015834
	bool IBuildValidation.BuildValidationCheck()
	{
		Debug.LogError("This build has a an object '" + base.gameObject.name + "' in it that was marked as 'Do Not Ship'");
		return false;
	}
}
