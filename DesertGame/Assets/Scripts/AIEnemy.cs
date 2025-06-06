using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AIEnemy : MonoBehaviour
{
    AIDestinationSetter chaser;
    AIPath aiPath;
    Transform player;
    public enum State
    { 
        idle,
        searching,
        chasing
    }
    public State currentState;

    [SerializeField] float alertRange = 10;
    [SerializeField] float chasingRange = 15;
    [SerializeField] float maxHearingRange = 10;
    [SerializeField] bool deaf = false;
    [SerializeField] bool returnsToOriginalPos;
    [SerializeField] bool patrols = true;
    [SerializeField] float minPatrolDistance = 1;
    [SerializeField] float maxPatrolDistance = 10;
    [SerializeField] float timeBetweeenPatrols = 4.0f;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip idleSound;
    [SerializeField] AudioClip chasingSound;
    Noise HighestPriorityNoise;

    Vector3 originalPos;
    Vector3 playersLastPosSeen;
    float timer;


    private void Awake()
    {
        chaser = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }
    protected virtual void Start()
    {
        SetAsIdle();
        StartPartolling();
        originalPos = transform.localPosition;
        player = GameplayController.Get().GetCurrentPlayer().transform;
        StartCoroutine(PlayIdleSound());
        StartCoroutine(PlayRunningSound());
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case State.idle:
                if (CanSeePlayer(alertRange)) Chase(true);
                else if (CanHearPlayerOrDecoy()) InvestigateNoise();
                else if (patrols && aiPath.reachedEndOfPath)
                    if (timer < timeBetweeenPatrols)
                    {
                        timer += Time.deltaTime;
                    }
                    else
                    {
                        StartPartolling();
                    }
                break;
            case State.chasing:
                if (!CanSeePlayer(chasingRange)) StartSearching();
                break;
            case State.searching:
                if (CanSeePlayer(chasingRange)) Chase(true);
                else if (CanHearPlayerOrDecoy()) InvestigateNoise();
                else if (aiPath.reachedEndOfPath)
                {
                    if (returnsToOriginalPos && aiPath.destination != originalPos) aiPath.destination = originalPos;// aiPath.FinalizeMovement(posToDefend, transform.rotation);
                    else SetAsIdle();
                }
                break;
        }
    }

    bool CanSeePlayer(float distanceToCheck)
    {
        int layer = LayerMask.GetMask("Obstacle", "Default");
        if (Vector2.Distance(transform.position, player.position) < distanceToCheck)
        {
            RaycastHit2D ray = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, distanceToCheck, layer);
            Debug.DrawRay(transform.position, (player.position - transform.position).normalized * distanceToCheck, Color.red, 1.0f);
            if (ray.collider.transform == player) return true;
        }
        return false;
    }

    bool CanHearPlayerOrDecoy()
    {
        return (HighestPriorityNoise.noiseLevel != NoiseLevel.none);
    }

    public void ReceiveNoise(Noise noise)
    {
        if (deaf) return;
        float distToNoise = Vector2.Distance(noise.noiseOrigin, transform.position);
        switch (noise.noiseLevel)
        {
            case NoiseLevel.none:
                return;
            case NoiseLevel.low:
                if (distToNoise > maxHearingRange / 4.0f) return;
                else break;
            case NoiseLevel.medium:
                if (distToNoise > maxHearingRange / 2.0f) return;
                else break;
            case NoiseLevel.high:
                if (distToNoise > maxHearingRange) return;
                else break;
            case NoiseLevel.global:
                break;
            default:
                break;
        }
        Debug.Log("Dist to noise " + distToNoise);
        HighestPriorityNoise = noise; //TODO hacer un sistema para que checkee segun volumen y distancia si el ruido anterior es mas prioritario que el nuevo
    }

    public void InvestigateNoise()
    {
        aiPath.destination = HighestPriorityNoise.noiseOrigin;
        HighestPriorityNoise.noiseLevel = NoiseLevel.none;
        StartSearching();

    }

    protected virtual void Chase(bool seesPlayer)
    {
        currentState = State.chasing;
        if (seesPlayer)
        {
            chaser.target = player;
        }
        else
        { 
            aiPath.destination = playersLastPosSeen;
        }
    }

    protected virtual void StartSearching()
    {
        currentState = State.searching;
        chaser.target = null;
        timer = 0.0f;
    }

    protected virtual void SetAsIdle()
    {
        currentState = State.idle;
    }

    public void TryListenToPlayer(Vector2 playerPos, float soundDistance)
    {
        if (currentState == State.chasing) return;
        if (Vector2.Distance(playerPos, new Vector2(transform.position.x, transform.position.y)) < soundDistance) //cambiar player pos por pos del objeto que hizo ruido
        {
            playersLastPosSeen = playerPos;
            Chase(false);
        }
    }

    protected void StartPartolling()
    {
        timer = 0.0f;
        int randX = Random.Range(-1,2);
        int randY = Random.Range(-1,2);
        Vector2 movementDir = new Vector2(randX, randY).normalized;
        Debug.Log("PATRULLANDO HACIA " + movementDir);
        RaycastHit2D ray = Physics2D.Raycast(transform.position, movementDir, maxPatrolDistance, (1 << LayerMask.NameToLayer("Obstacle")));
        Debug.DrawRay(transform.position, movementDir * maxPatrolDistance, Color.red, 2.0f);
        float movementRandLenght = Random.Range(minPatrolDistance, maxPatrolDistance);
        float movementLenght = ray.collider && (ray.distance < movementRandLenght) ? ray.distance : movementRandLenght;
        aiPath.destination = ((Vector2)transform.position + movementDir * movementLenght);
        aiPath.SearchPath();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("HURT");

            collision.GetComponent<PlayerController>().TakeDamage();
        }
    }

    IEnumerator PlayIdleSound()
    {
        int rand;
        while (true)
        {
            rand = Random.Range(2, 7);
            yield return new WaitForSeconds(rand);
            if (currentState == State.idle)
            { 
                audioSource.clip = idleSound;
                audioSource.Play();
            } 
        }
    }

    IEnumerator PlayRunningSound()
    {
        int rand;
        while (true)
        {
            rand = Random.Range(2, 3);
            yield return new WaitForSeconds(rand);
            if (currentState == State.chasing)
            {
                audioSource.clip = chasingSound;
                audioSource.Play();
            }
        }
    }
}
