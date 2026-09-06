using System;
using GorillaLocomotion;
using GorillaTag.Gravity;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000CEE RID: 3310
public class StickControlledGravity : MonoBehaviour
{
	// Token: 0x060051EF RID: 20975 RVA: 0x001B36AB File Offset: 0x001B18AB
	private void Start()
	{
		this.zone = base.GetComponent<PersonalGravityZone>();
		if (base.isActiveAndEnabled)
		{
			this.Register();
		}
	}

	// Token: 0x060051F0 RID: 20976 RVA: 0x001B36C7 File Offset: 0x001B18C7
	private void OnEnable()
	{
		if (this.zone != null)
		{
			this.Register();
		}
	}

	// Token: 0x060051F1 RID: 20977 RVA: 0x001B36DD File Offset: 0x001B18DD
	private void OnDisable()
	{
		if (this.zone != null)
		{
			this.Unregister();
		}
	}

	// Token: 0x060051F2 RID: 20978 RVA: 0x001B36F3 File Offset: 0x001B18F3
	private void Register()
	{
		StickControlledGravity.Instance = this;
		ControllerInputPoller.AddUpdateCallback(new Action(this.OnInputUpdate));
	}

	// Token: 0x060051F3 RID: 20979 RVA: 0x001B370C File Offset: 0x001B190C
	private void Unregister()
	{
		StickControlledGravity.Instance = null;
		ControllerInputPoller.RemoveUpdateCallback(new Action(this.OnInputUpdate));
	}

	// Token: 0x060051F4 RID: 20980 RVA: 0x001B3728 File Offset: 0x001B1928
	private void OnInputUpdate()
	{
		if (GTPlayerTransform.Instance.GravityZonesCount == 0)
		{
			return;
		}
		Vector2 vector = ControllerInputPoller.Primary2DAxis(this.stickHand);
		if (!this.enableXAxis)
		{
			vector.x = 0f;
		}
		if (vector.magnitude < this.deadzone)
		{
			this.triggered = false;
			return;
		}
		if (this.triggered)
		{
			return;
		}
		this.triggered = true;
		if (this.enableXAxis && PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GRAVDASH_FLIP_X))
		{
			vector.x = -vector.x;
		}
		if (PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GRAVDASH_FLIP_Y))
		{
			vector.y = -vector.y;
		}
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		Vector3 vector2;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			vector2 = ((vector.x > 0f) ? transform.right : (-transform.right));
		}
		else
		{
			Vector3 vector3 = Vector3.Cross(transform.right, GTPlayerTransform.PhysicsUp);
			vector2 = ((vector.y > 0f) ? vector3 : (-vector3));
		}
		GTPlayerTransform.Instance.SetPersonalGravityDirection(-StickControlledGravity.SnapToAxis(vector2));
	}

	// Token: 0x060051F5 RID: 20981 RVA: 0x001B3848 File Offset: 0x001B1A48
	private static Vector3 SnapToAxis(Vector3 v)
	{
		float num = Mathf.Abs(v.x);
		float num2 = Mathf.Abs(v.y);
		float num3 = Mathf.Abs(v.z);
		if (num >= num2 && num >= num3)
		{
			return new Vector3(Mathf.Sign(v.x), 0f, 0f);
		}
		if (num2 >= num3)
		{
			return new Vector3(0f, Mathf.Sign(v.y), 0f);
		}
		return new Vector3(0f, 0f, Mathf.Sign(v.z));
	}

	// Token: 0x04006447 RID: 25671
	[SerializeField]
	private float deadzone = 0.5f;

	// Token: 0x04006448 RID: 25672
	[SerializeField]
	private XRNode stickHand = XRNode.LeftHand;

	// Token: 0x04006449 RID: 25673
	[SerializeField]
	private bool enableXAxis;

	// Token: 0x0400644A RID: 25674
	private bool triggered;

	// Token: 0x0400644B RID: 25675
	private PersonalGravityZone zone;

	// Token: 0x0400644C RID: 25676
	[OnEnterPlay_SetNull]
	public static StickControlledGravity Instance;
}
