using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_Analyzer
{
	public class Rating
	{
		public int roundsPlayed;
		public int deaths;
		// <nK, number of kills>
		public Dictionary<int, int> multipleKillsDictionary;

		public Rating()
		{
			roundsPlayed = 0;
			deaths = 0;
			multipleKillsDictionary = new Dictionary<int, int>();
			for(var i =1; i <= 5; i++)
			{
				multipleKillsDictionary.Add(i,0);
			}
		}

		public double getRating()
		{
			if (roundsPlayed == 0)
			{
				return 0;
			}

			double killsRating = multipleKillsDictionary.Keys.Sum(k => multipleKillsDictionary[k])/(double)roundsPlayed/0.679;
			double survivalRating = (roundsPlayed - deaths)/(double)roundsPlayed/0.317;
			double multipleKillsRating = 0;
			for (int i = 1; i <= 5; i++)
				multipleKillsRating += multipleKillsDictionary[i]*Math.Pow(i, 2);
			multipleKillsRating = multipleKillsRating / roundsPlayed / 1.277;

			double rating = killsRating + 0.7*survivalRating + multipleKillsRating;
			rating = rating / 2.7;
			return rating;
		}

	}
}
