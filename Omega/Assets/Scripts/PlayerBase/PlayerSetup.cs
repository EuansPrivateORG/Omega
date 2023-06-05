using Omega.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Omega.Status;
using Omega.Visual;
using Unity.VisualScripting;

namespace Omega.Core
{
    public class PlayerSetup : MonoBehaviour
    {
        public AudioClip DRUp;
        public AudioClip DRDown;
        public AudioClip ShieldUp;
        public AudioClip ShieldDown;

        public Base playerBase;

        [HideInInspector] public GameObject playerPreFab;

        public int playerID = 0;

        private PlayerSpawnHandler playerSpawnHandler;
        public Transform diceTarget;
        public Transform diceSpawn;

        [HideInInspector] public GameObject icon;

        private GameObject playerShield;
        private GameObject playerDamageReduction;

        [HideInInspector] public int amountOfRoundsDOT;
        [HideInInspector] public int amountOfRoundsHOT;
        public int amountOfRoundsShield = 0;
        [HideInInspector] public int amountOfRoundsStun;
        [HideInInspector] 
        public int amountOfRoundsEOT;

        PlayerIdentifier playerIdentifier;

        private void Awake()
        {
            playerIdentifier = FindObjectOfType<PlayerIdentifier>();
            playerSpawnHandler = FindObjectOfType<PlayerSpawnHandler>();
        }

        void Start()
        {
            playerPreFab = playerBase.baseVarientPrefabList[Random.Range(0, playerBase.baseVarientPrefabList.Count)];
            GameObject instantiated = Instantiate(playerPreFab, transform);

            MeshRenderer playerMesh = instantiated.GetComponent<MeshRenderer>();
            playerMesh.material = playerBase.materialVarientOverrite;

            BaseCollection baseCollection = instantiated.GetComponent<BaseCollection>();

            foreach(Transform child in baseCollection.basePiecesParent.transform)
            {
                if(child.TryGetComponent<Renderer>(out Renderer childRen))
                {
                    childRen.material = playerBase.baseMaterialVarientOverrite;
                }
            }

            baseCollection.destroyedParent.SetActive(true);

            foreach (Transform child in baseCollection.destroyedBasePiecesParent.transform)
            {
                if (child.TryGetComponent<Renderer>(out Renderer childRen))
                {
                    childRen.material = playerBase.baseMaterialVarientOverrite;
                }
            }

            baseCollection.destroyedParent.SetActive(false);

            foreach (Transform child in baseCollection.emissivePiecesParent.transform)
            {
                if (child.TryGetComponent<Renderer>(out Renderer childRen))
                {
                    childRen.material = playerBase.emissiveMaterialVarientOverrite;
                }
            }

            foreach (GameObject emissive in baseCollection.emissiveGunPartsList)
            {
                emissive.GetComponent<Renderer>().material = playerBase.emissiveMaterialVarientOverrite;
            }

            Transform iconSpawnPosition = baseCollection.iconParent.gameObject.transform;
            GameObject faction3DIcon = Instantiate(playerBase.faction3DIcon, iconSpawnPosition);
            foreach (Transform child in faction3DIcon.transform)
            {
                child.GetComponent<MeshRenderer>().material = playerBase.emissiveMaterialVarientOverrite;
            }

        }

        public void SetPlayerIconDead()
        {
            if (playerIdentifier.currentlyAlivePlayers.Count > 1)
            {
                PlayerIconID playerIconID = icon.GetComponent<PlayerIconID>();
                playerIconID.iconBackground.SetActive(false);
                playerIconID.destroyedIconBackground.SetActive(true);
                playerIconID.playerIcon.GetComponent<Image>().color = playerIconID.destroyedIconBackground.GetComponent<Image>().color;
                playerSpawnHandler.playerImageList.Remove(icon);
            }
        }

        public void UpdatePlayerID()
        {
            playerID -= 1;
        }

        public void ActivateShield(GameObject shield)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = ShieldUp;
            audioSource.Play();
            playerShield = Instantiate(shield, transform);
        }

        public void ActivateDamageReduction(GameObject shield)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = DRUp;
            audioSource.Play();
            playerDamageReduction = Instantiate(shield, transform);
        }

        public void DeActivateShield()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = ShieldDown;
            audioSource.Play();
            Destroy(playerShield);
        }

        public void DeDamageReduction()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = DRDown;
            audioSource.Play();
            Destroy(playerDamageReduction);
        }
    }
}