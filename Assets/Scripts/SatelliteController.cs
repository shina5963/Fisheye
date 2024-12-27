using UnityEngine;
using UnityEngine.UI; // UI�p

public class SatelliteController : MonoBehaviour
{

    public EarthController earthController;
    // �O���X�Ίp(�x)
    [SerializeField] private float inclinationAngle = 51.6f;

    // �O������(�b) = 90�� = 5400�b
    [SerializeField] private float orbitPeriod = 5400f / 60 / 60;

    // 1��360�x��orbitPeriod�b�ŉ�邽�߂�1�b�������]�x��
    private float degreesPerSecond;

    public Vector2 rollSpeedRange = new Vector2(-0.5f, 0.5f);
    public Vector2 pitchSpeedRange = new Vector2(-0.5f, 0.5f);
    public Vector2 yawSpeedRange = new Vector2(-0.5f, 0.5f);

    public float rollSpeed=1;
    public float pitchSpeed=1;
    public float yawSpeed=1;

    public Vector3 satellitePower=new Vector3(1,1,1);

    // �X�V�Ԋu
    [SerializeField] private float randomizeInterval = 2.0f;
    private float timeSinceLastUpdate = 0f;

    public GameObject Satellite;
    public GameObject SatelliteParent;

    // UI Text�I�u�W�F�N�g�̎Q��
    [Header("UI Text References")]
    [SerializeField] private Text rollText;
    [SerializeField] private Text pitchText;
    [SerializeField] private Text yawText;

    // UI Text�I�u�W�F�N�g�̎Q��
    [Header("UI Text References")]
    [SerializeField] private Text rollTorqueText;
    [SerializeField] private Text pitchTorqueText;
    [SerializeField] private Text yawTorqueText;

    void Start()
    {

        // �O���ʂ̌X�ΐݒ�
       // SatelliteParent.transform.localRotation = Quaternion.Euler(0f, 0f, inclinationAngle);

        //SatelliteParent.transform.localRotation = Quaternion.Euler(inclinationAngle,-90f ,0f );


        // ���������_����]���x�ݒ�
        //UpdateRandomSpeeds();
    }

    void Update()
    {


        orbitPeriod = earthController.rotationPeriod / 16f;//earthController.rotationPeriod=24���Ԃ̏ꍇ�A90��

        // ��]���x�v�Z
        degreesPerSecond = 360f / orbitPeriod;

        // �O����]����
        //SatelliteParent.transform.Rotate(0f, -1 * degreesPerSecond * Time.deltaTime, 0f, Space.Self);
        //SatelliteParent.transform.Rotate(0f, 0, -1 * degreesPerSecond * Time.deltaTime, Space.Self);
        Vector3 localEulerAngles = SatelliteParent.transform.localRotation.eulerAngles;

        // ���݂̃��[�J����]�p���擾���Az��������������
        localEulerAngles.z += degreesPerSecond * Time.deltaTime;

        // �V������]��K�p����
        SatelliteParent.transform.localRotation = Quaternion.Euler(localEulerAngles);
        // �����_���p����]
        //Satellite.transform.Rotate(rollSpeed * Time.deltaTime, pitchSpeed * Time.deltaTime, yawSpeed * Time.deltaTime, Space.Self);
        Satellite.transform.Rotate(satellitePower.x* Time.deltaTime, satellitePower.y* Time.deltaTime, satellitePower.z* Time.deltaTime, Space.Self);

        // �����_�����x�X�V
      //  timeSinceLastUpdate += Time.deltaTime;
        //if (timeSinceLastUpdate >= randomizeInterval)
       // {
            //UpdateRandomSpeeds();
       //     timeSinceLastUpdate = 0f;
       // }

        // �e�L�X�g�X�V
       // UpdateUIText();
    }

    // �����_���ȑ��x�𐶐�����֐�
    private void UpdateRandomSpeeds()
    {
        rollSpeed = Random.Range(rollSpeedRange.x, rollSpeedRange.y);
        pitchSpeed = Random.Range(pitchSpeedRange.x, pitchSpeedRange.y);
        yawSpeed = Random.Range(yawSpeedRange.x, yawSpeedRange.y);
    }

    // �e�L�X�g�I�u�W�F�N�g�ɕ\�����X�V����֐�
    private void UpdateUIText()
    {
        if (rollText != null) rollTorqueText.text = $"Roll: {rollSpeed:F2}";
        if (pitchText != null) pitchTorqueText.text = $"Pitch: {pitchSpeed:F2}";
        if (yawText != null) yawTorqueText.text = $"Yaw: {yawSpeed:F2}";

        Vector3 eulerRotation = Satellite.transform.localEulerAngles;

        if (rollText != null) rollText.text = $"Roll: {eulerRotation.z:F2}��";
        if (pitchText != null) pitchText.text = $"Pitch: {eulerRotation.x:F2}��";
        if (yawText != null) yawText.text = $"Yaw: {eulerRotation.y:F2}��";
    }
}
