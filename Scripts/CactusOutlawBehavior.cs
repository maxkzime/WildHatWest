using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CactusOutlawBehavior : MonoBehaviour
{
    private int hitBeforeDeath;

    private int floorLimit;

    // Classic security for when we clean arena.
    public bool isReadyToBeDestroySecurity = true;


    private void Start()
    {
        // Random player's bullet it can handle before dying.
        hitBeforeDeath = Random.Range(6, 12);

        floorLimit = (ObjectsGeneration.Instance.GetFloorSize() / 2) - 2;

        // Target position intialization.
        pointToReachPosition = new Vector3(
            x: transform.position.x + Random.Range(-2.0f, 2.0f),
            y: transform.position.y + Random.Range(-2.0f, 2.0f),
            z: transform.position.z);


        // Assert weapon are hidden.
        firstGun.SetActive(false);
        secondGun.SetActive(false);
    }



    private GameObject target = null;
    private Vector3 targetToShootPosition;
    private Vector3 pointToReachPosition;
    private Vector3 targetDirection;
    private Quaternion rotation;

    private void Update()
    {
        if (canDie)
        {
            Destroy(obj: gameObject);
        }

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        if (target != null && hitOnceByPlayer && !isDying)
        {
            #region /*** Compute target parameters. ***/
            targetToShootPosition = target.transform.localPosition;

            targetDirection = targetToShootPosition - transform.position;

            rotation = Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.normalized.y, targetDirection.normalized.x) * Mathf.Rad2Deg);

            #endregion ^^^ /*** Compute target parameters. ^^^ ***/


            FlipToTargetDirection();

            MovementsMechanic();

            ShotMechanic();

            DoubleGunRotation();
        }
    }



    #region /*** Double gun rotation to target position ***/
    private void DoubleGunRotation()
    {
        if (firstGun != null && secondGun != null)
        {
            firstGun.transform.localRotation =
                targetDirection.x > 0.1f
                       ? rotation
                       : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, -targetDirection.x) * Mathf.Rad2Deg);

            secondGun.transform.localRotation =
                targetDirection.x > 0.1f
                       ? rotation
                       : Quaternion.Euler(0, 0, Mathf.Atan2(targetDirection.y, -targetDirection.x) * Mathf.Rad2Deg);
        }
    }
    #endregion /*** ^^^ Double gun rotation to target position ^^^ ***/



    #region /*** Cactus outlaw movements on the map. ***/
    private float accuracy = 1.0f;
    private float speed = 5.0f;

    private bool up = true, down = false, left = false, right = false;

    private Vector3 currentPos;

    private void MovementsMechanic()
    {
        currentPos = transform.position;

        Vector3 distanceToTarget = pointToReachPosition - currentPos;

        // Translate while not near target.
        if (distanceToTarget.magnitude > accuracy)
        {
            transform.Translate(
                translation: speed * Time.deltaTime * distanceToTarget.normalized,
                relativeTo: Space.World);
        }
        else
        {
            // Square shaped path.

            float xPosOffset = 0f;
            float yPosOffset = 0f;

            if (up)
            {
                up = false;
                yPosOffset = Random.Range(2.0f, 8.0f);
                left = true;
            }
            else if (down)
            {
                down = false;
                yPosOffset = Random.Range(-8.0f, -2.0f);
                right = true;
            }
            else if (right)
            {
                right = false;
                xPosOffset = Random.Range(-8.0f, -2.0f);
                up = true;
            }
            else if (left)
            {
                left = false;
                xPosOffset = Random.Range(2.0f, 8.0f);
                down = true;
            }

            // Generate new target direction.
            pointToReachPosition = new Vector3(
                x: currentPos.x + xPosOffset,
                y: currentPos.y + yPosOffset,
                z: currentPos.z);
        }


        if (currentPos.y > floorLimit ||
            currentPos.y < -floorLimit ||
            currentPos.x > floorLimit ||
            currentPos.x < -floorLimit)
        {
            pointToReachPosition = Vector3.zero;
        }
    }
    #endregion /*** ^^^ Cactus outlaw movements on the map. ^^^ ***/



    #region /*** Shoot at player position with shot interval. ***/
    private float lastTimeShot = 0.0f;
    private float shotInterval = 3.0f;

    private void ShotMechanic()
    {
        if (lastTimeShot + shotInterval < Time.time && !isDying)
        {
            GunShot();

            shotInterval = Random.Range(1.0f, 3.0f);

            lastTimeShot = Time.time;
        }
    }
    #endregion /*** ^^^ Shoot at player position with shot interval. ^^^ ***/



    #region /*** Double Gun shot in target direction + at guns positions. ***/
    private int bulletAmountInOneShot = 3;

    [SerializeField] private GameObject opponentBulletReference;

    private async void GunShot()
    {
        isReadyToBeDestroySecurity = false;


        // Shoot bullet in target direction.
        for (int i = 0; i < bulletAmountInOneShot; i++)
        {
            if (isDying)
                break;

            if (firstGun != null)
            {
                _ = Instantiate(
                    original: opponentBulletReference,
                    position: firstGun.transform.position,
                    rotation: rotation);

                // Playing gun shot sound effect.
                SoundManager.Instance.PlayRandomGunShotSoundEffect();
            }

            if (secondGun != null)
            {
                _ = Instantiate(
                    original: opponentBulletReference,
                    position: secondGun.transform.position,
                    rotation: rotation);

                // Playing gun shot sound effect.
                SoundManager.Instance.PlayRandomGunShotSoundEffect();
            }

            // Wait in ms before next bullet.
            await Task.Delay(200);

        }

        // Randomize next shot bullet amount.
        bulletAmountInOneShot = Random.Range(4, 7);



        isReadyToBeDestroySecurity = true;
    }
    #endregion /*** ^^^ Double Gun shot in target direction + at guns positions. ^^^ ***/



    #region /*** Flip cactus outlaw accordingly with target position (Always faces target position). ***/
    private bool loopTurningToRight = false;
    private bool loopTurningToLeft = false;

    private void FlipToTargetDirection()
    {
        if (targetToShootPosition.x < transform.position.x &&
           transform.localScale.x > 0 &&
           !loopTurningToLeft &&
           !loopTurningToRight)
        {
            loopTurningToLeft = true;
        }
        else if (targetToShootPosition.x > transform.position.x &&
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
    #endregion /*** ^^^ Flip cactus outlaw accordingly with target position (Always faces target position). ^^^ ***/




    #region /*** Player hitting cactus outlaw. ***/
    private bool hitOnceByPlayer = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player_bullet") && !isDying)
        {
            // Call to destroy bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();

            CreateCactusBlood(Random.Range(3, 5));

            // Play bullet hitting flesh sound effect.
            SoundManager.Instance.PlayRandomBulletHittingFleshSound();

            if (!hitOnceByPlayer)
            {
                hitOnceByPlayer = true;

                // Add double Gun.
                AddDoubleGun();

                GetComponent<Animator>().SetTrigger("Uncover");
            }
            else
            {
                // Decrease life points.
                hitBeforeDeath--;

                // Leave gold nugget explosion behind him after each hit.
                _ = GenerateGoldNuggetExplosion(Random.Range(3, 5));


                // If he has no more life points = death.
                if (hitBeforeDeath <= 0)
                {
                    CactusOutlawDeath();
                }

            }
        }
    }
    #endregion /*** ^^^ Player hitting cactus outlaw. ^^^ ***/



    #region /*** Start run animation. Called in animation event, at the end of uncover animation. ***/
    private void StartRunAnimation() => GetComponent<Animator>().SetTrigger("Run");

    #endregion /*** ^^^ Start run animation. Called in animation event, at the end of uncover animation. ^^^ ***/



    #region /*** Generate cactus blood : green particles explosion. ***/
    [SerializeField] private GameObject cactusBloodParticleSystemEffect;

    private void CreateCactusBlood(int repeat = 1)
    {
        for (int i = 0; i < repeat; i++)
        {
            // Instanciating cactus blood particle system effect.
            _ = Instantiate(
                original: cactusBloodParticleSystemEffect,
                position: transform.position,
                rotation: Quaternion.identity);
        }
    }
    #endregion /*** ^^^ Generate cactus blood : green particles explosion. ^^^ ***/



    #region /*** Add double gun ***/
    [Header("Guns: ")]
    [SerializeField] private GameObject firstGun;
    [SerializeField] private GameObject secondGun;

    private void AddDoubleGun()
    {
        if (firstGun != null && !firstGun.activeInHierarchy)
        {
            firstGun.SetActive(true);
        }

        if (secondGun != null && !secondGun.activeInHierarchy)
        {
            secondGun.SetActive(true);
        }
    }

    #endregion /*** ^^^ Add double gun ^^^ ***/



    #region /*** Generate gold nugget explosion. ***/
    [SerializeField]
    private GameObject goldNuggetReference;

    private async Task<Task> GenerateGoldNuggetExplosion(int goldNuggetAmount)
    {
        isReadyToBeDestroySecurity = false;


        for (int i = 0; i < goldNuggetAmount; i++)
        {
            if (gameObject == null) break;

            GameObject gn = Instantiate(
                original: goldNuggetReference,
                position: transform.position,
                rotation: Quaternion.identity);

            gn.GetComponent<GoldNuggetBehavior>().SetIsPartOfExplosion(true);


            // Wait in ms before next bullet.
            await Task.Delay(100);
        }



        isReadyToBeDestroySecurity = true;

        return Task.CompletedTask;
    }
    #endregion /*** ^^^ Generate gold nugget explosion. ^^^ ***/



    #region /*** Cactus Outlaw death. ***/
    private bool isDying = false;
    private bool canDie = false;
    private async void CactusOutlawDeath()
    {
        isDying = true;

        for (int i = -360; i <= 360; i += Random.Range(20, 35))
        {

            #region /*** Force security to avoid conflict with previous gold nugget explosion process. ***/
            if (isReadyToBeDestroySecurity)
            {
                isReadyToBeDestroySecurity = false;
            }
            #endregion /*** ^^^ Force security to avoid conflict with previous gold nugget explosion process. ^^^ ***/


            _ = Instantiate(
                original: opponentBulletReference,
                position: firstGun.transform.position,
                rotation: Quaternion.Euler(0, 0, i));

            _ = Instantiate(
                original: opponentBulletReference,
                position: secondGun.transform.position,
                rotation: Quaternion.Euler(0, 0, i));

            CreateCactusBlood(3);

            // Wait in ms before next bullet.
            await Task.Delay(175);
        }

        Task gnExplosionTask = GenerateGoldNuggetExplosion(10);
        await gnExplosionTask;

        if (gnExplosionTask.IsCompleted)
        {
            canDie = true;
            CreateCactusBlood(5);
        }

        isReadyToBeDestroySecurity = true;
    }
    #endregion /*** ^^^ Cactus Outlaw death. ^^^ ***/
}