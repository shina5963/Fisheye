using UnityEngine;

public class SatellitePositionTracker : MonoBehaviour
{
    public Camera satelliteCamera; // 衛星のカメラ
    public Transform earthTransform; // 地球のTransform
    public float satelliteAltitude = 400f; // 高度 400km
    private float earthRadius = 6371f; // 地球の半径 (km)
    public Vector2 latLon;
    void Update()
    {
        // 毎フレーム衛星の緯度経度を計算して出力
        Vector3 satellitePosition = satelliteCamera.transform.position;
         latLon = CartesianToLatLon(satellitePosition, earthRadius);

       // Debug.Log($"Current Satellite Position: Lat {latLon.x:F2}°, Lon {latLon.y:F2}°");
    }

    Vector2 CartesianToLatLon(Vector3 position, float radius)
    {
        // 地球の回転を取得
        Vector3 earthRotation = earthTransform.rotation.eulerAngles;

        // 衛星の位置を地球基準のローカル座標に変換
        Quaternion earthInverseRotation = Quaternion.Inverse(earthTransform.rotation);
        Vector3 localPosition = earthInverseRotation * position;

        // 緯度を計算
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg;

        // 経度を計算
        float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg;

        // 地球の回転を考慮して経度を補正
        //longitude += earthRotation.y;

        // 経度を -180～180 度に正規化
        //if (longitude > 180) longitude -= 360;
        //else longitude += 360;

        return new Vector2(latitude, longitude);
    }
}
