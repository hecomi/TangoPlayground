using System.Collections.Generic;
using UnityEngine;

public class FocusObject : MonoBehaviour 
{
    const float FOCUS_APPEAR_TIMER = 0.5f;
    const float FOCUS_DISAPPEAR_TIMER = 0.5f;
    const float MOVE_DISTANCE_BY_BUTTON = 0.01f;

    static HashSet<FocusObject> instances_ = new HashSet<FocusObject>();
    public static HashSet<FocusObject> instances
    {
        get { return instances_; }
    }

    enum State
    {
        Hidden,
        Visible,
    }
    State state_ = State.Hidden;

    [SerializeField]
    GameObject ui;

    Animator animator_;
    float focusTimer_ = 0f;

    public float timestamp { get; set; }
    public Matrix4x4 deviceLocalTRS { get; set; }

    bool selected_ = false;
    public bool selected
    {
        get { return selected_; }
    }

    void Awake()
    {
        instances_.Add(this);
    }

    void OnDestroy()
    {
        instances_.Remove(this);
    }

    void Start()
    {
        animator_ = GetComponent<Animator>();
    }

    void Update()
    {
        switch (state_) {
            case State.Hidden  : UpdateHidden(); break;
            case State.Visible : UpdateVisible(); break;
        }
    }

    void UpdateHidden()
    {
        if (IsInSight()) {
            focusTimer_ += Time.deltaTime;
        } else {
            focusTimer_ = 0f;
        }

        if (focusTimer_ > FOCUS_APPEAR_TIMER) {
            SetState(State.Visible);
        }
    }

    void UpdateVisible()
    {
        if (!IsInSight()) {
            focusTimer_ += Time.deltaTime;
        } else {
            focusTimer_ = 0f;
        }

        if (focusTimer_ > FOCUS_DISAPPEAR_TIMER) {
            SetState(State.Hidden);
        }
    }

    bool IsInSight()
    {
        var camera = Camera.main.transform;
        var to = transform.position - camera.position;
        var dir = to.normalized;
        var fwd = camera.forward;

        return Vector3.Angle(dir, fwd) < 30f;
    }

    void SetState(State state)
    {
        if (state_ == state) return;
        state_ = state;

        switch (state_) {
            case State.Hidden  : animator_.SetBool("appear", false); break;
            case State.Visible : animator_.SetBool("appear", true);  break;
        }

        focusTimer_ = 0f;
    }

    public void Select()
    {
        animator_.SetTrigger("select");
        selected_ = true;
        ui.SetActive(true);
    }

    public void Close()
    {
        ui.SetActive(false);
        selected_ = false;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    public void Move(Vector3 dir)
    {
        transform.position += dir * MOVE_DISTANCE_BY_BUTTON;
    }

    public void MoveLeft()
    {
        Move(-Camera.main.transform.right);
    }

    public void MoveRight()
    {
        Move(Camera.main.transform.right);
    }

    public void MoveUp()
    {
        Move(Camera.main.transform.up);
    }

    public void MoveDown()
    {
        Move(-Camera.main.transform.up);
    }

    public void MoveForward()
    {
        Move(Camera.main.transform.forward);
    }

    public void MoveBack()
    {
        Move(-Camera.main.transform.forward);
    }
}