using System;
using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;

public class PlayerStats : MonoBehaviour
{
    private ObjectsGeneration generatorGenerationScript;

    private void Start()
    {
        generatorGenerationScript = ObjectsGeneration.Instance;

        InitializeHatsOnPlayerAsync();
    }


    #region /*** Hats initialization. ***/
    private GameObject[] hatsArr;

    private int maxHats;

    private int currentHatNum = 0;

    private bool hatsAreInitialized = false;

    private async void InitializeHatsOnPlayerAsync()
    {
        maxHats = generatorGenerationScript.playerCurrentMaxHats;

        float firstYOffset = 0.5f;

        Tuple<GameObject[], int> hatsData = await
            HatsManager.Instance.InitializeHatsForEntity(
            maxHats,
            firstYOffset,
            gameObject,
            true
            );

        hatsArr = hatsData.Item1;
        currentHatNum = hatsData.Item2;

        hatsAreInitialized = true;
    }
    #endregion /*** ^^^ Hats initialization. ^^^ ***/



    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (hatsAreInitialized)
        {
            if (collider.gameObject.CompareTag("Opponent_bullet"))
            {
                Destroy(collider.GetComponent<BoxCollider2D>());
                collider.GetComponent<BulletLife>().InitBulletDeath();

                PlayerTakeDamage();
            }
            else if (collider.gameObject.CompareTag("Egg_bullet"))
            {
                Destroy(collider.GetComponent<BoxCollider2D>());
                collider.GetComponent<EggBulletLife>().ForcedKill();

                PlayerTakeDamage();
            }
            else if (collider.gameObject.CompareTag("GoldNugget"))
            {
                Destroy(collider.gameObject);

                // Playing collecting gold nugget sound effect.
                SoundManager.Instance.PlayCollectingGoldNuggetSoundEffect();

                // Update Gold Nugget counter.
                generatorGenerationScript.IncrementCurrentPlayerGoldNuggetBalance();
            }
        }
    }



    #region /*** Player taking damage mechanics. ***/
    private bool isDeath = false;

    public bool GetIsDeath()
    {
        return isDeath;
    }


    // TODO: add invicible frame after being hit to avoid hit wave.
    private async void PlayerTakeDamage()
    {
        if (!GetIsDeath())
        {
            // Shake effect when taking damage.
            Shaking.Instance.ShakeCamera(
                intensity: UnityEngine.Random.Range(6.0f, 8.75f),
                duration: UnityEngine.Random.Range(0.35f, 0.55f));


            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();

            // Play bullet hitting flesh sound effect.
            SoundManager.Instance.PlayRandomBulletHittingFleshSound();

            // Actions if player is hit by opponent's bullet.
            if (currentHatNum > 0)
            {
                // Send hat to hat manager to drop it on the floor, and then destroy it.
                HatsManager.Instance.AddHatToDropOnFloor(hatsArr[currentHatNum - 1], gameObject);

                currentHatNum--;

                CinematicBarsManager();
            }


            if (currentHatNum <= 0)
            {
                isDeath = true;

                GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                // Stop heart beat.
                SoundManager.Instance.TriggerHeartBeatLoopSoundEffect();

                SoundManager.Instance.PlayRandomPlayerDeathSound();

                await Task.Delay(3 * 1000);

                Destroy(GetComponent<Collider2D>());


                SoundManager.Instance.StopPlayerWalkingSoundEffect();

                CinematicBarsManager();

                // Clean footprints parent to avoid trash.
                Destroy(GetComponent<PlayerMovement>().footprintsEmpty);

                // Display shop when the player die.
                generatorGenerationScript.GenerateShop();
            }
        }
    }
    #endregion /*** ^^^ Player taking damage mechanics. ^^^ ***/




    #region /*** Cinematic bars animator. ***/
    private Animator cinematicBarsAnimator = null;

    private void CinematicBarsManager()
    {
        // Grab cinematic bars animator.
        if (cinematicBarsAnimator == null)
        {
            cinematicBarsAnimator = GameObject.FindGameObjectWithTag("CinematicBars").GetComponent<Animator>();
        }

        if (cinematicBarsAnimator != null)
        {
            if (currentHatNum == 2)
            {
                cinematicBarsAnimator.SetTrigger("FirstLevelBarsShowing");
            }
            else if (currentHatNum == 1)
            {
                // Start heart beat.
                SoundManager.Instance.TriggerHeartBeatLoopSoundEffect();

                cinematicBarsAnimator.SetTrigger("SecondLevelBarsShowing");
            }
            else if (currentHatNum <= 0)
            {
                cinematicBarsAnimator.SetTrigger("HideBars");
            }
        }
    }
    #endregion /*** ^^^ Cinematic bars animator. ^^^ ***/


}
