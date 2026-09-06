using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004A2 RID: 1186
public class GorillaReportButton : MonoBehaviour
{
	// Token: 0x06001CC1 RID: 7361 RVA: 0x0009BC86 File Offset: 0x00099E86
	public void AssignParentLine(GorillaPlayerScoreboardLine parent)
	{
		this.parentLine = parent;
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0009BC90 File Offset: 0x00099E90
	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled && this.touchTime + this.debounceTime < Time.time)
		{
			this.isOn = !this.isOn;
			this.UpdateColor();
			this.selected = !this.selected;
			this.touchTime = Time.time;
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
			GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.05f);
			if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
			{
				GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", RpcTarget.Others, new object[] { 67, false, 0.05f });
			}
		}
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x0009BD83 File Offset: 0x00099F83
	private void OnTriggerExit(Collider other)
	{
		if (this.metaReportType != GorillaReportButton.MetaReportReason.Cancel)
		{
			other.GetComponentInParent<GorillaTriggerColliderHandIndicator>() != null;
		}
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x0009BD9B File Offset: 0x00099F9B
	public void UpdateColor()
	{
		if (this.isOn)
		{
			base.GetComponent<MeshRenderer>().material = this.onMaterial;
			return;
		}
		base.GetComponent<MeshRenderer>().material = this.offMaterial;
	}

	// Token: 0x040026D9 RID: 9945
	public GorillaReportButton.MetaReportReason metaReportType;

	// Token: 0x040026DA RID: 9946
	public GorillaPlayerLineButton.ButtonType buttonType;

	// Token: 0x040026DB RID: 9947
	public GorillaPlayerScoreboardLine parentLine;

	// Token: 0x040026DC RID: 9948
	public bool isOn;

	// Token: 0x040026DD RID: 9949
	public Material offMaterial;

	// Token: 0x040026DE RID: 9950
	public Material onMaterial;

	// Token: 0x040026DF RID: 9951
	public string offText;

	// Token: 0x040026E0 RID: 9952
	public string onText;

	// Token: 0x040026E1 RID: 9953
	public Text myText;

	// Token: 0x040026E2 RID: 9954
	public float debounceTime = 0.25f;

	// Token: 0x040026E3 RID: 9955
	public float touchTime;

	// Token: 0x040026E4 RID: 9956
	public bool testPress;

	// Token: 0x040026E5 RID: 9957
	public bool selected;

	// Token: 0x020004A3 RID: 1187
	[SerializeField]
	public enum MetaReportReason
	{
		// Token: 0x040026E7 RID: 9959
		HateSpeech,
		// Token: 0x040026E8 RID: 9960
		Cheating,
		// Token: 0x040026E9 RID: 9961
		Toxicity,
		// Token: 0x040026EA RID: 9962
		Bullying,
		// Token: 0x040026EB RID: 9963
		Doxing,
		// Token: 0x040026EC RID: 9964
		Impersonation,
		// Token: 0x040026ED RID: 9965
		Submit,
		// Token: 0x040026EE RID: 9966
		Cancel
	}
}
