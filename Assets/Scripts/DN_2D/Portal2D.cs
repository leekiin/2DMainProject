using UnityEngine;

public class Portal2D : MonoBehaviour
{
    [Header("포탈 목적지")]
    [SerializeField] private Transform portalDestination;

    [HideInInspector]
    public bool isCoolingDown = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && isCoolingDown == false)
        {
            if(portalDestination != null)
            {
                Portal2D destPortal = portalDestination.GetComponent<Portal2D>();

                if(destPortal != null)
                {
                    destPortal.isCoolingDown = true;
                }
                collision.transform.position = portalDestination.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isCoolingDown = false;
        }
    }
}
