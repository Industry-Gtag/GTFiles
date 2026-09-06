using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200045C RID: 1116
[RequireComponent(typeof(PhotonView))]
public class NetworkSceneObject : SimulationBehaviour
{
	// Token: 0x170002B8 RID: 696
	// (get) Token: 0x06001AC7 RID: 6855 RVA: 0x00094AD4 File Offset: 0x00092CD4
	public bool IsMine
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06001AC8 RID: 6856 RVA: 0x00094AE1 File Offset: 0x00092CE1
	protected virtual void Start()
	{
		if (this.photonView == null)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06001AC9 RID: 6857 RVA: 0x00094AFD File Offset: 0x00092CFD
	protected virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
	}

	// Token: 0x06001ACA RID: 6858 RVA: 0x00094B05 File Offset: 0x00092D05
	protected virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
	}

	// Token: 0x06001ACB RID: 6859 RVA: 0x00094B10 File Offset: 0x00092D10
	private void RegisterOnRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.AddGlobal(this);
		}
	}

	// Token: 0x06001ACC RID: 6860 RVA: 0x00094B48 File Offset: 0x00092D48
	private void RemoveFromRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.RemoveGlobal(this);
		}
	}

	// Token: 0x04002568 RID: 9576
	public PhotonView photonView;
}
