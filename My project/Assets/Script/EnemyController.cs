using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    [SerializeField] private GameObject rotate;
    [SerializeField] private bool isRotating = false;
    [SerializeField] private Quaternion targetRotation;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject particleSystemContainer;
    [SerializeField] private bool canAttack = true;
    Quaternion init=Quaternion.identity;
    // Start is called before the first frame update
    void Awake(){
                player = GameObject.FindGameObjectWithTag("Player");
                objectPool = FindObjectOfType<ObjectPool>();
                agent = GetComponent<NavMeshAgent>();

    }
    private void OnEnable()
    {
        isRotating = true;
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
            Debug.Log(Quaternion.Equals(init, Quaternion.identity));
            if (Quaternion.Equals(init, Quaternion.identity))
            {
                init = rotate.transform.rotation;
            }
            Debug.Log(rotate.transform.rotation);
            Debug.Log(init);
            targetRotation = RotateTowardsTurret(rotate.transform.position, player.transform.position);
            if (isRotating){
                Quaternion newrotation = Quaternion.Lerp(rotate.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                rotate.transform.rotation = newrotation;
                float angle = Quaternion.Angle(rotate.transform.rotation, targetRotation);
                if (angle < 4f){
                    if (canAttack)
                            {
                                canAttack = false;
                                Shoot(); 
                                isRotating = false;
                    }
                }
            }
            else
            {
                if (Quaternion.Angle(rotate.transform.rotation, transform.rotation) > 1f)
                {
                    Debug.Log('-');
                    Debug.Log(rotate.transform.rotation);
                    Quaternion newRotation = Quaternion.Lerp(rotate.transform.rotation, init, rotationSpeed * Time.deltaTime);
                    Debug.Log(newRotation);
                    rotate.transform.rotation = newRotation;
                }
                else
                {
                    init = Quaternion.identity;
                    StartCoroutine(WaitAndMoveToNewPosition(2));
                }
            }




        }
    }
    private IEnumerator WaitAndMoveToNewPosition(float delay)
    {   
        yield return new WaitForSeconds(delay); // Chờ trong khoảng thời gian delay

        Debug.Log(rotate.transform.rotation);
        targetPosition = GetRandomPositionAroundTurret();
        MoveToTargetPosition();
        isRotating = true;
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
        //Debug.Log(IsEnemyAtPosition(newPosition, 5f));
        while (IsEnemyAtPosition(newPosition, 5f))
        {
            randomAngle = Random.Range(0f, 360f);
            randomDistance = Random.Range(minDistanceToTurret, 60f);

            rotation = Quaternion.Euler(0f, randomAngle, 0f);
            newPosition = player.transform.position + rotation * (Vector3.forward * randomDistance);
        }
        return newPosition;

            }
        private Quaternion RotateTowardsTurret(Vector3 position,Vector3 targetposition)
        {
        Vector3 direction = targetposition - position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        return targetRotation;
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
