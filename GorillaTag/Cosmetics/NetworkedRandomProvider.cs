using System;
using System.Text;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001349 RID: 4937
	public class NetworkedRandomProvider : MonoBehaviour
	{
		// Token: 0x06007BBF RID: 31679 RVA: 0x00286D12 File Offset: 0x00284F12
		private void Awake()
		{
			if (this.parentTransferable == null)
			{
				this.parentTransferable = base.GetComponentInParent<TransferrableObject>();
			}
		}

		// Token: 0x06007BC0 RID: 31680 RVA: 0x00286D2E File Offset: 0x00284F2E
		private void OnEnable()
		{
			this.EnsureOwner();
		}

		// Token: 0x06007BC1 RID: 31681 RVA: 0x00286D38 File Offset: 0x00284F38
		private void OnValidate()
		{
			if (this.windowSeconds < 0.01f)
			{
				this.windowSeconds = 0.01f;
			}
			if (this.floatRange.y < this.floatRange.x)
			{
				ref float ptr = ref this.floatRange.x;
				float y = this.floatRange.y;
				float x = this.floatRange.x;
				ptr = y;
				this.floatRange.y = x;
			}
			if (this.doubleMax < this.doubleMin)
			{
				double num = this.doubleMax;
				double num2 = this.doubleMin;
				this.doubleMin = num;
				this.doubleMax = num2;
			}
		}

		// Token: 0x06007BC2 RID: 31682 RVA: 0x00286DD8 File Offset: 0x00284FD8
		private void Update()
		{
			long num = (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
			this.debugWindow = num;
		}

		// Token: 0x06007BC3 RID: 31683 RVA: 0x00286E01 File Offset: 0x00285001
		private bool ShowFloatRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.FloatRange;
		}

		// Token: 0x06007BC4 RID: 31684 RVA: 0x00286E0C File Offset: 0x0028500C
		private bool ShowDoubleRange()
		{
			return this.outputMode == NetworkedRandomProvider.OutputMode.DoubleRange;
		}

		// Token: 0x06007BC5 RID: 31685 RVA: 0x00286E17 File Offset: 0x00285017
		private long GetWindowIndex()
		{
			return (long)Math.Floor(this.GetSharedTime() / (double)this.windowSeconds);
		}

		// Token: 0x06007BC6 RID: 31686 RVA: 0x00286E2D File Offset: 0x0028502D
		private double GetSharedTime()
		{
			if (PhotonNetwork.InRoom)
			{
				return PhotonNetwork.Time;
			}
			return (double)Time.realtimeSinceStartup;
		}

		// Token: 0x06007BC7 RID: 31687 RVA: 0x00286E42 File Offset: 0x00285042
		private static ulong Mix64(ulong x)
		{
			x += 11400714819323198485UL;
			x = (x ^ (x >> 30)) * 13787848793156543929UL;
			x = (x ^ (x >> 27)) * 10723151780598845931UL;
			x ^= x >> 31;
			return x;
		}

		// Token: 0x06007BC8 RID: 31688 RVA: 0x00286E7E File Offset: 0x0028507E
		private static ulong BuildSeed(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (ulong)(windowIndex ^ (long)((long)((ulong)ownerId) << 32) ^ (long)((ulong)objectSalt * 11400714819323198485UL) ^ (long)((ulong)roomSalt * 15183679468541472403UL));
		}

		// Token: 0x06007BC9 RID: 31689 RVA: 0x00286EA1 File Offset: 0x002850A1
		private static float UnitFloat01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (uint)(NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 40) * 5.9604645E-08f;
		}

		// Token: 0x06007BCA RID: 31690 RVA: 0x00286EBD File Offset: 0x002850BD
		private static double UnitDouble01(long windowIndex, int ownerId, int objectSalt, uint roomSalt)
		{
			return (NetworkedRandomProvider.Mix64(NetworkedRandomProvider.BuildSeed(windowIndex, ownerId, objectSalt, roomSalt)) >> 11) * 1.1102230246251565E-16;
		}

		// Token: 0x06007BCB RID: 31691 RVA: 0x00286EDC File Offset: 0x002850DC
		public float NextFloat01()
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string text;
				if (!PhotonNetwork.InRoom)
				{
					text = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					text = ((currentRoom != null) ? currentRoom.Name : null) ?? "no_room";
				}
				num = NetworkedRandomProvider.StableHash(text);
			}
			uint num2 = num;
			float num3 = NetworkedRandomProvider.UnitFloat01(windowIndex, this.OwnerID, this.objectSalt, num2);
			this.debugResult = num3;
			return num3;
		}

		// Token: 0x06007BCC RID: 31692 RVA: 0x00286F4C File Offset: 0x0028514C
		public float NextFloat(float min, float max)
		{
			float num = this.NextFloat01();
			if (max < min)
			{
				float num2 = max;
				float num3 = min;
				min = num2;
				max = num3;
			}
			return Mathf.Lerp(min, max, num);
		}

		// Token: 0x06007BCD RID: 31693 RVA: 0x00286F74 File Offset: 0x00285174
		public double NextDouble(double min, double max)
		{
			this.EnsureOwner();
			long windowIndex = this.GetWindowIndex();
			uint num;
			if (!this.includeRoomNameInSeed)
			{
				num = 0U;
			}
			else
			{
				string text;
				if (!PhotonNetwork.InRoom)
				{
					text = "no_room";
				}
				else
				{
					Room currentRoom = PhotonNetwork.CurrentRoom;
					text = ((currentRoom != null) ? currentRoom.Name : null) ?? "no_room";
				}
				num = NetworkedRandomProvider.StableHash(text);
			}
			uint num2 = num;
			double num3 = NetworkedRandomProvider.UnitDouble01(windowIndex, this.OwnerID, this.objectSalt, num2);
			if (max < min)
			{
				double num4 = max;
				double num5 = min;
				min = num4;
				max = num5;
			}
			double num6 = min + (max - min) * num3;
			this.debugResult = (float)num6;
			return num6;
		}

		// Token: 0x06007BCE RID: 31694 RVA: 0x00286FF8 File Offset: 0x002851F8
		public float GetSelectedAsFloat()
		{
			switch (this.outputMode)
			{
			default:
				return this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return (float)this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return (float)this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x06007BCF RID: 31695 RVA: 0x00287074 File Offset: 0x00285274
		public double GetSelectedAsDouble()
		{
			switch (this.outputMode)
			{
			default:
				return (double)this.NextFloat01();
			case NetworkedRandomProvider.OutputMode.Double01:
				return this.NextDouble(0.0, 1.0);
			case NetworkedRandomProvider.OutputMode.FloatRange:
				return (double)this.NextFloat(this.floatRange.x, this.floatRange.y);
			case NetworkedRandomProvider.OutputMode.DoubleRange:
				return this.NextDouble(this.doubleMin, this.doubleMax);
			}
		}

		// Token: 0x06007BD0 RID: 31696 RVA: 0x002870F0 File Offset: 0x002852F0
		private static uint StableHash(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0U;
			}
			uint num = 2166136261U;
			for (int i = 0; i < s.Length; i++)
			{
				num ^= (uint)s[i];
				num *= 16777619U;
			}
			return num;
		}

		// Token: 0x06007BD1 RID: 31697 RVA: 0x00287131 File Offset: 0x00285331
		private void EnsureOwner()
		{
			if (this.OwnerID == 0)
			{
				this.TrySetID();
			}
		}

		// Token: 0x06007BD2 RID: 31698 RVA: 0x00287144 File Offset: 0x00285344
		private void TrySetID()
		{
			if (this.parentTransferable == null)
			{
				string name = base.gameObject.scene.name;
				string text = "/";
				string hierarchyPath = NetworkedRandomProvider.GetHierarchyPath(base.transform);
				Type type = base.GetType();
				string text2 = name + text + hierarchyPath + ((type != null) ? type.ToString() : null);
				this.OwnerID = text2.GetStaticHash();
				return;
			}
			if (this.parentTransferable.IsLocalObject())
			{
				PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
				if (instance != null)
				{
					string playFabPlayerId = instance.GetPlayFabPlayerId();
					Type type2 = base.GetType();
					this.OwnerID = (playFabPlayerId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
					return;
				}
			}
			else if (this.parentTransferable.targetRig != null && this.parentTransferable.targetRig.creator != null)
			{
				string userId = this.parentTransferable.targetRig.creator.UserId;
				Type type3 = base.GetType();
				this.OwnerID = (userId + ((type3 != null) ? type3.ToString() : null)).GetStaticHash();
			}
		}

		// Token: 0x06007BD3 RID: 31699 RVA: 0x00287250 File Offset: 0x00285450
		private static string GetHierarchyPath(Transform t)
		{
			StringBuilder stringBuilder = new StringBuilder();
			while (t != null)
			{
				stringBuilder.Insert(0, "/" + t.name + "#" + t.GetSiblingIndex().ToString());
				t = t.parent;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04008DCF RID: 36303
		[Header("Time Granularity")]
		[Min(0.01f)]
		[Tooltip("Length of the time bucket (seconds). Within a bucket the pick is fixed; re-rolls next bucket.")]
		[SerializeField]
		private float windowSeconds = 1f;

		// Token: 0x04008DD0 RID: 36304
		[Tooltip("Mix room name into seed so different rooms never collide.")]
		[SerializeField]
		private bool includeRoomNameInSeed = true;

		// Token: 0x04008DD1 RID: 36305
		[Tooltip("Optional - If multiple component live on the same cosmetic, use different salts.")]
		[SerializeField]
		private int objectSalt;

		// Token: 0x04008DD2 RID: 36306
		[Header("Output")]
		[SerializeField]
		private NetworkedRandomProvider.OutputMode outputMode;

		// Token: 0x04008DD3 RID: 36307
		[SerializeField]
		private Vector2 floatRange = new Vector2(0f, 1f);

		// Token: 0x04008DD4 RID: 36308
		[SerializeField]
		private double doubleMin;

		// Token: 0x04008DD5 RID: 36309
		[SerializeField]
		private double doubleMax = 1.0;

		// Token: 0x04008DD6 RID: 36310
		private TransferrableObject parentTransferable;

		// Token: 0x04008DD7 RID: 36311
		private int OwnerID;

		// Token: 0x04008DD8 RID: 36312
		[Header("Debug")]
		[SerializeField]
		private long debugWindow;

		// Token: 0x04008DD9 RID: 36313
		[SerializeField]
		private float debugResult;

		// Token: 0x0200134A RID: 4938
		public enum OutputMode
		{
			// Token: 0x04008DDB RID: 36315
			Float01,
			// Token: 0x04008DDC RID: 36316
			Double01,
			// Token: 0x04008DDD RID: 36317
			FloatRange,
			// Token: 0x04008DDE RID: 36318
			DoubleRange
		}
	}
}
