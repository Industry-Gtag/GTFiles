using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x0200114A RID: 4426
	public class StoreController : MonoBehaviour
	{
		// Token: 0x06006F20 RID: 28448 RVA: 0x0023D314 File Offset: 0x0023B514
		public void Awake()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = this;
			}
			else if (StoreController.instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.CosmeticStandsDict = new Dictionary<string, DynamicCosmeticStand>();
			this.StandsByPlayfabID = new Dictionary<string, List<DynamicCosmeticStand>>();
		}

		// Token: 0x06006F21 RID: 28449 RVA: 0x0023D36C File Offset: 0x0023B56C
		public void RefreshCosmeticStandsDictionaryFromDepartments()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				if (!(storeDepartment == null) && !storeDepartment.departmentName.IsNullOrEmpty())
				{
					foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
					{
						if (!storeDisplay.displayName.IsNullOrEmpty())
						{
							foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
							{
								if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
								{
									string text = string.Concat(new string[] { storeDepartment.departmentName, "|", storeDisplay.displayName, "|", dynamicCosmeticStand.StandName });
									if (this.CosmeticStandsDict.ContainsKey(text))
									{
										Debug.LogError(string.Concat(new string[]
										{
											"StoreStuff: Duplicate Stand Name: ",
											text,
											" Please Fix Gameobject : ",
											dynamicCosmeticStand.gameObject.GetPath(),
											dynamicCosmeticStand.gameObject.name
										}), base.gameObject);
									}
									else
									{
										this.CosmeticStandsDict.Add(text, dynamicCosmeticStand);
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06006F22 RID: 28450 RVA: 0x0023D4F4 File Offset: 0x0023B6F4
		public void AddStandToCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: Duplicate Stand Name: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Add(text, stand);
		}

		// Token: 0x06006F23 RID: 28451 RVA: 0x0023D5FC File Offset: 0x0023B7FC
		public void RemoveStandFromDynamicCosmeticStandsDictionary(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(text))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"StoreStuff: StoreController doesn't have stand in its dict. that's weird!: ",
					text,
					" Please Fix Gameobject : ",
					stand.gameObject.GetPath(),
					stand.gameObject.name
				}), base.gameObject);
				return;
			}
			this.CosmeticStandsDict.Remove(text);
		}

		// Token: 0x06006F24 RID: 28452 RVA: 0x0023D704 File Offset: 0x0023B904
		private void Create_StandsByPlayfabIDDictionary()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				this.AddStandToPlayfabIDDictionary(dynamicCosmeticStand);
			}
		}

		// Token: 0x06006F25 RID: 28453 RVA: 0x0023D75C File Offset: 0x0023B95C
		public void AddStandToPlayfabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			if (!dynamicCosmeticStand.StandName.IsNullOrEmpty())
			{
				if (dynamicCosmeticStand.thisCosmeticName.IsNullOrEmpty())
				{
					return;
				}
				if (this.StandsByPlayfabID.ContainsKey(dynamicCosmeticStand.thisCosmeticName))
				{
					this.StandsByPlayfabID[dynamicCosmeticStand.thisCosmeticName].Add(dynamicCosmeticStand);
					return;
				}
				this.StandsByPlayfabID.Add(dynamicCosmeticStand.thisCosmeticName, new List<DynamicCosmeticStand> { dynamicCosmeticStand });
			}
		}

		// Token: 0x06006F26 RID: 28454 RVA: 0x0023D7CC File Offset: 0x0023B9CC
		public void RemoveStandFromPlayFabIDDictionary(DynamicCosmeticStand dynamicCosmeticStand)
		{
			List<DynamicCosmeticStand> list;
			if (this.StandsByPlayfabID.TryGetValue(dynamicCosmeticStand.thisCosmeticName, out list))
			{
				list.Remove(dynamicCosmeticStand);
			}
		}

		// Token: 0x06006F27 RID: 28455 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void ExportCosmeticStandLayoutWithItems()
		{
		}

		// Token: 0x06006F28 RID: 28456 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void ExportCosmeticStandLayoutWITHOUTItems()
		{
		}

		// Token: 0x06006F29 RID: 28457 RVA: 0x00002C2D File Offset: 0x00000E2D
		public void ImportCosmeticStandLayout()
		{
		}

		// Token: 0x06006F2A RID: 28458 RVA: 0x0023D7F6 File Offset: 0x0023B9F6
		private void InitializeFromTitleData()
		{
			PlayFabTitleDataCache.Instance.GetTitleData("StoreLayoutData", delegate(string data)
			{
				this.ImportCosmeticStandLayoutFromTitleData(data);
			}, delegate(PlayFabError e)
			{
				Debug.LogError(string.Format("Error getting StoreLayoutData data: {0}", e));
			}, false);
		}

		// Token: 0x06006F2B RID: 28459 RVA: 0x0023D834 File Offset: 0x0023BA34
		private void ImportCosmeticStandLayoutFromTitleData(string TSVData)
		{
			this.standImport = new StandImport();
			this.standImport.DecomposeFromTitleDataString(TSVData);
			foreach (StandTypeData standTypeData in this.standImport.standData)
			{
				string text = string.Concat(new string[] { standTypeData.departmentID, "|", standTypeData.displayID, "|", standTypeData.standID });
				this.standImport.standKeyToDataDict.Add(text, standTypeData);
				if (this.CosmeticStandsDict.ContainsKey(text))
				{
					this.CosmeticStandsDict[text].SetStandTypeString(standTypeData.bustType);
					this.CosmeticStandsDict[text].SpawnItemOntoStand(standTypeData.playFabID);
					this.CosmeticStandsDict[text].InitializeCosmetic();
				}
			}
		}

		// Token: 0x06006F2C RID: 28460 RVA: 0x0023D938 File Offset: 0x0023BB38
		public void InitializeStandFromTitleData(DynamicCosmeticStand stand)
		{
			if (stand.parentDepartment == null || stand.parentDepartment.departmentName.IsNullOrEmpty() || stand.parentDisplay == null || stand.parentDisplay.displayName.IsNullOrEmpty() || stand.StandName.IsNullOrEmpty() || this.CosmeticStandsDict == null)
			{
				Debug.LogError("Stand " + stand.name + " is missing important setup data somehow, please fix!", stand.gameObject);
				return;
			}
			string text = string.Concat(new string[]
			{
				stand.parentDepartment.departmentName,
				"|",
				stand.parentDisplay.displayName,
				"|",
				stand.StandName
			});
			if (!this.CosmeticStandsDict.ContainsKey(text) || !this.standImport.standKeyToDataDict.ContainsKey(text))
			{
				return;
			}
			StandTypeData standTypeData = this.standImport.standKeyToDataDict[text];
			this.CosmeticStandsDict[text].SetStandTypeString(standTypeData.bustType);
			this.CosmeticStandsDict[text].SpawnItemOntoStand(standTypeData.playFabID);
			this.CosmeticStandsDict[text].InitializeCosmetic();
		}

		// Token: 0x06006F2D RID: 28461 RVA: 0x0023DA6F File Offset: 0x0023BC6F
		public void InitalizeCosmeticStands()
		{
			this.cosmeticsInitialized = true;
			this.RefreshCosmeticStandsDictionaryFromDepartments();
			if (this.LoadFromTitleData)
			{
				this.InitializeFromTitleData();
			}
		}

		// Token: 0x06006F2E RID: 28462 RVA: 0x0023DA8C File Offset: 0x0023BC8C
		public void LoadCosmeticOntoStand(string standID, string playFabId)
		{
			if (this.CosmeticStandsDict.ContainsKey(standID))
			{
				this.CosmeticStandsDict[standID].SpawnItemOntoStand(playFabId);
				Debug.Log("StoreStuff: Cosmetic Loaded Onto Stand: " + standID + " | " + playFabId);
			}
		}

		// Token: 0x06006F2F RID: 28463 RVA: 0x0023DAC4 File Offset: 0x0023BCC4
		public void ClearCosmetics()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				StoreDisplay[] displays = storeDepartment.Displays;
				for (int i = 0; i < displays.Length; i++)
				{
					DynamicCosmeticStand[] stands = displays[i].Stands;
					for (int j = 0; j < stands.Length; j++)
					{
						stands[j].ClearCosmetics();
					}
				}
			}
		}

		// Token: 0x06006F30 RID: 28464 RVA: 0x0023DB48 File Offset: 0x0023BD48
		public static CosmeticSO FindCosmeticInAllCosmeticsArraySO(string playfabId)
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			return StoreController.instance.AllCosmeticsArraySO.SearchForCosmeticSO(playfabId);
		}

		// Token: 0x06006F31 RID: 28465 RVA: 0x0023DB78 File Offset: 0x0023BD78
		public DynamicCosmeticStand FindCosmeticStandByCosmeticName(string PlayFabID)
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in this.CosmeticStandsDict.Values)
			{
				if (dynamicCosmeticStand.thisCosmeticName == PlayFabID)
				{
					return dynamicCosmeticStand;
				}
			}
			return null;
		}

		// Token: 0x06006F32 RID: 28466 RVA: 0x0023DBE0 File Offset: 0x0023BDE0
		public void FindAllDepartments()
		{
			this.Departments = Object.FindObjectsByType<StoreDepartment>(FindObjectsSortMode.None).ToList<StoreDepartment>();
		}

		// Token: 0x06006F33 RID: 28467 RVA: 0x0023DBF4 File Offset: 0x0023BDF4
		public void SaveAllCosmeticsPositions()
		{
			foreach (StoreDepartment storeDepartment in this.Departments)
			{
				foreach (StoreDisplay storeDisplay in storeDepartment.Displays)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand in storeDisplay.Stands)
					{
						Debug.Log(string.Concat(new string[]
						{
							"StoreStuff: Saving Items mount transform: ",
							storeDepartment.departmentName,
							"|",
							storeDisplay.displayName,
							"|",
							dynamicCosmeticStand.StandName,
							"|",
							dynamicCosmeticStand.DisplayHeadModel.bustType.ToString(),
							"|",
							dynamicCosmeticStand.thisCosmeticName
						}));
						dynamicCosmeticStand.UpdateCosmeticsMountPositions();
					}
				}
			}
		}

		// Token: 0x06006F34 RID: 28468 RVA: 0x0023DD14 File Offset: 0x0023BF14
		public static void SetForGame()
		{
			if (StoreController.instance == null)
			{
				StoreController.instance = Object.FindAnyObjectByType<StoreController>();
			}
			StoreController.instance.RefreshCosmeticStandsDictionaryFromDepartments();
			foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.CosmeticStandsDict.Values)
			{
				dynamicCosmeticStand.SetStandType(dynamicCosmeticStand.DisplayHeadModel.bustType);
				dynamicCosmeticStand.SpawnItemOntoStand(dynamicCosmeticStand.thisCosmeticName);
			}
		}

		// Token: 0x04007F2A RID: 32554
		[OnEnterPlay_Clear]
		public static volatile StoreController instance;

		// Token: 0x04007F2B RID: 32555
		public List<StoreDepartment> Departments;

		// Token: 0x04007F2C RID: 32556
		private Dictionary<string, DynamicCosmeticStand> CosmeticStandsDict;

		// Token: 0x04007F2D RID: 32557
		public Dictionary<string, List<DynamicCosmeticStand>> StandsByPlayfabID;

		// Token: 0x04007F2E RID: 32558
		public AllCosmeticsArraySO AllCosmeticsArraySO;

		// Token: 0x04007F2F RID: 32559
		public bool cosmeticsInitialized;

		// Token: 0x04007F30 RID: 32560
		public bool LoadFromTitleData;

		// Token: 0x04007F31 RID: 32561
		private string exportHeader = "Department ID\tDisplay ID\tStand ID\tStand Type\tPlayFab ID";

		// Token: 0x04007F32 RID: 32562
		private StandImport standImport;
	}
}
