using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFork : MonoBehaviour
{
    [SerializeField] private float uprightAngle = -28.4f;
    [SerializeField] private float attackAngle = 95.6f;
    [SerializeField] private float swishDuration = 0.4f;
    [SerializeField] private float attackCooldown = 2.5f;

    [SerializeField] private GameObject miniTrishulPrefab;
    private Transform firePoint; // Where mini Trishul spawns

    private bool isSwishing = false;

    //Swishes fork and launches mini forks

    void Start()
    {
        StartCoroutine(SwishLoop());
        StartCoroutine(FireMiniTrishuls());
        firePoint = transform.Find("FirePoint");
    }

    IEnumerator SwishLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f)); // Random attack interval
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
        if (startZ > 180f) startZ -= 360f; // Handle angle wrap

        while (elapsed < swishDuration)
        {
            float z = Mathf.Lerp(startZ, targetZ, elapsed / swishDuration);
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
            yield return new WaitForSeconds(Random.Range(1.5f, 2.5f));

            GameObject trishulArrow = Instantiate(miniTrishulPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = trishulArrow.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Determine direction: +0.5 = left, -0.5 = right
                bool isFacingLeft = transform.localScale.x > 0;
                float direction = isFacingLeft ? -1f : 1f; // (-1 = left, +1 = right)

                // Apply force (flip X direction since +0.5 = left)
                Vector2 force = new Vector2(
                    Random.Range(3f, 6f) * direction,
                    Random.Range(6f, 9f)
                );
                rb.velocity = force;

                // Rotate 180° if firing right (since +0.5 = left)
                if (!isFacingLeft)
                {
                    trishulArrow.transform.rotation = Quaternion.Euler(0, 0, 100f);
                }
            }
        }
    }
}
