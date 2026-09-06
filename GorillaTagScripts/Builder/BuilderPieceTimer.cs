using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.Builder
{
	// Token: 0x0200103C RID: 4156
	public class BuilderPieceTimer : MonoBehaviour, IBuilderPieceComponent, ITickSystemTick
	{
		// Token: 0x0600678E RID: 26510 RVA: 0x002151E1 File Offset: 0x002133E1
		private void Awake()
		{
			this.buttonTrigger.TriggeredEvent.AddListener(new UnityAction(this.OnButtonPressed));
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x002151FF File Offset: 0x002133FF
		private void OnDestroy()
		{
			if (this.buttonTrigger != null)
			{
				this.buttonTrigger.TriggeredEvent.RemoveListener(new UnityAction(this.OnButtonPressed));
			}
		}

		// Token: 0x06006790 RID: 26512 RVA: 0x0021522C File Offset: 0x0021342C
		private void OnButtonPressed()
		{
			if (this.myPiece.state != BuilderPiece.State.AttachedAndPlaced)
			{
				return;
			}
			if (Time.time > this.lastTriggeredTime + this.debounceTime)
			{
				this.lastTriggeredTime = Time.time;
				if (!this.isStart && this.stopSoundBank != null)
				{
					this.stopSoundBank.Play();
				}
				else if (this.activateSoundBank != null)
				{
					this.activateSoundBank.Play();
				}
				if (this.isBoth && this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: 00:00:0";
				}
				PlayerTimerManager.instance.RequestTimerToggle(this.isStart);
			}
		}

		// Token: 0x06006791 RID: 26513 RVA: 0x002152E4 File Offset: 0x002134E4
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (this.isStart && !this.isBoth)
			{
				return;
			}
			double num = timeDelta;
			this.latestTime = num / 1000.0;
			if (this.latestTime > 3599.989990234375)
			{
				this.latestTime = 3599.989990234375;
			}
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds(this.latestTime).ToString("mm\\:ss\\:ff");
			if (this.isBoth && actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.isStart = true;
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06006792 RID: 26514 RVA: 0x00215393 File Offset: 0x00213593
		private void OnLocalTimerStarted()
		{
			if (this.isBoth)
			{
				this.isStart = false;
			}
			if (this.myPiece.state == BuilderPiece.State.AttachedAndPlaced && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006793 RID: 26515 RVA: 0x002153C0 File Offset: 0x002135C0
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(this.myPiece.GetTable().tableZone);
			if (this.displayText != null)
			{
				this.displayText.gameObject.SetActive(flag);
			}
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x00215408 File Offset: 0x00213608
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.latestTime = double.MaxValue;
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
				this.OnZoneChanged();
				this.displayText.text = "TIME: __:__:_";
			}
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x0021546E File Offset: 0x0021366E
		public void OnPieceDestroy()
		{
			if (this.displayText != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06006796 RID: 26518 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void OnPiecePlacementDeserialized()
		{
		}

		// Token: 0x06006797 RID: 26519 RVA: 0x002154A4 File Offset: 0x002136A4
		public void OnPieceActivate()
		{
			this.lastTriggeredTime = 0f;
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			if (this.isBoth)
			{
				this.isStart = !PlayerTimerManager.instance.IsLocalTimerStarted();
				if (!this.isStart && this.displayText != null)
				{
					this.displayText.text = "TIME: __:__:_";
				}
			}
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006798 RID: 26520 RVA: 0x00215550 File Offset: 0x00213750
		public void OnPieceDeactivate()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			if (this.displayText != null)
			{
				this.displayText.text = "TIME: --:--:-";
			}
		}

		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06006799 RID: 26521 RVA: 0x002155CC File Offset: 0x002137CC
		// (set) Token: 0x0600679A RID: 26522 RVA: 0x002155D4 File Offset: 0x002137D4
		public bool TickRunning { get; set; }

		// Token: 0x0600679B RID: 26523 RVA: 0x002155E0 File Offset: 0x002137E0
		public void Tick()
		{
			if (this.displayText != null)
			{
				float num = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
				num = Mathf.Clamp(num, 0f, 3599.99f);
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)num).ToString("mm\\:ss\\:f");
			}
		}

		// Token: 0x0400769B RID: 30363
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400769C RID: 30364
		[SerializeField]
		private bool isStart;

		// Token: 0x0400769D RID: 30365
		[SerializeField]
		private bool isBoth;

		// Token: 0x0400769E RID: 30366
		[SerializeField]
		private BuilderSmallHandTrigger buttonTrigger;

		// Token: 0x0400769F RID: 30367
		[SerializeField]
		private SoundBankPlayer activateSoundBank;

		// Token: 0x040076A0 RID: 30368
		[SerializeField]
		private SoundBankPlayer stopSoundBank;

		// Token: 0x040076A1 RID: 30369
		[SerializeField]
		private float debounceTime = 0.5f;

		// Token: 0x040076A2 RID: 30370
		private float lastTriggeredTime;

		// Token: 0x040076A3 RID: 30371
		private double latestTime = 3.4028234663852886E+38;

		// Token: 0x040076A4 RID: 30372
		[SerializeField]
		private TMP_Text displayText;
	}
}
