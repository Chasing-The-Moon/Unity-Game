using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pivot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool upOrDown; // Arriba es true y abajo es false
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;
    [SerializeField] private float offsetY;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("PIVOT CLICKED!");
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            float distanceCharacter;
            MainCharacterFSM mc = null;
            foreach (var mc_aux in FindObjectsOfType<MainCharacterFSM>())
            {
                mc = mc_aux.GetCharacterUpOrDown() == upOrDown ? mc_aux : mc;
            }
            distanceCharacter = transform.position.x - mc.transform.position.x;
            Debug.Log(distanceCharacter);
            if(!mc.ThrowArm.GetInTransition() && Mathf.Abs(distanceCharacter) < maxDistance && Mathf.Abs(distanceCharacter) > minDistance && mc.onControl)
            {
                mc.ThrowArm.SetStartParabola(mc.transform.position);
                mc.ThrowArm.SetEndParabola(new Vector2(transform.position.x + distanceCharacter, mc.transform.position.y));
                mc.ThrowArm.SetHeightParabola(transform.position.y - mc.transform.position.y + offsetY);
                mc.GetMovementState().SendEvent("ToThrowArm");
            }
        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 minPosLeft = new Vector3(transform.position.x - minDistance, transform.position.y - 1f, 0);
        Vector3 maxPosLeft = new Vector3(transform.position.x - maxDistance, transform.position.y - 1f, 0);
        Vector3 minPosRight = new Vector3(transform.position.x + minDistance, transform.position.y - 1f, 0);
        Vector3 maxPosRight = new Vector3(transform.position.x + maxDistance, transform.position.y - 1f, 0);

        Vector3 start1LineLeft = new Vector3(transform.position.x - minDistance, transform.position.y - 1f + 0.3f, 0);
        Vector3 end1LineLeft = new Vector3(transform.position.x - minDistance, transform.position.y - 1f - 0.3f, 0);
        Vector3 start2LineLeft = new Vector3(transform.position.x - maxDistance, transform.position.y - 1f + 0.3f, 0);
        Vector3 end2LineLeft = new Vector3(transform.position.x - maxDistance, transform.position.y - 1f - 0.3f, 0);
        Vector3 start1LineRight = new Vector3(transform.position.x + minDistance, transform.position.y - 1f + 0.3f, 0);
        Vector3 end1LineRight = new Vector3(transform.position.x + minDistance, transform.position.y - 1f - 0.3f, 0);
        Vector3 start2LineRight = new Vector3(transform.position.x + maxDistance, transform.position.y -1f + 0.3f, 0);
        Vector3 end2LineRight = new Vector3(transform.position.x + maxDistance, transform.position.y - 1f - 0.3f, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(minPosLeft, maxPosLeft);
        Gizmos.DrawLine(start1LineLeft, end1LineLeft);
        Gizmos.DrawLine(start2LineLeft, end2LineLeft);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(minPosRight, maxPosRight);
        Gizmos.DrawLine(start1LineRight, end1LineRight);
        Gizmos.DrawLine(start2LineRight, end2LineRight);
    }
#endif
}
