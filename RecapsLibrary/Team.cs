using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recaps
{
    [Serializable]
    public class Team
    {
        private List<Player> _players = new List<Player>();
        private string _teamName;
        private string _server;
        public int RatingChange;
        public int OldRating;

        public Team() { }

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

        public string Name
        {
            get
            {
                return _teamName;
            }
            set
            {
                _teamName = value;
            }
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

        public List<string> GUIDs
        {
            get
            {
                List<string> returnList = new List<String>();
                foreach(Player p in _players)
                    returnList.Add(p.guid);
                return returnList;
            }
        }

        public string UrlName()
        {
            return UrlName("");
        }

        public string UrlName(string className)
        {
            string retVal = "<a ";
            if (className.Length > 0)
                retVal += "class =\"" + className + "\" ";
            retVal += " href=\"http://www.wowarmory.com/team-info.xml?r=" + _server +
                    "&ts=" + _players.Count.ToString() + "&t=" + Name.Replace(' ', '+') + "&select=" + Name.Replace(' ', '+')
                    + "\">" + Name + "</a>";
            return retVal;
        }
    }
}
