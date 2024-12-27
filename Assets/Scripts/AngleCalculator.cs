using UnityEngine;

public class AngleCalculator : MonoBehaviour
{
    public GameObject satellite;  // �q���I�u�W�F�N�g
    public GameObject earth;      // �n���I�u�W�F�N�g
    public GameObject fisheye;      // �n���I�u�W�F�N�g
    public float angle;
    void Update()
    {
        if (satellite != null && earth != null && fisheye!= null)
        {
            Vector3 lineA = fisheye.transform.forward; // fisheye�̃��[�J��Z�������[���h���W�n�ɕϊ������x�N�g��
                                             // ���ʂ��f�o�b�O���O�ɕ\��
            Debug.Log("lineA : " + lineA);
            // ���C��B: �q�����S�ƒn�����S�����ԃx�N�g��
            Vector3 lineB = earth.transform.position - satellite.transform.position;
            Debug.Log("lineB : " + lineB.normalized);

            // �x�N�g���̊p�x�����߂�
             angle = Vector3.Angle(lineA, lineB.normalized);

            // ���ʂ��f�o�b�O���O�ɕ\��
            //Debug.Log("���C��A�ƃ��C��B�̌����p�x: " + angle + "�x");
        }
        else
        {
           // Debug.LogWarning("�q���A�n���A�܂��̓J�����̎Q�Ƃ��ݒ肳��Ă��܂���B");
        }
    }
}
