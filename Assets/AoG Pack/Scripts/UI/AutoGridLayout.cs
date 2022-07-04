using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("Layout/Auto Grid Layout Group", 152)]
public class AutoGridLayout : GridLayoutGroup
{
    //public int rows = 4;
    //public int columns = 4;
    //private float buttonWidth;                                        //Change
    //private float buttonHeight;                                        //Change
    //public Button prefab;
    //private Button button;

    //void Start() {

    //    RectTransform myRect = GetComponent<RectTransform>();        //Change
    //    buttonHeight = myRect.rect.height / (float)rows;            //Change
    //    buttonWidth = myRect.rect.width / (float)columns;            //Change
    //    GridLayoutGroup grid = this.GetComponent<GridLayoutGroup>();
    //    grid.cellSize = new Vector2(buttonWidth, buttonHeight);
    //    for(int i = 0; i < rows; i++) {
    //        for(int j = 0; j < columns; j++) {
    //            button = (Button)Instantiate(prefab);
    //            button.transform.SetParent(transform, false);        //Change
    //        }
    //    }
    //}

    [SerializeField] private int m_CellCount;

    [SerializeField]
#pragma warning disable CS0414 // The field 'AutoGridLayout.m_Column' is assigned but its value is never used
#pragma warning disable CS0414 // The field 'AutoGridLayout.m_Row' is assigned but its value is never used
    private int m_Column = 1, m_Row = 1;
#pragma warning restore CS0414 // The field 'AutoGridLayout.m_Row' is assigned but its value is never used
#pragma warning restore CS0414 // The field 'AutoGridLayout.m_Column' is assigned but its value is never used
    [SerializeField] private bool m_IsColumn;

    [SerializeField] private int m_MaxColumns;

    [SerializeField] private int m_RowCount;

    public float cellSizeX = 70;
    public float cellSizeY = 90;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        m_CellCount = transform.Cast<Transform>().SelectMany(t => t.GetComponents<RectTransform>()).ToArray().Length;
        m_MaxColumns = Mathf.FloorToInt((rectTransform.rect.width - (padding.right + padding.left)) / cellSize.x);
        m_RowCount = (int) ((float) m_CellCount / m_MaxColumns + 0.5f);


        var fHeight = Mathf.Clamp(rectTransform.rect.height / m_RowCount, 10, cellSizeY);
        var fWidth = Mathf.Clamp(rectTransform.rect.height / m_RowCount, 10, cellSizeX);
        var vSize = new Vector2(fWidth, fHeight);
        cellSize = vSize;
    }
}