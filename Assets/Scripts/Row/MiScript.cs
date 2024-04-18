using UnityEngine;
using UnityEngine.EventSystems;

public class MiScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // Aquí puedes escribir el código que deseas que se ejecute cuando se haga clic en el GameObject
        Debug.Log("Se hizo clic en el GameObject: " + gameObject.name);
    }
}
