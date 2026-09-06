using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002EA RID: 746
public class OnSqueezeTrigger : MonoBehaviour
{
	// Token: 0x060012FD RID: 4861 RVA: 0x000650B4 File Offset: 0x000632B4
	private void Start()
	{
		this.myRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060012FE RID: 4862 RVA: 0x000650C4 File Offset: 0x000632C4
	private void Update()
	{
		bool flag;
		if (this.myHoldable.InLeftHand())
		{
			flag = (this.indexFinger ? this.myRig.leftIndex.calcT : this.myRig.leftMiddle.calcT) > 0.5f;
		}
		else
		{
			flag = this.myHoldable.InRightHand() && (this.indexFinger ? this.myRig.rightIndex.calcT : this.myRig.rightMiddle.calcT) > 0.5f;
		}
		if (flag != this.triggerWasDown)
		{
			if (flag)
			{
				this.onPress.Invoke();
				this.updateWhilePressed.Invoke();
			}
			else
			{
				this.onRelease.Invoke();
			}
		}
		else if (flag)
		{
			this.updateWhilePressed.Invoke();
		}
		this.triggerWasDown = flag;
	}

	// Token: 0x04001734 RID: 5940
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x04001735 RID: 5941
	[SerializeField]
	private UnityEvent onPress;

	// Token: 0x04001736 RID: 5942
	[SerializeField]
	private UnityEvent onRelease;

	// Token: 0x04001737 RID: 5943
	[SerializeField]
	private UnityEvent updateWhilePressed;

	// Token: 0x04001738 RID: 5944
	private VRRig myRig;

	// Token: 0x04001739 RID: 5945
	private bool indexFinger = true;

	// Token: 0x0400173A RID: 5946
	private bool triggerWasDown;
}
