using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
/// IPointerDownHandler - Control mouse clicks on the object on which this script hangs
/// IPointerUpHandler -  Control mouse up on the object on which this script hangs
/// IDragHandler - Controll track of whether we are moving the mouse over the object
public class DragAndDropItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public InventorySlot oldSlot;
    private Transform player;
    private QuickslotInventory quickslotInventory; // added this++
    private CraftManager craftManager;
    public Transform _savingEnvironment;

    private void Start()
    {
        quickslotInventory = FindObjectOfType<QuickslotInventory>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        oldSlot = transform.GetComponentInParent<InventorySlot>();

        craftManager = FindObjectOfType<CraftManager>();
    }
    public void OnDrag(PointerEventData eventData)
    {
        // If the slot is empty, then we don't do what's below return
        if (oldSlot.isEmpty)
            return;
        GetComponent<RectTransform>().position += new Vector3(eventData.delta.x, eventData.delta.y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        //Making the image more transparent
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0.75f);
        // mouse clicks do not ignore this picture
        GetComponentInChildren<Image>().raycastTarget = false;
        // Make DraggableObject child for InventoryPanel;
        transform.SetParent(transform.parent.parent.parent);
    }
    public void ReturnBackToSlot()
    {
        if (oldSlot.isEmpty)
            return;
        // Making the image opaque again
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
        // Mouse can detect it again
        GetComponentInChildren<Image>().raycastTarget = true;

        //Put the DraggableObject back in its old slot
        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (oldSlot.isEmpty)
            return;
        //  Making the image opaque again
        GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1f);
        // Mouse can detect it again
        GetComponentInChildren<Image>().raycastTarget = true;

        //Put the DraggableObject back in its old slot
        transform.SetParent(oldSlot.transform);
        transform.position = oldSlot.transform.position;
        //If the mouse is released over an object named UIPanel, then...
        if (eventData.pointerCurrentRaycast.gameObject.name == "UIBG") // renamed to UIBG
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {

            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {

            }
            else
            {
                // Drop objects from inventory - Spawn prefab object in front of character
                GameObject itemObject = Instantiate(oldSlot.item.itemPrefab, player.position + Vector3.up + player.forward, Quaternion.identity);
                itemObject.transform.SetParent(_savingEnvironment);
                // Set the number of objects to what it was in the slot
                itemObject.GetComponent<Item>().amount = oldSlot.amount;
                // remove InventorySlot values
                NullifySlotData();

                craftManager.currentCraftItem.FillItemDetails();

            }
            quickslotInventory.CheckItemInHand();
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent == null)
        {
            return;
        }
        else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>() != null)
        {
            //Moving data from one slot to another
            ExchangeSlotData(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.GetComponent<InventorySlot>());
            quickslotInventory.CheckItemInHand();
        }

    }
    public void NullifySlotData() // made public 
    {
        // remove InventorySlot values
        oldSlot.item = null;
        oldSlot.amount = 0;
        oldSlot.isEmpty = true;
        oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        oldSlot.iconGO.GetComponent<Image>().sprite = null;
        oldSlot.itemAmountText.text = "";
    }
    void ExchangeSlotData(InventorySlot newSlot)
    {
        // Temporarily store newSlot data in separate variables
        ItemScriptableObject item = newSlot.item;
        int amount = newSlot.amount;
        bool isEmpty = newSlot.isEmpty;
        GameObject iconGO = newSlot.iconGO;
        TMP_Text itemAmountText = newSlot.itemAmountText;
        if (item == null)
        {
            if (oldSlot.item.maximumAmount > 1 && oldSlot.amount > 1)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    newSlot.item = oldSlot.item;
                    newSlot.amount = Mathf.CeilToInt((float)oldSlot.amount / 2);
                    newSlot.isEmpty = false;
                    newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                    newSlot.itemAmountText.text = newSlot.amount.ToString();

                    oldSlot.amount = Mathf.FloorToInt((float)oldSlot.amount / 2); ;
                    oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    return;
                }
                else if (Input.GetKey(KeyCode.LeftControl))
                {
                    newSlot.item = oldSlot.item;
                    newSlot.amount = 1;
                    newSlot.isEmpty = false;
                    newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
                    newSlot.itemAmountText.text = newSlot.amount.ToString();

                    oldSlot.amount--;
                    oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    return;
                }
            }
        }
        if (newSlot.item != null)
        {
            if (oldSlot.item.name.Equals(newSlot.item.name))
            {
                if (Input.GetKey(KeyCode.LeftShift) && oldSlot.amount > 1)
                {
                    if (Mathf.CeilToInt((float)oldSlot.amount / 2) < newSlot.item.maximumAmount - newSlot.amount)
                    {
                        newSlot.amount += Mathf.CeilToInt((float)oldSlot.amount / 2);
                        newSlot.itemAmountText.text = newSlot.amount.ToString();

                        oldSlot.amount -= Mathf.CeilToInt((float)oldSlot.amount / 2);
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    else
                    {
                        int difference = newSlot.item.maximumAmount - newSlot.amount;
                        newSlot.amount = newSlot.item.maximumAmount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();

                        oldSlot.amount -= difference;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();

                    }
                    return;
                }
                else if (Input.GetKey(KeyCode.LeftControl) && oldSlot.amount > 1)
                {
                    if (newSlot.item.maximumAmount != newSlot.amount)
                    {
                        newSlot.amount++;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();

                        oldSlot.amount--;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    return;
                }
                else
                {
                    if (newSlot.amount + oldSlot.amount >= newSlot.item.maximumAmount)
                    {
                        int difference = newSlot.item.maximumAmount - newSlot.amount;
                        newSlot.amount = newSlot.item.maximumAmount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();

                        oldSlot.amount -= difference;
                        oldSlot.itemAmountText.text = oldSlot.amount.ToString();
                    }
                    else
                    {
                        newSlot.amount += oldSlot.amount;
                        newSlot.itemAmountText.text = newSlot.amount.ToString();
                        NullifySlotData();
                    }
                    return;
                }

            }
        }

        // Replacing newSlot values ​​with oldSlot values
        newSlot.item = oldSlot.item;
        newSlot.amount = oldSlot.amount;
        if (oldSlot.isEmpty == false)
        {
            newSlot.SetIcon(oldSlot.iconGO.GetComponent<Image>().sprite);
            if (oldSlot.item.maximumAmount != 1) // added this if statement for single items
            {
                newSlot.itemAmountText.text = oldSlot.amount.ToString();
            }
            else
            {
                newSlot.itemAmountText.text = "";
            }
        }
        else
        {
            newSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            newSlot.iconGO.GetComponent<Image>().sprite = null;
            newSlot.itemAmountText.text = "";
        }

        newSlot.isEmpty = oldSlot.isEmpty;

        // Replace oldSlot values ​​with newSlot values ​​stored in variables
        oldSlot.item = item;
        oldSlot.amount = amount;
        if (isEmpty == false)
        {
            oldSlot.SetIcon(item.icon);
            if (item.maximumAmount != 1) // added this if statement for single items
            {
                oldSlot.itemAmountText.text = amount.ToString();
            }
            else
            {
                oldSlot.itemAmountText.text = "";
            }
        }
        else
        {
            oldSlot.iconGO.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            oldSlot.iconGO.GetComponent<Image>().sprite = null;
            oldSlot.itemAmountText.text = "";
        }

        oldSlot.isEmpty = isEmpty;
    }
}