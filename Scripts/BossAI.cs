using UnityEngine;
using System.Threading.Tasks;

public class BossAI : MonoBehaviour
{
    // Security to clean arena when async functions are not running. If value = 0, boss can be destroy.
    public int isReadyToBeDestroySecurity = 0;


    [Header("Guns: ")]
    [SerializeField] private GameObject firstGun;
    [SerializeField] private GameObject secondGun;


    private void Start()
    {
        FirstLifeInitialize();


        // Assert weapon are hidden.
        firstGun.SetActive(false);
        secondGun.SetActive(false);
    }


    #region /*** First life/wave of the boss. ***/
    private bool isReadyToFight = false;

    private async void FirstLifeInitialize()
    {
        isReadyToBeDestroySecurity++;

        await InitializeHatsOnBossAsync(10);

        // Activate first gun.
        firstGun.SetActive(true);

        lastTimeSalvo = Time.time;
        intervalBetweenSalvo = Random.Range(1.5f, 4.0f);

        isReadyToFight = true;


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ First life/wave of the boss. ^^^ ***/



    #region /*** Second life/wave of the boss. ***/
    private bool hasBeenInSecondLife = false;

    private async void SecondLifeInitialize()
    {
        isReadyToBeDestroySecurity++;


        hasBeenInSecondLife = true;

        isReadyToFight = false;

        await InitializeHatsOnBossAsync(20);

        // Activate second gun.
        secondGun.SetActive(true);

        lastTimeSalvo = Time.time;
        intervalBetweenSalvo = Random.Range(1.5f, 4.0f);

        isReadyToFight = true;


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Second life/wave of the boss. ^^^ ***/



    #region /*** Third life/wave of the boss. ***/
    private bool hasBeenInThirdLife = false;

    private async void ThirdLifeInitialize()
    {
        isReadyToBeDestroySecurity++;


        hasBeenInThirdLife = true;

        isReadyToFight = false;

        await InitializeHatsOnBossAsync(100);

        lastTimeSalvo = Time.time;
        intervalBetweenSalvo = Random.Range(1.5f, 4.0f);

        isReadyToFight = true;


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Third life/wave of the boss. ^^^ ***/





    private Quaternion rotation;

    private void WeaponsRotation()
    {
        rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, targetDirection.normalized.x) * Mathf.Rad2Deg);

        #region /*** First gun Rotate to target direction. ***/
        if (firstGun != null && !firstGun.activeInHierarchy)
        {
            firstGun.transform.localRotation = targetDirection.x > 0.1f
                ? rotation
                : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, -targetDirection.normalized.x) * Mathf.Rad2Deg);
        }
        #endregion /*** ^^^ First gun Rotate to target direction.  ^^^ ***/


