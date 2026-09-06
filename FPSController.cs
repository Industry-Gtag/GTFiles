using System;
using UnityEngine;

// Token: 0x02000D6B RID: 3435
public class FPSController : MonoBehaviour
{
	// Token: 0x14000097 RID: 151
	// (add) Token: 0x060054D6 RID: 21718 RVA: 0x001BD6C4 File Offset: 0x001BB8C4
	// (remove) Token: 0x060054D7 RID: 21719 RVA: 0x001BD6FC File Offset: 0x001BB8FC
	[HideInInspector]
	public event FPSController.OnStateChangeEventHandler OnStartEvent;

	// Token: 0x14000098 RID: 152
	// (add) Token: 0x060054D8 RID: 21720 RVA: 0x001BD734 File Offset: 0x001BB934
	// (remove) Token: 0x060054D9 RID: 21721 RVA: 0x001BD76C File Offset: 0x001BB96C
	public event FPSController.OnStateChangeEventHandler OnStopEvent;

	// Token: 0x04006644 RID: 26180
	public float baseMoveSpeed = 4f;

	// Token: 0x04006645 RID: 26181
	public float shiftMoveSpeed = 8f;

	// Token: 0x04006646 RID: 26182
	public float ctrlMoveSpeed = 1f;

	// Token: 0x04006647 RID: 26183
	public float lookHorizontal = 0.4f;

	// Token: 0x04006648 RID: 26184
	public float lookVertical = 0.25f;

	// Token: 0x04006649 RID: 26185
	[SerializeField]
	private Vector3 leftControllerPosOffset = new Vector3(-0.2f, -0.25f, 0.3f);

	// Token: 0x0400664A RID: 26186
	[SerializeField]
	private Vector3 leftControllerRotationOffset = new Vector3(265f, -82f, 28f);

	// Token: 0x0400664B RID: 26187
	[SerializeField]
	private Vector3 rightControllerPosOffset = new Vector3(0.2f, -0.25f, 0.3f);

	// Token: 0x0400664C RID: 26188
	[SerializeField]
	private Vector3 rightControllerRotationOffset = new Vector3(263f, 318f, 485f);

	// Token: 0x0400664D RID: 26189
	[SerializeField]
	private Vector3 noclipLeftControllerPosOffset = new Vector3(-0.3f, -0.1f, 0.65f);

	// Token: 0x0400664E RID: 26190
	[SerializeField]
	private Vector3 noclipLeftControllerRotationOffset = new Vector3(180f, -90f, -90f);

	// Token: 0x0400664F RID: 26191
	[SerializeField]
	private Vector3 noclipRightControllerPosOffset = new Vector3(0.3f, -0.1f, 0.65f);

	// Token: 0x04006650 RID: 26192
	[SerializeField]
	private Vector3 noclipRightControllerRotationOffset = new Vector3(0f, -90f, -90f);

	// Token: 0x04006651 RID: 26193
	[SerializeField]
	private bool toggleGrab;

	// Token: 0x04006652 RID: 26194
	[SerializeField]
	private bool clampGrab;

	// Token: 0x04006655 RID: 26197
	private bool controlRightHand;

	// Token: 0x04006656 RID: 26198
	public LayerMask HandMask;

	// Token: 0x02000D6C RID: 3436
	// (Invoke) Token: 0x060054DC RID: 21724
	public delegate void OnStateChangeEventHandler();
}
