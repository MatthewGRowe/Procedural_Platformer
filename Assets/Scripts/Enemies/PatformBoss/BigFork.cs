using System.Collections;
using UnityEngine;

public class BigFork : MonoBehaviour
{
    [Header("Swish")]
    [SerializeField] private float uprightAngle = -28.4f;
    [SerializeField] private float attackAngle = 95.6f;
    [SerializeField] private float swishDuration = 0.4f;

    [Header("Attack cadence")]
    [SerializeField] private float attackCooldownMin = 1.5f;
    [SerializeField] private float attackCooldownMax = 2.5f;

    [Header("Projectile")]
    [SerializeField] private GameObject miniTrishulPrefab;
    [SerializeField] private float horizSpeedMin = 3f;
    [SerializeField] private float horizSpeedMax = 6f;
    [SerializeField] private float vertSpeedMin = 6f;
    [SerializeField] private float vertSpeedMax = 9f;

    [Header("Facing")]
    [SerializeField] private bool spriteFacesRight = false; // set true if your art faces +X by default

    private Transform firePoint;              // child named "FirePoint"
    private bool isSwishing = false;

    private ProximitySensorPlatform sensor;   // sibling script
    private Coroutine swishRoutine;           // running swish coroutine
    private Coroutine fireRoutine;            // running fire coroutine
    private bool isAttacking = false;         // current attack state

    void Start()
    {
        // Find firePoint
        firePoint = transform.Find("FirePoint");
        if (firePoint == null)
        {
            Debug.LogError("[BigFork] FirePoint child not found.");
            enabled = false;
            return;
        }

        // Find sibling "ProximitySensor"
        Transform parentObject = transform.parent;
        if (parentObject != null)
        {
            Transform sensorTransform = parentObject.Find("ProximitySensor");
            if (sensorTransform != null)
            {
                sensor = sensorTransform.GetComponent<ProximitySensorPlatform>();
            }
        }
        if (sensor == null)
        {
            Debug.LogWarning("[BigFork] ProximitySensorPlatform not found. Enemy will never attack.");
        }
    }

    void OnEnable()
    {
        // make sure flags/handles are clean if re-enabled
        isAttacking = false;
        swishRoutine = null;
        fireRoutine = null;
    }

    void Update()
    {
        bool wantToAttack = (sensor != null) && (sensor.canWeAttack == true);

        if (wantToAttack && !isAttacking)
        {
            swishRoutine = StartCoroutine(SwishLoop());
            fireRoutine = StartCoroutine(FireMiniTrishuls());
            isAttacking = true;
        }
        else if (!wantToAttack && isAttacking)
        {
            if (swishRoutine != null) { StopCoroutine(swishRoutine); swishRoutine = null; }
            if (fireRoutine != null) { StopCoroutine(fireRoutine); fireRoutine = null; }
            isAttacking = false;
        }
    }

    void OnDisable()
    {
        if (swishRoutine != null) { StopCoroutine(swishRoutine); swishRoutine = null; }
        if (fireRoutine != null) { StopCoroutine(fireRoutine); fireRoutine = null; }
        isAttacking = false;
    }

    IEnumerator SwishLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            yield return StartCoroutine(SwishToAngle(attackAngle));
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(SwishToAngle(uprightAngle));
        }
    }

    IEnumerator SwishToAngle(float targetZ)
    {
        if (isSwishing) yield break;

        isSwishing = true;
        float elapsed = 0f;

        float startZ = transform.localEulerAngles.z;
        if (startZ > 180f) startZ -= 360f; // unwrap

        while (elapsed < swishDuration)
        {
            float t = elapsed / swishDuration;
            float z = Mathf.Lerp(startZ, targetZ, t);
            transform.localEulerAngles = new Vector3(0, 0, z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0, 0, targetZ);
        isSwishing = false;
    }

    IEnumerator FireMiniTrishuls()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(attackCooldownMin, attackCooldownMax));

            // Facing from scale.x sign, corrected by art orientation
            float sign = Mathf.Sign(transform.lossyScale.x);
            if (!spriteFacesRight) sign *= -1f; // invert if your sprite faces left by default
            if (sign == 0f) sign = 1f;

            GameObject proj = Instantiate(miniTrishulPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float vx = Random.Range(horizSpeedMin, horizSpeedMax) * sign;
                float vy = Random.Range(vertSpeedMin, vertSpeedMax);
                rb.velocity = new Vector2(vx, vy);

                if (rb.velocity.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
                    proj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }
            }
            else
            {
                Debug.LogWarning("[BigFork] Spawned miniTrishul has no Rigidbody2D.");
            }
        }
    }
}
