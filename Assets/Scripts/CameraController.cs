using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;
    public float padding = 1.0f;
    private Camera mainCamera;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        AdjustCameraSize();
        CalculatePixels();
    }

    void AdjustCameraSize()
    {
        float offset = 0.5f;
        float targetOrthoSize;
        float w = gameManager.mapWidth;
        float h = gameManager.mapHeight;
        float x = w * 0.5f - offset;
        float y = h * 0.5f - offset;
        
        mainCamera.transform.position = new Vector3(x, y, -10f);

        if (w > h * mainCamera.aspect)
        {
            targetOrthoSize = w / mainCamera.pixelWidth * mainCamera.pixelHeight;
        }
        else
        {
            targetOrthoSize = h;
        }

        mainCamera.orthographicSize = targetOrthoSize / 2;

        //Debug.Log("Pixel width: " + mainCamera.pixelWidth + ", and height: " + mainCamera.pixelHeight);
    }

    void CalculatePixels()
    {
        float viewportWidth = mainCamera.rect.width;
        float viewportHeight = mainCamera.rect.height;

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        int pixelsInViewport = Mathf.RoundToInt(viewportWidth * screenWidth) * Mathf.RoundToInt(viewportHeight * screenHeight);

        //Debug.Log("Pixels in camera viewport: " + pixelsInViewport);
    }
}
