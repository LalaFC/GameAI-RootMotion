using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]private Animator anim;
    private InputAction moveInput, jumpInput, crouchInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        moveInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        crouchInput = InputSystem.actions.FindAction("Crouch");
    }
    private void Update()
    {
        MovePlayer(moveInput.ReadValue<Vector2>(), jumpInput.triggered, crouchInput.IsInProgress());
    }
    public void GetAnim(Animator pAnim)
    {
        anim = pAnim;
    }
    private void MovePlayer(Vector2 move, bool jump, bool crouch)
    {

        Move(move);
        Jump(jump);
        Crouch(crouch);
    }

    public void Move(Vector2 move)
    {
        anim.SetFloat("Horizontal", move.x, 0.2f, Time.deltaTime);
        anim.SetFloat("Vertical", move.y, 0.2f, Time.deltaTime);
    }
    [SerializeField] float jumpForce;
    public void Jump(bool isJumping)
    {

        if (isJumping)
        {
            anim.ResetTrigger("Jump");
            anim.SetTrigger("Jump");
            var direction = new Vector3 (moveInput.ReadValue<Vector2>().x, 1, moveInput.ReadValue<Vector2>().y).normalized;
            Vector3 jumpForceVector = Vector3.up * jumpForce;

            if (direction.magnitude > 0.01f)
            {
                jumpForceVector += direction * jumpForce;
            }

            this.GetComponent<Rigidbody>().AddForce(jumpForceVector, ForceMode.Impulse); 

        }

    }
    public void Crouch(bool isCrouching)
    {
        anim.SetBool("Crouching", isCrouching);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<AIRootMotionCrtler>() != null)
        {
            other.GetComponent<AIRootMotionCrtler>().AcquireTarget(this.gameObject);
        }
    }
}
