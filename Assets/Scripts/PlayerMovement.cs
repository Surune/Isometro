using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Sprite idleSprite; // Sprite when not moving
    public Sprite upSprite;    // Sprite when moving up
    public Sprite downSprite;  // Sprite when moving down
    public Sprite leftSprite;  // Sprite when moving left
    public Sprite rightSprite; // Sprite when moving right

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // Normalize the movement vector to ensure consistent speed in all directions
        movement.Normalize();

        // Move the character
        rb.velocity = movement * moveSpeed;
        SetSprite(movement);
    }
    
    void SetSprite(Vector2 movement)
    {
        if (movement.magnitude > 0.1f)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                // Moving horizontally
                spriteRenderer.sprite = movement.x > 0 ? rightSprite : leftSprite;
            }
            else
            {
                // Moving vertically
                spriteRenderer.sprite = movement.y > 0 ? upSprite : downSprite;
            }
        }
        else
        {
            // Not moving, set the idle sprite
            spriteRenderer.sprite = idleSprite;
        }
    }
}
