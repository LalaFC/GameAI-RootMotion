using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AIRootMotionCrtler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float maxTarget = 360f;
    [SerializeField] private bool isOffNavmesh;
    [SerializeField] private GameObject target;

    private Vector3 lastPosition;
    private float stuckCheckTimer;
    private float stuckThreshold = 0.1f;
    private float stuckDuration = 1f;

    private void OnValidate()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning($"{name} is off the NavMesh!");
            return;
        }

        if (agent.hasPath)
        {
            if (agent.isOnOffMeshLink)
            {
                isOffNavmesh = true;
                var link = agent.currentOffMeshLinkData;
                StartCoroutine(DoOffMeshLink(link));
            }
            else
            {
                Vector3 dir = (agent.steeringTarget - transform.position).normalized;
                Vector3 aniDir = transform.InverseTransformDirection(dir);
                bool isFacingMoveDirection = Vector3.Dot(dir, transform.forward) > 0.5f;

                animator.SetFloat("Horizontal", isFacingMoveDirection ? aniDir.x : 0, 0.5f, Time.deltaTime);
                animator.SetFloat("Vertical", isFacingMoveDirection ? aniDir.z : 0, 0.5f, Time.deltaTime);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), maxTarget * Time.deltaTime);

                if (agent.remainingDistance <= agent.stoppingDistance + 0.1f && !agent.pathPending)
                {
                    agent.ResetPath();
                }
            }

            HandleStuckDetection();
        }
        else
        {
            animator.SetFloat("Horizontal", 0, 0.25f, Time.deltaTime);
            animator.SetFloat("Vertical", 0, 0.25f, Time.deltaTime);
        }

        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        if ((Input.GetMouseButtonDown(0) && target != null))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxTarget))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && target != null)
        {
            agent.SetDestination(target.transform.position);
        }
    }

    void HandleStuckDetection()
    {
        float movement = Vector3.Distance(transform.position, lastPosition);
        stuckCheckTimer += Time.deltaTime;

        if (movement < stuckThreshold)
        {
            if (stuckCheckTimer >= stuckDuration)
            {
                Debug.LogWarning($"{name} seems to be stuck. Recalculating path...");
                agent.SetDestination(agent.destination); // Refresh path
                stuckCheckTimer = 0;
            }
        }
        else
        {
            stuckCheckTimer = 0;
        }

        lastPosition = transform.position;
    }

    IEnumerator DoOffMeshLink(OffMeshLinkData link)
    {
        var startPos = transform.position;
        var endPos = link.endPos;
        var direction = (endPos - startPos).normalized;

        // Align to direction
        while (Vector3.Dot(direction, transform.forward) < 0.8f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), maxTarget * Time.deltaTime);
            yield return null;
        }

        animator.CrossFade("Jump", 0.2f);
        yield return new WaitForSeconds(0.2f);

        float time = 0.7f;
        float elapsed = 0f;

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        agent.CompleteOffMeshLink();
    }

    public void AcquireTarget(GameObject player)
    {
        target = target != player ? player : target;
    }

    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath && agent.path.corners.Length > 1)
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.blue);
            }
        }
    }
}
