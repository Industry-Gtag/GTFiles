using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class MenagerieDepositBox : MonoBehaviour
{
	// Token: 0x06000385 RID: 901 RVA: 0x00014B6C File Offset: 0x00012D6C
	public void OnTriggerEnter(Collider other)
	{
		MenagerieCritter component = other.transform.parent.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Combine(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00014BB4 File Offset: 0x00012DB4
	public void OnTriggerExit(Collider other)
	{
		MenagerieCritter component = other.transform.parent.GetComponent<MenagerieCritter>();
		if (component.IsNotNull())
		{
			MenagerieCritter menagerieCritter = component;
			menagerieCritter.OnReleased = (Action<MenagerieCritter>)Delegate.Remove(menagerieCritter.OnReleased, this.OnCritterInserted);
		}
	}

	// Token: 0x0400040A RID: 1034
	public Action<MenagerieCritter> OnCritterInserted;
}
