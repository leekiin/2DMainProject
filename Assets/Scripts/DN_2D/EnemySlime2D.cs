using UnityEngine;

enum EnemySlime2DState
{
    Idle,
    Move,
    Die
}

public class EnemySlime2D : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer_Enemy;

    public int EntityInstancId { get; private set; }

    private EnemySlime2DState _currentState = EnemySlime2DState.Idle;

    private Vector3 _moveDirection;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2.0f;

    [Header("애니메이터")]
    [SerializeField] private SlimeAnimatorControllerSlime2D AnimatorController_Entity;

    private float _stateTimer = 0f;
    private float _nextDecisionTime = 0f;

    void Start()
    {
        DecideNextAction();
    }

    void Update()
    {
        _stateTimer += Time.deltaTime;

        if ((_stateTimer >= _nextDecisionTime))
        {
            DecideNextAction();
        }

        HandleMovementOnUpdate();
    }

    public void InitEnemyInfo(int instanceId)
    {
        EntityInstancId = instanceId;

    }

    public void DecideNextAction()
    {
        _stateTimer = 0f;

        int randomChoice = Random.Range(0, 3);

        if(randomChoice == 0)
        {
            _currentState = EnemySlime2DState.Idle;
            _moveDirection = Vector3.zero;
            _nextDecisionTime = Random.Range(1.0f, 3.0f);
            ChangeEnemyState(SlimeEntityAnimState.Idle);
        }
        else
        {
            _currentState = EnemySlime2DState.Move;
            float randomX = (randomChoice == 1) ? -1f : 1f;
            _moveDirection = new Vector3(randomX, 0, 0);
            SetMeshDirectionByMoveDirection((int)_moveDirection.x);
            _nextDecisionTime = Random.Range(1.0f, 4.0f);
            ChangeEnemyState(SlimeEntityAnimState.Move);

        }
    }

    void SetMeshDirectionByMoveDirection(int x)
    {
        //SpriteRenderer_Enemy.flipX = (x < 0);

        //if (x < 0)
        //{
        //    transform.rotation = Quaternion.Euler(0, 0, 0);
        //}
        //else
        //{
        //    transform.rotation = Quaternion.Euler(0, 180f, 0);
        //}

        if (SpriteRenderer_Enemy != null)
        {
            // x가 0보다 작으면(왼쪽) flipX를 true로, 아니면 false로
            SpriteRenderer_Enemy.flipX = (x > 0);
        }
    }

    void HandleMovementOnUpdate()
    {
        if (_currentState == EnemySlime2DState.Idle) return;

        transform.position += _moveDirection * moveSpeed * Time.deltaTime;
    }

    private void ChangeEnemyState(SlimeEntityAnimState newState)
    {
        if (AnimatorController_Entity != null)
        {
            AnimatorController_Entity.SetState(newState);
        }
    }

}
