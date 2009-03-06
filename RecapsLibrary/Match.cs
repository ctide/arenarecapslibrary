using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

namespace Recaps
{
    [Serializable]
    public class Match
    {
        private Int64 guidMask = Int64.Parse("00F0000000000000", NumberStyles.HexNumber);
        private DateTime _start;
        private DateTime _end;
        private Team _greenTeam;
        private Team _goldTeam;
        private int _matchSize;
        private List<CombatLog.CombatLogEntry> _combatLog = new List<CombatLog.CombatLogEntry>();
        private List<Death> _deaths = new List<Death>();
        private List<Player> _players = new List<Player>();
        private List<Resurrection> _resurrections = new List<Resurrection>();
        public Hashtable GUIDs = new Hashtable();
        public Guid matchGuid = Guid.NewGuid();
        public string MapName { get; set; }
        public bool Starred { get; set; }
        public TimeSpan CombatLogOffset { get; set; }

        [Serializable]
        public struct Death
        {
            public Player Player;
            public DateTime TimeOfDeath;
            public DateTime StartDisplayTime;
        }

        [Serializable]
        public struct Resurrection
        {
            public Player Player;
            public DateTime TimeOfRes;
        }

        public Team Winner
        {
            get
            {
                Player thisPlayer = _players[0];
                if (thisPlayer.Team.RatingChange > 0)
                    return thisPlayer.Team;
                else
                {
                    if (thisPlayer.TeamFaction == "Green")
                        return _goldTeam;
                    else
                        return _greenTeam;
                }
            }
        }

        public Team Loser
        {
            get
            {
                Player thisPlayer = _players[0];
                if (thisPlayer.Team.RatingChange < 0)
                    return thisPlayer.Team;
                else
                {
                    if (thisPlayer.TeamFaction == "Green")
                        return _goldTeam;
                    else
                        return _greenTeam;
                }
            }
        }

        public List<Death> Deaths
        {
            get
            {
                return _deaths;
            }
            set
            {
                _deaths = value;
            }
        }

        public List<Resurrection> Resurrections
        {
            get
            {
                return _resurrections;
            }
            set
            {
                _resurrections = value;
            }
        }

        public List<Player> Players
        {
            get
            {
                return _players;
            }
            set
            {
                _players = value;
            }
        }

        public List<CombatLog.CombatLogEntry> CombatLog
        {
            get
            {
                return _combatLog;
            }
            set
            {
                _combatLog = value;
            }
        }

        public Match(DateTime StartTime, DateTime EndTime)
        {
            _start = StartTime;
            _end = EndTime;
        }



        public DateTime StartTime
        {
            get
            {
                return _start;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _end;
            }
        }

        public Team GreenTeam
        {
            get
            {
                return _greenTeam;
            }
            set
            {
                _greenTeam = value;
            }
        }

        public Team GoldTeam
        {
            get
            {
                return _goldTeam;
            }
            set
            {
                _goldTeam = value;
            }
        }

        public int MatchSize
        {
            get
            {
                return _matchSize;
            }
            set
            {
                _matchSize = value;
            }
        }

