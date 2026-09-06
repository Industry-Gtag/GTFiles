using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts.ScavengerHunt
{
	// Token: 0x02001002 RID: 4098
	[NullableContext(1)]
	[Nullable(0)]
	public class ScavengerManager : MonoBehaviour
	{
		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x06006609 RID: 26121 RVA: 0x0020D674 File Offset: 0x0020B874
		// (set) Token: 0x0600660A RID: 26122 RVA: 0x0020D67B File Offset: 0x0020B87B
		[Nullable(2)]
		public static ScavengerManager Instance
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			private set;
		}

		// Token: 0x0600660B RID: 26123 RVA: 0x0020D683 File Offset: 0x0020B883
		private void Awake()
		{
			if (ScavengerManager.Instance == null)
			{
				ScavengerManager.Instance = this;
				return;
			}
			throw new Exception("Two ScavengerManagers exist at once, this should never happen.");
		}

		// Token: 0x0600660C RID: 26124 RVA: 0x0020D6A3 File Offset: 0x0020B8A3
		private void Start()
		{
			base.StartCoroutine(this.ImportMothershipUserData());
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x0020D6B2 File Offset: 0x0020B8B2
		private IEnumerator ImportMothershipUserData()
		{
			while (!MothershipClientContext.IsClientLoggedIn())
			{
				PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
				if (instance != null && instance.loginFailed)
				{
					Debug.LogError("ScavengerManager critical error, could not log into Mothership.");
					yield break;
				}
				yield return new WaitForSecondsRealtime(0.5f);
			}
			MothershipClientApiUnity.GetUserDataValue("ScavengerHunt", new Action<MothershipUserData>(this.OnGetUserDataSuccess), new Action<MothershipError, int>(this.OnGetUserDataFailure), "");
			yield break;
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x0020D6C4 File Offset: 0x0020B8C4
		private void OnGetUserDataSuccess(MothershipUserData data)
		{
			Debug.Log("Successfully read scavenger hunt data from Mothership.");
			byte[] array = Convert.FromBase64String(data.value);
			string @string = Encoding.UTF8.GetString(array);
			this.FromJson(@string);
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x0020D6FA File Offset: 0x0020B8FA
		private void OnGetUserDataFailure(MothershipError error, int responseCode)
		{
			Debug.LogError(string.Format("Failed to read scavenger hunt user data (error {0} / {1}): {2}", error.Name, responseCode, error.Message));
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x0020D71D File Offset: 0x0020B91D
		private void OnDestroy()
		{
			ScavengerManager.Instance = null;
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x0020D728 File Offset: 0x0020B928
		[return: Nullable(2)]
		public ScavengerManager.Hunt GetHunt(string huntName)
		{
			foreach (ScavengerManager.Hunt hunt in this.Hunts)
			{
				if (hunt.Name == huntName)
				{
					return hunt;
				}
			}
			return null;
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x0020D760 File Offset: 0x0020B960
		public void RegisterTarget(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception("No hunt found with name " + target.HuntName + ".");
			}
			if (!hunt.Targets.Contains(target))
			{
				hunt.RegisterTarget(target);
			}
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x0020D7B0 File Offset: 0x0020B9B0
		public bool IsCollected(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			return hunt != null && hunt.IsCollected(target);
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x0020D7D8 File Offset: 0x0020B9D8
		public void Collect(ScavengerTarget target)
		{
			ScavengerManager.Hunt hunt = this.GetHunt(target.HuntName);
			if (hunt == null)
			{
				throw new Exception(string.Concat(new string[] { "Cannot collect scavenger hunt ", target.TargetName, ", hunt ", target.HuntName, " does not exist." }));
			}
			if (!hunt.Collect(target, false))
			{
				Debug.Log("Did not collect scavenger hunt " + target.TargetName + ". This is normally because the user already collected it.");
				return;
			}
			Debug.Log("Collected " + target.HuntName + "." + target.TargetName);
			string text = this.ToJson().Write();
			MothershipClientApiUnity.SetUserDataValue("ScavengerHunt", text, new Action<SetUserDataResponse>(this.OnSetUserDataSuccess), new Action<MothershipError, int>(this.OnSetUserDataFailure), "");
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x0020D8AA File Offset: 0x0020BAAA
		private void OnSetUserDataSuccess(SetUserDataResponse response)
		{
			Debug.Log("Successfully wrote scavenger hunt data for user " + response.user_id + " on Mothership key " + response.key_name);
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x0020D8CC File Offset: 0x0020BACC
		private void OnSetUserDataFailure(MothershipError error, int statusCode)
		{
			Debug.LogError(string.Format("Failed to write scavenger hunt data to Mothership (error {0} / {1}): {2}", error.Name, statusCode, error.Message));
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x0020D8EF File Offset: 0x0020BAEF
		public ScavengerManager.ScavengerJson ToJson()
		{
			return ScavengerManager.ScavengerJson.FromManager(this);
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x0020D8F7 File Offset: 0x0020BAF7
		public void FromJson(string json)
		{
			this.FromJson(ScavengerManager.ScavengerJson.FromJson(json));
		}

		// Token: 0x06006619 RID: 26137 RVA: 0x0020D908 File Offset: 0x0020BB08
		public void FromJson(ScavengerManager.ScavengerJson json)
		{
			this._collectOnLoad.Clear();
			ScavengerManager.Hunt[] hunts = this.Hunts;
			for (int i = 0; i < hunts.Length; i++)
			{
				hunts[i].ClearCollectedTargets();
			}
			foreach (KeyValuePair<string, string[]> keyValuePair in json.CollectedTargets)
			{
				ScavengerManager.Hunt hunt = this.GetHunt(keyValuePair.Key);
				if (hunt == null)
				{
					throw new Exception("Cannot import scavenger data, no hunt by name " + keyValuePair.Key + ".");
				}
				if (!hunt.Deprecated)
				{
					foreach (string text in keyValuePair.Value)
					{
						ScavengerTarget target = hunt.GetTarget(text);
						if (target != null)
						{
							hunt.Collect(target, true);
						}
						else
						{
							this._collectOnLoad.Add(new Tuple<string, string>(keyValuePair.Key, text));
						}
					}
				}
			}
		}

		// Token: 0x04007515 RID: 29973
		public static Action<string, bool> OnHuntCompleted;

		// Token: 0x04007516 RID: 29974
		public static Action<string, string, bool> OnTargetCollected;

		// Token: 0x04007517 RID: 29975
		public const string MothershipKey = "ScavengerHunt";

		// Token: 0x04007519 RID: 29977
		private List<Tuple<string, string>> _collectOnLoad = new List<Tuple<string, string>>();

		// Token: 0x0400751A RID: 29978
		public ScavengerManager.Hunt[] Hunts = new ScavengerManager.Hunt[0];

		// Token: 0x02001003 RID: 4099
		[Nullable(0)]
		[Serializable]
		public class Hunt
		{
			// Token: 0x170009C3 RID: 2499
			// (get) Token: 0x0600661B RID: 26139 RVA: 0x0020DA33 File Offset: 0x0020BC33
			public bool IsCompleted
			{
				get
				{
					return this.Targets.Count > 0 && this.Targets.Count == this.CollectedTargetNames.Count;
				}
			}

			// Token: 0x170009C4 RID: 2500
			// (get) Token: 0x0600661C RID: 26140 RVA: 0x0020DA60 File Offset: 0x0020BC60
			public IReadOnlyList<ScavengerTarget> Targets
			{
				get
				{
					List<ScavengerTarget> list;
					if ((list = this._targets) == null)
					{
						list = (this._targets = new List<ScavengerTarget>());
					}
					return list;
				}
			}

			// Token: 0x170009C5 RID: 2501
			// (get) Token: 0x0600661D RID: 26141 RVA: 0x0020DA88 File Offset: 0x0020BC88
			public IReadOnlyCollection<string> CollectedTargetNames
			{
				get
				{
					HashSet<string> hashSet;
					if ((hashSet = this._collectedTargetNamesNullable) == null)
					{
						hashSet = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return hashSet;
				}
			}

			// Token: 0x170009C6 RID: 2502
			// (get) Token: 0x0600661E RID: 26142 RVA: 0x0020DAB0 File Offset: 0x0020BCB0
			private HashSet<string> _collectedTargetNames
			{
				get
				{
					HashSet<string> hashSet;
					if ((hashSet = this._collectedTargetNamesNullable) == null)
					{
						hashSet = (this._collectedTargetNamesNullable = new HashSet<string>());
					}
					return hashSet;
				}
			}

			// Token: 0x0600661F RID: 26143 RVA: 0x0020DAD8 File Offset: 0x0020BCD8
			public Hunt(string name)
			{
				this.Name = name;
			}

			// Token: 0x06006620 RID: 26144 RVA: 0x0020DB30 File Offset: 0x0020BD30
			public bool Collect(ScavengerTarget target, bool initialLoad = false)
			{
				if (!this.Targets.Contains(target))
				{
					return false;
				}
				if (this._collectedTargetNames.Add(target.TargetName))
				{
					if (!initialLoad || this.SendTargetCollectedEventsOnLoad)
					{
						this.SendTargetCollectedEvents(target, initialLoad);
					}
					if (this.IsCompleted && (!initialLoad || this.SendHuntCompletedEventsOnLoad))
					{
						this.SendHuntCompletedEvents(initialLoad);
					}
					return true;
				}
				return false;
			}

			// Token: 0x06006621 RID: 26145 RVA: 0x0020DB90 File Offset: 0x0020BD90
			public void RegisterTarget(ScavengerTarget target)
			{
				if (this.Targets.Contains(target))
				{
					return;
				}
				if (!this.TargetNames.Contains(target.TargetName))
				{
					Debug.LogError(string.Concat(new string[] { "Scavenger hunt ", this.Name, " tried to register target ", target.TargetName, " even though it is not defined in the hunt in ScavengerManager." }));
					return;
				}
				this._targets.Add(target);
				if (ScavengerManager.Instance == null)
				{
					return;
				}
				Tuple<string, string> tuple = new Tuple<string, string>(this.Name, target.TargetName);
				if (ScavengerManager.Instance._collectOnLoad.Contains(tuple))
				{
					this.Collect(target, true);
				}
			}

			// Token: 0x06006622 RID: 26146 RVA: 0x0020DC44 File Offset: 0x0020BE44
			private void SendTargetCollectedEvents(ScavengerTarget target, bool initialLoad)
			{
				if (this.Deprecated)
				{
					return;
				}
				this.TargetCollected.InvokeAll();
				this.TargetCollectedArg.InvokeAll(target);
				target.TargetCollected.InvokeAll();
				target.TargetCollectedArg.InvokeAll(target);
				Action<string, string, bool> onTargetCollected = ScavengerManager.OnTargetCollected;
				if (onTargetCollected == null)
				{
					return;
				}
				onTargetCollected(this.Name, target.name, !initialLoad);
			}

			// Token: 0x06006623 RID: 26147 RVA: 0x0020DCA7 File Offset: 0x0020BEA7
			private void SendHuntCompletedEvents(bool initialLoad)
			{
				if (this.Deprecated)
				{
					return;
				}
				this.HuntCompleted.InvokeAll();
				this.HuntCompletedArg.InvokeAll(this);
				Action<string, bool> onHuntCompleted = ScavengerManager.OnHuntCompleted;
				if (onHuntCompleted == null)
				{
					return;
				}
				onHuntCompleted(this.Name, !initialLoad);
			}

			// Token: 0x06006624 RID: 26148 RVA: 0x0020DCE2 File Offset: 0x0020BEE2
			public bool IsCollected(ScavengerTarget target)
			{
				return this._collectedTargetNames.Contains(target.TargetName);
			}

			// Token: 0x06006625 RID: 26149 RVA: 0x0020DCF5 File Offset: 0x0020BEF5
			public void ClearCollectedTargets()
			{
				this._collectedTargetNames.Clear();
			}

			// Token: 0x06006626 RID: 26150 RVA: 0x0020DD04 File Offset: 0x0020BF04
			[return: Nullable(2)]
			public ScavengerTarget GetTarget(string name)
			{
				foreach (ScavengerTarget scavengerTarget in this.Targets)
				{
					if (scavengerTarget.TargetName == name)
					{
						return scavengerTarget;
					}
				}
				return null;
			}

			// Token: 0x0400751B RID: 29979
			public string Name;

			// Token: 0x0400751C RID: 29980
			public bool SendTargetCollectedEventsOnLoad;

			// Token: 0x0400751D RID: 29981
			public bool SendHuntCompletedEventsOnLoad;

			// Token: 0x0400751E RID: 29982
			public bool Deprecated;

			// Token: 0x0400751F RID: 29983
			public string[] TargetNames = new string[0];

			// Token: 0x04007520 RID: 29984
			public UnityEvent[] TargetCollected = new UnityEvent[0];

			// Token: 0x04007521 RID: 29985
			public UnityEvent<ScavengerTarget>[] TargetCollectedArg = new UnityEvent<ScavengerTarget>[0];

			// Token: 0x04007522 RID: 29986
			public UnityEvent[] HuntCompleted = new UnityEvent[0];

			// Token: 0x04007523 RID: 29987
			public UnityEvent<ScavengerManager.Hunt>[] HuntCompletedArg = new UnityEvent<ScavengerManager.Hunt>[0];

			// Token: 0x04007524 RID: 29988
			[Nullable(new byte[] { 2, 1 })]
			private List<ScavengerTarget> _targets;

			// Token: 0x04007525 RID: 29989
			[Nullable(new byte[] { 2, 1 })]
			private HashSet<string> _collectedTargetNamesNullable;
		}

		// Token: 0x02001004 RID: 4100
		[Nullable(0)]
		public class ScavengerJson
		{
			// Token: 0x06006627 RID: 26151 RVA: 0x0020DD60 File Offset: 0x0020BF60
			public static ScavengerManager.ScavengerJson FromManager(ScavengerManager manager)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				foreach (ScavengerManager.Hunt hunt in manager.Hunts)
				{
					string[] array = hunt.CollectedTargetNames.ToArray<string>();
					scavengerJson.CollectedTargets[hunt.Name] = array;
				}
				return scavengerJson;
			}

			// Token: 0x06006628 RID: 26152 RVA: 0x0020DDB0 File Offset: 0x0020BFB0
			public static ScavengerManager.ScavengerJson FromJson(string json)
			{
				ScavengerManager.ScavengerJson scavengerJson = new ScavengerManager.ScavengerJson();
				ScavengerManager.ScavengerJson scavengerJson2;
				using (TextReader textReader = new StringReader(json))
				{
					using (JsonReader jsonReader = new JsonTextReader(textReader))
					{
						Debug.Log("Scavenger hunt parsing raw json " + json);
						while (jsonReader.Read())
						{
							if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "CollectedTargets")
							{
								ScavengerManager.ScavengerJson.ReadCollectedTargets(scavengerJson, jsonReader);
							}
						}
						scavengerJson2 = scavengerJson;
					}
				}
				return scavengerJson2;
			}

			// Token: 0x06006629 RID: 26153 RVA: 0x0020DE48 File Offset: 0x0020C048
			private static void ReadCollectedTargets(ScavengerManager.ScavengerJson json, JsonReader reader)
			{
				int num = 0;
				bool flag = false;
				string text = null;
				List<string> list = new List<string>();
				while (reader.Read())
				{
					JsonToken tokenType = reader.TokenType;
					if (tokenType <= JsonToken.String)
					{
						switch (tokenType)
						{
						case JsonToken.StartObject:
							num++;
							break;
						case JsonToken.StartArray:
							if (flag)
							{
								throw new Exception("Json read error");
							}
							flag = true;
							break;
						case JsonToken.StartConstructor:
							break;
						case JsonToken.PropertyName:
						{
							if (text != null)
							{
								throw new Exception("Json read error");
							}
							string text2 = reader.Value as string;
							if (text2 == null)
							{
								throw new Exception("Json read error");
							}
							text = text2;
							break;
						}
						default:
							if (tokenType == JsonToken.String)
							{
								if (!flag)
								{
									throw new Exception("Json read error");
								}
								string text3 = reader.Value as string;
								if (text3 == null)
								{
									throw new Exception("Json read error");
								}
								list.Add(text3);
							}
							break;
						}
					}
					else if (tokenType != JsonToken.EndObject)
					{
						if (tokenType == JsonToken.EndArray)
						{
							if (!flag)
							{
								throw new Exception("Json read error");
							}
							if (string.IsNullOrEmpty(text))
							{
								throw new Exception("Json read error");
							}
							json.CollectedTargets[text] = list.ToArray();
							text = null;
							list.Clear();
							flag = false;
						}
					}
					else
					{
						num--;
					}
					if (num <= 0)
					{
						return;
					}
				}
				throw new Exception("Json read error");
			}

			// Token: 0x0600662A RID: 26154 RVA: 0x0020DF84 File Offset: 0x0020C184
			public string Write()
			{
				JsonSerializer jsonSerializer = new JsonSerializer();
				string text;
				using (TextWriter textWriter = new StringWriterWithEncoding(Encoding.UTF8))
				{
					using (JsonWriter jsonWriter = new JsonTextWriter(textWriter))
					{
						jsonSerializer.Serialize(jsonWriter, this);
						text = textWriter.ToString();
					}
				}
				return text;
			}

			// Token: 0x04007526 RID: 29990
			public readonly Dictionary<string, string[]> CollectedTargets = new Dictionary<string, string[]>();
		}
	}
}
