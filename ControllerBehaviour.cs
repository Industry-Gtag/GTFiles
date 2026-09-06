using System;
using UnityEngine;

// Token: 0x02000BEB RID: 3051
[Obsolete("Use ControllerInputPoller instead", false)]
public class ControllerBehaviour : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700074D RID: 1869
	// (get) Token: 0x06004CA7 RID: 19623 RVA: 0x00198F6E File Offset: 0x0019716E
	// (set) Token: 0x06004CA8 RID: 19624 RVA: 0x00198F75 File Offset: 0x00197175
	public static ControllerBehaviour Instance { get; private set; }

	// Token: 0x1700074E RID: 1870
	// (get) Token: 0x06004CA9 RID: 19625 RVA: 0x00198F7D File Offset: 0x0019717D
	private ControllerInputPoller Poller
	{
		get
		{
			if (this.poller != null)
			{
				return this.poller;
			}
			if (ControllerInputPoller.instance != null)
			{
				this.poller = ControllerInputPoller.instance;
				return this.poller;
			}
			return null;
		}
	}

	// Token: 0x1700074F RID: 1871
	// (get) Token: 0x06004CAA RID: 19626 RVA: 0x00198FB8 File Offset: 0x001971B8
	public bool ButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton);
		}
	}

	// Token: 0x17000750 RID: 1872
	// (get) Token: 0x06004CAB RID: 19627 RVA: 0x00199009 File Offset: 0x00197209
	public bool LeftButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerPrimaryButton || this.Poller.leftControllerSecondaryButton || this.Poller.leftControllerTriggerButton);
		}
	}

	// Token: 0x17000751 RID: 1873
	// (get) Token: 0x06004CAC RID: 19628 RVA: 0x00199042 File Offset: 0x00197242
	public bool RightButtonDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.rightControllerPrimaryButton || this.Poller.rightControllerSecondaryButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x17000752 RID: 1874
	// (get) Token: 0x06004CAD RID: 19629 RVA: 0x0019907C File Offset: 0x0019727C
	public bool IsLeftStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000753 RID: 1875
	// (get) Token: 0x06004CAE RID: 19630 RVA: 0x001990CC File Offset: 0x001972CC
	public bool IsRightStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.x, this.Poller.rightControllerPrimary2DAxis.x) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000754 RID: 1876
	// (get) Token: 0x06004CAF RID: 19631 RVA: 0x0019911C File Offset: 0x0019731C
	public bool IsUpStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Max(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) > this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000755 RID: 1877
	// (get) Token: 0x06004CB0 RID: 19632 RVA: 0x0019916C File Offset: 0x0019736C
	public bool IsDownStick
	{
		get
		{
			return !(this.Poller == null) && Mathf.Min(this.Poller.leftControllerPrimary2DAxis.y, this.Poller.rightControllerPrimary2DAxis.y) < -this.uxSettings.StickSensitvity;
		}
	}

	// Token: 0x17000756 RID: 1878
	// (get) Token: 0x06004CB1 RID: 19633 RVA: 0x001991BC File Offset: 0x001973BC
	public float StickXValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.x), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.x));
			}
			return 0f;
		}
	}

	// Token: 0x17000757 RID: 1879
	// (get) Token: 0x06004CB2 RID: 19634 RVA: 0x0019920C File Offset: 0x0019740C
	public float StickYValue
	{
		get
		{
			if (!(this.Poller == null))
			{
				return Mathf.Max(Mathf.Abs(this.Poller.leftControllerPrimary2DAxis.y), Mathf.Abs(this.Poller.rightControllerPrimary2DAxis.y));
			}
			return 0f;
		}
	}

	// Token: 0x17000758 RID: 1880
	// (get) Token: 0x06004CB3 RID: 19635 RVA: 0x0019925C File Offset: 0x0019745C
	public bool TriggerDown
	{
		get
		{
			return !(this.Poller == null) && (this.Poller.leftControllerTriggerButton || this.Poller.rightControllerTriggerButton);
		}
	}

	// Token: 0x14000087 RID: 135
	// (add) Token: 0x06004CB4 RID: 19636 RVA: 0x00199288 File Offset: 0x00197488
	// (remove) Token: 0x06004CB5 RID: 19637 RVA: 0x001992C0 File Offset: 0x001974C0
	public event ControllerBehaviour.OnActionEvent OnAction;

	// Token: 0x06004CB6 RID: 19638 RVA: 0x001992F5 File Offset: 0x001974F5
	private void Awake()
	{
		if (ControllerBehaviour.Instance != null)
		{
			Debug.LogError("[CONTROLLER_BEHAVIOUR] Trying to create new singleton but one already exists", base.gameObject);
			Object.DestroyImmediate(this);
			return;
		}
		ControllerBehaviour.Instance = this;
	}

	// Token: 0x06004CB7 RID: 19639 RVA: 0x00199324 File Offset: 0x00197524
	private void Update()
	{
		bool flag = (this.IsLeftStick && this.wasLeftStick) || (this.IsRightStick && this.wasRightStick) || (this.IsUpStick && this.wasUpStick) || (this.IsDownStick && this.wasDownStick);
		if (Time.time - this.actionTime < this.actionDelay / this.repeatAction)
		{
			return;
		}
		if (this.wasHeld && flag)
		{
			this.repeatAction += this.actionRepeatDelayReduction;
		}
		else
		{
			this.repeatAction = 1f;
		}
		if (this.IsLeftStick || this.IsRightStick || this.IsUpStick || this.IsDownStick || this.ButtonDown)
		{
			this.actionTime = Time.time;
		}
		if (this.OnAction != null)
		{
			this.OnAction();
		}
		this.wasHeld = flag;
		this.wasDownStick = this.IsDownStick;
		this.wasUpStick = this.IsUpStick;
		this.wasLeftStick = this.IsLeftStick;
		this.wasRightStick = this.IsRightStick;
	}

	// Token: 0x06004CB8 RID: 19640 RVA: 0x00199439 File Offset: 0x00197639
	public bool BuildValidationCheck()
	{
		if (this.uxSettings == null)
		{
			Debug.LogError("ControllerBehaviour must set UXSettings");
			return false;
		}
		return true;
	}

	// Token: 0x06004CB9 RID: 19641 RVA: 0x00199456 File Offset: 0x00197656
	public static ControllerBehaviour CreateNewControllerBehaviour(GameObject gameObject, UXSettings settings)
	{
		ControllerBehaviour controllerBehaviour = gameObject.AddComponent<ControllerBehaviour>();
		controllerBehaviour.uxSettings = settings;
		return controllerBehaviour;
	}

	// Token: 0x04005FD6 RID: 24534
	private float actionTime;

	// Token: 0x04005FD7 RID: 24535
	private float repeatAction = 1f;

	// Token: 0x04005FD8 RID: 24536
	[SerializeField]
	private UXSettings uxSettings;

	// Token: 0x04005FD9 RID: 24537
	[SerializeField]
	private float actionDelay = 0.5f;

	// Token: 0x04005FDA RID: 24538
	[SerializeField]
	private float actionRepeatDelayReduction = 0.5f;

	// Token: 0x04005FDB RID: 24539
	[Tooltip("Should the triggers modify the x axis like the sticks do?")]
	[SerializeField]
	private bool useTriggersAsSticks;

	// Token: 0x04005FDC RID: 24540
	private ControllerInputPoller poller;

	// Token: 0x04005FDD RID: 24541
	private bool wasLeftStick;

	// Token: 0x04005FDE RID: 24542
	private bool wasRightStick;

	// Token: 0x04005FDF RID: 24543
	private bool wasUpStick;

	// Token: 0x04005FE0 RID: 24544
	private bool wasDownStick;

	// Token: 0x04005FE1 RID: 24545
	private bool wasHeld;

	// Token: 0x02000BEC RID: 3052
	// (Invoke) Token: 0x06004CBC RID: 19644
	public delegate void OnActionEvent();
}
