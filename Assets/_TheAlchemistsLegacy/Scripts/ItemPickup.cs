using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("pick up setting")]
    public KeyCode pickupKey = KeyCode.E;  // 拾取物品的按键
    public float pickupRange = 2.0f;       // 拾取距离
    public Transform handPosition;         // 手的位置，物品将被放置在这里
    public LayerMask pickupLayer;          // 可拾取物品的层

    [Header("current situation")]
    public GameObject currentItem;         // 当前手中的物品
    public bool isHoldingItem = false;     // 是否正在持有物品

    private Camera playerCamera;

    [Header("current item name")]
    public string currentItemName = "";  // 当前拿着的物品名字

    public static ItemPickup instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 获取玩家摄像机
        playerCamera = Camera.main;

        // 确保手的位置已经设置
        if (handPosition == null)
        {
            Debug.LogError("未设置手的位置！请在Inspector中指定handPosition");
        }
    }

    void Update()
    {
        // 当按下拾取键时
        if (Input.GetKeyDown(pickupKey))
        {
            if (isHoldingItem)
            {
                // 如果已经拿着物品，则放下
                DropItem();
            }
            else
            {
                // 尝试拾取物品
                TryPickupItem();
            }
        }
    }

    void TryPickupItem()
    {
        RaycastHit hit;

        // 从摄像机中心发射射线
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        // 检测是否有可拾取的物品
        if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
        {
            // 找到了可拾取的物品
            if (hit.collider.CompareTag("Pickup"))
            {
                PickupItem(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("这个物体不能被捡起：" + hit.collider.name);
            }
        }
    }

    void PickupItem(GameObject item)
    {
        // 保存对物品的引用
        currentItem = item;
        isHoldingItem = true;
        currentItemName = item.name; // 保存当前物品的名字

        // 获取物品的Rigidbody（如果有）
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
        if (itemRigidbody != null)
        {
            // 禁用物理效果
            itemRigidbody.isKinematic = true;
        }

        // 禁用物品的碰撞器（可选）
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        // 将物品设置为手的子对象
        item.transform.SetParent(handPosition);

        // 重置物品的本地位置和旋转
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Debug.Log("已拾取物品: " + item.name);
    }

    void DropItem()
    {
        if (currentItem != null)
        {
            // 移除父对象关系
            currentItem.transform.SetParent(null);

            // 重新启用物理效果
            Rigidbody itemRigidbody = currentItem.GetComponent<Rigidbody>();
            if (itemRigidbody != null)
            {
                itemRigidbody.isKinematic = false;

                // 给物品一个轻微的向前抛出力（可选）
                itemRigidbody.AddForce(playerCamera.transform.forward * 2.0f, ForceMode.Impulse);
            }

            // 重新启用碰撞器
            Collider itemCollider = currentItem.GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true;
            }

            Debug.Log("已放下物品: " + currentItem.name);

            // 清除引用
            currentItem = null;
            currentItemName = "";
            isHoldingItem = false;
        }
    }
}