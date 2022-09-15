using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rig;
    public Transform mainCamera;
    public float jumpForce = 3.5f;
    public float walkingSpeed = 2f;
    public float runningSpeed = 6f;
    public float currentSpeed;
    private float animationInterpolation = 1f;
    public InventoryManager inventoryManager;
    public QuickslotInventory quickslotInventory;
    public CraftManager craftManager;
    public Indicators indicators;

    public Transform aimTarget;
    public Transform hitTarget;
    public Vector3 hitTargetOffset;
    // Start is called before the first frame update
    void Start()
    {
        // Move the cursor to the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // make it invisible
        Cursor.visible = false;

    }
    void Run()
    {
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1.5f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, runningSpeed, Time.deltaTime * 3);
    }
    void Walk()
    {
        // Mathf.Lerp - is responsible for ensuring that each frame the animationInterpolation number (in this case) approaches the number 1 at the rate of Time.deltaTime * 3.
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, walkingSpeed, Time.deltaTime * 3);
    }

    public void ChangeLayerWeight(float newLayerWeight)
    {
        StartCoroutine(SmoothLayerWeightChange(anim.GetLayerWeight(1), newLayerWeight, 0.3f));
    }

    IEnumerator SmoothLayerWeightChange(float oldWeight, float newWeight, float changeDuration)
    {
        float elapsed = 0f;
        while (elapsed < changeDuration)
        {
            float currentWeight = Mathf.Lerp(oldWeight, newWeight, elapsed / changeDuration);
            anim.SetLayerWeight(1, currentWeight);
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetLayerWeight(1, newWeight);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (quickslotInventory.activeSlot != null)
            {
                if (quickslotInventory.activeSlot.item != null)
                {
                    if (quickslotInventory.activeSlot.item.itemType == ItemType.Instrument)
                    {
                        if (inventoryManager.isOpened == false && craftManager.isOpened == false)
                        {
                            anim.SetBool("Hit", true);
                        }
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("Hit", false);
        }

        // Set the rotation of the character when the camera rotates
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, mainCamera.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);


        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                Walk();
            }
            else
            {
                Run();
            }
        }
        else
        {
            Walk();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Jump");
        }

        Ray desiredTargetRay = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Vector3 desiredTargetPosition = desiredTargetRay.origin + desiredTargetRay.direction * 1.5f; // changed from 0.7 to 1.5
        aimTarget.position = desiredTargetPosition;
        //hitTarget.position = new Vector3(desiredTargetPosition.x + hitTargetOffset.x, desiredTargetPosition.y + hitTargetOffset.y, desiredTargetPosition.z + hitTargetOffset.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // set the movement of the character depending on the direction in which the camera is looking.
        // Keeping the direction forward and to the right of the camera
        Vector3 camF = mainCamera.forward;
        Vector3 camR = mainCamera.right;
        // So that the forward and right directions do not depend on whether the camera is looking up or down, otherwise when we look forward, the character will go faster than when looking up or down
        camF.y = 0;
        camR.y = 0;
        Vector3 movingVector;
        // multiply our pressing of the W & S buttons by the direction of the camera forward and add to the pressing of the A & D buttons and multiply by the direction of the camera to the right
        movingVector = Vector3.ClampMagnitude(camF.normalized * Input.GetAxis("Vertical") * currentSpeed + camR.normalized * Input.GetAxis("Horizontal") * currentSpeed, currentSpeed);
        anim.SetFloat("magnitude", movingVector.magnitude / currentSpeed);
        rig.velocity = new Vector3(movingVector.x, rig.velocity.y, movingVector.z);
        rig.angularVelocity = Vector3.zero;
    }
    public void Jump()
    {
        rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void Hit()
    {
        foreach (Transform item in quickslotInventory.allWeapons)
        {
            if (item.gameObject.activeSelf)
            {
                item.GetComponent<GatherResources>().GatherResource();
                craftManager.currentCraftItem.FillItemDetails();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            indicators.isInWater = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            indicators.isInWater = false;
        }
    }
}