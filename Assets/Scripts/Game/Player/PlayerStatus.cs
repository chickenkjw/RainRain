using UnityEngine;

namespace Game.Player
{
    public class PlayerStatus: MonoBehaviour
    {
        public Sprite up;
        public Sprite down;
        public Sprite build;
        public Sprite open;
        public Sprite broke;
        public Sprite none;

        public SpriteRenderer Renderer;

        private void Start() {
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void ChangeState(PlayerActionState state) {
            switch (state) {
                case PlayerActionState.Up:
                    Renderer.sprite = up;
                    break;
                case PlayerActionState.Down:
                    Renderer.sprite = down;
                    break;
                case PlayerActionState.Build:
                    Renderer.sprite = build;
                    break;
                case PlayerActionState.Repair:
                    Renderer.sprite = open;
                    break;
                case PlayerActionState.Open:
                    Renderer.sprite = open;
                    break;
                default:
                    Renderer.sprite = none;
                    break;
            }
        }
    }
}