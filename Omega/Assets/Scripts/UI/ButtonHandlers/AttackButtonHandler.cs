using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Omega.Actions;

namespace Omega.UI
{
    public class AttackButtonHandler : ActionButtonHandler
    {
        public Dice dice;

        public void ButtonPressed()
        {
            int damage = dice.roll();


        }
    }
}