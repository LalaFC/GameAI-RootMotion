using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] int dummyCount = 0;
    [SerializeField] UIManager uiManager;
    void Start()
    {
        dummyCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
     if(dummyCount >= 4)
        uiManager.GameFinished();
            
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<AIRootMotionCrtler>() != null)
        {
            dummyCount++;
            if(Vector3.Distance(other.transform.position, this.transform.position)<0.8f )
            other.GetComponent<AIRootMotionCrtler>().enabled = false;
        }
    }
}
