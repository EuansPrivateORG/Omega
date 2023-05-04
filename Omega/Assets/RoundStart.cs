using Omega.Core;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Omega.UI
{
    public class RoundStart : MonoBehaviour
    {
        public List<Base> playerTypesList = new List<Base>();

        [SerializeField] TextMeshProUGUI numberOfPlayers;
        [SerializeField] Button startRoundButton;

        [Header("Faction Icons")]
        [SerializeField] GameObject playerFactionIconPrefab;
        [SerializeField] RectTransform iconSpawnPosition;

        [Header("PlayerCounter Buttons")]
        [SerializeField] Button minusButton;
        [SerializeField] Button plusButton;
        public List<GameObject> iconList = new List<GameObject>();
        public CanvasGroup GameHUD;

        PlayerSpawnHandler playerSpawnHandler;
        EventSystem eventSystem;

        public List<Base> playerTypesListToSpawn = new List<Base>();


        private void Awake()
        {
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
                GameObject instantiatedIcon = Instantiate(playerTypesList[ran].turnOrderVarientIcon, iconSpawnPosition);
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

            if(eventSystem.currentSelectedGameObject == null)
            {
                if(plusButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(minusButton.gameObject);
                }
                else if (minusButton.interactable == false)
                {
                    eventSystem.SetSelectedGameObject(plusButton.gameObject);
                }
                else eventSystem.SetSelectedGameObject(startRoundButton.gameObject);
            }
            if (playerSpawnHandler.numberOfPlayers == 6)
            {
                plusButton.interactable = false;
                return;
            }
            else plusButton.interactable = true;

            if (playerSpawnHandler.numberOfPlayers == 3)
            {
                minusButton.interactable = false;

                return;
            }
            else minusButton.interactable = true;
        }
        public void addPlayer()
        {

            playerSpawnHandler.numberOfPlayers++;
            int ran = Random.Range(0, playerTypesList.Count);
            GameObject instantiatedIcon = Instantiate(playerTypesList[ran].turnOrderVarientIcon, iconSpawnPosition);
            RectTransform iconRect = instantiatedIcon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(50, 50);
            playerTypesListToSpawn.Add(playerTypesList[ran]);
            playerTypesList.RemoveAt(ran);
            iconList.Add(instantiatedIcon);
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
            playerSpawnHandler.SpawnPlayers(playerTypesListToSpawn);

            GameHUD.alpha = 1;
            GameHUD.interactable = true;

            CanvasGroup canvas = GetComponent<CanvasGroup>();
            canvas.alpha = 0;
            canvas.interactable = false;
        }
    }
}