        #region /*** Second gun rotation if it's unlocked. ***/
        if (secondGun != null && !secondGun.activeInHierarchy)
        {
            secondGun.transform.localRotation = targetDirection.x > 0.1f
            ? rotation
            : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, -targetDirection.normalized.x) * Mathf.Rad2Deg);
        }

        #endregion /*** ^^^ Second gun rotation if it's unlocked. ^^^ ***/
    }




    private float lastTimeSalvo = 0.0f;
    private float intervalBetweenSalvo;

    private void Update()
    {
        #region /*** Set target security. ***/
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        #endregion /*** ^^^ Set target security. ^^^ ***/
        else if (!isDying && target != null && firstGun.activeInHierarchy)
        {
            targetDirection = target.transform.localPosition - transform.position;

            WeaponsRotation();

            BossMovements();

            FliBossToTargetDirection();


            #region /*** Shooting logic. ***/
            if (lastTimeSalvo + intervalBetweenSalvo < Time.time && !waitForCircle)
            {
                if (Random.value > 0.25)
                {
                    if (!hasBeenInThirdLife)
                    {
                        // First gun Shot salvo.
                        GunSalvo(firstGun);

                        // Second gun Shot salvo.
                        if (secondGun != null && secondGun.activeInHierarchy)
                        {
                            GunSalvo(secondGun);
                        }
                    }
                    else
                    {
                        TurningCircleShot();
                    }
                }
                else
                {
                    InstantCirlceShot();
                }

                lastTimeSalvo = Time.time;

                // Interval between salvo accordingly with current hat number.
                intervalBetweenSalvo = Random.Range(0.5f, (currentHatNum / 10) + 0.5f);
            }
            #endregion /*** ^^^ Shooting logic. ^^^ ***/
        }
    }



    #region /*** Boss Movements manager. ***/
    private GameObject target = null;
    private Vector3 targetDirection;
    private float accuracy = 4.5f;
    private readonly float speed = 1.5f;

    private Vector3 customTargetPos = Vector3.zero;

    private bool nearTarget = false;

    private float yOffset, xOffset;

    private void BossMovements()
    {
        if (target != null && !waitForCircle)
        {
            Vector3 targetPos = target.transform.localPosition;

            if (targetDirection.magnitude > accuracy && !nearTarget)
            {
                transform.position = Vector3.Slerp(
                    a: transform.position,
                    b: targetPos,
                    t: speed / 100);

                xOffset = Random.Range(-4.0f, 4.0f);
                yOffset = Random.Range(-4.0f, 4.0f);

                if (targetDirection.magnitude <= accuracy + 0.5f)
                {
                    nearTarget = true;
                }
            }

            GetComponent<Animator>().SetBool("Run", targetDirection.magnitude > accuracy);

            if (nearTarget)
            {
                customTargetPos = new(
                    x: targetPos.x + xOffset,
                    y: targetPos.y + yOffset,
                    z: targetPos.z);

                Vector3 customTargetDirection = customTargetPos - transform.position;

                transform.position = Vector3.Slerp(
                    a: transform.position,
                    b: customTargetPos,
                    t: speed / 100);

                if (customTargetDirection.magnitude <= accuracy + 0.5f)
                {
                    nearTarget = false;

                    accuracy = Random.Range(3.5f, 5.5f);
                }
            }
        }
    }
    #endregion /*** ^^^ Boss Movements manager. ^^^ ***/



    #region /*** Hit by Player. ***/
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player_bullet")
            && isReadyToFight
            && !isDying)
        {
            // Delete bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();

            // Play bullet hitting flesh sound effect.
            SoundManager.Instance.PlayRandomBulletHittingFleshSound();


            // Actions if it is hit by player's bullet.
            if (currentHatNum > 0)
            {
                // Send hat to hat manager to drop it on the floor, and then destroy it.
                HatsManager.Instance.AddHatToDropOnFloor(hatsArr[currentHatNum - 1], gameObject);

                currentHatNum--;
            }

            if (currentHatNum <= 0)
            {
                // Death unlocked.
                if (hasBeenInSecondLife && hasBeenInThirdLife)
                {
                    BossDeath();
                }

                // Third life unlocked.
                if (hasBeenInSecondLife && !hasBeenInThirdLife)
                {
                    ThirdLifeInitialize();
                }

                // Second life unlocked.
                if (!hasBeenInSecondLife)
                {
                    SecondLifeInitialize();
                }
            }
        }
    }
    #endregion /*** ^^^ Hit by Player ^^^ ***/



    #region /*** Boss death. ***/
    private bool isDying = false;
    private async void BossDeath()
    {
        if (!isDying)
        {
            isReadyToBeDestroySecurity++;


            isDying = true;

            await GenerateGoldNuggetExplosion(Random.Range(125, 185));

            for (int i = -720; i <= 720; i += Random.Range(10, 20))
            {
                _ = Instantiate(
                    original: opponentBulletReference,
                    position: transform.position,
                    rotation: Quaternion.Euler(0, 0, i));

                // Wait in ms before next bullet.
                await Task.Delay(25);
            }

            // Unlock boss trophy.
            TrophiesManager.Instance.AddBeatBossOnceTrophy();

            // Boss is no more on field.
            ObjectsGeneration.Instance.SetBossIsOnField(false);


            isReadyToBeDestroySecurity--;


            Destroy(obj: gameObject);
        }
    }
    #endregion /*** ^^^ Boss death. ^^^ ***/



    #region /*** Generate gold nugget explosion. ***/
    [SerializeField] private GameObject goldNuggetReference;

    private async Task GenerateGoldNuggetExplosion(int goldNuggetAmount)
    {
        for (int i = 0; i < goldNuggetAmount; i++)
        {
            GameObject gn = Instantiate(
                original: goldNuggetReference,
                position: transform.position,
                rotation: Quaternion.identity);

            gn.GetComponent<GoldNuggetBehavior>().SetIsPartOfExplosion(true);

            // Wait in ms before next gold nugget.
            await Task.Delay(25);
        }
    }
    #endregion /*** ^^^ Generate gold nugget explosion. ^^^ ***/



    #region /*** Hats initialization. ***/
    private GameObject[] hatsArr;

    private int currentHatNum = 0;

    private async Task InitializeHatsOnBossAsync(int hatNumberToInitialize)
    {
        isReadyToBeDestroySecurity++;


        // Initialize hat offset.
        float firstYOffset = GetComponent<BoxCollider2D>().size.y * 2.5f;

        // Initialize hats.
        System.Tuple<GameObject[], int> hatsData = await
            HatsManager.Instance.InitializeHatsForEntity(
            hatNumberToInitialize,
            firstYOffset,
            gameObject,
            true
            );

        // Set hats parameters.
        hatsArr = hatsData.Item1;
        currentHatNum = hatsData.Item2;


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Hats initialization. ^^^ ***/



    #region /*** Turning Circle shot mechanics. ***/
    private async void TurningCircleShot()
    {
        isReadyToBeDestroySecurity++;


        for (int i = 0; i <= 360; i += 24)
        {
            _ = Instantiate(
                original: opponentBulletReference,
                position: transform.position,
                rotation: Quaternion.Euler(0, 0, i));

            // delay between bullet in ms.
            await Task.Delay(35);
        }

        for (int i = 0; i <= 360; i += 24)
        {
            _ = Instantiate(
                original: opponentBulletReference,
                position: transform.position,
                rotation: Quaternion.Euler(0, 0, -i));

            // delay between bullet in ms.
            await Task.Delay(35);
        }


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Turning Circle Gun shot mechanics. ^^^ ***/



    #region /*** Instant Circle shot mechanics. ***/
    private bool waitForCircle = false;

    private async void InstantCirlceShot()
    {
        isReadyToBeDestroySecurity++;


        waitForCircle = true;

        for (int i = 0; i <= 380; i += 20)
        {
            _ = Instantiate(
                original: opponentBulletReference,
                position: transform.position,
                rotation: Quaternion.Euler(0, 0, -i));

            // Delay between bullet in ms.
            await Task.Delay(1 / 2);
        }

        waitForCircle = false;


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Instant Circle shot mechanics ^^^ ***/



    #region /*** Basic Gun shot mechanics. ***/
    [SerializeField] private GameObject opponentBulletReference;

    private int bulletsInSalvo;

    private async void GunSalvo(GameObject weapon)
    {
        isReadyToBeDestroySecurity++;


        // Random bullet number in salvo.
        bulletsInSalvo = Random.Range(4, 10);

        for (int i = 0; i < bulletsInSalvo; i++)
        {
            float addedOffset = (bulletsInSalvo / 2) + Random.Range(-15.0f, 15.0f);

            Quaternion newRotWithOffset = Quaternion.Euler(
                x: rotation.eulerAngles.x,
                y: rotation.eulerAngles.y,
                z: rotation.eulerAngles.z + addedOffset);

            // Shoot bullet in target direction + at gun position.
            Instantiate(opponentBulletReference, weapon.transform.position, newRotWithOffset);

            // Delay between bullet.
            await Task.Delay(100);
        }


        isReadyToBeDestroySecurity--;
    }
    #endregion /*** ^^^ Basic Gun shot mechanics. ^^^ ***/



    #region /*** Flip Boss accordingly with target position. ***/
    private bool loopRunningToRight = false;
    private bool loopRunningToLeft = false;

    private void FliBossToTargetDirection()
    {
        // Flip boss accordingly with target position.
        if (target.transform.localPosition.x < transform.position.x &&
           transform.localScale.x > 0 &&
           !loopRunningToLeft &&
           !loopRunningToRight)
        {
            loopRunningToLeft = true;
        }
        else if (target.transform.localPosition.x > transform.position.x &&
            transform.localScale.x < 0 &&
            !loopRunningToLeft &&
            !loopRunningToRight)
        {
            loopRunningToRight = true;
        }

        if (loopRunningToLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x - 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x <= -5)
            {
                loopRunningToLeft = false;
            }
        }

        if (loopRunningToRight)
        {
            transform.localScale = new Vector3(transform.localScale.x + 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 5)
            {
                loopRunningToRight = false;
            }
        }
    }
    #endregion /*** ^^^ Flip Boss accordingly with target position. ^^^ ***/
}