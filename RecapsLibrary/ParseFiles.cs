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


        public static List<Match> Parse(string combatLogLocation, string luaFileLocation, string outputPath, TimeSpan ts)
        {
            Hashtable results = HelperFunctions.LuaToHash(luaFileLocation);
            Match tempMatch = null;
            List<Match> matchesPlayed = new List<Match>();

            int i = 0;

            while ((tempMatch = Match.GenerateMatch(results, i, ts)) != null)
            {
                if (tempMatch.MatchSize > 0)
                {
                    matchesPlayed.Add(tempMatch);
                }
                i++;
            }

            for (i = 0; i < matchesPlayed.Count; i++)
            {
                matchesPlayed[i] = ParseCombatLog(matchesPlayed[i], combatLogLocation, outputPath, matchesPlayed[i].CombatLogOffset);
            }

            return matchesPlayed;
        }

        public static Match ParseCombatLog(Match thisMatch, string combatLogLocation, string outputPath, TimeSpan ts)
        {
            string thisLine = "";
            
            Hashtable deadGuids = new Hashtable();
            Hashtable ressedGuids = new Hashtable();
            StreamReader thisReader = new StreamReader(combatLogLocation);
            TextWriter newCombatLog = new StreamWriter(outputPath + thisMatch.matchGuid + ".txt");

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
                string[] chopItUp = thisLine.Split(' ');
                time = chopItUp[0] + " " + chopItUp[1];
                entry = "";
                for (int i = 2; i < chopItUp.Length; i++)
                    entry += chopItUp[i] + " ";
                entry = entry.Trim();
                eventTime = DateTime.ParseExact(time, "M/d H:mm:ss.fff", provider);
                eventTime = eventTime.Subtract(ts);
                eventTime = DateTime.SpecifyKind(eventTime, DateTimeKind.Utc);
                if (eventTime > endTime)
                    break;
                if (eventTime < startTime)
                    continue;
                newCombatLog.WriteLine(thisLine);
                string[] variables = entry.Split(',');
                eventType = variables[0];
                string name = variables[5].Replace("\"", "");
                if (name.Contains('-'))
                    name = name.Split('-')[0];
                if (!thisMatch.GUIDs.ContainsKey(variables[4]))
                    thisMatch.GUIDs[variables[4]] = name;
                if (eventType == "UNIT_DIED")
                {
                    Player deadPlayer = thisMatch.Players.Find(delegate(Player p) { return p.Name == name; });
                    if (deadPlayer != null)
                    {
                        string hexValue = variables[4];
                        Int64 guid = Int64.Parse(variables[4].Substring(2), NumberStyles.HexNumber);
                        if ((guid & guidMask) == 0)
                        {
                            Match.Death thisDeath = new Match.Death();
                            thisDeath.StartDisplayTime = eventTime.AddSeconds(-10);
                            thisDeath.TimeOfDeath = eventTime;
                            thisDeath.Player = deadPlayer;
                            thisMatch.Deaths.Add(thisDeath);
                            deadGuids[variables[4]] = thisDeath;
                        }

                    }
                }
                if (eventType == "SPELL_RESURRECT")
                {
                    Player ressedPlayer = thisMatch.Players.Find(delegate(Player p) { return p.Name == name; });
                    if (ressedPlayer != null)
                        ressedGuids[variables[7]] = ressedPlayer;
                    Match.Resurrection thisRes = new Match.Resurrection();
                    thisRes.Player = ressedPlayer;
                    thisRes.TimeOfRes = eventTime;
                    thisMatch.Resurrections.Add(thisRes);
                }
                if (eventType == "SPELL_DAMAGE" || eventType == "SPELL_HEAL")
                {
                    if (deadGuids.ContainsKey(variables[4]) && !ressedGuids.ContainsKey(variables[4]))
                        thisMatch.Deaths.Remove((Match.Death)deadGuids[variables[4]]);
                }
                CombatLog.CombatLogEntry thisEntry;
                thisEntry = CombatLog.ParseRow(entry, eventTime);
                if (thisEntry.Damage > 0 && thisMatch.GUIDs.ContainsKey(variables[4]))
                {
                    Player damagedPlayer = thisMatch.Players.Find(delegate(Player p) { return p.Name == thisMatch.GUIDs[variables[4]].ToString(); });
                    if (damagedPlayer != null)
                        damagedPlayer.DamageIn += thisEntry.Damage;
                }
                thisMatch.CombatLog.Add(thisEntry);
            }
            thisMatch.UpdateGUIDS();
            thisReader.Close();
            newCombatLog.Close();
            return thisMatch;
        }
    }
}
