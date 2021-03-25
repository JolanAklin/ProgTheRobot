using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // from https://pastebin.com/YBbFGZzD, modified by me
    public float mouseSensitivity = 250;
    public float moveSpeed;

    private float verticalLookRotation;
    private float horizontalLookRotation;

    private bool canMove;
    private bool inMove;

    public static CameraController instance;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetMouseButton(2))
        {
            if(canMove || inMove)
            {
                inMove = true;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Vector3 moveDir = new Vector3(-Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"), 0).normalized;
                    Vector3 targetMoveAmount = moveDir * moveSpeed * Time.deltaTime;
                    Vector3 localMove = transform.TransformDirection(targetMoveAmount) * Time.fixedDeltaTime;
                    transform.Translate(transform.InverseTransformDirection(localMove));
                }
                else
                {
                    horizontalLookRotation += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

                    verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                    verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 0);

                    transform.localEulerAngles = new Vector3(-1 * verticalLookRotation, 1 * horizontalLookRotation, 0);
                }
            }
        }else
        {
            inMove = false;
        }
        if(Input.mouseScrollDelta.y != 0 && canMove && !inMove)
        {
            transform.Translate(Vector3.forward * Input.mouseScrollDelta.y * moveSpeed * Time.deltaTime / 4);
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
