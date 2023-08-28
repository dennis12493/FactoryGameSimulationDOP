using UnityEngine;
public class PlaceObjects : MonoBehaviour
{

    private GameObject currentlySelectedObject;
    private bool isAnObjectSelected;

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isAnObjectSelected)
        {
            Destroy(currentlySelectedObject);
            isAnObjectSelected = false;
        }
        if (Input.GetKeyDown("r") && isAnObjectSelected)
        {
            currentlySelectedObject.transform.Rotate(0, 0, 90);
        }
    }

    public void SelectBuilding(GameObject building)
    {
        RemoveIfSelected();
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 spawnPos = new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y));
        currentlySelectedObject = Instantiate(building, spawnPos, Quaternion.identity);
        isAnObjectSelected = true;
    }

    private void RemoveIfSelected()
    {
        if (isAnObjectSelected)
        {
            Destroy(currentlySelectedObject);
            isAnObjectSelected = false;
        }
    }
}
