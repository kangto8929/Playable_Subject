using UnityEngine;


public class GridCell : MonoBehaviour
{
    public int x, y; // �� ��ǥ
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
                Debug.Log($"Ŭ���� ����� Ÿ��: {clickedGoodsType}");

                // ���� ���̴� Ʈ������ GoodsType ã��
                TrayContentInitializer visibleTrayInitializer = FindVisibleTrayInitializer();

                if (visibleTrayInitializer != null)
                {
                    GoodsType requiredGoodsType = visibleTrayInitializer.CurrentGoodsType;
                    Debug.Log($"���� Ʈ���̰� �䱸�ϴ� Ÿ��: {requiredGoodsType}");

                    if (clickedGoodsType == requiredGoodsType)
                    {
                        Debug.Log($"Ÿ�� ��ġ! ��� [{x},{y}] �ı�!");
                        // ��� �ı�
                        Destroy(CurrentBlock);
                        CurrentBlock = null; 

                        if (GridManager.Instance != null)
                        {
                            GridManager.Instance.OnBlockRemoved(x, y);
                        }

                        // Ʈ������ ��ǰ ���� ����
                        visibleTrayInitializer.DecreaseGoodsCount();
                    }
                    else
                    {
                        Debug.LogWarning("Ÿ�� ����ġ!");
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

            // ī�޶��� �þ߿� ���Դ���
            if (GeometryUtility.TestPlanesAABB(planes, trayRenderer.bounds))
            {
                TrayContentInitializer initializer = trayChild.GetComponent<TrayContentInitializer>();
                if (initializer != null)
                {
                    return initializer;
                }
            }
        }
        return null; // ���̴� Ʈ���̰� ����
    }
}