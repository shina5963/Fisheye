using UnityEngine;

public class SatellitePositionTracker : MonoBehaviour
{
    public Camera satelliteCamera; // q―ΜJ
    public Transform earthTransform; // nΜTransform
    public float satelliteAltitude = 400f; // x 400km
    private float earthRadius = 6371f; // nΜΌa (km)
    public Vector2 latLon;
    void Update()
    {
        // t[q―ΜάxoxπvZ΅ΔoΝ
        Vector3 satellitePosition = satelliteCamera.transform.position;
         latLon = CartesianToLatLon(satellitePosition, earthRadius);

       // Debug.Log($"Current Satellite Position: Lat {latLon.x:F2}, Lon {latLon.y:F2}");
    }

    Vector2 CartesianToLatLon(Vector3 position, float radius)
    {
        // nΜρ]πζΎ
        Vector3 earthRotation = earthTransform.rotation.eulerAngles;

        // q―ΜΚuπnξΜ[JΐWΙΟ·
        Quaternion earthInverseRotation = Quaternion.Inverse(earthTransform.rotation);
        Vector3 localPosition = earthInverseRotation * position;

        // άxπvZ
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg;

        // oxπvZ
        float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg;

        // nΜρ]πlΆ΅Δoxπβ³
        //longitude += earthRotation.y;

        // oxπ -180`180 xΙ³K»
        //if (longitude > 180) longitude -= 360;
        //else longitude += 360;

        return new Vector2(latitude, longitude);
    }
}
