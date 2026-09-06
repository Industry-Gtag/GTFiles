using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class MenagerieCritter : MonoBehaviour, IHoldableObject, IEyeScannable
{
	// Token: 0x1700003C RID: 60
	// (get) Token: 0x0600036A RID: 874 RVA: 0x0001453E File Offset: 0x0001273E
	public Menagerie.CritterData CritterData
	{
		get
		{
			return this._critterData;
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x0600036B RID: 875 RVA: 0x00014546 File Offset: 0x00012746
	// (set) Token: 0x0600036C RID: 876 RVA: 0x00014550 File Offset: 0x00012750
	public MenagerieSlot Slot
	{
		get
		{
			return this._slot;
		}
		set
		{
			if (value == this._slot)
			{
				return;
			}
			if (this._slot && this._slot.critter == this)
			{
				this._slot.critter = null;
			}
			this._slot = value;
			if (this._slot)
			{
				this._slot.critter = this;
			}
		}
	}

	// Token: 0x0600036D RID: 877 RVA: 0x000145B8 File Offset: 0x000127B8
	private void Update()
	{
		this.UpdateAnimation();
	}

	// Token: 0x0600036E RID: 878 RVA: 0x000145C0 File Offset: 0x000127C0
	public void ApplyCritterData(Menagerie.CritterData critterData)
	{
		this._critterData = critterData;
		this._critterConfiguration = this._critterData.GetConfiguration();
		this._critterData.instance = this;
		this._critterData.GetConfiguration().ApplyVisualsTo(this.visuals, false);
		this.visuals.SetAppearance(this._critterData.appearance);
		this._animRoot = this.visuals.bodyRoot;
		this._bodyScale = this._animRoot.localScale;
		this.PlayAnimation(this.heldAnimation, global::UnityEngine.Random.value);
	}

	// Token: 0x0600036F RID: 879 RVA: 0x00014654 File Offset: 0x00012854
	private void PlayAnimation(CrittersAnim anim, float time = 0f)
	{
		this._currentAnim = anim;
		this._currentAnimTime = time;
		if (this._currentAnim == null)
		{
			this._animRoot.localPosition = Vector3.zero;
			this._animRoot.localRotation = Quaternion.identity;
			this._animRoot.localScale = this._bodyScale;
		}
	}

	// Token: 0x06000370 RID: 880 RVA: 0x000146A8 File Offset: 0x000128A8
	private void UpdateAnimation()
	{
		if (this._currentAnim != null)
		{
			this._currentAnimTime += Time.deltaTime * this._currentAnim.playSpeed;
			this._currentAnimTime %= 1f;
			float num = this._currentAnim.squashAmount.Evaluate(this._currentAnimTime);
			float num2 = this._currentAnim.forwardOffset.Evaluate(this._currentAnimTime);
			float num3 = this._currentAnim.horizontalOffset.Evaluate(this._currentAnimTime);
			float num4 = this._currentAnim.verticalOffset.Evaluate(this._currentAnimTime);
			this._animRoot.localPosition = Vector3.Scale(this._bodyScale, new Vector3(num3, num4, num2));
			float num5 = 1f - num;
			num5 *= 0.5f;
			num5 += 1f;
			this._animRoot.localScale = Vector3.Scale(this._bodyScale, new Vector3(num5, num, num5));
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x06000371 RID: 881 RVA: 0x00002076 File Offset: 0x00000276
	public bool TwoHanded
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000372 RID: 882 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
	{
	}

	// Token: 0x06000373 RID: 883 RVA: 0x000147A8 File Offset: 0x000129A8
	public void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		this.isHeld = true;
		this.isHeldLeftHand = grabbingHand == EquipmentInteractor.instance.leftHand;
		if (this.grabbedHaptics)
		{
			CrittersManager.PlayHaptics(this.grabbedHaptics, this.grabbedHapticsStrength, this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(true);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		base.transform.parent = grabbingHand.transform;
		this.isHeld = true;
		this.heldBy = grabbingHand;
		Action onDataChange = this.OnDataChange;
		if (onDataChange == null)
		{
			return;
		}
		onDataChange();
	}

	// Token: 0x06000374 RID: 884 RVA: 0x00014854 File Offset: 0x00012A54
	public bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
		{
			return false;
		}
		if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
		{
			return false;
		}
		if (this.grabbedHaptics)
		{
			CrittersManager.StopHaptics(this.isHeldLeftHand);
		}
		if (this.grabbedFX)
		{
			this.grabbedFX.SetActive(false);
		}
		EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
		this.isHeld = false;
		this.isHeldLeftHand = false;
		Action<MenagerieCritter> onReleased = this.OnReleased;
		if (onReleased != null)
		{
			onReleased(this);
		}
		Action onDataChange = this.OnDataChange;
		if (onDataChange != null)
		{
			onDataChange();
		}
		this.ResetToTransform();
		return true;
	}

	// Token: 0x06000375 RID: 885 RVA: 0x00014927 File Offset: 0x00012B27
	public void ResetToTransform()
	{
		base.transform.parent = this._slot.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = quaternion.identity;
	}

	// Token: 0x06000376 RID: 886 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void DropItemCleanup()
	{
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000377 RID: 887 RVA: 0x0001104F File Offset: 0x0000F24F
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000378 RID: 888 RVA: 0x00014964 File Offset: 0x00012B64
	Vector3 IEyeScannable.Position
	{
		get
		{
			return this.bodyCollider.bounds.center;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x06000379 RID: 889 RVA: 0x00014984 File Offset: 0x00012B84
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return this.bodyCollider.bounds;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x0600037A RID: 890 RVA: 0x00014991 File Offset: 0x00012B91
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.BuildEyeScannerData();
		}
	}

	// Token: 0x0600037B RID: 891 RVA: 0x00014999 File Offset: 0x00012B99
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
	}

	// Token: 0x0600037C RID: 892 RVA: 0x000149A1 File Offset: 0x00012BA1
	public void OnDisable()
	{
		EyeScannerMono.Unregister(this);
	}

	// Token: 0x0600037D RID: 893 RVA: 0x000149AC File Offset: 0x00012BAC
	private IList<KeyValueStringPair> BuildEyeScannerData()
	{
		this.eyeScanData[0] = new KeyValueStringPair("Name", this._critterConfiguration.critterName);
		this.eyeScanData[1] = new KeyValueStringPair("Type", this._critterConfiguration.animalType.ToString());
		this.eyeScanData[2] = new KeyValueStringPair("Temperament", this._critterConfiguration.behaviour.temperament);
		this.eyeScanData[3] = new KeyValueStringPair("Habitat", this._critterConfiguration.biome.GetHabitatDescription());
		this.eyeScanData[4] = new KeyValueStringPair("Size", this.visuals.Appearance.size.ToString("0.00"));
		this.eyeScanData[5] = new KeyValueStringPair("State", this.GetCurrentStateName());
		return this.eyeScanData;
	}

	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600037E RID: 894 RVA: 0x00014AA8 File Offset: 0x00012CA8
	// (remove) Token: 0x0600037F RID: 895 RVA: 0x00014AE0 File Offset: 0x00012CE0
	public event Action OnDataChange;

	// Token: 0x06000380 RID: 896 RVA: 0x00014B15 File Offset: 0x00012D15
	private string GetCurrentStateName()
	{
		if (!this.isHeld)
		{
			return "Content";
		}
		return "Happy";
	}

	// Token: 0x06000382 RID: 898 RVA: 0x000066D3 File Offset: 0x000048D3
	GameObject IHoldableObject.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00014B5B File Offset: 0x00012D5B
	string IHoldableObject.get_name()
	{
		return base.name;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00014B63 File Offset: 0x00012D63
	void IHoldableObject.set_name(string value)
	{
		base.name = value;
	}

	// Token: 0x040003F2 RID: 1010
	public CritterVisuals visuals;

	// Token: 0x040003F3 RID: 1011
	public Collider bodyCollider;

	// Token: 0x040003F4 RID: 1012
	[Header("Feedback")]
	public CrittersAnim heldAnimation;

	// Token: 0x040003F5 RID: 1013
	public AudioClip grabbedHaptics;

	// Token: 0x040003F6 RID: 1014
	public float grabbedHapticsStrength = 1f;

	// Token: 0x040003F7 RID: 1015
	public GameObject grabbedFX;

	// Token: 0x040003F8 RID: 1016
	private CrittersAnim _currentAnim;

	// Token: 0x040003F9 RID: 1017
	private float _currentAnimTime;

	// Token: 0x040003FA RID: 1018
	private Transform _animRoot;

	// Token: 0x040003FB RID: 1019
	private Vector3 _bodyScale;

	// Token: 0x040003FC RID: 1020
	public MenagerieCritter.MenagerieCritterState currentState = MenagerieCritter.MenagerieCritterState.Displaying;

	// Token: 0x040003FD RID: 1021
	private CritterConfiguration _critterConfiguration;

	// Token: 0x040003FE RID: 1022
	private Menagerie.CritterData _critterData;

	// Token: 0x040003FF RID: 1023
	private MenagerieSlot _slot;

	// Token: 0x04000400 RID: 1024
	private List<GorillaGrabber> activeGrabbers = new List<GorillaGrabber>();

	// Token: 0x04000401 RID: 1025
	private GameObject heldBy;

	// Token: 0x04000402 RID: 1026
	private bool isHeld;

	// Token: 0x04000403 RID: 1027
	private bool isHeldLeftHand;

	// Token: 0x04000404 RID: 1028
	public Action<MenagerieCritter> OnReleased;

	// Token: 0x04000405 RID: 1029
	private KeyValueStringPair[] eyeScanData = new KeyValueStringPair[6];

	// Token: 0x0200008A RID: 138
	public enum MenagerieCritterState
	{
		// Token: 0x04000408 RID: 1032
		Donating,
		// Token: 0x04000409 RID: 1033
		Displaying
	}
}
