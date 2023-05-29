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

    private bool reMove = true;
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
        IsEnemyAtPosition();
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
                if (Quaternion.Angle(rotate.transform.rotation, transform.rotation) >0.01f)
                {
                    Debug.Log('-');
                    Debug.Log(rotate.transform.rotation);
                    Quaternion newRotation = Quaternion.Lerp(rotate.transform.rotation, transform.rotation, rotationSpeed * Time.deltaTime);
                    Debug.Log(newRotation);
                    rotate.transform.rotation = newRotation;
                }
                else if(reMove)
                {
                    
                    StartCoroutine(WaitAndMoveToNewPosition(2));
                    reMove= false;
                }
            }
        }
    }
    private IEnumerator WaitAndMoveToNewPosition(float delay)
    {   
        yield return new WaitForSeconds(delay); // Chờ trong khoảng thời gian delay
        targetPosition = GetRandomPositionAroundTurret();
        MoveToTargetPosition();
        isRotating = true;
        canAttack = true;
        init = Quaternion.identity;
        reMove= true;

    }
    private void MoveToTargetPosition()
    {
        agent.SetDestination(targetPosition);   
    }
    private void IsEnemyAtPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, agent.radius*2f);

        foreach (Collider collider in colliders)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            Debug.Log(enemy);
            if (enemy != null&& enemy.gameObject != gameObject)
            {
                Vector3 avoidDirection = transform.position - collider.transform.position;
                avoidDirection.Normalize();

                // Tạo hướng di chuyển ngẫu nhiên
                Vector3 randomDirection = Random.insideUnitSphere;

                // Kết hợp hướng ngẫu nhiên với hướng tránh va chạm
                Vector3 newDirection = avoidDirection + randomDirection;
                newDirection.Normalize();

                // Đặt hướng di chuyển mới cho agent
                agent.velocity = newDirection * agent.speed;
            }
                
        }

    }
    private Vector3 GetRandomPositionAroundTurret()
    {
        
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(minDistanceToTurret, 60f);

        Quaternion rotation = Quaternion.Euler(0f, randomAngle, 0f);
        Vector3 newPosition = player.transform.position + rotation * (Vector3.forward * randomDistance);
        Debug.DrawRay(newPosition, Vector3.up, Color.blue, 5);
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
