﻿using Engine.Actions;
using Engine.Statistics;
using Engine.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Skills
{
    public class MovementSkills
    {
        public string Name { get; set; }
        public int Rank { get; set; }
        public string Description { get; set; }
        public CharacterStat MainStat{ get; set; }
        public List<SkillModifier> TotalSkillModifiers { get; set; }

        private int _value;
        public int ModifiedValue //this is the public value that is constantly displayed
        {
            get
            {
                _value = CalculateFinalValue();
                return _value;
            }
        }


        private int CalculateFinalValue()
        {
            int finalValue = ModifiedValue;

            for (int i = 0; i < TotalSkillModifiers.Count; i++)
            {
                finalValue += TotalSkillModifiers[i].Value;
            }
            return finalValue;
        }

        //contstructor 
        public MovementSkills(string name, int rank = 0, string description = "")
        {
            Name = name;
            Rank = rank;
            Description = description;
            TotalSkillModifiers = new List<SkillModifier>();
            TotalSkillModifiers.Add(SkillModifiersLists.GetAptitudesById(rank));
        }


        public void AddModifier(SkillModifier mod)
        {
            this.TotalSkillModifiers.Add(mod);
        }
        public void RemoveModifier(SkillModifier mod)
        {
            this.TotalSkillModifiers.Remove(mod);
        }

        public bool ModifyRank(bool isLevelUp)
        {
            var result = true;
            if(isLevelUp && Rank != 4)
            {
                var currentAptitude = TotalSkillModifiers.Where(x => x.Id == Rank).FirstOrDefault();//finds the current aptidude
                TotalSkillModifiers.Remove(currentAptitude);//removes this
                TotalSkillModifiers.Add(SkillModifiersLists.AptitudesList.FirstOrDefault(x => x.Id == Rank + 1));//Replace with the next level up.
                Rank += 1;
                result = true;
            }
            else 
            {
                //return a warning
                result = false;
            }
            if(!isLevelUp)
            {
                Rank -= 1;
            }
            return result;
        }
    }




    public class Acrobatics : MovementSkills
    {
        public Acrobatics(int rank = 0, string description = "") : base(rank, description)
        {
            Rank = rank;
            Description = description;
        }

        public Tuple<string, bool, int> AcrobaticsCheck()
        {
            return CharacteristicChecks.DoCharacteristicCheck(this.ModifiedValue);
        }
    }
}
