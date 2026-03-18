using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public GameObject hotbarObj;
    public GameObject inventorySlotParent;
    public GameObject container;

    public Image dragIcon;

    public float pickupRange = 3f;
    private Item lookedAtItem = null;
    public Material highlightMaterial;
    private Material originalMaterial;
    private Renderer lookedAtRenderer = null;

    private int equippedHotbarIndex = 0; //0-3 for 4 hotbar slots
    public float equippedOpacity = 0.9f;
    public float normalOpacity = 0.58f;
    public Transform hand;
    private GameObject currentHandItem;

    public GameObject itemDescriptionParent;
    public Image itemDescriptionImage;
    public TextMeshProUGUI descriptionItemNameTxt;
    public TextMeshProUGUI itemDescriptionTxt;

    private List<Slot> inventorySlots = new List<Slot>();
    private List<Slot> hotbarSlots = new List<Slot>();
    private List<Slot> allSlots = new List<Slot>();

    private Slot draggedSlot;
    private bool isDragging = false;

    private void Awake()
    {
        inventorySlots.AddRange(inventorySlotParent.GetComponentsInChildren<Slot>());
        hotbarSlots.AddRange(hotbarObj.GetComponentsInChildren<Slot>());

        allSlots.AddRange(inventorySlots);
        allSlots.AddRange(hotbarSlots);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            container.SetActive(!container.activeInHierarchy);
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            PlayerCam.instance.updatingRotation = !PlayerCam.instance.updatingRotation;
        }

        DetectLookedAtItem();
        Pickup();

        StartDrag();
        UpdateDragItemPosition();
        EndDrag();

        HandleHotBarSelection();
        HandleDropEquippedItem();
        UpdateHotbarOpacity();

        UpdateItemDescription();
    }

    public void AddItem(ItemsS0 itemToAdd, int amount)
    {
        int remaining = amount;

        foreach(Slot slot in allSlots)
        {
            if(slot.HasItem() && slot.GetItem() == itemToAdd)
            {
                int currentAmount = slot.GetAmount();
                int maxStack = itemToAdd.maxStackSize;

                if(currentAmount < maxStack)
                {
                    int spaceLeft = maxStack - currentAmount;
                    int amountToAdd = Mathf.Min(spaceLeft, remaining);

                    slot.SetItem(itemToAdd, currentAmount + amountToAdd);
                    remaining -= amountToAdd;

                    if(remaining <= 0)
                        return;
                }
            }
        }


        foreach(Slot slot in allSlots)
        {
            if(!slot.HasItem())
            {
                int amountToPlace = Mathf.Min(itemToAdd.maxStackSize, remaining);
                slot.SetItem(itemToAdd, amountToPlace);
                remaining -= amountToPlace;

                if(remaining <= 0)
                    return;
            }
        }

        if(remaining > 0)
        {
            Debug.Log("Inventory is full, could notadd all items. " + remaining + " of " + itemToAdd.itemName);
        }
    }
    
    private void StartDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Slot hovered = GetHoveredSlot();

            if(hovered !=null && hovered.HasItem())
            {
                draggedSlot = hovered;
                isDragging = true;

                //show drag icon
                dragIcon.sprite = hovered.GetItem().icon;
                dragIcon.color = new Color(1, 1, 1, 0.5f);
                dragIcon.enabled = true;
            }
        }
    }

    private void EndDrag()
    {
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Slot hovered = GetHoveredSlot();

            if(hovered != null)
            {
                HandleDrop(draggedSlot, hovered);

                dragIcon.enabled = false;

                draggedSlot = null;
                isDragging = false;
            }
        }
    }

    private Slot GetHoveredSlot()
    {
        foreach (Slot s in allSlots)
        {
            if (s.hovering)
            return s;
        }

        return null;
    }

    private void HandleDrop(Slot from, Slot to)
    {
        if (from == to) return;


        //Stacking
        if(to.HasItem() && to.GetItem() == from.GetItem())
        {
            int max = to.GetItem().maxStackSize;
            int space = max - to.GetAmount();

            if(space > 0)
            {
                int move = Mathf.Min(space, from.GetAmount());

                to.SetItem(to.GetItem(), to.GetAmount() + move);
                from.SetItem(from.GetItem(), from.GetAmount() - move);

                if(from.GetAmount() <= 0)
                    from.ClearSlot();
                
                return;
            }
        }

        //Different Item
        if (to.HasItem())
        {
            ItemsS0 tempItem = to.GetItem();
            int tempAmount = to.GetAmount();

            to.SetItem(from.GetItem(), from.GetAmount());
            from.SetItem(tempItem, tempAmount);
            return;
        }

        //Empty Slot
        to.SetItem(from.GetItem(), from.GetAmount());
        from.ClearSlot();
    }

    private void UpdateDragItemPosition()
    {
        if(isDragging)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
    }

    private void Pickup()
    {
            if(lookedAtRenderer != null && Input.GetKeyDown(KeyCode.E))
            {
                Item item = lookedAtRenderer.GetComponent<Item>();
                if(item != null)
                {
                    AddItem(item.item, item.amount);
                    Destroy(item.gameObject);
                    EquipHandItem();
                }
            }
    }

    private void DetectLookedAtItem()
    {
        if(lookedAtRenderer != null)
        {
            lookedAtRenderer.material = originalMaterial;
            lookedAtRenderer = null;
            originalMaterial = null;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            Item item = hit.collider.GetComponent<Item>();
            if(item != null)
            {
                Renderer rend = item.GetComponent<Renderer>();
                if(rend != null)
                {
                    originalMaterial = rend.material;
                    rend.material = highlightMaterial;
                    lookedAtRenderer = rend;
                }
            }
        }
    }

    private void UpdateHotbarOpacity()
    {
        for(int i = 0; i < hotbarSlots.Count; i++)
        {
            Image icon = hotbarSlots[i].GetComponent<Image>();
            if(icon != null)
            {
                icon.color = (i == equippedHotbarIndex) ? new Color(1, 1, 1, equippedOpacity) : new Color(1, 1, 1, normalOpacity);
            }
        }
    }

    private void HandleHotBarSelection()
    {
        for(int i = 0; i < 4; i++)
        {
            if(Input.GetKeyDown((i + 1).ToString()))
            {
                equippedHotbarIndex = i;
                UpdateHotbarOpacity();
                EquipHandItem();
            }
        }
    }

    private void HandleDropEquippedItem()
    {
        if(!Input.GetKeyDown(KeyCode.Q)) return;

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];

        if(!equippedSlot.HasItem()) return;

        ItemsS0 itemsS0 = equippedSlot.GetItem();
        GameObject prefab = itemsS0.itemPrefab;

        if(prefab == null) return;

        GameObject dropped = Instantiate(prefab, Camera.main.transform.position + Camera.main.transform.forward, Quaternion.identity);

        Item item = dropped.GetComponent<Item>();
        item.item = itemsS0;
        item.amount = equippedSlot.GetAmount();

        equippedSlot.ClearSlot();

        EquipHandItem();
    }

    private void EquipHandItem()
    {
        if(currentHandItem != null) Destroy(currentHandItem);

        Slot equippedSlot = hotbarSlots[equippedHotbarIndex];
        if(!equippedSlot.HasItem()) return;

        ItemsS0 item = equippedSlot.GetItem();
        if (item.handItemPrefab == null) return;

        currentHandItem = Instantiate(item.handItemPrefab, hand);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;
    }

    private void UpdateItemDescription()
    {
        Slot hoveredSlot = GetHoveredSlot();

        if(hoveredSlot != null)
        {
            ItemsS0 hoveredItem = hoveredSlot.GetItem();

            if(hoveredItem != null)
            {
                itemDescriptionParent.SetActive(true);
                itemDescriptionImage.sprite = hoveredItem.icon;
                itemDescriptionTxt.text = hoveredItem.description;
                descriptionItemNameTxt.text = hoveredItem.name;
                return;
            }
        }
        itemDescriptionParent.SetActive(false);
    }
}
