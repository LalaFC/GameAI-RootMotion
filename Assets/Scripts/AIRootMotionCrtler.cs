using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIRootMotionCrtler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float maxTarget;
    [SerializeField] private bool isOffNavmesh;

    private void OnValidate()
    {
      if(!agent) agent = GetComponent<NavMeshAgent>();
      if(!animator) animator = GetComponent<Animator>();
    }
    private void Update()
    {
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
                var dir = (agent.steeringTarget - transform.position).normalized;
                var aniDir = transform.InverseTransformDirection(dir);
                var isFacingMoveDirection = Vector2.Dot(dir, transform.forward) > .5f;

                animator.SetFloat("Horizontal", isFacingMoveDirection ? aniDir.x : 0, 0.5f, Time.deltaTime);
                animator.SetFloat("Vertical", isFacingMoveDirection ? aniDir.z : 0, 0.5f, Time.deltaTime);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(dir), maxTarget * Time.deltaTime);

                if (Vector3.Distance(transform.position, agent.destination) < agent.radius)
                {
                    agent.ResetPath();
                }
            }

        }
        else
        {
            animator.SetFloat("Horizontal", 0, 0.25f, Time.deltaTime);
            animator.SetFloat("Vertical", 0, 0.25f, Time.deltaTime);
        }

        if ((Input.GetMouseButtonDown(0)))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var isHit = Physics.Raycast(ray, out RaycastHit hit, maxTarget);
            if (isHit)
            {
                agent.destination = hit.point; 
            }
        }
    }
    IEnumerator DoOffMeshLink(OffMeshLinkData link)
    {
        animator.CrossFade("Jump", 0.2f);
        var time = 0.7f;
        var totalTime = time;

        while (time > 0)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            transform.position = Vector3.Lerp(link.startPos, link.endPos, 1-time / totalTime);
            yield return new WaitForSeconds(0);
        }
        agent.CompleteOffMeshLink();
    }

    private void OnDrawGizmos()
    {
        if (agent.hasPath) 
        {

            for (int i = 0; i<agent.path.corners.Length-1; i++)
            {
                Debug.DrawLine(agent.path.corners[1], agent.path.corners[i+1], Color.red);
            }
        }
    }
}
