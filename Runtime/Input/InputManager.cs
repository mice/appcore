using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// mgr.Register<UGUIInteractiveLayer>("UGUI");
/// mgr.Register<FGUIInteractiveLayer>("FGUI");
/// mgr.EnableLayer("UGUI", true);
/// mgr.EnableLayer("FGUI", true);
/// </summary>
public interface IInteractiveLayer
{
    Transform GetHitTransform(Vector3 pos);
    void InitTransform(Transform container);
}


public class UGUIInteractiveLayer : IInteractiveLayer
{
    private static List<RaycastResult> results = new List<RaycastResult>(8);
    public PointerEventData eventDataCurrentPosition { get; private set; }
    public static bool IgnoreUI = false;
    public UGUIInteractiveLayer()
    {
        eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    }

    public void InitTransform(Transform container)
    {

    }

    public Transform GetHitTransform(Vector3 pos)
    {
        if (IgnoreUI)
        {
            return null;
        }

        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        results.Clear();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0 ? results[0].gameObject.transform : null;
    }
}

public class InputManager : MonoBehaviourSingleton<InputManager>
{
    public class TouchInfo
    {
        public int id = -1;

        public int unique;

        public Transform hit;

        public int touchCount;

        public Vector2 position;

        public Vector2 beginPosition;

        public List<Vector2> prevPositions;

        public Vector2 move;

        public float moveLength;

        public float moveLengthTotal;

        public float beginTime;

        public float endTime;

        public bool activeAxis;

        public Vector2 axis;

        public Vector2 axisNoLimit;

        public bool calledTap;

        public bool calledFlick;

        public bool calledLongTouch;

        public bool enable
        {
            get
            {
                return this.hit == null && this.id != -1;
            }
        }

        public void Clear()
        {
            this.id = -1;
            this.hit = null;
        }
    }

    public delegate void OnTouchDelegate(InputManager.TouchInfo touch_info);

    public delegate void OnDoubleTouchDelegate(InputManager.TouchInfo touch_info0, InputManager.TouchInfo touch_info1);

    public delegate void OnPinchDelegate(InputManager.TouchInfo touch_info0, InputManager.TouchInfo touch_info1, float pinch_length, float origin_length);

    public delegate void OnStickDelegate(Vector2 stick_vec);

    public delegate void OnFlickDelegate(Vector2 flick_vec);

    public delegate void OnWheelDelegate(float delta);

    private const int PREV_POSITIONS_NUM = 3;

    public static float pxRate = 1f;

    public static InputManager.OnTouchDelegate OnTap;

    public static InputManager.OnTouchDelegate OnDoubleTap;

    public static InputManager.OnStickDelegate OnStick;

    public static InputManager.OnTouchDelegate OnLongTouch;

    public static InputManager.OnFlickDelegate OnFlick;

    public static InputManager.OnTouchDelegate OnTouchOn;

    public static InputManager.OnTouchDelegate OnTouchOff;

    public static InputManager.OnTouchDelegate OnDrag;

    public static InputManager.OnDoubleTouchDelegate OnDoubleDrag;

    public static InputManager.OnPinchDelegate OnPinch;

    public static InputManager.OnTouchDelegate OnTouchOnAlways;

    public static InputManager.OnTouchDelegate OnTouchOffAlways;

    public static InputManager.OnTouchDelegate OnDragAlways;

    public static InputManager.OnWheelDelegate OnWheelScroll;

    public int enableTouchCount = 2;

    public int enableStickCount = 1;

    public float dragThresholdLength = 1f;

    public float stickMaxLength = 80f;

    public float stickThresholdLength = 20f;

    public float flickThresholdLengthHigh = 8f;

    public float flickThresholdLengthLow = 24f;

    public float flickThresholdSpeedHigh = 200f;

    public float flickThresholdSpeedLow = 600f;

    public float flickLimitTime = 0.2f;

    public float longTouchTime = 0.15f;

    public float doubleTapSingleTime = 0.15f;

    public float doubleTapEnableTime = 0.4f;

    private float doubleTapBeginTime;

    private int doubleTapCount;

    private float scaledDragThresholdLength;

    private float scaledStickMaxLength;

    private float scaledStickThresholdLength;

    private float scaledFlickThresholdLength;

    private float scaledFlickThresholdSpeed;

    private InputManager.TouchInfo[] touchInfos;

    private int unique;

    private bool _untouch;

