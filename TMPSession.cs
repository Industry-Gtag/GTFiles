using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KID.Model;
using UnityEngine;

// Token: 0x02000B4A RID: 2890
public class TMPSession
{
	// Token: 0x1700070F RID: 1807
	// (get) Token: 0x060049A1 RID: 18849 RVA: 0x00188726 File Offset: 0x00186926
	public bool IsValidSession
	{
		get
		{
			return (this.IsDefault && this.Permissions != null && this.Permissions.Count > 0) || (!this.IsDefault && this.SessionId != Guid.Empty);
		}
	}

	// Token: 0x060049A2 RID: 18850 RVA: 0x00188764 File Offset: 0x00186964
	public TMPSession(Session session, KIDDefaultSession defaultSession, SessionStatus status)
	{
		this.Permissions = new Dictionary<EKIDFeatures, Permission>();
		this.OptedInPermissions = new HashSet<EKIDFeatures>();
		this.SessionStatus = status;
		if (session == null && defaultSession == null)
		{
			return;
		}
		if (session == null)
		{
			this.IsDefault = true;
			this.AgeStatus = defaultSession.AgeStatus;
			this.Age = defaultSession.Age;
			this.InitialiseDefaultPermissionSet(defaultSession);
			return;
		}
		this.SessionId = session.SessionId;
		this.Etag = session.Etag;
		this.AgeStatus = session.AgeStatus;
		this.KidStatus = session.Status;
		this.DateOfBirth = session.DateOfBirth;
		this.KUID = session.Kuid;
		this.Jurisdiction = session.Jurisdiction;
		this.ManagedBy = session.ManagedBy;
		this.Age = this.GetAgeFromDateOfBirth();
		for (int i = 0; i < session.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(session.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, session.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x060049A3 RID: 18851 RVA: 0x001888A8 File Offset: 0x00186AA8
	public void SetOptInPermissions(string[] optedInPermissions)
	{
		if (optedInPermissions == null || optedInPermissions.Length == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning without setting.");
			return;
		}
		int num = 0;
		for (;;)
		{
			int num2 = num;
			int? num3 = ((optedInPermissions != null) ? new int?(optedInPermissions.Length) : null);
			if (!((num2 < num3.GetValueOrDefault()) & (num3 != null)))
			{
				break;
			}
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(optedInPermissions[num]);
			if (ekidfeatures != null)
			{
				this.OptInToPermission(ekidfeatures.Value, true);
			}
			num++;
		}
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Constructor OptedInPermissions: {0}", this.GetOptedInPermissions()));
	}

	// Token: 0x060049A4 RID: 18852 RVA: 0x0018892F File Offset: 0x00186B2F
	public bool TryGetPermission(EKIDFeatures feature, out Permission permission)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.LogError("[KID::SESSION] Tried retreiving permission for [" + feature.ToStandardisedString() + "], but does not exist");
			permission = null;
			return false;
		}
		permission = this.Permissions[feature];
		return true;
	}

	// Token: 0x060049A5 RID: 18853 RVA: 0x0018896D File Offset: 0x00186B6D
	public List<Permission> GetAllPermissions()
	{
		return this.Permissions.Values.ToList<Permission>();
	}

	// Token: 0x060049A6 RID: 18854 RVA: 0x00188980 File Offset: 0x00186B80
	public bool HasPermissionForFeature(EKIDFeatures feature)
	{
		Permission permission;
		if (!this.TryGetPermission(feature, out permission))
		{
			Debug.LogError("[KID::SESSION] Tried checking for permission but couldn't find [" + feature.ToStandardisedString() + "]. Assuming disabled");
			return false;
		}
		return permission.Enabled;
	}

	// Token: 0x060049A7 RID: 18855 RVA: 0x001889BC File Offset: 0x00186BBC
	public void OptInToPermission(EKIDFeatures feature, bool optIn)
	{
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Opting in to permission for [{0}] with optIn: {1}", feature.ToStandardisedString(), optIn));
		if (optIn && !this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Add(feature);
			return;
		}
		if (!optIn && this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Remove(feature);
			return;
		}
	}

	// Token: 0x060049A8 RID: 18856 RVA: 0x00188A22 File Offset: 0x00186C22
	public bool HasOptedInToPermission(EKIDFeatures feature)
	{
		return this.OptedInPermissions.Contains(feature);
	}

	// Token: 0x060049A9 RID: 18857 RVA: 0x00188A30 File Offset: 0x00186C30
	public string[] GetOptedInPermissions()
	{
		if (this.OptedInPermissions == null || this.OptedInPermissions.Count == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning empty array.");
			return Array.Empty<string>();
		}
		return this.OptedInPermissions.Select((EKIDFeatures f) => f.ToStandardisedString()).ToArray<string>();
	}

	// Token: 0x060049AA RID: 18858 RVA: 0x00188A94 File Offset: 0x00186C94
	public void UpdatePermission(EKIDFeatures feature, Permission newData)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.Log("[KID::SESSION] Trying to update permission, but could not find [" + feature.ToStandardisedString() + "] in dictionary. Will add new one");
			this.Permissions.Add(feature, null);
		}
		this.Permissions[feature] = newData;
	}

	// Token: 0x060049AB RID: 18859 RVA: 0x00188AE4 File Offset: 0x00186CE4
	private void InitialiseDefaultPermissionSet(KIDDefaultSession defaultSession)
	{
		for (int i = 0; i < defaultSession.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(defaultSession.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, defaultSession.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x060049AC RID: 18860 RVA: 0x00188B68 File Offset: 0x00186D68
	private int GetAgeFromDateOfBirth()
	{
		DateTime today = DateTime.Today;
		int num = today.Year - this.DateOfBirth.Year;
		int num2 = today.Month - this.DateOfBirth.Month;
		if (num2 < 0)
		{
			num--;
		}
		else if (num2 == 0 && today.Day - this.DateOfBirth.Day < 0)
		{
			num--;
		}
		return num;
	}

	// Token: 0x060049AD RID: 18861 RVA: 0x00188BCC File Offset: 0x00186DCC
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("New TMPSession]:");
		stringBuilder.AppendLine(string.Format("    - Is Default    :   {0}", this.IsDefault));
		stringBuilder.AppendLine(string.Format("    - Is Valid      :   {0}", this.IsValidSession));
		stringBuilder.AppendLine(string.Format("    - SessionID     :   {0}", this.SessionId));
		stringBuilder.AppendLine(string.Format("    - Age           :   {0}", this.Age));
		stringBuilder.AppendLine(string.Format("    - AgeStatus     :   {0}", this.AgeStatus));
		stringBuilder.AppendLine(string.Format("    - SessionStatus :   {0}", this.KidStatus));
		stringBuilder.AppendLine("    - DoB           :   " + this.DateOfBirth.ToString());
		stringBuilder.AppendLine("    - KUID          :   " + this.KUID);
		stringBuilder.AppendLine("    - Jurisdiction  :   " + this.Jurisdiction);
		stringBuilder.AppendLine("    - PERMISSIONS   :");
		if (this.Permissions != null)
		{
			foreach (Permission permission in this.Permissions.Values)
			{
				stringBuilder.AppendLine(string.Format("        - {0} - Enabled: {1} - ManagedBy: {2}", permission.Name, permission.Enabled, permission.ManagedBy));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04005BFE RID: 23550
	public readonly Guid SessionId;

	// Token: 0x04005BFF RID: 23551
	public readonly string Etag;

	// Token: 0x04005C00 RID: 23552
	public readonly AgeStatusType AgeStatus;

	// Token: 0x04005C01 RID: 23553
	public readonly Session.StatusEnum KidStatus;

	// Token: 0x04005C02 RID: 23554
	public readonly Session.ManagedByEnum ManagedBy;

	// Token: 0x04005C03 RID: 23555
	public readonly DateTime DateOfBirth;

	// Token: 0x04005C04 RID: 23556
	public readonly string Jurisdiction;

	// Token: 0x04005C05 RID: 23557
	public readonly string KUID;

	// Token: 0x04005C06 RID: 23558
	public readonly int Age;

	// Token: 0x04005C07 RID: 23559
	public readonly bool IsDefault;

	// Token: 0x04005C08 RID: 23560
	public readonly SessionStatus SessionStatus;

	// Token: 0x04005C09 RID: 23561
	private Dictionary<EKIDFeatures, Permission> Permissions;

	// Token: 0x04005C0A RID: 23562
	private HashSet<EKIDFeatures> OptedInPermissions;
}
