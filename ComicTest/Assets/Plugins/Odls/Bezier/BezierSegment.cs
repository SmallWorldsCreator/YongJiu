using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BezierSegment{
	public Vector3 startPoint = Vector3.zero;
	public Vector3 endPoint = Vector3.zero;
	public float distance = 0;
	public float distanceSum = 0;

	public BezierSegment(float p_distanceBefor, Vector3 p_startPoint, Vector3 p_endPoint){
		SetPoint (p_distanceBefor, p_startPoint, p_endPoint);
	}
	public void SetPoint(float p_distanceBefor, Vector3 p_startPoint, Vector3 p_endPoint){
		startPoint = p_startPoint;
		endPoint = p_endPoint;
		distance = (endPoint - startPoint).magnitude;
		distanceSum = p_distanceBefor + distance;
	}

	public Vector3 GetPoint(float p_targetDistance){
		return Vector3.Lerp (startPoint, endPoint, (p_targetDistance / distance));
	}
	public Vector3 GetPointInSum(float p_targetDistance){
		float _laveDistance = distanceSum - p_targetDistance;
		return GetPoint(distance - _laveDistance);

	}
}
