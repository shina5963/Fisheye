using UnityEngine;
using UnityEngine.UIElements;

public class TransformRotationSync : MonoBehaviour
{
    public SatelliteController satelliteController;
    public GameObject targetObject; // ��]�𓯊�����Ώۂ̃I�u�W�F�N�g
   
    private Vector3Field SatelliteRotation;
    private Vector3Field SatellitePower;


    private void OnEnable()
    {
        // UIDocument�̃��[�g�v�f���擾
        var root = GetComponent<UIDocument>().rootVisualElement;

        SatelliteRotation = root.Query<Vector3Field>().Where(v => v.label == "Satellite Rotation").First();
        SatellitePower= root.Query<Vector3Field>().Where(v => v.label == "Satellite Power").First();


        // �����l��ݒ�
        if (targetObject != null)
        {
            SatelliteRotation.value = RoundVector3(targetObject.transform.rotation.eulerAngles, 2);
        }

       
            SatellitePower.value = RoundVector3(satelliteController.satellitePower  , 2);
        


        // Vector3Field�̒l�ύX�C�x���g��o�^
        SatelliteRotation.RegisterValueChangedCallback(evt =>
        {
            if (targetObject != null)
            {
                // �l���ۂ߂�Transform�ɔ��f
                var roundedValue = RoundVector3(evt.newValue, 2);
                targetObject.transform.rotation = Quaternion.Euler(roundedValue);

                // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
                SatelliteRotation.SetValueWithoutNotify(roundedValue);
            }
        });

        SatellitePower.RegisterValueChangedCallback(evt =>
        {

            // �l���ۂ߂�Transform�ɔ��f
            var roundedValue = RoundVector3(evt.newValue, 2);
            satelliteController.satellitePower = roundedValue;
     
            // �ۂ߂��l���t�B�[���h�ɍđ���i�t�B�[���h���X�V�j
            SatellitePower.SetValueWithoutNotify(roundedValue);
            
        });
    }

    private void Update()
    {
        // �I�u�W�F�N�g��Transform���ύX���ꂽ�ꍇ�AVector3Field���X�V
        if (targetObject != null)
        {
            var roundedValue = RoundVector3(targetObject.transform.rotation.eulerAngles, 2);
            if (SatelliteRotation.value != roundedValue)
            {
                SatelliteRotation.value = roundedValue;
            }
        }
    }

    // Vector3 �̊e�������w�肵�������_�ȉ��̌����Ŋۂ߂�
    private Vector3 RoundVector3(Vector3 value, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return new Vector3(
            Mathf.Round(value.x * multiplier) / multiplier,
            Mathf.Round(value.y * multiplier) / multiplier,
            Mathf.Round(value.z * multiplier) / multiplier
        );
    }
}
