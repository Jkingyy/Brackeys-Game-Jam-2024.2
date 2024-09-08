using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Helpers
{
    
    private static Camera _camera;
    public static Camera Camera
    {
        get {
            if (_camera == null) _camera = Camera.main;
            return _camera;
        }
    }

    public static Vector3 GetMousePosition()
    {
        return Camera.ScreenToWorldPoint(Input.mousePosition);
    }
    
    
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
    public static WaitForSeconds GetWait(float seconds)
    {
        if (!WaitDictionary.ContainsKey(seconds)) WaitDictionary.Add(seconds, new WaitForSeconds(seconds));
        
        return WaitDictionary[seconds];
    }
    
    
    private static PointerEventData _eventDataCurrentPosition;
    private static List<RaycastResult> _results;
    public static bool IsOverUI()
    {
        _eventDataCurrentPosition = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
        _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
        return _results.Count > 0;
    }

    public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(element, element.position, Camera, out var result);
        return result;
    }

    public static void DeleteChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
}
