using System;
using System.Collections.Generic;
using System.IO;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class SICombinedTerminal : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060007D2 RID: 2002 RVA: 0x0002ACB7 File Offset: 0x00028EB7
	public bool IsAuthority
	{
		get
		{
			return this.superInfection.siManager.gameEntityManager.IsAuthority();
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060007D3 RID: 2003 RVA: 0x0002ACCE File Offset: 0x00028ECE
	public SuperInfectionManager SIManager
	{
		get
		{
			return this.superInfection.siManager;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060007D4 RID: 2004 RVA: 0x0002ACDB File Offset: 0x00028EDB
	public int ActivePage
	{
		get
		{
			return this._activePage;
		}
	}

	// Token: 0x060007D5 RID: 2005 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060007D6 RID: 2006 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060007D7 RID: 2007 RVA: 0x0002ACE4 File Offset: 0x00028EE4
	public void SliceUpdate()
	{
		this.wasOccupied = this.isOccupied;
		this.isOccupied = false;
		this.isOccupiedByActivePlayer = false;
		VRRigCache.Instance.GetActiveRigs(this.rigs);
		for (int i = 0; i < this.rigs.Count; i++)
		{
			if (this.activeUserBounds.bounds.Contains(this.rigs[i].transform.position))
			{
				this.isOccupied = true;
				if (this.rigs[i].Creator.IsLocal)
				{
					this.isOccupiedByActivePlayer = true;
					break;
				}
			}
		}
		if (this.isOccupied)
		{
			float num = Time.time - SIProgression.Instance.timeTelemetryLastChecked;
			if (this.activePlayer != null && this.activePlayer.ActorNr == SIPlayer.LocalPlayer.ActorNr && this.isOccupiedByLocalPlayer)
			{
				SIProgression.Instance.activeTerminalTimeInterval += num;
				SIProgression.Instance.activeTerminalTimeTotal += num;
			}
			if (!this.wasOccupied && this.state == EKioskAnimState.Closing)
			{
				this.AnimQueueState(EKioskAnimState.Opening);
			}
			this.foldupTimeStart = Time.time;
			return;
		}
		if (this.state == EKioskAnimState.Opening && Time.time > this.foldupTimeStart + this.foldupDelay && !this.isOccupied)
		{
			this.AnimQueueState(EKioskAnimState.Closing);
		}
	}

	// Token: 0x060007D8 RID: 2008 RVA: 0x0002AE40 File Offset: 0x00029040
	public void Reset()
	{
		this.activePlayer = null;
		this.SetActivePage(0);
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.dispenser.Reset();
		this.techTree.Reset();
		this.resourceCollection.Reset();
		this.AnimQueueState(EKioskAnimState.Closing);
	}

	// Token: 0x060007D9 RID: 2009 RVA: 0x0002AEA4 File Offset: 0x000290A4
	public void Awake()
	{
		if (this.superInfection == null)
		{
			this.superInfection = base.GetComponentInParent<SuperInfection>();
		}
		this.dispenser.Initialize();
		this.techTree.Initialize();
		this.resourceCollection.Initialize();
		this.Reset();
	}

	// Token: 0x060007DA RID: 2010 RVA: 0x0002AEF4 File Offset: 0x000290F4
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.activePlayer != null)
		{
			stream.SendNext(this.activePlayer.ActorNr);
		}
		else
		{
			stream.SendNext(-1);
		}
		stream.SendNext(this._activePage);
		this.dispenser.WriteDataPUN(stream, info);
		this.techTree.WriteDataPUN(stream, info);
		this.resourceCollection.WriteDataPUN(stream, info);
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002AF6C File Offset: 0x0002916C
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.activePlayer = SIPlayer.Get((int)stream.ReceiveNext());
		this._activePage = (int)stream.ReceiveNext();
		this.dispenser.ReadDataPUN(stream, info);
		this.techTree.ReadDataPUN(stream, info);
		this.resourceCollection.ReadDataPUN(stream, info);
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x0002AFC7 File Offset: 0x000291C7
	public void SerializeZoneData(BinaryWriter writer)
	{
		writer.Write(this._activePage);
		this.dispenser.ZoneDataSerializeWrite(writer);
		this.techTree.ZoneDataSerializeWrite(writer);
		this.resourceCollection.ZoneDataSerializeWrite(writer);
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x0002AFF9 File Offset: 0x000291F9
	public void DeserializeZoneData(BinaryReader reader)
	{
		this._activePage = reader.ReadInt32();
		this.SetActivePage(this._activePage);
		this.dispenser.ZoneDataSerializeRead(reader);
		this.techTree.ZoneDataSerializeRead(reader);
		this.resourceCollection.ZoneDataSerializeRead(reader);
	}

	// Token: 0x060007DE RID: 2014 RVA: 0x0002B038 File Offset: 0x00029238
	public void PlayerHandScanned(int actorNr)
	{
		if (!this.IsAuthority)
		{
			this.superInfection.siManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalHandScan, new object[] { this.index });
			return;
		}
		SIPlayer siplayer = SIPlayer.Get(actorNr);
		if (this.activePlayer != null && this.activePlayer.isActiveAndEnabled && siplayer != this.activePlayer && this.activeUserBounds.bounds.Contains(this.activePlayer.transform.position))
		{
			return;
		}
		this.activePlayer = siplayer;
		this.dispenser.PlayerHandScanned(actorNr);
		this.techTree.PlayerHandScanned(actorNr);
		this.resourceCollection.PlayerHandScanned(actorNr);
	}

	// Token: 0x060007DF RID: 2015 RVA: 0x0002B0F4 File Offset: 0x000292F4
	public void TouchscreenButtonPressed(SITouchscreenButton.SITouchscreenButtonType buttonType, int data, int actorNr, SICombinedTerminal.TerminalSubFunction subFunction)
	{
		if (!this.IsAuthority)
		{
			this.SIManager.CallRPC(SuperInfectionManager.ClientToAuthorityRPC.CombinedTerminalButtonPress, new object[]
			{
				(int)buttonType,
				data,
				(int)subFunction,
				this.index
			});
			return;
		}
		switch (subFunction)
		{
		case SICombinedTerminal.TerminalSubFunction.TechTree:
			this.techTree.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.GadgetDispenser:
			this.dispenser.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		case SICombinedTerminal.TerminalSubFunction.ResourceCollection:
			this.resourceCollection.TouchscreenButtonPressed(buttonType, data, actorNr);
			return;
		default:
			return;
		}
	}

	// Token: 0x060007E0 RID: 2016 RVA: 0x0002B186 File Offset: 0x00029386
	public void SetActivePage(int pageId)
	{
		this._activePage = pageId;
		if (this.techTree.IsValidPage(pageId))
		{
			this.techTree.SetActivePage();
		}
		if (this.dispenser.IsValidPage(pageId))
		{
			this.dispenser.SetActivePage();
		}
	}

	// Token: 0x060007E1 RID: 2017 RVA: 0x0002B1C4 File Offset: 0x000293C4
	private void AnimQueueState(EKioskAnimState newState)
	{
		this.state = newState;
		for (int i = 0; i < this.m_gtAnimators.Length; i++)
		{
			if (!(this.m_gtAnimators[i] == null))
			{
				this.m_gtAnimators[i].QueueState((long)newState);
			}
		}
	}

	// Token: 0x060007E2 RID: 2018 RVA: 0x0002B20A File Offset: 0x0002940A
	public void PlayWrongPlayerBuzz(Transform xForm)
	{
		this.wrongPlayerBuzz.transform.position = xForm.position;
		this.wrongPlayerBuzz.PlayOneShot(this.wrongPlayerBuzz.clip);
	}

	// Token: 0x040009E2 RID: 2530
	[DebugReadout]
	internal int index;

	// Token: 0x040009E3 RID: 2531
	[DebugReadout]
	internal SIPlayer activePlayer;

	// Token: 0x040009E4 RID: 2532
	[DebugReadout]
	internal bool isOccupiedByActivePlayer;

	// Token: 0x040009E5 RID: 2533
	[DebugReadout]
	internal bool isOccupiedByLocalPlayer;

	// Token: 0x040009E6 RID: 2534
	[DebugReadout]
	internal bool isOccupied;

	// Token: 0x040009E7 RID: 2535
	[DebugReadout]
	internal bool wasOccupied;

	// Token: 0x040009E8 RID: 2536
	[DebugReadout]
	internal SuperInfection superInfection;

	// Token: 0x040009E9 RID: 2537
	public SIGadgetDispenser dispenser;

	// Token: 0x040009EA RID: 2538
	public SITechTreeStation techTree;

	// Token: 0x040009EB RID: 2539
	public SIResourceCollection resourceCollection;

	// Token: 0x040009EC RID: 2540
	[SerializeField]
	private GTAnimator[] m_gtAnimators;

	// Token: 0x040009ED RID: 2541
	public Collider activeUserBounds;

	// Token: 0x040009EE RID: 2542
	public float foldupDelay = 20f;

	// Token: 0x040009EF RID: 2543
	private float foldupTimeStart;

	// Token: 0x040009F0 RID: 2544
	private EKioskAnimState state;

	// Token: 0x040009F1 RID: 2545
	[DebugReadout]
	private int _activePage;

	// Token: 0x040009F2 RID: 2546
	[Header("Flattener")]
	public Transform zeroZeroImage;

	// Token: 0x040009F3 RID: 2547
	public Transform onePointTwoText;

	// Token: 0x040009F4 RID: 2548
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x040009F5 RID: 2549
	public AudioSource wrongPlayerBuzz;

	// Token: 0x0200013B RID: 315
	public enum TerminalSubFunction
	{
		// Token: 0x040009F7 RID: 2551
		TechTree,
		// Token: 0x040009F8 RID: 2552
		GadgetDispenser,
		// Token: 0x040009F9 RID: 2553
		ResourceCollection
	}
}
