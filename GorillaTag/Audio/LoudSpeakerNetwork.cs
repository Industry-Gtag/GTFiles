using System;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012AD RID: 4781
	public class LoudSpeakerNetwork : MonoBehaviour
	{
		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x06007822 RID: 30754 RVA: 0x0026E3B0 File Offset: 0x0026C5B0
		public AudioSource[] SpeakerSources
		{
			get
			{
				return this._speakerSources;
			}
		}

		// Token: 0x06007823 RID: 30755 RVA: 0x0026E3B8 File Offset: 0x0026C5B8
		private void Awake()
		{
			if (this._speakerSources == null || this._speakerSources.Length == 0)
			{
				this._speakerSources = base.transform.GetComponentsInChildren<AudioSource>();
			}
			this._currentSpeakers = new List<Speaker>();
		}

		// Token: 0x06007824 RID: 30756 RVA: 0x0026E3E8 File Offset: 0x0026C5E8
		private void Start()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer) && rigContainer.Voice != null)
			{
				GTSpeaker gtspeaker = (GTSpeaker)rigContainer.Voice.SpeakerInUse;
				if (gtspeaker != null)
				{
					gtspeaker.AddExternalAudioSources(this._speakerSources);
				}
			}
		}

		// Token: 0x06007825 RID: 30757 RVA: 0x0026E433 File Offset: 0x0026C633
		private bool GetParentRigContainer(out RigContainer rigContainer)
		{
			if (this._rigContainer == null)
			{
				this._rigContainer = base.transform.GetComponentInParent<RigContainer>();
			}
			rigContainer = this._rigContainer;
			return rigContainer != null;
		}

		// Token: 0x06007826 RID: 30758 RVA: 0x0026E464 File Offset: 0x0026C664
		private void OnEnable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.AddLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06007827 RID: 30759 RVA: 0x0026E484 File Offset: 0x0026C684
		private void OnDisable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.RemoveLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06007828 RID: 30760 RVA: 0x0026E4A2 File Offset: 0x0026C6A2
		public void AddSpeaker(Speaker speaker)
		{
			if (this._currentSpeakers.Contains(speaker))
			{
				return;
			}
			this._currentSpeakers.Add(speaker);
		}

		// Token: 0x06007829 RID: 30761 RVA: 0x0026E4BF File Offset: 0x0026C6BF
		public void RemoveSpeaker(Speaker speaker)
		{
			this._currentSpeakers.Remove(speaker);
		}

		// Token: 0x0600782A RID: 30762 RVA: 0x0026E4CE File Offset: 0x0026C6CE
		public void StartBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(true, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x0600782B RID: 30763 RVA: 0x0026E4EC File Offset: 0x0026C6EC
		public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = true;
					if (this.ReparentLocalSpeaker)
					{
						Transform transform = this._rigContainer.Voice.SpeakerInUse.transform;
						transform.transform.SetParent(base.transform, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
					gtspeaker.ToggleAudioSource(true);
					gtspeaker.BroadcastExternal = true;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
					{
						Transform transform2 = rigContainer.Voice.SpeakerInUse.transform;
						transform2.SetParent(base.transform, false);
						transform2.localPosition = Vector3.zero;
					}
				}
			}
			this._currentSpeakerActor = actorNumber;
		}

		// Token: 0x0600782C RID: 30764 RVA: 0x0026E60C File Offset: 0x0026C80C
		public void StopBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(false, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x0600782D RID: 30765 RVA: 0x0026E62C File Offset: 0x0026C82C
		public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = false;
					RigContainer rigContainer;
					if (this.ReparentLocalSpeaker && this.GetParentRigContainer(out rigContainer))
					{
						Transform transform = rigContainer.Voice.SpeakerInUse.transform;
						transform.SetParent(rigContainer.SpeakerHead, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			if (actorNumber == this._currentSpeakerActor)
			{
				using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
						gtspeaker.ToggleAudioSource(false);
						gtspeaker.BroadcastExternal = false;
						RigContainer rigContainer2;
						if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer2))
						{
							Transform transform2 = rigContainer2.Voice.SpeakerInUse.transform;
							transform2.SetParent(rigContainer2.SpeakerHead, false);
							transform2.localPosition = Vector3.zero;
						}
					}
				}
				this._currentSpeakerActor = -1;
			}
		}

		// Token: 0x04008855 RID: 34901
		[SerializeField]
		private AudioSource[] _speakerSources;

		// Token: 0x04008856 RID: 34902
		[SerializeField]
		private List<Speaker> _currentSpeakers;

		// Token: 0x04008857 RID: 34903
		[SerializeField]
		private int _currentSpeakerActor = -1;

		// Token: 0x04008858 RID: 34904
		public bool ReparentLocalSpeaker = true;

		// Token: 0x04008859 RID: 34905
		private RigContainer _rigContainer;

		// Token: 0x0400885A RID: 34906
		private GTRecorder _localRecorder;
	}
}
