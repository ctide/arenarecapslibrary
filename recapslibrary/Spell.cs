using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recaps
{
    public class Spell
    {
        [Serializable]
        public enum SpellType
        {
            none = 0,
            cc = 1,
            interrupt = 2,
            defensive = 3,
            ms = 4
        }

        public static SpellType SpellClassification(String spellName, string spellId)
        {
            spellName = spellName.Trim('"');
            switch (spellName.ToLower())
            {
                // racial
                case "arcane torrent":
                    return SpellType.interrupt;
                case "warstomp":
                    return SpellType.cc;

                // hunter
                case "freezing trap":
                    return SpellType.cc;
                case "scatter shot":
                    return SpellType.cc;
                case "wyvern sting":
                    if (spellId == "49012")
                        return SpellType.cc;
                    break;
                case "silencing shot":
                    return SpellType.interrupt;
                case "aimed shot":
                    return SpellType.ms;
                case "intimidation":
                    return SpellType.cc;

                // paladin
                case "divine shield":
                    return SpellType.defensive;
                case "hand of protection":
                    return SpellType.defensive;
                case "repentance":
                    return SpellType.cc;
                case "hammer of justice":
                    return SpellType.cc;
                case "hand of sacrifice":
                    return SpellType.defensive;
                case "divine protection":
                    return SpellType.defensive;
                case "turn evil":
                    return SpellType.cc;

                // rogue
                case "evasion":
                    return SpellType.defensive;
                case "cloak of shadows":
                    return SpellType.defensive;
                case "sap":
                    return SpellType.cc;
                case "cheap shot":
                    return SpellType.cc;
                case "kidney shot":
                    return SpellType.cc;
                case "kick":
                    return SpellType.interrupt;
                case "wound poison":
                    return SpellType.ms;
                case "blind":
                    return SpellType.cc;
                case "gouge":
                    return SpellType.cc;

                // warrior
                case "shield wall":
                    return SpellType.defensive;
                case "pummel":
                    return SpellType.interrupt;
                case "concussion blow":
                    return SpellType.cc;
                case "mortal strike":
                    return SpellType.ms;
                case "intimidating shout":
                    return SpellType.cc;
                case "charge":
                    return SpellType.cc;
                case "intercept":
                    return SpellType.cc;

                // priest
                case "psychic scream":
                    return SpellType.cc;
                case "mind control":
                    return SpellType.cc;
                case "divine hymn":
                    return SpellType.cc;
                case "shackle undead":
                    return SpellType.cc;

                // druid
                case "cyclone":
                    return SpellType.cc;
                case "hibernate":
                    return SpellType.cc;
                case "barkskin":
                    return SpellType.defensive;
                case "bash":
                    return SpellType.cc;
                case "feral charge":
                    return SpellType.cc;

                // death knight
                case "bone armor":
                    return SpellType.defensive;
                case "ice bound fortitude":
                    return SpellType.defensive;
                case "anti-magic shell":
                    return SpellType.defensive;
                case "mind freeze":
                    return SpellType.interrupt;
                case "strangulate":
                    return SpellType.cc;
                case "gnaw":
                    return SpellType.cc;

                // shaman
                case "hex":
                    return SpellType.cc;
                case "earth shock":
                    return SpellType.interrupt;

                // warlock
                case "fear":
                    return SpellType.cc;
                case "howl of terror":
                    return SpellType.cc;
                case "death coil":
                    return SpellType.cc;
                case "seduction":
                    return SpellType.cc;
                case "spell lock":
                    return SpellType.interrupt;
                case "banish":
                    return SpellType.cc;
                case "enslave demon":
                    return SpellType.cc;

                // mage
                case "polymorph":
                    return SpellType.cc;
                case "counterspell":
                    return SpellType.interrupt;
                case "iceblock":
                    return SpellType.defensive;
                case "impact":
                    return SpellType.cc;

                // default
                default:
                    break;
            }

            return SpellType.none;
        }

    }
}
