using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Recaps
{
    [Serializable]
    public class Player
    {
        public string guid;
        public int DamageDone { get; set; }
        public int HealingDone { get; set; }
        public int KillingBlows { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public string TeamName { get; set; }
        public string TeamFaction { get; set; }
        public int DamageIn { get; set; }
        public Team Team { get; set; }
        public int RatingChange { get; set; }

        private String _name { get; set; }
        private String _server { get; set; }


        public String Name
        {
            get
            {
                return _name;
            }
            set { SplitFullyQualifiedPlayerName(value); }
        }

        

        public string Server
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }

        public string QualifiedName
        {
            get
            {
                return String.Format("{0} of {1}", _name, _server);
            }
        }

        public Player() { }
        public Player(String FullyQualifiedPlayerName)
        {
            SplitFullyQualifiedPlayerName(FullyQualifiedPlayerName);
        }

        public override String ToString()
        {
            return _name;
        }

        protected void SplitFullyQualifiedPlayerName(String FullyQualifiedPlayerName)
        {
            if (FullyQualifiedPlayerName == null)
                throw new Exception("Null player name");

            FullyQualifiedPlayerName = FullyQualifiedPlayerName.Trim('"');
            if (FullyQualifiedPlayerName.Contains("-"))
            {
                string[] r = FullyQualifiedPlayerName.Split('-');
                _name = r[0];
                _server = r[1];
            }
            else
            {
                _name = FullyQualifiedPlayerName;
                _server = null;
            }
        }

        public static Player GeneratePlayer(Hashtable SavedVariables, int MatchNumber, int PlayerNumber, string team, int version)
        {
            string thisKey = "";
            Player thisPlayer = new Player();

            if (version == 1)
            {
                thisKey = "RECAPS_SAVED/details/" + MatchNumber.ToString() + "/details/" + PlayerNumber.ToString();
                if (SavedVariables[thisKey + "/server"] == null)
                    return null;
                thisPlayer.TeamFaction = SavedVariables[thisKey + "/faction"].ToString();
                thisPlayer.TeamName = SavedVariables[thisKey + "/team"].ToString();
                thisPlayer.Server = SavedVariables[thisKey + "/server"].ToString();
                thisPlayer.RatingChange = Convert.ToInt32(SavedVariables[thisKey + "/honorGained"].ToString());
            }
            else if (version == 2)
            {
                thisKey = "RECAPS_MATCHES/" + MatchNumber.ToString() + "/" + team + "/members/" + PlayerNumber.ToString();
                if (SavedVariables[thisKey + "/name"] == null)
                    return null;
            }

            thisPlayer.KillingBlows = Convert.ToInt32(SavedVariables[thisKey + "/killingBlows"].ToString());
            thisPlayer.HealingDone = Convert.ToInt32(SavedVariables[thisKey + "/healingDone"].ToString());
            thisPlayer.Name = SavedVariables[thisKey + "/name"].ToString();
            thisPlayer.Class = SavedVariables[thisKey + "/class"].ToString();
            thisPlayer.DamageDone = Convert.ToInt32(SavedVariables[thisKey + "/damageDone"].ToString());
            thisPlayer.Race = SavedVariables[thisKey + "/race"].ToString();
            thisPlayer.DamageIn = 0;

            return thisPlayer;
        }

        public bool OnMyTeam(string guid)
        {
            if (Team.GUIDs.Contains(guid))
                return true;
            else
                return false;
        }

        public string UrlName()
        {
            return UrlName("");
        }

        public string UrlName(string className)
        {
            string retVal = "<a ";
            if (className.Length > 0)
                retVal += "class=\"" + className + "\" ";

            retVal += "href=\"http://www.wowarmory.com/character-sheet.xml?r=" + Server + "&n=" + Name + "\">" + Name + "</a>";
            return retVal;
        }
    }
}
