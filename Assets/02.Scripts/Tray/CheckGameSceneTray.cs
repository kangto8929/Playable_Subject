using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGameSceneTray : MonoBehaviour
{
    [SerializeField]
    private Transform _trayParent;

    private void Awake()
    {
        TraySpawner traySpanwer = FindObjectOfType<TraySpawner>();

        if (traySpanwer != null)
        {
            _trayParent = traySpanwer.TrayParent;
        }
        else
        {
            Debug.LogWarning("TraySpawner ��ũ��Ʈ�� ã�� �� ����");
        }
    }

    void Start()
    {
        //�ֱ������� ȭ�� �ȿ� Ʈ���� ���� �� Ÿ�� Ȯ��
        StartCoroutine(CheckTraysRoutine());
    }

    IEnumerator CheckTraysRoutine()
    {
        while (true)
        {
            LogVisibleTraysAndTypesInCameraView(Camera.main);
            yield return new WaitForSeconds(0.5f); // 0.5�ʸ��� Ȯ��
        }
    }

    public void LogVisibleTraysAndTypesInCameraView(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogWarning("ī�޶� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ī�޶� �þ� ����(Frustum) ���
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        int count = 0;

        foreach (Transform child in _trayParent)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            // �ڽ� �߿� Renderer�� �ִ��� Ȯ��
            Renderer renderer = child.GetComponentInChildren<Renderer>();
            if (renderer == null)
                continue;

            // ī�޶��� �þ߿� ���Դ��� üũ
            if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
            {
                TrayContentInitializer initializer = child.GetComponent<TrayContentInitializer>();
                if (initializer != null)
                {
                    Debug.Log($"[���̴� Ʈ���� {count}] GoodsType: {initializer.CurrentGoodsType}");
                    count++;
                }
            }
        }

        Debug.Log($"���� ���� ��(ī�޶� �þ�) �ȿ� �ִ� Ʈ���� ����: {count}");
    }

}
