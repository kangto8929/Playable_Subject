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
            Debug.LogWarning("TraySpawner 스크립트를 찾을 수 없음");
        }
    }

    void Start()
    {
        //주기적으로 화면 안에 트레이 개수 및 타입 확인
        StartCoroutine(CheckTraysRoutine());
    }

    IEnumerator CheckTraysRoutine()
    {
        while (true)
        {
            LogVisibleTraysAndTypesInCameraView(Camera.main);
            yield return new WaitForSeconds(0.5f); // 0.5초마다 확인
        }
    }

    public void LogVisibleTraysAndTypesInCameraView(Camera cam)
    {
        if (cam == null)
        {
            Debug.LogWarning("카메라가 지정되지 않았습니다.");
            return;
        }

        // 카메라 시야 영역(Frustum) 계산
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        int count = 0;

        foreach (Transform child in _trayParent)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            // 자식 중에 Renderer가 있는지 확인
            Renderer renderer = child.GetComponentInChildren<Renderer>();
            if (renderer == null)
                continue;

            // 카메라의 시야에 들어왔는지 체크
            if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
            {
                TrayContentInitializer initializer = child.GetComponent<TrayContentInitializer>();
                if (initializer != null)
                {
                    Debug.Log($"[보이는 트레이 {count}] GoodsType: {initializer.CurrentGoodsType}");
                    count++;
                }
            }
        }

        Debug.Log($"현재 게임 뷰(카메라 시야) 안에 있는 트레이 개수: {count}");
    }

}
