using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000CCC RID: 3276
public class GorillaNetworkJoinTriggerXSceneRef : MonoBehaviour
{
	// Token: 0x06005136 RID: 20790 RVA: 0x001AECF5 File Offset: 0x001ACEF5
	protected void Awake()
	{
		if (this.m_joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this._joinTrigger) && this._joinTrigger != null)
		{
			return;
		}
		this.m_joinTriggerRef.AddCallbackOnLoad(new Action(this._OnTargetSceneLoaded));
	}

	// Token: 0x06005137 RID: 20791 RVA: 0x001AED30 File Offset: 0x001ACF30
	protected void OnDestroy()
	{
		this.m_joinTriggerRef.RemoveCallbackOnLoad(new Action(this._OnTargetSceneLoaded));
	}

	// Token: 0x06005138 RID: 20792 RVA: 0x001AED49 File Offset: 0x001ACF49
	private void _OnTargetSceneLoaded()
	{
		this.m_joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this._joinTrigger);
	}

	// Token: 0x06005139 RID: 20793 RVA: 0x001AED5D File Offset: 0x001ACF5D
	public void SubsPublicJoin()
	{
		if (this._joinTrigger != null)
		{
			this._joinTrigger.SubsPublicJoin();
		}
	}

	// Token: 0x04006339 RID: 25401
	[FormerlySerializedAs("joinTriggerRef")]
	[SerializeField]
	private XSceneRef m_joinTriggerRef;

	// Token: 0x0400633A RID: 25402
	private GorillaNetworkJoinTrigger _joinTrigger;
}
