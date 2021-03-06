using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BoidManager
{
	public static List<Obsticle> obsticles = new List<Obsticle>();
	public static List<Boid> boids = new List<Boid>();

	public static void AddObsticle(Obsticle obsticle)
	{
		obsticles.Add(obsticle);
	}

	

	public static void RemoveObsticle(Obsticle obsticle)
	{
		obsticles.Remove(obsticle);
	}

	public static void AddBoid(Boid boid)
	{
		boids.Add(boid);
	}

	public static void Reset()
	{
		obsticles.Clear();
		boids.Clear();
	}
}

public class Boid : MonoBehaviour
{
	[HideInInspector]
	public Vector3 target { get; private set; }
	

	[Header("Airplane Controls")]
	public float maxSpeed;
	[Range(10, 120)]
	public float maxTurnAngle;

	[Space]

	[Header("Pilot Controls")]
	[Range(1, 10)]
	public float viewDistance = 1;
	[Range(10, 360)]
	public int viewAngle = 10;
	[Range(0, 1)]
	public float bravery = 0.5f;

	[Space]

	[Header("Path Display")]
	public int displaySegments;

	[Space]

	[Header("Scoring System")]
	public float maxScore = 100.0f;
	public float minScore = 20.0f;
	public float delayTimeForgiveness = 2.0f;
	public float delayTimeMax= 10.0f;

	[Space]

	[Header("Visualization")]
	public MeshRenderer airplane;
	public Material material;

	// Physics Variables
	private Rigidbody rb;
	private LineRenderer line;
	private new SphereCollider collider;
	private Vector3 desiredVelocity;
	private float desiredSpeed;

	//Score variable;
	private float timeOfSpawn;
	private float timeElapsed;
	private float totalDistance;
	private float estimatedTimeToFly;

	// Automation Variables
	[HideInInspector]
	public bool isLanding;
	private static int braveryShaderID = -1;
	private static int outlineShaderID = -1;

	#region Setters

	void SetInitialVelocity(Vector3 intial)
	{
		rb.velocity = intial;
	}
	public void SetTarget(Vector3 position)
	{
		target = position;
	}

	#endregion

	public void OnBraveryChanged(float value)
	{
		bravery = value;
		airplane.material.SetFloat(braveryShaderID, value);
	}

	public void Init()
	{
		rb = GetComponent<Rigidbody>();
		line = GetComponent<LineRenderer>();
		collider = GetComponent<SphereCollider>();

		line.positionCount = displaySegments + 1;
		SetOutline(false);

		isLanding = false;
		if (landing != null)
		{
			StopCoroutine(landing);
			landing = null;
		}
		transform.localScale = Vector3.one;
		
		desiredSpeed = maxSpeed;
		SetInitialVelocity(transform.forward * desiredSpeed);
		OnBraveryChanged(Random.Range(0.0f, 1.0f));
		
		//Calculate optimal distance for score
		timeOfSpawn = Time.time;
		totalDistance = Vector3.Distance(target, transform.position);
		estimatedTimeToFly = timeOfSpawn + (totalDistance / maxSpeed); // * Time.deltaTime;

	}

	public void SetTangibility(bool isTangible)
    {
		collider.isTrigger = !isTangible;
    }

	public void SetOutline(bool useOutline)
    {
		airplane.material.SetFloat(outlineShaderID, useOutline ? 1.0f : 0.0f);
	}

	void Start()
	{
		airplane.material = new Material(material);
		if (braveryShaderID < 0)
			braveryShaderID = Shader.PropertyToID("_Bravery");
		if (outlineShaderID < 0)
			outlineShaderID = Shader.PropertyToID("_EnableOutline");

		BoidManager.AddBoid(this);
		Init();
	}

	void Update()
	{
		LandingCheck();
	
		SetTangibility(IsInFrustum());
		desiredVelocity = Aim(target);
		desiredVelocity += Avoidance() * (1 - bravery);

		EstimatedPathDisplay();
		transform.LookAt(transform.position + rb.velocity);
	}

	void FixedUpdate()
	{
		Vector3 newVelocity = (rb.velocity + desiredVelocity).normalized;

		float desiredAngle = Mathf.Min(180 - (1 + Vector3.Dot(transform.forward, newVelocity)) * 90.0f, maxTurnAngle);
		Vector3 side = Mathf.Sign(Vector3.Dot(transform.right, newVelocity)) * transform.right;
		newVelocity = DirectionFromAngle(transform.forward, side, desiredAngle) * desiredSpeed;

		rb.velocity = LimitVelocity(newVelocity, desiredSpeed);
	}

	bool IsInFrustum()
    {
		float height = Camera.main.orthographicSize;
		float width = Camera.main.orthographicSize * Camera.main.aspect;

		//print(height);
		//print(width);

		return transform.position.z < height && transform.position.z > -height && transform.position.x < width && transform.position.x > -width;
    }

	void LandingCheck()
	{
		if (isLanding)
			return;
		
		if (Vector3.Distance(target, transform.position) <= viewDistance)
		{
			isLanding = true;
			landing = StartCoroutine(Land());
		}
	}

	public void Score(float score)
    {
		GM.addScoreToTotal((int)score);
    }

	Coroutine landing = null;
	IEnumerator Land()
    {
		float time = 0;
		while(time < 1.5f)
        {
			transform.localScale = Vector3.one * (1.0f - time / 1.5f);
			yield return null;
			time += Time.deltaTime;
        }

		float endTime = Time.time;
		float delayTime = endTime - estimatedTimeToFly;

		//Debug.Log("Delay Time: " + delayTime);

        if (delayTime < delayTimeForgiveness)
        {
			Score(maxScore);
		}else if (delayTime > delayTimeForgiveness + delayTimeMax)
		{
			Score(minScore);
        }
        else
        {
			float percentOfScore = (delayTime - delayTimeForgiveness) / delayTimeMax;
			float endScore = ((maxScore - minScore) * percentOfScore) + minScore;
			Score(endScore);
        }
		gameObject.SetActive(false);
    }

