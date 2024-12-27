using UnityEngine;

public class SatellitePositionTracker : MonoBehaviour
{
    public Camera satelliteCamera; // �q���̃J����
    public Transform earthTransform; // �n����Transform
    public float satelliteAltitude = 400f; // ���x 400km
    private float earthRadius = 6371f; // �n���̔��a (km)
    public Vector2 latLon;
    void Update()
    {
        // ���t���[���q���̈ܓx�o�x���v�Z���ďo��
        Vector3 satellitePosition = satelliteCamera.transform.position;
         latLon = CartesianToLatLon(satellitePosition, earthRadius);

       // Debug.Log($"Current Satellite Position: Lat {latLon.x:F2}��, Lon {latLon.y:F2}��");
    }

    Vector2 CartesianToLatLon(Vector3 position, float radius)
    {
        // �n���̉�]���擾
        Vector3 earthRotation = earthTransform.rotation.eulerAngles;

        // �q���̈ʒu��n����̃��[�J�����W�ɕϊ�
        Quaternion earthInverseRotation = Quaternion.Inverse(earthTransform.rotation);
        Vector3 localPosition = earthInverseRotation * position;

        // �ܓx���v�Z
        float latitude = Mathf.Asin(localPosition.y / radius) * Mathf.Rad2Deg;

        // �o�x���v�Z
        float longitude = Mathf.Atan2(localPosition.z, localPosition.x) * Mathf.Rad2Deg;

        // �n���̉�]���l�����Čo�x��␳
        //longitude += earthRotation.y;

        // �o�x�� -180�`180 �x�ɐ��K��
        //if (longitude > 180) longitude -= 360;
        //else longitude += 360;

        return new Vector2(latitude, longitude);
    }
}
