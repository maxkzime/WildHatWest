using UnityEngine;
using TMPro;
using System.Threading.Tasks;


// TODO : BOSS CHICKEN, BIGGER CHICKEN AND FASTER, LAY EGG BULLET 
// INCREMENT MAX CHICKEN ON FIELD AT EACH BULLET HIT, CAN TAKE 30 BULLET 


public class ObjectsGeneration : MonoBehaviour, ISaveable
{

    #region /*** Initialize generator manager instance (always alive). ***/
    public static ObjectsGeneration Instance;

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
    #endregion /*** ^^^ Initialize generator instance (always alive). ^^^ ***/




    #region /*** Create new game. ***/

    private GameObject canvas;

    [SerializeField] private TMP_Text goldNuggetTextUI;

    [SerializeField] private GameObject goldNuggetUIIndicator;


    public void CreateNewGame()
    {
        #region /*** Initialize gold nugget counter on UI. ***/
        canvas = GameObject.FindGameObjectWithTag("Canvas");

        goldNuggetTextUI = Instantiate(goldNuggetTextUI);
        goldNuggetTextUI.transform.SetParent(canvas.transform, false);

        // Reset counter.
        goldNuggetTextUI.text = currentPlayerGoldNuggetBalance.ToString();

        goldNuggetUIIndicator = Instantiate(goldNuggetUIIndicator);
        goldNuggetUIIndicator.transform.SetParent(canvas.transform, false);

        #endregion /*** ^^^ Initialize gold nugget counter on UI. ^^^ ***/

        GenerateArena();


        // Initialize random spawning entities time.
        lastTimeTumbleweedGeneration = Time.time;
        lastTimeChickenGeneration = Time.time;
    }

    #endregion /*** ^^^ Create new game. ^^^ ***/




    #region /*** Generate a new arena. ***/
    private bool gameIsRunning = false;

    public int playerCurrentMaxHats = 3;

    public void GenerateArena()
    {
        CleanArena();

        GenerateFloor();

        GeneratePlayButtonObstacle();

        GenerateMapBounds();

        GenerateRocks();

        GenerateCactus();

        // Instantiate one random tutorial object at each game.
        GenerateOneRandomTutorialObjects();

        // Unlock cactus outlaw when player has enough hat.
        if (playerCurrentMaxHats > 6)
        {
            GenerateCactusOutlaw();
        }

        GeneratePlayer();


        // Unlock bison groups only after the second upgrade on floor size.
        if (FloorScaleFactor >= 6)
        {
            GenerateBisonGroups();
        }


        gameIsRunning = true;

        GenerateIndian();
    }
    #endregion /*** ^^^ Generate a new arena. ^^^ ***/




    #region /*** Generate player. ***/

    [SerializeField] private GameObject playerReference;
    private GameObject playerInstance;

    private void GeneratePlayer()
    {
        playerInstance = Instantiate(
            original: playerReference,
            position: Vector3.zero,
            rotation: Quaternion.identity);
    }
    #endregion /*** ^^^ Generate player. ^^^ ***/




    #region /*** Shop management parameters. ***/

    #region /*** Shop management : Magnet. ***/
    public bool HasUnlockedMagnet { get; set; } = false;
    public float MagnetLevel { get; set; } = 1.0f;

    #endregion /*** ^^^ Shop management : Magnet. ^^^ **/



    #region /*** Shop management : Double gun. ***/
    public bool HasUnlockedDoubleGun { get; set; } = false;

    #endregion /*** ^^^ Shop management : Double gun. ^^^ ***/



    #region /*** Shop management : Shotgun. ***/
    public bool HasUnlockedShotgun { get; set; } = false;
    public int ShotgunReloadAmount { get; set; } = 3;

    #endregion /*** ^^^ Shop management : Shotgun. ^^^ ***/

    #endregion /*** ^^^ Shop management parameters. ^^^ ***/



    #region /*** Player gold nugget balance management. **/
    private int currentPlayerGoldNuggetBalance = 0;

    public void IncrementCurrentPlayerGoldNuggetBalance()
    {
        currentPlayerGoldNuggetBalance++;
        goldNuggetTextUI.text = currentPlayerGoldNuggetBalance.ToString();
    }

    public bool EditCurrentPlayerGoldNuggetBalance(int objectPrice)
    {
        bool operationComplete = false;

        if (currentPlayerGoldNuggetBalance - objectPrice >= 0)
        {
            currentPlayerGoldNuggetBalance -= objectPrice;
            goldNuggetTextUI.text = currentPlayerGoldNuggetBalance.ToString();

            operationComplete = true;
        }

        return operationComplete;
    }
    #endregion /*** ^^^ Player gold nugget balance management. ^^^ **/



    #region /*** Indian generation. ***/
    [SerializeField] private GameObject indianReference;

    public bool HasUnlockedIndian { get; set; } = false;
    public int IndianNumber { get; set; } = 1;
    public int IndianDistanceToOpponentToShoot { get; set; } = 5;

