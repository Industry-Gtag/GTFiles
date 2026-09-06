using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x020011DC RID: 4572
	public class GenericNetworkedEventsProvider : MonoBehaviour
	{
		// Token: 0x0600743D RID: 29757 RVA: 0x0025CFAC File Offset: 0x0025B1AC
		private void OnEnable()
		{
			if (this.myRig == null)
			{
				this.myRig = base.GetComponentInParent<VRRig>();
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			}
			NetPlayer netPlayer = ((this.myRig != null) ? (this.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : NetworkSystem.Instance.LocalPlayer);
			if (netPlayer != null)
			{
				this._events.Init(netPlayer);
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Activate += this.TriggerSharedEvents;
			}
		}

		// Token: 0x0600743E RID: 29758 RVA: 0x0025D078 File Offset: 0x0025B278
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.TriggerSharedEvents;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x0600743F RID: 29759 RVA: 0x0025D0C8 File Offset: 0x0025B2C8
		private void TriggerSharedEvents(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target || info.senderID != this.myRig.creator.ActorNumber)
			{
				return;
			}
			MonkeAgent.IncrementRPCCall(info, "TriggerSharedEvents");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			this.InvokeSharedEvents(args);
		}

		// Token: 0x06007440 RID: 29760 RVA: 0x0025D11C File Offset: 0x0025B31C
		private void InvokeSharedEvents(object[] args)
		{
			if (args == null || args.Length == 0)
			{
				UnityEvent unityEvent = this.sharedEvent;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke();
				return;
			}
			else
			{
				object obj = args[0];
				if (!(obj is byte))
				{
					return;
				}
				switch ((byte)obj)
				{
				case 0:
				{
					UnityEvent unityEvent2 = this.sharedEvent;
					if (unityEvent2 == null)
					{
						return;
					}
					unityEvent2.Invoke();
					return;
				}
				case 1:
				{
					obj = args[1];
					if (!(obj is int))
					{
						return;
					}
					int num = (int)obj;
					UnityEvent<int> unityEvent3 = this.sharedEvent_int;
					if (unityEvent3 == null)
					{
						return;
					}
					unityEvent3.Invoke(num);
					return;
				}
				case 2:
					obj = args[1];
					if (obj is float)
					{
						float num2 = (float)obj;
						if (float.IsFinite(num2))
						{
							UnityEvent<float> unityEvent4 = this.sharedEvent_float;
							if (unityEvent4 == null)
							{
								return;
							}
							unityEvent4.Invoke(num2);
							return;
						}
					}
					return;
				case 3:
				{
					obj = args[1];
					if (!(obj is bool))
					{
						return;
					}
					bool flag = (bool)obj;
					UnityEvent<bool> unityEvent5 = this.sharedEvent_bool;
					if (unityEvent5 == null)
					{
						return;
					}
					unityEvent5.Invoke(flag);
					return;
				}
				case 4:
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						float num3 = 10000f;
						if ((in vector).IsValid(in num3))
						{
							UnityEvent<Vector3> unityEvent6 = this.sharedEvent_vector3;
							if (unityEvent6 == null)
							{
								return;
							}
							unityEvent6.Invoke(vector);
							return;
						}
					}
					return;
				case 5:
				{
					string text = args[1] as string;
					if (text == null || text.Length > 10)
					{
						return;
					}
					UnityEvent<string> unityEvent7 = this.sharedEvent_string;
					if (unityEvent7 == null)
					{
						return;
					}
					unityEvent7.Invoke(text);
					return;
				}
				case 6:
				{
					obj = args[1];
					if (!(obj is long))
					{
						return;
					}
					long num4 = (long)obj;
					UnityEvent<long> unityEvent8 = this.sharedEvent_long;
					if (unityEvent8 == null)
					{
						return;
					}
					unityEvent8.Invoke(num4);
					return;
				}
				case 7:
					obj = args[1];
					if (obj is Quaternion)
					{
						Quaternion quaternion = (Quaternion)obj;
						if ((in quaternion).IsValid())
						{
							UnityEvent<Quaternion> unityEvent9 = this.sharedEvent_quaternion;
							if (unityEvent9 == null)
							{
								return;
							}
							unityEvent9.Invoke(quaternion);
							return;
						}
					}
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x06007441 RID: 29761 RVA: 0x0025D2D4 File Offset: 0x0025B4D4
		private void Raise(object[] args)
		{
			if (PhotonNetwork.InRoom)
			{
				RubberDuckEvents events = this._events;
				if (((events != null) ? events.Activate : null) != null)
				{
					this._events.Activate.RaiseAll(args);
					return;
				}
			}
			this.InvokeSharedEvents(args);
		}

		// Token: 0x06007442 RID: 29762 RVA: 0x0025D310 File Offset: 0x0025B510
		public void TriggerSharedEvent()
		{
			this.Raise(new object[] { 0 });
		}

		// Token: 0x06007443 RID: 29763 RVA: 0x0025D327 File Offset: 0x0025B527
		public void TriggerSharedEvent_Int(int value)
		{
			this.Raise(new object[] { 1, value });
		}

		// Token: 0x06007444 RID: 29764 RVA: 0x0025D347 File Offset: 0x0025B547
		public void TriggerSharedEvent_Float(float value)
		{
			this.Raise(new object[] { 2, value });
		}

		// Token: 0x06007445 RID: 29765 RVA: 0x0025D367 File Offset: 0x0025B567
		public void TriggerSharedEvent_Bool(bool value)
		{
			this.Raise(new object[] { 3, value });
		}

		// Token: 0x06007446 RID: 29766 RVA: 0x0025D387 File Offset: 0x0025B587
		public void TriggerSharedEvent_Vector3(Vector3 value)
		{
			this.Raise(new object[] { 4, value });
		}

		// Token: 0x06007447 RID: 29767 RVA: 0x0025D3A7 File Offset: 0x0025B5A7
		public void TriggerSharedEvent_String(string value)
		{
			this.Raise(new object[] { 5, value });
		}

		// Token: 0x06007448 RID: 29768 RVA: 0x0025D3C2 File Offset: 0x0025B5C2
		public void TriggerSharedEvent_Long(long value)
		{
			this.Raise(new object[] { 6, value });
		}

		// Token: 0x06007449 RID: 29769 RVA: 0x0025D3E2 File Offset: 0x0025B5E2
		public void TriggerSharedEvent_Quaternion(Quaternion value)
		{
			this.Raise(new object[] { 7, value });
		}

		// Token: 0x04008406 RID: 33798
		private RubberDuckEvents _events;

		// Token: 0x04008407 RID: 33799
		private VRRig myRig;

		// Token: 0x04008408 RID: 33800
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04008409 RID: 33801
		public UnityEvent sharedEvent;

		// Token: 0x0400840A RID: 33802
		public UnityEvent<int> sharedEvent_int;

		// Token: 0x0400840B RID: 33803
		public UnityEvent<float> sharedEvent_float;

		// Token: 0x0400840C RID: 33804
		public UnityEvent<bool> sharedEvent_bool;

		// Token: 0x0400840D RID: 33805
		public UnityEvent<Vector3> sharedEvent_vector3;

		// Token: 0x0400840E RID: 33806
		public UnityEvent<string> sharedEvent_string;

		// Token: 0x0400840F RID: 33807
		public UnityEvent<long> sharedEvent_long;

		// Token: 0x04008410 RID: 33808
		public UnityEvent<Quaternion> sharedEvent_quaternion;

		// Token: 0x020011DD RID: 4573
		private enum EventType : byte
		{
			// Token: 0x04008412 RID: 33810
			None,
			// Token: 0x04008413 RID: 33811
			Int,
			// Token: 0x04008414 RID: 33812
			Float,
			// Token: 0x04008415 RID: 33813
			Bool,
			// Token: 0x04008416 RID: 33814
			Vector3,
			// Token: 0x04008417 RID: 33815
			String,
			// Token: 0x04008418 RID: 33816
			Long,
			// Token: 0x04008419 RID: 33817
			Quaternion
		}
	}
}
