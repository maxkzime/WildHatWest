using System.Threading.Tasks;
using UnityEngine;

public class BisonAI : MonoBehaviour
{
    private Animator animator;
    private Vector3 targetPosition;
    private Vector3 spawnPosition;
    private int floorLimit;

    private void Start()
    {
        // Grabing animator.
        animator = GetComponent<Animator>();

        // Initialize last time target generation.
        lastTimeTargetGeneration = Time.time;

        // Target position intialization.
        targetPosition = new Vector3(
            x: transform.position.x + Random.Range(-2.0f, 2.0f),
            y: transform.position.y + Random.Range(-2.0f, 2.0f),
            z: transform.position.z);

        // Saving spawn position.
        spawnPosition = transform.position;

        // Getting floor size limit.
        floorLimit = (ObjectsGeneration.Instance.GetFloorSize() / 2) - 1;

        // Initialize random player's bullet number, it can handle before getting angry.
        hitBeforeAngry = Random.Range(1, 4);

        // Initialize random player's bullet number, it can handle before dying.
        hitBeforeDead = Random.Range(-6, -4);

        // Initialize random max distance to spawn the bison can go.
        maxDistanceToSpawn = Random.Range(2.0f, 5.5f);

        // Hide shotgun in passive mode.
        shotgunReference.SetActive(false);
    }



    #region /*** Bison movements on map manager. ***/
    private readonly float accuracy = 1.0f;
    private float speed = 1.0f;

    private bool isWaiting = false;
    private float lastTimeTargetGeneration = 0.0f;
    private float randTimeToWaitBeforeNewTarget = 10000f;

    private float maxDistanceToSpawn = 1.0f;

    private void BisonMovementsOnMap()
    {
        Vector3 distanceToImaginaryTarget = targetPosition - transform.position;


        #region /*** Translate while not near imaginary target. ***/
        if (distanceToImaginaryTarget.magnitude > accuracy)
        {
            transform.Translate(
                translation: speed * Time.deltaTime * distanceToImaginaryTarget.normalized,
                relativeTo: Space.World);
        }
        else
        {
            #region /*** Wait when near to target. ***/
            if (!isWaiting)
            {
                isWaiting = true;

                // Generate stop time.
                lastTimeTargetGeneration = Time.time;


                if (isAngry)
                {
                    // Angry mode = change target faster.
                    randTimeToWaitBeforeNewTarget = 0.2f;
                }
                else
                {
                    // Randomize time to wait before new target. 
                    randTimeToWaitBeforeNewTarget = Random.Range(0.5f, 5.0f);
                }
            }
            #endregion /*** ^^^ Wait when near to target. ^^^ ***/
        }

        #endregion /*** ^^^ Translate while not near imaginary target. ^^^ ***/



        #region /*** Animate bison accordingly with its states. ***/
        if (isAngry)
        {
            // If angry, activate angry run and desactivate classic run.
            animator.SetBool("ClassicRun", false);
            animator.SetBool("AngryRun", isAngry);
        }
        else
        {
            // If not angry, set classic run to true if is not waiting.
            animator.SetBool("ClassicRun", distanceToImaginaryTarget.magnitude > accuracy && !isAngry && !isWaiting);
        }

        #endregion /*** ^^^ Animate bison accordingly with its states. ^^^ ***/



        #region /*** After wait time, generate new target position. ***/
        if (isWaiting && lastTimeTargetGeneration + randTimeToWaitBeforeNewTarget < Time.time)
        {
            if (isAngry)
            {
                // When angry, set larger target possibility (whole map).
                targetPosition = new Vector3(
                    x: transform.position.x + Random.Range(-4.0f, 4.0f),
                    y: transform.position.y + Random.Range(-4.0f, 4.0f),
                    z: transform.position.z);

            }
            else
            {
                Vector3 distanceToSpawnPos = spawnPosition - transform.position;

                // If is too far from spawn position, go back to spawn.
                if (distanceToSpawnPos.magnitude > maxDistanceToSpawn)
                {
                    targetPosition = spawnPosition;
                }
                else
                {
                    // If not angry, stay near is spawn pos = smaller movement possibility.
                    targetPosition = new Vector3(
                        x: transform.position.x + Random.Range(-2.0f, 2.0f),
                        y: transform.position.y + Random.Range(-2.0f, 2.0f),
                        z: transform.position.z);
                }
            }

            isWaiting = false;

        }
        #endregion /*** ^^^ After wait time, generate new target position. ^^^ ***/
    }
    #endregion /*** ^^^ Bison movements on map manager. ^^^ ***/