    private void GenerateIndian()
    {
        if (HasUnlockedIndian)
        {
            for (int i = 0; i < IndianNumber; i++)
            {
                Vector3 tempPos = new Vector3(
                    0 + Random.Range(-1.5f, 1.5f),
                    0 + Random.Range(-1.5f, 1.5f),
                    0);

                GameObject tempIndian = Instantiate(
                    original: indianReference,
                    position: tempPos,
                    rotation: Quaternion.identity,
                    parent: transform);

                tempIndian.name = "Indian " + i;
            }
        }
    }
    #endregion /*** ^^^ Indian generation. ^^^ ***/




    #region /*** Bison generation. ***/
    [SerializeField] private GameObject bisonReference;

    [SerializeField] private GameObject grassFloorReference;

    private GameObject bisonEmpty = null;

    private void GenerateBisonGroups()
    {
        // Empty generation to order objects.
        if (bisonEmpty == null)
        {
            bisonEmpty = new GameObject("BisonGroups");
            bisonEmpty.transform.SetParent(transform);
        }

        // Between 1 and 2 bison groups on the map.
        int bisonGroupNumber = Random.Range(1, 3);

        for (int i = 0; i < bisonGroupNumber; i++)
        {
            int bisonNumberInGroup = Random.Range(2, 5);

            float spawnLimit = (GetFloorSize() / 2) - 2;

            Vector3 groupSpawnPos = new(
                x: Random.Range(-spawnLimit, spawnLimit),
                y: Random.Range(-spawnLimit, spawnLimit),
                z: 0);

            // Grass floor for bison spawn point.
            GameObject tempGrassFloor = Instantiate(
                original: grassFloorReference,
                position: groupSpawnPos,
                rotation: Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f)),
                parent: bisonEmpty.transform);

            tempGrassFloor.name = "bison grass floor : group = " + i;

