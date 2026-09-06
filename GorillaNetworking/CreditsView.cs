using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x020010D7 RID: 4311
	public class CreditsView : MonoBehaviour
	{
		// Token: 0x17000A47 RID: 2631
		// (get) Token: 0x06006BB7 RID: 27575 RVA: 0x0022B7BE File Offset: 0x002299BE
		private int TotalPages
		{
			get
			{
				return this.creditsSections.Sum((CreditsSection section) => this.PagesPerSection(section));
			}
		}

		// Token: 0x06006BB8 RID: 27576 RVA: 0x0022B7D8 File Offset: 0x002299D8
		private void Start()
		{
			this.creditsSections = new CreditsSection[]
			{
				new CreditsSection
				{
					Title = "DEV TEAM",
					Entries = new List<string>
					{
						"Anton \"NtsFranz\" Franzluebbers", "Carlo Grossi Jr", "Cody O'Quinn", "David Neubelt", "David \"AA_DavidY\" Yee", "Derek \"DunkTrain\" Arabian", "Elie Arabian", "John Sleeper", "Haunted Army", "Kerestell Smith",
						"Keith \"ElectronicWall\" Taylor", "Laura \"Poppy\" Lorian", "Lilly Tothill", "Matt \"Crimity\" Ostgard", "Nick Taylor", "Ross Furmidge", "Sasha \"Kayze\" Sanders"
					}
				},
				new CreditsSection
				{
					Title = "SPECIAL THANKS",
					Entries = new List<string> { "The \"Sticks\"", "Alpha Squad", "Meta", "Scout House", "Mighty PR", "Caroline Arabian", "Clarissa & Declan", "Calum Haigh", "EZ ICE", "Gwen" }
				},
				new CreditsSection
				{
					Title = "MUSIC BY",
					Entries = new List<string> { "Stunshine", "David Anderson Kirk", "Jaguar Jen", "Audiopfeil", "Owlobe" }
				}
			};
			PlayFabTitleDataCache.Instance.GetTitleData("CreditsData", delegate(string result)
			{
				this.creditsSections = JsonMapper.ToObject<CreditsSection[]>(result);
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error fetching credits data: " + error.ErrorMessage);
			}, false);
		}

		// Token: 0x06006BB9 RID: 27577 RVA: 0x0022B9E6 File Offset: 0x00229BE6
		private int PagesPerSection(CreditsSection section)
		{
			return (int)Math.Ceiling((double)section.Entries.Count / (double)this.pageSize);
		}

		// Token: 0x06006BBA RID: 27578 RVA: 0x0022BA02 File Offset: 0x00229C02
		private IEnumerable<string> PageOfSection(CreditsSection section, int page)
		{
			return section.Entries.Skip(this.pageSize * page).Take(this.pageSize);
		}

		// Token: 0x06006BBB RID: 27579 RVA: 0x0022BA24 File Offset: 0x00229C24
		[return: TupleElementNames(new string[] { "creditsSection", "subPage" })]
		private ValueTuple<CreditsSection, int> GetPageEntries(int page)
		{
			int num = 0;
			foreach (CreditsSection creditsSection in this.creditsSections)
			{
				int num2 = this.PagesPerSection(creditsSection);
				if (num + num2 > page)
				{
					int num3 = page - num;
					return new ValueTuple<CreditsSection, int>(creditsSection, num3);
				}
				num += num2;
			}
			return new ValueTuple<CreditsSection, int>(this.creditsSections.First<CreditsSection>(), 0);
		}

		// Token: 0x06006BBC RID: 27580 RVA: 0x0022BA80 File Offset: 0x00229C80
		public void ProcessButtonPress(GorillaKeyboardBindings buttonPressed)
		{
			if (buttonPressed == GorillaKeyboardBindings.enter)
			{
				this.currentPage++;
				this.currentPage %= this.TotalPages;
			}
		}

		// Token: 0x06006BBD RID: 27581 RVA: 0x0022BAA8 File Offset: 0x00229CA8
		public string GetScreenText()
		{
			return this.GetPage(this.currentPage);
		}

		// Token: 0x06006BBE RID: 27582 RVA: 0x0022BAB8 File Offset: 0x00229CB8
		private string GetPage(int page)
		{
			ValueTuple<CreditsSection, int> pageEntries = this.GetPageEntries(page);
			CreditsSection item = pageEntries.Item1;
			int item2 = pageEntries.Item2;
			IEnumerable<string> enumerable = this.PageOfSection(item, item2);
			string text = "CREDITS";
			string text2;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS", out text2, text);
			text = "(CONT)";
			string text3;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_CONTINUED", out text3, text);
			string text4 = text2 + " - " + ((item2 == 0) ? item.Title : (item.Title + " " + text3));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(text4);
			stringBuilder.AppendLine();
			foreach (string text5 in enumerable)
			{
				stringBuilder.AppendLine(text5);
			}
			for (int i = 0; i < this.pageSize - enumerable.Count<string>(); i++)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine();
			text = "PRESS ENTER TO CHANGE PAGES";
			string text6;
			LocalisationManager.TryGetKeyForCurrentLocale("CREDITS_PRESS_ENTER", out text6, text);
			stringBuilder.AppendLine(text6);
			return stringBuilder.ToString();
		}

		// Token: 0x04007B2E RID: 31534
		private const string CREDITS_KEY = "CREDITS";

		// Token: 0x04007B2F RID: 31535
		private const string CREDITS_PRESS_ENTER_KEY = "CREDITS_PRESS_ENTER";

		// Token: 0x04007B30 RID: 31536
		private const string CREDITS_CONTINUED_KEY = "CREDITS_CONTINUED";

		// Token: 0x04007B31 RID: 31537
		private CreditsSection[] creditsSections;

		// Token: 0x04007B32 RID: 31538
		public int pageSize = 7;

		// Token: 0x04007B33 RID: 31539
		private int currentPage;

		// Token: 0x04007B34 RID: 31540
		private const string PlayFabKey = "CreditsData";
	}
}
