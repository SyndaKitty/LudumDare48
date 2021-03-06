using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    // Configuration
    public float FallingSpeed;
    public float MovementSpeed;
    public float DrillInputThreshold = 0.5f;
    public float FallingBlockWalkGap = 0.75f;
    public float FallingBlockKillGap = 0.3f;
    public int LeftMost = 0;
    public int RightMost = 8;
    public GameObject BreakingParticlesSourcePrefab;
    public float LandingTime = 0.2f;
    public Image[] CreditImages;
    
    public Sprite[] UINUmbers;
    public Image[] CoinNumbers;

    public RectTransform SummaryScreen;
    public RectTransform CoinCounter;
    public Sound CoinSoundPrefab;
    public Sound DeathSoundPrefab;
    public AudioSource MusicSource;
    public AudioSource GameOverSoundPrefab;
    public SpriteRenderer DrillOver;
    public SpriteRenderer GrandTotal;
    public float MusicFadeSpeed;
    float startingVolumeMusic;
    public SpriteCounter FinalScoreDeathCounter;

    // Runtime
    public bool grounded = false;
    
    bool _drilling;
    public bool drilling {
        get => _drilling;
        set {
            if (_drilling != value) {
                _drilling = value;
                if (value)
                    source.Play();
                else source.Stop();
            }
        }
    }
    
    float drillTime;
    public bool moving = false;
    public bool falling = false;
    public bool dead = false;
    public bool landing = false;
    public bool victory = false;
    bool start = true;
    float landingTimer;
    int nextBottom;
    SpriteRenderer blackoutPanel;
    Reveal PressAnyText;
    AudioSource source;

    Animator animator;
    GridObject drillingBlock;

    float nextFallCheck;

    GridManager grid;
    float timer;

    Vector2Int Location;
    public Vector2Int Left => Location.Offset(Vector2Int.left);
    public Vector2Int Right => Location.Offset(Vector2Int.right);
    public Vector2Int Down => Location.Offset(Vector2Int.down);
    public Vector2Int Up => Location.Offset(Vector2Int.up);

    GameObject lastSource;
    float musicT;

    void Start() {
        defaultCoinCounterPos = CoinCounter.anchoredPosition;
        defaultSummaryPanelPos = SummaryScreen.anchoredPosition;
        startingVolumeMusic = MusicSource.volume;
        source = GetComponent<AudioSource>();
        grid = GridManager.Instance;
        Location = transform.position.xy().RoundToInt();
        transform.position = Location.ToFloat();
        animator = GetComponent<Animator>();
        SetState(State.Idle);
        blackoutPanel = transform.Find("BlackoutPanel").GetComponent<SpriteRenderer>();

        nextBottom = -FindObjectOfType<LevelGenerator>().Height - 10;

        PressAnyText = FindObjectOfType<Reveal>();
        cameraFollow = FindObjectOfType<CameraFollow>();
    }

    enum State {
        Idle = 0, 
        Walking = 1,
        
        Windup = 2,
        Drill = 3,
        
        WindupUp = 4,
        DrillUp = 5,
        
        WindupDown = 6,
        DrillDown = 7,

        Falling = 8,
        Landing = 9,

        Dead = 10
    }

    void SetState(State state) {
        if (animator.GetInteger("State") != (int)state)
            animator.SetInteger("State", (int)state);
    }    

    void GridMovement() {
        if (falling) {
            transform.position += Vector3.down * Time.deltaTime * FallingSpeed;
            if (transform.position.y <= nextFallCheck) {
                transform.position = Location.ToFloat();
                var nextGridPos = Location + Vector2Int.down;

                if (nextGridPos.y <= nextBottom) {
                    Victory();
                }

                if (grid.HasGridObject(nextGridPos)) {
                    grounded = true;
                    falling = false;
                    landing = true;
                    SetState(State.Landing);
                }
                else {
                    Location = nextGridPos;
                    nextFallCheck--;
                }
            }

            return;
        }
        
        var fallingGrid = grid.GetGridObject(Location);
        if (fallingGrid) {
            float y = 0;
            if (fallingGrid.Falling) {
                y = fallingGrid.transform.position.y - fallingGrid.Location.y;
            }
            if (y < FallingBlockKillGap) {
                dead = true;
                FinalScoreDeathCounter.Show();
                source.Stop();
                Instantiate(DeathSoundPrefab);
                Instantiate(GameOverSoundPrefab);
                SetState(State.Dead);
                return;
            }
        }

        if (landing) {
            landingTimer += Time.deltaTime;
            if (landingTimer >= LandingTime) {
                landing = false;
                landingTimer = 0;
            }
        }

        if (moving) {
            var target = Location.ToFloat();
            transform.position = Vector3.MoveTowards(transform.position, target, MovementSpeed * Time.deltaTime);
            if ((transform.position.xy() - target).magnitude < 0.01f) {
                transform.position = target;
                moving = false;
                SetState(State.Idle);
            }
            return;
        }

        if (drilling) {
            var delta = drillingBlock.Location - Location;
            drillingBlock.UpdateBreakTexture(delta, timer / drillTime);

            // Drill cancel
            var input = new Vector2Int();
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            if (h < -DrillInputThreshold) {
                input.x = -1;
            }
            else if (h > DrillInputThreshold) {
                input.x = 1;
            }
            else if (v > DrillInputThreshold) {
                input.y = 1;
            }
            else if (v < -DrillInputThreshold) {
                input.y = -1;
            }
            if (delta != input && input != Vector2Int.zero) {
                drilling = false;
                timer = 0;
                drillingBlock = null;
                if (lastSource != null)
                    lastSource.GetComponent<Lifetime>().Amount = 0;
            }
            else {
                timer += Time.deltaTime;
                if (timer >= drillTime) {
                    timer = 0;
                    drilling = false;

                    drillingBlock.Drill();
                }
                return;
            }
        }

        // Check for ground
        var groundGridPos = Location.xy();
        groundGridPos.y--;
        if (!grid.HasGridObject(groundGridPos)) {
            grounded = false;
            falling = true;
            nextFallCheck = groundGridPos.y;
            Location = groundGridPos;

            SetState(State.Falling);
            animator.SetTrigger("Fall");
            return;
        }

        void MoveTowardsBlock(Vector2Int pos) {
            var box = grid.GetGridObject(pos);
            if (!box) return;
            if (!box.Falling) {
                drillTime = grid.GetGridObject(pos).DrillTime;
                drilling = true;
                drillingBlock = box;
                
                if (box.HasParticles) {
                    lastSource = Instantiate(BreakingParticlesSourcePrefab, (transform.position + pos.WithZ(0).ToFloat()) * 0.5f, Quaternion.identity); 
                    var force = 2 * (transform.position - pos.WithZ(0).ToFloat());
                    var source = lastSource.GetComponent<BreakingParticlesSource>();
                    source.Force = force; 
                    source.BlockColor = box.BlockColor;
                    source.GetComponent<Lifetime>().Amount = drillTime;
                }

                var delta = pos - Location;
                if (delta.y < 0) {
                    SetState(State.WindupDown);
                }
                else if (delta.y > 0) {
                    SetState(State.WindupUp);
                }
                else SetState(State.Windup);

            }
            else if (box.transform.position.y - box.Location.y > FallingBlockWalkGap && box.Location.y == Location.y) {
                var below = grid.GetGridObject(pos.Offset(Vector2Int.down));
                if (!below || !below.Falling) {
                    moving = true;
                    Location = pos;
                }
            }
        }

        if (Input.GetAxisRaw("Horizontal") < -DrillInputThreshold) { 
            transform.localScale = Vector3.one.WithX(-1);
            if (grid.HasGridObject(Left)) {
                MoveTowardsBlock(Left);
            }
            else {
                if (Left.x >= LeftMost) {
                    // Move left
                    moving = true;
                    Location = Left;
                }
            }
        }
        else if (Input.GetAxisRaw("Horizontal") > DrillInputThreshold) {
            transform.localScale = Vector3.one.WithX(1);
            if (grid.HasGridObject(Right)) {
                // Drill right
                MoveTowardsBlock(Right);
            }
            else {
                if (Right.x <= RightMost) {
                    // Move right
                    moving = true;
                    Location = Right;
                }
            }
        }
        else if (Input.GetAxisRaw("Vertical") < -DrillInputThreshold) {
            MoveTowardsBlock(Down);
        }
        else if (Input.GetAxisRaw("Vertical") > DrillInputThreshold) {
            MoveTowardsBlock(Up);
        }

        if (moving) {
            SetState(State.Walking);
        }
        else if (drilling) {
            
        }
        else if (landing) {

        }
        else {
            SetState(State.Idle);
        }
    }

    public float DeathFadeSpeed = 0.5f;
    float deadTimer = 0;
    bool cameraUnassigned = true;
    CameraFollow cameraFollow;

    public float SummaryScreenSwapSpeed = 1;
    float summaryT = 0;

    Vector2 defaultCoinCounterPos;
    Vector2 defaultSummaryPanelPos;

    public float CoinCounterMoveAmount;
    public float SummaryPanelMoveAmount;
    bool coinCollectedThisFrame = false;

    public Counter CoinCounterNumber;
    public Counter CoinScoreCounter;
    public Counter TimeCounter;
    public Counter TimeScoreCounter;
    public Counter TotalScore;
    public Reveal SummaryPressAny;

    float secondsTaken = 0;
    int score = 0;

    IEnumerator ScoreCard() {
        CoinCounterNumber.SetAmount(coinCount);
        int coinScore = coinCount * 100;
        if (coinScore <= 0) coinScore = 1;
        CoinScoreCounter.SetAmount(coinScore);
        yield return new WaitUntil(() => CoinCounterNumber.Done);

        TimeCounter.SetAmount((int)secondsTaken);
        int timerScore = 5400 - (int)secondsTaken * 30;
        if (timerScore <= 0) timerScore = 1;
        TimeScoreCounter.SetAmount(timerScore);
        
        yield return new WaitUntil(() => TimeScoreCounter.Done && TimeCounter.Done);
        
        var beforeScore = score;
        score += coinScore + timerScore;

        // Set score on UI
        FinalScoreDeathCounter.SetAmount(score);
        FinalScoreDeathCounter.Show();

        TotalScore.SetAmount(score, beforeScore);

        yield return new WaitUntil(() => TotalScore.Done);
        SummaryPressAny.StartReveal();

        yield return new WaitUntil(() => Input.anyKey);
        victory = false;
        GridManager.Instance.Clear();
        nextBottom = LevelGenerator.Instance.Generate(Location.y - 15) - 15;
        SummaryPressAny.Hide(); // Doesn't fucking work apparently

        secondsTaken = 0;
        SetCoinCount(0);
    }

    bool counting;

    void Update() {

        if (!victory) {
            secondsTaken += Time.deltaTime;
            counting = false;
        }

        coinCollectedThisFrame = false;
        if (victory) {
            summaryT = Mathf.Clamp01(summaryT + Time.deltaTime * SummaryScreenSwapSpeed);
            if (summaryT == 1 && !counting) {
                counting = true;
                StartCoroutine(ScoreCard());
            }
        }
        else {
            summaryT = Mathf.Clamp01(summaryT - Time.deltaTime * SummaryScreenSwapSpeed);
            if (summaryT == 0) {
                CoinCounterNumber.Reset();
                CoinScoreCounter.Reset();
                TimeCounter.Reset();
                TimeScoreCounter.Reset();
                TotalScore.Reset();
                TotalScore.UpdateNumbers(score);
            }
        }
        float a = -summaryT * summaryT + 2 * summaryT;

        SummaryScreen.anchoredPosition = Vector2.Lerp(defaultSummaryPanelPos, defaultSummaryPanelPos + new Vector2(SummaryPanelMoveAmount, 0), a);
        CoinCounter.anchoredPosition = Vector2.Lerp(defaultCoinCounterPos, defaultCoinCounterPos + new Vector2(CoinCounterMoveAmount, 0), a);

        if (dead) {
            source.Stop();
            musicT = Mathf.Clamp01(musicT - Time.deltaTime * MusicFadeSpeed);
            MusicSource.volume = musicT * startingVolumeMusic;
        }

        if (start) {
            if (Input.anyKeyDown) {
                start = false;
                transform.localScale = Vector3.one;
                foreach (var image in CreditImages) {
                    var lt = image.gameObject.AddComponent<Lifetime>();
                    lt.Amount = 1f;
                    lt.FadeAlpha = true;
                }
            }
            return;
        }
        if (cameraUnassigned) {
            if (transform.position.y < cameraFollow.transform.position.y) {
                cameraFollow.Follow = transform;
                cameraUnassigned = true;
            }
        }
        if (!dead) {
            FinalScoreDeathCounter.Hide();
            deadTimer = Mathf.Clamp01(deadTimer - Time.deltaTime * DeathFadeSpeed);
            DrillOver.color = new Color(DrillOver.color.r, DrillOver.color.g, DrillOver.color.b, 0);
            GrandTotal.color = new Color(GrandTotal.color.r, GrandTotal.color.g, GrandTotal.color.b, 0);
            if (deadTimer < .001f) {
                blackoutPanel.sortingOrder = 1;
                DrillOver.sortingOrder = 2;
                GrandTotal.sortingOrder = 2;
            }
            GridMovement();
        }
        else {
            deadTimer = Mathf.Clamp01(deadTimer + Time.deltaTime * DeathFadeSpeed);

            if (deadTimer > 0.8f) {
                PressAnyText.StartReveal();
            }

            if (deadTimer > 0.99f && Input.anyKey) {
                PressAnyText.Hide();
                ResetLevel();
            }
            
            DrillOver.color = new Color(DrillOver.color.r, DrillOver.color.g, DrillOver.color.b, deadTimer);
            GrandTotal.color = new Color(GrandTotal.color.r, GrandTotal.color.g, GrandTotal.color.b, deadTimer);
        }
        blackoutPanel.color = new Color(blackoutPanel.color.r, blackoutPanel.color.g, blackoutPanel.color.b, deadTimer);
    }

    public void ResetLevel() {
        SetCoinCount(0);
        MusicSource.volume = startingVolumeMusic;
        MusicSource.Play();
        LevelGenerator.Instance.Reset();
        blackoutPanel.sortingOrder = 3;
        DrillOver.sortingOrder = 3;
        GrandTotal.sortingOrder = 3;
        GridManager.Instance.Clear();
        Location = new Vector2Int(4, 1);
        transform.position = Location.ToFloat();
        SetState(State.Idle);
        animator.SetTrigger("Reset");
        grounded = true;
        drilling = false;
        moving = false;
        falling = false;
        dead = false;
        landing = false;
        secondsTaken = 0;
        
        nextBottom = LevelGenerator.Instance.Generate(0);
    }

    int coinCount;
    void CollectCoin() {
        if (!coinCollectedThisFrame) {
            coinCollectedThisFrame = true;
            var sound = Instantiate(CoinSoundPrefab);
            sound.Pitch = Random.Range(.9f, 1.1f);
        }
        
        SetCoinCount(coinCount+1);
    }

    void SetCoinCount(int newCoinCount) {
        coinCount = Mathf.Clamp(newCoinCount, 0, 999);
        int h = coinCount / 100;
        int t = (coinCount / 10) % 10;
        int u = coinCount % 10;
        CoinNumbers[0].sprite = UINUmbers[h];
        CoinNumbers[1].sprite = UINUmbers[t];
        CoinNumbers[2].sprite = UINUmbers[u];
    }

    void OnCollisionEnter2D(Collision2D other) {
        Destroy(other.gameObject);
        CollectCoin();
    }

    public void Victory() {
        if (victory) return;
        victory = true;
    }
}