using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources : ")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource playerWalkSource;


    #region /*** Play audio music on start. ***/
    [SerializeField] private AudioClip desertAmbient;

    [SerializeField] private AudioClip[] startGameMusicEffect;

    private void Start()
    {
        ambientSource.clip = desertAmbient;
        ambientSource.Play();

        AudioClip intro = startGameMusicEffect[Random.Range(0, startGameMusicEffect.Length)];
        musicSource.PlayOneShot(intro);

        lastTimeMusicPlayed = Time.time;
    }
    #endregion /*** ^^^ Play audio music on start. ^^^ ***/



    #region /*** Background music playing manager. ***/
    private float musicTimeInterval = 20.0f;
    private float lastTimeMusicPlayed;

    [Header("Background Musics : ")]
    [SerializeField] private AudioClip[] backgroundMusics;

    private void Update()
    {
        if(!musicSource.isPlaying)
        {
            if (lastTimeMusicPlayed + musicTimeInterval < Time.time)
            {
                AudioClip nextClipPlaying = backgroundMusics[Random.Range(0, backgroundMusics.Length)];
                musicSource.PlayOneShot(nextClipPlaying, 0.75f);

                lastTimeMusicPlayed = Time.time;

                musicTimeInterval = nextClipPlaying.length + Random.Range(20.0f, 60.0f);
            }
        }
    }
    #endregion /*** ^^^ Background music playing manager. ^^^ ***/





    #region /*** Initialize sound manager instance (always alive). ***/
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion /*** ^^^ Initialize sound manager instance (always alive). ^^^ ***/



    #region /*** Mute sound effects sources if not already mute. ***/
    public void ToggleSoundEffect()
    {
        PlaySelectedButtonSoundEffect();

        effectSource.mute = !effectSource.mute;

        playerWalkSource.mute = !playerWalkSource.mute;

        heartBeatAudioSource.mute = !heartBeatAudioSource.mute;

        ambientSource.mute = !ambientSource.mute;
    }

    #endregion /*** ^^^ Mute sound effects sources if not already mute. ^^^ ***/




    #region /*** Mute music sources if not already mute. ***/
    public void ToggleMusic()
    {
        PlaySelectedButtonSoundEffect();

        musicSource.mute = !musicSource.mute;
    }

    #endregion /*** ^^^ Mute music sources if not already mute. ^^^ ***/






    #region /*** Play sound effect general function. ***/
    public void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        effectSource.PlayOneShot(clip, volume);
    }

    #endregion /*** ^^^ Play sound effect general function. ^^^ ***/




    #region /*** Specific sound needed in multiple objects. ***/
    [Header("Specific Sound Effects : ")]

    #region /*** Play breaking tutorial object sound effect. ***/
    [Header("Breaking wood Sound Effects : ")]
    [SerializeField] private AudioClip breakingWoodSoundEffect;
    public void PlayBreakingWoodSoundEffect()
    {
        PlaySound(breakingWoodSoundEffect, 0.7f);
    }

    #endregion /*** ^^^ Play breaking tutorial object sound effect. ^^^ ***/



    #region /*** Play collecting gold nugget sound effect. ***/
    [Header("Collecting gold nugget Sound Effects : ")]
    [SerializeField] private AudioClip collectingGoldNuggetSoundEffect;
    public void PlayCollectingGoldNuggetSoundEffect()
    {
        PlaySound(collectingGoldNuggetSoundEffect);
    }

    #endregion /*** ^^^ Play collecting gold nugget sound effect. ^^^ ***/


    #region /*** Play heart beat loop sound effect. ***/
    [Header("Heart Beat Sound Effects : ")]
    [SerializeField] private AudioSource heartBeatAudioSource;
    public void TriggerHeartBeatLoopSoundEffect()
    {
        if(heartBeatAudioSource.isPlaying)
        {
            heartBeatAudioSource.Stop();
        }
        else
        {
            heartBeatAudioSource.Play();
        }
    }

    #endregion /*** ^^^ Play collecting gold nugget sound effect. ^^^ ***/


    #region /*** Play shotgun shot sound effect. ***/
    [Header("Shotgun shot Sound Effects : ")]
    [SerializeField] private AudioClip[] shotgunShotSoundEffects;

    public void PlayRandomShotgunShotSoundEffect()
    {
        PlaySound(shotgunShotSoundEffects[Random.Range(0, shotgunShotSoundEffects.Length)], 0.40f);
    }

    #endregion /*** ^^^ Play shotgun shot sound effect. ^^^ ***/



    #region /*** Play gun shot sound effect. ***/
    [Header("Gun shot Sound Effects : ")]
    [SerializeField] private AudioClip[] gunShotSoundEffects;

    public void PlayRandomGunShotSoundEffect()
    {
        PlaySound(gunShotSoundEffects[Random.Range(0, gunShotSoundEffects.Length)], 0.35f);
    }
    #endregion /*** ^^^ Play gun shot sound effect. ^^^ ***/



    #region /*** Play pressed button sound effect. ***/
    [Header("Actions on Buttons Sound Effects : ")]
    [SerializeField] private AudioClip pressedButtonSoundEffect;

    public void PlayPressedButtonSoundEffect()
    {
        PlaySound(pressedButtonSoundEffect, 0.9f);
    }
    #endregion /*** ^^^ Play pressed button sound effect. ^^^ ***/

    #region /*** Play selected button sound effect. ***/
    [SerializeField] private AudioClip selectedButtonSoundEffect;

    public void PlaySelectedButtonSoundEffect()
    {
        PlaySound(selectedButtonSoundEffect, 0.9f);
    }
    #endregion /*** ^^^ Play selected button sound effect. ^^^ ***/

    #region /*** Play locked button sound effect. ***/
    [SerializeField] private AudioClip lockedButtonSoundEffect;

    public void PlayLockedButtonSoundEffect()
    {
        PlaySound(lockedButtonSoundEffect, 0.5f);
    }
    #endregion /*** ^^^ Play locked button sound effect. ^^^ ***/





    #region /*** Player walking sound effect. ***/
    [Space(25f)]
    [SerializeField] private AudioClip playerWalkSound;

    private bool stopWalkSoundEffect = false;
    private bool walkSoundEffectIsPlaying = false;

    // Start playing player walking sound effect if its not already playing.
    public void PlayPlayerWalkingSoundEffect()
    {
        if (!walkSoundEffectIsPlaying)
        {
            playerWalkSource.Play();
            walkSoundEffectIsPlaying = true;
            stopWalkSoundEffect = false;
        }
    }

    // Stop playing player walking sound effect if its playing.
    public void StopPlayerWalkingSoundEffect()
    {
        if (!stopWalkSoundEffect)
        {
            playerWalkSource.Stop();
            stopWalkSoundEffect = true;
            walkSoundEffectIsPlaying = false;
        }
    }

    #endregion /*** ^^^ Player walking sound effect. ^^^ ***/



    #region /*** Chicken sound effect. ***/
    [Header("Angry chicken Sound Effects : ")]
    [SerializeField] private AudioClip[] angryChickenSoundEffects;

    public void PlayRandomAngryChickenSound()
    {
        PlaySound(angryChickenSoundEffects[Random.Range(0, angryChickenSoundEffects.Length)], 0.65f);
    }
    #endregion /*** ^^^ Chicken sound effect. ^^^ ***/



    #region /*** Bullet hitting cactus sound effect. ***/
    [Header("Bullet hitting cactus Sound Effects : ")]
    [SerializeField] private AudioClip[] cactusBreakingSoundEffects;

    public void PlayRandomBulletHittingCactusSound()
    {
        PlaySound(cactusBreakingSoundEffects[Random.Range(0, cactusBreakingSoundEffects.Length)], 0.50f);
    }
    #endregion /*** ^^^ Bullet hitting cactus sound effect. ^^^ ***/




    #region /*** Bullet hitting rock sound effect. ***/
    [Header("Bullet hitting rock Sound Effects : ")]
    [SerializeField] private AudioClip[] bulletHittingRockSoundEffects;

    public void PlayRandomBulletHittingRockSound()
    {
       PlaySound(bulletHittingRockSoundEffects[Random.Range(0, bulletHittingRockSoundEffects.Length)], 0.25f);
    }
    #endregion /*** ^^^ Bullet hitting rock sound effect. ^^^ ***/




    #region /*** Bullet hitting flesh sound effect. ***/
    [Header("Bullet hitting flesh Sound Effects : ")]
    [SerializeField] private AudioClip[] bulletHittingFleshSoundEffects;

    public void PlayRandomBulletHittingFleshSound()
    {
        PlaySound(bulletHittingFleshSoundEffects[Random.Range(0, bulletHittingFleshSoundEffects.Length)], 0.55f);
    }
    #endregion /*** ^^^ Bullet hitting flesh sound effect. ^^^ ***/





    #region /*** Player death sound effect. ***/
    [Header("Player death Sound Effects : ")]
    [SerializeField] private AudioClip[] playerDeathSoundEffects;

    public void PlayRandomPlayerDeathSound()
    {
        PlaySound(playerDeathSoundEffects[Random.Range(0, playerDeathSoundEffects.Length)], 0.95f);
    }
    #endregion /*** ^^^ Player death sound effect. ^^^ ***/


    #endregion /*** ^^^ Specific sound needed in multiple objects. ^^^ ***/
}