        public static Match GenerateMatch(Hashtable SavedVariables, int MatchNumber, TimeSpan ts)
        {
            DateTime startTime;
            DateTime stopTime;
            Team greenTeam = new Team();
            Team goldTeam = new Team();
            bool v2 = true;

            string thisKey = "";

            if (SavedVariables.ContainsKey("RECAPS_MATCHES/0/version"))
            {
                // new style formatting
                thisKey = "RECAPS_MATCHES/" + MatchNumber.ToString();
            }
            else
            {
                // old style formatting
                thisKey = "RECAPS_SAVED/details/" + MatchNumber.ToString();
                v2 = false;
            }

            if (SavedVariables[thisKey + "/arenaStart"] == null)
            {
                return null;
            }

            startTime = HelperFunctions.ConvertFromEpoch(Convert.ToInt32(SavedVariables[thisKey + "/arenaStart"].ToString()));
            stopTime = HelperFunctions.ConvertFromEpoch(Convert.ToInt32(SavedVariables[thisKey + "/arenaStop"].ToString()));
            stopTime = stopTime.AddSeconds(10);
            startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
            stopTime = DateTime.SpecifyKind(stopTime, DateTimeKind.Utc);

            Match thisMatch = new Match(startTime, stopTime);

            int i = 0;
            Player thisPlayer = null;

            // new
            if (v2 == true)
            {
                thisMatch.MapName = SavedVariables[thisKey + "/mapName"].ToString();

                greenTeam.Name = SavedVariables[thisKey + "/green/team"].ToString();
                greenTeam.RatingChange = Convert.ToInt32(SavedVariables[thisKey + "/green/honorGained"].ToString());
                greenTeam.Server = SavedVariables[thisKey + "/green/server"].ToString();
                greenTeam.OldRating = Convert.ToInt32(SavedVariables[thisKey + "/green/teamRating"].ToString());

                goldTeam.Name = SavedVariables[thisKey + "/gold/team"].ToString();
                goldTeam.RatingChange = Convert.ToInt32(SavedVariables[thisKey + "/gold/honorGained"].ToString());
                goldTeam.Server = SavedVariables[thisKey + "/gold/server"].ToString();
                goldTeam.OldRating = Convert.ToInt32(SavedVariables[thisKey + "/gold/teamRating"].ToString());

                thisMatch.MatchSize = Convert.ToInt32(SavedVariables[thisKey + "/teamSize"].ToString());
                
                if (Convert.ToInt32(SavedVariables[thisKey + "/starred"]) == 1)
                    thisMatch.Starred = true;
                else
                    thisMatch.Starred = false;

                for (i = 0; i < thisMatch.MatchSize; i++)
                {
                    thisPlayer = Player.GeneratePlayer(SavedVariables, MatchNumber, i, "green", 2);
                    thisPlayer.Team = greenTeam;
                    thisPlayer.TeamFaction = "Green";
                    thisPlayer.TeamName = greenTeam.Name;
                    greenTeam.Players.Add(thisPlayer);
                    thisPlayer.Server = greenTeam.Server;
                    thisMatch.Players.Add(thisPlayer);

                    thisPlayer = Player.GeneratePlayer(SavedVariables, MatchNumber, i, "gold", 2);
                    thisPlayer.Team = goldTeam;
                    thisPlayer.TeamFaction = "Gold";
                    thisPlayer.TeamName = goldTeam.Name;
                    goldTeam.Players.Add(thisPlayer);
                    thisPlayer.Server = goldTeam.Server;
                    thisMatch.Players.Add(thisPlayer);
                }
                string tzOffset = SavedVariables[thisKey + "/timeZone"].ToString();
                tzOffset = tzOffset.Split(' ')[1];
                if (tzOffset.Length == 5)
                {
                    int hour = Int32.Parse(tzOffset.Substring(0, 3));
                    int minute = Int32.Parse(tzOffset.Substring(3));
                    thisMatch.CombatLogOffset = new TimeSpan(hour, minute, 0);
                }
            }
            else
            {
                thisMatch.MapName = SavedVariables[thisKey + "/map_name"].ToString();
                thisMatch.Starred = false;

                while ((thisPlayer = Player.GeneratePlayer(SavedVariables, MatchNumber, i, null, 1)) != null)
                {
                    i++;
                    if (thisPlayer.TeamFaction == "Green")
                    {
                        greenTeam.Name = thisPlayer.TeamName;
                        greenTeam.Players.Add(thisPlayer);
                        thisPlayer.Team = greenTeam;
                    }
                    else
                    {
                        goldTeam.Name = thisPlayer.TeamName;
                        goldTeam.Players.Add(thisPlayer);
                        thisPlayer.Team = goldTeam;
                    }
                    thisPlayer.Team.RatingChange = thisPlayer.RatingChange;
                    thisPlayer.Team.Server = thisPlayer.Server;
                    thisMatch.Players.Add(thisPlayer);
                }

                int matchSize = goldTeam.Players.Count;
                if (matchSize < greenTeam.Players.Count)
                    matchSize = greenTeam.Players.Count;

                thisMatch.MatchSize = matchSize;
                thisMatch.CombatLogOffset = ts;
            }
            
            thisMatch.GoldTeam = goldTeam;
            thisMatch.GreenTeam = greenTeam;
            return thisMatch;
        }

