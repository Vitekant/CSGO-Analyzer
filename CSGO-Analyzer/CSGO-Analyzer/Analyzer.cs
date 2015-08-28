using DemoInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGO_Analyzer
{
    public static class Analyzer
    {
        public static void Analyze(string demoPath)
        {
            using (var fileStream = File.OpenRead(demoPath))
            {
                Console.WriteLine("Parsing demo " + demoPath);
                using (var parser = new DemoParser(fileStream))
                {
                    parser.ParseHeader();

                    string map = parser.Map;

                    Console.WriteLine("Map: " + map);

                    bool hasMatchStarted = false;
                    bool firstBlood = true;
                    int round = 0;
                    Dictionary<Player, int> killsThisRound = new Dictionary<Player, int>();
                    Dictionary<Player, int> entryFrags = new Dictionary<Player, int>();
                    Dictionary<Player, int> entryFragAttempts = new Dictionary<Player, int>();
					Dictionary<Player, Rating> ratings = new Dictionary<Player, Rating>();

                    parser.MatchStarted += (sender, e) =>
                    {
                        hasMatchStarted = true;
                    };

                    parser.RoundStart += (sender, e) =>
                    {
                        if (!hasMatchStarted)
                            return;
						
                        firstBlood = true;
                        round++;
						killsThisRound.Clear();
	                    foreach (var player in parser.PlayingParticipants)
	                    {
							if (!ratings.ContainsKey(player))
							{
								ratings.Add(player, new Rating());
							}

		                    ratings[player].roundsPlayed++;
	                    }
                    };

					parser.RoundEnd += (sender, e) =>
					{
						if (!hasMatchStarted)
							return;

						//firstBlood = true;
						//round++;
						//killsThisRound.Clear();
						foreach (var player in killsThisRound.Keys)
						{
							if (!ratings.ContainsKey(player))
							{
								ratings.Add(player,new Rating());
							}

							ratings[player].multipleKillsDictionary[killsThisRound[player]]++;
						}
					};

					parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) => {

                    if (!hasMatchStarted)
                        return;
                        //the killer is null if you're killed by the world - eg. by falling
                        if (e.Killer != null)
                        {
                            if (!killsThisRound.ContainsKey(e.Killer))
                                killsThisRound[e.Killer] = 0;

                            if(e.Killer.Team == e.Victim.Team)
                            {
                                //killsThisRound[e.Killer]--;

                            }
                            else
                            {
                                killsThisRound[e.Killer]++;
                            }
							//Remember how many kills each player made this rounds

							if (!ratings.ContainsKey(e.Victim))
							{
								ratings.Add(e.Victim, new Rating());
							}

							ratings[e.Victim].deaths++;

							if (firstBlood)
                            {
                                if (!entryFrags.ContainsKey(e.Killer))
                                    entryFrags[e.Killer] = 0;

                                entryFrags[e.Killer]++;

                                if (!entryFragAttempts.ContainsKey(e.Killer))
                                    entryFragAttempts[e.Killer] = 0;

                                entryFragAttempts[e.Killer]++;

                                if (!entryFragAttempts.ContainsKey(e.Victim))
                                    entryFragAttempts[e.Victim] = 0;

                                entryFragAttempts[e.Victim]++;

                                firstBlood = false;
                            }
                        }
                    };

                    parser.ParseToEnd();

                    //foreach(var player in killsThisRound.Keys)
                    //{
                    //    if (!entryFragAttempts.ContainsKey(player))
                    //    {
                    //        entryFragAttempts[player] = 0;
                    //    }

                    //    if (!entryFrags.ContainsKey(player))
                    //    {
                    //        entryFrags[player] = 0;
                    //    }

                    //    Console.WriteLine(player.Name + "(" + player.Team + ")" + " entry frags: " + entryFrags[player] + "/" + entryFragAttempts[player]);
                    //}

	                foreach (var player in ratings.Keys)
	                {
		                Console.WriteLine(player.Name + "(" + player.Team + ")" + " rating: " + ratings[player].getRating());
	                }

				}
            }
        }
    }
}
