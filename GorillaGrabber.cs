using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000920 RID: 2336
public class GorillaGrabber : MonoBehaviour
{
	// Token: 0x17000598 RID: 1432
	// (get) Token: 0x06003D15 RID: 15637 RVA: 0x0014C5F0 File Offset: 0x0014A7F0
	public bool isGrabbing
	{
		get
		{
			return this.currentGrabbable != null;
		}
	}

	// Token: 0x17000599 RID: 1433
	// (get) Token: 0x06003D16 RID: 15638 RVA: 0x0014C5FB File Offset: 0x0014A7FB
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	// Token: 0x1700059A RID: 1434
	// (get) Token: 0x06003D17 RID: 15639 RVA: 0x0014C603 File Offset: 0x0014A803
	public bool IsLeftHand
	{
		get
		{
			return this.XrNode == XRNode.LeftHand;
		}
	}

	// Token: 0x1700059B RID: 1435
	// (get) Token: 0x06003D18 RID: 15640 RVA: 0x0014C60E File Offset: 0x0014A80E
	public bool IsRightHand
	{
		get
		{
			return this.XrNode == XRNode.RightHand;
		}
	}

	// Token: 0x1700059C RID: 1436
	// (get) Token: 0x06003D19 RID: 15641 RVA: 0x0014C619 File Offset: 0x0014A819
	public GTPlayer Player
	{
		get
		{
			return this.player;
		}
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x0014C624 File Offset: 0x0014A824
	private void Start()
	{
		this.hapticStrengthActual = this.hapticStrength;
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<GTPlayer>();
		if (!this.player)
		{
			Debug.LogWarning("Gorilla Grabber Component has no player in hierarchy. Disabling this Gorilla Grabber");
			base.GetComponent<GorillaGrabber>().enabled = false;
		}
	}

	// Token: 0x06003D1B RID: 15643 RVA: 0x0014C678 File Offset: 0x0014A878
	public void CheckGrabber(bool initiateGrab)
	{
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab(null);
		}
		if (grabMomentary)
		{
			this.grabTimeStamp = Time.time;
		}
		if (initiateGrab && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.TryGrab(Time.time - this.grabTimeStamp < this.coyoteTimeDuration);
		}
		if (this.currentGrabbable != null && this.hapticStrengthActual > 0f)
		{
			GorillaTagger.Instance.DoVibration(this.xrNode, this.hapticStrengthActual, Time.deltaTime);
			this.hapticStrengthActual -= this.hapticDecay * Time.deltaTime;
		}
	}

	// Token: 0x06003D1C RID: 15644 RVA: 0x0014C737 File Offset: 0x0014A937
	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.TransformPoint(this.localGrabbedPosition)) > this.breakDistance;
	}

	// Token: 0x06003D1D RID: 15645 RVA: 0x0014C774 File Offset: 0x0014A974
	internal void Ungrab(IGorillaGrabable specificGrabbable = null)
	{
		if (specificGrabbable != null && specificGrabbable != this.currentGrabbable)
		{
			return;
		}
		this.currentGrabbable.OnGrabReleased(this);
		PlayerGameEvents.DroppedObject(this.currentGrabbable.name);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
		this.hapticStrengthActual = this.hapticStrength;
	}

	// Token: 0x06003D1E RID: 15646 RVA: 0x0014C7C8 File Offset: 0x0014A9C8
	private IGorillaGrabable TryGrab(bool momentary)
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * (this.grabRadius * this.player.scale), Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius * this.player.scale, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(out gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.FindClosestPoint(this.grabCastResults[i], base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null && (!gorillaGrabable.MomentaryGrabOnly() || momentary) && gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable.OnGrabbed(this, out this.currentGrabbedTransform, out this.localGrabbedPosition);
			PlayerGameEvents.GrabbedObject(gorillaGrabable.name);
		}
		if (gorillaGrabable != null && !gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable = null;
		}
		return gorillaGrabable;
	}

	// Token: 0x06003D1F RID: 15647 RVA: 0x0014C8DB File Offset: 0x0014AADB
	private Vector3 FindClosestPoint(Collider collider, Vector3 position)
	{
		if (collider is MeshCollider && !(collider as MeshCollider).convex)
		{
			return position;
		}
		return collider.ClosestPoint(position);
	}

	// Token: 0x06003D20 RID: 15648 RVA: 0x0014C8FC File Offset: 0x0014AAFC
	public void Inject(Transform currentGrabbableTransform, Vector3 localGrabbedPosition)
	{
		if (this.currentGrabbable != null)
		{
			this.Ungrab(null);
		}
		if (currentGrabbableTransform != null)
		{
			this.currentGrabbable = currentGrabbableTransform.GetComponent<IGorillaGrabable>();
			this.currentGrabbedTransform = currentGrabbableTransform;
			this.localGrabbedPosition = localGrabbedPosition;
			this.currentGrabbable.OnGrabbed(this, out this.currentGrabbedTransform, out localGrabbedPosition);
		}
	}

	// Token: 0x04004DCF RID: 19919
	private GTPlayer player;

	// Token: 0x04004DD0 RID: 19920
	[SerializeField]
	private XRNode xrNode = XRNode.LeftHand;

	// Token: 0x04004DD1 RID: 19921
	private AudioSource audioSource;

	// Token: 0x04004DD2 RID: 19922
	private Transform currentGrabbedTransform;

	// Token: 0x04004DD3 RID: 19923
	private Vector3 localGrabbedPosition;

	// Token: 0x04004DD4 RID: 19924
	private IGorillaGrabable currentGrabbable;

	// Token: 0x04004DD5 RID: 19925
	[SerializeField]
	private float grabRadius = 0.015f;

	// Token: 0x04004DD6 RID: 19926
	[SerializeField]
	private float breakDistance = 0.3f;

	// Token: 0x04004DD7 RID: 19927
	[SerializeField]
	private float hapticStrength = 0.2f;

	// Token: 0x04004DD8 RID: 19928
	private float hapticStrengthActual = 0.2f;

	// Token: 0x04004DD9 RID: 19929
	[SerializeField]
	private float hapticDecay;

	// Token: 0x04004DDA RID: 19930
	[SerializeField]
	private ParticleSystem gripEffects;

	// Token: 0x04004DDB RID: 19931
	private Collider[] grabCastResults = new Collider[32];

	// Token: 0x04004DDC RID: 19932
	private float grabTimeStamp;

	// Token: 0x04004DDD RID: 19933
	[SerializeField]
	private float coyoteTimeDuration = 0.25f;
}