        public List<CombatLog.CombatLogEntry> CombatLogForDeath(Death Death)
        {
            List<CombatLog.CombatLogEntry> tempList = new List<CombatLog.CombatLogEntry>();
            for (int i = 0; i < _combatLog.Count; i++)
            {
                if (_combatLog[i].TimeStamp >= Death.StartDisplayTime && _combatLog[i].TimeStamp <= Death.TimeOfDeath)
                    tempList.Add(_combatLog[i]);
            }
            return tempList;
        }

        public void UpdateGUIDS()
        {
            foreach (DictionaryEntry d in GUIDs)
            {
                Int64 guid = Int64.Parse(d.Key.ToString().Substring(2), NumberStyles.HexNumber);
                if ((guid & guidMask) == 0)
                {
                    string name = "";
                    if (d.Value.ToString().Contains('-'))
                        name = d.Value.ToString().Split('-')[0];
                    else
                        name = d.Value.ToString(); 
                    foreach (Player a in this.Players)
                    {
                        if (a.Name == name)
                            a.guid = d.Key.ToString();
                    }
                }
            }
        }
        public string GenerateMD5()
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            string stringToHash = "";

            this.Players.Sort(delegate(Player p1, Player p2) { return p1.Name.CompareTo(p2.Name); });
            foreach (Player p in this.Players)
            {
                stringToHash += p.Name + p.KillingBlows.ToString() + p.DamageDone.ToString() + p.HealingDone.ToString() + p.Class + p.TeamName;
            }

            byte[] unicodeText = System.Text.Encoding.ASCII.GetBytes(stringToHash);
            unicodeText = md5.ComputeHash(unicodeText);

            string retVal = "";
            
            for (int i = 0; i < unicodeText.Length; i++)
            {
                retVal += unicodeText[i].ToString("X2");
            }

            return retVal;
        }

        public void CleanLog()
        {
            List<CombatLog.CombatLogEntry> newLog = new List<CombatLog.CombatLogEntry>();
            foreach (Death thisDeath in this.Deaths)
            {
                Player targetPlayer = thisDeath.Player;

                foreach (CombatLog.CombatLogEntry thisEntry in this.CombatLogForDeath(thisDeath))
                {
                    bool relevant = false;

                    if (thisEntry.DestGuid == targetPlayer.guid)
                    {
                        if (thisEntry.Damage > 0)
                            relevant = true;
                        if (thisEntry.Healing > 0)
                            relevant = true;
                    }
                    if (relevant == false)
                        if (thisEntry.SpellName != null)
                            if (Spell.SpellClassification(thisEntry.SpellName, thisEntry.SpellID) == Spell.SpellType.cc)
                            {
                                if (targetPlayer.OnMyTeam(thisEntry.DestGuid))
                                {
                                    relevant = true;
                                    if (thisEntry.Applied == 0)
                                        relevant = false;
                                }
                            }
                            else if (thisEntry.EventType == "SPELL_INTERRUPT")
                            {
                                if (targetPlayer.OnMyTeam(thisEntry.DestGuid))
                                    relevant = true;
                            }
                            else if (thisEntry.EventType == "SPELL_RESURRECT")
                            {
                                relevant = true;
                            }
                    if (relevant == true)
                    {
                        if (!newLog.Contains(thisEntry))
                            newLog.Add(thisEntry);
                    }
                }
            }
            newLog.Sort(delegate(CombatLog.CombatLogEntry c1, CombatLog.CombatLogEntry c2) { return c1.TimeStamp.CompareTo(c2.TimeStamp); });
            _combatLog = newLog;
        }
    }
}
