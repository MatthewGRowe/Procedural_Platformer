using UnityEngine;
using UnityEngine.Tilemaps;
// using UnityEngine.U2D; // <- uncomment if you use SpriteShapeRenderer

public class BackgroundChooser : MonoBehaviour
{
    public GameObject[] parralaxBackgrounds;
    public Sprite[] backgroundImages;
    public GameObject simpleParallaxPrefab;

    [Header("Rendering")]
    public string backgroundSortingLayer = "Background";
    public int backgroundOrderInLayer = 0;   // usually 0 is fine on the Background layer

    void Awake()
    {
        bool parallaxBG = Random.value > 0.5f;
        Camera mainCamera = Camera.main;

        if (parallaxBG)
        {
            int i = Random.Range(0, parralaxBackgrounds.Length);
            Vector3 p = parralaxBackgrounds[i].transform.position;

            GameObject theBG = Instantiate(parralaxBackgrounds[i], p, Quaternion.identity);

            ScaleBackgroundToCamera(theBG, mainCamera);
            theBG.transform.position = new Vector3(p.x, p.y + mainCamera.transform.position.y, p.z);
            theBG.transform.SetParent(mainCamera.transform, true);

            ApplyBackgroundSortingDeep(theBG, backgroundSortingLayer, backgroundOrderInLayer);
            StartCoroutine(ApplyNextFrame(theBG)); // catch late-spawned children
        }
        else
        {
            int i = Random.Range(0, backgroundImages.Length);
            GameObject simpleParallax = Instantiate(simpleParallaxPrefab, transform.position, Quaternion.identity);

            ScaleBackgroundToCamera(simpleParallax, mainCamera);
            simpleParallax.transform.SetParent(mainCamera.transform, true);

            var parallaxScript = simpleParallax.GetComponent<SimpleParallax>();
            if (parallaxScript != null) parallaxScript.backgroundSprite = backgroundImages[i];
            else Debug.LogError("SimpleParallax script not found on the prefab.");

            ApplyBackgroundSortingDeep(simpleParallax, backgroundSortingLayer, backgroundOrderInLayer);
            StartCoroutine(ApplyNextFrame(simpleParallax));
        }
    }

    private void ScaleBackgroundToCamera(GameObject backgroundObj, Camera cam)
    {
        var sr = backgroundObj.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float camH = 2f * cam.orthographicSize;
        float camW = camH * cam.aspect;

        float spriteW = sr.sprite.bounds.size.x;
        float spriteH = sr.sprite.bounds.size.y;

        float scaleX = (camW * 2f) / spriteW;
        float scaleY = (camH * 2f) / spriteH;

        backgroundObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }

    // Apply on next frame in case the prefab builds children in Start()
    private System.Collections.IEnumerator ApplyNextFrame(GameObject root)
    {
        yield return null;
        ApplyBackgroundSortingDeep(root, backgroundSortingLayer, backgroundOrderInLayer);
    }

    // Force ALL child renderers to the Background sorting layer (and chosen order)
    private void ApplyBackgroundSortingDeep(GameObject root, string sortingLayer, int order)
    {
        var srs = root.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in srs) { r.sortingLayerName = sortingLayer; r.sortingOrder = order; }

        // If you use these, uncomment:
        // var shapes = root.GetComponentsInChildren<SpriteShapeRenderer>(true);
        // foreach (var r in shapes) { r.sortingLayerName = sortingLayer; r.sortingOrder = order; }

        var tmrs = root.GetComponentsInChildren<TilemapRenderer>(true);
        foreach (var r in tmrs) { r.sortingLayerName = sortingLayer; r.sortingOrder = order; }
    }
}
