using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Newtonsoft.Json;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000086 RID: 134
public class Menagerie : MonoBehaviour
{
	// Token: 0x0600033E RID: 830 RVA: 0x00013900 File Offset: 0x00011B00
	private void Start()
	{
		CrittersCageDeposit[] array = Object.FindObjectsByType<CrittersCageDeposit>(FindObjectsInactive.Include, FindObjectsSortMode.None);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnDepositCritter += this.OnDepositCritter;
		}
		CrittersManager.CheckInitialize();
		this._totalPages = this.critterIndex.critterTypes.Count / this.collection.Length + ((this.critterIndex.critterTypes.Count % this.collection.Length == 0) ? 0 : 1);
		this.Load();
		MenagerieDepositBox donationBox = this.DonationBox;
		donationBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(donationBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInDonationBox));
		MenagerieDepositBox favoriteBox = this.FavoriteBox;
		favoriteBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(favoriteBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInFavoriteBox));
		MenagerieDepositBox collectionBox = this.CollectionBox;
		collectionBox.OnCritterInserted = (Action<MenagerieCritter>)Delegate.Combine(collectionBox.OnCritterInserted, new Action<MenagerieCritter>(this.CritterDepositedInCollectionBox));
	}

	// Token: 0x0600033F RID: 831 RVA: 0x000139F8 File Offset: 0x00011BF8
	private void CritterDepositedInDonationBox(MenagerieCritter critter)
	{
		if (this.newCritterPen.Contains(critter.Slot))
		{
			critter.currentState = MenagerieCritter.MenagerieCritterState.Donating;
			this.DonateCritter(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			PlayerGameEvents.CritterEvent("Donate" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x00013A80 File Offset: 0x00011C80
	private void CritterDepositedInFavoriteBox(MenagerieCritter critter)
	{
		if (this.collection.Contains(critter.Slot))
		{
			this._savedCritters.favoriteCritter = critter.CritterData.critterType;
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Favorite" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x06000341 RID: 833 RVA: 0x00013AEC File Offset: 0x00011CEC
	private void CritterDepositedInCollectionBox(MenagerieCritter critter)
	{
		if (this.newCritterPen.Contains(critter.Slot))
		{
			this.AddCritterToCollection(critter.CritterData);
			this._savedCritters.newCritters.Remove(critter.CritterData);
			this.DespawnCritterFromSlot(critter.Slot);
			this.Save();
			this.UpdateFavoriteCritter();
			PlayerGameEvents.CritterEvent("Collect" + this.critterIndex[critter.CritterData.critterType].critterName);
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x00013B74 File Offset: 0x00011D74
	private void OnDepositCritter(Menagerie.CritterData depositedCritter, int playerID)
	{
		try
		{
			if (playerID == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.AddCritterToNewCritterPen(depositedCritter);
				this.Save();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00013BB4 File Offset: 0x00011DB4
	private void AddCritterToNewCritterPen(Menagerie.CritterData critterData)
	{
		if (this._savedCritters.newCritters.Count < this.newCritterPen.Length)
		{
			foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
			{
				if (!menagerieSlot.critter)
				{
					this.SpawnCritterInSlot(menagerieSlot, critterData);
					this._savedCritters.newCritters.Add(critterData);
					return;
				}
			}
		}
		this.DonateCritter(critterData);
		this.Save();
	}

	// Token: 0x06000344 RID: 836 RVA: 0x00013C28 File Offset: 0x00011E28
	private void AddCritterToCollection(Menagerie.CritterData critterData)
	{
		Menagerie.CritterData critterData2;
		if (this._savedCritters.collectedCritters.TryGetValue(critterData.critterType, out critterData2))
		{
			this.DonateCritter(critterData2);
		}
		this._savedCritters.collectedCritters[critterData.critterType] = critterData;
		this.SpawnCollectionCritterIfShowing(critterData);
	}

	// Token: 0x06000345 RID: 837 RVA: 0x00013C74 File Offset: 0x00011E74
	private void DonateCritter(Menagerie.CritterData critterData)
	{
		this._savedCritters.donatedCritterCount++;
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount));
	}

	// Token: 0x06000346 RID: 838 RVA: 0x00013CB0 File Offset: 0x00011EB0
	private void SpawnCritterInSlot(MenagerieSlot slot, Menagerie.CritterData critterData)
	{
		if (slot.IsNull() || critterData == null)
		{
			return;
		}
		this.DespawnCritterFromSlot(slot);
		MenagerieCritter menagerieCritter = Object.Instantiate<MenagerieCritter>(this.prefab, slot.critterMountPoint);
		menagerieCritter.Slot = slot;
		menagerieCritter.ApplyCritterData(critterData);
		this._critters.Add(menagerieCritter);
		if (slot.label)
		{
			slot.label.text = this.critterIndex[critterData.critterType].critterName;
		}
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00013D2C File Offset: 0x00011F2C
	private void SpawnCollectionCritterIfShowing(Menagerie.CritterData critter)
	{
		int num = critter.critterType - this._collectionPageIndex * this.collection.Length;
		if (num < 0 || num >= this.collection.Length)
		{
			return;
		}
		this.SpawnCritterInSlot(this.collection[num], critter);
	}

	// Token: 0x06000348 RID: 840 RVA: 0x00013D6F File Offset: 0x00011F6F
	private void UpdateMenagerie()
	{
		this.UpdateNewCritterPen();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
		this.donationCounter.SetText(string.Format(this.DonationText, this._savedCritters.donatedCritterCount));
	}

	// Token: 0x06000349 RID: 841 RVA: 0x00013DAC File Offset: 0x00011FAC
	private void UpdateNewCritterPen()
	{
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			if (i < this._savedCritters.newCritters.Count)
			{
				this.SpawnCritterInSlot(this.newCritterPen[i], this._savedCritters.newCritters[i]);
			}
			else
			{
				this.DespawnCritterFromSlot(this.newCritterPen[i]);
			}
		}
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00013E10 File Offset: 0x00012010
	private void UpdateCollection()
	{
		int num = this._collectionPageIndex * this.collection.Length;
		for (int i = 0; i < this.collection.Length; i++)
		{
			int num2 = num + i;
			MenagerieSlot menagerieSlot = this.collection[i];
			Menagerie.CritterData critterData;
			if (this._savedCritters.collectedCritters.TryGetValue(num2, out critterData))
			{
				this.SpawnCritterInSlot(menagerieSlot, critterData);
			}
			else
			{
				this.DespawnCritterFromSlot(menagerieSlot);
				CritterConfiguration critterConfiguration = this.critterIndex[num2];
				menagerieSlot.label.text = ((critterConfiguration == null) ? "" : "??????");
			}
		}
	}

	// Token: 0x0600034B RID: 843 RVA: 0x00013EA0 File Offset: 0x000120A0
	private void UpdateFavoriteCritter()
	{
		Menagerie.CritterData critterData;
		if (this._savedCritters.collectedCritters.TryGetValue(this._savedCritters.favoriteCritter, out critterData))
		{
			this.SpawnCritterInSlot(this.favoriteCritterSlot, critterData);
			return;
		}
		this.ClearSlot(this.favoriteCritterSlot);
	}

	// Token: 0x0600034C RID: 844 RVA: 0x00013EE6 File Offset: 0x000120E6
	public void NextGroupCollectedCritters()
	{
		this._collectionPageIndex++;
		if (this._collectionPageIndex >= this._totalPages)
		{
			this._collectionPageIndex = 0;
		}
		this.UpdateCollection();
	}

	// Token: 0x0600034D RID: 845 RVA: 0x00013F11 File Offset: 0x00012111
	public void PrevGroupCollectedCritters()
	{
		this._collectionPageIndex--;
		if (this._collectionPageIndex < 0)
		{
			this._collectionPageIndex = this._totalPages - 1;
		}
		this.UpdateCollection();
	}

	// Token: 0x0600034E RID: 846 RVA: 0x00013F3E File Offset: 0x0001213E
	private void GenerateNewCritters()
	{
		this.GenerateNewCritterCount(Random.Range(Mathf.Min(1, this.newCritterPen.Length), this.newCritterPen.Length + 1));
	}

	// Token: 0x0600034F RID: 847 RVA: 0x00013F64 File Offset: 0x00012164
	private void GenerateLegalNewCritters()
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < this.newCritterPen.Length; i++)
		{
			int randomCritterType = this.critterIndex.GetRandomCritterType(null);
			if (randomCritterType < 0)
			{
				Debug.LogError("Failed to spawn valid critter. No critter configuration found.");
				return;
			}
			Menagerie.CritterData critterData = new Menagerie.CritterData(randomCritterType, this.critterIndex[randomCritterType].GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x06000350 RID: 848 RVA: 0x00013FC8 File Offset: 0x000121C8
	private void GenerateNewCritterCount(int critterCount)
	{
		this.ClearNewCritterPen();
		for (int i = 0; i < critterCount; i++)
		{
			int num = Random.Range(0, this.critterIndex.critterTypes.Count);
			CritterConfiguration critterConfiguration = this.critterIndex[num];
			Menagerie.CritterData critterData = new Menagerie.CritterData(num, critterConfiguration.GenerateAppearance());
			this.AddCritterToNewCritterPen(critterData);
		}
	}

	// Token: 0x06000351 RID: 849 RVA: 0x00014020 File Offset: 0x00012220
	private void GenerateCollectedCritters(float spawnChance)
	{
		this.ClearCollection();
		for (int i = 0; i < this.critterIndex.critterTypes.Count; i++)
		{
			if (Random.value <= spawnChance)
			{
				CritterConfiguration critterConfiguration = this.critterIndex[i];
				Menagerie.CritterData critterData = new Menagerie.CritterData(i, critterConfiguration.GenerateAppearance());
				this.AddCritterToCollection(critterData);
				critterData.instance;
			}
		}
	}

	// Token: 0x06000352 RID: 850 RVA: 0x00014084 File Offset: 0x00012284
	private void MoveNewCrittersToCollection()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInCollectionBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x06000353 RID: 851 RVA: 0x000140C4 File Offset: 0x000122C4
	private void DonateNewCritters()
	{
		foreach (MenagerieSlot menagerieSlot in this.newCritterPen)
		{
			if (menagerieSlot.critter)
			{
				this.CritterDepositedInDonationBox(menagerieSlot.critter);
			}
		}
	}

	// Token: 0x06000354 RID: 852 RVA: 0x00014103 File Offset: 0x00012303
	private void ClearSlot(MenagerieSlot slot)
	{
		this.DespawnCritterFromSlot(slot);
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x06000355 RID: 853 RVA: 0x0001412C File Offset: 0x0001232C
	private void DespawnCritterFromSlot(MenagerieSlot slot)
	{
		if (slot.IsNull())
		{
			return;
		}
		if (!slot.critter)
		{
			return;
		}
		this._critters.Remove(slot.critter);
		Object.Destroy(slot.critter.gameObject);
		slot.critter = null;
		if (slot.label)
		{
			slot.label.text = "";
		}
	}

	// Token: 0x06000356 RID: 854 RVA: 0x00014196 File Offset: 0x00012396
	private void ClearNewCritterPen()
	{
		this._savedCritters.newCritters.Clear();
		this.UpdateNewCritterPen();
	}

	// Token: 0x06000357 RID: 855 RVA: 0x000141AE File Offset: 0x000123AE
	private void ClearCollection()
	{
		this._savedCritters.collectedCritters.Clear();
		this.UpdateCollection();
		this.UpdateFavoriteCritter();
	}

	// Token: 0x06000358 RID: 856 RVA: 0x000141CC File Offset: 0x000123CC
	private void ClearAll()
	{
		this._savedCritters.Clear();
		this.UpdateMenagerie();
	}

	// Token: 0x06000359 RID: 857 RVA: 0x000141DF File Offset: 0x000123DF
	private void ResetSavedCreatures()
	{
		this.ClearAll();
		this.Save();
	}

	// Token: 0x0600035A RID: 858 RVA: 0x000141F0 File Offset: 0x000123F0
	private void Load()
	{
		this.ClearAll();
		string @string = PlayerPrefs.GetString("_SavedCritters", string.Empty);
		this.LoadCrittersFromJson(@string);
		this.UpdateMenagerie();
	}

	// Token: 0x0600035B RID: 859 RVA: 0x00014220 File Offset: 0x00012420
	private void Save()
	{
		Debug.Log(string.Format("Saving {0} critters", this._critters.Count));
		string text = this.SaveCrittersToJson();
		PlayerPrefs.SetString("_SavedCritters", text);
	}

	// Token: 0x0600035C RID: 860 RVA: 0x00014260 File Offset: 0x00012460
	private void LoadCrittersFromJson(string jsonString)
	{
		this._savedCritters.Clear();
		if (!string.IsNullOrEmpty(jsonString))
		{
			try
			{
				this._savedCritters = JsonConvert.DeserializeObject<Menagerie.CritterSaveData>(jsonString);
			}
			catch (Exception ex)
			{
				Debug.LogError("Unable to deserialize critters from json: " + jsonString);
				Debug.LogException(ex);
			}
		}
		this.ValidateSaveData();
	}

	// Token: 0x0600035D RID: 861 RVA: 0x000142BC File Offset: 0x000124BC
	private string SaveCrittersToJson()
	{
		this.ValidateSaveData();
		string text = JsonConvert.SerializeObject(this._savedCritters, Formatting.Indented);
		Debug.Log("Critters save to JSON: " + text);
		return text;
	}

	// Token: 0x0600035E RID: 862 RVA: 0x000142F0 File Offset: 0x000124F0
	private void ValidateSaveData()
	{
		if (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
		{
			Debug.LogError(string.Format("Too many new critters in CrittersSaveData ({0} vs {1}) - correcting.", this._savedCritters.newCritters.Count, this.newCritterPen.Length));
			while (this._savedCritters.newCritters.Count > this.newCritterPen.Length)
			{
				this._savedCritters.newCritters.RemoveAt(this.newCritterPen.Length);
			}
			this.Save();
		}
	}

	// Token: 0x0600035F RID: 863 RVA: 0x00014384 File Offset: 0x00012584
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		MenagerieSlot[] array = this.newCritterPen;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		array = this.collection;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawWireSphere(array[i].critterMountPoint.position, 0.1f);
		}
		Gizmos.DrawWireSphere(this.favoriteCritterSlot.critterMountPoint.position, 0.1f);
	}

	// Token: 0x040003DC RID: 988
	[FormerlySerializedAs("creatureIndex")]
	public CritterIndex critterIndex;

	// Token: 0x040003DD RID: 989
	public MenagerieCritter prefab;

	// Token: 0x040003DE RID: 990
	private List<MenagerieCritter> _critters = new List<MenagerieCritter>();

	// Token: 0x040003DF RID: 991
	private Menagerie.CritterSaveData _savedCritters = new Menagerie.CritterSaveData();

	// Token: 0x040003E0 RID: 992
	public MenagerieSlot[] collection;

	// Token: 0x040003E1 RID: 993
	public MenagerieSlot[] newCritterPen;

	// Token: 0x040003E2 RID: 994
	public MenagerieSlot favoriteCritterSlot;

	// Token: 0x040003E3 RID: 995
	private int _collectionPageIndex;

	// Token: 0x040003E4 RID: 996
	private int _totalPages;

	// Token: 0x040003E5 RID: 997
	public MenagerieDepositBox DonationBox;

	// Token: 0x040003E6 RID: 998
	public MenagerieDepositBox FavoriteBox;

	// Token: 0x040003E7 RID: 999
	public MenagerieDepositBox CollectionBox;

	// Token: 0x040003E8 RID: 1000
	public TextMeshPro donationCounter;

	// Token: 0x040003E9 RID: 1001
	public string DonationText = "DONATED:{0}";

	// Token: 0x040003EA RID: 1002
	private const string CrittersSavePrefsKey = "_SavedCritters";

	// Token: 0x02000087 RID: 135
	public class CritterData
	{
		// Token: 0x06000361 RID: 865 RVA: 0x00014436 File Offset: 0x00012636
		public CritterConfiguration GetConfiguration()
		{
			return CrittersManager.instance.creatureIndex[this.critterType];
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00002050 File Offset: 0x00000250
		public CritterData()
		{
		}

		// Token: 0x06000363 RID: 867 RVA: 0x0001444F File Offset: 0x0001264F
		public CritterData(CritterConfiguration config, CritterAppearance appearance)
		{
			this.critterType = CrittersManager.instance.creatureIndex.critterTypes.IndexOf(config);
			this.appearance = appearance;
		}

		// Token: 0x06000364 RID: 868 RVA: 0x0001447B File Offset: 0x0001267B
		public CritterData(int critterType, CritterAppearance appearance)
		{
			this.critterType = critterType;
			this.appearance = appearance;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00014491 File Offset: 0x00012691
		public CritterData(CritterVisuals visuals)
		{
			this.critterType = visuals.critterType;
			this.appearance = visuals.Appearance;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x000144B1 File Offset: 0x000126B1
		public CritterData(Menagerie.CritterData source)
		{
			this.critterType = source.critterType;
			this.appearance = source.appearance;
		}

		// Token: 0x06000367 RID: 871 RVA: 0x000144D1 File Offset: 0x000126D1
		public override string ToString()
		{
			return string.Format("{0} {1} [instance]", this.critterType, this.appearance);
		}

		// Token: 0x040003EB RID: 1003
		public int critterType;

		// Token: 0x040003EC RID: 1004
		public CritterAppearance appearance;

		// Token: 0x040003ED RID: 1005
		[NonSerialized]
		public MenagerieCritter instance;
	}

	// Token: 0x02000088 RID: 136
	[Serializable]
	public class CritterSaveData
	{
		// Token: 0x06000368 RID: 872 RVA: 0x000144F3 File Offset: 0x000126F3
		public void Clear()
		{
			this.newCritters.Clear();
			this.collectedCritters.Clear();
			this.donatedCritterCount = 0;
			this.favoriteCritter = -1;
		}

		// Token: 0x040003EE RID: 1006
		public List<Menagerie.CritterData> newCritters = new List<Menagerie.CritterData>();

		// Token: 0x040003EF RID: 1007
		public Dictionary<int, Menagerie.CritterData> collectedCritters = new Dictionary<int, Menagerie.CritterData>();

		// Token: 0x040003F0 RID: 1008
		public int donatedCritterCount;

		// Token: 0x040003F1 RID: 1009
		public int favoriteCritter = -1;
	}
}
