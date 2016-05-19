using UnityEngine;
using System.Collections;

public class RandomWalk : MonoBehaviour {

	private NavMeshAgent agent;
	private Vector3 target;

	// Use this for initialization
	void Start () {
		agent = this.GetComponent<NavMeshAgent> ();
		target = getRandomTarget ();
	}
	
	// Update is called once per frame
	void Update () {
		agent.SetDestination (target);

		if (this.transform.position == target) {
			target = getRandomTarget ();
		}
	}

	private Vector3 getRandomTarget(){
		agent.speed = Random.Range (1, 5);
		Vector3 randomDirection = Random.insideUnitSphere * 30;
		randomDirection += transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition (randomDirection, out hit, 30, 1);
		return hit.position;
	}
}
