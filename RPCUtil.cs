using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000856 RID: 2134
internal class RPCUtil
{
	// Token: 0x06003709 RID: 14089 RVA: 0x0012EB20 File Offset: 0x0012CD20
	public static bool NotSpam(string id, PhotonMessageInfoWrapped info, float delay)
	{
		RPCUtil.RPCCallID rpccallID = new RPCUtil.RPCCallID(id, info.senderID);
		if (!RPCUtil.RPCCallLog.ContainsKey(rpccallID))
		{
			RPCUtil.RPCCallLog.Add(rpccallID, Time.time);
			return true;
		}
		if (Time.time - RPCUtil.RPCCallLog[rpccallID] > delay)
		{
			RPCUtil.RPCCallLog[rpccallID] = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x0600370A RID: 14090 RVA: 0x0012EB81 File Offset: 0x0012CD81
	public static bool SafeValue(float v)
	{
		return !float.IsNaN(v) && float.IsFinite(v);
	}

	// Token: 0x0600370B RID: 14091 RVA: 0x0012EB93 File Offset: 0x0012CD93
	public static bool SafeValue(float v, float min, float max)
	{
		return RPCUtil.SafeValue(v) && v <= max && v >= min;
	}

	// Token: 0x04004783 RID: 18307
	private static Dictionary<RPCUtil.RPCCallID, float> RPCCallLog = new Dictionary<RPCUtil.RPCCallID, float>();

	// Token: 0x02000857 RID: 2135
	private struct RPCCallID : IEquatable<RPCUtil.RPCCallID>
	{
		// Token: 0x0600370E RID: 14094 RVA: 0x0012EBB8 File Offset: 0x0012CDB8
		public RPCCallID(string nameOfFunction, int senderId)
		{
			this._senderID = senderId;
			this._nameOfFunction = nameOfFunction;
		}

		// Token: 0x170004E3 RID: 1251
		// (get) Token: 0x0600370F RID: 14095 RVA: 0x0012EBC8 File Offset: 0x0012CDC8
		public readonly int SenderID
		{
			get
			{
				return this._senderID;
			}
		}

		// Token: 0x170004E4 RID: 1252
		// (get) Token: 0x06003710 RID: 14096 RVA: 0x0012EBD0 File Offset: 0x0012CDD0
		public readonly string NameOfFunction
		{
			get
			{
				return this._nameOfFunction;
			}
		}

		// Token: 0x06003711 RID: 14097 RVA: 0x0012EBD8 File Offset: 0x0012CDD8
		bool IEquatable<RPCUtil.RPCCallID>.Equals(RPCUtil.RPCCallID other)
		{
			return other.NameOfFunction.Equals(this.NameOfFunction) && other.SenderID.Equals(this.SenderID);
		}

		// Token: 0x04004784 RID: 18308
		private int _senderID;

		// Token: 0x04004785 RID: 18309
		private string _nameOfFunction;
	}
}