    #region /*** Pushing back bison to his spawn position if he is out of bounds. ***/
    private void PushingOutOfBoundBisonBackToSpawn()
    {
        Vector3 bisonPos = transform.localPosition;

        if (bisonPos.y > floorLimit ||
            bisonPos.y < -floorLimit ||
            bisonPos.x > floorLimit ||
            bisonPos.x < -floorLimit)
        {
            targetPosition = spawnPosition;
        }
    }
    #endregion /*** ^^^ Pushing back bison to his spawn position if he is out of bounds. ^^^ ***/



    #region /*** Shot at player position, after shot interval, when angry. ***/
    private float lastTimeShot = 0.0f;
    private float shotInterval = 3.0f;

    private void ShotgunMechanic()
    {
        if (isAngry && lastTimeShot + shotInterval < Time.time)
        {
            ShotgunShot();

            shotInterval = Random.Range(1.5f, 3.0f);

            lastTimeShot = Time.time;
        }
    }
    #endregion /*** ^^^ Shot at player position, after shot interval, when angry. ^^^ ***/



    #region /*** Shotgun shot in target direction + at shotgun position. ***/
    private readonly float spread = 2;
    private int bulletAmountInOneShot = 3;

    [SerializeField] private GameObject opponentBullet;

    private void ShotgunShot()
    {
        // Playing shotgun shot sound effect.
        SoundManager.Instance.PlayRandomShotgunShotSoundEffect();


        // Shoot bullets in target direction + random offset.
        for (int i = 0; i < bulletAmountInOneShot; i++)
        {
            float addedOffset = (bulletAmountInOneShot / 2 * spread) + Random.Range(-25.0f, 25.0f);

            Quaternion newRotWithOffset = Quaternion.Euler(
                x: rotation.eulerAngles.x,
                y: rotation.eulerAngles.y,
                z: rotation.eulerAngles.z + addedOffset);

            _ = Instantiate(
                original: opponentBullet,
                position: shotgunReference.transform.position,
                rotation: newRotWithOffset);

        }

        // Randomize next shot bullet amount.
        bulletAmountInOneShot = Random.Range(3, 5);
    }
    #endregion /*** ^^^ Shotgun shot in target direction + at shotgun position. ^^^ ***/



    #region /*** Flip Bison accordingly with target position (Bison faces target position = no moonwalk). ***/
    private bool loopTurningToRight = false;
    private bool loopTurningToLeft = false;

    private Vector3 UpdatedTargetPosition;

    private void FlipBisonToTargetDirection()
    {
        // Target is set to player if is angry.
        UpdatedTargetPosition = isAngry ? player.transform.localPosition : targetPosition;

        if (UpdatedTargetPosition.x < transform.position.x &&
           transform.localScale.x > 0 &&
           !loopTurningToLeft &&
           !loopTurningToRight)
        {
            loopTurningToLeft = true;
        }
        else if (UpdatedTargetPosition.x > transform.position.x &&
            transform.localScale.x < 0 &&
            !loopTurningToLeft &&
            !loopTurningToRight)
        {
            loopTurningToRight = true;
        }

        if (loopTurningToLeft)
        {
            transform.localScale = new Vector3(transform.localScale.x - 1.0f,
                transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x <= -5)
            {
                loopTurningToLeft = false;
            }
        }

        if (loopTurningToRight)
        {
            transform.localScale = new Vector3(transform.localScale.x + 1.0f,
                transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 5)
            {
                loopTurningToRight = false;
            }
        }
    }
    #endregion /*** ^^^ Flip Bison accordingly with target position. ^^^ ***/



    private Vector3 playertargetDirection;
    private Quaternion rotation;
    private GameObject player = null;

    // Update is called once per frame
    private void Update()
    {
        // Setting player if it's not already set. Needed to fix target when angry.
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }


        // Flip bison to target, when it will not mess with instanciating.
        if (!waitForAngryBreathingSecurity)
        {
            FlipBisonToTargetDirection();
        }


        #region /*** Target logic when angry. ***/
        if (isAngry)
        {

            #region /*** Compute shotgun rotation and shoot parameters. ****/
            playertargetDirection = player.transform.localPosition - transform.position;
            playertargetDirection.Normalize();

            rotation = Quaternion.Euler(
                x: 0,
                y: 0,
                z: Mathf.Atan2(playertargetDirection.y, playertargetDirection.x) * Mathf.Rad2Deg);

            #endregion /*** ^^^ Compute shotgun rotation and shoot parameters. ^^^ ****/

            RotateShotgunToTarget();

            ShotgunMechanic();

        }
        #endregion /*** ^^^ Target logic when angry. ^^^ ***/


