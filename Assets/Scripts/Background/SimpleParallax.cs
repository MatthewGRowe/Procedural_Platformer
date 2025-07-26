using UnityEngine;
using System.Collections;

public class SimpleParallax : MonoBehaviour
{
    // PARALLAX SETTINGS
    public float parallaxSpeed = 0.5f; // Speed multiplier for parallax effect
    public Sprite backgroundSprite;    // The sprite to use for backgrounds
    public float yOffset = 0f;        // Vertical offset from camera center

    // PRIVATE VARIABLES
    private float gapDistance;        // Exact horizontal distance between backgrounds
    private float backgroundWidth;    // Calculated width of background sprite

    private Transform cameraTransform; // Reference to main camera
    private Vector3 lastCameraPosition; // Stores camera position from last frame
    private Transform[] backgrounds;   // Array holding the 3 background copies

    private bool readyToGo = false;   // Flag for initialization delay
    private bool firstFrame = true;   // Flag for first frame handling

    private void Start()
    {
        // Disable parent's renderer if it exists (cleaner hierarchy)
        SpriteRenderer parentRenderer = GetComponent<SpriteRenderer>();
        if (parentRenderer != null)
        {
            parentRenderer.enabled = false;
        }

        // Get main camera reference
        cameraTransform = Camera.main.transform;
        if (cameraTransform == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        // CREATE THREE BACKGROUND COPIES
        backgrounds = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            // Create new background object
            GameObject bgCopy = new GameObject("BackgroundCopy_" + i);
            bgCopy.transform.parent = transform; // Parent to this object

            // Add and configure sprite renderer
            SpriteRenderer copyRenderer = bgCopy.AddComponent<SpriteRenderer>();
            copyRenderer.sprite = backgroundSprite;

            // Calculate width based on sprite settings
            backgroundWidth = (backgroundSprite.pixelsPerUnit > 100)
                ? backgroundSprite.rect.width / backgroundSprite.pixelsPerUnit
                : copyRenderer.bounds.size.x;

            // Set scale and initial position
            bgCopy.transform.localScale = Vector3.one;
            float xPos = (i - 1) * backgroundWidth; // Positions: -width, 0, +width
            bgCopy.transform.localPosition = new Vector3(xPos, 0f, 0f);
            backgrounds[i] = bgCopy.transform;
        }

        // Position parent object to align with camera
        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y + yOffset,
            10f // Z-position (further back than gameplay objects)
        );

        // Calculate exact gap between backgrounds
        gapDistance = backgrounds[1].position.x - backgrounds[0].position.x;

        // Store initial camera position
        lastCameraPosition = cameraTransform.position;

