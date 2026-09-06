using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using GorillaExtensions;
using GorillaNetworking;
using GorillaNetworking.Store;
using TMPro;
using UnityEngine;

namespace FXP
{
	// Token: 0x02001081 RID: 4225
	[Obsolete("CosmeticItemPrefab is deprecated, if we want to use this we need services to re-activate a webservice that was called gt-featureditem-dev.")]
	public class CosmeticItemPrefab : MonoBehaviour
	{
		// Token: 0x06006971 RID: 26993 RVA: 0x0021EF47 File Offset: 0x0021D147
		private void Awake()
		{
			this.JonsAwakeCode();
		}

		// Token: 0x06006972 RID: 26994 RVA: 0x0021EF50 File Offset: 0x0021D150
		private void JonsAwakeCode()
		{
			this.lastUpdated = -this.updateClock;
			this.isValid = this.goPedestal && this.goMannequin && this.goCosmeticItem && this.goCosmeticItemNameplate && this.goClock && this.goPreviewMode && this.goAttractMode && this.goPurchaseMode;
			this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			this.goAttractModeSFX = this.goAttractMode.transform.FindChildRecursive("SFXAttractMode").GetComponent<AudioSource>();
			this.goPurchaseModeSFX = this.goPurchaseMode.transform.FindChildRecursive("SFXPurchaseMode").GetComponent<AudioSource>();
			this.goAttractModeVFX = this.goAttractMode.transform.FindChildRecursive("VFXAttractMode").GetComponent<ParticleSystem>();
			this.goPurchaseModeVFX = this.goPurchaseMode.transform.FindChildRecursive("VFXPurchaseMode").GetComponent<ParticleSystem>();
			this.clockTextMesh = this.goClock.GetComponent<TextMeshPro>();
			this.clockTextMeshIsValid = this.clockTextMesh != null;
			if (this.clockTextMeshIsValid)
			{
				this.defaultCountdownTextTemplate = this.clockTextMesh.text;
			}
			this.isValid = this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX;
		}

		// Token: 0x06006973 RID: 26995 RVA: 0x0021F0D9 File Offset: 0x0021D2D9
		private void OnDisable()
		{
			if (StoreUpdater.instance != null)
			{
				this.countdownTimerCoRoutine = null;
				this.StopCountdownCoroutine();
				StoreUpdater.instance.PedestalAsleep(this);
			}
		}

		// Token: 0x06006974 RID: 26996 RVA: 0x0021F104 File Offset: 0x0021D304
		private void OnEnable()
		{
			if (this.goPreviewModeSFX == null)
			{
				this.goPreviewModeSFX = this.goPreviewMode.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goAttractModeSFX == null)
			{
				this.goAttractModeSFX = this.goAttractMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			if (this.goPurchaseModeSFX == null)
			{
				this.goPurchaseModeSFX = this.goPurchaseMode.transform.transform.GetComponentInChildren<AudioSource>();
			}
			this.isValid = this.goPreviewModeSFX && this.goAttractModeSFX && this.goPurchaseModeSFX;
			if (StoreUpdater.instance != null)
			{
				StoreUpdater.instance.PedestalAwakened(this);
			}
		}

		// Token: 0x06006975 RID: 26997 RVA: 0x0021F1D4 File Offset: 0x0021D3D4
		public void SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode NewDisplayMode)
		{
			if (!this.isValid)
			{
				return;
			}
			if (NewDisplayMode.Equals(CosmeticItemPrefab.EDisplayMode.NULL))
			{
				return;
			}
			if (NewDisplayMode == this.currentDisplayMode)
			{
				return;
			}
			switch (NewDisplayMode)
			{
			case CosmeticItemPrefab.EDisplayMode.HIDDEN:
			{
				this.goPedestal.SetActive(false);
				this.goMannequin.SetActive(false);
				this.goCosmeticItem.SetActive(false);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				AudioSource audioSource = this.goPreviewModeSFX;
				if (audioSource != null)
				{
					audioSource.GTStop();
				}
				this.goAttractMode.SetActive(false);
				AudioSource audioSource2 = this.goAttractModeSFX;
				if (audioSource2 != null)
				{
					audioSource2.GTStop();
				}
				this.goPurchaseMode.SetActive(false);
				AudioSource audioSource3 = this.goPurchaseModeSFX;
				if (audioSource3 != null)
				{
					audioSource3.GTStop();
				}
				this.StopPreviewTimer();
				this.StopAttractTimer();
				break;
			}
			case CosmeticItemPrefab.EDisplayMode.PREVIEW:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(true);
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goPreviewMode.SetActive(true);
				this.goPreviewModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.ATTRACT:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(true);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.goAttractMode.SetActive(true);
				this.goAttractModeSFX.GTPlay();
				this.StopPreviewTimer();
				this.StartAttractTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.PURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(true);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(true);
				this.goPurchaseModeSFX.GTPlay();
				this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = "Purchased!";
				this.StopPreviewTimer();
				break;
			case CosmeticItemPrefab.EDisplayMode.POSTPURCHASE:
				this.goPedestal.SetActive(true);
				this.goMannequin.SetActive(true);
				this.goCosmeticItem.SetActive(true);
				this.goCosmeticItemNameplate.SetActive(false);
				this.goClock.SetActive(false);
				this.goPreviewMode.SetActive(false);
				this.goPreviewModeSFX.GTStop();
				this.goAttractMode.SetActive(false);
				this.goAttractModeSFX.GTStop();
				this.goPurchaseMode.SetActive(false);
				this.goPurchaseModeSFX.GTStop();
				this.StopPreviewTimer();
				break;
			}
			this.currentDisplayMode = NewDisplayMode;
		}

