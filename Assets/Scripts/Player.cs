using UnityEngine;

namespace Exit {
  public class Player : MonoBehaviour {
    [SerializeField] private GameplayCharacterController CharacterController;
    [SerializeField] private float walkSpeed;
    
    [SerializeField] private float minJumpForce;
    [SerializeField] private float maxJumpForce;
    
    private float horizontalInput;
    private float horizontalMovement;
    private bool isJumping;
    private bool isWalking;
    private float nextJumpForce;
    
    private Input input;

    private void Awake()
    {
      input = new Input(this);
    }

    void Update() {
      horizontalMovement = horizontalInput * walkSpeed;
      input.Update();
      // UpdateWalkAnimationState();
    }

    public void TryJump() {
      isJumping = true;
      SetNextJumpForce();
    }

    private void SetNextJumpForce()
    {
      nextJumpForce = Random.Range(minJumpForce, maxJumpForce);
    }

    public void SetHorizontalInput(float input) {
      horizontalInput = input;
    }
    
    // private void UpdateWalkAnimationState() {
    //   if (Mathf.Abs(horizontalMovement) > 0 && isWalking == false) {
    //     isWalking = true;
    //     animator.SetBool(Constants.Animation.Walking, true);
    //   } else if (Math.Abs(horizontalMovement) < 0.01f && isWalking) {
    //     isWalking = false;
    //     animator.SetBool(Constants.Animation.Walking, false);
    //   }
    // }

    void FixedUpdate() {
      CharacterController.Move(horizontalMovement * Time.fixedDeltaTime, false, isJumping, nextJumpForce);
      isJumping = false;
    }
  }
}