            // Bison generation.
            for (int j = 0; j < bisonNumberInGroup; j++)
            {
                Vector3 tempPos = new(
                    x: groupSpawnPos.x + Random.Range(-1.5f, 1.5f),
                    y: groupSpawnPos.y + Random.Range(-1.5f, 1.5f),
                    z: 0);

                GameObject tempBison = Instantiate(
                    original: bisonReference,
                    position: tempPos,
                    rotation: Quaternion.identity,
                    parent: bisonEmpty.transform);

                tempBison.name = "bison : group = " + i + " id = " + j;
            }
        }
    }
    #endregion /*** ^^^ Bison generation. ^^^ ***/




    #region /*** Update loop : random entities generation. (Chicken & Tumbleweed) ***/
    private float lastTimeChickenGeneration = 0.0f;
    private float nextChickenGenerationOffsetTime = 10.0f;

    private float lastTimeTumbleweedGeneration = 0.0f;
    private float tumbleweedGenerationTimeOffset = 2.0f;

    private void Update()
    {
        if (gameIsRunning)
        {
            #region /*** Tumbleweed random spawning. ***/
            if (lastTimeTumbleweedGeneration + tumbleweedGenerationTimeOffset < Time.time)
            {
                lastTimeTumbleweedGeneration = Time.time;
                tumbleweedGenerationTimeOffset = Random.Range(2.0f, 5.0f);

                GenerateTumbleweed();
            }

            #endregion /*** ^^^ Tumbleweed random spawning. ^^^ ***/


            #region /*** Chicken random spawning. ***/
            if (lastTimeChickenGeneration + nextChickenGenerationOffsetTime < Time.time)
            {
                lastTimeChickenGeneration = Time.time;
                nextChickenGenerationOffsetTime = Random.Range(10.0f, 60.0f);

                GenerateChicken();
            }
            #endregion /*** ^^^ Chicken random spawning. ^^^ ***/
        }
    }
    #endregion /*** ^^^ Update loop : random entities generation. (Chicken & Tumbleweed) ^^^ ***/




    #region /*** Clean arena + add shop with unity animator. Called when player die. ***/
    public bool displayingShop = false;

    [SerializeField] private GameObject shop;
    public void GenerateShop()
    {
        gameIsRunning = false;

        if (!displayingShop)
        {
            displayingShop = true;

            CleanArena();

            shop.GetComponent<Animator>().SetTrigger("DisplayShop");
        }
    }
    #endregion /*** ^^^ Clean arena + add shop with unity animator. Called when player die. ^^^ ***/


    private void LateUpdate()
    {
        if (!gameIsRunning)
        {
            CleanArenaWithUpdateNeeded();
        }
    }


    #region /*** Delete every objects on arena that need to end process (async process). ***/
    private void CleanArenaWithUpdateNeeded()
    {
        #region /*** Remove player. ***/
        if (playerInstance != null)
        {
            if (playerInstance.GetComponent<PlayerShoot>().ReloadFinished)
            {
                #region /*** Removing Bullet indicators. ***/
                foreach (var gameObj in GameObject.FindGameObjectsWithTag("BulletIndicator"))
                {
                    Destroy(gameObj);
                }
                #endregion /*** ^^^ Removing Bullet indicators. ^^^ ***/

                Destroy(playerInstance);
            }
        }
        #endregion /*** ^^^ Remove player. ^^^ ***/


        #region /*** Removing Bison. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("Bison"))
        {
            if (gameObj.GetComponent<BisonAI>().isReadyToBeDestroySecurity)
            {
                Destroy(gameObj);
            }
        }
        #endregion /*** ^^^ Removing Bison. ^^^ ***/


        #region /*** Removing Indian. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("Indian"))
        {
            if (gameObj.GetComponent<IndianAI>().isReadyToBeDestroySecurity)
            {
                Destroy(gameObj);
            }
        }
        #endregion /*** ^^^ Removing Indian. ^^^ ***/


        #region /*** Removing Cactus Outlaw. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("CactusOutlaw"))
        {
            if (gameObj.GetComponent<CactusOutlawBehavior>().isReadyToBeDestroySecurity)
            {
                Destroy(gameObj);
            }
        }
        #endregion /*** ^^^ Removing Cactus Outlaw. ^^^ ***/


        #region /*** Removing Boss. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Boss"))
        {
            if (gameObj.GetComponent<BossAI>().isReadyToBeDestroySecurity <= 0)
            {
                bossIsOnField = false;
                Destroy(gameObj);
            }
        }
        #endregion /*** ^^^ Removing Boss. ^^^ ***/



        #region /*** Removing Opponents. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Opponent"))
        {
            if (gameObj.GetComponent<OpponentAI>().isReadyToBeDestroySecurity)
            {
                Destroy(gameObj);
            }
        }

        if (OpponentsOnField > 0)
        {
            OpponentsOnField = 0;
        }
        #endregion /*** ^^^ Removing Opponents. ^^^ ***/



        #region /*** Removing old goldnuggets and new ones that could be created by async process. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("GoldNugget"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing old goldnuggets and new ones that could be created by async process. ^^^ ***/


        #region /*** Removing old bullets and new ones that could be created by async process. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("Player_bullet"))
        {
            Destroy(gameObj);
        }
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("Opponent_bullet"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing old bullets and new ones that could be created by async process. ^^^ ***/


        #region /*** Removing hats. ***/
        if (!bossIsOnField)
        {
            foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Hat"))
            {
                Destroy(gameObj);
            }
        }
        #endregion /*** ^^^ Removing hats. ^^^ ***/

    }
    #endregion /*** ^^^ Delete every objects on arena that need to end process (async process). ^^^ ***/




    #region /*** Delete every objects on arena that doesnt need to end process (no async process). ***/
    private void CleanArena()
    {
        #region /*** Removing small rocks. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("Rock"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing small rocks. ^^^ ***/


        #region /*** Removing big rocks. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("BigRock"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing big rocks. ^^^ ***/


        #region /*** Removing Bison Grass floor. ***/
        foreach (var gameObj in GameObject.FindGameObjectsWithTag("BisonGrassFloor"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing Bison Grass floor. ^^^ ***/


        #region /*** Removing tumbleweeds. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Tumbleweed"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing tumbleweeds. ^^^ ***/


        #region /*** Removing cactus. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Cactus"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing cactus. ^^^ ***/


        #region /*** Removing tutorial objects. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("TutorialObject"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing tutorial objects. ^^^ ***/


        #region /*** Removing bounds. ***/
        Destroy(bound_1);
        Destroy(bound_2);
        Destroy(bound_3);
        Destroy(bound_4);
        #endregion /*** ^^^ Removing bounds. ^^^ ***/


        #region /*** Removing floor. ***/
        Destroy(floorInstance);
        #endregion /*** ^^^ Removing floor. ^^^ ***/


        #region /*** Removing chicken. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Chicken"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing chicken. ^^^ ***/


        #region /*** Removing eggbullet. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Egg_bullet"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing eggbullet. ^^^ ***/


        #region /*** Removing grave. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Grave"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing grave. ^^^ ***/


        #region /*** Removing play button. ***/
        Destroy(playButtonInstance);
        #endregion /*** ^^^ Removing play button. ^^^ ***/



        #region /*** Removing player's footprints. ***/
        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("FootPrints"))
        {
            Destroy(gameObj);
        }
        #endregion /*** ^^^ Removing player's footprints. ^^^ ***/



    }
    #endregion /***  ^^^ Delete every objects on arena that doesnt need to end process (no async process). ^^^ ***/





    #region /*** Generate chicken. ***/
    [SerializeField] private GameObject chickenReference;
    private int chickensOnField = 0;
    private int maxChickenOnField = 6;

    private void GenerateChicken()
    {
        if (chickensOnField < maxChickenOnField)
        {
            int r = Random.Range(1, 4);

            Vector3 tempPos;

            float spawnLimit = (GetFloorSize() / 2) - 1.5f;

            switch (r)
            {
                case 1:
                    tempPos = new Vector3(spawnLimit, Random.Range(-spawnLimit, spawnLimit), 0);
                    break;
                case 2:
                    tempPos = new Vector3(-spawnLimit, Random.Range(-spawnLimit, spawnLimit), 0);
                    break;
                case 3:
                    tempPos = new Vector3(Random.Range(-spawnLimit, spawnLimit), spawnLimit, 0);
                    break;
                default:
                    tempPos = new Vector3(Random.Range(-spawnLimit, spawnLimit), -spawnLimit, 0);
                    break;
            }


            GameObject tempChicken = Instantiate(
                original: chickenReference,
                position: tempPos,
                rotation: Quaternion.identity);

            tempChicken.name = "chicken " + chickensOnField;

            chickensOnField++;
        }
    }
    #endregion /*** Generate Chicken. ***/




    #region /*** Decrease chicken on fied counter by one. ***/
    public void DecreaseChickensOnField() => chickensOnField--;
    #endregion /*** ^^^ Decrease chicken on fied counter by one. ^^^ ***/



    #region /*** Generate floor + scale it + random color. ***/ 
    [SerializeField] private GameObject floorReference;

    private GameObject floorInstance;

    private void GenerateFloor()
    {
        floorInstance = Instantiate(
            original: floorReference,
            position: transform.position,
            rotation: Quaternion.identity);

        floorInstance.transform.localScale = new Vector3(
            x: FloorScaleFactor,
            y: FloorScaleFactor,
            z: 1);

        // Random color.
        floorInstance.GetComponent<SpriteRenderer>().color = new Color(
                r: Random.Range(0.80f, 0.85f),
                g: Random.Range(0.68f, 0.80f),
                b: 0.5f);
    }

    #endregion /*** ^^^ Generate floor + scale it + random color. ^^^ ***/



    #region /*** Floor size parameters. ***/
    public int FloorScaleFactor { get; set; } = 4;

    private int floorSystemScaleFactor = 5;

    public int GetFloorSize()
    {
        return floorSystemScaleFactor * FloorScaleFactor;
    }
    #endregion /*** ^^^ Floor size parameters. ^^^ ***/



    #region /*** Generate play button obstacle at random pos. ***/ 
    [SerializeField] private GameObject playButtonReference;
    private GameObject playButtonInstance;

    private void GeneratePlayButtonObstacle()
    {
        playButtonInstance =
            Instantiate(
                original: playButtonReference,
                position: new Vector3(
                    x: Random.Range((-GetFloorSize() / 2) + 2, (GetFloorSize() / 2) - 2),
                    y: Random.Range((-GetFloorSize() / 2) + 2, (GetFloorSize() / 2) - 2),
                    z: 0),
                rotation: Quaternion.identity
                );
    }

    #endregion /*** ^^^ Generate play button obstacle at random pos. ^^^ ***/



    #region /*** Generate map bounds (rocks limits). ***/
    [SerializeField] private GameObject boundReference;

    private GameObject bound_1, bound_2, bound_3, bound_4;

    private GameObject boundsEmpty = null;

    private void GenerateMapBounds()
    {
        #region /*** Create empty to order objects. ***/
        if (boundsEmpty == null)
        {
            boundsEmpty = new GameObject("Map Bounds");
            boundsEmpty.transform.SetParent(transform);
        }
        #endregion /*** ^^^ Create empty to order objects. ^^^ ***/


        Vector2 boundSize = new(x: 0.35f,
            y: FloorScaleFactor < 5 ?
                FloorScaleFactor - 1
                :
                FloorScaleFactor > 6 ?
                    FloorScaleFactor - 2
                    :
                    FloorScaleFactor - 1.5f);

        Vector2 boundColliderSize = new(0.18f, FloorScaleFactor - 1);

        bound_1 = Instantiate(
            original: boundReference,
            position: new Vector3(GetFloorSize() / 2, 0, 0),
            rotation: Quaternion.identity,
            parent: boundsEmpty.transform);
        bound_1.GetComponent<SpriteRenderer>().size = boundSize;
        bound_1.GetComponent<BoxCollider2D>().size = boundColliderSize;


        bound_2 = Instantiate(
            original: boundReference,
            position: new Vector3(-GetFloorSize() / 2, 0, 0),
            rotation: Quaternion.identity,
            parent: boundsEmpty.transform);
        bound_2.GetComponent<SpriteRenderer>().size = boundSize;
        bound_2.GetComponent<BoxCollider2D>().size = boundColliderSize;


        bound_3 = Instantiate(
            original: boundReference,
            position: new Vector3(0, GetFloorSize() / 2, 0),
            rotation: Quaternion.Euler(0, 0, 90),
            parent: boundsEmpty.transform);
        bound_3.GetComponent<SpriteRenderer>().size = boundSize;
        bound_3.GetComponent<BoxCollider2D>().size = boundColliderSize;


        bound_4 = Instantiate(
            original: boundReference,
            position: new Vector3(0, -GetFloorSize() / 2, 0),
            rotation: Quaternion.Euler(0, 0, -90),
            parent: boundsEmpty.transform);
        bound_4.GetComponent<SpriteRenderer>().size = boundSize;
        bound_4.GetComponent<BoxCollider2D>().size = boundColliderSize;

    }
    #endregion /*** ^^^ Generate map bounds (rocks limits). ^^^ ***/



    #region /*** Tumbleweed generation. ***/
    [SerializeField] private GameObject tumbleweedReference;
    private GameObject tumbleweedInstance;
    private GameObject tumbleweedEmptyParent = null;

    private void GenerateTumbleweed()
    {
        // Create empty to order objects.
        if (tumbleweedEmptyParent == null)
        {
            tumbleweedEmptyParent = new GameObject("TumbleweedGroup");
            tumbleweedEmptyParent.transform.SetParent(transform);
        }

        Vector3 spawnPos = Random.value > 0.5f
            ? new Vector3(-GetFloorSize() / 2, Random.Range((-GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1), 0)
            : new Vector3(GetFloorSize() / 2, Random.Range((-GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1), 0);


        tumbleweedInstance = Instantiate(
                original: tumbleweedReference,
                position: spawnPos,
                rotation: Quaternion.identity);

        // Link to parent object.
        tumbleweedInstance.transform.SetParent(tumbleweedEmptyParent.transform);
    }
    #endregion /*** ^^^ Tumbleweed generation. ^^^ ***/



    #region /*** Cactus generation. ***/
    [SerializeField] private GameObject cactusReference;
    private GameObject cactusInstance;
    private GameObject cactusEmptyParent = null;

    private void GenerateCactus()
    {
        // Create empty to order objects
        if (cactusEmptyParent == null)
        {
            cactusEmptyParent = new GameObject("CactusGroup");
            cactusEmptyParent.transform.SetParent(transform);
        }

        // Random cactus amount depending of the floor size
        int cactusNumber = (int)Random.Range(GetFloorSize() / 3, (GetFloorSize() / 1.4f) + 1);


        for (int i = 0; i < cactusNumber; i++)
        {
            cactusInstance = Instantiate(
                original: cactusReference,
                position: new Vector3(
                    i > cactusNumber / 2 ?
                    Random.Range(0.0f, (GetFloorSize() / 2) - 1)
                    : Random.Range(-(GetFloorSize() / 2) + 1, 0.0f),
                    Random.Range(-(GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1),
                    0),
                rotation: Quaternion.identity
                );

            cactusInstance.name = "Cactus " + i;

            cactusInstance.transform.SetParent(p: cactusEmptyParent.transform);
        }
    }
    #endregion /*** ^^^ Cactus generation. ^^^ ***/



    #region /*** Cactus Outlaw generation. ***/
    [SerializeField] private GameObject cactusOutlawReference;
    private GameObject cactusOutlawInstance;
    private GameObject cactusOutlawEmptyParent = null;

    private void GenerateCactusOutlaw()
    {
        // Create empty to order objects.
        if (cactusOutlawEmptyParent == null)
        {
            cactusOutlawEmptyParent = new GameObject("CactusOutlawGroup");
            cactusOutlawEmptyParent.transform.SetParent(transform);
        }

        // Random cactus amount depending of the floor size.
        int cactusOutlawNumber = Random.Range(GetFloorSize() / 16, GetFloorSize() / 6);

        if (Random.value > 0.5f)
        {
            for (int i = 0; i < cactusOutlawNumber; i++)
            {
                cactusOutlawInstance = Instantiate(
                    original: cactusOutlawReference,
                    position: new Vector3(
                        i > cactusOutlawNumber / 2 ?
                        Random.Range(0.0f, (GetFloorSize() / 2) - 1)
                        : Random.Range(-(GetFloorSize() / 2) + 1, 0.0f),
                        Random.Range(-(GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1),
                        0),
                    rotation: Quaternion.identity
                    );

                cactusOutlawInstance.name = "Cactus Outlaw " + i;

                cactusOutlawInstance.transform.SetParent(cactusOutlawEmptyParent.transform);
            }
        }
    }
    #endregion /*** ^^^ Cactus Outlaw generation. ^^^ ***/




    #region /*** Generate rocks. ***/
    [SerializeField] private GameObject rockReference;

    [SerializeField] private GameObject bigRockReference;

    private GameObject rocksEmptyParent = null;

    private void GenerateRocks()
    {
        // Create empty to order objects.
        if (rocksEmptyParent == null)
        {
            rocksEmptyParent = new GameObject("RocksGroup");
            rocksEmptyParent.transform.SetParent(transform);
        }


        #region /*** Small rocks. ***/
        int rockNumber = Random.Range(10, 21);

        for (int i = 0; i < rockNumber; i++)
        {
            GameObject temp = Instantiate(
                rockReference,
                new Vector3(
                    i > rockNumber / 2 ? Random.Range(0.0f, (GetFloorSize() / 2) - 1) : Random.Range(-(GetFloorSize() / 2) + 1, 0.0f),
                    Random.Range(-(GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1),
                    0),
                Quaternion.identity
                );

            temp.name = "rock " + i;

            temp.transform.SetParent(rocksEmptyParent.transform);
        }
        #endregion /*** ^^^ Small rocks. ^^^ ***/


        #region /*** Big rocks. ***/
        int bigRockNumber = rockNumber / 4;

        for (int i = 0; i < bigRockNumber; i++)
        {
            GameObject temp = Instantiate(
                bigRockReference,
                new Vector3(
                    i > rockNumber / 2 ? Random.Range(0.0f, (GetFloorSize() / 2) - 1) : Random.Range(-(GetFloorSize() / 2) + 1, 0.0f),
                    Random.Range(-(GetFloorSize() / 2) + 1, (GetFloorSize() / 2) - 1),
                    0),
                Quaternion.identity
                );

            temp.name = "bigRock " + i;

            temp.transform.SetParent(rocksEmptyParent.transform);
        }
        #endregion /*** ^^^ Big rocks. ^^^ ***/

    }
    #endregion /*** ^^^ Generate rocks. ^^^ ***/




    #region /*** Generate Boss Mechanics. ***/
    private bool bossIsOnField = false;

    public bool GetBossIsOnField() => bossIsOnField;

    public async void SetBossIsOnField(bool value)
    {
        // If boss was on field and he is no more, put opponent spawning back after delay.
        if (GetBossIsOnField())
        {
            // Wait in ms before spawning opponent back.
            await Task.Delay(5000);

            // We need to change the value first to allow the initialization next, else it will not initialize the new opponent.
            bossIsOnField = value;

            // Initialize new opponent wave.
            SpawnNewOpponent();
        }
        else
        {
            bossIsOnField = value;
        }
    }


    #region /*** Add boss if not already on field. ***/
    [SerializeField] private GameObject bossOpponentReference;

    private GameObject bossOnField = null;

    private void GenerateBossOpponent()
    {
        if (bossOnField == null && !GetBossIsOnField())
        {
            SetBossIsOnField(true);

            bossOnField =
                Instantiate(
                    original: bossOpponentReference,
                    position: GenerateRandomSpawnPosOnBounds(),
                    rotation: Quaternion.identity
                    );

            bossOnField.name = "Boss";
        }
    }

    #endregion /*** ^^^ Add boss if not already on field. ^^^ ***/


    #endregion /*** ^^^ Generate Boss Mechanics. ^^^ ***/



    #region /*** Return a random position on the bounds of the map, use this pos to spawn entities. ***/
    private Vector3 GenerateRandomSpawnPosOnBounds()
    {
        Vector3 pos = Vector3.zero;

        int r = Random.Range(1, 5);

        switch (r)
        {
            case 1:
                pos = new Vector3(
                        (GetFloorSize() / 2) + 1,
                        Random.Range(-(GetFloorSize() / 2) - 1, (GetFloorSize() / 2) + 1),
                        0);
                break;
            case 2:
                pos = new Vector3(
                        -(GetFloorSize() / 2) - 1,
                        Random.Range(-(GetFloorSize() / 2) - 1, (GetFloorSize() / 2) + 1),
                        0);
                break;
            case 3:
                pos = new Vector3(
                        Random.Range(-(GetFloorSize() / 2) - 1, (GetFloorSize() / 2) + 1),
                        (GetFloorSize() / 2) + 1,
                        0);
                break;
            case 4:
                pos = new Vector3(
                        Random.Range(-(GetFloorSize() / 2) - 1, (GetFloorSize() / 2) + 1),
                        -(GetFloorSize() / 2) - 1,
                        0);
                break;
        }

        return pos;
    }

    #endregion /*** ^^^ Return a random position on the bounds of the map, use this pos to spawn entities. ^^^ ***/




    #region /*** Random chance to generate one or more opponent when one die. ***/
    public void SpawnNewOpponent()
    {
        switch (Random.Range(1, 31))
        {
            case < 29:
                GenerateAnOpponent();
                break;
            default:
                GenerateAnOpponent();
                GenerateAnOpponent();
                break;
        }
    }
    #endregion /*** ^^^ Random chance to generate one or more opponent when one die. ^^^ ***/




    #region /*** Generate an opponent. ***/
    [SerializeField] private GameObject opponentReference;

    private readonly int maxOpponentOnField = 120;

    public void GenerateAnOpponent()
    {
        if (OpponentsOnField < maxOpponentOnField &&
            !GetBossIsOnField())
        {
            GameObject tempOpponent;
            tempOpponent = Instantiate(
                original: opponentReference,
                position: GenerateRandomSpawnPosOnBounds(),
                rotation: Quaternion.identity
                );

            tempOpponent.name = "opponent " + OpponentsOnField;

            OpponentsOnField++;
        }
    }
    #endregion /*** ^^^ Generate an opponent. ^^^ ***/




    #region /*** Setter for opponentOnField (decrease by one, when an opponent is killed). ***/
    private int OpponentsKilledByPlayer = 0;
    public int OpponentsOnField { get; set; } = 0;

    private int opponentKilledCounterForTrophy = 0;

    [SerializeField] private AudioClip bossEntrySoundEffect;

    public void DecreaseOpponentsOnField()
    {
        OpponentsOnField--;

        OpponentsKilledByPlayer++;

        // Generate boss when the player killed 150 opponents.
        if (OpponentsKilledByPlayer > 150)
        {
            // Reset opponents killed by player counter.
            OpponentsKilledByPlayer = 0;


            #region /*** Beat thousand opponents trophy mechanic. ***/
            opponentKilledCounterForTrophy++;

            if (opponentKilledCounterForTrophy >= 7)
            {
                // Unlock beat thousand opponents trophy.
                TrophiesManager.Instance.AddBeatThousandOpponentsTrophy();
            }
            #endregion /*** ^^^ Beat thousand opponents trophy mechanic. ^^^ ***/


            GenerateBossOpponent();

            // Play Boss Entry sound effect.
            SoundManager.Instance.PlaySound(bossEntrySoundEffect, 1.0f);
        }
    }
    #endregion /*** ^^^ Setter for opponentOnField (decrease by one, when an opponent is killed). ^^^ ***/




    #region /*** Tutorial objects generation. ***/

    #region /*** Generate one random tutorial objects. ***/

    [Header("Tutorial objects references :")]
    [SerializeField] [Tooltip("Arrow reference.")] private GameObject arrowReference;
    [SerializeField] [Tooltip("Reload reference.")] private GameObject reloadReference;
    [SerializeField] [Tooltip("Shoot reference.")] private GameObject shootReference;
    [SerializeField] [Tooltip("Switch weapon reference.")] private GameObject switchWeaponReference;

    private GameObject tutorialObjectsEmptyParent = null;

    private void GenerateOneRandomTutorialObjects()
    {
        // Create empty to order objects.
        if (tutorialObjectsEmptyParent == null)
        {
            tutorialObjectsEmptyParent = new GameObject("TutorialObjectsGroup");
            tutorialObjectsEmptyParent.transform.SetParent(transform);
        }


        int randObject = Random.Range(0, 4);

        switch (randObject)
        {
            case 0:
                GenerateArrowsTutorialObjects();
                break;
            case 1:
                GenerateReloadTutorialObject();
                break;
            case 2:
                GenerateShootTutorialObject();
                break;
            case 3:
                if (HasUnlockedShotgun)
                {
                    GenerateSwitchWeaponTutorialObject();
                }
                else
                {
                    // Generate another tips if has not unlocked second weapon.
                    GenerateOneRandomTutorialObjects();
                }
                break;
            default:
                break;
        }

    }
    #endregion /*** ^^^ Generate one random tutorial objects. ^^^ ***/



    #region /*** Generate Arrows tutorial objects. ***/
    private void GenerateArrowsTutorialObjects()
    {
        float angle = 0f;
        float xPos = Random.Range(-2.0f, 2.0f);
        float yPos = Random.Range(-2.0f, 2.0f);

        for (int i = 0; i < 4; i++)
        {
            if (i >= 3)
            {
                yPos++;
                xPos -= 2f;
            }

            GameObject tempArrow = Instantiate(
                original: arrowReference,
                position: new Vector3(xPos, yPos, 0),
                rotation: Quaternion.Euler(0, 0, angle)
                );

            tempArrow.name = "Tutorial object arrow : " + i;

            tempArrow.transform.SetParent(tutorialObjectsEmptyParent.transform);

            angle += 90;
            xPos++;
        }
    }
    #endregion /*** Generate Arrows tutorial objects. ***/



    #region /*** Generate reload tutorial objects. ***/
    private void GenerateReloadTutorialObject()
    {
        float xPos = Random.Range(-2.0f, 2.0f);
        float yPos = Random.Range(-2.0f, 2.0f);

        GameObject tempReload = Instantiate(
            original: reloadReference,
            position: new Vector3(xPos, yPos, 0),
            rotation: Quaternion.Euler(0, 0, Random.Range(-15.0f, 15.0f))
            );

        tempReload.name = "Tutorial object reload.";

        tempReload.transform.SetParent(tutorialObjectsEmptyParent.transform);
    }
    #endregion /*** Generate reload tutorial objects. ***/



    #region /*** Generate shoot tutorial objects. ***/
    private void GenerateShootTutorialObject()
    {
        float xPos = Random.Range(-2.0f, 2.0f);
        float yPos = Random.Range(-2.0f, 2.0f);

        GameObject tempShoot = Instantiate(
            original: shootReference,
            position: new Vector3(xPos, yPos, 0),
            rotation: Quaternion.Euler(0, 0, Random.Range(-15.0f, 15.0f))
            );

        tempShoot.name = "Tutorial object shoot.";

        tempShoot.transform.SetParent(tutorialObjectsEmptyParent.transform);
    }
    #endregion /*** Generate shoot tutorial objects. ***/



    #region /*** Generate switch weapon tutorial objects. ***/
    private void GenerateSwitchWeaponTutorialObject()
    {
        float xPos = Random.Range(-2.0f, 2.0f);
        float yPos = Random.Range(-2.0f, 2.0f);

        GameObject tempSwitch = Instantiate(
            original: switchWeaponReference,
            position: new Vector3(xPos, yPos, 0),
            rotation: Quaternion.Euler(0, 0, Random.Range(-15.0f, 15.0f))
            );

        tempSwitch.name = "Tutorial object switch weapon.";

        tempSwitch.transform.SetParent(tutorialObjectsEmptyParent.transform);
    }
    #endregion /*** Generate switch weapon tutorial objects. ***/

    #endregion /*** ^^^ Tutorial objects generation. ^^^ ***/



    #region /*** Bison kill counter. ***/
    private int bisonKilledByPlayer = 0;
    public void IncreaseBisonKillCounter()
    {
        bisonKilledByPlayer++;

        if (bisonKilledByPlayer >= 50 && bisonKilledByPlayer < 51)
        {
            // Unlock bison trophy.
            TrophiesManager.Instance.AddBeatFiftyBisonTrophy();
        }
    }
    #endregion /*** ^^^ Bison kill counter. ^^^ ***/









    #region /*** Save and load general data system. ***/


    #region /*** Save data in new struct and return it to save in file. ***/
    public object SaveState()
    {
        // Not saving bisonKilledByPlayer, as the player has to kill 50 bison in one game to unlock the trophy.

        return new SaveData()
        {
            opponentKilledCounterForTrophy = opponentKilledCounterForTrophy,

            floorScaleFactor = FloorScaleFactor,

            hasUnlockedIndian = HasUnlockedIndian,
            indianNumber = IndianNumber,
            indianDistanceToOpponentToShoot = IndianDistanceToOpponentToShoot,

            currentPlayerGoldNuggetBalance = currentPlayerGoldNuggetBalance,

            playerCurrentMaxHats = playerCurrentMaxHats,

            hasUnlockedMagnet = HasUnlockedMagnet,
            magnetLevel = MagnetLevel,
            hasUnlockedDoubleGun = HasUnlockedDoubleGun,
            hasUnlockedShotgun = HasUnlockedShotgun,
            shotgunReloadAmount = ShotgunReloadAmount

        };
    }
    #endregion /*** ^^^ Save data in new struct and return it to save in file. ^^^ ***/



    #region /*** Load saved data. ***/
    public void LoadState(object state)
    {
        var saveData = (SaveData)state;

        opponentKilledCounterForTrophy = saveData.opponentKilledCounterForTrophy;

        FloorScaleFactor = saveData.floorScaleFactor;

        HasUnlockedIndian = saveData.hasUnlockedIndian;
        IndianNumber = saveData.indianNumber;
        IndianDistanceToOpponentToShoot = saveData.indianDistanceToOpponentToShoot;


        currentPlayerGoldNuggetBalance = saveData.currentPlayerGoldNuggetBalance;

        playerCurrentMaxHats = saveData.playerCurrentMaxHats;

        HasUnlockedMagnet = saveData.hasUnlockedMagnet;
        MagnetLevel = saveData.magnetLevel;
        HasUnlockedDoubleGun = saveData.hasUnlockedDoubleGun;
        HasUnlockedShotgun = saveData.hasUnlockedShotgun;
        ShotgunReloadAmount = saveData.shotgunReloadAmount;

    }
    #endregion /*** Load saved data. ***/



    #region /*** Struct storing data to save. ***/
    [System.Serializable]
    private struct SaveData
    {
        public int opponentKilledCounterForTrophy;
        public int floorScaleFactor;

        public bool hasUnlockedIndian;
        public int indianNumber;
        public int indianDistanceToOpponentToShoot;

        public int currentPlayerGoldNuggetBalance;


        public int playerCurrentMaxHats;


        public bool hasUnlockedMagnet;
        public float magnetLevel;
        public bool hasUnlockedDoubleGun;
        public bool hasUnlockedShotgun;
        public int shotgunReloadAmount;

    }
    #endregion /*** ^^^ Struct storing data to save. ^^^ ***/

    #endregion /*** ^^^ Save and load general data system. ^^^ ***/
}
