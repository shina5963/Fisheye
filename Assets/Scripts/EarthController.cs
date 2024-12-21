using UnityEngine;
using UnityEngine.UI; // UI���g�p����ꍇ�K�v

public class EarthController : MonoBehaviour
{
    public GameObject Earth;

    // �f�t�H���g24����: 86400�b(1��) / 3600 = 24�B 
    // ���݂�1�b=1���Ԋ��Z���邽��24��OK�B
    [SerializeField]
    public float rotationPeriod =1440;

    // ���݂̃V�~�����[�V����������(���ԒP��)
    private float simulatedTimeHours = 0f;

    // ���ݎ�����\������UI�e�L�X�g
    public Text timeText;

    private void Start()
    {
        // simulatedTimeHours��0����J�n
        simulatedTimeHours = 0f;
    }
    public string timeString;
    private void Update()
    {
        // �n����]����
        float degreesPerSecond = 360f / rotationPeriod;
        Earth.transform.Rotate(Vector3.up, -1*degreesPerSecond * Time.deltaTime, Space.Self);

        // �V�~�����[�V�����������̐i�s
        // �����ł�1�b���Ƃ�1���Ԃ��o�߂���Ɖ���
        simulatedTimeHours += Time.deltaTime*(24f / rotationPeriod);

        // 24���Ԃ𒴂�����0�ɖ߂�(1���T�C�N��)
        if (simulatedTimeHours >= 24f)
        {
            simulatedTimeHours -= 24f;
        }

        // ���ݎ����̃t�H�[�}�b�g
        int hours = Mathf.FloorToInt(simulatedTimeHours);
        float fractionalHour = simulatedTimeHours - hours;
        int minutes = Mathf.FloorToInt(fractionalHour * 60);
        float fractionalMinute = (fractionalHour * 60) - minutes;
        int seconds = Mathf.FloorToInt(fractionalMinute * 60);

        // "HH:MM:SS"�`���Ńe�L�X�g�\��
        //string timeString = string.Format("Time {0:D2}:{1:D2}", hours, minutes);
        timeString = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        if (timeText != null)
        {
            timeText.text = timeString;
        }
    }
}
