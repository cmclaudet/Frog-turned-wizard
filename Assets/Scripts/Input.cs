namespace Exit {
    public class Input {
        private bool canMove = true;
        private readonly Player player;
        private bool verticalAxisInUse;
        public bool isPendingTeleport;

        public Input(Player player) {
            this.player = player;
        }

        public void Update() {
            if (canMove == false) {
                player.SetHorizontalInput(0);
                return;
            }

            player.SetHorizontalInput(UnityEngine.Input.GetAxisRaw("Horizontal"));

            if (UnityEngine.Input.GetButtonDown("Jump")) {
                player.TryJump();
            } else if (UnityEngine.Input.GetButtonUp ("Jump") && isPendingTeleport) {
                isPendingTeleport = false;
                player.TryTeleport();
            }
        }

        public void ToggleCanMove(bool canMove) {
            this.canMove = canMove;
        }
    }
}