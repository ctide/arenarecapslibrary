using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recaps
{
    [Serializable]
    public class CombatLog
    {
        /* Prefixes
    Event Prefix	 Description
    SPELL_	         Spell is the prefix for most effects even if the spell is a DoT or channeled. IE when the spell begins to be cast, SPELL_CAST_START is fired and not SPELL_PERIODIC_CAST_START. This is the same with _MISS, _FAILED, etc.
    SPELL_PERIODIC	 Spell_PERIODIC Only the effects that are periodic start with this PREFIX. IE: Successfully casting a DoT only happens once therefor even though the spell is periodic use the SPELL_ prefix. However, the damage is periodic so it will start with SPELL_PERIODIC_. 90% of the time you will only care about _DAMAGE or _HEAL.
    SPELL_BUILDING	 SPELL_BUILDING New in WotLK, assumed to be damage that can affect destructable buildings.
   Suffixes
    Event Prefix	 Description
    _DAMAGE	         Triggered on damage to health. Nothing Special
    _MISSED	         Triggered When Effect isn't applied but mana/energy is used IE: ABSORB BLOCK DEFLECT DODGE EVADE IMMUNE MISS PARRY REFLECT RESIST
    _HEAL	         Triggered when a unit is healed
    _ENERGIZE	     Any effect that restores energy/mana. Spell/trinket/item set bonuses can trigger this event. IE: Vampiric Touch, or Mark of Defiance (Trinket)
    _DRAIN	         Same as _ENERGIZE but this time you are losing energy/mana. Caused by enemies.
    _LEECH	         Same as _DRAIN, but the source unit will simultaneously receive the same kind of energy (specified in extraAmount)
    _INTERRUPT	     Spellcasting being interrupted by an ability such as Kick or Pummel.
    _DISPEL	         A buff or debuff being actively dispelled by a spell like Remove Curse or Dispel Magic.
    _DISPEL_FAILED	 A failed attempt to dispel a buff or debuff, most likely due to immunity.
    _AURA_STOLEN	 A buff being transferred from the destination unit to the source unit (i.e. mages' Spellsteal).
    _EXTRA_ATTACKS	 Unit gains extra melee attacks due to an ability (like Sword Sepcialization or Windfury). These attacks usually happen in brief succession 100-200ms following this event.
    _AURA_APPLIED	 Triggered When Buffs/Debuffs are Applied. Note: This event doesn't fire if a debuff is applied twice without being removed. IE: casting Vampiric Embrace twice in a row only triggers this event once. This can make it difficult to track whether a debuff was successfully reapplied to the target. However, for instant cast spells, SPELL_CAST_SUCCESS can be used.
    _AURA_REMOVED	 Triggered When Buffs/Debuffs expire.
    _AURA_APPLIED_DOSE	 Triggered by stacking Debuffs if the debuff is already applied to a target. IE: If you cast Mind Flay twice it causes 2 doses of shadow vunerability, the first time it will trigger, SPELL_AURA_APPLIED (arg10 = shadow vulnerability), and SPELL_AURA_APPLIED_DOSE (arg10 = shadow vunerability) the second. The last argument reflects the new number of doses on the unit.
    _AURA_REMOVED_DOSE	 The opposite of _AURA_APPLIED_DOSE, reducing the amount of doses on a buff/debuff on the unit.
    _AURA_REFRESH	 Resets the expiration timer of a buff/debuff on the unit.
    _AURA_BROKEN	 A buff or debuff is being removed by melee damage
    _AURA_BROKEN_SPELL	 A buff or debuff is being removed by spell damage (specified in extraSpell...)
    _CAST_START	     Triggered when a spell begins casting. Instant Cast and channeled spells don't invoke this event. They trigger _CAST_SUCCESS, _MISS instead.
    _CAST_SUCCESS	 Triggered when channeled spells begin or when instant cast spells are cast. This (obviously) isn't triggered when this spell misses. On a miss SPELL_MISS will be triggered instead. Also, spells that invoke _CAST_START don't trigger this event when they are done casting. Use _SPELL_MISS or _SPELL_DAMAGE or _SPELL_AURA_APPLIED to see when they were cast
    _CAST_FAILED	 If the cast fails before it starts (IE invalid target), then _CAST_START never triggers. However it is possible for a cast to fail after beginning. (IE you jump, move, hit escape etc.)
    _INSTAKILL	     Immediately kills the destination unit (usually happens when warlocks sacrifice their minions).
    _DURABILITY_DAMAGE	
    _DURABILITY_DAMAGE_ALL	
    _CREATE	         Creates an object (as opposed to an NPC who are 'summoned') like a hunter's trap or a mage's portal.
    _SUMMON	         Summmons an NPC such as a pet or totem.
 */

        [Serializable]
        public struct CombatLogEntry
        {
            private string _spellSchool;
            private string _extraSpellSchool;
            private string _damageSchool;
            private string _powerType;

            public DateTime TimeStamp;
            public string EventType;
            public string SrcGuid;
            public string DestGuid;
            public string SpellName;
            public int Damage;
            public int Overkill;
            public int Healing;
            public int Overhealing;
            public int Blocked;
            public int Absorbed;
            public int Resisted;
            public int EnergizeAmount;
            public bool Critical;
            public string MissType;
            public int AmountMissed;
            public int ExtraAmount;
            public string ExtraSpellName;
            public string AuraType;
            public int ExtraAttacks;
            public int Doses;
            public string FailedType;
            public int Applied;
            public string SpellID;

            public string WowheadSpellName()
            {
                return WowheadSpellName("");
            }

            public string WowheadSpellName(string className)
            {
                string retVal = "<a ";
                if (className != "")
                    retVal += "class=\"" + className + "\" ";
                retVal += "href=\"http://www.wowhead.com/?spell=" + this.SpellID.ToString() + "\">" + this.SpellName + "</a>";
                return retVal;
            }

                    

            public string ExtraSpellSchool
            {
                get
                {
                    return _extraSpellSchool;
                }
                set
                {
                    _extraSpellSchool = SpellSchoolConvert(value);
                }
            }

            public string SpellSchool
            {
                get
                {
                    return _spellSchool;
                }
                set
                {
                    _spellSchool = SpellSchoolConvert(value);
                }
            }

            public string DamageSchool
            {
                get
                {
                    return _damageSchool;
                }
                set
                {
                    _damageSchool = SpellSchoolConvert(value);
                }
            }

            public string PowerType
            {
                get
                {
                    return _powerType;
                }
                set
                {
                    _powerType = PowerTypeConvert(value);
                }
            }

            private static string PowerTypeConvert(string powerType)
            {
                switch (powerType)
                {
                    case "-2":
                        return "health";
                    case "0":
                        return "mana";
                    case "1":
                        return "rage";
                    case "2":
                        return "focus";
                    case "3":
                        return "energy";
                    case "4":
                        return "pet happiness";
                    case "5":
                        return "runes";
                    case "6":
                        return "runic power";
                }
                return "";
            }

            private static string SpellSchoolConvert(string internalSpellSchool)
            {
                switch (internalSpellSchool)
                {
                    case "0x01":
                        return "physical";
                    case "0x02":
                        return "holy";
                    case "0x04":
                        return "fire";
                    case "0x08":
                        return "nature";
                    case "0x10":
                        return "frost";
                    case "0x14":
                        return "frostfire";
                    case "0x18":
                        return "froststorm";
                    case "0x20":
                        return "shadow";
                    case "0x28":
                        return "shadowstorm";
                    case "0x40":
                        return "arcane";
                }
                return "";
            }
        }

        public static CombatLogEntry ParseRow(string RawEvent, DateTime timeStamp)
        {
            CombatLogEntry thisEntry = new CombatLogEntry();
            string[] eventParameters = RawEvent.Split(',');

            thisEntry.TimeStamp = timeStamp;
            thisEntry.EventType = eventParameters[0];
            thisEntry.SrcGuid = eventParameters[1];
            thisEntry.DestGuid = eventParameters[4];

            int baseParam = 7;
            string[] eventTypeSplit = thisEntry.EventType.Split('_');
            int next = 1;


            switch (eventTypeSplit[0])
            {
                case "SWING":
                    break;
                case "ENVIRONMENTAL":
                    baseParam = 8;
                    break;
                case "DAMAGE":
                    if (eventTypeSplit.Length >= 3)
                    {
                        eventTypeSplit[2] = "MISSED";
                        next = 2;
                    }
                    else
                        eventTypeSplit[1] = "DAMAGE";
                    baseParam = 10;
                    thisEntry.SpellName = eventParameters[8].Trim('"');
                    thisEntry.SpellSchool = eventParameters[9];
                    break;
                case "SPELL":
                    if (eventTypeSplit[1] == "PERIODIC" || eventTypeSplit[1] == "BUILDING")
                        next = 2;
                    baseParam = 10;
                    thisEntry.SpellID = eventParameters[7];
                    thisEntry.SpellName = eventParameters[8].Trim('"');
                    thisEntry.SpellSchool = eventParameters[9];
                    break;
                case "RANGE":
                    if (eventTypeSplit[1] == "PERIODIC" || eventTypeSplit[1] == "BUILDING")
                        next = 2;
                    baseParam = 10;
                    thisEntry.SpellID = eventParameters[7];
                    thisEntry.SpellName = eventParameters[8].Trim('"');
                    thisEntry.SpellSchool = eventParameters[9];
                    break;
            }
            switch (eventTypeSplit[next])
            {
                case "DAMAGE":
                    thisEntry.Damage = Convert.ToInt32(eventParameters[baseParam]);
                    thisEntry.Overkill = Convert.ToInt32(eventParameters[baseParam + 1]);
                    thisEntry.DamageSchool = eventParameters[baseParam + 2];
                    thisEntry.Resisted = Convert.ToInt32(eventParameters[baseParam + 3]);
                    thisEntry.Blocked = Convert.ToInt32(eventParameters[baseParam + 4]);
                    thisEntry.Absorbed = Convert.ToInt32(eventParameters[baseParam + 5]);
                    if (eventParameters[baseParam + 6] == "1")
                        thisEntry.Critical = true;
                    else
                        thisEntry.Critical = false;
                    break;
                case "MISSED":
                    thisEntry.MissType = eventParameters[baseParam];
                    if (eventParameters.Length == 9)
                        thisEntry.AmountMissed = Convert.ToInt32(eventParameters[baseParam + 1]);
                    break;
                case "HEAL":
                    thisEntry.Healing = Convert.ToInt32(eventParameters[baseParam]);
                    thisEntry.Overhealing = Convert.ToInt32(eventParameters[baseParam + 1]);
                    if (eventParameters[baseParam + 2] == "1")
                        thisEntry.Critical = true;
                    else
                        thisEntry.Critical = false;
                    break;
                case "ENERGIZE":
                    thisEntry.EnergizeAmount = Convert.ToInt32(eventParameters[baseParam]);
                    thisEntry.PowerType = eventParameters[baseParam + 1];
                    break;
                case "DRAIN":
                    thisEntry.EnergizeAmount = Convert.ToInt32(eventParameters[baseParam]) * -1;
                    thisEntry.PowerType = eventParameters[baseParam + 1];
                    thisEntry.ExtraAmount = Convert.ToInt32(eventParameters[baseParam + 2]);
                    break;
                case "LEECH":
                    thisEntry.EnergizeAmount = Convert.ToInt32(eventParameters[baseParam]) * -1;
                    thisEntry.PowerType = eventParameters[baseParam + 1];
                    thisEntry.ExtraAmount = Convert.ToInt32(eventParameters[baseParam + 2]);
                    break;
                case "INTERRUPT":
                    thisEntry.ExtraSpellName = eventParameters[baseParam + 1];
                    thisEntry.ExtraSpellSchool = eventParameters[baseParam + 2];
                    break;
                case "DISPEL":
                    thisEntry.ExtraSpellName = eventParameters[baseParam + 1];
                    thisEntry.ExtraSpellSchool = eventParameters[baseParam + 2];
                    if (eventTypeSplit.Length < next + 2)
                        thisEntry.AuraType = eventParameters[baseParam + 3];
                    break;
                case "STOLEN":
                    thisEntry.ExtraSpellName = eventParameters[baseParam + 1];
                    thisEntry.ExtraSpellSchool = eventParameters[baseParam + 2];
                    thisEntry.AuraType = eventParameters[baseParam + 3];
                    break;
                case "EXTRA":
                    thisEntry.ExtraAttacks = Convert.ToInt32(eventParameters[baseParam]);
                    break;
                case "AURA":
                    if (eventTypeSplit[next + 1] == "BROKEN" && eventTypeSplit.Length >= next + 3)
                    {
                        thisEntry.ExtraSpellName = eventParameters[baseParam + 1];
                        thisEntry.ExtraSpellSchool = eventParameters[baseParam + 2];
                        thisEntry.AuraType = eventParameters[baseParam + 3];
                    }
                    if (eventTypeSplit[next + 1] == "APPLIED")
                        thisEntry.Applied = 1;
                    if (eventTypeSplit[next + 1] == "REMOVED")
                        thisEntry.Applied = -1;
                    thisEntry.AuraType = eventParameters[baseParam];
                    if (eventTypeSplit.Length >= next + 3 && eventTypeSplit[next + 2] == "DOSE")
                        thisEntry.Doses = Convert.ToInt32(eventParameters[baseParam + 1]);
                    break;
                case "CAST":
                    if (eventTypeSplit[next + 1] == "FAILED")
                        thisEntry.FailedType = eventParameters[baseParam];
                    break;
            }
            return thisEntry;
        }

    }
}
