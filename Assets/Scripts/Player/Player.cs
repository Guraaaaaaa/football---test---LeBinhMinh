using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;
    public Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        HandleMovement(); 
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (h != 0 || v != 0)
        {
            animator.SetBool("Run", true);
        }
        if (h == 0 && v == 0)
        {
            animator.SetBool("Run", false);
        }

        // Lấy Camera chính trong scene
        Camera mainCam = Camera.main;
        Vector3 moveDir;

        if (mainCam != null)
        {
            Vector3 camForward = mainCam.transform.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = mainCam.transform.right;
            camRight.y = 0f;
            camRight.Normalize();

            moveDir = (camForward * v + camRight * h).normalized;
        }
        else
        {
            moveDir = new Vector3(h, 0f, v).normalized;
        }

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public float MoveSpeed => moveSpeed;
}
