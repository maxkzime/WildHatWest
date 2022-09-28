using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Threading.Tasks;
using Cinemachine;


public class PlayerShoot : MonoBehaviour
{
    private RectTransform canvasRect;
    private ObjectsGeneration generatorGenerationScript;
    private int shotgunAvailableReload;


    private void Start()
    {
        bulletLoaded = 0;

        canvasRect = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>();

        generatorGenerationScript = GameObject.FindGameObjectWithTag("Generator").GetComponent<ObjectsGeneration>();

        shotgunAvailableReload = generatorGenerationScript.ShotgunReloadAmount;


        InitializeSecondGun();


        bulletUIIndicatorPos = Vector3.zero;

        InitializeBulletLoadedUIIndicator();

    }



    #region /*** If has unlocked second gun : instantiate second gun ***/
    [SerializeField] private GameObject gunRef;
    private GameObject secondGun = null;

    private void InitializeSecondGun()
    {
        if (generatorGenerationScript.HasUnlockedDoubleGun)
        {
            secondGun = Instantiate(
                original: gunRef,
                position: transform.position,
                rotation: Quaternion.identity,
                parent: transform);

            Vector3 gunReferenceScale = gunRef.transform.localScale;
            Vector3 parentScale = transform.localScale;

            // Scale security.
            secondGun.transform.localScale = new Vector3(
                -gunReferenceScale.x / parentScale.x,
                gunReferenceScale.y / parentScale.y,
                gunReferenceScale.z / parentScale.z);

            secondGun.transform.localPosition = new Vector3(
                x: -0.11f,
                y: -0.03f,
                z: -1);
        }
    }
    #endregion /*** ^^^ If has unlocked second gun : instantiate second gun ^^^ ***/




    #region /*** Initialize bullet loaded UI indicator. ***/
    private Vector3 bulletUIIndicatorPos;

    private int bulletLoaded;
    private readonly int maxBulletLoadedInGun = 6;
    private readonly int maxBulletLoadedInShotgun = 3;

    private GameObject bulletUIIndicatorToLoad = null;

    [SerializeField] private GameObject bulletUIIndicator;

    [SerializeField] private Sprite gunBulletUIIndicatorSprite, shotgunBulletUIIndicatorSprite;

    private readonly int scaleRatioForBulletUIIndicator = 5;

    private void InitializeBulletLoadedUIIndicator()
    {
        bulletUIIndicatorPos.x = canvasRect.position.x;

        // Modifying bullet ui indicators number accordingly with selected weapon.
        int bulletUIIndicatorToLoadNumber = gunIsSelected ? maxBulletLoadedInGun : maxBulletLoadedInShotgun;

        for (int i = 0; i < bulletUIIndicatorToLoadNumber; i++)
        {
            bulletUIIndicatorToLoad =
                Instantiate(
                    original: bulletUIIndicator,
                    parent: canvasRect,
                    worldPositionStays: true);

            // Modifying bullet ui indicators sprite accordingly with selected weapon.
            bulletUIIndicatorToLoad.GetComponent<Image>().sprite = gunIsSelected ? gunBulletUIIndicatorSprite : shotgunBulletUIIndicatorSprite;


            RectTransform indicatorRect = bulletUIIndicatorToLoad.GetComponent<RectTransform>();

            // Initializing position out of screen.
            indicatorRect.position =
                new Vector3(
                    x: bulletUIIndicatorPos.x,
                    y: indicatorRect.rect.height * -5 * canvasRect.localScale.y,
                    z: 0);

            bulletUIIndicatorToLoad.transform.localScale = new Vector3(
                x: scaleRatioForBulletUIIndicator,
                y: scaleRatioForBulletUIIndicator,
                z: 1);

            bulletUIIndicatorToPushUp.Enqueue(bulletUIIndicatorToLoad);

            bulletLoaded++;

            // Offset.
            bulletUIIndicatorPos.x += indicatorRect.rect.width * 2 * scaleRatioForBulletUIIndicator * canvasRect.localScale.x;
        }

        PushUpSequentiallyAllBulletIndicators();

    }
    #endregion /*** ^^^ Initialize bullet loaded UI indicator. ^^^ ***/




    #region /*** Push up bullet UI Indicator sequentially : push in screen bullet UI Indicator. ***/

    public bool ReloadFinished { get; set; } = false;

