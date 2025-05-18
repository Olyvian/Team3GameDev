using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform subject;
    Vector2 startPosition;
    float startZ;
    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distanceFromSubject => transform.position.z - subject.position.z;
    float clipingPlane => (cam.transform.position.z + (distanceFromSubject > 0? cam.farClipPlane : cam.nearClipPlane));
    float paralaxFactor => Mathf.Abs(distanceFromSubject)/clipingPlane;
    public void Start(){
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    public void Update(){
        Vector2 newPos = startPosition + travel *0.9f;
        transform.position = new Vector3(newPos.x, newPos.y, startZ);
    }
}
