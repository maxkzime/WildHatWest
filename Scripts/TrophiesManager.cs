using System;
using UnityEngine;

public class TrophiesManager : MonoBehaviour, ISaveable
{
    #region /*** Initialize trophies manager instance (always alive). ***/
    public static TrophiesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(target: gameObject);
        }
        else
        {
            Destroy(obj: gameObject);
        }

        // Reset trophies when new game starts.
        ResetTrophies();
    }

    #endregion /*** ^^^ Initialize trophies manager instance (always alive). ^^^ ***/



    #region /*** Desactivate all trophies. ***/
    private void ResetTrophies()
    {
        bossTrophy.SetActive(false);
        bisonTrophy.SetActive(false);
        opponentTrophy.SetActive(false);
        indianTrophy.SetActive(false);
        cactusTrophy.SetActive(false);
        chickenTrophy.SetActive(false);
        allUpgradesTrophy.SetActive(false);
        minHatBossTrophy.SetActive(false);

    }
    #endregion /*** ^^^ Desactivate all trophies. ^^^ ***/



    #region /*** Update all trophies. ***/
    private bool bossTrophyVisibility = false;
    private bool bisonTrophyVisibility = false;
    private bool opponentTrophyVisibility = false;
    private bool indianTrophyVisibility = false;
    private bool cactusTrophyVisibility = false;
    private bool chickenTrophyVisibility = false;
    private bool allUpgradesTrophyVisibility = false;
    private bool minHatBossTrophyVisibility = false;

    private void UpdateTrophies()
    {
        bossTrophy.SetActive(bossTrophyVisibility);
        bisonTrophy.SetActive(bisonTrophyVisibility);
        opponentTrophy.SetActive(opponentTrophyVisibility);
        indianTrophy.SetActive(indianTrophyVisibility);
        cactusTrophy.SetActive(cactusTrophyVisibility);
        chickenTrophy.SetActive(chickenTrophyVisibility);
        allUpgradesTrophy.SetActive(allUpgradesTrophyVisibility);
        minHatBossTrophy.SetActive(minHatBossTrophyVisibility);
    }
    #endregion /*** ^^^ Update all trophies. ^^^ ***/




    #region /*** Play trophy unlocked sound effect. ***/
    [SerializeField] private AudioClip trophyUnlockedSound;

    private void PlayTrophyUnlockedSoundEffect()
    {
        SoundManager.Instance.PlaySound(trophyUnlockedSound);
    }

    #endregion /*** ^^^ Play trophy unlocked sound effect. ^^^ ***/



    [Header("Trophies :")]



    #region /*** Boss trophy mechanic. ***/
    [SerializeField] private GameObject bossTrophy;
    [SerializeField] private GameObject minHatBossTrophy;

    private bool hasUnlockedBossTrophy = false;
    public void AddBeatBossOnceTrophy()
    {
        if (!hasUnlockedBossTrophy)
        {
            hasUnlockedBossTrophy = true;

            if (ObjectsGeneration.Instance.playerCurrentMaxHats < 4)
            {
                minHatBossTrophyVisibility = true;

                minHatBossTrophy.SetActive(minHatBossTrophyVisibility);
            }

            PlayTrophyUnlockedSoundEffect();

            bossTrophyVisibility = true;

            bossTrophy.SetActive(bossTrophyVisibility);
        }
    }
    #endregion /*** ^^^ Boss trophy mechanic. ^^^ ***/



    #region /*** Bison trophy mechanic. ***/
    [SerializeField] private GameObject bisonTrophy;
    public void AddBeatFiftyBisonTrophy()
    {
        PlayTrophyUnlockedSoundEffect();

        bisonTrophyVisibility = true;

        bisonTrophy.SetActive(bisonTrophyVisibility);
    }
    #endregion /*** ^^^ Bison trophy mechanic. ^^^ ***/



    #region /*** Opponents trophy mechanic. ***/
    [SerializeField] private GameObject opponentTrophy;

    private bool hasUnlockedOpponentTrophy = false;
    public void AddBeatThousandOpponentsTrophy()
    {
        if (!hasUnlockedOpponentTrophy)
        {
            hasUnlockedOpponentTrophy = true;

            PlayTrophyUnlockedSoundEffect();

            opponentTrophyVisibility = true;

            opponentTrophy.SetActive(opponentTrophyVisibility);
        }
    }

    #endregion /*** ^^^ Opponents trophy mechanic. ^^^ ***/



    #region /*** Indian trophy mechanic. ***/
    [SerializeField] private GameObject indianTrophy;

    private bool hasUnlockedIndianTrophy = false;
    public void AddAllIndianUnlockedTrophy()
    {
        if (!hasUnlockedIndianTrophy)
        {
            hasUnlockedIndianTrophy = true;

            PlayTrophyUnlockedSoundEffect();

            indianTrophyVisibility = true;

            indianTrophy.SetActive(indianTrophyVisibility);
        }
    }
    #endregion /*** ^^^ Indian trophy mechanic. ^^^ ***/



    #region /*** Cactus trophy mechanic. ***/
    [SerializeField] private GameObject cactusTrophy;

    private int cactusKilledCounter = 0;
    private bool hasUnlockedCactusTrophy = false;

    public void IncrementCactusKilledCounter()
    {
        cactusKilledCounter++;

        if (cactusKilledCounter > 200 && !hasUnlockedCactusTrophy)
        {
            AddTwoHundredCactusTrophy();
        }
    }

    private void AddTwoHundredCactusTrophy()
    {
        hasUnlockedCactusTrophy = true;

        PlayTrophyUnlockedSoundEffect();

        cactusTrophyVisibility = true;

        cactusTrophy.SetActive(cactusTrophyVisibility);
    }
    #endregion /*** ^^^ Cactus trophy mechanic. ^^^ ***/



    #region /*** Chicken trophy mechanic. ***/
    [SerializeField] private GameObject chickenTrophy;

    private int chickenKilledCounter = 0;
    private bool hasUnlockedChickenTrophy = false;

    public void IncrementChickenKilledCounter()
    {
        chickenKilledCounter++;

        if (chickenKilledCounter >= 100 && !hasUnlockedChickenTrophy)
        {
            AddTwoHundredChickenTrophy();
        }
    }


    private void AddTwoHundredChickenTrophy()
    {
        hasUnlockedChickenTrophy = true;

        PlayTrophyUnlockedSoundEffect();

        chickenTrophyVisibility = true;

        chickenTrophy.SetActive(chickenTrophyVisibility);
    }

    #endregion /*** ^^^ Chicken trophy mechanic. ^^^ ***/



    #region /*** All uprades trophy mechanic. ***/
    [SerializeField] private GameObject allUpgradesTrophy;

    [SerializeField] private int upgradesCounter = 0;

    public void IncrementAllUpgradesCounter()
    {
        upgradesCounter++;

        if (upgradesCounter >= 6)
        {
            AddAllUpgradesTrophy();
        }
    }


    private void AddAllUpgradesTrophy()
    {
        PlayTrophyUnlockedSoundEffect();

        allUpgradesTrophyVisibility = true;

        allUpgradesTrophy.SetActive(allUpgradesTrophyVisibility);
    }

    #endregion /*** ^^^ All uprades trophy mechanic. ^^^ ***/




    #region /*** Save data in new struct and return it to save in file. ***/
    public object SaveState()
    {
        return new SaveData()
        {
            upgradesCounter = this.upgradesCounter,

            bossTrophyVisibility = this.bossTrophyVisibility,
            bisonTrophyVisibility = this.bisonTrophyVisibility,
            opponentTrophyVisibility = this.opponentTrophyVisibility,
            indianTrophyVisibility = this.indianTrophyVisibility,
            cactusTrophyVisibility = this.cactusTrophyVisibility,
            chickenTrophyVisibility = this.chickenTrophyVisibility,
            allUpgradesTrophyVisibility = this.allUpgradesTrophyVisibility,
            minHatBossTrophyVisibility = this.minHatBossTrophyVisibility
        };
    }
    #endregion /*** ^^^ Save data in new struct and return it to save in file. ^^^ ***/



    #region /*** Load saved data and update trophies. ***/
    public void LoadState(object state)
    {
        var saveData = (SaveData)state;

        upgradesCounter = saveData.upgradesCounter;

        bossTrophyVisibility = saveData.bossTrophyVisibility;
        bisonTrophyVisibility = saveData.bisonTrophyVisibility;
        opponentTrophyVisibility = saveData.opponentTrophyVisibility;
        indianTrophyVisibility = saveData.indianTrophyVisibility;
        cactusTrophyVisibility = saveData.cactusTrophyVisibility;
        chickenTrophyVisibility = saveData.chickenTrophyVisibility;
        allUpgradesTrophyVisibility = saveData.allUpgradesTrophyVisibility;
        minHatBossTrophyVisibility = saveData.minHatBossTrophyVisibility;

        // Update trophies visibility after loading saved visibilities.
        UpdateTrophies();
    }
    #endregion /*** Load saved data and update trophies. ***/


    #region /*** Struct storing to save data. ***/
    [Serializable]
    private struct SaveData
    {
        public int upgradesCounter;

        public bool bossTrophyVisibility;
        public bool bisonTrophyVisibility;
        public bool opponentTrophyVisibility;
        public bool indianTrophyVisibility;
        public bool cactusTrophyVisibility;
        public bool chickenTrophyVisibility;
        public bool allUpgradesTrophyVisibility;
        public bool minHatBossTrophyVisibility;
    }
    #endregion /*** ^^^ Struct storing to save data. ^^^ ***/
}