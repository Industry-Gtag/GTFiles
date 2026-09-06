using System;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200090A RID: 2314
public class HoverboardVisual : MonoBehaviour, ICallBack
{
	// Token: 0x17000580 RID: 1408
	// (get) Token: 0x06003CA7 RID: 15527 RVA: 0x0014AF42 File Offset: 0x00149142
	// (set) Token: 0x06003CA8 RID: 15528 RVA: 0x0014AF4A File Offset: 0x0014914A
	public Color boardColor { get; private set; }

	// Token: 0x06003CA9 RID: 15529 RVA: 0x0014AF54 File Offset: 0x00149154
	private void Awake()
	{
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x17000581 RID: 1409
	// (get) Token: 0x06003CAA RID: 15530 RVA: 0x0014AF90 File Offset: 0x00149190
	// (set) Token: 0x06003CAB RID: 15531 RVA: 0x0014AF98 File Offset: 0x00149198
	public bool IsHeld { get; private set; }

	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x06003CAC RID: 15532 RVA: 0x0014AFA1 File Offset: 0x001491A1
	// (set) Token: 0x06003CAD RID: 15533 RVA: 0x0014AFA9 File Offset: 0x001491A9
	public bool IsLeftHanded { get; private set; }

	// Token: 0x17000583 RID: 1411
	// (get) Token: 0x06003CAE RID: 15534 RVA: 0x0014AFB2 File Offset: 0x001491B2
	// (set) Token: 0x06003CAF RID: 15535 RVA: 0x0014AFBA File Offset: 0x001491BA
	public Vector3 NominalLocalPosition { get; private set; }

	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x0014AFC3 File Offset: 0x001491C3
	// (set) Token: 0x06003CB1 RID: 15537 RVA: 0x0014AFCB File Offset: 0x001491CB
	public Quaternion NominalLocalRotation { get; private set; }

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x06003CB2 RID: 15538 RVA: 0x0014AFD4 File Offset: 0x001491D4
	private Transform NominalParentTransform
	{
		get
		{
			if (!this.IsHeld)
			{
				return base.transform.parent;
			}
			return (this.IsLeftHanded ? this.parentRig.leftHand : this.parentRig.rightHand).rigTarget.transform;
		}
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x0014B014 File Offset: 0x00149214
	public void SetIsHeld(bool isHeldLeftHanded, Vector3 localPosition, Quaternion localRotation, Color boardColor)
	{
		if (!this.isCallbackActive)
		{
			this.parentRig.AddLateUpdateCallback(this);
			this.isCallbackActive = true;
		}
		this.IsHeld = true;
		base.gameObject.SetActive(true);
		this.IsLeftHanded = isHeldLeftHanded;
		this.NominalLocalPosition = localPosition;
		this.NominalLocalRotation = localRotation;
		Transform nominalParentTransform = this.NominalParentTransform;
		this.interpolatedLocalPosition = nominalParentTransform.InverseTransformPoint(base.transform.position);
		this.interpolatedLocalRotation = nominalParentTransform.InverseTransformRotation(base.transform.rotation);
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(true);
		}
		this.colorMaterial.color = boardColor;
		this.boardColor = boardColor;
	}

	// Token: 0x06003CB4 RID: 15540 RVA: 0x0014B11D File Offset: 0x0014931D
	public void SetNotHeld(bool isLeftHanded)
	{
		this.IsLeftHanded = isLeftHanded;
		this.SetNotHeld();
	}

	// Token: 0x06003CB5 RID: 15541 RVA: 0x0014B12C File Offset: 0x0014932C
	public void SetNotHeld()
	{
		bool isHeld = this.IsHeld;
		base.gameObject.SetActive(false);
		this.IsHeld = false;
		this.interpolatedLocalPosition = base.transform.localPosition;
		this.interpolatedLocalRotation = base.transform.localRotation;
		this.positionLerpSpeed = (this.interpolatedLocalPosition - this.NominalLocalPosition).magnitude / this.lerpIntoHandDuration;
		float num;
		Vector3 vector;
		(Quaternion.Inverse(this.interpolatedLocalRotation) * this.NominalLocalRotation).ToAngleAxis(out num, out vector);
		this.rotationLerpSpeed = num / this.lerpIntoHandDuration;
		if (!isHeld)
		{
			base.transform.position = base.transform.parent.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = base.transform.parent.TransformRotation(this.NominalLocalRotation);
		}
		if (this.parentRig.isLocal)
		{
			GTPlayer.Instance.SetHoverActive(false);
		}
		this.hoverboardAudio.Stop();
	}

	// Token: 0x06003CB6 RID: 15542 RVA: 0x0014B234 File Offset: 0x00149434
	void ICallBack.CallBack()
	{
		Transform nominalParentTransform = this.NominalParentTransform;
		if ((this.interpolatedLocalPosition - this.NominalLocalPosition).IsShorterThan(0.01f))
		{
			base.transform.position = nominalParentTransform.TransformPoint(this.NominalLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.NominalLocalRotation);
			if (!this.IsHeld)
			{
				this.parentRig.RemoveLateUpdateCallback(this);
				this.isCallbackActive = false;
			}
		}
		else
		{
			this.interpolatedLocalPosition = Vector3.MoveTowards(this.interpolatedLocalPosition, this.NominalLocalPosition, this.positionLerpSpeed * Time.deltaTime);
			this.interpolatedLocalRotation = Quaternion.RotateTowards(this.interpolatedLocalRotation, this.NominalLocalRotation, this.rotationLerpSpeed * Time.deltaTime);
			base.transform.position = nominalParentTransform.TransformPoint(this.interpolatedLocalPosition);
			base.transform.rotation = nominalParentTransform.TransformRotation(this.interpolatedLocalRotation);
		}
		if (this.IsHeld)
		{
			if (this.parentRig.isLocal)
			{
				GTPlayer.Instance.SetHoverboardPosRot(base.transform.position, base.transform.rotation);
				return;
			}
			this.hoverboardAudio.UpdateAudioLoop(this.parentRig.LatestVelocity().magnitude, 0f, 0f, 0f);
		}
	}

	// Token: 0x06003CB7 RID: 15543 RVA: 0x0014B38A File Offset: 0x0014958A
	public void PlayGrindHaptic()
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, this.grindHapticStrength, this.grindHapticDuration);
		}
	}

	// Token: 0x06003CB8 RID: 15544 RVA: 0x0014B3B0 File Offset: 0x001495B0
	public void PlayCarveHaptic(float carveForce)
	{
		if (this.IsHeld)
		{
			GorillaTagger.Instance.StartVibration(this.IsLeftHanded, carveForce * this.carveHapticStrength, this.carveHapticDuration);
		}
	}

	// Token: 0x06003CB9 RID: 15545 RVA: 0x0014B3D8 File Offset: 0x001495D8
	public void ProxyGrabHandle(bool isLeftHand)
	{
		EquipmentInteractor.instance.UpdateHandEquipment(this.handlePosition, isLeftHand);
	}

	// Token: 0x06003CBA RID: 15546 RVA: 0x0014B3ED File Offset: 0x001495ED
	public void DropFreeBoard()
	{
		FreeHoverboardManager.instance.SendDropBoardRPC(base.transform.position, base.transform.rotation, this.velocityEstimator.linearVelocity, this.velocityEstimator.angularVelocity, this.boardColor);
	}

	// Token: 0x06003CBB RID: 15547 RVA: 0x0014B42B File Offset: 0x0014962B
	public void SetRaceDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.racePositionReadout.gameObject.SetActive(false);
			return;
		}
		this.racePositionReadout.gameObject.SetActive(true);
		this.racePositionReadout.text = text;
	}

	// Token: 0x06003CBC RID: 15548 RVA: 0x0014B464 File Offset: 0x00149664
	public void SetRaceLapsDisplay(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			this.raceLapsReadout.gameObject.SetActive(false);
			return;
		}
		this.raceLapsReadout.gameObject.SetActive(true);
		this.raceLapsReadout.text = text;
	}

	// Token: 0x04004D5F RID: 19807
	[SerializeField]
	private VRRig parentRig;

	// Token: 0x04004D60 RID: 19808
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04004D61 RID: 19809
	[SerializeField]
	[FormerlySerializedAs("audio")]
	private HoverboardAudio hoverboardAudio;

	// Token: 0x04004D62 RID: 19810
	[SerializeField]
	private HoverboardHandle handlePosition;

	// Token: 0x04004D63 RID: 19811
	[SerializeField]
	private float grindHapticStrength;

	// Token: 0x04004D64 RID: 19812
	[SerializeField]
	private float grindHapticDuration;

	// Token: 0x04004D65 RID: 19813
	[SerializeField]
	private float carveHapticStrength;

	// Token: 0x04004D66 RID: 19814
	[SerializeField]
	private float carveHapticDuration;

	// Token: 0x04004D67 RID: 19815
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04004D68 RID: 19816
	[SerializeField]
	private InteractionPoint handleInteractionPoint;

	// Token: 0x04004D69 RID: 19817
	[SerializeField]
	private TextMeshPro racePositionReadout;

	// Token: 0x04004D6A RID: 19818
	[SerializeField]
	private TextMeshPro raceLapsReadout;

	// Token: 0x04004D6B RID: 19819
	private Material colorMaterial;

	// Token: 0x04004D71 RID: 19825
	private Vector3 interpolatedLocalPosition;

	// Token: 0x04004D72 RID: 19826
	private Quaternion interpolatedLocalRotation;

	// Token: 0x04004D73 RID: 19827
	[SerializeField]
	private float lerpIntoHandDuration;

	// Token: 0x04004D74 RID: 19828
	private float positionLerpSpeed;

	// Token: 0x04004D75 RID: 19829
	private float rotationLerpSpeed;

	// Token: 0x04004D76 RID: 19830
	private bool isCallbackActive;
}
