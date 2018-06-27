﻿using Engine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Actions
{
    public static class CharacteristicChecks
    {
        public static Tuple<string, bool, int> DoCharacteristicCheck(int modCharValue)
        {
            Random rn = new Random();
            var d100 = DiceRolls.RollD100(rn);

            var isSuccess = false;
            isSuccess = d100.IntValue <= (modCharValue) ? true : false;
            int[] modcharArray = modCharValue.ToString().ToCharArray().Select(x => (int)Char.GetNumericValue(x)).ToArray();
            Array.Reverse(modcharArray);
            var degreesOfSuccessOrFailure = modcharArray[1] - d100.Result[1];
            isSuccess = d100.IntValue == 1 ? true : isSuccess;//if the roll is a 1, then you automatically pass
            return Tuple.Create(d100.StrValue, isSuccess, degreesOfSuccessOrFailure);
        }
    }
}
