using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    STARTING,
    RUNNING,
    PAUSED,
    WINNING,
    LOSING,
}

public class GameController : MonoBehaviour
{
    public bool InstantiateLevel = true;
    public GameState GameState;

    [Header("Grid System")]
    public int GridX = 20;
    public int GridY = 25;

    [Header("Resolution and Display")]
    public Camera WorldCamera;
    public int TargetX = 160;
    public int TargetY = 200;
    public Material RenderTexture;
    private float pixelRatioAdjustment;

    public float CameraLerpSpeed = 0.1f;
    public Vector3 CameraPositionOffset = new Vector3(10, 12, 0);
    public int PlayerVectorMultiplier = 20;
    public Transform gameGrid;
    private Player player;

    [Header("Phase Barrier Attributes")]
    public GameObject PhaseBarrierPrefab;
    public List<GameObject> PhaseBarrierList;
    public List<GameObject> ActiveEmitterList;
    public List<GameObject> InactiveEmitterList;
    private float nextPhaseBarrierSpawnTime;
    private float nextPhaseBarrierDeactivateTime;

    [Header("Game Balance")]
    public float PhaseBarrierSpawnTime = 0.75f;
    public float PhaseBarrierDeactivateTime = 1.5f;
    public float RandomSpawnChance = 0.5f;
    public int BarrierDamageAmount = 5;
    public int RobotDamageAmount = 1;
    public float BarrierDamageTime = 0.5f;
    public int CurrentCannonPower;
    public int CannonShotEnergy = 3;
    public float CannonCooldownTime;
    public float CannonRechargeTime;
    public bool CannonCooldown;

    public int DeactivatEmitterScore = 200;
    public int DestroyBarrierScore = 50;
    public int LevelClearScore = 1000;

    private float nextBarrierDamageTime;
    private float nextCannonCooldownTime;
    private float nextCannonRechargeTime;

    [Header("UI Elements")]
    public Transform HealthBar;
    public Transform PowerBar;
    public RectTransform LivesImage;
    public int HealthBarWidth = 24;
    public int PowerBarWidth = 24;
    public int LivesImageTile = 10;
    public Text ScoreText;
    public Text LevelText;
    public Text RemainingText;
    public Image CoverImage;

    [Header("Game State Timing")]
    public float StartTime = 1f;
    public float PauseFadeTime = 0.2f;
    public float WinTime = 1f;
    public float LoseTime = 1f;
    private float nextStateChange;
    private bool hasStarted = false;
    [Header("Levels")]
    public GameObject[] Levels;

    // Use this for initialization
    void Start()
    {
        InitializeCamera();
        if (InstantiateLevel)
            Instantiate(Levels[ScoreManager.Instance.Level-1], new Vector3(0, 0, 0), Quaternion.identity);
        nextPhaseBarrierSpawnTime = Time.time + PhaseBarrierSpawnTime;
        nextPhaseBarrierDeactivateTime = Time.time + PhaseBarrierDeactivateTime;
        nextBarrierDamageTime = Time.time + BarrierDamageTime;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        gameGrid = FindObjectOfType<Grid>().transform;
        Vector3 cameraTargetPosition = (player.transform.position + CameraPositionOffset
                                + ((Vector3)player.LastMoveVector * PlayerVectorMultiplier));
        WorldCamera.transform.position = new Vector3(Mathf.RoundToInt(cameraTargetPosition.x), Mathf.Round(cameraTargetPosition.y),
                                             Mathf.Round(cameraTargetPosition.y - 150));
        CurrentCannonPower = ScoreManager.Instance.MaxCannonPower;
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
        LevelText.text = string.Format("LEVEL {0}", ScoreManager.Instance.Level);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameState)
        {
            case GameState.STARTING:
                if (!hasStarted)
                {
                    hasStarted = true;
                    CrossFadeAlphaWithCallBack(CoverImage, 0f, StartTime, delegate
                    {
                        SwitchState(GameState.RUNNING);
                    });
                };
                break;
            case GameState.RUNNING:
                CameraFollow();
                if (Input.GetButtonDown("Pause"))
                {
                    SwitchState(GameState.PAUSED);
                    CrossFadeAlphaWithCallBack(CoverImage, 0.5f, PauseFadeTime, delegate { });
                }
                break;
            case GameState.PAUSED:
                if (Input.GetButtonDown("Pause"))
                {
                    SwitchState(GameState.PAUSED);
                    CrossFadeAlphaWithCallBack(CoverImage, 0f, PauseFadeTime, delegate
                    {
                        SwitchState(GameState.RUNNING);
                    });
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Win();
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Lose();
        }
        if (Time.time >= nextPhaseBarrierSpawnTime)
        {
            nextPhaseBarrierSpawnTime = Time.time + PhaseBarrierSpawnTime;
            StartCoroutine("CreatePhaseBarrier");
        }
        if (Time.time >= nextPhaseBarrierDeactivateTime)
        {
            nextPhaseBarrierDeactivateTime = Time.time + PhaseBarrierDeactivateTime;
            StartCoroutine("RemovePhaseBarrier");
        }
        if (CannonCooldown)
        {
            if (Time.time > nextCannonCooldownTime)
            {
                CannonCooldown = false;
                nextCannonRechargeTime = Time.time + CannonRechargeTime;
            }
        }
        else
        {
            if (Time.time > nextCannonRechargeTime)
            {
                if (CurrentCannonPower < ScoreManager.Instance.MaxCannonPower)
                {
                    CurrentCannonPower++;
                }
                nextCannonRechargeTime = Time.time + CannonRechargeTime;
            }
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        int currentHealthBarWidth = Mathf.RoundToInt(ScoreManager.Instance.HitPoints * HealthBarWidth / ScoreManager.Instance.MaxHitPoints);
        HealthBar.localScale = new Vector3(currentHealthBarWidth, 1, 1);
        int currentPowerBarWidth = Mathf.RoundToInt(CurrentCannonPower * PowerBarWidth / ScoreManager.Instance.MaxCannonPower);
        PowerBar.localScale = new Vector3(currentPowerBarWidth, 1, 1);
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
        RemainingText.text = string.Format("REMAINING: {0}", ActiveEmitterList.Count);
        LivesImage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, LivesImageTile * ScoreManager.Instance.Lives);
    }

