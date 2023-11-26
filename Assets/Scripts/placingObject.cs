using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CameraMovement : MonoBehaviour
{
    private GameObject transparentObject;
    private GameObject[] playerObjects;
    private StageController stageController;
    private Color invalidColor = new(1.0f, 0.0f, 0.0f, 0.05f);
    private Color validColor = new(0.0f, 1.0f, 0.0f, 0.05f);
    private Dictionary<string, GameObject> name2object;
    private float rotationX = 0;
    private float rotationY = 0;
    private float diviateX = 0;
    private float diviateZ = 0;
    public bool isAddingObject = false;
    private const string FOLDERPATH = "Item";

    void Start() {
        stageController = GameObject.FindGameObjectWithTag("GameController").GetComponent<StageController>();
        playerObjects = GameObject.FindGameObjectsWithTag("Player");
        // load prefab for creating object
        name2object = new Dictionary<string, GameObject>();
        LoadAllPrefabsInFolder();
    }

    void Update() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float sensitive = 0.2f;
        float x1=-100, x2=100, y1=-20, y2=100, z1=-100, z2=100;
        if (!isAddingObject) {
            rotationX -= mouseY * sensitive * 5;
            rotationY += mouseX * sensitive * 5;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

        
        if (Input.GetKey(KeyCode.W)) {
            transform.position += 2*sensitive * transform.forward;
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.position += 2*sensitive * -transform.forward;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += sensitive * -transform.right;
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position += sensitive * transform.right;
        }
        if (Input.GetKey(KeyCode.Space)) {
            transform.position += new Vector3(0, sensitive, 0);
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            transform.position += new Vector3(0, -sensitive, 0);
        }
        if (transform.position.x < x1) {
            transform.position = new Vector3(x1, transform.position.y, transform.position.z);
        }
        if (transform.position.x > x2) {
            transform.position = new Vector3(x2, transform.position.y, transform.position.z);
        }
        if (transform.position.z < z1) {
            transform.position = new Vector3(transform.position.x, transform.position.y, z1);
        }
        if (transform.position.z > z2) {
            transform.position = new Vector3(transform.position.x, transform.position.y, z2);
        }
        if (transform.position.y < y1) {
            transform.position = new Vector3(transform.position.x, y1, transform.position.z);
        }
        if (transform.position.y > y2) {
            transform.position = new Vector3(transform.position.x, y2, transform.position.z);
        }
        string name = playerObjects[0].GetComponent<Player>().GetItemName();
        if (name != null && !isAddingObject) {
            isAddingObject = true;
            TransparentObject();
        } else if (Input.GetKeyDown(KeyCode.E)) {
            if (isAddingObject && PlacingIsValid()) {
                CreateObject(); 
                isAddingObject = false;
            }
        }
        if (name != null && isAddingObject) {
            AddingObject(mouseX, mouseY, 5.0f);
        }
    }

    private void TransparentObject() {
        string name = playerObjects[0].GetComponent<Player>().GetItemName();
        transparentObject = Instantiate(Resources.Load<GameObject>(FOLDERPATH + "/" + name));
    }
    
    private void CreateObject() {
        string name = playerObjects[0].GetComponent<Player>().GetItemName();
        GameObject obj = Instantiate(name2object[name], transparentObject.transform.position, transparentObject.transform.rotation);
        obj.name = name;
        stageController.items.Add(obj);
        Destroy(transparentObject);
        playerObjects[0].GetComponent<Player>().RemoveItem();
    }

    private bool PlacingIsValid() {
        // not finished
        if (transparentObject.transform.position.x > 0)
            return false;
        return true;
    }

    private void AddingObject(float mouseX, float mouseY, float sensitive, float x1=-100, float x2=100, float y1=-20, float y2=100, float z1=-100, float z2=100) {
        transparentObject.transform.position = transform.position + 25.0f* transform.forward;
        if (transparentObject.transform.position.x < x1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((x1 - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.x > x2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((x2 - transparentObject.transform.position.x) / transform.forward.x) * transform.forward;
        }
        if (transparentObject.transform.position.y < y1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((y1 - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.y > y2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((y2 - transparentObject.transform.position.y) / transform.forward.y) * transform.forward;
        }
        if (transparentObject.transform.position.z < z1) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((z1 - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }
        if (transparentObject.transform.position.z > z2) {
            transparentObject.transform.position = transparentObject.transform.position - Math.Abs((z2 - transparentObject.transform.position.z) / transform.forward.z) * transform.forward;
        }

        if (Input.GetMouseButton(0)) {
            //mouseY = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            //horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);
            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            diviateX = newRotation.x;
            diviateZ = newRotation.z;
            newRotation.x = newRotation.x - diviateX;
            newRotation.z = newRotation.z - diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);
        } else if (Input.GetMouseButton(1)) {
            mouseX = 0;
            Vector3 cameraForward = -transform.right;

            float verticalRotationAngle = -mouseY * sensitive;

            float horizontalRotationAngle = mouseX * sensitive;
            //horizontalRotationAngle = Mathf.Clamp(horizontalRotationAngle, -90f, 90f);

            Vector3 transparentObjectForward = transparentObject.transform.forward;
            //transparentObjectForward.y = 0.0f;
            Vector3 rotatedForward = Quaternion.AngleAxis(horizontalRotationAngle, Vector3.up) * transparentObjectForward;
            Vector3 finalForward = Quaternion.AngleAxis(verticalRotationAngle, cameraForward) * rotatedForward;

            transparentObject.transform.rotation = Quaternion.LookRotation(finalForward, cameraForward);

            Vector3 newRotation = transparentObject.transform.rotation.eulerAngles;
            newRotation.x = newRotation.x - diviateX;
            newRotation.z = newRotation.z - diviateZ;
            transparentObject.transform.rotation = Quaternion.Euler(newRotation);

        } else {
            rotationX -= mouseY ;
            rotationY += mouseX ;
            rotationX = Mathf.Clamp(rotationX, -90, 90);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);
        }

        ItemVisible(transparentObject, PlacingIsValid());
    }
    private void ItemVisible(GameObject item, bool visible)
    {

        Transform parentTransform = item.transform;
        if (item.TryGetComponent<Renderer>(out var renderer))
            renderer.enabled = visible;
        for (int i = 0; i < parentTransform.childCount; i++) {
            Transform childTransform = parentTransform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            ItemVisible(childObject, visible);
        }
    }
    private void LoadAllPrefabsInFolder() {
        UnityEngine.Object[] loadedObjects = Resources.LoadAll(FOLDERPATH);
        foreach (UnityEngine.Object obj in loadedObjects) {
            if (obj is GameObject) {
                name2object[obj.name] = obj as GameObject;
            }
        }
    }
}
