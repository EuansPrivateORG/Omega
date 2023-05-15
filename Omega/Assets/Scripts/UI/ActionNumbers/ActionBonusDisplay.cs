using Omega.Actions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omega.UI
{
    public class ActionBonusDisplay : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI bonusText;
        AttackButtonHandler attackButtonHandler = null;
        PlayerAction dice = null;


        private void Awake()
        {
            attackButtonHandler = GetComponent<AttackButtonHandler>();
            dice = attackButtonHandler.attack;
        }

        private void Start()
        {
            if(attackButtonHandler != null)
            {
            bonusText.text = "+" + dice.rollBonus.ToString();
            }
        }
    }
}
