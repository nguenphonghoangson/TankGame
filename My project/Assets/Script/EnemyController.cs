using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour,IDamageable
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject player;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float minDistanceToTurret = 20f;
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private GameObject gun;
    [SerializeField] private bool isRotating = false;
    [SerializeField] private Quaternion targetRotation;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject particleSystemContainer;
    [SerializeField] private bool canAttack = true;

    // Start is called before the first frame update
    void Awake(){
                health = maxHealth;
                player = GameObject.FindGameObjectWithTag("Player");
                objectPool = FindObjectOfType<ObjectPool>();
                agent = GetComponent<NavMeshAgent>();
                targetPosition = GetRandomPositionAroundTurret();
                Debug.Log(targetPosition);
                MoveToTargetPosition();
                healthBar.UpdateHeathBar(maxHealth, health);
        }
    private void OnEnable()
    {
        isRotating = false;
        canAttack = true;
        health = maxHealth;
        healthBar.UpdateHeathBar(maxHealth, health);
        targetPosition = GetRandomPositionAroundTurret();
        MoveToTargetPosition();
    }
    // Update is called once per frame
    void Update(){

            
            if (!agent.pathPending && agent.remainingDistance <= stoppingDistance)
            {
                
                RotateTowardsTurret();
                if (isRotating)
                    {
                        
                        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                        transform.rotation = newRotation;


                        float angle = Quaternion.Angle(transform.rotation, targetRotation);


                        if (angle < 4f)
                        {

                            if (canAttack)
                            {
                                Debug.Log('s');

                                canAttack = false;
                                Shoot();
                                StartCoroutine(WaitAndMoveToNewPosition(3f));
                    }
                }
              
            }
            }
     }

    private IEnumerator WaitAndMoveToNewPosition(float delay)
    {
        yield return new WaitForSeconds(delay); // Chờ trong khoảng thời gian delay

        targetPosition = GetRandomPositionAroundTurret();
        MoveToTargetPosition();
        canAttack = true;
    }
    private void MoveToTargetPosition()
    {
        agent.SetDestination(targetPosition);
    }
    private bool IsEnemyAtPosition(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);

        foreach (Collider collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                return true; // Có enemy ở vị trí này
            }
        }

        return false; // Không có enemy ở vị trí này
    }
    private Vector3 GetRandomPositionAroundTurret()
    {
        
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minDistanceToTurret, 60f);

        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 newPosition = player.transform.position + rotation * (Vector3.forward * randomDistance);
        Debug.Log(IsEnemyAtPosition(newPosition, 5f));
        while (IsEnemyAtPosition(newPosition, 5f))
        {
            randomAngle = Random.Range(0f, 360f);
            randomDistance = Random.Range(minDistanceToTurret, 60f);

            rotation = Quaternion.Euler(0f, randomAngle, 0f);
            newPosition = player.transform.position + rotation * (Vector3.forward * randomDistance);
        }
        return newPosition;

            }
        private void RotateTowardsTurret()
        {
        Vector3 direction = player.transform.position - transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        direction.y = 0;
        targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        isRotating = true;
        
    }
    private void Shoot()
    {
        if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hitInfo, Mathf.Infinity))
        {
            Debug.Log('a');
            var tracer = Instantiate(trailRenderer, gun.transform.position, Quaternion.identity);
            tracer.AddPosition(hitInfo.point);
            PlayerController damageable = hitInfo.collider.GetComponent<PlayerController>();
            Debug.Log(hitInfo.collider.name);
            foreach (ParticleSystem particleSystem in particleSystemContainer.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem newParticle = Instantiate(particleSystem, hitInfo.point, Quaternion.identity);
                newParticle.Play();
                Destroy(newParticle.gameObject, newParticle.main.duration);
            }
            damageable?.TakeDamage(Random.Range(1, 6));
        }


    }
    public void TakeDamage(float damage){
        health -= damage;
        healthBar.UpdateHeathBar(maxHealth, health);
        if (health <= 0){
            objectPool.ReturnGameObject(gameObject);
            var playerscore=player.GetComponent<PlayerController>();
            playerscore.Score++;
        };
    }
}
