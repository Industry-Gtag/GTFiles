using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class CrittersFood : CrittersActor
{
	// Token: 0x060001DF RID: 479 RVA: 0x0000B31D File Offset: 0x0000951D
	public override void Initialize()
	{
		base.Initialize();
		this.currentFood = this.maxFood;
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000B334 File Offset: 0x00009534
	public void SpawnData(float _maxFood, float _currentFood, float _startingSize)
	{
		this.maxFood = _maxFood;
		this.currentFood = _currentFood;
		this.startingSize = _startingSize;
		this.currentSize = this.currentFood / this.maxFood * this.startingSize;
		this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x0000B394 File Offset: 0x00009594
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.ProcessFood();
		bool flag2 = Mathf.FloorToInt(this.currentFood) != this.lastFood;
		this.lastFood = Mathf.FloorToInt(this.currentFood);
		if (this.currentFood == 0f && this.disableWhenEmpty)
		{
			this.isEnabled = false;
		}
		if (base.gameObject.activeSelf != this.isEnabled)
		{
			base.gameObject.SetActive(this.isEnabled);
		}
		this.updatedSinceLastFrame = flag || flag2 || this.wasEnabled != this.isEnabled;
		return this.updatedSinceLastFrame;
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x0000B456 File Offset: 0x00009656
	public override void ProcessRemote()
	{
		base.ProcessRemote();
		if (!this.isEnabled)
		{
			return;
		}
		this.ProcessFood();
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000B470 File Offset: 0x00009670
	public void ProcessFood()
	{
		if (this.currentSize != this.currentFood / this.maxFood * this.startingSize)
		{
			this.currentSize = this.currentFood / this.maxFood * this.startingSize;
			this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
			if (this.storeCollider != null)
			{
				this.storeCollider.radius = this.currentSize / 2f;
			}
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x0000B4FA File Offset: 0x000096FA
	public void Feed(float amountEaten)
	{
		this.currentFood = Mathf.Max(0f, this.currentFood - amountEaten);
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x0000B514 File Offset: 0x00009714
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		int num;
		float num2;
		float num3;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num3)))
		{
			return false;
		}
		this.currentFood = (float)num;
		this.maxFood = num2.GetFinite();
		this.startingSize = num3.GetFinite();
		return true;
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000B578 File Offset: 0x00009778
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(Mathf.FloorToInt(this.currentFood));
		stream.SendNext(this.maxFood);
		stream.SendNext(this.startingSize);
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x0000B5C4 File Offset: 0x000097C4
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(Mathf.FloorToInt(this.currentFood));
		objList.Add(this.maxFood);
		objList.Add(this.startingSize);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000B61A File Offset: 0x0000981A
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 3;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x0000B624 File Offset: 0x00009824
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		float num2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out num2))
		{
			return this.TotalActorDataLength();
		}
		float num3;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 2], out num3))
		{
			return this.TotalActorDataLength();
		}
		this.currentFood = (float)num;
		this.maxFood = num2.GetFinite();
		this.startingSize = num3.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x04000223 RID: 547
	public float maxFood;

	// Token: 0x04000224 RID: 548
	public float currentFood;

	// Token: 0x04000225 RID: 549
	private int lastFood;

	// Token: 0x04000226 RID: 550
	public float startingSize;

	// Token: 0x04000227 RID: 551
	public float currentSize;

	// Token: 0x04000228 RID: 552
	public Transform food;

	// Token: 0x04000229 RID: 553
	public bool disableWhenEmpty = true;
}
