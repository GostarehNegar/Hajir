using MassTransit.Registration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Xrm.Helpers
{
	// http://ttidbit.blogspot.com/2013/01/crm-timezone-codes.html
	public class CrmTimeZones
	{
		public const string CRM_TIME_ZONES =
		@"
0	Dateline Time	-12
1	Samoa Time	-11
2	Hawaiian Time	-10
3	Alaskan Time	-9
4	Pacific Time	-8
10	Mountain Time	-7
13	Mexico2 Time	-7
15	US Mountain Time	-7
20	Central Time	-6
25	Canada Central Time	-6
30	Mexico Time	-6
33	Central America Time	-6
35	Eastern Time	-5
40	US Eastern Time	-5
45	SA Pacific Time	-5
50	Atlantic Time	-4
55	SA Western Time	-4
56	Pacific SA Time	-4
60	Newfoundland Time	-3.5
65	E.South America Time	-3
70	SA Eastern Time	-3
73	Greenland Time	-3
75	Mid-Atlantic Time	-2
80	Azores Time	-1
83	Cape Verde Time	-1
85	GMT Time	0
90	Greenwich Time	0
95	Central Europe Time	1
100	Central European Time	1
105	Romance Time	1
110	W.Europe Time	1
113	W.Central Africa Time	1
115	E.Europe Time	2
120	Egypt Time	2
125	FLE Time	2
130	GTB Time	2
135	Jerusalem Time	2
140	South Africa Time	2
145	Russian Time	3
150	Arab Time	3
155	E.Africa Time	3
158	Arabic Time	3
160	Iran Time	3.5
165	Arabian Time	4
170	Caucasus Time	4
175	Afghanistan Time	4.5
180	Ekaterinburg Time	5
185	West Asia Time	5
190	India Time	5.5
193	Nepal Time	5.75
195	Central Asia Time	6
200	Sri Lanka Time	6
201	N.Central Asia Time	6
203	Myanmar Time	6.5
205	SE Asia Time	7
207	North Asia Time	7
210	China Time	8
215	Malay Peninsula Time	8
220	Taipei Time	8
225	W.Australia Time	8
227	North Asia East Time	8
230	Korea Time	9
235	Tokyo Time	9
240	Yakutsk Time	9
245	AUS Central Time	9.5
250	Cen.Australia Time	9.5
255	AUS Eastern Time	10
260	E.Australia Time	10
265	Tasmania Time	10
270	Vladivostok Time	10
275	West Pacific Time	10
280	Central Pacific Time	11
285	Fiji Islands Time	12
290	New Zealand Time	12
300	Tonga Time	13
360	Coordinated Universal Time(UTC) 0";


		public int Code { get; private set; }
		public string Name { get; private set; }

		public Single Offset { get; private set; }

		public override string ToString()
		{
			return $"{Name} ({Offset})";
		}
		private static CrmTimeZones Parse(string line)
		{
			var items = line.Split('\t');
			var result = new CrmTimeZones();
			if (items.Length == 3)
			{
				if (int.TryParse(items[0], out var code))
					result.Code = code;
				if (Single.TryParse(items[2], out var offset))
					result.Offset = offset;
				result.Name = items[1].Trim();
			}
			return result;

		}
		private static List<CrmTimeZones> items;

		public TimeZoneInfo TimeZoneInfo
		{
			get
			{
				return TimeZoneInfo.GetSystemTimeZones()
					.FirstOrDefault(x => x.BaseUtcOffset == TimeSpan.FromHours(Offset));
			}
		}
		public static CrmTimeZones[] TimeZones
		{
			get
			{
				if (items == null)
				{
					items = CRM_TIME_ZONES.Split('\r')
					.Where(x => x.Split('\t').Length > 1)
					.Select(x => Parse(x.Replace("\n", "")))
					.ToList();

				}
				return items.ToArray();
			}
		}
		public static CrmTimeZones GetTymeZoneByCode(int code)
		{
			
			return TimeZones.FirstOrDefault(x => x.Code == code);
		}
	}


}
