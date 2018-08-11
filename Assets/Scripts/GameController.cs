using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
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
    private Player player;

    [Header("Phase Barrier Attributes")]
    public GameObject PhaseBarrierPrefab;
    public List<GameObject> PhaseBarrierList;
    public List<GameObject> ActiveEmitterList;
    public float PhaseBarrierTime;
    private float nextPhaseBarrierTime;

    // Use this for initialization
    void Start()
    {
        InitializeCamera();
        nextPhaseBarrierTime = Time.time + PhaseBarrierTime;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextPhaseBarrierTime)
        {
            nextPhaseBarrierTime = Time.time + PhaseBarrierTime;
            StartCoroutine("CreatePhaseBarrier");
        }
        Vector3 cameraTargetPosition = (player.transform.position + CameraPositionOffset
                                        + ((Vector3)player.LastMoveVector * PlayerVectorMultiplier));
        Vector3 newCameraPosition = Vector3.Lerp(WorldCamera.transform.position, cameraTargetPosition, CameraLerpSpeed);
        WorldCamera.transform.position = new Vector3(Mathf.RoundToInt(newCameraPosition.x), Mathf.Round(newCameraPosition.y),
                                                     WorldCamera.transform.position.z);

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
        bool spawnedBarrier = false;

        foreach (GameObject emitter in ActiveEmitterList)
        {
            while (!spawnedBarrier)
            {
                List<GameObject> emitterBarriers = new List<GameObject>();
                foreach (GameObject barrier in PhaseBarrierList)
                {
                    Debug.Log(barrier);
                    if (barrier.GetComponent<PhaseBarrier>().ParentEmitter == emitter)
                    {
                        emitterBarriers.Add(barrier);
                    }
                }
                Vector2 newPosition = new Vector2();
                bool goodPosition = true;
                if (emitterBarriers.Count == 0)
                {
                    newPosition = emitter.transform.position;
                }
                else
                {

                    GameObject randomBarrier = emitterBarriers[Random.Range(0, PhaseBarrierList.Count)];
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

    public void RegisterEmitter(GameObject emitter)
    {
        if (!ActiveEmitterList.Contains(emitter))
        {
            ActiveEmitterList.Add(emitter);
        }
    }
}
