using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005B4 RID: 1460
public class DearLemmingKiosk : MonoBehaviour
{
	// Token: 0x170003E1 RID: 993
	// (get) Token: 0x06002500 RID: 9472 RVA: 0x000C6750 File Offset: 0x000C4950
	public int NextSubmit
	{
		get
		{
			return this.nextSubmit;
		}
	}

	// Token: 0x170003E2 RID: 994
	// (get) Token: 0x06002501 RID: 9473 RVA: 0x000C6758 File Offset: 0x000C4958
	public bool CanSubmit
	{
		get
		{
			return this.canSubmit;
		}
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000C6760 File Offset: 0x000C4960
	public void Fetch()
	{
		if (Time.time - this.fetchTime < 300f)
		{
			return;
		}
		this.fetchTime = Time.time;
		DearLemmingController.instance.CheckCanSubmit();
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000C678C File Offset: 0x000C498C
	public void Send()
	{
		if (!this.canSubmit && (float)this.nextSubmit - Time.time >= 0f)
		{
			this.popUp.text = "Your last message has been delivered. You can send another in " + this.secondsToTimeSpanString((float)this.nextSubmit - Time.time) + ".";
			this.popUp.gameObject.SetActive(true);
			return;
		}
		if (this.src.Text.Length > 6)
		{
			DearLemmingController.instance.SubmitMessage(this.src.Text);
			return;
		}
		this.popUp.text = "That's a little too short... type a little more!";
		this.popUp.gameObject.SetActive(true);
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000C6840 File Offset: 0x000C4A40
	private string secondsToTimeSpanString(float s)
	{
		TimeSpan timeSpan = new TimeSpan(0, 0, 0, Mathf.FloorToInt(s));
		if (timeSpan.Days > 0)
		{
			return string.Format("{0} days, {1} hours, {2} minutes, and {3} seconds", new object[] { timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds });
		}
		if (timeSpan.Hours > 0)
		{
			return string.Format("{0} hours, {1} minutes, and {2} seconds", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
		}
		if (timeSpan.Minutes > 0)
		{
			return string.Format("{0} minutes, and {1} seconds", timeSpan.Minutes, timeSpan.Seconds);
		}
		return string.Format("{0} seconds", timeSpan.Seconds);
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000C6930 File Offset: 0x000C4B30
	private void OnEnable()
	{
		DearLemmingController.instance.OnSubmitComplete += this.Instance_OnSubmitComplete;
		DearLemmingController.instance.OnCheckComplete += this.Instance_OnCheckComplete;
		this.Fetch();
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000C6964 File Offset: 0x000C4B64
	private void Instance_OnCheckComplete(DearLemmingController.DearLemmingResponse obj)
	{
		Debug.Log("DearLemmingKiosk :: Instance_OnCheckComplete");
		if (obj.Error.IsNullOrEmpty())
		{
			this.SetData(obj);
		}
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000C6984 File Offset: 0x000C4B84
	private void Instance_OnSubmitComplete(DearLemmingController.DearLemmingResponse obj)
	{
		Debug.Log("DearLemmingKiosk :: Instance_OnSubmitComplete");
		if (obj.Error.IsNullOrEmpty())
		{
			UnityEvent submitSuccess = this.SubmitSuccess;
			if (submitSuccess != null)
			{
				submitSuccess.Invoke();
			}
			this.SetData(obj);
			this.popUp.text = "message sent!";
			this.popUp.gameObject.SetActive(true);
			return;
		}
		UnityEvent submitFail = this.SubmitFail;
		if (submitFail != null)
		{
			submitFail.Invoke();
		}
		this.popUp.text = "message could not be sent at this time. try again later.";
		this.popUp.gameObject.SetActive(true);
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000C6A14 File Offset: 0x000C4C14
	private void SetData(DearLemmingController.DearLemmingResponse obj)
	{
		SimpleCountdown simpleCountdown = this.countDown;
		simpleCountdown.ManualCountdownComplete = (Action)Delegate.Remove(simpleCountdown.ManualCountdownComplete, new Action(this.countDownComplete));
		this.fetchTime = Time.time;
		this.nextSubmit = 0;
		this.canSubmit = obj.CanSubmit;
		if (!this.canSubmit)
		{
			this.nextSubmit = Mathf.CeilToInt(Time.time) + (int)obj.SecondsUntilNextSubmit.Value;
			this.countDown.gameObject.SetActive(true);
			this.countDown.StartCountdown((int)obj.SecondsUntilNextSubmit.Value);
			SimpleCountdown simpleCountdown2 = this.countDown;
			simpleCountdown2.ManualCountdownComplete = (Action)Delegate.Combine(simpleCountdown2.ManualCountdownComplete, new Action(this.countDownComplete));
		}
		this.ready.SetActive(!this.countDown.gameObject.activeInHierarchy);
		UnityEvent refreshed = this.Refreshed;
		if (refreshed == null)
		{
			return;
		}
		refreshed.Invoke();
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000C6B08 File Offset: 0x000C4D08
	private void countDownComplete()
	{
		SimpleCountdown simpleCountdown = this.countDown;
		simpleCountdown.ManualCountdownComplete = (Action)Delegate.Remove(simpleCountdown.ManualCountdownComplete, new Action(this.countDownComplete));
		this.countDown.gameObject.SetActive(false);
		this.ready.SetActive(true);
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000C6B5C File Offset: 0x000C4D5C
	private void OnDisable()
	{
		DearLemmingController.instance.OnSubmitComplete -= this.Instance_OnSubmitComplete;
		DearLemmingController.instance.OnCheckComplete -= this.Instance_OnCheckComplete;
		SimpleCountdown simpleCountdown = this.countDown;
		simpleCountdown.ManualCountdownComplete = (Action)Delegate.Remove(simpleCountdown.ManualCountdownComplete, new Action(this.countDownComplete));
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000C6BBC File Offset: 0x000C4DBC
	private void OnDestroy()
	{
		DearLemmingController.instance.OnSubmitComplete -= this.Instance_OnSubmitComplete;
		DearLemmingController.instance.OnCheckComplete -= this.Instance_OnCheckComplete;
		SimpleCountdown simpleCountdown = this.countDown;
		simpleCountdown.ManualCountdownComplete = (Action)Delegate.Remove(simpleCountdown.ManualCountdownComplete, new Action(this.countDownComplete));
	}

	// Token: 0x04003080 RID: 12416
	[SerializeField]
	private TypingTarget src;

	// Token: 0x04003081 RID: 12417
	[SerializeField]
	private TMP_Text popUp;

	// Token: 0x04003082 RID: 12418
	[SerializeField]
	private SimpleCountdown countDown;

	// Token: 0x04003083 RID: 12419
	[SerializeField]
	private GameObject ready;

	// Token: 0x04003084 RID: 12420
	[SerializeField]
	private UnityEvent Refreshed;

	// Token: 0x04003085 RID: 12421
	[SerializeField]
	private UnityEvent SubmitSuccess;

	// Token: 0x04003086 RID: 12422
	[SerializeField]
	private UnityEvent SubmitFail;

	// Token: 0x04003087 RID: 12423
	private bool canSubmit = true;

	// Token: 0x04003088 RID: 12424
	private int nextSubmit;

	// Token: 0x04003089 RID: 12425
	private float fetchTime = -300f;
}
