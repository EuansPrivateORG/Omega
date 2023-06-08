using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Omega.UI
{
    public class RoundStart : MonoBehaviour
    {
        public List<Base> playerTypesList = new List<Base>();

        [SerializeField] TextMeshProUGUI numberOfPlayers;
        [SerializeField] TextMeshProUGUI numberOfRounds;
        [SerializeField] public Button startRoundButton;
        [SerializeField] public Button initiateButton;
        [SerializeField] Button skipButton;

        [Header("Faction Icons")]
        [SerializeField] GameObject playerFactionIconPrefab;
        [SerializeField] RectTransform iconSpawnPosition;

        [Header("RoundCounter Buttons")]
        [SerializeField] Button minusRoundsButton;
        [SerializeField] Button plusRoundsButton;

        [Header("PlayerCounter Buttons")]
        [SerializeField] Button minusPlayerButton;
        [SerializeField] Button plusPlayerButton;
        public List<GameObject> iconList = new List<GameObject>();
        public CanvasGroup GameHUD;

        PlayerSpawnHandler playerSpawnHandler;
        EventSystem eventSystem;
        RoundHandler roundHandler;

        public List<Base> playerTypesListToSpawn = new List<Base>();

        [HideInInspector] public bool continueFade = false;

        private void Awake()
        {
            roundHandler = FindObjectOfType<RoundHandler>();
            eventSystem = FindObjectOfType<EventSystem>();
            playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();
        }

        private void Start()
        {
            GameHUD.alpha = 0;
            GameHUD.interactable = false;
            for (int i = 0; i < playerSpawnHandler.numberOfPlayers; i++) 
            {
                int ran = Random.Range(0, playerTypesList.Count);
                GameObject instantiatedIcon = Instantiate(playerTypesList[ran].startMenuVarientIcon, iconSpawnPosition);
                RectTransform iconRect = instantiatedIcon.GetComponent<RectTransform>();
                iconRect.sizeDelta = new Vector2(50, 50);
                playerTypesListToSpawn.Add(playerTypesList[ran]);
                playerTypesList.RemoveAt(ran);
                iconList.Add(instantiatedIcon);
            }
        }

        private void Update()
        {
            numberOfPlayers.text = playerSpawnHandler.numberOfPlayers.ToString();

            numberOfRounds.text = roundHandler.numOfRounds.ToString();

            if (eventSystem.currentSelectedGameObject == null)
            {
                if(plusPlayerButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(minusPlayerButton.gameObject);
                }
                else if (minusPlayerButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(plusPlayerButton.gameObject);
                }
                else if (plusRoundsButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(minusRoundsButton.gameObject);
                }
                else if (minusRoundsButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(plusRoundsButton.gameObject);
                }
                else eventSystem.SetSelectedGameObject(initiateButton.gameObject);
            }


            if (playerSpawnHandler.numberOfPlayers == 6)
            {
                plusPlayerButton.interactable = false;
                return;
            }
            else plusPlayerButton.interactable = true;

            if (playerSpawnHandler.numberOfPlayers == 3)
            {
                minusPlayerButton.interactable = false;

                return;
            }
            else minusPlayerButton.interactable = true;



            if (roundHandler.numOfRounds == 9)
            {
                plusRoundsButton.interactable = false;
                return;
            }
            else plusRoundsButton.interactable = true;

            if (roundHandler.numOfRounds == 1)
            {
                minusRoundsButton.interactable = false;

                return;
            }
            else minusRoundsButton.interactable = true;
        }
        public void addPlayer()
        {

            playerSpawnHandler.numberOfPlayers++;
            int ran = Random.Range(0, playerTypesList.Count);
            GameObject instantiatedIcon = Instantiate(playerTypesList[ran].startMenuVarientIcon, iconSpawnPosition);
            RectTransform iconRect = instantiatedIcon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(50, 50);
            playerTypesListToSpawn.Add(playerTypesList[ran]);
            playerTypesList.RemoveAt(ran);
            iconList.Add(instantiatedIcon);
        }

        public void AddRound()
        {
            roundHandler.numOfRounds++;
        }
        public void RemoveRound()
        {
            roundHandler.numOfRounds--;
        }

        public void removePlayer()
        {

            playerSpawnHandler.numberOfPlayers--;
            if(iconList.Count > 0)
            {
                GameObject lastPlayer = iconList[iconList.Count - 1];

                playerTypesList.Add(playerTypesListToSpawn[playerTypesListToSpawn.Count - 1]);
                playerTypesListToSpawn.RemoveAt(playerTypesListToSpawn.Count - 1);
                iconList.Remove(lastPlayer);
                Destroy(lastPlayer);
            }   
        }


        public void StartRound()
        {
            StartCoroutine(StartRoundIntiate());
        }

        private IEnumerator StartRoundIntiate()
        {
            FindObjectOfType<InputSystemUIInputModule>().enabled = false;

            FindObjectOfType<WhiteFade>().StartFade(true);

            while (!continueFade)
            {
                yield return null;
            }

            roundHandler.StartFirstRound(playerTypesListToSpawn);

            GameHUD.alpha = 1;
            GameHUD.interactable = true;

            FindObjectOfType<Pause>().canPause = true;

            CanvasGroup canvas = GetComponent<CanvasGroup>();
            canvas.alpha = 0;
            canvas.interactable = false;
            eventSystem.SetSelectedGameObject(FindObjectOfType<DrawCardHandler>().attackButton.gameObject);
        }
    }
}
