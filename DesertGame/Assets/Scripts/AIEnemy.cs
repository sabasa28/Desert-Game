using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AIEnemy : MonoBehaviour
{
    AIDestinationSetter chaser;
    AIPath aiPath;
    Transform player;
    protected enum State
    { 
        idle,
        searching,
        chasing
    }
    State currentState;

    [SerializeField] float alertRange = 10;
    [SerializeField] float chasingRange = 15;
    [SerializeField] float hearingRange = 10;
    [SerializeField] bool returnsToOriginalPos;
    [SerializeField] bool patrols = true;
    [SerializeField] float minPatrolDistance = 1;
    [SerializeField] float maxPatrolDistance = 10;
    [SerializeField] float timeBetweeenPatrols = 4.0f;
    Vector3 originalPos;
    Vector3 playersLastPos;
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
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case State.idle:
                if (CanSeePlayer(alertRange)) Chase(true);
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
        return Vector2.Distance(transform.position, player.position) < distanceToCheck;
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
            aiPath.destination = playersLastPos;
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
            playersLastPos = playerPos;
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
}
