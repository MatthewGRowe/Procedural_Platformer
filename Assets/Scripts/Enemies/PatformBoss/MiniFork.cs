using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This script controls the behaviour of a "Mini Fork" projectile (the mini Trishul).
/// When it hits the player, it causes damage.
/// When it hits the ground, it explodes and destroys tiles in a small radius.
/// </summary>
public class MiniFork : MonoBehaviour
{
    // ------------------------
    // VARIABLES FOR EXPLOSION
    // ------------------------

    [Header("Explosion Settings")]

    // Optional particle effect prefab that plays when the fork explodes
    public GameObject explosionFx;

    // The size of the explosion in Unity world units (1 = about one tile)
    public float explosionRadiusWorld = 1.5f;

    // The amount of force used to push nearby objects when the explosion happens
    public float rbExplosionForce = 6f;

    // A small upward boost added to the explosion force so objects fly slightly upward
    public float rbExplosionUpBoost = 0.5f;

    // A LayerMask so we only apply explosion forces to specific objects (e.g., enemies, player)
    public LayerMask affectRbMask;

    private ParticleSystem ps; 


    // ------------------------
    // TILEMAP REFERENCES
    // ------------------------

    [Header("Tilemap Settings")]

    // The tilemap that represents the ground. Assign this in the Unity Inspector.
    public Tilemap groundTilemap;

    // Optional: layer mask for detecting ground (not essential here)
    public LayerMask groundMask;

    // How far from the impact (in tiles) we’ll check when removing tiles.
    public int maxCellsRadius = 3;

    [Header("Rotation")]
    [Tooltip("How fast the fork is allowed to turn (deg/sec). 180–540 feels good.")]
    public float rotateDegreesPerSecond = 360f;

    [Tooltip("Don’t rotate if moving slower than this (prevents jitter).")]
    public float minSpeedToRotate = 1.0f;

    [Tooltip("Delay before we begin auto-aim rotation (seconds).")]
    public float rotationDelay = 0.05f;

    [Tooltip("If your sprite points UP by default, set -90. If it points RIGHT, leave 0.")]
    public float spriteForwardAngleOffset = 0f;

    [Tooltip("How quickly we trust the new velocity direction (0–1 per second).")]
    public float velocitySmoothing = 10f;

    private Rigidbody2D rb;
    private Vector2 smoothedVelocity;
    private float spawnTime;

    // ------------------------
    // INTERNAL VARIABLES
    // ------------------------

    // Used to prevent damaging the player repeatedly while the fork overlaps
    private bool stopSpammingDamage = false;


    // ------------------------
    // UNITY METHODS
    // ------------------------

    void Awake()
    {
        //Rotation logic
        rb = GetComponent<Rigidbody2D>();
        // Prevent physics torque from spinning it:
        rb.freezeRotation = true;

        //Prevent tilemap reference being lost
        if (groundTilemap == null)
        {
            // Prefer: tag your ground GO (the one with Tilemap/Collider) and find by tag
            var tagged = GameObject.FindWithTag(MyTags.GROUND_LAYER_TAG);
            if (tagged) groundTilemap = tagged.GetComponent<Tilemap>();
            if (groundTilemap == null) groundTilemap = FindObjectOfType<Tilemap>();
        }

    }

    void Start()
    {
        // Automatically destroy the fork after 5 seconds
        // so unused projectiles don't stay in the game forever.
        Destroy(gameObject, 6f);

        //Find the particle system so we can enable it at the right time
        ps = GetComponentInChildren<ParticleSystem>(true);

        spawnTime = Time.time;
        smoothedVelocity = rb.velocity;

        // Set an initial reasonable facing so there’s no big first-frame snap.
        if (smoothedVelocity.sqrMagnitude > 0.001f)
        {
            float a = Mathf.Atan2(smoothedVelocity.y, smoothedVelocity.x) * Mathf.Rad2Deg + spriteForwardAngleOffset;
            transform.rotation = Quaternion.AngleAxis(a, Vector3.forward);
        }
    }