        BisonMovementsOnMap();

        PushingOutOfBoundBisonBackToSpawn();
    }



    #region /*** Shotgun rotation mechanic. ***/
    private void RotateShotgunToTarget()
    {
        if (shotgunReference != null)
        {
            shotgunReference.transform.localRotation =
                playertargetDirection.x > 0.1f
                       ? rotation
                       : Quaternion.Euler(0, 0, Mathf.Atan2(playertargetDirection.y, -playertargetDirection.x) * Mathf.Rad2Deg);
        }
    }
    #endregion /***  ^^^ Shotgun rotation mechanic. ^^^ ***/





    #region /*** Add shotgun weapon. ***/
    [SerializeField] private GameObject shotgunReference;

    private void AddShotgun()
    {
        shotgunReference.SetActive(true);
    }
    #endregion /*** ^^^ Add shotgun weapon ^^^ ***/




    #region /*** Take damage mechanics. ***/
    private bool isAngry = false;

    private int hitBeforeAngry = 1;
    private int hitBeforeDead = -3;

    private bool isDying = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player_bullet") && !isDying)
        {
            // Call to destroy bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();

            // Play bullet hitting flesh sound effect.
            SoundManager.Instance.PlayRandomBulletHittingFleshSound();

            #region /*** If it can't handle more player's bullets = is angry. ***/
            if (hitBeforeAngry == 0 && !isAngry)
            {
                isAngry = true;

                // Update speed, bison is faster when angry.
                speed = 3.0f;

                // Call to other bison, make them angry too.
                foreach (var go in GameObject.FindGameObjectsWithTag("Bison"))
                {
                    go.GetComponent<BisonAI>().hitBeforeAngry = 0;
                    go.GetComponent<BisonAI>().isAngry = true;
                    go.GetComponent<BisonAI>().speed = 3.0f;

                    // Add shotguns to all bisons.
                    go.GetComponent<BisonAI>().AddShotgun();

                    // Add angry breathing particles to all bisons.
                    go.GetComponent<BisonAI>().CreateAngryBreathing();
                }
            }

            #endregion /*** ^^^ If it can't handle more player's bullets = is angry. ^^^ ***/


            // Decrease hit before being angry, equivalent of life points.
            hitBeforeAngry--;


            // Create angry breathing particles.
            CreateAngryBreathing();


            #region /*** Bison death. ***/
            if (hitBeforeAngry <= hitBeforeDead)
            {
                isDying = true;

                // Increase bison killed by player counter.
                ObjectsGeneration.Instance.IncreaseBisonKillCounter();

                // Bullet explosion.
                for (int i = 0; i <= 360; i += Random.Range(25, 41))
                {
                    _ = Instantiate(
                        original: opponentBullet,
                        position: transform.position,
                        rotation: Quaternion.Euler(0, 0, i));
                }

                Destroy(obj: gameObject);
            }
            #endregion /*** ^^^ Bison death. ^^^ ***/

        }
    }
    #endregion /*** ^^^ Take damage mechanics. ^^^ ***/



    #region /*** Create breathing particles for when is hit. ***/
    [SerializeField]
    private ParticleSystem
        firstAngryBreathingParticleSystem,
        secondAngryBreathingParticleSystem;

    private bool waitForAngryBreathingSecurity = false;

    public bool isReadyToBeDestroySecurity = true;

    private async void CreateAngryBreathing()
    {
        isReadyToBeDestroySecurity = false;


        if (!waitForAngryBreathingSecurity)
        {
            waitForAngryBreathingSecurity = true;

            // Scaler security to stop flip scaling error as the particle systems are children of the bison.
            if (transform.localScale.x <= -5)
            {
                transform.localScale =
                    new Vector3(
                        x: Mathf.Sign(transform.localScale.x) * transform.localScale.x,
                        y: transform.localScale.y,
                        z: transform.localScale.z
                    );
            }

            // Play particles.
            firstAngryBreathingParticleSystem.Play();
            secondAngryBreathingParticleSystem.Play();


            // Wait delay before unlocking flip to target security.
            await Task.Delay(20);
            waitForAngryBreathingSecurity = false;
        }


        isReadyToBeDestroySecurity = true;
    }
    #endregion /*** ^^^ Create breathing particles for when is hit. ^^^ ***/
}