    private Queue<GameObject> bulletUIIndicatorToPushUp = new();

    private async void PushUpSequentiallyAllBulletIndicators()
    {
        if (bulletUIIndicatorToPushUp.Count > 0)
        {
            ReloadFinished = false;

            foreach (GameObject obj in bulletUIIndicatorToPushUp)
            {
                await PushUpBulletIndicator(obj);
            }

            ReloadFinished = true;
        }

    }
    #endregion /*** ^^^ Push up bullet UI Indicator sequentially : push in screen bullet UI Indicator ^^^ ***/




    #region /*** Play reload sound effect accordingly with weapon selected. ***/
    [Header("Reload Sound Effects : ")]
    [SerializeField] private AudioClip gunReloadSound;
    [SerializeField] private AudioClip shotgunReloadSound;

    private void PlayReloadSoundEffects()
    {
        if (gunIsSelected)
        {
            SoundManager.Instance.PlaySound(gunReloadSound, 0.75f);
        }
        else
        {
            SoundManager.Instance.PlaySound(shotgunReloadSound, 0.75f);
        }
    }
    #endregion /*** ^^^ Play reload sound effect accordingly with weapon selected. ^^^ ***/




    #region /*** Translate up bullet to load indicator passed in parameter, while it is not positionned. ***/
    private async Task PushUpBulletIndicator(GameObject obj)
    {
        PlayReloadSoundEffects();

        RectTransform indicatorToPushRect = obj.GetComponent<RectTransform>();
        RectTransform indicatorReferenceRect = bulletUIIndicator.GetComponent<RectTransform>();

        while (indicatorToPushRect.position.y * canvasRect.localScale.y < indicatorReferenceRect.rect.height * canvasRect.localScale.y)
        {
            if (obj == null)
                break;

            indicatorToPushRect.Translate(translation: 500 * Time.deltaTime * Vector3.up);

            await Task.Yield();
        }


        #region /*** Repositionning bullet UI Indicator if it's above it's position. ***/
        if (indicatorToPushRect.position.y * canvasRect.localScale.y >= indicatorReferenceRect.rect.height - (5 * canvasRect.localScale.y) ||
            indicatorToPushRect.position.y * canvasRect.localScale.y < 50 * canvasRect.localScale.y)
        {
            indicatorToPushRect.position = new Vector3(
                x: indicatorToPushRect.position.x,
                y: 50 * canvasRect.localScale.y,
                z: indicatorToPushRect.position.z);
        }
        #endregion /*** ^^^ Repositionning bullet UI Indicator if it's above it's position. ^^^ ***/

    }
    #endregion /*** ^^^ Translate up bullet to load indicator passed in parameter, while it is not positionned. ^^^ ***/



    #region /*** Decrease by one the bullet UI indicator. ***/
    private GameObject TempBulletUIIndicator = null;

    private Queue<GameObject> bulletUIIndicatorToPushDown = new();

    private void DecreaseBulletUIIndicator()
    {
        TempBulletUIIndicator = bulletUIIndicatorToPushUp.Peek();

        bulletUIIndicatorToPushUp.Dequeue();

        bulletUIIndicatorToPushUp.TrimExcess();

        bulletUIIndicatorToPushDown.Enqueue(TempBulletUIIndicator);

        bulletLoaded--;
    }
    #endregion /*** ^^^ Decrease by one the bullet UI indicator. ^^^ ***/



    #region /*** Reload bullet loaded + clean way to reload bullet UI Indicator. ***/
    private void ReloadBullets()
    {
        #region /*** If there's still bullets loaded, clear queue + push them down. ***/
        if (bulletUIIndicatorToPushUp.Count > 0)
        {
            List<GameObject> list = bulletUIIndicatorToPushUp.ToList();

            bulletUIIndicatorToPushUp.Clear();
            bulletUIIndicatorToPushUp.TrimExcess();

            for (int i = 0; i < list.Count; i++)
            {
                bulletUIIndicatorToPushDown.Enqueue(list[i]);
            }

            list.Clear();

        }
        #endregion /*** ^^^ If there's still bullets loaded, clear queue + push them down. ^^^ ***/

        bulletLoaded = 0;


        if (gunIsSelected || shotgunAvailableReload > 0)
        {
            InitializeBulletLoadedUIIndicator();
        }

        // Decrease reload amount if the player hasn't unlocked infinite reload.
        if (!gunIsSelected && shotgunAvailableReload < 20)
        {
            shotgunAvailableReload--;
        }
    }
    #endregion /*** ^^^ Reload bullet loaded + clean way to reload bullet UI Indicator. ^^^ ***/