		// Token: 0x06006976 RID: 26998 RVA: 0x0021F522 File Offset: 0x0021D722
		private void Update()
		{
			if (Time.time > this.lastUpdated + this.updateClock)
			{
				this.lastUpdated = Time.time;
				this.UpdateClock();
			}
		}

		// Token: 0x06006977 RID: 26999 RVA: 0x0021F54C File Offset: 0x0021D74C
		private void UpdateClock()
		{
			if (this.currentUpdateEvent != null && this.clockTextMeshIsValid && this.clockTextMesh.isActiveAndEnabled)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				this.clockTextMesh.text = CountdownText.GetTimeDisplay(timeSpan, this.defaultCountdownTextTemplate);
			}
		}

		// Token: 0x06006978 RID: 27000 RVA: 0x0021F5B0 File Offset: 0x0021D7B0
		public void SetDefaultProperties()
		{
			if (!this.isValid)
			{
				return;
			}
			this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.defaultPedestalMesh;
			this.goPedestal.GetComponent<MeshRenderer>().sharedMaterial = this.defaultPedestalMaterial;
			this.goMannequin.GetComponent<MeshFilter>().sharedMesh = this.defaultMannequinMesh;
			this.goMannequin.GetComponent<MeshRenderer>().sharedMaterial = this.defaultMannequinMaterial;
			this.goCosmeticItem.GetComponent<MeshFilter>().sharedMesh = this.defaultCosmeticMesh;
			this.goCosmeticItem.GetComponent<MeshRenderer>().sharedMaterial = this.defaultCosmeticMaterial;
			this.goCosmeticItemNameplate.GetComponent<TextMesh>().text = this.defaultItemText;
			this.goPreviewModeSFX.clip = this.defaultSFXPreviewMode;
			this.goAttractModeSFX.clip = this.defaultSFXAttractMode;
			this.goPurchaseModeSFX.clip = this.defaultSFXPurchaseMode;
		}

		// Token: 0x06006979 RID: 27001 RVA: 0x0021F693 File Offset: 0x0021D893
		private void ClearCosmeticMesh()
		{
			Object.Destroy(this.goCosmeticItemGameObject);
		}

		// Token: 0x0600697A RID: 27002 RVA: 0x0021F6A0 File Offset: 0x0021D8A0
		private void ClearCosmeticAtlas()
		{
			if (this.goCosmeticItemMeshAtlas.IsNotNull())
			{
				Object.Destroy(this.goCosmeticItemMeshAtlas);
			}
		}

		// Token: 0x0600697B RID: 27003 RVA: 0x0021F6BC File Offset: 0x0021D8BC
		public void SetCosmeticItemFromCosmeticController(CosmeticsController.CosmeticItem item)
		{
			if (!this.isValid)
			{
				return;
			}
			this.ClearCosmeticAtlas();
			this.ClearCosmeticMesh();
			this.oldItemID = this.itemID;
			this.itemID = item.itemName;
			this.itemName = item.displayName;
			if (item.overrideDisplayName != string.Empty)
			{
				this.itemName = item.overrideDisplayName;
			}
			this.HeadModel.SetCosmeticActive(this.itemID, false);
			this.SetCosmeticStand();
		}

