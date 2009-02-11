using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Recaps
{
    public class ParseFiles
    {
        static CultureInfo provider = CultureInfo.InvariantCulture;
        static Int64 guidMask = Int64.Parse("00F0000000000000", NumberStyles.HexNumber);

        public static List<Match> Parse(string combatLogLocation, string luaFileLocation)
        {
            Hashtable results = HelperFunctions.LuaToHash(luaFileLocation);
            Match tempMatch = null;
            List<Match> matchesPlayed = new List<Match>();

            string thisLine = "";

            int i = 0;

            while ((tempMatch = Match.GenerateMatch(results, i)) != null)
            {
                if (tempMatch.MatchSize > 0)
                {
                    matchesPlayed.Add(tempMatch);
                }
                i++;
            }

            foreach (Match thisMatch in matchesPlayed)
            {
                StreamReader thisReader = new StreamReader(combatLogLocation);

                DateTime startTime = thisMatch.StartTime;
                DateTime endTime = thisMatch.EndTime;
                DateTime eventTime;

                string time = "";
                string entry = "";
                string eventType = "";
                ArrayList times = new ArrayList();

                ArrayList deadPlayers = new ArrayList();
                ArrayList timeIntoMatch = new ArrayList();

                while ((thisLine = thisReader.ReadLine()) != null)
                {
                    time = thisLine.Substring(0, 19).TrimEnd();
                    eventTime = DateTime.ParseExact(time, "M/d H:mm:ss.fff", provider);
                    if (eventTime > endTime)
                        break;
                    if (eventTime < startTime)
                        continue;
                    entry = thisLine.Substring(19);
                    string[] variables = entry.Split(',');
                    eventType = variables[0];
                    if (!thisMatch.GUIDs.ContainsKey(variables[4]))
                        thisMatch.GUIDs[variables[4]] = variables[5].Replace("\"", "");
                    switch (eventType)
                    {
                        case "UNIT_DIED":
                            for (int j = 0; j < thisMatch.Players.Count; j++)
                            {
                                if (thisMatch.Players[j].Name == variables[5].Replace("\"", ""))
                                {
                                    string hexValue = variables[4];
                                    Int64 guid = Int64.Parse(variables[4].Substring(2), NumberStyles.HexNumber);
                                    if ((guid & guidMask) == 0)
                                    {
                                        Match.Death thisDeath = new Match.Death();
                                        thisDeath.StartDisplayTime = eventTime.AddSeconds(-10);
                                        thisDeath.TimeOfDeath = eventTime;
                                        thisDeath.Player = thisMatch.Players[j];
                                        thisMatch.Deaths.Add(thisDeath);
                                        break;
                                    }
                                }
                            }
                            break;
                    }
                    EventResults.CombatLogEntry thisEntry;
                    thisEntry = EventResults.ParseRow(entry, eventTime);
                    thisMatch.CombatLog.Add(thisEntry);
                }
                thisMatch.UpdateGUIDS();
                thisReader.Close();
            }

            return matchesPlayed;
        }
    }
}
