using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using Oculus.Platform;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// Token: 0x020004A0 RID: 1184
public class GorillaMetaReport : MonoBehaviour
{
	// Token: 0x1700030E RID: 782
	// (get) Token: 0x06001CA8 RID: 7336 RVA: 0x0009B2CA File Offset: 0x000994CA
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001CA9 RID: 7337 RVA: 0x0009B2D1 File Offset: 0x000994D1
	private void Start()
	{
		this.localPlayer.inOverlay = false;
		MothershipClientApiUnity.OnMessageNotificationSocket += this.OnNotification;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001CAA RID: 7338 RVA: 0x0009B2FC File Offset: 0x000994FC
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.localPlayer.inOverlay = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06001CAB RID: 7339 RVA: 0x0009B318 File Offset: 0x00099518
	private void OnReportButtonIntentNotif(Message<string> message)
	{
		if (message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Unhandled);
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			this.ReportText.SetActive(true);
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
			return;
		}
		if (!message.IsError)
		{
			AbuseReport.ReportRequestHandled(ReportRequestResponse.Handled);
			this.StartOverlay(false);
		}
	}

	// Token: 0x06001CAC RID: 7340 RVA: 0x0009B370 File Offset: 0x00099570
	private void OnNotification(NotificationsMessageResponse notification, [NativeInteger] IntPtr _)
	{
		string title = notification.Title;
		if (title == "Warning")
		{
			this.OnWarning(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Warning");
			return;
		}
		if (title == "Mute")
		{
			this.OnMuteSanction(notification.Body);
			GorillaTelemetry.PostNotificationEvent("Mute");
			return;
		}
		if (!(title == "Unmute"))
		{
			return;
		}
		if (GorillaTagger.hasInstance)
		{
			GorillaTagger.moderationMutedTime = -1f;
		}
		GorillaTelemetry.PostNotificationEvent("Unmute");
	}

	// Token: 0x06001CAD RID: 7341 RVA: 0x0009B3F8 File Offset: 0x000995F8
	private void OnWarning(string warningNotification)
	{
		string[] array = warningNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 2)
		{
			Debug.LogError("Invalid warning notification");
			return;
		}
		string text = array[0];
		string[] array2 = array[1].Split(',', StringSplitOptions.None);
		if (array2.Length == 0)
		{
			Debug.LogError("Missing warning notification reasons");
			return;
		}
		string text2 = GorillaMetaReport.FormatListToString(in array2);
		this.ReportText.GetComponent<Text>().text = text.ToUpper() + " WARNING FOR " + text2.ToUpper();
		this.StartOverlay(true);
	}

