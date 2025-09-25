using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuildingsGrid : MonoBehaviour
{
    public Vector2Int GridSize = new Vector2Int(10, 10);

    private Building[,] grid;
    private Building flyingBuilding;
    private Camera mainCamera;
    private Vector3 gridOrigin;
    private int currentRotation = 0;

    public Text messageText;

    private void Awake()
    {
        grid = new Building[GridSize.x, GridSize.y];

        mainCamera = Camera.main;
        float cellSize = 1f; 
        gridOrigin = transform.position - new Vector3(GridSize.x * cellSize * 0.5f, 0, GridSize.y * cellSize * 0.5f);
    }

    public void StartPlacingBuilding(Building buildingPrefab)
    {
        if (!EconomyManager.Instance.CanAfford(buildingPrefab.Cost))
        {
            if (messageText != null)
            {
                messageText.text = "Not enough resources!";
                CancelInvoke(nameof(ClearMessage));   
                Invoke(nameof(ClearMessage), 2f);  
            }

            Debug.Log("Not enough resources to place this building!");
            return;
            
        }

        if (flyingBuilding != null)
        {
            Destroy(flyingBuilding.gameObject);
        }

        flyingBuilding = Instantiate(buildingPrefab);
        currentRotation = 0;
    }

    private void Update()
    {
        if (flyingBuilding != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(flyingBuilding.gameObject);
                flyingBuilding = null;
                return;
            }
            //HandleRotation();
            var groundPlane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float position))
            {
                Vector3 worldPosition = ray.GetPoint(position);

                float cellSize = flyingBuilding.CellSize;

                int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize);
                int y = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / cellSize);

                bool available = true;

                if (x < 0 || x > GridSize.x - flyingBuilding.CurrentSize.x) available = false;
                if (y < 0 || y > GridSize.y - flyingBuilding.CurrentSize.y) available = false;

                if (available && IsPlaceTaken(x, y)) available = false;

                flyingBuilding.transform.position = new Vector3(x * cellSize + gridOrigin.x, 0, y * cellSize + gridOrigin.z);
                flyingBuilding.SetTransparent(available);

                if (available && Input.GetMouseButtonDown(0))
                {
                    if (EconomyManager.Instance.SpendResources(flyingBuilding.Cost))
                    {
                        PlaceFlyingBuilding(x, y);
                    }
                }

            }
        }
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.R))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
            {
                currentRotation = (currentRotation + 90) % 360;
                ApplyRotation();
            }
            else if (scroll < 0f)
            {
                currentRotation = (currentRotation - 90 + 360) % 360;
                ApplyRotation();
            }
        }
    }
    
    private void ApplyRotation()
    {
        flyingBuilding.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        if (currentRotation % 180 == 0)
        {
            flyingBuilding.CurrentSize = flyingBuilding.Size;
        }
        else
        {
            flyingBuilding.CurrentSize = new Vector2Int(flyingBuilding.Size.y, flyingBuilding.Size.x);
        }
    }

    private bool IsPlaceTaken(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.CurrentSize.x; x++)
        {
            for (int y = 0; y < flyingBuilding.CurrentSize.y; y++)
            {
                if (grid[placeX + x, placeY + y] != null) return true;
            }
        }

        return false;
    }

    private void PlaceFlyingBuilding(int placeX, int placeY)
    {
        for (int x = 0; x < flyingBuilding.CurrentSize.x; x++)
        {
            for (int y = 0; y < flyingBuilding.CurrentSize.y; y++)
            {
                grid[placeX + x, placeY + y] = flyingBuilding;
            }
        }

        flyingBuilding.SetNormal();
        flyingBuilding = null;
    }
    public void ClearScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }
}