    private void CameraFollow()
    {
        Vector3 cameraTargetPosition = (player.transform.position + CameraPositionOffset
                                        + ((Vector3)player.LastMoveVector * PlayerVectorMultiplier));
        Vector3 newCameraPosition = Vector3.Lerp(WorldCamera.transform.position, cameraTargetPosition, CameraLerpSpeed);
        WorldCamera.transform.position = new Vector3(Mathf.RoundToInt(newCameraPosition.x), Mathf.Round(newCameraPosition.y),
                                                     Mathf.Round(newCameraPosition.y - 150));
        gameGrid.position = new Vector3(gameGrid.position.x, gameGrid.position.y, player.transform.position.z);
    }

    private void InitializeCamera()
    {
        pixelRatioAdjustment = (float)TargetX / (float)TargetY;
        if (pixelRatioAdjustment <= 1)
        {
            RenderTexture.mainTextureScale = new Vector2(pixelRatioAdjustment, 1);
            RenderTexture.mainTextureOffset = new Vector2((1 - pixelRatioAdjustment) / 2, 0);
            WorldCamera.orthographicSize = TargetY / 2;
        }
        else
        {
            pixelRatioAdjustment = 1f / pixelRatioAdjustment;
            RenderTexture.mainTextureScale = new Vector2(1, pixelRatioAdjustment);
            RenderTexture.mainTextureOffset = new Vector2(0, (1 - pixelRatioAdjustment) / 2);
            WorldCamera.orthographicSize = TargetX / 2;
        }
    }

