using UnityEngine;

public class Building : MonoBehaviour
{
    public Renderer MainRenderer;
    public Vector2Int Size = Vector2Int.one;
    public float CellSize = 10f;


    [HideInInspector] public Vector2Int CurrentSize;
    private GameObject highlightParent;
    public Material highlightMaterial;

    [Header("Cost to build")]
    public ResourceCost Cost;

    private void Awake()
     {
         CurrentSize = Size;
     }
    void Start()
    {
        CreateHighlight();
    }
    public void SetTransparent(bool available)
    {
        Color targetColor = available ? Color.green : Color.red;
        foreach (var mat in MainRenderer.materials)
        {
            mat.color = targetColor;
        }
    }

    public void SetNormal()
    {
        foreach (var mat in MainRenderer.materials)
        {
            mat.color = Color.white;
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                if ((x + y) % 2 == 0) Gizmos.color = new Color(0.8f, 0f, 1f, 0.3f);
                else Gizmos.color = new Color(1f, 0.6f, 0f, 0.3f);

                Vector3 pos = transform.position + new Vector3(x * CellSize, 0, y * CellSize);
                Vector3 size = new Vector3(CellSize, 0.1f, CellSize);

                Gizmos.DrawCube(pos, size); 
            }
        }
    }

    void CreateHighlight()
    {
        if (highlightParent != null)
            Destroy(highlightParent);

        highlightParent = new GameObject("Highlight");
        highlightParent.transform.parent = transform;
        highlightParent.transform.localPosition = Vector3.zero;

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.parent = highlightParent.transform;

                quad.transform.localPosition = new Vector3(x * CellSize, 0.01f, y * CellSize);
                quad.transform.localRotation = Quaternion.Euler(90, 0, 0);
                quad.transform.localScale = Vector3.one * CellSize;

                Renderer quadRenderer = quad.GetComponent<Renderer>();
                quadRenderer.material = new Material(highlightMaterial);
                quadRenderer.material.color = (x + y) % 2 == 0 ? new Color(0.76f, 0.60f, 0.42f, 0.5f) : new Color(0.76f, 0.60f, 0.42f, 0.5f);

                Destroy(quad.GetComponent<Collider>());
            }
        }
    }

}