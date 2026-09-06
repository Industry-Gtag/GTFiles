using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000470 RID: 1136
public class NetworkWrapper : MonoBehaviour
{
	// Token: 0x06001BA4 RID: 7076 RVA: 0x00095D0F File Offset: 0x00093F0F
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void AutoInstantiate()
	{
		Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(Resources.Load<GameObject>("P_NetworkWrapper")));
	}

	// Token: 0x06001BA5 RID: 7077 RVA: 0x00095D28 File Offset: 0x00093F28
	private void Awake()
	{
		if (this.titleRef != null)
		{
			this.titleRef.text = "PUN";
		}
		this.activeNetworkSystem = base.gameObject.AddComponent<NetworkSystemPUN>();
		this.activeNetworkSystem.AddVoiceSettings(this.VoiceSettings);
		this.activeNetworkSystem.config = this.netSysConfig;
		this.activeNetworkSystem.regionNames = this.networkRegionNames;
		this.activeNetworkSystem.OnPlayerJoined += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnPlayerLeft += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnMultiplayerStarted += this.UpdatePlayerCount;
		this.activeNetworkSystem.OnReturnedToSinglePlayer += this.UpdatePlayerCount;
		Debug.Log("<color=green>initialize Network System</color>");
		this.activeNetworkSystem.Initialise();
	}

	// Token: 0x06001BA6 RID: 7078 RVA: 0x00095E34 File Offset: 0x00094034
	private void UpdatePlayerCountWrapper(NetPlayer player)
	{
		this.UpdatePlayerCount();
	}

	// Token: 0x06001BA7 RID: 7079 RVA: 0x00095E3C File Offset: 0x0009403C
	private void UpdatePlayerCount()
	{
		if (this.playerCountTextRef == null)
		{
			return;
		}
		if (!this.activeNetworkSystem.IsOnline)
		{
			this.playerCountTextRef.text = string.Format("0/{0}", this.netSysConfig.MaxPlayerCount);
			Debug.Log("Player count updated");
			return;
		}
		Debug.Log("Player count not updated");
		this.playerCountTextRef.text = string.Format("{0}/{1}", this.activeNetworkSystem.AllNetPlayers.Length, this.netSysConfig.MaxPlayerCount);
	}

	// Token: 0x040025D0 RID: 9680
	[HideInInspector]
	public NetworkSystem activeNetworkSystem;

	// Token: 0x040025D1 RID: 9681
	public Text titleRef;

	// Token: 0x040025D2 RID: 9682
	[Header("NetSys settings")]
	public NetworkSystemConfig netSysConfig;

	// Token: 0x040025D3 RID: 9683
	public string[] networkRegionNames;

	// Token: 0x040025D4 RID: 9684
	public string[] devNetworkRegionNames;

	// Token: 0x040025D5 RID: 9685
	[Header("Debug output refs")]
	public Text stateTextRef;

	// Token: 0x040025D6 RID: 9686
	public Text playerCountTextRef;

	// Token: 0x040025D7 RID: 9687
	[SerializeField]
	private SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x040025D8 RID: 9688
	private const string WrapperResourcePath = "P_NetworkWrapper";
}
