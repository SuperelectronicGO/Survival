using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormAI : CreatureAIInterface
{
    [SerializeField] private bool hasTarget;
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask moveLayers;
    [SerializeField] private LayerMask targetLayers;
    private bool moving;
    /*
     * 0: Idle   ->   0 - Still, 01 - Idle Moving
     * 10: Agro   ->   10 - Moving towards target, 11 - Attacking
     */
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(IdleBehavior());
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
          transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }


    //Coroutine that scans the surrounding area for targets every so often
    private IEnumerator CheckForTargets(int radius)
    {
        while (true)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayers);
            bool foundTag = false;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].TryGetComponent<ColliderTags>(out ColliderTags tagList))
                {
                    for (int j = 0; j < tagList.tags.Length; j++)
                    {
                        for (int k = 0; k < targetTags.Length; k++)
                        {
                            if (tagList.tags[j] == targetTags[k])
                            {
                                //If detected object is player
                                if (targetTags[k] == ColliderTags.TagType.Player)
                                {
                                    MovePlayer playerTarget = tagList.transform.parent.transform.GetComponent<MovePlayer>();
                                    for (int p = 0; i < 40; i++)
                                    {
                                        if (true)
                                        {
                                            target = playerTarget.transform;
                                            hasTarget = true;
                                            foundTag = true;
                                            break;
                                        }
                                        else
                                        {
                                            yield return new WaitForSecondsRealtime(0.1f);
                                        }
                                    }
                                }
                                else
                                {
                                    target = tagList.transform.parent.transform;
                                    hasTarget = true;
                                    foundTag = true;
                                    break;
                                }
                            }
                        }
                        if (foundTag)
                        {
                            break;
                        }
                    }
                    if (foundTag)
                    {
                        break;
                    }
                }
            }
            if (!foundTag)
            {
                yield return new WaitForSecondsRealtime(2f);
                scanned = true;
            }
            else
            {
                Emerge(target.transform.position.y);
                StopCoroutine(IdleBehavior());
                yield break;
            }
            }
        }

    //Coroutine that controls the worms idle behavior
    bool scanned = false;
    private IEnumerator IdleBehavior()
    {
        while (true)
        {
            //Get target position to move to
            Vector3 targetMovePosition = GetRandomPositionForMove(40);
            //Burrow
            Burrow();
            //Move to the target position
            Vector3 posVecYOut = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 targetPosVecYOut = new Vector3(targetMovePosition.x, 0, targetMovePosition.z);
            float dist = Vector3.Distance(posVecYOut, targetPosVecYOut);
            while (dist > 3)
            {
                Vector3 newPosition = Vector3.MoveTowards(posVecYOut, targetPosVecYOut, (moveSpeed) * Time.deltaTime);
                transform.position = newPosition;
                posVecYOut = new Vector3(transform.position.x, 0, transform.position.z);
                dist = Vector3.Distance(posVecYOut, targetPosVecYOut);
                yield return new WaitForEndOfFrame();
            
            }
            dist = 100;
            //Emerge
            Emerge(targetMovePosition.y);
            int scanTimes = Random.Range(3, 7);
            while (scanTimes > 0)
            {

                StartCoroutine(CheckForTargets(50));
                yield return new WaitUntil(() => scanned == true);
                scanTimes -= 1;
                scanned = false;
                yield return new WaitForEndOfFrame();
            }
        }
    }

    RaycastHit hit;
    Ray ray;
    //Method that uses a raycast to get a random position to move to
    private Vector3 GetRandomPositionForMove(float range)
    {
        Vector3 targetPosition = Vector3.zero;
        float offsetX = transform.position.x + Random.Range(-range, range);
        float offsetY = transform.position.z + Random.Range(-range, range);
        if(Physics.Raycast(new Vector3(offsetX, 1000, offsetY), Vector3.down, out hit, 1000, moveLayers))
        {
            targetPosition = hit.point;
        }
        else
        {
            Debug.LogError("How the fuck did we not hit the ground?");
        }
        return targetPosition;
    }
    private void Burrow()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y-100, transform.position.z);
    }
    private void Emerge(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    
}
