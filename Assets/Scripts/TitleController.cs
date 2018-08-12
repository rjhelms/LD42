using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {

    public GameState GameState;

    [Header("Resolution and Display")]
    public Camera WorldCamera;
    public int TargetX = 160;
    public int TargetY = 200;
    public Material RenderTexture;
    private float pixelRatioAdjustment;

    [Header("UI Elements")]
    public Image CoverImage;
    public string SceneToLoad;
    public GameObject MusicPlayer;
    private bool hasStarted = false;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1f;
        InitializeCamera();
        if (GameObject.FindGameObjectsWithTag("Music").Length == 0)
        {
            GameObject music = Instantiate(MusicPlayer);
            DontDestroyOnLoad(music);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        switch (GameState)
        {
            case GameState.STARTING:
                if (!hasStarted)
                {
                    hasStarted = true;
                    CrossFadeAlphaWithCallBack(CoverImage, 0f, 1f, delegate
                    {
                        GameState = GameState.RUNNING;
                    });
                };
                break;
            case GameState.RUNNING:
                if (Input.anyKeyDown)
                {
                    GameState = GameState.WINNING;
                    CrossFadeAlphaWithCallBack(CoverImage, 1f, 1f, delegate
                    {
                        SceneManager.LoadScene(SceneToLoad);
                    });
                }
                break;
        }
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
