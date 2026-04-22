using UnityEngine;
using Fusion;
using TMPro;
using Animations;

public struct PlayerInputData : INetworkInput
{
    public Vector2 move;
    public NetworkBool jump;
    
}
namespace Movements
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [Networked] public bool FacingRight { get; set; }
        [Networked] public Vector2 NetVelocity { get; set; }
        string lastPlayerName = "";
        public TextMeshProUGUI playerName_Txt;
        PlayerNetworkData playerNetworkData;
        Flip flip_cs;
        CharacterAnimation anim_cs;
        GroundCheck groundCheck_cs;
        [SerializeField] float maxDistance = 12f; // giới hạn khoảng cách 2 player 
        [SerializeField] float _jumpForce = 12f; // Tăng nhẹ lên vì mạng thường có độ trễ vật lý
        [SerializeField] float _horizontalSpeed = 10f;
        [SerializeField] float _fallMultiplier = 2f;
        [SerializeField] float _lowJumpMultiplier = 2f;

        private float _horizontalDirection = 1;

        Rigidbody2D _rb;

        public float VelocityY => _rb.linearVelocity.y;
        
        // Fix lỗi cho con quái Trunk: Để public set
        public float HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            playerNetworkData = GetComponent<PlayerNetworkData>();
            flip_cs = GetComponentInChildren<Flip>();
            anim_cs = GetComponentInChildren<CharacterAnimation>();
            groundCheck_cs = GetComponentInChildren<GroundCheck>();

            if(playerName_Txt.text == null) playerName_Txt = GetComponentInChildren<TextMeshProUGUI>();
            // CỰC KỲ QUAN TRỌNG: 
            // Trong Multiplayer, ta nên để Rigidbody ở chế độ này để tránh bị rung (jitter)
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        }
        public override void Spawned()
        {
            base.Spawned();
            Debug.Log("Đã spawn player ");
            CameraManager.Instance.playerTransforms.Add(gameObject.transform);

        }
        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out PlayerInputData input))
                return;
            if (!Object.HasStateAuthority) return;
            float distance = GetMaxDistanceFromOthers();

            Vector2 move = input.move;

            if (distance > maxDistance)
            {
                // tìm player gần nhất (hoặc bất kỳ player khác)
                Transform other = null;

                foreach (var t in CameraManager.Instance.playerTransforms)
                {
                    if (t != null && t != transform)
                    {
                        other = t;
                        break;
                    }
                }

                if (other != null)
                {
                    Vector2 toOther = (other.position - transform.position).normalized;

                    // hướng input của player
                    Vector2 inputDir = move.normalized;

                    // dot < 0 nghĩa là đang đi RA XA
                    float dot = Vector2.Dot(inputDir, toOther);

                    if (dot < 0)
                    {
                        move = Vector2.zero; // ❗ chỉ chặn khi đi xa
                    }
                }
            }
            _rb.linearVelocity = new Vector2(move.x * _horizontalSpeed, _rb.linearVelocity.y);
            // flip hướng
            if (input.move.x > 0) FacingRight = true;
            else if (input.move.x < 0) FacingRight = false;

            // xử lý jump
            if (input.jump && Mathf.Abs(_rb.linearVelocity.y) < 0.01f) // chỉ nhảy khi đang đứng trên flatform
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            }

            // rơi nhanh hơn
            float velocityY = _rb.linearVelocity.y;
            if (velocityY < 0)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _fallMultiplier * Runner.DeltaTime;
            }
            else if (velocityY > 0.01f)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * _lowJumpMultiplier * Runner.DeltaTime;
            }
            NetVelocity = _rb.linearVelocity;
            
        }
        public override void Render()
        {
            base.Render();
            if(lastPlayerName != playerName_Txt.text && playerName_Txt != null && lastPlayerName != playerNetworkData.playerName.ToString())
            {
                lastPlayerName = playerNetworkData.playerName.ToString();
                playerName_Txt.text = lastPlayerName;
            }
            // flip
            flip_cs.FlipCharacter(FacingRight ? 1 : -1);

            anim_cs.HorizontalAnim(NetVelocity.x);
            anim_cs.JumpAnFallAnim(groundCheck_cs.IsOnGround, NetVelocity.y);
        }
        public void Jump()
        {
            // Reset velocity Y về 0 trước khi nhảy để lực nhảy luôn đồng nhất
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
        }

        public void HorizontalMove(float direction)
        {
            if (Mathf.Abs(direction) > 0.01f)
            {
                HorizontalDirection = Mathf.Sign(direction);
            }

            // Áp dụng vận tốc trực tiếp
            _rb.linearVelocity = new Vector2(direction * _horizontalSpeed, _rb.linearVelocity.y);
        }
        float GetMaxDistanceFromOthers()
        {
            float maxDist = 0f;

            foreach (var t in CameraManager.Instance.playerTransforms)
            {
                if (t == null || t == transform) continue;

                float dist = Vector3.Distance(transform.position, t.position);
                if (dist > maxDist)
                    maxDist = dist;
            }

            return maxDist;
        }
    }
}