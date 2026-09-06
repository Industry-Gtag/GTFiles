using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;

namespace MaterialCycler
{
	// Token: 0x0200116D RID: 4461
	[RequireComponent(typeof(PhotonView))]
	public class MaterialCyclerManager : MonoBehaviour
	{
		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x06006FD7 RID: 28631 RVA: 0x00240D73 File Offset: 0x0023EF73
		// (set) Token: 0x06006FD8 RID: 28632 RVA: 0x00240D7A File Offset: 0x0023EF7A
		public static MaterialCyclerManager Instance { get; private set; }

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x06006FD9 RID: 28633 RVA: 0x00240D82 File Offset: 0x0023EF82
		// (set) Token: 0x06006FDA RID: 28634 RVA: 0x00240D8A File Offset: 0x0023EF8A
		public float SyncTimeOut { get; private set; } = 1f;

		// Token: 0x06006FDB RID: 28635 RVA: 0x00240D93 File Offset: 0x0023EF93
		private void Awake()
		{
			if (MaterialCyclerManager.Instance == null)
			{
				MaterialCyclerManager.Instance = this;
			}
			else if (MaterialCyclerManager.Instance != this)
			{
				Object.Destroy(this);
			}
			this._photonView = base.GetComponent<PhotonView>();
		}

		// Token: 0x06006FDC RID: 28636 RVA: 0x00240DCC File Offset: 0x0023EFCC
		public void RegisterCycler(int key, MaterialCycler cycler)
		{
			List<MaterialCycler> list;
			if (!this._cyclers.TryGetValue(key, out list))
			{
				this._cyclers[key] = new List<MaterialCycler> { cycler };
				this._numMaterialsByKey[key] = cycler.NumMaterials;
				this._currentMaterialIndexByKey[key] = cycler.index;
				return;
			}
			if (!list.Contains(cycler))
			{
				list.Add(cycler);
			}
			if (cycler.NumMaterials != this._numMaterialsByKey[key])
			{
				throw new Exception("New cycler does not match size of other matching cyclers.");
			}
			int num = this._currentMaterialIndexByKey[key];
			if (num < 0 || num >= cycler.NumMaterials)
			{
				num = list[0].index;
				this._currentMaterialIndexByKey[key] = num;
			}
			cycler.CycleMaterial(num);
		}

		// Token: 0x06006FDD RID: 28637 RVA: 0x00240E90 File Offset: 0x0023F090
		public void UnregisterCycler(MaterialCycler cycler)
		{
			int keyHash = cycler.KeyHash;
			List<MaterialCycler> list;
			if (!this._cyclers.TryGetValue(keyHash, out list))
			{
				return;
			}
			if (list.Remove(cycler) && list.Count == 0)
			{
				this._cyclers.Remove(keyHash);
				this._numMaterialsByKey.Remove(keyHash);
				this._currentMaterialIndexByKey.Remove(keyHash);
			}
		}

		// Token: 0x06006FDE RID: 28638 RVA: 0x00240EF0 File Offset: 0x0023F0F0
		public void CycleKey(int key)
		{
			int num = (this._currentMaterialIndexByKey[key] + 1) % this._numMaterialsByKey[key];
			this._photonView.RPC("RPC_CycleKey", RpcTarget.All, new object[] { key, num });
		}

		// Token: 0x06006FDF RID: 28639 RVA: 0x00240F44 File Offset: 0x0023F144
		private void RPC_CycleKey(int key, int index, PhotonMessageInfo info)
		{
			List<MaterialCycler> list;
			if (!this._cyclers.TryGetValue(key, out list))
			{
				return;
			}
			this._currentMaterialIndexByKey[key] = index;
			foreach (MaterialCycler materialCycler in list)
			{
				materialCycler.CycleMaterial(index);
			}
		}

		// Token: 0x06006FE0 RID: 28640 RVA: 0x00240FB0 File Offset: 0x0023F1B0
		public void Synchronize(int key, int materialIndex, Color c)
		{
			this.UpdateMaterialCycler(key, materialIndex, c);
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			int num = MaterialCyclerManager.PackColor(c);
			this._photonView.RPC("RPC_Synchronize", RpcTarget.Others, new object[] { key, materialIndex, num });
		}

		// Token: 0x06006FE1 RID: 28641 RVA: 0x00241008 File Offset: 0x0023F208
		[PunRPC]
		private void RPC_Synchronize(int key, int materialIndex, int colourPacked, PhotonMessageInfo info)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 21, info.SentServerTime) || materialIndex < 0)
			{
				return;
			}
			this.UpdateMaterialCycler(key, materialIndex, MaterialCyclerManager.UnPackColour(colourPacked));
		}

		// Token: 0x06006FE2 RID: 28642 RVA: 0x00241058 File Offset: 0x0023F258
		private void UpdateMaterialCycler(int key, int materialIndex, Color colour)
		{
			List<MaterialCycler> list;
			if (!this._cyclers.TryGetValue(key, out list))
			{
				return;
			}
			this._currentMaterialIndexByKey[key] = materialIndex;
			foreach (MaterialCycler materialCycler in list)
			{
				materialCycler.MaterialCyclerNetworked_OnSynchronize(materialIndex, colour);
			}
		}

		// Token: 0x06006FE3 RID: 28643 RVA: 0x002410C4 File Offset: 0x0023F2C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int PackColor(Color c)
		{
			int num = Mathf.CeilToInt(c.r * 9f);
			int num2 = Mathf.CeilToInt(c.g * 9f);
			int num3 = Mathf.CeilToInt(c.b * 9f);
			return num | (num2 << 8) | (num3 << 16);
		}

		// Token: 0x06006FE4 RID: 28644 RVA: 0x00241110 File Offset: 0x0023F310
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Color UnPackColour(int colourPacked)
		{
			int num = colourPacked & 255;
			int num2 = (colourPacked >> 8) & 255;
			int num3 = (colourPacked >> 16) & 255;
			float num4 = (float)Mathf.Clamp(num, 0, 9);
			num2 = Mathf.Clamp(num2, 0, 9);
			num3 = Mathf.Clamp(num3, 0, 9);
			float num5 = num4 / 9f;
			float num6 = (float)num2 / 9f;
			float num7 = (float)num3 / 9f;
			return new Color(num5, num6, num7);
		}

		// Token: 0x04007FC6 RID: 32710
		private readonly Dictionary<int, List<MaterialCycler>> _cyclers = new Dictionary<int, List<MaterialCycler>>();

		// Token: 0x04007FC7 RID: 32711
		private readonly Dictionary<int, int> _numMaterialsByKey = new Dictionary<int, int>();

		// Token: 0x04007FC8 RID: 32712
		private readonly Dictionary<int, int> _currentMaterialIndexByKey = new Dictionary<int, int>();

		// Token: 0x04007FC9 RID: 32713
		private PhotonView _photonView;
	}
}
