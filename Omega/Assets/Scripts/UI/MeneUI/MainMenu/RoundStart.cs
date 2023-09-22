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
        [SerializeField] GameObject CurrentPlayersParent;
        [SerializeField] GameObject SelectionTemplatePrefab;
        [SerializeField] GameObject playerFactionIconPrefab;
        [SerializeField] RectTransform iconSpawnPosition;

        [Header("RoundCounter Buttons")]
        [SerializeField] Button minusRoundsButton;
        [SerializeField] Button plusRoundsButton;

        [Header("PlayerCounter Buttons")]
        [SerializeField] Button minusPlayerButton;
        [SerializeField] Button plusPlayerButton;
        [SerializeField] Button confirmPlayers;
        [SerializeField] Button back;
        public List<GameObject> iconList = new List<GameObject>();
        public List<GameObject> currentPlayerSelectionList = new List<GameObject>();
        public CanvasGroup GameHUD;

        PlayerSelectionIdentifier firstPlayer;

        PlayerSpawnHandler playerSpawnHandler;
        EventSystem eventSystem;
        RoundHandler roundHandler;

        int readyPlayers = 0;

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
                int ranFaction = Random.Range(0, playerTypesList.Count);
                GameObject newPlayer = Instantiate(SelectionTemplatePrefab, CurrentPlayersParent.transform);
                PlayerSelectionIdentifier newPlayerIdentifier = newPlayer.GetComponent<PlayerSelectionIdentifier>();
                newPlayerIdentifier.playerNumber.text = "P" + (i+1).ToString();
                newPlayerIdentifier.FactionIconImage.sprite = playerTypesList[ranFaction].PlayerSelectionFactionIcon;
                newPlayerIdentifier.FactionIconImage.color = playerTypesList[ranFaction].uiOverriteColor;
                playerTypesListToSpawn.Add(playerTypesList[ranFaction]);
                playerTypesList.RemoveAt(ranFaction);

                if (currentPlayerSelectionList.Count == 0)
                {
                    ButtonNavSetup(minusPlayerButton, back, plusPlayerButton, newPlayerIdentifier.leftButton, null);

                    ButtonNavSetup(plusPlayerButton, minusPlayerButton, null, newPlayerIdentifier.rightButton, null);

                    ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, minusPlayerButton);

                    ButtonNavSetup(newPlayerIdentifier.rightButton, newPlayerIdentifier.leftButton, newPlayerIdentifier.confirmButton, null, plusPlayerButton);

                    ButtonNavSetup(newPlayerIdentifier.confirmButton, newPlayerIdentifier.rightButton, null, null, plusPlayerButton);

                    firstPlayer = newPlayerIdentifier;
                }
                else
                {
                    PlayerSelectionIdentifier previousPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();

                    //new player nav
                    if (currentPlayerSelectionList.Count == 2)
                    {
                        ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, previousPlayer.leftButton);

                        ButtonNavSetup(newPlayerIdentifier.rightButton, newPlayerIdentifier.leftButton, newPlayerIdentifier.confirmButton, null, previousPlayer.rightButton);

                        ButtonNavSetup(newPlayerIdentifier.confirmButton, newPlayerIdentifier.rightButton, null, null, previousPlayer.confirmButton);

                        ButtonNavSetup(confirmPlayers, minusPlayerButton, plusPlayerButton, null, newPlayerIdentifier.confirmButton);
                    }
                    else
                    {
                        ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, previousPlayer.leftButton);

                        ButtonNavSetup(newPlayerIdentifier.rightButton, newPlayerIdentifier.leftButton, newPlayerIdentifier.confirmButton, null, previousPlayer.rightButton);

                        ButtonNavSetup(newPlayerIdentifier.confirmButton, newPlayerIdentifier.rightButton, null, null, previousPlayer.confirmButton);
                    }


                    //previous player nav
                    ButtonNavSetup(previousPlayer.leftButton, null, previousPlayer.rightButton, newPlayerIdentifier.leftButton, previousPlayer.leftButton.navigation.selectOnUp.GetComponent<Button>());

                    ButtonNavSetup(previousPlayer.rightButton, previousPlayer.leftButton, previousPlayer.confirmButton, newPlayerIdentifier.rightButton, previousPlayer.rightButton.navigation.selectOnUp.GetComponent<Button>());

                    ButtonNavSetup(previousPlayer.confirmButton, previousPlayer.rightButton, null, newPlayerIdentifier.confirmButton, previousPlayer.confirmButton.navigation.selectOnUp.GetComponent<Button>());
                }

                currentPlayerSelectionList.Add(newPlayer);
            }
        }

        private void ButtonNavSetup(Button button, Button onSelectLeft, Button onSelectRight, Button onSelectDown, Button onSelectUp)
        {
            Navigation buttonNav = new Navigation();
            buttonNav.mode = Navigation.Mode.Explicit;
            if(onSelectLeft != null) buttonNav.selectOnLeft = onSelectLeft;
            if (onSelectRight != null) buttonNav.selectOnRight = onSelectRight;
            if (onSelectDown != null) buttonNav.selectOnDown = onSelectDown;
            if (onSelectUp != null) buttonNav.selectOnUp = onSelectUp;
            button.navigation = buttonNav;
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
            int ranFaction = Random.Range(0, playerTypesList.Count);
            GameObject newPlayer = Instantiate(SelectionTemplatePrefab, CurrentPlayersParent.transform);
            PlayerSelectionIdentifier newPlayerIdentifier = newPlayer.GetComponent<PlayerSelectionIdentifier>();
            newPlayerIdentifier.playerNumber.text = "P" + playerSpawnHandler.numberOfPlayers.ToString();
            newPlayerIdentifier.FactionIconImage.sprite = playerTypesList[ranFaction].PlayerSelectionFactionIcon;
            newPlayerIdentifier.FactionIconImage.color = playerTypesList[ranFaction].uiOverriteColor;
            playerTypesListToSpawn.Add(playerTypesList[ranFaction]);
            playerTypesList.RemoveAt(ranFaction);


            PlayerSelectionIdentifier previousPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();

            //new player nav
            ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, previousPlayer.leftButton);

            ButtonNavSetup(newPlayerIdentifier.rightButton, newPlayerIdentifier.leftButton, newPlayerIdentifier.confirmButton, null, previousPlayer.rightButton);

            ButtonNavSetup(newPlayerIdentifier.confirmButton, newPlayerIdentifier.rightButton, null, null, previousPlayer.confirmButton);

            ButtonNavSetup(confirmPlayers, minusPlayerButton, plusPlayerButton, null, newPlayerIdentifier.confirmButton);



            //previous player nav
            ButtonNavSetup(previousPlayer.leftButton, null, previousPlayer.rightButton, newPlayerIdentifier.leftButton, previousPlayer.leftButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.rightButton, previousPlayer.leftButton, previousPlayer.confirmButton, newPlayerIdentifier.rightButton, previousPlayer.rightButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.confirmButton, previousPlayer.rightButton, null, newPlayerIdentifier.confirmButton, previousPlayer.confirmButton.navigation.selectOnUp.GetComponent<Button>());

            currentPlayerSelectionList.Add(newPlayer);

            if (playerSpawnHandler.numberOfPlayers == 5)
            {
                plusPlayerButton.interactable = false;
                ButtonNavSetup(minusPlayerButton, back, null, firstPlayer.leftButton, null);
                return;
            }
            else plusPlayerButton.interactable = true;
            ButtonNavSetup(minusPlayerButton, back, plusPlayerButton, firstPlayer.leftButton, null);
        }

        public void Ready()
        {
            readyPlayers++;

            CheckReady();
        }

        public void CheckReady()
        {
            if (readyPlayers == currentPlayerSelectionList.Count)
            {
                Debug.Log("Here");
                PlayerSelectionIdentifier player = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();
                ButtonNavSetup(player.leftButton, null, player.rightButton, confirmPlayers, player.leftButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.rightButton, player.leftButton, player.confirmButton, confirmPlayers, player.rightButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.confirmButton, player.rightButton, null, confirmPlayers, player.confirmButton.navigation.selectOnUp.GetComponent<Button>());
            }
        }

        public void UnReady()
        {
            readyPlayers--;
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

            GameObject lastPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1];

            playerTypesList.Add(playerTypesListToSpawn[playerTypesListToSpawn.Count - 1]);
            playerTypesListToSpawn.RemoveAt(playerTypesListToSpawn.Count - 1);
            currentPlayerSelectionList.Remove(lastPlayer);
            Destroy(lastPlayer);

            PlayerSelectionIdentifier previousPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 2].GetComponent<PlayerSelectionIdentifier>();

            //previous player nav
            //previous player nav
            ButtonNavSetup(previousPlayer.leftButton, null, previousPlayer.rightButton, null, previousPlayer.leftButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.rightButton, previousPlayer.leftButton, previousPlayer.confirmButton, null, previousPlayer.rightButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.confirmButton, previousPlayer.rightButton, null, null, previousPlayer.confirmButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(confirmPlayers, minusPlayerButton, plusPlayerButton, null, previousPlayer.confirmButton);

            if (playerSpawnHandler.numberOfPlayers == 3)
            {
                minusPlayerButton.interactable = false;
                ButtonNavSetup(plusPlayerButton, back, null, firstPlayer.rightButton, null);
                ButtonNavSetup(back, null, plusPlayerButton, null, null);
                return;
            }
            else minusPlayerButton.interactable = true;
            ButtonNavSetup(plusPlayerButton, minusPlayerButton, null, firstPlayer.rightButton, null);
            ButtonNavSetup(back, null, minusPlayerButton, null, null);

            CheckReady();
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
