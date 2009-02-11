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
    }
}
