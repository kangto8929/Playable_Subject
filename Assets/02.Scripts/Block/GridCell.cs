using UnityEngine;


public class GridCell : MonoBehaviour
{
    public int x, y; // 셀 좌표
    public GameObject CurrentBlock;

    public void SetCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }


    public void OnClicked()
    {
        Debug.Log($"Cell [{x},{y}] clicked!");

        if (CurrentBlock != null)
        {
            BlockData blockData = CurrentBlock.GetComponent<BlockData>();
            if (blockData != null)
            {
                GoodsType clickedGoodsType = blockData.GoodsType;
                Debug.Log($"클릭된 블록의 타입: {clickedGoodsType}");

                // 현재 보이는 트레이의 GoodsType 찾기
                TrayContentInitializer visibleTrayInitializer = FindVisibleTrayInitializer();

                if (visibleTrayInitializer != null)
                {
                    GoodsType requiredGoodsType = visibleTrayInitializer.CurrentGoodsType;
                    Debug.Log($"현재 트레이가 요구하는 타입: {requiredGoodsType}");

                    if (clickedGoodsType == requiredGoodsType)
                    {
                        Debug.Log($"타입 일치! 블록 [{x},{y}] 파괴!");
                        // 블록 파괴
                        Destroy(CurrentBlock);
                        CurrentBlock = null; 

                        if (GridManager.Instance != null)
                        {
                            GridManager.Instance.OnBlockRemoved(x, y);
                        }

                        // 트레이의 상품 개수 감소
                        visibleTrayInitializer.DecreaseGoodsCount();
                    }
                    else
                    {
                        Debug.LogWarning("타입 불일치!");
                    }
                }

            }

        }

    }


    private TrayContentInitializer FindVisibleTrayInitializer()
    {
        Camera cam = Camera.main;


        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        TraySpawner traySpawner = FindObjectOfType<TraySpawner>();
        if (traySpawner == null || traySpawner.TrayParent == null)
        {
            return null;
        }

        foreach (Transform trayChild in traySpawner.TrayParent)
        {
            if (!trayChild.gameObject.activeInHierarchy)
                continue;

            Renderer trayRenderer = trayChild.GetComponentInChildren<Renderer>();
            if (trayRenderer == null)
                continue;

            // 카메라의 시야에 들어왔는지
            if (GeometryUtility.TestPlanesAABB(planes, trayRenderer.bounds))
            {
                TrayContentInitializer initializer = trayChild.GetComponent<TrayContentInitializer>();
                if (initializer != null)
                {
                    return initializer;
                }
            }
        }
        return null; // 보이는 트레이가 없음
    }
}