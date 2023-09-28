using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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
        [SerializeField] Button returnButton;
        [SerializeField] Button startGameButton;

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

        public List<Base> allFactions = new List<Base>();

        [HideInInspector] public bool continueFade = false;

        private void Awake()
        {
            roundHandler = FindObjectOfType<RoundHandler>();
            eventSystem = FindObjectOfType<EventSystem>();
            playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();

            allFactions.AddRange(playerTypesList);
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            playerTypesList.Clear();
            playerTypesListToSpawn.Clear();

            playerTypesList.AddRange(allFactions);

            confirmPlayers.interactable = false;

            readyPlayers = 0;

            if (currentPlayerSelectionList.Count > 0)
            {
                List<GameObject> players = new List<GameObject>();
                players.AddRange(currentPlayerSelectionList);
                for (int i = 0; i < players.Count; i++)
                {
                    Destroy(currentPlayerSelectionList[i]);
                }

                currentPlayerSelectionList.Clear();
            }

            GameHUD.alpha = 0;
            GameHUD.interactable = false;
            for (int i = 0; i < playerSpawnHandler.numberOfPlayers; i++)
            {
                int ranFaction = Random.Range(0, playerTypesList.Count);
                GameObject newPlayer = Instantiate(SelectionTemplatePrefab, CurrentPlayersParent.transform);
                PlayerSelectionIdentifier newPlayerIdentifier = newPlayer.GetComponent<PlayerSelectionIdentifier>();
                newPlayerIdentifier.playerNumber.text = "P" + (i + 1).ToString();
                newPlayerIdentifier.FactionIconImage.sprite = playerTypesList[ranFaction].PlayerSelectionFactionIcon;
                newPlayerIdentifier.FactionIconImage.color = playerTypesList[ranFaction].uiOverriteColor;
                int placeInList = 0;
                for (int l = 0; l < allFactions.Count; l++)
                {
                    if (allFactions[l] == playerTypesList[ranFaction]) placeInList = l;
                }
                newPlayerIdentifier.placeInFactionList = placeInList;

                playerTypesListToSpawn.Add(playerTypesList[ranFaction]);
                playerTypesList.RemoveAt(ranFaction);

                if (currentPlayerSelectionList.Count == 0)
                {
                    ButtonNavSetup(minusPlayerButton, back, plusPlayerButton, newPlayerIdentifier.leftButton, null);

                    ButtonNavSetup(plusPlayerButton, back, null, newPlayerIdentifier.rightButton, null);

                    ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, plusPlayerButton);

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

                        ButtonNavSetup(confirmPlayers, null, null, null, newPlayerIdentifier.confirmButton);
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
            int placeInList = 0;
            for (int i = 0; i < allFactions.Count; i++)
            {
                if (allFactions[i] == playerTypesList[ranFaction]) placeInList = i;
            }
            newPlayerIdentifier.placeInFactionList = placeInList;

            playerTypesListToSpawn.Add(playerTypesList[ranFaction]);
            playerTypesList.RemoveAt(ranFaction);

            PlayerSelectionIdentifier previousPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();

            //new player nav
            ButtonNavSetup(newPlayerIdentifier.leftButton, null, newPlayerIdentifier.rightButton, null, previousPlayer.leftButton);

            ButtonNavSetup(newPlayerIdentifier.rightButton, newPlayerIdentifier.leftButton, newPlayerIdentifier.confirmButton, null, previousPlayer.rightButton);

            ButtonNavSetup(newPlayerIdentifier.confirmButton, newPlayerIdentifier.rightButton, null, null, previousPlayer.confirmButton);

            ButtonNavSetup(confirmPlayers, null, null, null, newPlayerIdentifier.confirmButton);



            //previous player nav
            ButtonNavSetup(previousPlayer.leftButton, null, previousPlayer.rightButton, newPlayerIdentifier.leftButton, previousPlayer.leftButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.rightButton, previousPlayer.leftButton, previousPlayer.confirmButton, newPlayerIdentifier.rightButton, previousPlayer.rightButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.confirmButton, previousPlayer.rightButton, null, newPlayerIdentifier.confirmButton, previousPlayer.confirmButton.navigation.selectOnUp.GetComponent<Button>());

            currentPlayerSelectionList.Add(newPlayer);

            if (playerSpawnHandler.numberOfPlayers == 5)
            {
                plusPlayerButton.interactable = false;
                minusPlayerButton.interactable = true;
                ButtonNavSetup(minusPlayerButton, back, null, firstPlayer.leftButton, null);
                ButtonNavSetup(back, null, minusPlayerButton, null, null);
                ButtonNavSetup(firstPlayer.rightButton, firstPlayer.leftButton, firstPlayer.confirmButton, firstPlayer.rightButton.navigation.selectOnDown.GetComponent<Button>(), minusPlayerButton);
                ButtonNavSetup(firstPlayer.confirmButton, null, firstPlayer.rightButton, firstPlayer.confirmButton.navigation.selectOnDown.GetComponent<Button>(), minusPlayerButton);
                Debug.Log("Only Minus");
            }
            else
            {
                plusPlayerButton.interactable = true;
                minusPlayerButton.interactable = true;
                ButtonNavSetup(minusPlayerButton, back, plusPlayerButton, firstPlayer.leftButton, null);
                ButtonNavSetup(plusPlayerButton, minusPlayerButton, null, firstPlayer.rightButton, null);
                ButtonNavSetup(back, null, minusPlayerButton, null, null);
                ButtonNavSetup(firstPlayer.rightButton, firstPlayer.leftButton, firstPlayer.confirmButton, firstPlayer.rightButton.navigation.selectOnDown.GetComponent<Button>(), plusPlayerButton);
                ButtonNavSetup(firstPlayer.confirmButton, null, firstPlayer.rightButton, firstPlayer.confirmButton.navigation.selectOnDown.GetComponent<Button>(), plusPlayerButton);
                ButtonNavSetup(firstPlayer.leftButton, null, firstPlayer.rightButton, firstPlayer.leftButton.navigation.selectOnDown.GetComponent<Button>(), minusPlayerButton);
                Debug.Log("Free");
            }

            CheckReady();
        }

        //public void RefreshFactions()
        //{
        //    List<Base> allFactions = new List<Base>();
        //    allFactions.AddRange(playerTypesList);

        //    inactiveFactions.AddRange(allFactions);

        //    for (int i = 0; i < allFactions.Count; i++)
        //    {
        //        if (inactiveFactions[i] == allFactions[i])
        //        {
        //            inactiveFactions.RemoveAt(i);
        //        }
        //    }
        //}

        public void UpFaction(PlayerSelectionIdentifier newPlayerIdentifier, int placeInFactionList)
        {
            Base newFaction = null;
            Base oldFaction = allFactions[placeInFactionList];
            int newPlaceInList = 0;

            for (int i = placeInFactionList; i < allFactions.Count; i++)
            {
                if (playerTypesListToSpawn.Contains(allFactions[i]))
                {
                    if (i == allFactions.Count - 1) i = -1;
                    continue;
                }
                else
                {
                    newFaction = allFactions[i];
                    newPlaceInList = i;
                    break;
                }
            }

            newPlayerIdentifier.FactionIconImage.sprite = newFaction.PlayerSelectionFactionIcon;
            newPlayerIdentifier.FactionIconImage.color = newFaction.uiOverriteColor;

            playerTypesListToSpawn.Add(newFaction);
            playerTypesList.Remove(newFaction);

            playerTypesListToSpawn.Remove(oldFaction);
            playerTypesList.Add(oldFaction);

            newPlayerIdentifier.placeInFactionList = newPlaceInList;
        }

        public void DownFaction(PlayerSelectionIdentifier newPlayerIdentifier, int placeInFactionList)
        {
            Base newFaction = null;
            Base oldFaction = allFactions[placeInFactionList];

            int newPlaceInList = 0;

            for (int i = placeInFactionList; i > -1; i--)
            {
                if (playerTypesListToSpawn.Contains(allFactions[i]))
                {
                    if (i == 0) i = allFactions.Count;
                    continue;
                }
                else
                {
                    newFaction = allFactions[i];
                    newPlaceInList = i;
                    break;
                }
            }

            newPlayerIdentifier.FactionIconImage.sprite = newFaction.PlayerSelectionFactionIcon;
            newPlayerIdentifier.FactionIconImage.color = newFaction.uiOverriteColor;

            playerTypesListToSpawn.Add(newFaction);
            playerTypesList.Remove(newFaction);

            playerTypesListToSpawn.Remove(oldFaction);
            playerTypesList.Add(oldFaction);

            newPlayerIdentifier.placeInFactionList = newPlaceInList;
        }

        public void Ready()
        {
            readyPlayers++;

            CheckReady();
        }

        public void UnReadyAll()
        {
            for (int i = 0; i < currentPlayerSelectionList.Count; i++)
            {
                currentPlayerSelectionList[i].GetComponentInChildren<ReadyButton>().UnReady();
            }
        }

        public void CheckReady()
        {
            PlayerSelectionIdentifier player = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();

            if (readyPlayers == currentPlayerSelectionList.Count)
            {
                ButtonNavSetup(player.leftButton, null, player.rightButton, confirmPlayers, player.leftButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.rightButton, player.leftButton, player.confirmButton, confirmPlayers, player.rightButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.confirmButton, player.rightButton, null, confirmPlayers, player.confirmButton.navigation.selectOnUp.GetComponent<Button>());

                confirmPlayers.interactable = true;
            }
            else
            {
                ButtonNavSetup(player.leftButton, null, player.rightButton, null, player.leftButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.rightButton, player.leftButton, player.confirmButton, null, player.rightButton.navigation.selectOnUp.GetComponent<Button>());

                ButtonNavSetup(player.confirmButton, player.rightButton, null, null, player.confirmButton.navigation.selectOnUp.GetComponent<Button>());

                confirmPlayers.interactable = false;
            }
        }

        public void UnReady()
        {
            readyPlayers--;

            CheckReady();
        }

        public void AddRound()
        {
            roundHandler.numOfRounds++;

            if (roundHandler.numOfRounds == 9)
            {
                plusRoundsButton.interactable = false;
                minusRoundsButton.interactable = true;

                eventSystem.SetSelectedGameObject(minusRoundsButton.gameObject);

                ButtonNavSetup(minusPlayerButton, returnButton, null, startGameButton, null);
                ButtonNavSetup(startGameButton, returnButton, null, null, minusRoundsButton);
            }
            else
            {
                plusRoundsButton.interactable = true;
                minusRoundsButton.interactable = true;

                ButtonNavSetup(plusRoundsButton, minusRoundsButton, null, startGameButton, null);
                ButtonNavSetup(minusRoundsButton, returnButton, plusRoundsButton, startGameButton, null);
                ButtonNavSetup(returnButton, null, minusRoundsButton, startGameButton, null);
            }
        }
        public void RemoveRound()
        {
            roundHandler.numOfRounds--;

            if (roundHandler.numOfRounds == 1)
            {
                minusRoundsButton.interactable = false;
                plusRoundsButton.interactable = true;

                eventSystem.SetSelectedGameObject(plusRoundsButton.gameObject);

                ButtonNavSetup(plusRoundsButton, returnButton, null, startGameButton, null);
                ButtonNavSetup(returnButton, null, plusRoundsButton, startGameButton, null);
            }
            else
            {
                plusRoundsButton.interactable = true;
                minusRoundsButton.interactable = true;

                ButtonNavSetup(plusRoundsButton, minusRoundsButton, null, startGameButton, null);
                ButtonNavSetup(minusRoundsButton, returnButton, plusRoundsButton, startGameButton, null);
                ButtonNavSetup(startGameButton, returnButton, null, null, plusRoundsButton);
            }
        }

        public void removePlayer()
        {
            playerSpawnHandler.numberOfPlayers--;

            GameObject lastPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1];

            if (lastPlayer.GetComponentInChildren<ReadyButton>().isReady)
            {
                readyPlayers--;
            }

            Base oldFaction = allFactions[lastPlayer.GetComponent<PlayerSelectionIdentifier>().placeInFactionList];

            playerTypesList.Add(oldFaction);
            playerTypesListToSpawn.Remove(oldFaction);
            currentPlayerSelectionList.Remove(lastPlayer);
            Destroy(lastPlayer);

            PlayerSelectionIdentifier previousPlayer = currentPlayerSelectionList[currentPlayerSelectionList.Count - 1].GetComponent<PlayerSelectionIdentifier>();

            //previous player nav
            //previous player nav
            ButtonNavSetup(previousPlayer.leftButton, null, previousPlayer.rightButton, null, previousPlayer.leftButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.rightButton, previousPlayer.leftButton, previousPlayer.confirmButton, null, previousPlayer.rightButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(previousPlayer.confirmButton, previousPlayer.rightButton, null, null, previousPlayer.confirmButton.navigation.selectOnUp.GetComponent<Button>());

            ButtonNavSetup(confirmPlayers, null, null, null, previousPlayer.confirmButton);

            if (playerSpawnHandler.numberOfPlayers == 3)
            {
                minusPlayerButton.interactable = false;
                plusPlayerButton.interactable = true;
                ButtonNavSetup(plusPlayerButton, back, null, firstPlayer.rightButton, null);
                ButtonNavSetup(back, null, plusPlayerButton, null, null);
                ButtonNavSetup(firstPlayer.leftButton, null, firstPlayer.rightButton, firstPlayer.leftButton.navigation.selectOnDown.GetComponent<Button>(), plusPlayerButton);
                Debug.Log("Only Plus");
            }
            else
            {
                Debug.Log("Free");
                minusPlayerButton.interactable = true;
                plusPlayerButton.interactable = true;
                ButtonNavSetup(plusPlayerButton, minusPlayerButton, null, firstPlayer.rightButton, null);
                ButtonNavSetup(minusPlayerButton, back, plusPlayerButton, firstPlayer.leftButton, null);
                ButtonNavSetup(back, null, minusPlayerButton, null, null);
                ButtonNavSetup(firstPlayer.leftButton, null, firstPlayer.rightButton, firstPlayer.leftButton.navigation.selectOnDown.GetComponent<Button>(), minusPlayerButton);
                ButtonNavSetup(firstPlayer.rightButton, firstPlayer.leftButton, firstPlayer.confirmButton, firstPlayer.rightButton.navigation.selectOnDown.GetComponent<Button>(), plusPlayerButton);
                ButtonNavSetup(firstPlayer.confirmButton, firstPlayer.rightButton, null, firstPlayer.confirmButton.navigation.selectOnDown.GetComponent<Button>(), plusPlayerButton);
            }

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
