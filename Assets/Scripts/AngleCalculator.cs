using UnityEngine;

public class AngleCalculator : MonoBehaviour
{
    public GameObject satellite;  // 衛星オブジェクト
    public GameObject earth;      // 地球オブジェクト
    public GameObject fisheye;      // 地球オブジェクト
    public float angle;
    void Update()
    {
        if (satellite != null && earth != null && fisheye!= null)
        {
            Vector3 lineA = fisheye.transform.forward; // fisheyeのローカルZ軸をワールド座標系に変換したベクトル
                                             // 結果をデバッグログに表示
            Debug.Log("lineA : " + lineA);
            // ラインB: 衛星中心と地球中心を結ぶベクトル
            Vector3 lineB = earth.transform.position - satellite.transform.position;
            Debug.Log("lineB : " + lineB.normalized);

            // ベクトルの角度を求める
             angle = Vector3.Angle(lineA, lineB.normalized);

            // 結果をデバッグログに表示
            //Debug.Log("ラインAとラインBの交差角度: " + angle + "度");
        }
        else
        {
           // Debug.LogWarning("衛星、地球、またはカメラの参照が設定されていません。");
        }
    }
}
