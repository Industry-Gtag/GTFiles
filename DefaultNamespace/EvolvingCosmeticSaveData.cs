using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaTag.Scripts.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace DefaultNamespace
{
	// Token: 0x020013CD RID: 5069
	[NullableContext(1)]
	[Nullable(0)]
	public class EvolvingCosmeticSaveData
	{
		// Token: 0x17000C56 RID: 3158
		// (get) Token: 0x06007EDD RID: 32477 RVA: 0x00298D8C File Offset: 0x00296F8C
		public static EvolvingCosmeticSaveData Instance
		{
			get
			{
				EvolvingCosmeticSaveData evolvingCosmeticSaveData;
				if ((evolvingCosmeticSaveData = EvolvingCosmeticSaveData.s_instance) == null)
				{
					evolvingCosmeticSaveData = (EvolvingCosmeticSaveData.s_instance = new EvolvingCosmeticSaveData());
				}
				return evolvingCosmeticSaveData;
			}
		}

		// Token: 0x06007EDE RID: 32478 RVA: 0x00298DA4 File Offset: 0x00296FA4
		private EvolvingCosmeticSaveData()
		{
			string @string = PlayerPrefs.GetString("EvolvingCosmeticSaveData");
			if (@string != null)
			{
				this.ReadFromJson(@string);
			}
		}

		// Token: 0x06007EDF RID: 32479 RVA: 0x00298DD8 File Offset: 0x00296FD8
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

		// Token: 0x06007EE0 RID: 32480 RVA: 0x00298E40 File Offset: 0x00297040
		private void ReadFromJson(string json)
		{
			using (TextReader textReader = new StringReader(json))
			{
				using (JsonReader jsonReader = new JsonTextReader(textReader))
				{
					while (jsonReader.Read())
					{
						if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "SelectedIndices")
						{
							this.ReadSelectedIndices(jsonReader);
						}
					}
				}
			}
		}

		// Token: 0x06007EE1 RID: 32481 RVA: 0x00298EC0 File Offset: 0x002970C0
		private void ReadSelectedIndices(JsonReader reader)
		{
			int num = 0;
			string text = null;
			while (reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType <= JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.StartObject)
					{
						if (tokenType == JsonToken.PropertyName)
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
						}
					}
					else
					{
						num++;
					}
				}
				else if (tokenType != JsonToken.Integer)
				{
					if (tokenType == JsonToken.EndObject)
					{
						num--;
					}
				}
				else
				{
					if (text == null)
					{
						throw new Exception("Json read error");
					}
					object value = reader.Value;
					if (!(value is long))
					{
						throw new Exception("Json read error");
					}
					long num2 = (long)value;
					this.SelectedIndices[text] = (int)num2;
				}
				if (num <= 0)
				{
					return;
				}
			}
			throw new Exception("Json read error");
		}

		// Token: 0x04009149 RID: 37193
		public readonly Dictionary<string, int> SelectedIndices = new Dictionary<string, int>();

		// Token: 0x0400914A RID: 37194
		[Nullable(2)]
		private static EvolvingCosmeticSaveData s_instance;

		// Token: 0x0400914B RID: 37195
		public const string PlayerPrefsKey = "EvolvingCosmeticSaveData";
	}
}
