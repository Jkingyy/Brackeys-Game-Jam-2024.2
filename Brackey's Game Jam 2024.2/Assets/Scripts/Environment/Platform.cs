using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Callbacks;

public class Platform : MonoBehaviour
{
    public bool isRaining = false;
    [Header("Growing Settings")]
    [SerializeField] Vector2 growthSize;
    [SerializeField] float growthSteps; // amount grow calls in the coroutine
    [SerializeField] float growthInterval; // wait time between growth calls in coroutine
    [SerializeField] bool CanGrow;

    Vector2 _originalSize;
    Vector2 _currentSize;

    private bool _isGrown;
    private bool _isChangingSize;







    IEnumerator _coroutine;


    SpriteRenderer _sr;
    BoxCollider2D _col;
    private void Awake()
    {

        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _originalSize = _sr.size;
        _currentSize = _originalSize;

        _startingPosition = RoundVectorValuesToInt(transform.position);
        transform.position = _startingPosition;
        ListOfPositions.Insert(0, _startingPosition);
        _currentTarget = ListOfPositions[_listIndex];
    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        HandleGrowth();
        HandleMoving();
    }

    #region Growth

    void HandleGrowth()
    {
        if (!CanGrow) return;
        if (_isChangingSize) return;

        if (isRaining && _currentSize == _originalSize)
        { //if its raining and Platform hasnt grown

            Vector2 change = (growthSize - _currentSize) / growthSteps;
            _isChangingSize = true;
            _coroutine = ChangeSize(change, true);
            StartCoroutine(_coroutine);

        }
        else if (!isRaining && _currentSize == growthSize)
        { //if its not raining and the platform hasnt shrunk
            Vector2 change = (_originalSize - _currentSize) / growthSteps;
            _isChangingSize = true;
            _coroutine = ChangeSize(change, false);
            StartCoroutine(_coroutine);
        }
    }

    IEnumerator ChangeSize(Vector2 change, bool isGrowing)
    {

        for (int i = 0; i < growthSteps; i++)
        {
            _sr.size += change;
            _col.size += change;

            yield return new WaitForSeconds(growthInterval);
        }

        if (isGrowing)
        {
            _currentSize = growthSize;
            _isGrown = true;
            _col.enabled = true;

        }
        else
        {
            _currentSize = _originalSize;
            _isGrown = false;
            _col.enabled = false;
        }

        _sr.size = _currentSize;
        _col.size = _currentSize;

        _isChangingSize = false;
    }
    #endregion
    #region Movement

    [Header("Movement Settings")]

    [SerializeField] float SmoothDampTime;
    [SerializeField] List<Vector2> ListOfPositions;
    [SerializeField] bool CanMoveBeforeGrowing;
    [SerializeField] bool CanMoveAfterGrowing;
    [SerializeField] float TargetDistance; // how close platform has to get to target before moving onto the next in the list
    [SerializeField] float WaitTimeSeconds; // how long the platform waits at its position before moving again

    Vector2 _startingPosition;
    Vector2 _currentTarget;
    Vector2 velocity = Vector2.zero;
    bool _isIncreasing = true;
    int _listIndex = 0;
    bool _isWaiting;
    bool _isMoving;

    void HandleMoving()
    {   
        if (!_isMoving){
            if (!CanMoveAfterGrowing && _isGrown) return; //cant move after growing and is grown
            if (!CanMoveBeforeGrowing && !_isGrown) return; // cant move before growing and isnt grown;
            if (_isWaiting) return;
        }

        if (Vector2.Distance(transform.position, _currentTarget) > TargetDistance)
        { //platform is not close enough to target position - keep moving
            transform.position = Vector2.SmoothDamp(transform.position, _currentTarget, ref velocity, SmoothDampTime);
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
            _coroutine = WaitAtPosition();
            StartCoroutine(_coroutine);
            if(_listIndex == ListOfPositions.Count - 1) {
                _isIncreasing = false;
            } else if(_listIndex == 0) {
                _isIncreasing = true;
            }

            if (_isIncreasing)
            {
                _listIndex++;
            }
            else
            {
                _listIndex--;
                
            }

            _currentTarget = ListOfPositions[_listIndex];

        }
    }
    Vector2 RoundVectorValuesToInt(Vector2 value){
        return new Vector2(BetterRoundToInt(value.x),BetterRoundToInt(value.y));
    }
    float BetterRoundToInt(float value){
        return (float)System.Math.Round(value, System.MidpointRounding.AwayFromZero);   
    }
    public void AddPositionToList()
    {
        ListOfPositions.Add(RoundVectorValuesToInt(transform.position));
    }

    IEnumerator WaitAtPosition()
    {
        _isWaiting = true;
        yield return new WaitForSeconds(WaitTimeSeconds);
        _isWaiting = false;
    }
    #endregion

    #region Collisions
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            other.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if(other.gameObject.tag == "Player") {
            other.transform.SetParent(null);
        }
    }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(Platform))]
public class PlatformScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Platform platformScript = (Platform)target;

        GUIContent buttonTextWithTooltip = new GUIContent("Add Position", "Adds current position to the list (Rounded to the nearest integer)");

        if (GUILayout.Button(buttonTextWithTooltip))
        {
            platformScript.AddPositionToList();
        }
    }
}
#endif
