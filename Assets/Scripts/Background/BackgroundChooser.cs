using UnityEngine;

public class BackgroundChooser : MonoBehaviour
{
    public GameObject[] parralaxBackgrounds; // Array of parallax background prefabs
    public Sprite[] backgroundImages; // Array of regular background sprites
    public GameObject simpleParallaxPrefab; // Prefab for the SimpleParallax GameObject

    void Awake()
    {
        bool parallaxBG = Random.value > 0.5f;
        int selectedBackground = 0;
        Camera mainCamera = Camera.main; // Cache the main camera

        if (parallaxBG)
        {
            selectedBackground = Random.Range(0, parralaxBackgrounds.Length);
            Vector3 prefabPosition = parralaxBackgrounds[selectedBackground].transform.position;

            GameObject theBG = Instantiate(parralaxBackgrounds[selectedBackground], prefabPosition, Quaternion.identity);

            // Scale the background to ~2x camera size
            ScaleBackgroundToCamera(theBG, mainCamera);

            // Adjust position relative to camera
            theBG.transform.position = new Vector3(
                prefabPosition.x,
                prefabPosition.y + mainCamera.transform.position.y,
                prefabPosition.z
            );

            theBG.transform.SetParent(mainCamera.transform, true);
        }
        else
        {
            selectedBackground = Random.Range(0, backgroundImages.Length);
            GameObject simpleParallax = Instantiate(simpleParallaxPrefab, transform.position, Quaternion.identity);

            // Scale the background to ~2x camera size
            ScaleBackgroundToCamera(simpleParallax, mainCamera);

            simpleParallax.transform.SetParent(mainCamera.transform, true);

            SimpleParallax parallaxScript = simpleParallax.GetComponent<SimpleParallax>();
            if (parallaxScript != null)
            {
                parallaxScript.backgroundSprite = backgroundImages[selectedBackground];
            }
            else
            {
                Debug.LogError("SimpleParallax script not found on the prefab.");
            }
        }
    }

    // Helper method to scale a background object to ~2x camera size
    private void ScaleBackgroundToCamera(GameObject backgroundObj, Camera camera)
    {
        SpriteRenderer spriteRenderer = backgroundObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        // Get the camera's viewable height & width in world units
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;

        // Get the sprite's native size
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float spriteHeight = spriteRenderer.sprite.bounds.size.y;

        // Calculate the scale needed to make the sprite ~2x camera dimensions
        float scaleX = (cameraWidth * 2f) / spriteWidth; // Target: 2x camera width
        float scaleY = (cameraHeight * 2f) / spriteHeight; // Target: 2x camera height

        // Apply non-uniform scaling (will stretch if aspect ratios don't match)
        backgroundObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}