    void Update()
    {
        // Don’t rotate immediately on spawn
        if (Time.time - spawnTime < rotationDelay) return;

        // Exponential smoothing toward current velocity to reduce jitter
        smoothedVelocity = Vector2.Lerp(
            smoothedVelocity,
            rb.velocity,
            1f - Mathf.Exp(-velocitySmoothing * Time.deltaTime)
        );

        float speed = smoothedVelocity.magnitude;
        if (speed < minSpeedToRotate) return;

        // Target angle from (smoothed) velocity
        float targetAngle = Mathf.Atan2(smoothedVelocity.y, smoothedVelocity.x) * Mathf.Rad2Deg + spriteForwardAngleOffset;

        // Smoothly rotate toward target angle with a max degrees/sec cap
        Quaternion targetRot = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRot,
            rotateDegreesPerSecond * Time.deltaTime
        );
    }


    /// <summary>
    /// Called automatically when this object’s collider touches another collider.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the fork hits the player and we haven’t already applied damage recently:
        if (collision.CompareTag(MyTags.PLAYER_TAG) && !stopSpammingDamage)
        {
            stopSpammingDamage = true; // temporarily stop repeated hits

            Debug.Log("Player hit by mini Trishul!");

            // Play a sound (requires an AudioManager in your game)
            AudioManager.instance.Play("Sizzle");

            // Reduce player health slightly
            GameManager.instance.DropHealth(0.1f);

            // Allow damage again after 0.8 seconds
            StartCoroutine(AllowDamage());
        }

        // If the fork hits the ground:
        else if (collision.CompareTag(MyTags.GROUND_LAYER_TAG))
        {
            // Trigger explosion effects and remove nearby tiles
            DoExplosion(transform.position);

            // Destroy the fork object itself so it doesn’t keep moving
            Destroy(gameObject);
        }
    }


    // ------------------------
    // EXPLOSION LOGIC
    // ------------------------

    /// <summary>
    /// Handles everything that should happen when the fork hits the ground.
    /// </summary>
    void DoExplosion(Vector3 worldPos)
    {
        //Called when the fork hits the ground
        //Enable the particle system (ps)
        ps.gameObject.SetActive(true);
        AudioManager.instance.Play("Explosion");

        // 1) Spawn an explosion visual effect (if one is assigned)
        if (explosionFx)
            Instantiate(explosionFx, worldPos, Quaternion.identity);

        // 2) Remove (carve out) tiles around the impact point
        if (groundTilemap != null)
            CarveTiles(worldPos);

        // 3) Push nearby rigidbodies outward for a dramatic effect
        PushNearbyRigidbodies(worldPos);
    }


    /// <summary>
    /// Removes tiles in a circular area around the explosion.
    /// </summary>
    void CarveTiles(Vector3 worldPos)
    {
        // Convert the world position (e.g. 3.2, 1.4) into a tile coordinate (integer x, y)
        Vector3Int centerCell = groundTilemap.WorldToCell(worldPos);

        // Calculate how many tiles to check based on the world radius
        int cellRadius = Mathf.CeilToInt(explosionRadiusWorld / groundTilemap.cellSize.x);
        cellRadius = Mathf.Clamp(cellRadius, 1, maxCellsRadius);

        // Loop through a square region of tiles and remove those inside the explosion circle
        for (int y = -cellRadius; y <= cellRadius; y++)
        {
            for (int x = -cellRadius; x <= cellRadius; x++)
            {
                // Current tile being checked
                Vector3Int cell = centerCell + new Vector3Int(x, y, 0);

                // Find how far this tile is from the explosion’s centre
                Vector3 cellWorld = groundTilemap.GetCellCenterWorld(cell);
                float dist = Vector2.Distance(worldPos, cellWorld);

                // Only remove tiles that fall within the circular radius
                if (dist > explosionRadiusWorld)
                    continue;

                // If a tile exists here, delete it (set to null)
                if (groundTilemap.HasTile(cell))
                    groundTilemap.SetTile(cell, null);
            }
        }

        // Update the collider so the player/enemies can immediately fall through the hole
        var col = groundTilemap.GetComponent<TilemapCollider2D>();
        if (col != null)
            col.ProcessTilemapChanges();
    }


    /// <summary>
    /// Pushes nearby rigidbodies outward to simulate a small explosion shockwave.
    /// </summary>
    void PushNearbyRigidbodies(Vector3 worldPos)
    {
        // Don’t bother if force is zero
        if (rbExplosionForce <= 0f) return;

        // Slightly larger radius than the tile destruction area
        float radius = explosionRadiusWorld * 1.25f;

        // Find all colliders within the explosion radius on the chosen layers
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius, affectRbMask);

        // Loop through everything we found
        foreach (var hit in hits)
        {
            // Only affect objects that have a Rigidbody2D
            if (hit.attachedRigidbody != null)
            {
                // Work out direction from explosion centre to the object
                Vector2 dir = (hit.attachedRigidbody.worldCenterOfMass - (Vector2)worldPos).normalized;

                // Apply a force in that direction plus a small upward boost
                Vector2 force = dir * rbExplosionForce + Vector2.up * rbExplosionUpBoost;
                hit.attachedRigidbody.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }


    // ------------------------
    // DAMAGE CONTROL
    // ------------------------

    /// <summary>
    /// After a short delay, allow the fork to damage the player again.
    /// </summary>
    IEnumerator AllowDamage()
    {
        yield return new WaitForSeconds(0.8f);
        stopSpammingDamage = false;
    }


    // ------------------------
    // EDITOR VISUAL AID
    // Note: No need to uncomment this, it works during testing and not in the final compilation!
    // ------------------------

#if UNITY_EDITOR
    /// <summary>
    /// Draws a circle in the Scene view to show how big the explosion will be.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadiusWorld);
    }
#endif
}