    #region /*** Push down bullet UI Indicator : destroy bullet UI Indicator. ***/
    private void PushDownBulletUIIndicator()
    {
        if (bulletUIIndicatorToPushDown.Count > 0)
        {
            #region /*** Translate down shot bullet to destroy (out of screen). ***/
            foreach (GameObject obj in bulletUIIndicatorToPushDown)
            {
                RectTransform indicator = obj.GetComponent<RectTransform>();

                if (indicator.position.y * canvasRect.localScale.y >
                    indicator.rect.height * -10 * canvasRect.localScale.y)
                {
                    indicator.Translate(translation: 1000 * Time.deltaTime * Vector3.down);
                }
            }
            #endregion /*** ^^^ Translate down shot bullet to destroy (out of screen). ^^^ ***/


            #region /*** Destroy + remove bullet UI Indicator. ***/
            List<GameObject> list = bulletUIIndicatorToPushDown.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                RectTransform indicator = list[i].GetComponent<RectTransform>();

                if (indicator.position.y * canvasRect.localScale.y
                    <= indicator.rect.height * -5 * canvasRect.localScale.y)
                {
                    Destroy(list[i]);
                    list.RemoveAt(i);
                }
            }

            bulletUIIndicatorToPushDown = new Queue<GameObject>(list);

            #endregion /*** ^^^ Destroy + remove bullet UI Indicator. ^^^ ***/
        }
    }
    #endregion /*** ^^^ Push down bullet UI Indicator : destroy bullet UI Indicator. ^^^ ***/



    private Vector3 mousePosition;
    private Vector2 targetPos;
    private Vector2 playerPos;

    private void Update()
    {
        // Destroy bullet UI Indicators that have been shot.
        PushDownBulletUIIndicator();


        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z += Camera.main.nearClipPlane;

        #region /*** Change player direction accordingly with the mouse cursor position when he is not running. ***/
        if (mousePosition.x - transform.localPosition.x < 0.01f && transform.localScale.x >= 5)
        {
            GetComponent<PlayerMovement>().loopRunningToLeft = true;
        }

        if (mousePosition.x - transform.localPosition.x > 0.01f && transform.localScale.x <= -5)
        {
            GetComponent<PlayerMovement>().loopRunningToRight = true;
        }
        #endregion /*** ^^^ Change player direction accordingly with the mouse cursor position when he is not running. ^^^ ***/


        #region /*** Shoot bullet in cursor direction + at gun position. ***/
        targetPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y)); // Target pos = mouse pos.

        playerPos = new Vector2(transform.position.x, transform.position.y);

        targetDirection = targetPos - playerPos;

        targetDirection.Normalize();


        #region /*** Weapons mechanics. ***/
        if (ReloadFinished && !GetComponent<PlayerStats>().GetIsDeath())
        {
            if (!Helpers.IsOverUI() && Input.GetMouseButtonDown(0) && bulletLoaded > 0)
            {
                Shoot();
            }
            else if (!Helpers.IsOverUI() && Input.GetMouseButtonDown(0) && bulletLoaded <= 0)
            {
                // PLay locked button when no more bullets loaded.
                SoundManager.Instance.PlayLockedButtonSoundEffect();
            }
            #endregion /*** ^^^ Shoot bullet in cursor direction + at gun position. ^^^ ***/



            #region /*** Reload bullets in gun. ***/
            if (Input.GetMouseButtonDown(2))
            {
                ReloadBullets();
            }
            #endregion /*** ^^^ Reload bullets in gun. ^^^ ***/



            #region /*** Switch weapon if shotgun is unlocked only. ***/
            if (Input.GetMouseButtonDown(1) && generatorGenerationScript.HasUnlockedShotgun)
            {
                SwitchWeapon();
            }
            #endregion /*** ^^^ Switch weapon if shotgun is unlocked only. ^^^ ***/
        }
        #endregion /*** ^^^ Weapons mechanics. ^^^ ***/


        CameraZoomEffect();


        GunsRotation();

    }



    #region /*** Player's guns rotation, following cursor position. ***/
    private void GunsRotation()
    {
        #region /*** First gun rotation (following cursor). ***/
        transform.GetChild(0).transform.localRotation =
            Quaternion.Euler(
                x: 0,
                y: 0,
                z: Mathf.Atan2(targetDirection.y, Mathf.Sign(mousePosition.x - transform.localPosition.x) * targetDirection.x) * Mathf.Rad2Deg);

        #endregion /*** ^^^ Fisrt gun rotation (following cursor). ^^^ ***/


        #region /*** Second gun rotation if it's unlocked. ***/
        if (generatorGenerationScript.HasUnlockedDoubleGun && secondGun != null)
        {
            secondGun.transform.localRotation =
                Quaternion.Euler(
                    x: 0,
                    y: 0,
                    z: Mathf.Atan2(targetDirection.y, Mathf.Sign(mousePosition.x - transform.localPosition.x) * targetDirection.x) * Mathf.Rad2Deg);
        }
        #endregion /*** ^^^ Second gun rotation if it's unlocked. ^^^ ***/

    }
    #endregion /*** ^^^ Player's guns rotation, following cursor position. ^^^ ***/




    #region /*** Camera zoom in and out effect for each shot. ***/
    bool cameraZoomInEffectIsRunning = false;

    private CinemachineVirtualCamera cineCam;

    private void CameraZoomEffect()
    {
        if(cineCam == null)
        {
            cineCam = FindObjectOfType<CinemachineVirtualCamera>();
        }
        else
        {

            if (cameraZoomInEffectIsRunning)
            {
                if (cineCam.m_Lens.OrthographicSize <= 6.1f)
                {
                    cameraZoomInEffectIsRunning = false;
                }
                else
                {
                    cineCam.m_Lens.OrthographicSize -= 0.1f;
                }
            }
            else if (cineCam.m_Lens.OrthographicSize <= 6.3f)
            {
                cineCam.m_Lens.OrthographicSize += 0.1f;
            }
        }
    }
    #endregion /*** ^^^ Camera zoom in and out effect for each shot. ^^^ ***/





    #region /*** Gun shot logic. ***/
    private Vector2 targetDirection;

    [SerializeField] private GameObject bullet;

    private void GunShot()
    {
        Quaternion rotation = Quaternion.Euler(
            x: 0,
            y: 0,
            z: Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg);

        _ = Instantiate(
            original: bullet,
            position: transform.GetChild(0).transform.position,
            rotation: rotation);


        // Playing gun shot sound effect.
        SoundManager.Instance.PlayRandomGunShotSoundEffect();


        #region /*** If has unlocked double gun = shoot second bullet with reverse rotation. ***/
        if (generatorGenerationScript.HasUnlockedDoubleGun && secondGun != null)
        {
            Quaternion reverseRotation = Quaternion.Euler(
                x: 0,
                y: 0,
                z: Mathf.Atan2(-targetDirection.y, -targetDirection.x) * Mathf.Rad2Deg);

            _ = Instantiate(
                original: bullet,
                position: secondGun.transform.position,
                rotation: reverseRotation);

            // Playing gun shot sound effect.
            SoundManager.Instance.PlayRandomGunShotSoundEffect();

        }
        #endregion /*** ^^^ If has unlocked double gun = shoot second bullet with reverse rotation. ^^^ ***/

        cameraZoomInEffectIsRunning = true;

        DecreaseBulletUIIndicator();

    }
    #endregion /*** ^^^ Gun shot logic. ^^^ ***/




    #region /*** Switch weapon by changing weapon(s) sprite(s). ***/
    [Header("Weapons Sprites : ")]
    [SerializeField] private Sprite gunSprite;
    [SerializeField] private Sprite shotgunSprite;

    private bool gunIsSelected = true; // False = shotgun is selected.

    private void SwitchWeapon()
    {
        if (gunIsSelected)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = shotgunSprite;

            #region /*** If has unlocked double gun = switch second weapon to shotgun too. ***/
            if (generatorGenerationScript.HasUnlockedDoubleGun && secondGun != null)
            {
                secondGun.GetComponent<SpriteRenderer>().sprite = shotgunSprite;
            }
            #endregion /*** ^^^ If has unlocked double gun = switch second weapon to shotgun too. ^^^ ***/

            gunIsSelected = false;

        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = gunSprite;


            #region /*** If has unlocked double gun = change second weapon to gun too. ***/
            if (generatorGenerationScript.HasUnlockedDoubleGun && secondGun != null)
            {
                secondGun.GetComponent<SpriteRenderer>().sprite = gunSprite;
            }
            #endregion /*** ^^^ If has unlocked double gun = change second weapon to gun too. ^^^ ***/


            gunIsSelected = true;

        }

        // Reload to change bullet UI Indicators.
        ReloadBullets();

    }
    #endregion /*** ^^^ Switch weapon by changing weapon(s) sprite(s). ^^^ ***/



    #region /*** Shoot accordingly with selected weapon. ***/
    private void Shoot()
    {
        if (gunIsSelected)
        {
            GunShot();
        }
        else
        {
            ShotgunShot();
        }
    }
    #endregion /*** ^^^ Shoot accordingly with selected weapon. ^^^ ***/




    #region /*** Shotgun shot mechanics. ***/
    private int bulletAmountInOneShot = 3;

    private void ShotgunShot()
    {
        #region /*** First shot. ***/
        Quaternion rotation = Quaternion.Euler(
            x: 0,
            y: 0,
            z: Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg);

        // Playing shotgun shot sound effect.
        SoundManager.Instance.PlayRandomShotgunShotSoundEffect();

        ShotGunMechanic(
            rotation: rotation,
            position: transform.GetChild(0).transform.position);

        #endregion /*** ^^^ First shot. ^^^ ***/


        #region /*** if has unlocked double gun = shoot second time with reverse rotation. ***/
        if (generatorGenerationScript.HasUnlockedDoubleGun && secondGun != null)
        {
            Quaternion reverseRotation = Quaternion.Euler(
                x: 0,
                y: 0,
                z: Mathf.Atan2(-targetDirection.y, -targetDirection.x) * Mathf.Rad2Deg);

            // Playing shotgun shot sound effect.
            SoundManager.Instance.PlayRandomShotgunShotSoundEffect();

            ShotGunMechanic(
                rotation: reverseRotation,
                position: secondGun.transform.position);
        }
        #endregion /*** ^^^ if has unlocked double gun = shoot second time with reverse rotation. ^^^ ***/


        cameraZoomInEffectIsRunning = true;

        DecreaseBulletUIIndicator();
    }


    private void ShotGunMechanic(Quaternion rotation, Vector3 position)
    {
        float spread = 2;

        for (int i = 0; i < bulletAmountInOneShot; i++)
        {
            float addedOffset = (bulletAmountInOneShot / 2 * spread) + Random.Range(-30.0f, 30.0f);

            Quaternion newRotWithOffset = Quaternion.Euler(
                x: rotation.eulerAngles.x,
                y: rotation.eulerAngles.y,
                z: rotation.eulerAngles.z + addedOffset);

            _ = Instantiate(
                original: bullet,
                position: position,
                rotation: newRotWithOffset);
        }

        // Apply knockback on the player.
        // Only if the player has only one shotgun.
        if (!generatorGenerationScript.HasUnlockedDoubleGun)
        {
            ShotgunKnockback();
        }

        // Randomize next shot bullet amount.
        bulletAmountInOneShot = Random.Range(3, 6);
    }



    #region /*** Shotgun Knockback on player when he shoots. ***/
    private Rigidbody2D playerRb = null;


    private async void ShotgunKnockback()
    {
        // Grab rigidbody to apply force on it.
        if (playerRb == null)
        {
            playerRb = GetComponent<Rigidbody2D>();
        }

        if (playerRb != null)
        {
            // Apply knockback force on player.
            // Force is  opposite normalize mouse cursor position multiply by the amount of bullet shot.
            playerRb.AddForce(
                force: bulletAmountInOneShot * 3 * -targetDirection,
                mode: ForceMode2D.Impulse);

            // Wait random delay before stopping knockback.
            await Task.Delay(Random.Range(200, 400));

            // Reset velocity of the rigidbody (no more force).
            playerRb.velocity = Vector3.zero;
        }
    }
    #endregion /*** ^^^ Shotgun Knockback on player when he shoots. ^^^ ***/


    #endregion /*** ^^^ Shotgun shot mechanics. ^^^ ***/
}