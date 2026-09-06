using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class SnowballMaker : MonoBehaviourPostTick
{
	// Token: 0x17000142 RID: 322
	// (get) Token: 0x06000DAF RID: 3503 RVA: 0x0004AFF9 File Offset: 0x000491F9
	// (set) Token: 0x06000DB0 RID: 3504 RVA: 0x0004B000 File Offset: 0x00049200
	public static SnowballMaker leftHandInstance { get; private set; }

	// Token: 0x17000143 RID: 323
	// (get) Token: 0x06000DB1 RID: 3505 RVA: 0x0004B008 File Offset: 0x00049208
	// (set) Token: 0x06000DB2 RID: 3506 RVA: 0x0004B00F File Offset: 0x0004920F
	public static SnowballMaker rightHandInstance { get; private set; }

	// Token: 0x17000144 RID: 324
	// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x0004B017 File Offset: 0x00049217
	// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x0004B01F File Offset: 0x0004921F
	public SnowballThrowable[] snowballs { get; private set; }

	// Token: 0x06000DB5 RID: 3509 RVA: 0x0004B028 File Offset: 0x00049228
	private void Awake()
	{
		if (this.snowballs == null)
		{
			this.snowballs = new SnowballThrowable[0];
		}
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (!snowballThrowable.IsNull() && snowballThrowable is GrowingSnowballThrowable)
			{
				this.m_hasGrowingSnowball = true;
			}
		}
		if (this.isLeftHand)
		{
			if (SnowballMaker.leftHandInstance == null)
			{
				SnowballMaker.leftHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
		else
		{
			if (SnowballMaker.rightHandInstance == null)
			{
				SnowballMaker.rightHandInstance = this;
				return;
			}
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x06000DB6 RID: 3510 RVA: 0x0004B0C0 File Offset: 0x000492C0
	private void Start()
	{
		this.handTransform = (this.isLeftHand ? GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.leftHandTransform : GorillaTagger.Instance.offlineVRRig.myBodyDockPositions.rightHandTransform);
	}

	// Token: 0x06000DB7 RID: 3511 RVA: 0x0004B0FC File Offset: 0x000492FC
	internal void SetupThrowables(SnowballThrowable[] newThrowables)
	{
		this.snowballs = newThrowables;
		for (int i = 0; i < this.snowballs.Length; i++)
		{
			if (!(this.snowballs[i] == null))
			{
				for (int j = 0; j < this.snowballs[i].matDataIndexes.Count; j++)
				{
					this.matSnowballLookup.TryAdd(this.snowballs[i].matDataIndexes[j], this.snowballs[i]);
				}
			}
		}
	}

	// Token: 0x06000DB8 RID: 3512 RVA: 0x0004B178 File Offset: 0x00049378
	public override void PostTick()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (BuilderPieceInteractor.instance != null && BuilderPieceInteractor.instance.BlockSnowballCreation())
		{
			return;
		}
		if (!GTPlayer.hasInstance || !EquipmentInteractor.hasInstance || !GorillaTagger.hasInstance || !GorillaTagger.Instance.offlineVRRig)
		{
			return;
		}
		int materialTouchIndex = GTPlayer.Instance.GetMaterialTouchIndex(this.isLeftHand);
		if (materialTouchIndex == 0)
		{
			if (Time.time > this.lastGroundContactTime + this.snowballCreationCooldownTime)
			{
				this.requiresFreshMaterialContact = false;
			}
			return;
		}
		this.lastGroundContactTime = Time.time;
		this.InitializeSnowballFromMatIndex(materialTouchIndex);
		EquipmentInteractor instance = EquipmentInteractor.instance;
		bool flag = (this.isLeftHand ? instance.leftHandHeldEquipment : instance.rightHandHeldEquipment) != null;
		bool flag2 = (this.isLeftHand ? instance.isLeftGrabbing : instance.isRightGrabbing);
		bool flag3 = (this.isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab);
		bool flag4 = false;
		if (flag2 && !flag3 && !this.requiresFreshMaterialContact)
		{
			int num = -1;
			for (int i = 0; i < this.snowballs.Length; i++)
			{
				SnowballThrowable snowballThrowable = this.snowballs[i];
				if (!(snowballThrowable == null) && snowballThrowable.gameObject.activeSelf)
				{
					num = i;
					break;
				}
			}
			SnowballThrowable snowballThrowable2 = ((num > -1) ? this.snowballs[num] : null);
			GrowingSnowballThrowable growingSnowballThrowable = snowballThrowable2 as GrowingSnowballThrowable;
			bool flag5 = (this.isLeftHand ? (!ConnectedControllerHandler.Instance.RightValid) : (!ConnectedControllerHandler.Instance.LeftValid));
			if (growingSnowballThrowable != null && (!GrowingSnowballThrowable.twoHandedSnowballGrowing || flag5 || flag4))
			{
				if (snowballThrowable2.matDataIndexes.Contains(materialTouchIndex))
				{
					growingSnowballThrowable.IncreaseSize(1);
					GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
					this.requiresFreshMaterialContact = true;
					return;
				}
			}
			else if (!flag)
			{
				SnowballThrowable snowballThrowable3;
				if (!this.matSnowballLookup.TryGetValue(materialTouchIndex, out snowballThrowable3))
				{
					return;
				}
				Transform transform = snowballThrowable3.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable3.SpawnOffset;
				snowballThrowable3.SetSnowballActiveLocal(true);
				snowballThrowable3.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength * 0.5f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				this.requiresFreshMaterialContact = true;
			}
		}
	}

	// Token: 0x06000DB9 RID: 3513 RVA: 0x0004B414 File Offset: 0x00049614
	public bool TryCreateSnowball(int materialIndex, out SnowballThrowable result)
	{
		EquipmentInteractor instance = EquipmentInteractor.instance;
		if (this.isLeftHand ? instance.disableLeftGrab : instance.disableRightGrab)
		{
			result = null;
			return false;
		}
		this.InitializeSnowballFromMatIndex(materialIndex);
		foreach (SnowballThrowable snowballThrowable in this.snowballs)
		{
			if (!(snowballThrowable == null) && snowballThrowable.matDataIndexes.Contains(materialIndex))
			{
				Transform transform = snowballThrowable.transform;
				Transform transform2 = this.handTransform;
				XformOffset spawnOffset = snowballThrowable.SpawnOffset;
				snowballThrowable.SetSnowballActiveLocal(true);
				snowballThrowable.velocityEstimator = this.velocityEstimator;
				transform.position = transform2.TransformPoint(spawnOffset.pos);
				transform.rotation = transform2.rotation * spawnOffset.rot;
				GorillaTagger.Instance.StartVibration(this.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 8f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
				result = snowballThrowable;
				return true;
			}
		}
		result = null;
		return false;
	}

	// Token: 0x06000DBA RID: 3514 RVA: 0x0004B518 File Offset: 0x00049718
	private void InitializeSnowballFromMatIndex(int matIndex)
	{
		string text;
		if (CosmeticsV2Spawner_Dirty.GetThrowableIDFromMaterialIndex(this.isLeftHand, matIndex, out text))
		{
			VRRig.LocalRig.cosmeticsObjectRegistry.Cosmetic(text);
		}
	}

	// Token: 0x04001055 RID: 4181
	public bool isLeftHand;

	// Token: 0x04001057 RID: 4183
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001058 RID: 4184
	private float snowballCreationCooldownTime = 0.1f;

	// Token: 0x04001059 RID: 4185
	private float lastGroundContactTime;

	// Token: 0x0400105A RID: 4186
	private bool requiresFreshMaterialContact;

	// Token: 0x0400105B RID: 4187
	private Transform handTransform;

	// Token: 0x0400105C RID: 4188
	private Dictionary<int, SnowballThrowable> matSnowballLookup = new Dictionary<int, SnowballThrowable>();

	// Token: 0x0400105D RID: 4189
	private Dictionary<int, SnowballThrowable> snowballByThrowableIndex = new Dictionary<int, SnowballThrowable>();

	// Token: 0x0400105E RID: 4190
	private Dictionary<int, string> snowballPlayfabIdByThrowableIndex = new Dictionary<int, string>();

	// Token: 0x0400105F RID: 4191
	private Dictionary<int, string> snowballPlayfabIdByMaterialIndex = new Dictionary<int, string>();

	// Token: 0x04001060 RID: 4192
	private bool m_hasGrowingSnowball;
}
