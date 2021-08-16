using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


// Simple script that works in tandem with the LaserPointerWrapper to convert Pointer actions to regular Unity Interaction Events
public class PointerInteractable : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent OnClick, PointerOut, PointerIn;

   public void OnPointerClick(PointerEventData eventData)
   {
       OnClick.Invoke();
   }

   public void OnPointerOut(PointerEventData eventData)
   {
       PointerOut.Invoke();
   }

   public void OnPointerIn(PointerEventData eventData)
   {
       PointerIn.Invoke();
   }
}
