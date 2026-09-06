using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class CrittersLoudNoise : CrittersActor
{
	// Token: 0x060001F2 RID: 498 RVA: 0x0000B7A4 File Offset: 0x000099A4
	public override void OnEnable()
	{
		base.OnEnable();
		this.SetTimeEnabled();
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000B7B2 File Offset: 0x000099B2
	public void SpawnData(float _soundVolume, float _soundDuration, float _soundMultiplier, bool _soundEnabled)
	{
		this.soundVolume = _soundVolume;
		this.volumeFearAttractionMultiplier = _soundMultiplier;
		this.soundDuration = _soundDuration;
		this.soundEnabled = _soundEnabled;
		this.Initialize();
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000B7D8 File Offset: 0x000099D8
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.wasSoundEnabled = this.soundEnabled;
		if (PhotonNetwork.InRoom)
		{
			if (PhotonNetwork.Time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > PhotonNetwork.Time)
			{
				this.soundEnabled = false;
			}
		}
		else if ((double)Time.time > this.timeSoundEnabled + (double)this.soundDuration || this.timeSoundEnabled > (double)Time.time)
		{
			this.soundEnabled = false;
		}
		if (this.disableWhenSoundDisabled && !this.soundEnabled)
		{
			this.isEnabled = false;
			if (base.gameObject.activeSelf != this.isEnabled)
			{
				base.gameObject.SetActive(this.isEnabled);
			}
		}
		this.updatedSinceLastFrame = flag || this.wasSoundEnabled != this.soundEnabled || this.wasEnabled != this.isEnabled;
		return this.updatedSinceLastFrame;
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000B8DC File Offset: 0x00009ADC
	public override void ProcessRemote()
	{
		if (!this.wasEnabled && this.isEnabled)
		{
			this.SetTimeEnabled();
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000B8F4 File Offset: 0x00009AF4
	public void SetTimeEnabled()
	{
		if (PhotonNetwork.InRoom)
		{
			this.timeSoundEnabled = PhotonNetwork.Time;
			return;
		}
		this.timeSoundEnabled = (double)Time.time;
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000B918 File Offset: 0x00009B18
	public override void CalculateFear(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseFear(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000B994 File Offset: 0x00009B94
	public override void CalculateAttraction(CrittersPawn critter, float multiplier)
	{
		if (this.soundEnabled)
		{
			if (this.soundDuration == 0f)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * multiplier, this);
				return;
			}
			if ((PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) - this.timeSoundEnabled < (double)this.soundDuration)
			{
				critter.IncreaseAttraction(this.soundVolume * this.volumeFearAttractionMultiplier * Time.deltaTime * multiplier, this);
			}
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000BA10 File Offset: 0x00009C10
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		float num;
		float num2;
		bool flag;
		float num3;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num2) & CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out num3)))
		{
			return false;
		}
		this.soundVolume = num.GetFinite();
		this.soundDuration = num2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = num3.GetFinite();
		return true;
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000BA8C File Offset: 0x00009C8C
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.soundVolume);
		stream.SendNext(this.soundDuration);
		stream.SendNext(this.soundEnabled);
		stream.SendNext(this.volumeFearAttractionMultiplier);
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000BAE4 File Offset: 0x00009CE4
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.soundVolume);
		objList.Add(this.soundDuration);
		objList.Add(this.soundEnabled);
		objList.Add(this.volumeFearAttractionMultiplier);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000BB47 File Offset: 0x00009D47
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 4;
	}

	// Token: 0x060001FD RID: 509 RVA: 0x0000BB54 File Offset: 0x00009D54
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		float num;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		float num2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out num2))
		{
			return this.TotalActorDataLength();
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex + 2], out flag))
		{
			return this.TotalActorDataLength();
		}
		float num3;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 3], out num3))
		{
			return this.TotalActorDataLength();
		}
		this.soundVolume = num.GetFinite();
		this.soundDuration = num2.GetFinite();
		this.soundEnabled = flag;
		this.volumeFearAttractionMultiplier = num3.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x060001FE RID: 510 RVA: 0x0000BBF0 File Offset: 0x00009DF0
	public void PlayHandTapLocal(bool isLeft)
	{
		this.timeSoundEnabled = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time));
		this.soundEnabled = true;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x0000BC13 File Offset: 0x00009E13
	public void PlayHandTapRemote(double serverTime, bool isLeft)
	{
		this.timeSoundEnabled = serverTime;
		this.soundEnabled = true;
	}

	// Token: 0x06000200 RID: 512 RVA: 0x0000BC23 File Offset: 0x00009E23
	public void PlayVoiceSpeechLocal(double serverTime, float duration, float volume)
	{
		this.soundDuration = duration;
		this.timeSoundEnabled = serverTime;
		this.soundVolume = volume;
		this.soundEnabled = true;
	}

	// Token: 0x04000237 RID: 567
	public float soundVolume;

	// Token: 0x04000238 RID: 568
	public float volumeFearAttractionMultiplier;

	// Token: 0x04000239 RID: 569
	public float soundDuration;

	// Token: 0x0400023A RID: 570
	public double timeSoundEnabled;

	// Token: 0x0400023B RID: 571
	public bool soundEnabled;

	// Token: 0x0400023C RID: 572
	private bool wasSoundEnabled;

	// Token: 0x0400023D RID: 573
	public bool disableWhenSoundDisabled;
}
