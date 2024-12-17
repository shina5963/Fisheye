using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    // �O���X�Ίp(�x)
    [SerializeField] private float inclinationAngle = 51.6f;

    // �O������(�b) = 90�� = 5400�b
    [SerializeField] private float orbitPeriod = 5400f/60/60;

    // 1��360�x��orbitPeriod�b�ŉ�邽�߂�1�b�������]�x��
    private float degreesPerSecond;

    public GameObject Satellite;
    void Start()
    {
        // �O���ʂ̌X�ΐݒ�
        // ������Ԃ͖{���q�ߐ���ɉq�����ʒu���A�ԓ��ʏ�Ƃ����Y������̉�]����{�O���ʁB
        // �X�΂�Z�����ŕt�^����ƁAX-Z���ʂ��X���C���[�W�ƂȂ�B

        // ���݁A�q����(-6771,0,0)�ɂ���A����͖{���q�ߐ���B
        // �X�΂�Z���ŗ^����iX�����ԓ��AZ���ŌX�΂�t����j
        Satellite.transform.localRotation = Quaternion.Euler(0f, 0f, inclinationAngle);

        // ��]���x�v�Z
        // 360�x / orbitPeriod�b = deg/sec
        degreesPerSecond = 360f / orbitPeriod;
    }

    void Update()
    {
        // �e�I�u�W�F�N�g��Y�����ŉ�]������iZ���ŌX�΂����������Y��]���邱�ƂŌX�΋O�������j
        Satellite.transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f, Space.Self);
    }
}