	// Token: 0x06001CAE RID: 7342 RVA: 0x0009B474 File Offset: 0x00099674
	private void OnMuteSanction(string muteNotification)
	{
		string[] array = muteNotification.Split('|', StringSplitOptions.None);
		if (array.Length != 3)
		{
			Debug.LogError("Invalid mute notification");
			return;
		}
		if (!array[0].Equals("voice", StringComparison.OrdinalIgnoreCase))
		{
			return;
		}
		int num;
		if (array[2].Length > 0 && int.TryParse(array[2], out num))
		{
			int num2 = num / 60;
			this.ReportText.GetComponent<Text>().text = string.Format("MUTED FOR {0} MINUTES\nBAD MONKE", num2);
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = (float)num;
			}
		}
		else
		{
			this.ReportText.GetComponent<Text>().text = "MUTED FOREVER";
			if (GorillaTagger.hasInstance)
			{
				GorillaTagger.moderationMutedTime = float.PositiveInfinity;
			}
		}
		this.StartOverlay(true);
	}

	// Token: 0x06001CAF RID: 7343 RVA: 0x0009B528 File Offset: 0x00099728
	private static string FormatListToString(in string[] list)
	{
		int num = list.Length;
		string text3;
		if (num != 1)
		{
			if (num != 2)
			{
				string text = RuntimeHelpers.GetSubArray<string>(list, Range.EndAt(new Index(1, true))).Join(", ");
				string text2 = ", AND ";
				string[] array = list;
				text3 = text + text2 + array[array.Length - 1];
			}
			else
			{
				text3 = list[0] + " AND " + list[1];
			}
		}
		else
		{
			text3 = list[0];
		}
		return text3;
	}

	// Token: 0x06001CB0 RID: 7344 RVA: 0x0009B591 File Offset: 0x00099791
	private IEnumerator Submitted()
	{
		yield return new WaitForSeconds(1.5f);
		this.Teardown();
		yield break;
	}

	// Token: 0x06001CB1 RID: 7345 RVA: 0x0009B5A0 File Offset: 0x000997A0
	private void DuplicateScoreboard()
	{
		this.currentScoreboard.gameObject.SetActive(true);
		if (GorillaScoreboardTotalUpdater.instance != null)
		{
			GorillaScoreboardTotalUpdater.instance.UpdateScoreboard(this.currentScoreboard);
		}
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
	}

	// Token: 0x06001CB2 RID: 7346 RVA: 0x0009B60C File Offset: 0x0009980C
	private void ToggleLevelVisibility(bool state)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (state)
		{
			if (this.hasSavedCullingMask)
			{
				component.cullingMask = this.savedCullingLayers;
				this.hasSavedCullingMask = false;
				return;
			}
		}
		else
		{
			if (!this.hasSavedCullingMask)
			{
				this.savedCullingLayers = component.cullingMask;
				this.hasSavedCullingMask = true;
			}
			component.cullingMask = this.visibleLayers;
		}
	}

	// Token: 0x06001CB3 RID: 7347 RVA: 0x0009B674 File Offset: 0x00099874
	private void Teardown()
	{
		this.ReportText.GetComponent<Text>().text = "NOT CURRENTLY CONNECTED TO A ROOM";
		this.ReportText.SetActive(false);
		this.localPlayer.inOverlay = false;
		this.localPlayer.disableMovement = false;
		this.closeButton.selected = false;
		this.closeButton.isOn = false;
		this.closeButton.UpdateColor();
		this.localPlayer.InReportMenu = false;
		this.ToggleLevelVisibility(true);
		base.gameObject.SetActive(false);
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			gorillaPlayerScoreboardLine.doneReporting = false;
		}
		GorillaScoreboardTotalUpdater.instance.UpdateActiveScoreboards();
	}

	// Token: 0x06001CB4 RID: 7348 RVA: 0x0009B750 File Offset: 0x00099950
	private void CheckReportSubmit()
	{
		if (this.currentScoreboard == null)
		{
			return;
		}
		foreach (GorillaPlayerScoreboardLine gorillaPlayerScoreboardLine in this.currentScoreboard.lines)
		{
			if (gorillaPlayerScoreboardLine.doneReporting)
			{
				this.ReportText.SetActive(true);
				this.ReportText.GetComponent<Text>().text = "REPORTED " + gorillaPlayerScoreboardLine.playerNameVisible;
				this.currentScoreboard.gameObject.SetActive(false);
				base.StartCoroutine(this.Submitted());
			}
		}
	}

	// Token: 0x06001CB5 RID: 7349 RVA: 0x0009B804 File Offset: 0x00099A04
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * this.playerLocalScreenPosition * scale.x;
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x0009B890 File Offset: 0x00099A90
	private void StartOverlay(bool isSanction = false)
	{
		if (this.localPlayer.InReportMenu)
		{
			return;
		}
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		this.currentScoreboard.transform.localScale = vector2 * 2f;
		this.reportScoreboard.transform.localScale = vector2;
		this.leftHandObject.transform.localScale = vector2;
		this.rightHandObject.transform.localScale = vector2;
		this.occluder.transform.localScale = vector2;
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		this.localPlayer.InReportMenu = true;
		this.localPlayer.disableMovement = true;
		this.localPlayer.inOverlay = true;
		base.gameObject.SetActive(true);
		if (PhotonNetwork.InRoom && !isSanction)
		{
			this.DuplicateScoreboard();
		}
		else
		{
			this.ReportText.SetActive(true);
			this.reportScoreboard.transform.SetPositionAndRotation(vector, quaternion);
			this.currentScoreboard.transform.SetPositionAndRotation(vector, quaternion);
		}
		this.ToggleLevelVisibility(false);
		this.UpdateHandPosRot();
		if (isSanction)
		{
			this.currentScoreboard.gameObject.SetActive(false);
			return;
		}
		this.currentScoreboard.gameObject.SetActive(true);
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x0009B9C4 File Offset: 0x00099BC4
	private void CheckDistance()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		this.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		float num = Vector3.Distance(this.reportScoreboard.transform.position, vector);
		float num2 = 1f;
		if (num > num2 && !this.isMoving)
		{
			this.isMoving = true;
			this.movementTime = 0f;
		}
		if (this.isMoving)
		{
			this.movementTime += Time.deltaTime;
			float num3 = this.movementTime;
			this.reportScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.reportScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.reportScoreboard.transform.rotation, quaternion, num3));
			if (this.currentScoreboard != null)
			{
				this.currentScoreboard.transform.SetPositionAndRotation(Vector3.Lerp(this.currentScoreboard.transform.position, vector, num3), Quaternion.Lerp(this.currentScoreboard.transform.rotation, quaternion, num3));
			}
			if (num3 >= 1f)
			{
				this.isMoving = false;
				this.movementTime = 0f;
			}
		}
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x0009BAE4 File Offset: 0x00099CE4
	private void Update()
	{
		if (this.blockButtonsUntilTimestamp > Time.time)
		{
			return;
		}
		if (SteamVR_Actions.gorillaTag_System.GetState(SteamVR_Input_Sources.LeftHand) && this.localPlayer.InReportMenu)
		{
			this.Teardown();
			this.blockButtonsUntilTimestamp = Time.time + 0.75f;
		}
		if (this.localPlayer.InReportMenu)
		{
			this.localPlayer.inOverlay = true;
			this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
			this.UpdateHandPosRot();
			this.CheckDistance();
			this.CheckReportSubmit();
		}
		if (this.closeButton.selected)
		{
			this.Teardown();
		}
		if (this.testPress)
		{
			this.testPress = false;
			this.StartOverlay(false);
		}
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x0009BBA8 File Offset: 0x00099DA8
	private void UpdateHandPosRot()
	{
		Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
		Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
		this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation * this.handRotOffset);
		this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation * this.handRotOffset);
	}

	// Token: 0x040026C6 RID: 9926
	[SerializeField]
	private GameObject occluder;

	// Token: 0x040026C7 RID: 9927
	[SerializeField]
	private GameObject reportScoreboard;

	// Token: 0x040026C8 RID: 9928
	[SerializeField]
	private GameObject ReportText;

	// Token: 0x040026C9 RID: 9929
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x040026CA RID: 9930
	[SerializeField]
	private GorillaReportButton closeButton;

	// Token: 0x040026CB RID: 9931
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x040026CC RID: 9932
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x040026CD RID: 9933
	[SerializeField]
	private Quaternion handRotOffset;

	// Token: 0x040026CE RID: 9934
	[SerializeField]
	private Vector3 playerLocalScreenPosition;

	// Token: 0x040026CF RID: 9935
	private float blockButtonsUntilTimestamp;

	// Token: 0x040026D0 RID: 9936
	[SerializeField]
	private GorillaScoreBoard currentScoreboard;

	// Token: 0x040026D1 RID: 9937
	private int savedCullingLayers;

	// Token: 0x040026D2 RID: 9938
	private bool hasSavedCullingMask;

	// Token: 0x040026D3 RID: 9939
	public bool testPress;

	// Token: 0x040026D4 RID: 9940
	public bool isMoving;

	// Token: 0x040026D5 RID: 9941
	private float movementTime;
}
