using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Steering : MonoBehaviour
{
    private Transform target;
    private Vector2[] eightDirections;
    private Vector2 resultDirection;
    public float speed = 10f;
    public float smoothTime = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").transform;
        
        eightDirections = new Vector2[8];
        for (int i = 0; i < 8; i++)
        {
            float angle = Mathf.Deg2Rad * 45 * i;
            eightDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetDirection = determineDirection();
        Vector3 targetPosition = transform.position + new Vector3(targetDirection.x, targetDirection.y, 0f) * speed;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothTime * Time.deltaTime);
    }

    private Vector2 determineDirection()
    {
        Vector2 targetDirection = new Vector2(target.position.x - transform.position.x,
            target.position.y - transform.position.y);
        
        Vector2 outputDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            outputDirection += eightDirections[i] * Vector2.Dot(targetDirection, eightDirections[i]);
        }

        outputDirection.Normalize();
        resultDirection = outputDirection;

        return resultDirection;
    }
}
