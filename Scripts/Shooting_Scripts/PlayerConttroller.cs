using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerConttroller : LivingEntity
{
    private Rigidbody rb;
    private Vector3 moveInput;
    [SerializeField] private float moveSpeed;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        LookAtCursor();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    private void LookAtCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        float distToGround;
        if(plane.Raycast(ray, out distToGround))
        {
            Vector3 point = ray.GetPoint(distToGround);
            Vector3 rightPoint = new Vector3(point.x, transform.position.y, point.z);

            transform.LookAt(rightPoint);
        }
    }
}
