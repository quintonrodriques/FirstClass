using System.Collections;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public bool isAvailable = false;
    public float flySpeed = 6.0f;

    [Range(0.0f, 1.0f)]
    public float careFactor = 0.5f;
    public int pathRequestsPerSecond = 1;
    public float proximityThreshold = 0.5f;

    public int travelIndex;
    private Vector3 targetPosition;
    private Vector3[] calculatedPath;

    public void SetTarget(Vector3 target) { targetPosition = target; }

    public void BeginJourney()
    {
        gameObject.SetActive(true);

        travelIndex = 1;
        transform.LookAt(targetPosition);

        calculatedPath = Pathfinding.FindPath(transform.position, targetPosition);

        isRequestingPath = StartCoroutine(RequestPath(60.0f / pathRequestsPerSecond));
        
        isAvailable = false;
    }

    public void EndJourney()
    {
        if (isRequestingPath != null)
        {
            StopCoroutine(isRequestingPath);
            isRequestingPath = null;
        }

        isAvailable = true;
        gameObject.SetActive(false);
    }

    Coroutine isRequestingPath = null;
    IEnumerator RequestPath(float timeBetweenRequests)
    {
        while (!isAvailable)
        {
            calculatedPath = Pathfinding.FindPath(transform.position, targetPosition);
            yield return new WaitForSeconds(timeBetweenRequests);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (!isAvailable)
            Travel();
    }

    void Travel()
    {
        Vector3[] path = new Vector3[calculatedPath.Length];
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 directPathPos = Vector3.Lerp(calculatedPath[0], targetPosition, (float)i / (path.Length - 1));
            path[i] = Vector3.Lerp(directPathPos, calculatedPath[i], careFactor);
        }

        transform.LookAt(path[travelIndex]);
        transform.position = Vector3.MoveTowards(transform.position, path[travelIndex], flySpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, path[travelIndex]) <= proximityThreshold)
        {
            if (travelIndex >= path.Length - 1)
            {
                EndJourney();
                return;
            }

            travelIndex++;
        }
    }
}