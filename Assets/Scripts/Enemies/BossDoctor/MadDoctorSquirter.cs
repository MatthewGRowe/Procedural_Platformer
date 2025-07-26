using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadDoctorSquirter : MonoBehaviour
{
    public ParticleSystem chemicalSpray;  //Colourful squirt
    public GameObject hitbox; // Damage is calculated with a box collider, easier
                              // to keep this as a 

    public float minInterval = 0.8f;
    public float maxInterval = 2f;
    public float squirtDuration = 1f;
    public float angleOffset = 240;

    //private bool isSquirting;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(SquirtRoutine());
    }



    IEnumerator SquirtRoutine()
    {


        while (true)
        {
            if (GameManager.instance.bossAttack == true)
            {
                // Find doctor and tell it to try lunging
                MadDoctor doctor = GetComponentInParent<MadDoctor>();
                if (doctor != null)
                {
                    doctor.TryLungeForward();
                }


                //Calculate where to aim
                float waitTime = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(waitTime);

                // Find Player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                Transform playerPos = player.transform;

                // Calculate the direction vector from the syringe to the player
                Vector3 direction = playerPos.position - transform.position;

                // Convert the direction vector into an angle in degrees (radians → degrees)
                // Atan2 gives you the angle between the X-axis and the vector pointing to the player
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Set the rotation of the object to point toward the calculated angle
                // Quaternion.Euler converts the angle into a usable rotation (only rotating around Z for 2D)
                transform.rotation = Quaternion.Euler(0f, 0f, angle);

                // Optional: Add a correction if the sprite is facing the wrong default direction
                // This rotates the object further so it actually *points* at the target instead of away
                //angleOffset = 240f; // Adjust this value as needed to match your sprite’s facing

                // Apply the corrected rotation to the object (angle + offset)
                transform.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset);


           
                // Begin squirting
                chemicalSpray.Play();
                hitbox.SetActive(true);
               
                AudioManager.instance.Play("Squirt");

                yield return new WaitForSeconds(squirtDuration);

                // Stop squirting
                chemicalSpray.Stop();
                hitbox.SetActive(false);
                
            }
            else
            {
                yield return new WaitForSeconds(squirtDuration);
            }
        }
    }
   
}
