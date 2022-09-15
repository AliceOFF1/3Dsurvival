using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickslotInventory : MonoBehaviour
{
    public Transform quickslotParent;
    public InventoryManager inventoryManager;
    public int currentQuickslotID = 0;
    public Sprite selectedSprite;
    public Sprite notSelectedSprite;
    public Transform itemContainer;
    public InventorySlot activeSlot = null;
    public Transform allWeapons;
    public Indicators indicators;

    // Update is called once per frame
    void Update()
    {
        float mw = Input.GetAxis("Mouse ScrollWheel");
        // Using the mouse wheel

        if (mw > 0.1)
        {
            // take the previous slot and change its picture to the usual one

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
            // Here we add what happens when we REMOVE SELECTION from the slot (Turn off the item we need, change the animator ...)

            // If we turn the mouse wheel forward and our currentQuickslotID number is equal to the last slot, then we select our first slot (the first slot is considered zero)
            if (currentQuickslotID >= quickslotParent.childCount - 1)
            {
                currentQuickslotID = 0;
            }
            else
            {
                // add one to the number of currentQuickslotID
                currentQuickslotID++;
            }
            // take the previous slot and change its picture to the "selected" one
            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
            activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            ShowItemInHand();
            // add what happens when we SELECT the slot (Enable the item we need, change the animator ...)


        }
        if (mw < -0.1)
        {
            // take the previous slot and change its picture to the usual one

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
            // add what happens when we REMOVE SELECTION from the slot (Turn off the item we need, change the animator ...)


            // If we turn the mouse wheel back and our currentQuickslotID number is 0, then we select our last slot
            if (currentQuickslotID <= 0)
            {
                currentQuickslotID = quickslotParent.childCount - 1;
            }
            else
            {
                // Decrement the number of currentQuickslotID by 1
                currentQuickslotID--;
            }
            // We take the previous slot and change its picture to the "selected" one

            quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
            activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
            ShowItemInHand();
            // add what happens when we SELECT the slot (Enable the item we need, change the animator ...)

        }
        // Using numbers

        for (int i = 0; i < quickslotParent.childCount; i++)
        {
            // if we press keys 1 to 5 then...
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                // check if our selected slot is equal to the slot that we already have selected, then
                if (currentQuickslotID == i)
                {
                    // put the picture "selected" on the slot if it is "not selected" or vice versa
                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == notSelectedSprite)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                        activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                        ShowItemInHand();
                        //foreach ...
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;
                        activeSlot = null;
                        HideItemsInHand();
                        //foreach ...

                    }
                }
                // Otherwise, we remove the glow from the previous slot and shine the slot that we select
                else
                {
                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = notSelectedSprite;

                    currentQuickslotID = i;

                    quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite = selectedSprite;
                    activeSlot = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>();
                    ShowItemInHand();
                }
            }
        }
        // Use the item by pressing the left mouse button
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item != null)
            {
                if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.isConsumeable && !inventoryManager.isOpened && quickslotParent.GetChild(currentQuickslotID).GetComponent<Image>().sprite == selectedSprite)
                {
                    // Apply changes to health, hunger, thirst 
                    ChangeCharacteristics();

                    if (quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount <= 1)
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponentInChildren<DragAndDropItem>().NullifySlotData();
                    }
                    else
                    {
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount--;
                        quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().itemAmountText.text = quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().amount.ToString();
                    }
                }
            }
        }
    }
    public void CheckItemInHand()
    {
        if (activeSlot != null)
        {
            ShowItemInHand();
        }
        else
        {
            HideItemsInHand();
        }
    }

    private void ChangeCharacteristics()
    {
        indicators.ChangeFoodAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeHunger);
        indicators.ChangeWaterAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeThirst);
        indicators.ChangeHealthAmount(quickslotParent.GetChild(currentQuickslotID).GetComponent<InventorySlot>().item.changeHealth);
    }

    private void ShowItemInHand()
    {
        HideItemsInHand();
        if (activeSlot.item == null)
        {
            return;
        }
        for (int i = 0; i < allWeapons.childCount; i++)
        {
            if (activeSlot.item.inHandName == allWeapons.GetChild(i).name)
            {
                allWeapons.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    private void HideItemsInHand()
    {
        for (int i = 0; i < allWeapons.childCount; i++)
        {
            allWeapons.GetChild(i).gameObject.SetActive(false);
        }
    }
}

