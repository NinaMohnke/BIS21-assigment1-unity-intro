using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    private Rigidbody playerRb;
    private float speed = 500;
    private GameObject focalPoint;

    public bool hasPowerup;
    public GameObject powerupIndicator;
    public int powerUpDuration = 5;

    private float normalStrength = 10; // how hard to hit enemy without powerup
    private float powerupStrength = 25; // how hard to hit enemy with powerup

    public ParticleSystem boostParticles;

    private float boost_factor = 1;

    public float boost_time = 1;
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");
    }

    void Update()
    {
        // Add force to player in direction of the focal point (and camera)
        float verticalInput = Input.GetAxis("Vertical");


        playerRb.AddForce(focalPoint.transform.forward * verticalInput * Mathf.Round(speed * boost_factor) * Time.deltaTime);

        // Set powerup indicator position to beneath player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.6f, 0);
        if (Input.GetButtonDown("Boost"))
        {

            boost_factor += 0.05f;
            boostParticles.transform.position = transform.position + new Vector3(0, 0, -0.6f);
            boostParticles.Play();
            CancelInvoke("BoostCooldown"); // if we previously picked up an powerup
            Invoke("BoostCooldown", boost_time);
        }

        if (transform.position.y < -10)
        {
            ResetPlayerPosition();
        }

    }

    // If Player collides with powerup, activate powerup
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            hasPowerup = true;
            powerupIndicator.SetActive(true);
            CancelInvoke("PowerupCooldown"); // if we previously picked up an powerup
            Invoke("PowerupCooldown", powerUpDuration);
        }
    }
    void BoostCooldown()
    {
        boost_factor = 1;
    }
    void PowerupCooldown()
    {
        hasPowerup = false;
        powerupIndicator.SetActive(false);
    }

    // If Player collides with enemy
    private void OnCollisionEnter(Collision other)
    {
        Rigidbody enemyRigidbody = other.gameObject.GetComponent<Rigidbody>();
        Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength, ForceMode.Impulse);
            }


        }
        else if (other.gameObject.CompareTag("EvilEnemy"))
        {
            if (hasPowerup) // if have powerup hit enemy with powerup force
            {
                enemyRigidbody.AddForce(awayFromPlayer * powerupStrength * 1.5f, ForceMode.Impulse);
            }
            else // if no powerup, hit enemy with normal strength 
            {
                enemyRigidbody.AddForce(awayFromPlayer * normalStrength * 0.5f, ForceMode.Impulse);
            }
        }
        else if (!(other.gameObject.CompareTag("ground")))
        {
            playerRb.AddForce(awayFromPlayer * 0.5f, ForceMode.Impulse);
        }

    }
    void ResetPlayerPosition()
    {
        transform.position = new Vector3(0, 1, -7);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

    }



}
