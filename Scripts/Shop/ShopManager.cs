using UnityEngine;
using TMPro;
using System;

public class ShopManager : MonoBehaviour, ISaveable
{

    #region /*** Hat Upgrade. ***/
    [Header("Hat Upgrade : ")]
    [SerializeField] private TMP_Text hatUpgradePriceText;

    [SerializeField] private GameObject hatUpgradeCurrency;

    private int currentHatPrice = 5;

    public void BuyingHat()
    {
        // Upgrade limit.
        if (currentHatPrice >= 100)
        {
            hatUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (hatUpgradeCurrency != null)
            {
                Destroy(hatUpgradeCurrency);

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentHatPrice))
            {
                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                // Increasing price.
                currentHatPrice += 5;

                // Updating price tag.
                hatUpgradePriceText.text = currentHatPrice.ToString();

                // Increasing player hat number.
                generationScript.playerCurrentMaxHats++;

            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }
    #endregion /*** ^^^ Hat Upgrade. ^^^ ***/



    #region /*** Magnet Upgrade. ***/
    [Header("Magnet Upgrade : ")]
    [SerializeField] private TMP_Text magnetUpgradePriceText;

    [SerializeField] private GameObject magnetUpgradeCurrency;

    private int currentMagnetPrice = 25;

    public void BuyingMagnet()
    {
        // Upgrade limit.
        if (currentMagnetPrice >= 150)
        {
            magnetUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (magnetUpgradeCurrency != null)
            {
                Destroy(magnetUpgradeCurrency);

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            // If player can pay = pay.
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentMagnetPrice))
            {
                // If he hasnt unlocked magnet then unlock it.
                if (!generationScript.HasUnlockedMagnet)
                {
                    generationScript.HasUnlockedMagnet = true;
                }
                // Else upgrade the magnet.
                else
                {
                    // Increasing magnet level.
                    generationScript.MagnetLevel += 0.5f;
                }

                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                // Increasing price.
                currentMagnetPrice += 25;

                // Updating price tag.
                magnetUpgradePriceText.text = currentMagnetPrice.ToString();
            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }
    #endregion /*** ^^^ Magnet Upgrade. ^^^ ***/



    #region /*** Floor size Upgrade. ***/
    [Header("Floor size Upgrade : ")]
    [SerializeField] private TMP_Text floorSizeUpgradePriceText;

    [SerializeField] private GameObject floorSizeUpgradeCurrency;

    private int currentFloorSizePrice = 30;

    public void BuyingFloorSize()
    {
        // Upgrade limit.
        if (currentFloorSizePrice >= 120)
        {
            floorSizeUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (floorSizeUpgradeCurrency != null)
            {
                Destroy(floorSizeUpgradeCurrency);

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            // If player can pay = pay.
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentFloorSizePrice))
            {
                // Upgrade floor size.
                generationScript.FloorScaleFactor++;

                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                // Increasing price.
                currentFloorSizePrice += 30;

                // Updating price tag.
                floorSizeUpgradePriceText.text = currentFloorSizePrice.ToString();
            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }
    #endregion /*** ^^^ Floor size Upgrade. ^^^ ***/



    #region /*** Double gun Upgrade. ***/
    [Header("Double Gun Upgrade : ")]
    [SerializeField] private TMP_Text doubleGunUpgradePriceText;

    [SerializeField] private GameObject doubleGunUpgradeCurrency;

    private int currentDoubleGunPrice = 250;

    public void BuyingDoubleGun()
    {
        // Upgrade limit.
        if (generationScript.HasUnlockedDoubleGun)
        {
            doubleGunUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (doubleGunUpgradeCurrency != null)
            {
                Destroy(doubleGunUpgradeCurrency);

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            // If player can pay = pay.
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentDoubleGunPrice))
            {
                generationScript.HasUnlockedDoubleGun = true;

                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                doubleGunUpgradePriceText.text = "MAX";
            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }
    #endregion /*** ^^^ Double gun Upgrade. ^^^ ***/



    #region /*** Shotgun Upgrade. ***/
    [Header("Shotgun Upgrade : ")]
    [SerializeField] private TMP_Text shotgunUpgradePriceText;

    [SerializeField] private GameObject shotgunUpgradeCurrency;

    private int currentShotgunPrice = 300;

    public void BuyingShotgun()
    {
        if (generationScript.HasUnlockedShotgun &&
            generationScript.ShotgunReloadAmount > 10)
        {
            shotgunUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (shotgunUpgradeCurrency != null)
            {
                Destroy(shotgunUpgradeCurrency);

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            // If player can pay = pay.
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentShotgunPrice))
            {
                // If he hasn't unlocked shotgun, unlock it.
                if (!generationScript.HasUnlockedShotgun)
                {
                    generationScript.HasUnlockedShotgun = true;

                }
                // Else increase reload amount.
                else
                {
                    // If max reload amount, made it max.
                    if (generationScript.ShotgunReloadAmount > 5)
                    {
                        generationScript.ShotgunReloadAmount = 100000;
                    }
                    else
                    {
                        generationScript.ShotgunReloadAmount++;
                    }
                }

                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                // Increasing price.
                currentShotgunPrice += 50;

                // Updating price tag.
                shotgunUpgradePriceText.text = currentShotgunPrice.ToString();
            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }
    #endregion /*** ^^^ Shotgun Upgrade. ^^^ ***/



    #region /*** Indian Upgrade. ***/
    [Header("Indian Upgrade : ")]
    [SerializeField] private TMP_Text indianUpgradePriceText;

    [SerializeField] private GameObject indianUpgradeCurrency;

    private int currentIndianPrice = 50;

    public void BuyingIndian()
    {
        if (generationScript.HasUnlockedIndian && generationScript.IndianNumber > 25)
        {
            indianUpgradePriceText.text = "MAX";

            SoundManager.Instance.PlayLockedButtonSoundEffect();

            if (indianUpgradeCurrency != null)
            {
                Destroy(indianUpgradeCurrency);

                // Unlock "all indian unlocked" trophy.
                TrophiesManager.Instance.AddAllIndianUnlockedTrophy();

                // Increment upgrades unlocked counter for "All upgrades unlocked" trophy.
                TrophiesManager.Instance.IncrementAllUpgradesCounter();
            }
        }
        else
        {
            // If player can pay = pay.
            if (generationScript.EditCurrentPlayerGoldNuggetBalance(currentIndianPrice))
            {
                // If he hasn't unlocked indian, unlock it.
                if (!generationScript.HasUnlockedIndian)
                {
                    generationScript.HasUnlockedIndian = true;
                }
                // Else increase indian amount and indian's stats.
                else
                {
                    generationScript.IndianNumber++;
                    generationScript.IndianDistanceToOpponentToShoot++;
                }

                SoundManager.Instance.PlaySelectedButtonSoundEffect();

                // Increasing price.
                currentIndianPrice += 25;

                // Updating price tag.
                indianUpgradePriceText.text = currentIndianPrice.ToString();
            }
            #region /*** If player can't play, play locked button sound effect. ***/
            else
            {
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** If player can't play, play locked button sound effect. ***/
        }
    }

    #endregion /*** ^^^ Indian Upgrade. ^^^ ***/



    private ObjectsGeneration generationScript;

    // Start is called before the first frame update
    void Start()
    {
        // Grab reference to object generation manager instance.
        generationScript = ObjectsGeneration.Instance;


        #region /*** Initialize shop price tags. ***/
        hatUpgradePriceText.text = currentHatPrice.ToString();
        magnetUpgradePriceText.text = currentMagnetPrice.ToString();
        floorSizeUpgradePriceText.text = currentFloorSizePrice.ToString();
        doubleGunUpgradePriceText.text = currentDoubleGunPrice.ToString();
        shotgunUpgradePriceText.text = currentShotgunPrice.ToString();
        indianUpgradePriceText.text = currentIndianPrice.ToString();

        #endregion /*** ^^^ Initialize shop price tags. ^^^ ***/
    }




    #region /*** Save and load data for shop system. ***/


    private void UpdateShopWithLoadedData()
    {
        #region /*** Initialize shop price tags. ***/
        /*
        hatUpgradePriceText.text = currentHatPrice.ToString();
        magnetUpgradePriceText.text = currentMagnetPrice.ToString();
        floorSizeUpgradePriceText.text = currentFloorSizePrice.ToString();
        doubleGunUpgradePriceText.text = currentDoubleGunPrice.ToString();
        shotgunUpgradePriceText.text = currentShotgunPrice.ToString();
        indianUpgradePriceText.text = currentIndianPrice.ToString();
        */

        #endregion /*** ^^^ Initialize shop price tags. ^^^ ***/

        #region /*** Clean upgrade currency for the full unlocked upgrades and avoid incrementing all upgrades counter. ***/
        if (indianUpgradePriceText.text == "MAX" && indianUpgradeCurrency != null)
        {
            Destroy(indianUpgradeCurrency);
        }

        if (shotgunUpgradePriceText.text == "MAX" && shotgunUpgradeCurrency != null)
        {
            Destroy(shotgunUpgradeCurrency);
        }

        if (doubleGunUpgradePriceText.text == "MAX" && doubleGunUpgradeCurrency != null)
        {
            Destroy(doubleGunUpgradeCurrency);
        }

        if (floorSizeUpgradePriceText.text == "MAX" && floorSizeUpgradeCurrency != null)
        {
            Destroy(floorSizeUpgradeCurrency);
        }

        if (magnetUpgradePriceText.text == "MAX" && magnetUpgradeCurrency != null)
        {
            Destroy(magnetUpgradeCurrency);
        }

        if (hatUpgradePriceText.text == "MAX" && hatUpgradeCurrency != null)
        {
            Destroy(hatUpgradeCurrency);
        }
        #endregion /*** ^^^ Clean upgrade currency for the full unlocked upgrades and avoid incrementing all upgrades counter. ^^^ ***/
    }


    #region /*** Save data in new struct and return it to save in file. ***/
    public object SaveState()
    {
        return new SaveData()
        {
            hatUpgradePriceText = hatUpgradePriceText.text,
            magnetUpgradePriceText = magnetUpgradePriceText.text,
            floorSizeUpgradePriceText = floorSizeUpgradePriceText.text,
            doubleGunUpgradePriceText = doubleGunUpgradePriceText.text,
            shotgunUpgradePriceText = shotgunUpgradePriceText.text,
            indianUpgradePriceText = indianUpgradePriceText.text,

            currentIndianPrice = currentIndianPrice,
            currentShotgunPrice = currentShotgunPrice,
            currentDoubleGunPrice = currentDoubleGunPrice,
            currentFloorSizePrice = currentFloorSizePrice,
            currentMagnetPrice = currentMagnetPrice,
            currentHatPrice = currentHatPrice
        };

    }
    #endregion /*** ^^^ Save data in new struct and return it to save in file. ^^^ ***/


    #region /*** Load saved data and update shop. ***/
    public void LoadState(object state)
    {
        var saveData = (SaveData)state;

        hatUpgradePriceText.text = saveData.hatUpgradePriceText;
        magnetUpgradePriceText.text = saveData.magnetUpgradePriceText;
        floorSizeUpgradePriceText.text = saveData.floorSizeUpgradePriceText;
        doubleGunUpgradePriceText.text = saveData.doubleGunUpgradePriceText;
        shotgunUpgradePriceText.text = saveData.shotgunUpgradePriceText;
        indianUpgradePriceText.text = saveData.indianUpgradePriceText;

        currentIndianPrice = saveData.currentIndianPrice;
        currentShotgunPrice = saveData.currentShotgunPrice;
        currentDoubleGunPrice = saveData.currentDoubleGunPrice;
        currentFloorSizePrice = saveData.currentFloorSizePrice;
        currentMagnetPrice = saveData.currentMagnetPrice;
        currentHatPrice = saveData.currentHatPrice;


        // Update shop after loading saved data.
        UpdateShopWithLoadedData();
    }
    #endregion /*** Load saved data and update shop. ***/


    #region /*** Struct storing to save data. ***/
    [Serializable]
    private struct SaveData
    {
        public string hatUpgradePriceText;
        public string magnetUpgradePriceText;
        public string floorSizeUpgradePriceText;
        public string doubleGunUpgradePriceText;
        public string shotgunUpgradePriceText;
        public string indianUpgradePriceText;


        public int currentIndianPrice;
        public int currentShotgunPrice;
        public int currentDoubleGunPrice;
        public int currentFloorSizePrice;
        public int currentMagnetPrice;
        public int currentHatPrice;

    }
    #endregion /*** ^^^ Struct storing to save data. ^^^ ***/

    #endregion /*** ^^^ Save and load data for shop system. ^^^ ***/
}