        // Start initialization delay
        StartCoroutine(SmallPauseBeforeWeStart());
    }

    // Small delay to ensure everything initializes properly
    IEnumerator SmallPauseBeforeWeStart()
    {
        yield return new WaitForSeconds(0.1f);
        readyToGo = true; // Enable parallax effect after delay
    }

    private void LateUpdate()
    {
        if (!readyToGo) return; // Wait until initialization complete

        // Safety check for references
        if (cameraTransform == null || backgrounds == null || backgrounds.Length != 3)
        {
            Debug.LogError("Camera or backgrounds not initialized properly");
            return;
        }

        // Calculate camera movement since last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // FIRST FRAME HANDLING
        if (firstFrame)
        {
            firstFrame = false;
            RepositionAllBackgrounds(); // Force perfect initial positioning
        }
        else
        {
            // APPLY PARALLAX MOVEMENT
            foreach (Transform bg in backgrounds)
            {
                // Move backgrounds opposite to camera movement (slower due to parallaxSpeed)
                bg.position -= new Vector3(deltaMovement.x * parallaxSpeed, 0, 0);
            }

            // BOUNDARY CHECKING AND SHIFTING
            float cameraX = cameraTransform.position.x;
            float centerX = backgrounds[1].position.x; // Center background's position

            // Shift right if camera crosses right boundary (50% into right background)
            if (cameraX > centerX + gapDistance * 0.5f)
            {
                ShiftBackgroundLeftToRight();
            }
            // Shift left if camera crosses left boundary (50% into left background)
            else if (cameraX < centerX - gapDistance * 0.5f)
            {
                ShiftBackgroundRightToLeft();
            }

            // Ensure perfect spacing between backgrounds
            MaintainBackgroundSpacing();

            // DEBUGGING: Visualize shift boundaries (commented out)
            /*
            // Green line shows left shift boundary
            Debug.DrawLine(new Vector3(backgrounds[1].position.x - gapDistance / 2, -10, 0),
               new Vector3(backgrounds[1].position.x - gapDistance / 2, 10, 0),
               Color.green);
               
            // Red line shows right shift boundary   
            Debug.DrawLine(new Vector3(backgrounds[1].position.x + gapDistance / 2, -10, 0),
                           new Vector3(backgrounds[1].position.x + gapDistance / 2, 10, 0),
                           Color.red);*/
        }

        // Update vertical position to follow camera (horizontal handled by parallax)
        transform.position = new Vector3(
            transform.position.x,
            cameraTransform.position.y + yOffset,
            transform.position.z
        );

        // Remember current camera position for next frame
        lastCameraPosition = cameraTransform.position;
    }

    // Positions all backgrounds perfectly relative to camera
    private void RepositionAllBackgrounds()
    {
        // Center background aligns with camera
        backgrounds[1].position = new Vector3(
            cameraTransform.position.x,
            backgrounds[1].position.y,
            backgrounds[1].position.z
        );

        // Left background exactly one gap distance left of center
        backgrounds[0].position = new Vector3(
            backgrounds[1].position.x - gapDistance,
            backgrounds[1].position.y,
            backgrounds[1].position.z
        );

        // Right background exactly one gap distance right of center
        backgrounds[2].position = new Vector3(
            backgrounds[1].position.x + gapDistance,
            backgrounds[1].position.y,
            backgrounds[1].position.z
        );
    }

    // Ensures perfect spacing between backgrounds every frame
    private void MaintainBackgroundSpacing()
    {
        // Keep left background exactly one gap left of center
        backgrounds[0].position = new Vector3(
            backgrounds[1].position.x - gapDistance,
            backgrounds[1].position.y,
            backgrounds[1].position.z
        );

        // Keep right background exactly one gap right of center
        backgrounds[2].position = new Vector3(
            backgrounds[1].position.x + gapDistance,
            backgrounds[1].position.y,
            backgrounds[1].position.z
        );
    }

    // Shifts backgrounds when moving right
    private void ShiftBackgroundLeftToRight()
    {
        // Move leftmost background to right side
        backgrounds[0].position = new Vector3(
            backgrounds[2].position.x + gapDistance, // Position right of current right bg
            backgrounds[2].position.y,
            backgrounds[2].position.z
        );

        // Reorder array: [0,1,2] becomes [1,2,0]
        Transform temp = backgrounds[0];
        backgrounds[0] = backgrounds[1]; // Old center becomes new left
        backgrounds[1] = backgrounds[2]; // Old right becomes new center
        backgrounds[2] = temp;           // Old left becomes new right
    }

    // Shifts backgrounds when moving left
    private void ShiftBackgroundRightToLeft()
    {
        // Move rightmost background to left side
        backgrounds[2].position = new Vector3(
            backgrounds[0].position.x - gapDistance, // Position left of current left bg
            backgrounds[0].position.y,
            backgrounds[0].position.z
        );

        // Reorder array: [0,1,2] becomes [2,0,1]
        Transform temp = backgrounds[2];
        backgrounds[2] = backgrounds[1]; // Old center becomes new right
        backgrounds[1] = backgrounds[0]; // Old left becomes new center
        backgrounds[0] = temp;           // Old right becomes new left
    }
}