    IEnumerator CreatePhaseBarrier()
    {
        List<GameObject> tempEmitterList = new List<GameObject>(ActiveEmitterList);
        foreach (GameObject emitter in tempEmitterList)
        {
            bool spawnedBarrier = true;
            if (Random.value < RandomSpawnChance)
                spawnedBarrier = false;
            while (emitter.GetComponent<Emitter>().Active & !spawnedBarrier)
            {
                List<GameObject> emitterBarriers = new List<GameObject>();
                foreach (GameObject barrier in PhaseBarrierList)
                {
                    if (barrier.GetComponent<PhaseBarrier>().ParentEmitter == emitter)
                    {
                        emitterBarriers.Add(barrier);
                    }
                }
                Vector2 newPosition = new Vector2();
                bool goodPosition = true;
                if (emitterBarriers.Count == 0)
                {
                    newPosition = emitter.transform.position + new Vector3(0, GridY, 0);
                }
                else
                {
                    GameObject randomBarrier = emitterBarriers[Random.Range(0, emitterBarriers.Count)];
                    int direction = Random.Range(0, 4);
                    newPosition = randomBarrier.transform.position;
                    switch (direction)
                    {
                        case 0:
                            newPosition += new Vector2(GridX, 0);
                            break;
                        case 1:
                            newPosition -= new Vector2(GridX, 0);
                            break;
                        case 2:
                            newPosition += new Vector2(0, GridY);
                            break;
                        case 3:
                            newPosition -= new Vector2(0, GridY);
                            break;
                    }
                }
                foreach (GameObject barrier in PhaseBarrierList)
                {
                    if ((Vector2)barrier.transform.position == newPosition)
                    {
                        goodPosition = false;
                        break;
                    }
                }
                if (goodPosition)
                {
                    GameObject newBarrier = Instantiate(PhaseBarrierPrefab, newPosition, Quaternion.identity);
                    PhaseBarrierList.Add(newBarrier);
                    newBarrier.GetComponent<PhaseBarrier>().ParentEmitter = emitter;
                    spawnedBarrier = true;
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

    IEnumerator RemovePhaseBarrier()
    {
        List<GameObject> tempEmitterList = new List<GameObject>(InactiveEmitterList);
        foreach (GameObject emitter in tempEmitterList)
        {
            List<GameObject> emitterBarriers = new List<GameObject>();
            bool removedBarriers = false;
            while (!removedBarriers)
            {
                foreach (GameObject barrier in PhaseBarrierList)
                {
                    if (barrier.GetComponent<PhaseBarrier>().ParentEmitter == emitter)
                    {
                        emitterBarriers.Add(barrier);
                    }
                }
                if (emitterBarriers.Count > 0)
                {
                    GameObject randomBarrier = emitterBarriers[Random.Range(0, emitterBarriers.Count)];
                    if (randomBarrier != null & randomBarrier.GetComponent<PhaseBarrier>().State == PhaseBarrierState.ACTIVE)
                    {
                        randomBarrier.GetComponent<PhaseBarrier>().Die();
                        removedBarriers = true;
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    removedBarriers = true;
                }
            }
        }
    }

    public void RegisterEmitter(GameObject emitter)
    {
        if (!ActiveEmitterList.Contains(emitter))
        {
            ActiveEmitterList.Add(emitter);
        }
    }

    public void DeactivateEmitter(GameObject emitter)
    {
        if (ActiveEmitterList.Contains(emitter))
            ActiveEmitterList.Remove(emitter);
        if (!InactiveEmitterList.Contains(emitter))
            InactiveEmitterList.Add(emitter);
        Debug.Log(string.Format("{0} active emitters left", ActiveEmitterList.Count));
        ScoreManager.Instance.Score += DeactivatEmitterScore;
        if (ActiveEmitterList.Count == 0)
        {
            ScoreManager.Instance.Score += LevelClearScore;
            Win();
        }
    }

    public void BarrierHit()
    {
        if (Time.time > nextBarrierDamageTime)
        {
            TakeDamage(BarrierDamageAmount);
            nextBarrierDamageTime = Time.time + BarrierDamageTime;
        }
    }

    public void RobotHit()
    {
        TakeDamage(RobotDamageAmount);
    }

    public void TakeDamage(int damageAmount)
    {
        ScoreManager.Instance.HitPoints -= damageAmount;
        if (ScoreManager.Instance.HitPoints <= 0)
        {
            ScoreManager.Instance.HitPoints = 0;
            Lose();
        }
    }
    public bool CanShoot()
    {
        if (CurrentCannonPower >= CannonShotEnergy)
        {
            CurrentCannonPower -= CannonShotEnergy;
            CannonCooldown = true;
            nextCannonCooldownTime = Time.time + CannonCooldownTime;
            return true;
        } else
        {
            return false;
        }
    }

    public void SwitchState(GameState newState)
    {
        switch (newState)
        {
            case (GameState.STARTING):
                Time.timeScale = 0f;
                break;
            case (GameState.RUNNING):
                Time.timeScale = 1f;
                break;
            case (GameState.PAUSED):
                Time.timeScale = 0f;
                break;
            case (GameState.WINNING):
                Time.timeScale = 0f;
                break;
            case (GameState.LOSING):
                Time.timeScale = 0f;
                break;
        }
        GameState = newState;
        Debug.Log("Game state is " + GameState);
    }

    public void Win()
    {
        SwitchState(GameState.WINNING);
        ScoreManager.Instance.Level++;
        CrossFadeAlphaWithCallBack(CoverImage, 1f, WinTime, delegate
        {
            if (ScoreManager.Instance.Level > Levels.Length)
            {
                SceneManager.LoadScene("win");
            }
            else
            {
                SceneManager.LoadScene("main");
            }
        });
    }

    public void Lose()
    {
        SwitchState(GameState.LOSING);
        ScoreManager.Instance.Lives--;
        CrossFadeAlphaWithCallBack(CoverImage, 1f, LoseTime, delegate
        {
            if (ScoreManager.Instance.Lives >= 0)
            {
                // reload current level
                ScoreManager.Instance.HitPoints = ScoreManager.Instance.MaxHitPoints;
                SceneManager.LoadScene("main");
            }
            else
            {
                SceneManager.LoadScene("lose");
            }
        });
    }
    void CrossFadeAlphaWithCallBack(Image img, float alpha, float duration, System.Action action)
    {
        StartCoroutine(CrossFadeAlphaCOR(img, alpha, duration, action));
    }

    IEnumerator CrossFadeAlphaCOR(Image img, float alpha, float duration, System.Action action)
    {
        Color currentColor = img.color;

        Color visibleColor = img.color;
        visibleColor.a = alpha;


        float counter = 0;

        while (counter < duration)
        {
            counter += Time.unscaledDeltaTime;
            img.color = Color.Lerp(currentColor, visibleColor, counter / duration);
            yield return null;
        }

        //Done. Execute callback
        action.Invoke();
    }
}
