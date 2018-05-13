using UnityEngine;

public class AddRemoveParentHelper: MonoBehaviour {
    private static AddRemoveParentHelper instance;
    private AddRemoveParentHelper() { }

    public static AddRemoveParentHelper Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AddRemoveParentHelper();
            }
            return instance;
        }
    }

    public void SetParentObject(GameObject childObject, GameObject parentObject)
    {
        childObject.transform.parent = parentObject.transform;
    }

    public void SetParentObject(Component childObject, GameObject parentObject)
    {
        childObject.transform.parent = parentObject.transform;
    }

    public Component CloneObject(Component objectToClone, GameObject parentObject, string newObjectName)
    {
        Component cp = Instantiate(objectToClone, parentObject.transform.position, parentObject.transform.rotation);
        cp.name = newObjectName;
        SetParentObject(cp, parentObject);
        cp.transform.localScale = objectToClone.gameObject.transform.localScale;
        cp.gameObject.SetActive(true);
        return cp;
    }

    public void RemoveParentObject(GameObject childObject)
    {
        childObject.transform.parent = null;
    }

    public GameObject InstantiatePrefab(string prefabName, string nameObject, GameObject parentObject)
    {
        GameObject newObject = Instantiate(Resources.Load(prefabName), parentObject.transform) as GameObject;
        newObject.name = nameObject;
        newObject.gameObject.SetActive(true);
        return newObject;
    }

    public void ClearGameObjectChildren(GameObject parent)
    {
        if (parent == null)
            return;
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
