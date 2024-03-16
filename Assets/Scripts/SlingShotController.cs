using Unity.VisualScripting;
using UnityEngine;

public class SlingShotController : MonoBehaviour
{
    public float maxStretch = 3.0f;
    public float projectileSpeed = 50000f;
    public float frictionRate = 10f;

    [SerializeField] private Rigidbody2D projectileRigidbody;
    public Transform player;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private float power;
    private float pressedTime;
    [SerializeField] private LineRenderer lineRenderer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = player.position;
            pressedTime = Time.time;
            lineRenderer.SetPosition(0, player.position);
            lineRenderer.SetPosition(1, GetMouseWorldPosition());
            lineRenderer.enabled = true;
        }

        if (Input.GetMouseButton(0))
        {
            lineRenderer.SetPosition(1, GetMouseWorldPosition());
        }

        if (Input.GetMouseButtonUp(0))
        {
            endPoint = GetMouseWorldPosition();

            power = Time.time - pressedTime < maxStretch ? Time.time - pressedTime : maxStretch;
            lineRenderer.enabled = false;
        }

        player.position = Vector2.MoveTowards(player.position, endPoint, Time.deltaTime * power * projectileSpeed);
    }

    Vector3 GetMouseWorldPosition()
    {
        return (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
