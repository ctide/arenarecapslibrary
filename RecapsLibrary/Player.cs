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

        public static Player GeneratePlayer(Hashtable SavedVariables, int MatchNumber, int PlayerNumber)
        {
            string thisKey = "RUPTURE_SAVED/details/" + MatchNumber.ToString() + "/details/" + PlayerNumber.ToString();
            if (SavedVariables[thisKey + "/server"] == null)
                return null;

            Player thisPlayer = new Player();
            thisPlayer.DamageDone = Convert.ToInt32(SavedVariables[thisKey + "/damageDone"].ToString());
            thisPlayer.Class = SavedVariables[thisKey + "/class"].ToString();
            thisPlayer.KillingBlows = Convert.ToInt32(SavedVariables[thisKey + "/killingBlows"].ToString());
            thisPlayer.Race = SavedVariables[thisKey + "/race"].ToString();
            thisPlayer.HealingDone = Convert.ToInt32(SavedVariables[thisKey + "/healingDone"].ToString());
            thisPlayer.Name = SavedVariables[thisKey + "/name"].ToString();
            thisPlayer.TeamFaction = SavedVariables[thisKey + "/faction"].ToString();
            thisPlayer.TeamName = SavedVariables[thisKey + "/team"].ToString();
            thisPlayer.Server = SavedVariables[thisKey + "/server"].ToString();

            return thisPlayer;
        }
    }

}
