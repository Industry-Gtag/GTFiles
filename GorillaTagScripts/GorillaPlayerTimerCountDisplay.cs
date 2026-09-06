using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000F95 RID: 3989
	public class GorillaPlayerTimerCountDisplay : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x0600633B RID: 25403 RVA: 0x001FEF9C File Offset: 0x001FD19C
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x0600633C RID: 25404 RVA: 0x001FEF9C File Offset: 0x001FD19C
		private void OnEnable()
		{
			this.TryInit();
		}

		// Token: 0x0600633D RID: 25405 RVA: 0x001FEFA4 File Offset: 0x001FD1A4
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.OnTimerStopped.AddListener(new UnityAction<int, int>(this.OnTimerStopped));
			PlayerTimerManager.instance.OnLocalTimerStarted.AddListener(new UnityAction(this.OnLocalTimerStarted));
			this.displayText.text = "TIME: --.--.-";
			if (PlayerTimerManager.instance.IsLocalTimerStarted() && !this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.isInitialized = true;
		}

		// Token: 0x0600633E RID: 25406 RVA: 0x001FF030 File Offset: 0x001FD230
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.OnTimerStopped.RemoveListener(new UnityAction<int, int>(this.OnTimerStopped));
				PlayerTimerManager.instance.OnLocalTimerStarted.RemoveListener(new UnityAction(this.OnLocalTimerStarted));
			}
			this.isInitialized = false;
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x001FF095 File Offset: 0x001FD295
		private void OnLocalTimerStarted()
		{
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x001FF0A8 File Offset: 0x001FD2A8
		private void OnTimerStopped(int actorNum, int timeDelta)
		{
			if (actorNum == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				double num = timeDelta / 1000.0;
				this.displayText.text = "TIME: " + TimeSpan.FromSeconds(num).ToString("mm\\:ss\\:f");
				if (this.TickRunning)
				{
					TickSystem<object>.RemoveTickCallback(this);
				}
			}
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x001FF10C File Offset: 0x001FD30C
		private void UpdateLatestTime()
		{
			float timeForPlayer = PlayerTimerManager.instance.GetTimeForPlayer(NetworkSystem.Instance.LocalPlayer.ActorNumber);
			this.displayText.text = "TIME: " + TimeSpan.FromSeconds((double)timeForPlayer).ToString("mm\\:ss\\:f");
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06006342 RID: 25410 RVA: 0x001FF15C File Offset: 0x001FD35C
		// (set) Token: 0x06006343 RID: 25411 RVA: 0x001FF164 File Offset: 0x001FD364
		public bool TickRunning { get; set; }

		// Token: 0x06006344 RID: 25412 RVA: 0x001FF16D File Offset: 0x001FD36D
		public void Tick()
		{
			this.UpdateLatestTime();
		}

		// Token: 0x040071EA RID: 29162
		[SerializeField]
		private TMP_Text displayText;

		// Token: 0x040071EB RID: 29163
		private bool isInitialized;
	}
}
