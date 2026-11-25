using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaTagScripts;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

public class BananaMail : MonoBehaviour
{
	[Serializable]
	public class BananaMailMessage : IComparable<BananaMailMessage>
	{
		[SerializeField]
		private string subject;

		[SerializeField]
		private string body;

		[SerializeField]
		private string sent;

		[SerializeField]
		private string expires;

		private DateTime? dtSent;

		private DateTime? dtExpires;

		public string Subject
		{
			get
			{
				return subject;
			}
			set
			{
				subject = value;
			}
		}

		public string Body
		{
			get
			{
				return body;
			}
			set
			{
				body = value;
			}
		}

		[JsonIgnore]
		public string SubjectKey { get; set; }

		[JsonIgnore]
		public string BodyKey { get; set; }

		[JsonIgnore]
		public string[] HighlightPhrases { get; set; }

		[JsonIgnore]
		public bool OffersFanClubSignUp { get; set; }

		public DateTime Sent
		{
			get
			{
				if (dtSent.HasValue)
				{
					return dtSent.Value;
				}
				try
				{
					dtSent = DateTime.Parse(sent, CultureInfo.InvariantCulture);
				}
				catch
				{
					dtSent = DateTime.MinValue;
				}
				return dtSent.Value;
			}
			set
			{
				dtSent = value;
			}
		}

		public DateTime Expires
		{
			get
			{
				if (dtExpires.HasValue)
				{
					return dtExpires.Value;
				}
				try
				{
					dtExpires = DateTime.Parse(expires, CultureInfo.InvariantCulture);
				}
				catch
				{
					dtExpires = DateTime.MaxValue;
				}
				return dtExpires.Value;
			}
			set
			{
				dtExpires = value;
			}
		}

		public int GetId()
		{
			return (subject + body + sent).GetHashCode();
		}

		public int CompareTo(BananaMailMessage bmm)
		{
			return -Sent.CompareTo(bmm.Sent);
		}
	}

	public enum BananaMailStatus
	{
		none,
		loading,
		error,
		ready
	}

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private BananaMailMessage[] messages;

	[Tooltip("Editor only: pretend the fan club (VIM) subscription ended, so the goodbye mail shows when playing")]
	[SerializeField]
	private bool testFanClubEnded;

	[Tooltip("Editor only: how many days ago the pretend subscription ended (the goodbye mail's sent date)")]
	[SerializeField]
	[Min(0f)]
	private int testFanClubEndedDaysAgo = 3;

	[Tooltip("Editor only: add a long lorem ipsum message, to check how a long body fits on the computer screen")]
	[SerializeField]
	private bool testLongMessage;

	[Tooltip("Editor only: show the Steam details before the fan club checkout on the computer, as on a Steam build")]
	[SerializeField]
	private bool testSteamWarning;

	public static BananaMailMessage[] Messages;

	private static BananaMailStatus _status;

	public static int Unread;

	private const string ReadMessagesPrefKey = "BananaMailRead";

	private static readonly HashSet<int> readMessageIds = new HashSet<int>();

	public static BananaMailStatus Status
	{
		get
		{
			return _status;
		}
		internal set
		{
			_status = value;
			BananaMailStatusChange?.Invoke();
		}
	}

	public static event Action BananaMailStatusChange;

	public static event Action BananaMailUnreadChange;

	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Unread = 0;
		Messages = null;
		_status = BananaMailStatus.none;
		readMessageIds.Clear();
	}

	private static void LoadReadMessageIds()
	{
		readMessageIds.Clear();
		string[] array = PlayerPrefs.GetString("BananaMailRead", string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i], NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
			{
				readMessageIds.Add(result);
			}
		}
	}

	private static void SaveReadMessageIds()
	{
		PlayerPrefs.SetString("BananaMailRead", string.Join(",", readMessageIds));
	}

	public static void MarkMessageRead(int id)
	{
		if (Status == BananaMailStatus.ready && readMessageIds.Add(id))
		{
			Unread = Math.Max(0, Unread - 1);
			BananaMailUnreadChange?.Invoke();
			SaveReadMessageIds();
		}
	}

	private async void Start()
	{
		int tdCallsOut = 0;
		while (PlayFabTitleDataCache.Instance == null || GorillaComputer.instance == null || GorillaComputer.instance.GetServerTime().Year < 2000)
		{
			await Task.Delay(500);
		}
		List<BananaMailMessage> messages = new List<BananaMailMessage>();
		tdCallsOut++;
		PlayFabTitleDataCache.Instance.GetTitleData(titleDataKey, delegate(string data)
		{
			try
			{
				BananaMailMessage[] array = JsonConvert.DeserializeObject<BananaMailMessage[]>(data);
				if (array != null)
				{
					messages.AddRange(array);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("BananaMail :: Start :: Couldn't read the " + titleDataKey + " title data: " + ex.Message);
			}
			finally
			{
				tdCallsOut--;
			}
		}, delegate(PlayFabError err)
		{
			Debug.Log($"BananaMail :: Start :: TitleData Error {err}");
			tdCallsOut--;
		});
		while (tdCallsOut > 0)
		{
			await Task.Yield();
		}
		SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.LocalSubscriptionDetails();
		if (!subscriptionDetails.active && subscriptionDetails.daysAccrued > 0 && subscriptionDetails.subscriptionActiveUntilDate < GorillaComputer.instance.GetServerTime())
		{
			messages.Add(new BananaMailMessage
			{
				Subject = "WE'LL MISS YOU!!",
				Body = "Hello Very Important Monke!\n\nLooks like your Gorilla Tag Fan Club subscription has ended. I hope you had a good time and I'd love to see you come back soon! Sign up again by pressing OPTION 1 or at any GTFC Kiosk. -Lemming",
				SubjectKey = "MAIL_FAN_CLUB_ENDED_SUBJECT",
				BodyKey = "MAIL_FAN_CLUB_ENDED_BODY",
				HighlightPhrases = new string[1] { "OPTION 1" },
				OffersFanClubSignUp = true,
				Sent = subscriptionDetails.subscriptionActiveUntilDate,
				Expires = subscriptionDetails.subscriptionActiveUntilDate.AddYears(1)
			});
		}
		messages.RemoveAll(BadDates);
		Unread = 0;
		LoadReadMessageIds();
		HashSet<int> hashSet = new HashSet<int>();
		for (int num = messages.Count - 1; num >= 0; num--)
		{
			int id = messages[num].GetId();
			if (readMessageIds.Contains(id))
			{
				hashSet.Add(id);
			}
			else
			{
				Unread++;
			}
		}
		readMessageIds.Clear();
		readMessageIds.UnionWith(hashSet);
		SaveReadMessageIds();
		messages.Sort();
		Messages = messages.ToArray();
		Status = BananaMailStatus.ready;
	}

	private static bool BadDates(BananaMailMessage m)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		if (!(m.Expires < serverTime))
		{
			return m.Sent > serverTime;
		}
		return true;
	}
}