    private bool isMouseDown = false;
    private List<IInteractiveLayer> layers = new List<IInteractiveLayer>();

    public INPUT_DISABLE_FACTOR disableFlags
    {
        get;
        private set;
    }

    protected override void _OnDestroy()
    {
        InputManager.OnTap = null;
        InputManager.OnDoubleTap = null;
        InputManager.OnStick = null;
        InputManager.OnLongTouch = null;
        InputManager.OnFlick = null;
        InputManager.OnTouchOn = null;
        InputManager.OnTouchOff = null;
        InputManager.OnDrag = null;
        InputManager.OnDoubleDrag = null;
        InputManager.OnPinch = null;
        InputManager.OnTouchOnAlways = null;
        InputManager.OnTouchOffAlways = null;
        InputManager.OnDragAlways = null;
    }

    private Dictionary<string, System.Type> typeDict = new Dictionary<string, System.Type>();
    public void Register<T>(string key) where T : IInteractiveLayer
    {
        typeDict[key] = typeof(T);
    }

    public void EnableLayer(string key, bool enabled)
    {
        if (typeDict.TryGetValue(key, out var type))
        {
            if (enabled)
            {
                EnableLayer(type);
            }
            else
            {
                DisableLayer(type);
            }
        }
    }

    private void EnableLayer(System.Type type)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            if (layers[i].GetType() == type)
            {
                return;
            }
        }
        var fGUILayer = System.Activator.CreateInstance(type) as IInteractiveLayer;
        fGUILayer.InitTransform(transform);
        layers.Add(fGUILayer);
    }

    private void DisableLayer(System.Type type)
    {
        for (int i = layers.Count - 1; i >= 0; i--)
        {
            if (layers[i].GetType() == type)
            {
                layers.RemoveAt(i);
                break;
            }
        }
    }

    public bool IsDisable()
    {
        return this.disableFlags != (INPUT_DISABLE_FACTOR)0;
    }

    public void SetDisable(INPUT_DISABLE_FACTOR factor, bool disable)
    {
        if (disable)
        {
            this.disableFlags |= factor;
        }
        else
        {
            this.disableFlags &= ~factor;
        }
    }

    public int GetNativeInputCount() => Input.touchCount;

    public bool IsActiveStick()
    {
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1 && touchInfo.activeAxis && touchInfo.enable)
            {
                return true;
            }
            i++;
        }
        return false;
    }

    public Vector2 GetStickVector()
    {
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1 && touchInfo.activeAxis && touchInfo.enable)
            {
                return touchInfo.axis;
            }
            i++;
        }
        return Vector2.zero;
    }

    public InputManager.TouchInfo GetStickInfo()
    {
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1 && touchInfo.activeAxis && touchInfo.enable)
            {
                return touchInfo;
            }
            i++;
        }
        return null;
    }

    public bool IsTouch()
    {
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1 && touchInfo.enable)
            {
                return true;
            }
            i++;
        }
        return false;
    }

    public int GetActiveInfoCount()
    {
        int num = 0;
        int i = 0;
        int num2 = this.touchInfos.Length;
        while (i < num2)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1)
            {
                num++;
            }
            i++;
        }
        return num;
    }

    public InputManager.TouchInfo GetActiveInfo(bool check_enable = false)
    {
        if (touchInfos == null) return null;
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1)
            {
                if (!check_enable || touchInfo.enable)
                {
                    return touchInfo;
                }
            }
            i++;
        }
        return null;
    }

    private InputManager.TouchInfo GetInfo(int id)
    {
        int i = 0;
        if (touchInfos == null)
        {
            return null;
        }
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id == id)
            {
                return touchInfo;
            }
            i++;
        }
        return null;
    }

    private void Start()
    {
        if (Screen.dpi > 0f)
        {
            InputManager.pxRate = Screen.dpi / 160f;
        }
        this.scaledDragThresholdLength = this.dragThresholdLength * InputManager.pxRate;
        this.scaledStickMaxLength = this.stickMaxLength * InputManager.pxRate;
        this.scaledStickThresholdLength = this.stickThresholdLength * InputManager.pxRate;
        this.UpdateConfigInput();
        this.touchInfos = new InputManager.TouchInfo[this.enableTouchCount];
        for (int i = 0; i < this.enableTouchCount; i++)
        {
            this.touchInfos[i] = new InputManager.TouchInfo();
        }
        this.Untouch();
    }

    public void UpdateConfigInput()
    {
        float num = 0.5f;
        float num2 = this.flickThresholdLengthLow + (this.flickThresholdLengthHigh - this.flickThresholdLengthLow) * num;
        this.scaledFlickThresholdLength = num2 * InputManager.pxRate;
        float num3 = this.flickThresholdSpeedLow + (this.flickThresholdSpeedHigh - this.flickThresholdSpeedLow) * num;
        this.scaledFlickThresholdSpeed = num3 * InputManager.pxRate;
    }

    
    private bool HasTouchBy(int finger, int touchCount)
    {
        for (int i = 0; i < touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == finger)
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        int touchCount = Input.touchCount;

        if (this.disableFlags == (INPUT_DISABLE_FACTOR)0)
        {
            bool needSimulate = false;
#if UNITY_EDITOR
            needSimulate = true;
#endif
            if (needSimulate)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    this.Touch(0, TouchPhase.Began, Input.mousePosition);
                    isMouseDown = true;
                }
                if (isMouseDown)
                {
                    this.Touch(0, TouchPhase.Moved, Input.mousePosition);
                }

                if (isMouseDown && Input.GetMouseButtonUp(0))
                {
                    this.Touch(0, TouchPhase.Ended, Input.mousePosition);
                }
            }
            else
            {
                touchCount = Mathf.Min(touchCount, enableTouchCount);

                for (int i = 0; i < touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    this.Touch(touch.fingerId, touch.phase, touch.position);
                }


                for (int i = 0; i < enableTouchCount; i++)
                {
                    InputManager.TouchInfo touchInfo = this.touchInfos[i];
                    if (touchInfo.id > 0)
                    {
                        if (touchCount == 0 || !HasTouchBy(touchInfo.id, enableTouchCount))
                        {
                            this.Touch(touchInfo.id, TouchPhase.Ended, touchInfo.position);
                        }
                    }
                }
            }
        }
        else
        {
            this.Untouch();
        }

        if (InputManager.OnDrag != null || InputManager.OnDoubleDrag != null || InputManager.OnPinch != null || InputManager.OnDragAlways != null)
        {
            InputManager.TouchInfo touchInfo = null;
            InputManager.TouchInfo touchInfo2 = null;
            int num = 0;
            int j = 0;
            int num2 = this.touchInfos.Length;
            while (j < num2)
            {
                InputManager.TouchInfo touchInfo3 = this.touchInfos[j];
                if (touchInfo3.id != -1)
                {
                    num++;
                    if (touchInfo == null)
                    {
                        touchInfo = touchInfo3;
                    }
                    else if (touchInfo2 == null)
                    {
                        touchInfo2 = touchInfo3;
                    }
                }
                j++;
            }
            if (num == 1)
            {
                if (touchInfo.moveLengthTotal > this.scaledDragThresholdLength && touchInfo.moveLength > 0f)
                {
                    if (touchInfo.enable && InputManager.OnDrag != null)
                    {
                        InputManager.OnDrag(touchInfo);
                    }
                    if (InputManager.OnDragAlways != null)
                    {
                        InputManager.OnDragAlways(touchInfo);
                    }
                }
            }
            else if (num == 2 && (touchInfo.moveLength > 0f || touchInfo2.moveLength > 0f))
            {
                bool flag = false;
                if (InputManager.OnPinch != null && (touchInfo.moveLengthTotal > this.scaledDragThresholdLength || touchInfo2.moveLengthTotal > this.scaledDragThresholdLength) && (touchInfo.move == Vector2.zero || touchInfo2.move == Vector2.zero || Vector2.Angle(touchInfo.move.normalized, touchInfo2.move.normalized) > 90f))
                {
                    float magnitude = (touchInfo.position - touchInfo.move - (touchInfo2.position - touchInfo2.move)).magnitude;
                    float magnitude2 = (touchInfo.position - touchInfo2.position).magnitude;
                    float num3 = magnitude2 - magnitude;
                    if (Mathf.Abs(num3) > this.scaledDragThresholdLength)
                    {
                        this.SetEnableUIInput(false);
                        InputManager.OnPinch(touchInfo, touchInfo2, num3, magnitude2);
                        flag = true;
                    }
                }
                if (InputManager.OnDoubleDrag != null && !flag && touchInfo.moveLengthTotal > this.scaledDragThresholdLength && touchInfo2.moveLengthTotal > this.scaledDragThresholdLength)
                {
                    this.SetEnableUIInput(false);
                    InputManager.OnDoubleDrag(touchInfo, touchInfo2);
                }
            }
        }

        if (this.disableFlags == (INPUT_DISABLE_FACTOR)0 && OnWheelScroll != null)
        {
            var delta = Input.GetAxis("Mouse ScrollWheel");
            if (delta == 0) return;
            InputManager.OnWheelScroll(delta);
        }
    }

    private void Touch(int id, TouchPhase phase, Vector2 pos)
    {
        switch (phase)
        {
            case TouchPhase.Began:
                {
                    if (this.GetInfo(id) != null)
                    {
                        return;
                    }
                    InputManager.TouchInfo info = this.GetInfo(-1);
                    if (info != null)
                    {
                        info.id = id;
                        info.unique = ++this.unique;
                        info.hit = this.GetHitTransform(pos);
                        info.touchCount = this.GetActiveInfoCount();
                        info.position = pos;
                        info.beginPosition = pos;
                        if (info.prevPositions == null)
                        {
                            info.prevPositions = new List<Vector2>();
                        }
                        else
                        {
                            info.prevPositions.Clear();
                        }
                        info.move = Vector2.zero;
                        info.moveLength = 0f;
                        info.moveLengthTotal = 0f;
                        info.beginTime = Time.time;
                        info.endTime = 0f;
                        info.activeAxis = false;
                        info.axis = Vector2.zero;
                        info.axisNoLimit = Vector2.zero;
                        info.calledTap = false;
                        info.calledFlick = false;
                        info.calledLongTouch = false;
                        if (info.enable && InputManager.OnTouchOn != null)
                        {
                            InputManager.OnTouchOn(info);
                        }
                        if (InputManager.OnTouchOnAlways != null)
                        {
                            InputManager.OnTouchOnAlways(info);
                        }
                        if (info.touchCount > 1)
                        {
                            this.doubleTapCount = 0;
                        }
                    }
                    break;
                }
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                {
                    InputManager.TouchInfo info2 = this.GetInfo(id);
                    if (info2 != null)
                    {
                        info2.move = pos - info2.position;
                        if (pos != info2.position)
                        {
                            info2.move = pos - info2.position;
                            info2.moveLength = info2.move.magnitude;
                            info2.moveLengthTotal += info2.moveLength;
                        }
                        else
                        {
                            info2.move = Vector3.zero;
                            info2.moveLength = 0f;
                        }
                        info2.position = pos;
                        info2.prevPositions.Insert(0, pos);
                        if (info2.prevPositions.Count > 3)
                        {
                            info2.prevPositions.RemoveRange(3, info2.prevPositions.Count - 3);
                        }
                        Vector2 vector = info2.position - info2.beginPosition;
                        float magnitude = vector.magnitude;
                        if (!info2.activeAxis && magnitude >= this.scaledStickThresholdLength)
                        {
                            info2.activeAxis = true;
                        }
                        vector.Normalize();
                        info2.axis = vector;
                        info2.axisNoLimit = vector;
                        if (magnitude < this.scaledStickMaxLength)
                        {
                            info2.axis *= magnitude / this.scaledStickMaxLength;
                        }
                        info2.axisNoLimit *= magnitude / this.scaledStickMaxLength;
                        if (info2.activeAxis)
                        {
                            if (info2.enable && InputManager.OnStick != null)
                            {
                                InputManager.OnStick(info2.axis);
                            }
                        }
                        else if (this.longTouchTime > 0f && !info2.calledLongTouch && Time.time - info2.beginTime >= this.longTouchTime && info2.enable && InputManager.OnLongTouch != null)
                        {
                            InputManager.OnLongTouch(info2);
                            info2.calledLongTouch = true;
                        }
                    }
                    break;
                }
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                {
                    InputManager.TouchInfo info3 = this.GetInfo(id);
                    if (info3 != null)
                    {
                        info3.position = pos;
                        info3.prevPositions.Insert(0, pos);
                        if (info3.prevPositions.Count > 3)
                        {
                            info3.prevPositions.RemoveRange(3, info3.prevPositions.Count - 3);
                        }
                        info3.endTime = Time.time;
                        float num = info3.endTime - info3.beginTime;
                        if (num <= this.flickLimitTime)
                        {
                            Vector3 vector2 = pos - info3.beginPosition;
                            float magnitude2 = vector2.magnitude;
                            if (magnitude2 >= this.scaledFlickThresholdLength && info3.enable && InputManager.OnFlick != null)
                            {
                                InputManager.OnFlick(vector2.normalized);
                                info3.calledFlick = true;
                            }
                        }
                        else
                        {
                            int index = (info3.prevPositions.Count <= 1) ? 0 : 1;
                            Vector3 vector3 = info3.prevPositions[index] - info3.prevPositions[info3.prevPositions.Count - 1];
                            float num2 = (Time.deltaTime <= 0f) ? 0f : (vector3.magnitude / Time.deltaTime);
                            if (num2 >= this.scaledFlickThresholdSpeed && info3.enable && InputManager.OnFlick != null)
                            {
                                InputManager.OnFlick(vector3.normalized);
                                info3.calledFlick = true;
                            }
                        }
                        if (!info3.activeAxis && !info3.calledFlick && !info3.calledLongTouch && info3.enable && InputManager.OnTap != null)
                        {
                            InputManager.OnTap(info3);
                            info3.calledTap = true;
                        }
                        if (Time.time - info3.beginTime <= this.doubleTapSingleTime && !info3.calledFlick)
                        {
                            if (this.doubleTapCount == 0)
                            {
                                this.doubleTapCount = 1;
                                this.doubleTapBeginTime = info3.beginTime;
                            }
                            else if (this.doubleTapCount == 1)
                            {
                                if (Time.time - this.doubleTapBeginTime <= this.doubleTapEnableTime)
                                {
                                    if (info3.enable && InputManager.OnDoubleTap != null)
                                    {
                                        InputManager.OnDoubleTap(info3);
                                    }
                                }
                                else
                                {
                                    this.doubleTapCount = 1;
                                    this.doubleTapBeginTime = info3.beginTime;
                                }
                            }
                        }
                        else
                        {
                            this.doubleTapCount = 0;
                        }
                        if (info3.enable && InputManager.OnTouchOff != null)
                        {
                            InputManager.OnTouchOff(info3);
                        }
                        if (InputManager.OnTouchOffAlways != null)
                        {
                            InputManager.OnTouchOffAlways(info3);
                        }
                        if (info3.touchCount == 1 && !info3.enable)
                        {
                            info3.id = -1;
                            this.Untouch();
                        }
                        info3.Clear();
                        if (!this.IsEnableUIInput())
                        {
                            int num3 = 0;
                            int i = 0;
                            int num4 = this.touchInfos.Length;
                            while (i < num4)
                            {
                                if (this.touchInfos[i].enable)
                                {
                                    num3++;
                                }
                                i++;
                            }
                            if (num3 <= 1)
                            {
                                this.SetEnableUIInput(true);
                            }
                        }
                    }
                    break;
                }
        }
    }

    private void Untouch()
    {
        if (this.touchInfos == null)
        {
            return;
        }
        if (this._untouch)
        {
            return;
        }
        this._untouch = true;
        int i = 0;
        int num = this.touchInfos.Length;
        while (i < num)
        {
            InputManager.TouchInfo touchInfo = this.touchInfos[i];
            if (touchInfo.id != -1)
            {
                this.Touch(touchInfo.id, TouchPhase.Ended, touchInfo.position);
            }
            touchInfo.Clear();
            i++;
        }
        this.SetEnableUIInput(true);
        this._untouch = false;
    }

    private void SetEnableUIInput(bool is_enable)
    {
    }

    private bool IsEnableUIInput()
    {
        return true;
    }

    protected override void OnAwake()
    {
        eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    }

    public PointerEventData eventDataCurrentPosition { get; private set; }

    private Transform GetHitTransform(Vector3 pos)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            var tmpTfm = layers[i].GetHitTransform(pos);
            if (tmpTfm!=null)
            {
                return tmpTfm;
            }
        }
        return null;
    }


    private static List<RaycastResult> _hitList = new List<RaycastResult>();

    public bool IsPointerOverUI(GameObject ignoreObj = null)
    {
       
        PointerEventData eventData = eventDataCurrentPosition;
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        var count = 0;
        if (EventSystem.current != null)
        {
            EventSystem.current.RaycastAll(eventData, _hitList);
            count = _hitList.Count;
            for (int i = 0; i < _hitList.Count; i++)
            {
                if (_hitList[i].gameObject == ignoreObj)
                {
                    count--;
                }
            }
        }

        _hitList.Clear();
        return count > 0;
        // #endif
    }
}