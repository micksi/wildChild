using UnityEngine;
using System.Collections;

public class RailTrack : MonoBehaviour {

	public GameObject[] railTrackPoints;
	public float speed = 3.0f;
	int nextWayPoint = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if(nextWayPoint < railTrackPoints.Length )
		{
			if (railTrackPoints.Length > 0)
			{
				if(Vector3.Distance(transform.position,railTrackPoints[nextWayPoint].transform.position) > 0.1)
				{
					transform.position += transform.forward * speed * Time.deltaTime;
					LookAtLerp(railTrackPoints[nextWayPoint].transform);
				}
				else if(railTrackPoints[nextWayPoint].GetComponent<RailPoint>().isCheckPoint == true)
				{
					LookAtLerp(railTrackPoints[nextWayPoint+1].transform);
				}

				if(Vector3.Distance(transform.position,railTrackPoints[nextWayPoint].transform.position) < 0.5
				   && railTrackPoints[nextWayPoint].GetComponent<RailPoint>().isCheckPoint == false)
				{
					nextWayPoint++;
				}
			}
		}
		else
		{
			print ("finish");
		}
	}

	protected virtual void RotateTowards(Quaternion rotation, float margin = 0.999f)
	{
		if(Quaternion.Dot(transform.rotation, rotation) < margin)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 3.0f * Time.deltaTime);
		}
		else if(transform.rotation == rotation)
		{
			return;
		}
		else
		{
			transform.rotation = rotation;
		}
	}
	
	private void LookAtLerp(Transform go)
	{
		LookAtLerp(go.position);
	}
	
	protected virtual void LookAtLerp(Vector3 lookAt)
	{
		Vector3 direction = (lookAt - transform.position);
		Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
		RotateTowards(targetRotation);
	}
}
