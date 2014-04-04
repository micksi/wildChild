using UnityEngine;
using System.Collections;

// author: Thorbjørn
// This class is used for debugging. It will display 
// the focus circle by means of many little yellow
// primitives.
// Its purpose is/was to prove that MasterOfLOD worked like it should.
public class FocusIllustrator : MonoBehaviour {

	private MasterOfLOD mol;
	private GameObject[] points;
	private int noOfPoints = 40;

	// Use this for initialization
	void Start () {
		points = new GameObject[noOfPoints];
		for(int i = 0; i < noOfPoints; i++)
		{
			points[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
			points[i].renderer.material.color = Color.yellow;
			points[i].transform.localScale = new Vector3(0.01f,0.01f,0.01f);
		}

		mol = GetComponent<MasterOfLOD>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 focus = FocusProvider.GetFocusDirection();
		float focusAngle = mol.ingameFocusRadiusRadians;

		Vector3 edge = Vector3.RotateTowards(focus, Camera.main.transform.up, focusAngle, 1).normalized;
		Vector3 edgeParallel = focus * Vector3.Dot(edge, focus);
		Vector3 edgePerpendicular = edge - edgeParallel;

		for(int i = 0; i < noOfPoints; i++)
		{
			/*
			Decompose P into P = proj(P on A) + perp(P on A), 
			then rotate perp(P on A) around A and adding the components back together. 
			The rotation of the perpendicular component is then given by NEWLTAB 
			perp(P on A) * cos a + (A x P) * sin a
			*/

			float rotationAngle = i * 2f * 3.1415f / noOfPoints;
			Vector3 rotatedOrthogonal = 		 edgePerpendicular * Mathf.Cos(rotationAngle) 
									  + Vector3.Cross(focus, edge) * Mathf.Sin(rotationAngle);
			points[i].transform.position = Camera.main.transform.position + edgeParallel + rotatedOrthogonal;
		}
	}
}
