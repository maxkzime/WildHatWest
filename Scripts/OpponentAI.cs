using System;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    // Security to avoid object destruction while async functions are running.
    public bool isReadyToBeDestroySecurity = true;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Initialize last time shot.
        lastTimeshot = Time.time;

        InitializeHatsOnOpponentAsync();

        InitializeGun();
    }


    #region /*** Hats initialization. ***/
    private GameObject[] hatsArr;

    private int maxHats;

    private int currentHatNum = 0;

    private bool hatsAreInitialized = false;

    private async void InitializeHatsOnOpponentAsync()
    {
        isReadyToBeDestroySecurity = false;


        maxHats = UnityEngine.Random.Range(2, 4);

        float firstYOffset = 0.6f;

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


        isReadyToBeDestroySecurity = true;
    }
    #endregion /*** ^^^ Hats initialization. ^^^ ***/



    #region /*** Gun intialization. ***/
    [SerializeField] private GameObject gunReference;

    private GameObject gunInstance;

    private void InitializeGun()
    {
        gunInstance = Instantiate(
            original: gunReference,
            position: new Vector3(
                x: transform.position.x + 0.6f,
                y: transform.position.y - 0.25f,
                z: transform.position.z),
            rotation: Quaternion.identity);

        gunInstance.transform.SetParent(p: transform);

    }
    #endregion /*** ^^^ Gun intialization. ^^^ ***/




    #region /*** Create dust when stop running. ***/
    [SerializeField] private ParticleSystem dustRunParticleSystem;

    private bool stoppingDust = false;

    private void CreateDust()
    {
        dustRunParticleSystem.Play();
    }
    #endregion /*** ^^^ Create dust when stop running. ^^^ ***/




    private GameObject target = null;
    private Vector3 targetDirection;
    private Quaternion rotation;

    private void Update()
    {
        // Set target
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            #region /*** Compute target parameters. ***/
            targetDirection = target.transform.localPosition - transform.position;

            rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, targetDirection.normalized.x) * Mathf.Rad2Deg);

            #endregion ^^^ /*** Compute target parameters ^^^ ***/


            OpponentMovements();

            FlipToTargetDirection();

            RotateGun();

            OpponentShoot();
        }
    }




    #region /*** Gun rotate in target direction. ***/
    private void RotateGun()
    {
        gunInstance.transform.localRotation = targetDirection.x > 0.1f
            ? rotation
            : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, -targetDirection.normalized.x) * Mathf.Rad2Deg);

    }
    #endregion /*** ^^^ Gun rotate in target direction. ^^^ ***/




    #region /*** Opponent shooting mechanics. ***/
    private float lastTimeshot = 0.0f;
    private float shotTimeInterval = 0.0f;

    [SerializeField] private GameObject opponentBullet;

    private int bulletShot = 0;
    private int maxBulletShotBeforeNewAccuracy = 2;

    private void OpponentShoot()
    {
        // Shoot bullet in target direction + at gun position.
        if (lastTimeshot + shotTimeInterval < Time.time)
        {
            _ = Instantiate(
                original: opponentBullet,
                position: gunInstance.transform.position,
                rotation: rotation);

            // Playing gun shot sound effect.
            SoundManager.Instance.PlayRandomGunShotSoundEffect();

            lastTimeshot = Time.time;

            shotTimeInterval = UnityEngine.Random.Range(0.5f, 1.0f);

            bulletShot++;


            // Randomize accuracy (distance between opponent and target).
            minDistanceToTarget = UnityEngine.Random.Range(3.0f, 7.0f);

            // Randomize parameters after few shots.
            if (bulletShot > maxBulletShotBeforeNewAccuracy)
            {
                // Randomize new limit.
                maxBulletShotBeforeNewAccuracy = UnityEngine.Random.Range(2, 5);

                // Reset bullet count.
                bulletShot = 0;
            }
        }
    }
    #endregion /*** ^^^ Opponent shooting mechanics. ^^^ ***/



    #region /*** Opponent's movements manager. ***/
    private readonly float speed = 3.0f;

    private float minDistanceToTarget = 3.0f;

    private void OpponentMovements()
    {

        if (targetDirection.magnitude > minDistanceToTarget)
        {
            transform.Translate(speed * Time.deltaTime * targetDirection.normalized, Space.World);

            stoppingDust = false;
        }
        // Create dust at the moment the opponent stop running.
        else if (!stoppingDust)
        {
            stoppingDust = true;

            CreateDust();
        }

        // Set Run to true if is still moving.
        animator.SetBool(
            name: "Run",
            value: targetDirection.magnitude > minDistanceToTarget);

    }
    #endregion /*** ^^^ Opponent's movements manager. ^^^ ***/




    #region /*** Flip opponent to target direction. ***/
    private bool loopRunningToRight = false;
    private bool loopRunningToLeft = false;

    private void FlipToTargetDirection()
    {
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

                // Create dust when turning end.
                CreateDust();
            }
        }

        if (loopRunningToRight)
        {
            transform.localScale = new Vector3(transform.localScale.x + 1.0f, transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 5)
            {
                loopRunningToRight = false;

                // Create dust when turning end.
                CreateDust();
            }
        }
    }
    #endregion /*** ^^^ Flip opponent to target direction. ^^^ ***/




    [SerializeField] private GameObject graveReference;
    private GameObject graveInstance = null;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        #region /*** Hit by player mechanics. ***/
        if (collider.gameObject.CompareTag("Player_bullet") && hatsAreInitialized)
        {
            // Delete bullet.
            Destroy(collider.GetComponent<BoxCollider2D>());
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();

            // Play bullet hitting flesh sound effect.
            SoundManager.Instance.PlayRandomBulletHittingFleshSound();

            if (currentHatNum > 0)
            {
                // Send hat to hat manager to drop it on the floor, and then destroy it.
                HatsManager.Instance.AddHatToDropOnFloor(hatsArr[currentHatNum - 1], gameObject);

                currentHatNum--;
            }

            if (currentHatNum <= 0 && graveInstance == null)
            {
                Destroy(obj: GetComponent<BoxCollider2D>());

                // Instantiate opponent's grave.
                graveInstance = Instantiate(
                    original: graveReference,
                    position: transform.position,
                    rotation: Quaternion.Euler(0, 0, UnityEngine.Random.Range(-20.0f, 20.0f)));


                ObjectsGeneration generatorGenerationScript = ObjectsGeneration.Instance;

                generatorGenerationScript.SpawnNewOpponent();
                generatorGenerationScript.DecreaseOpponentsOnField();


                Destroy(obj: gameObject);
            }
        }
        #endregion /*** ^^^ Hit by player mechanics. ^^^ ***/
    }
}
