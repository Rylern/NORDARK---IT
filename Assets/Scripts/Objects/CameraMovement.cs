using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float lookSpeedH = 5f;
    [SerializeField] private float lookSpeedV = 5f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float dragSpeed = 5f;
    [SerializeField] private float dragSpeedY = 5f;
    private float yaw;
    private float pitch;

    void Start()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        KeyboardMovement();
        MouseRotation();
    }

    private void KeyboardMovement()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
        transform.Translate(new Vector3(0, 0, -this.dragSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(new Vector3(0, 0, this.dragSpeed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.Q)) {
            transform.Translate(new Vector3(0, -this.dragSpeedY * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.E)) {
            transform.Translate(new Vector3(0, this.dragSpeedY * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            transform.Translate(new Vector3(this.dragSpeed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            transform.Translate(new Vector3(-this.dragSpeed * Time.deltaTime, 0, 0));
        }
    }

    private void MouseRotation()
    {
        if (Input.GetMouseButton(1)) {
            yaw += lookSpeedH * Input.GetAxis("Mouse X");
            pitch -= lookSpeedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }

        if (Input.GetMouseButton(2)) {
            transform.Translate(
                -Input.GetAxisRaw("Mouse X") * Time.deltaTime * dragSpeed,
                -Input.GetAxisRaw("Mouse Y") * Time.deltaTime * dragSpeed,
                0
            );
        }

        transform.Translate(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, Space.Self);
    }
}
