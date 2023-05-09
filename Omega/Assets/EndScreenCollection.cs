using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Omega.UI
{
    public class EndScreenCollection : MonoBehaviour
    {
        public Button resetButton;

        [Header("First Player")]
        public Image player1Icon;
        public TextMeshProUGUI totalScorePlayer1;
        public TextMeshProUGUI killAmountPlayer1;
        public TextMeshProUGUI damageAmountPlayer1;
        public TextMeshProUGUI healAmountPlayer1;

        [Header("Second Player")]
        public Image player2Icon;
        public TextMeshProUGUI totalScorePlayer2;
        public TextMeshProUGUI killAmountPlayer2;
        public TextMeshProUGUI damageAmountPlayer2;
        public TextMeshProUGUI healAmountPlayer2;

        [Header("Third Player")]
        public Image player3Icon;
        public TextMeshProUGUI totalScorePlayer3;
        public TextMeshProUGUI killAmountPlayer3;
        public TextMeshProUGUI damageAmountPlayer3;
        public TextMeshProUGUI healAmountPlayer3;
    }
}