using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public bool InstantiateLevel = true;
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
    public int HealthBarWidth = 24;
    public int PowerBarWidth = 24;
    public Text ScoreText;
    public Text LevelText;

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
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            ScoreManager.Instance.Level++;
            SceneManager.LoadScene("main");
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
        } else
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
        Vector3 cameraTargetPosition = (player.transform.position + CameraPositionOffset
                                        + ((Vector3)player.LastMoveVector * PlayerVectorMultiplier));
        Vector3 newCameraPosition = Vector3.Lerp(WorldCamera.transform.position, cameraTargetPosition, CameraLerpSpeed);
        WorldCamera.transform.position = new Vector3(Mathf.RoundToInt(newCameraPosition.x), Mathf.Round(newCameraPosition.y),
                                                     Mathf.Round(newCameraPosition.y - 150));
        gameGrid.position = new Vector3(gameGrid.position.x, gameGrid.position.y, player.transform.position.z);

        int currentHealthBarWidth = Mathf.RoundToInt(ScoreManager.Instance.HitPoints * HealthBarWidth / ScoreManager.Instance.MaxHitPoints);
        HealthBar.localScale = new Vector3(currentHealthBarWidth, 1, 1);
        int currentPowerBarWidth = Mathf.RoundToInt(CurrentCannonPower * PowerBarWidth / ScoreManager.Instance.MaxCannonPower);
        PowerBar.localScale = new Vector3(currentPowerBarWidth, 1, 1);
        ScoreText.text = string.Format("{0}", ScoreManager.Instance.Score);
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
            Debug.Log("Level clear!");
        }
    }

    public void CheckHit()
    {
        if (Time.time > nextBarrierDamageTime)
        {
            ScoreManager.Instance.HitPoints -= BarrierDamageAmount;
            nextBarrierDamageTime = Time.time + BarrierDamageTime;
            Debug.Log("Ouch! " + ScoreManager.Instance.HitPoints);
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
}