	// Calculates the estimated trajectory of the airplane based off of a squared-bezier curve.
	void EstimatedPathDisplay()
	{
		Vector3[] displayPoints = new Vector3[displaySegments + 1];

		for (int i = 0; i <= displaySegments; i++)
		{
			float t = (float)i / displaySegments;
			Vector3 point = transform.forward * desiredSpeed * 3.0f + transform.position;
			
			Vector3 lp1 = Vector3.Lerp(transform.position, point, t);
			Vector3 lp2 = Vector3.Lerp(point, target, t);

			displayPoints[i] = Vector3.Lerp(lp1, lp2, t);
		}

		line.SetPositions(displayPoints);
	}

    #region Flocking Functions

    Vector3 Aim(Vector3 point)
	{
		Vector3 aim = (point - transform.position).normalized;
		float desiredAngle = Mathf.Min(180 - (1 + Vector3.Dot(transform.forward, aim)) * 90.0f, maxTurnAngle);
		
		desiredSpeed = (desiredAngle > maxTurnAngle) ? maxSpeed : Mathf.Min(Vector3.Distance(point, transform.position), maxSpeed);
		
		Vector3 side = Mathf.Sign(Vector3.Dot(transform.right, aim)) * transform.right;
		return DirectionFromAngle(transform.forward, side, desiredAngle);
	}

	Vector3 Avoidance()
	{
		Vector3 avoidance = Vector3.zero;
		int inRangeCount = 0;

		// Avoid over boids
		foreach(Boid boid in BoidManager.boids)
		{
			if (boid == this || boid.isLanding)
				continue;

			float distToBoid = Vector3.Distance(transform.position, boid.transform.position);
			float boidViewDist = viewDistance * 2.0f;
			if (distToBoid > boidViewDist)
				continue;

			Vector3 vectorToBoid = (boid.transform.position - transform.position).normalized;
			float angleToBoid = 180.0f - (1 + Vector3.Dot(transform.forward, vectorToBoid)) * 90.0f;
			if (angleToBoid > viewAngle / 2.0f)
				continue;
			
			avoidance += -vectorToBoid * (boidViewDist - distToBoid) / boidViewDist * 2.0f;
			inRangeCount++;
		}

		// Avoid obsticles
		foreach (Obsticle obsticle in BoidManager.obsticles)
		{
			float distToObsticle = Vector3.Distance(transform.position, obsticle.transform.position);
			float obsticleViewDist = viewDistance + obsticle.radius;
			if (distToObsticle > obsticleViewDist)
				continue;

			Vector3 vectorToObsticle = (obsticle.transform.position - transform.position).normalized;
			/* This code checks to see if the obsticle falls inside the boids view range but the resulting force feels rather harsh.
			 * 
			float angleToBoid = 180.0f - (1 + Vector3.Dot(transform.forward, vectorToObsticle)) * 90.0f;
			if (angleToBoid > viewAngle / 2.0f)
				continue;
			*/

			avoidance += -vectorToObsticle * (obsticleViewDist - distToObsticle) / obsticleViewDist * 3.0f;
			inRangeCount++;
		}

		if (inRangeCount != 0)
			avoidance /= inRangeCount;

		return avoidance;
	}

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
		if (!collision.gameObject.GetComponent<Boid>())
			return;

		// A collision between airplanes just happened
		GM.explosionAt(this.transform);
		
		//GM.GameOver();
		//FIX THIS SHOULD DECREMENT SCORE
		gameObject.SetActive(false);
	}

    #region Utility Functions

    Vector3 LimitVelocity(Vector3 velocity, float limit)
	{
		return Vector3.ClampMagnitude(velocity, limit);
	}
	Vector3 DirectionFromAngle(Vector3 forward, Vector3 side, float angle)
	{
		float radAngle = Mathf.Deg2Rad * angle;
		return (forward * Mathf.Cos(radAngle) + side * Mathf.Sin(radAngle)).normalized;
	}

	#endregion

	#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		// Pilot Variable Visualizers
		Handles.DrawWireArc(transform.position, Vector3.up, DirectionFromAngle(transform.forward, -transform.right, viewAngle / 2), viewAngle, viewDistance);
		Gizmos.DrawRay(transform.position, DirectionFromAngle(transform.forward,  transform.right, viewAngle / 2) * viewDistance);
		Gizmos.DrawRay(transform.position, DirectionFromAngle(transform.forward, -transform.right, viewAngle / 2) * viewDistance);
		

		Handles.color = Color.cyan;
		Gizmos.color = Color.cyan;


		// Plane Variable Visualizers
		Handles.DrawWireArc(transform.position, Vector3.up, DirectionFromAngle(transform.forward, -transform.right, maxTurnAngle), maxTurnAngle * 2, maxSpeed);
		Gizmos.DrawRay(transform.position, DirectionFromAngle(transform.forward, transform.right, maxTurnAngle) * maxSpeed);
		Gizmos.DrawRay(transform.position, DirectionFromAngle(transform.forward, -transform.right, maxTurnAngle) * maxSpeed);

		if (!Application.isPlaying)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position, transform.forward * maxSpeed);
			return;
		}

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, desiredVelocity * desiredSpeed);

		Gizmos.color = (isLanding) ? Color.green : Color.red;
		Gizmos.DrawRay(transform.position, rb.velocity);
	}
	#endif
}
