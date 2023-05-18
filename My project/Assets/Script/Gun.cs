using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject gun;
    public TrailRenderer trailRenderer;
    public GameObject particleSystemContainer;
    bool isShooting=false;
    private void Awake()
    {
        PlayerShoot.shoot += StartShooting;
        PlayerShoot.stopShoot += StopShooting;
        StartCoroutine(ShootRepeatedly(0.1f));
       
    }
    private void StartShooting()
    {
        if (!isShooting)
        {
            Shoot();
            isShooting = true;
            StartCoroutine(ShootRepeatedly(0.1f));
        }
    }

    private void StopShooting()
    {
        if (isShooting)
        {
            isShooting = false;
            StopCoroutine(ShootRepeatedly(0.1f));
        }
    }
    private IEnumerator ShootRepeatedly(float delay)
    {
        while (isShooting)
        {
            
            yield return new WaitForSeconds(delay);
        }
    }
    private void Shoot()
    {
            if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hitInfo, Mathf.Infinity)) {
                    var tracer = Instantiate(trailRenderer, gun.transform.position, Quaternion.identity);
                    tracer.AddPosition(hitInfo.point);
                    IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
                    Debug.Log(hitInfo.collider.name);
                    foreach (ParticleSystem particleSystem in particleSystemContainer.GetComponentsInChildren<ParticleSystem>())
                    {
                        ParticleSystem newParticle = Instantiate(particleSystem, hitInfo.point, Quaternion.identity);
                        newParticle.Play();
                        Destroy(newParticle.gameObject, newParticle.main.duration);
                    }
                    damageable?.TakeDamage(Random.Range(1,6));
            }

    }

    private void Update()
    {
       
    }

}
