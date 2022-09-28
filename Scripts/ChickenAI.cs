using UnityEngine;

public class ChickenAI : MonoBehaviour
{
    private float lastTimeTargetGeneration = 0.0f;
    private Vector3 targetPosition;

    private void Start()
    {
        lastTimeTargetGeneration = Time.time;

        // Target direction intialization.
        targetPosition = new Vector3(
            x: transform.position.x + Random.Range(-5.0f, 5.0f),
            y: transform.position.y + Random.Range(-5.0f, 5.0f),
            z: transform.position.z);


        floorLimit = (ObjectsGeneration.Instance.GetFloorSize() / 2) - 1;
    }



    private Vector3 distanceToTarget;
    private float accuracy = 1.0f;
    private bool isWaiting = false;

    private float speed = 2.0f;

    private float randTimeDistance = 10000f;


    private void FixedUpdate()
    {

        FlipChickenToTargetPos();


        #region /*** Chicken movement on the map. ***/

        distanceToTarget = targetPosition - transform.position;

        // Translate while not near target.
        if (distanceToTarget.magnitude > accuracy)
        {
            transform.Translate(
                translation: speed * Time.deltaTime * distanceToTarget.normalized,
                relativeTo: Space.World);
        }
        else
        {
            if (!isWaiting)
            {
                isWaiting = true;

                lastTimeTargetGeneration = Time.time;

                // Generate new random speed if not hit by player.
                if (!hasBeenHitOnceByPlayer)
                {
                    randTimeDistance = Random.Range(0.5f, 5.0f);
                    speed = Random.Range(0.5f, 2.0f);
                }
            }
        }


        // Set Run to true if is still moving.
        GetComponent<Animator>().SetBool("Run", distanceToTarget.magnitude > accuracy && !isWaiting);


        #region /*** After stop time, generate new target direction + generate egg bullet if has been shot once by player. ***/
        if (isWaiting && lastTimeTargetGeneration + randTimeDistance < Time.time)
        {
            isWaiting = false;

            LayEggBullet();

            targetPosition = new Vector3(
                x: transform.position.x + Random.Range(-7.0f, 7.0f),
                y: transform.position.y + Random.Range(-7.0f, 7.0f),
                z: transform.position.z);
        }
        #endregion /*** ^^^ After stop time, generate new target direction + generate egg bullet if has been shot once by player. ^^^ ***/


        OutOfBoundsSecurity();

        #endregion /*** Chicken movement on the map. ***/
    }



    #region /*** Chicken out of bounds = recalculate target direction. ***/

    private Vector3 currentPos;
    private int floorLimit;

    private void OutOfBoundsSecurity()
    {
        currentPos = transform.localPosition;

        if (currentPos.y > floorLimit ||
            currentPos.y < -floorLimit ||
            currentPos.x > floorLimit ||
            currentPos.x < -floorLimit)
        {
            targetPosition = new Vector3(
                x: 0 - (transform.position.x / 2),
                y: 0 - (transform.position.y / 2),
                z: transform.position.z);
        }
    }
    #endregion /*** ^^^ Chicken out of bounds = recalculate target direction. ^^^ ***/




    #region /*** Flip opponent accordingly with target position. ***/
    private bool loopTurningToRight = false;
    private bool loopTurningToLeft = false;

    private void FlipChickenToTargetPos()
    {
        if (targetPosition.x > transform.position.x &&
           transform.localScale.x > 0 &&
           !loopTurningToLeft &&
           !loopTurningToRight)
        {
            loopTurningToLeft = true;
        }
        else if (targetPosition.x < transform.position.x &&
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

            if (transform.localScale.x <= -4)
            {
                loopTurningToLeft = false;
            }
        }

        if (loopTurningToRight)
        {
            transform.localScale = new Vector3(transform.localScale.x + 1.0f,
                transform.localScale.y, transform.localScale.z);

            if (transform.localScale.x >= 4)
            {
                loopTurningToRight = false;
            }
        }
    }
    #endregion /*** ^^^ Flip opponent accordingly with target position. ^^^ ***/



    #region /*** Lay egg bullet. ***/
    [SerializeField] private GameObject eggBullet;

    private void LayEggBullet()
    {
        if (hasBeenHitOnceByPlayer)
        {
            _ = Instantiate(
              original: eggBullet,
              position: transform.position,
              rotation: Quaternion.identity);
        }

    }
    #endregion /*** ^^^ Lay egg bullet. ^^^ ***/



    private bool hasBeenHitOnceByPlayer = false;
    private bool hasBeenHitSecondByPlayer = false;

    [SerializeField] private GameObject opponentBullet;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player_bullet"))
        {
            // Call to destroy bullet.
            collider.GetComponent<BulletLife>().InitBulletDeath();

            // Trigger flash effect when taking damage.
            GetComponent<FlashEffect>().Flash();


            #region /*** First hit by player's bullet. ***/
            if (!hasBeenHitOnceByPlayer)
            {
                hasBeenHitOnceByPlayer = true;

                SoundManager.Instance.PlayRandomAngryChickenSound();

                speed = 5.0f;
                randTimeDistance = 0.1f;
            }
            #endregion /*** ^^^ First hit by player's bullet. ^^^ ***/
            #region /*** Second hit by player's bullet : death. ***/
            else if(!hasBeenHitSecondByPlayer)
            {
                hasBeenHitSecondByPlayer = true;

                Destroy(GetComponent<BoxCollider2D>());

                LayEggBullet();

                // Star shaped bullet explosion.
                _ = Instantiate(opponentBullet, transform.position, Quaternion.Euler(0, 0, 0));
                _ = Instantiate(opponentBullet, transform.position, Quaternion.Euler(0, 0, 90));
                _ = Instantiate(opponentBullet, transform.position, Quaternion.Euler(0, 0, 180));
                _ = Instantiate(opponentBullet, transform.position, Quaternion.Euler(0, 0, 225));
                _ = Instantiate(opponentBullet, transform.position, Quaternion.Euler(0, 0, 315));


                ObjectsGeneration.Instance.DecreaseChickensOnField();

                // Increment chicken killed counter for chicken trophy.
                TrophiesManager.Instance.IncrementChickenKilledCounter();


                Destroy(obj: gameObject);
            }
            #endregion /*** ^^^ Second hit by player's bullet : death. ^^^ ***/
        }
    }
}