		// Token: 0x0600697C RID: 27004 RVA: 0x0021F738 File Offset: 0x0021D938
		public void SetCosmeticStand()
		{
			this.cosmeticStand.thisCosmeticName = this.itemID;
			this.cosmeticStand.InitializeCosmetic();
			if (this.oldItemID.Length > 0)
			{
				if (this.oldItemID != this.itemID)
				{
					this.cosmeticStand.isOn = false;
				}
				this.cosmeticStand.UpdateColor();
			}
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x0021F79C File Offset: 0x0021D99C
		public void SetStoreUpdateEvent(StoreUpdateEvent storeUpdateEvent, bool playFX)
		{
			if (!this.isValid || !this.AffectedByStoreUpdateEvents)
			{
				return;
			}
			if (playFX)
			{
				this.goAttractMode.SetActive(true);
				this.goAttractModeVFX.Play();
			}
			this.currentUpdateEvent = storeUpdateEvent;
			this.SetCosmeticItemFromCosmeticController(CosmeticsController.instance.GetItemFromDict(storeUpdateEvent.ItemName));
			if (base.isActiveAndEnabled)
			{
				this.countdownTimerCoRoutine = base.StartCoroutine(this.PlayCountdownTimer());
			}
			this.UpdateClock();
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x0021F813 File Offset: 0x0021DA13
		private IEnumerator PlayCountdownTimer()
		{
			yield return new WaitForSeconds(Mathf.Clamp((float)((this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted).TotalSeconds - 10.0), 0f, float.MaxValue));
			this.PlaySFX();
			yield break;
		}

		// Token: 0x0600697F RID: 27007 RVA: 0x0021F822 File Offset: 0x0021DA22
		public void StopCountdownCoroutine()
		{
			this.CountdownSFX.GTStop();
			this.goAttractModeVFX.Stop();
			if (this.countdownTimerCoRoutine != null)
			{
				base.StopCoroutine(this.countdownTimerCoRoutine);
				this.countdownTimerCoRoutine = null;
			}
		}

		// Token: 0x06006980 RID: 27008 RVA: 0x0021F858 File Offset: 0x0021DA58
		private void PlaySFX()
		{
			if (this.currentUpdateEvent != null)
			{
				TimeSpan timeSpan = this.currentUpdateEvent.EndTimeUTC.ToUniversalTime() - StoreUpdater.instance.DateTimeNowServerAdjusted;
				if (timeSpan.TotalSeconds >= 10.0)
				{
					this.CountdownSFX.time = 0f;
					this.CountdownSFX.GTPlay();
					return;
				}
				this.CountdownSFX.time = 10f - (float)timeSpan.TotalSeconds;
				this.CountdownSFX.GTPlay();
			}
		}

		// Token: 0x06006981 RID: 27009 RVA: 0x0021F8E4 File Offset: 0x0021DAE4
		public void SetCosmeticItemProperties(string WhichGUID, string Name, List<Transform> SocketsList, int Socket, string PedestalMesh = null, string MannequinMesh = null)
		{
			if (!this.isValid)
			{
				return;
			}
			Guid guid;
			if (!Guid.TryParse(WhichGUID, out guid))
			{
				return;
			}
			this.itemName = Name;
			this.itemSocket = Socket;
			if (this.pedestalMesh != null)
			{
				this.goPedestal.GetComponent<MeshFilter>().sharedMesh = this.pedestalMesh;
			}
		}

		// Token: 0x06006982 RID: 27010 RVA: 0x0021F938 File Offset: 0x0021DB38
		private void StartPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.coroutinePreviewTimer = this.DoPreviewTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInPreviewMode ?? this.defaultHoursInPreviewMode) * 60 * 60)));
			base.StartCoroutine(this.coroutinePreviewTimer);
		}

		// Token: 0x06006983 RID: 27011 RVA: 0x0021F9B7 File Offset: 0x0021DBB7
		private void StopPreviewTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutinePreviewTimer != null)
			{
				base.StopCoroutine(this.coroutinePreviewTimer);
				this.coroutinePreviewTimer = null;
			}
			this.clockTextMesh.text = "Clock";
		}

		// Token: 0x06006984 RID: 27012 RVA: 0x0021F9ED File Offset: 0x0021DBED
		private IEnumerator DoPreviewTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.clockTextMesh.text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.ATTRACT);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x06006985 RID: 27013 RVA: 0x0021FA04 File Offset: 0x0021DC04
		public void StartAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.coroutineAttractTimer = this.DoAttractTimer(DateTime.UtcNow + TimeSpan.FromSeconds((double)((this.hoursInAttractMode ?? this.defaultHoursInAttractMode) * 60 * 60)));
			base.StartCoroutine(this.coroutineAttractTimer);
		}

		// Token: 0x06006986 RID: 27014 RVA: 0x0021FA83 File Offset: 0x0021DC83
		private void StopAttractTimer()
		{
			if (!this.isValid)
			{
				return;
			}
			if (this.coroutineAttractTimer != null)
			{
				base.StopCoroutine(this.coroutineAttractTimer);
				this.coroutineAttractTimer = null;
			}
			this.goClock.GetComponent<TextMesh>().text = "Clock";
		}

		// Token: 0x06006987 RID: 27015 RVA: 0x0021FABE File Offset: 0x0021DCBE
		private IEnumerator DoAttractTimer(DateTime ReleaseTime)
		{
			if (this.isValid)
			{
				bool timerDone = false;
				TimeSpan remainingTime = ReleaseTime - DateTime.UtcNow;
				while (!timerDone)
				{
					string text;
					int delayTime;
					if (remainingTime.TotalSeconds <= 59.0)
					{
						text = remainingTime.Seconds.ToString() + "s";
						delayTime = 1;
					}
					else
					{
						delayTime = 60;
						text = string.Empty;
						if (remainingTime.Days > 0)
						{
							text = text + remainingTime.Days.ToString() + "d ";
						}
						if (remainingTime.Hours > 0)
						{
							text = text + remainingTime.Hours.ToString() + "h ";
						}
						if (remainingTime.Minutes > 0)
						{
							text = text + remainingTime.Minutes.ToString() + "m ";
						}
						text = text.TrimEnd();
					}
					this.goClock.GetComponent<TextMesh>().text = text;
					yield return new WaitForSecondsRealtime((float)delayTime);
					remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds((double)delayTime));
					if (remainingTime.TotalSeconds <= 0.0)
					{
						timerDone = true;
					}
				}
				this.SwitchDisplayMode(CosmeticItemPrefab.EDisplayMode.HIDDEN);
				yield return null;
				remainingTime = default(TimeSpan);
			}
			yield break;
		}

		// Token: 0x04007908 RID: 30984
		public string PedestalID = "";

		// Token: 0x04007909 RID: 30985
		public HeadModel HeadModel;

		// Token: 0x0400790A RID: 30986
		public bool AffectedByStoreUpdateEvents = true;

		// Token: 0x0400790B RID: 30987
		[SerializeField]
		private Guid? itemGUID;

		// Token: 0x0400790C RID: 30988
		[SerializeField]
		private string itemName = string.Empty;

		// Token: 0x0400790D RID: 30989
		[SerializeField]
		private List<Transform> sockets = new List<Transform>();

		// Token: 0x0400790E RID: 30990
		[SerializeField]
		private int itemSocket = int.MinValue;

		// Token: 0x0400790F RID: 30991
		[SerializeField]
		private int? hoursInPreviewMode;

		// Token: 0x04007910 RID: 30992
		[SerializeField]
		private int? hoursInAttractMode;

		// Token: 0x04007911 RID: 30993
		[SerializeField]
		private Mesh pedestalMesh;

		// Token: 0x04007912 RID: 30994
		[SerializeField]
		private Mesh mannequinMesh;

		// Token: 0x04007913 RID: 30995
		[SerializeField]
		private Mesh cosmeticMesh;

		// Token: 0x04007914 RID: 30996
		[SerializeField]
		private AudioClip sfxPreviewMode;

		// Token: 0x04007915 RID: 30997
		[SerializeField]
		private AudioClip sfxAttractMode;

		// Token: 0x04007916 RID: 30998
		[SerializeField]
		private AudioClip sfxPurchaseMode;

		// Token: 0x04007917 RID: 30999
		[SerializeField]
		private ParticleSystem vfxPreviewMode;

		// Token: 0x04007918 RID: 31000
		[SerializeField]
		private ParticleSystem vfxAttractMode;

		// Token: 0x04007919 RID: 31001
		[SerializeField]
		private ParticleSystem vfxPurchaseMode;

		// Token: 0x0400791A RID: 31002
		[SerializeField]
		private GameObject goPedestal;

		// Token: 0x0400791B RID: 31003
		[SerializeField]
		private GameObject goMannequin;

		// Token: 0x0400791C RID: 31004
		[SerializeField]
		private GameObject goCosmeticItem;

		// Token: 0x0400791D RID: 31005
		[SerializeField]
		private GameObject goCosmeticItemGameObject;

		// Token: 0x0400791E RID: 31006
		[SerializeField]
		private GameObject goCosmeticItemNameplate;

		// Token: 0x0400791F RID: 31007
		[SerializeField]
		private GameObject goClock;

		// Token: 0x04007920 RID: 31008
		[SerializeField]
		private GameObject goPreviewMode;

		// Token: 0x04007921 RID: 31009
		[SerializeField]
		private GameObject goAttractMode;

		// Token: 0x04007922 RID: 31010
		[SerializeField]
		private GameObject goPurchaseMode;

		// Token: 0x04007923 RID: 31011
		[SerializeField]
		private Mesh defaultPedestalMesh;

		// Token: 0x04007924 RID: 31012
		[SerializeField]
		private Material defaultPedestalMaterial;

		// Token: 0x04007925 RID: 31013
		[SerializeField]
		private Mesh defaultMannequinMesh;

		// Token: 0x04007926 RID: 31014
		[SerializeField]
		private Material defaultMannequinMaterial;

		// Token: 0x04007927 RID: 31015
		[SerializeField]
		private Mesh defaultCosmeticMesh;

		// Token: 0x04007928 RID: 31016
		[SerializeField]
		private Material defaultCosmeticMaterial;

		// Token: 0x04007929 RID: 31017
		[SerializeField]
		private string defaultItemText;

		// Token: 0x0400792A RID: 31018
		[SerializeField]
		private int defaultHoursInPreviewMode;

		// Token: 0x0400792B RID: 31019
		[SerializeField]
		private int defaultHoursInAttractMode;

		// Token: 0x0400792C RID: 31020
		[SerializeField]
		private AudioClip defaultSFXPreviewMode;

		// Token: 0x0400792D RID: 31021
		[SerializeField]
		private AudioClip defaultSFXAttractMode;

		// Token: 0x0400792E RID: 31022
		[SerializeField]
		private AudioClip defaultSFXPurchaseMode;

		// Token: 0x0400792F RID: 31023
		private GameObject goCosmeticItemMeshAtlas;

		// Token: 0x04007930 RID: 31024
		public AudioSource CountdownSFX;

		// Token: 0x04007931 RID: 31025
		private CosmeticItemPrefab.EDisplayMode currentDisplayMode;

		// Token: 0x04007932 RID: 31026
		private bool isValid;

		// Token: 0x04007933 RID: 31027
		[Nullable(2)]
		private AudioSource goPreviewModeSFX;

		// Token: 0x04007934 RID: 31028
		[Nullable(2)]
		private AudioSource goAttractModeSFX;

		// Token: 0x04007935 RID: 31029
		[Nullable(2)]
		private AudioSource goPurchaseModeSFX;

		// Token: 0x04007936 RID: 31030
		[Nullable(2)]
		private ParticleSystem goAttractModeVFX;

		// Token: 0x04007937 RID: 31031
		[Nullable(2)]
		private ParticleSystem goPurchaseModeVFX;

		// Token: 0x04007938 RID: 31032
		private IEnumerator coroutinePreviewTimer;

		// Token: 0x04007939 RID: 31033
		private IEnumerator coroutineAttractTimer;

		// Token: 0x0400793A RID: 31034
		private DateTime startTime;

		// Token: 0x0400793B RID: 31035
		private TextMeshPro clockTextMesh;

		// Token: 0x0400793C RID: 31036
		private bool clockTextMeshIsValid;

		// Token: 0x0400793D RID: 31037
		private StoreUpdateEvent currentUpdateEvent;

		// Token: 0x0400793E RID: 31038
		private string defaultCountdownTextTemplate = "";

		// Token: 0x0400793F RID: 31039
		public CosmeticStand cosmeticStand;

		// Token: 0x04007940 RID: 31040
		public string itemID = "";

		// Token: 0x04007941 RID: 31041
		public string oldItemID = "";

		// Token: 0x04007942 RID: 31042
		private Coroutine countdownTimerCoRoutine;

		// Token: 0x04007943 RID: 31043
		private float updateClock = 60f;

		// Token: 0x04007944 RID: 31044
		private float lastUpdated;

		// Token: 0x02001082 RID: 4226
		[SerializeField]
		public enum EDisplayMode
		{
			// Token: 0x04007946 RID: 31046
			NULL,
			// Token: 0x04007947 RID: 31047
			HIDDEN,
			// Token: 0x04007948 RID: 31048
			PREVIEW,
			// Token: 0x04007949 RID: 31049
			ATTRACT,
			// Token: 0x0400794A RID: 31050
			PURCHASE,
			// Token: 0x0400794B RID: 31051
			POSTPURCHASE
		}
	}
}
