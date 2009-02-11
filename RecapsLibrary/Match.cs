using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Globalization;

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
        private List<EventResults.CombatLogEntry> _combatLog = new List<EventResults.CombatLogEntry>();
        private List<Death> _deaths = new List<Death>();
        private List<Player> _players = new List<Player>();
        public Hashtable GUIDs = new Hashtable();

        [Serializable]
        public struct Death
        {
            public Player Player;
            public DateTime TimeOfDeath;
            public DateTime StartDisplayTime;
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

        public List<EventResults.CombatLogEntry> CombatLog
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

        public static Match GenerateMatch(Hashtable SavedVariables, int MatchNumber)
        {
            DateTime startTime;
            DateTime stopTime;
            Team greenTeam = new Team();
            Team goldTeam = new Team();



            string thisKey = "RUPTURE_SAVED/details/" + MatchNumber.ToString();
            if (SavedVariables[thisKey + "/arenaStart"] == null)
            {
                return null;
            }

            startTime = HelperFunctions.ConvertFromEpoch(Convert.ToInt32(SavedVariables[thisKey + "/arenaStart"].ToString()));
            stopTime = HelperFunctions.ConvertFromEpoch(Convert.ToInt32(SavedVariables[thisKey + "/arenaStop"].ToString()));

            Match thisMatch = new Match(startTime, stopTime);

            int i = 0;
            Player thisPlayer = null;

            while ((thisPlayer = Player.GeneratePlayer(SavedVariables, MatchNumber, i)) != null)
            {
                i++;
                if (thisPlayer.TeamFaction == "Green")
                {
                    greenTeam.Name = thisPlayer.TeamName;
                    greenTeam.Players.Add(thisPlayer);
                }
                else
                {
                    goldTeam.Name = thisPlayer.TeamName;
                    goldTeam.Players.Add(thisPlayer);
                }
                thisMatch.Players.Add(thisPlayer);
            }

            int matchSize = goldTeam.Players.Count;
            if (matchSize < greenTeam.Players.Count)
                matchSize = greenTeam.Players.Count;

            thisMatch.MatchSize = matchSize;

            thisMatch.GoldTeam = goldTeam;
            thisMatch.GreenTeam = greenTeam;

            return thisMatch;
        }

        public List<EventResults.CombatLogEntry> CombatLogForDeath(Death Death)
        {
            List<EventResults.CombatLogEntry> tempList = new List<EventResults.CombatLogEntry>();
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
    }
}
