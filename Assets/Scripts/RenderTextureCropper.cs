using UnityEngine;
using System.IO;

public class RenderTextureCropper : MonoBehaviour
{
    public RenderTexture renderTexture; // 2592x2592��RenderTexture

    void SaveRenderTextureAsCroppedPNG()
    {
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture is not assigned.");
            return;
        }

        // RenderTexture���A�N�e�B�u�ɂ���
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // RenderTexture�̓��e��Texture2D�ɃR�s�[
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // RenderTexture�����ɖ߂�
        RenderTexture.active = currentRT;

        // ����������؂蔲��
        Texture2D croppedTexture = CropTexture(texture, 2592, 1944);

        // ���t�����𗘗p���ăt�@�C������ݒ�
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string savePath = Path.Combine(Application.persistentDataPath, $"CroppedImage_{timeStamp}.png");

        // PNG�ɃG���R�[�h���ĕۑ�
        byte[] bytes = croppedTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        Debug.Log($"Cropped image saved to: {savePath}");

        // �����������
        Destroy(texture);
        Destroy(croppedTexture);
    }

    Texture2D CropTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        int startX = (source.width - targetWidth) / 2; // �������̊J�n�ʒu
        int startY = (source.height - targetHeight) / 2; // �c�����̊J�n�ʒu

        Color[] pixels = source.GetPixels(startX, startY, targetWidth, targetHeight);
        Texture2D cropped = new Texture2D(targetWidth, targetHeight, source.format, false);
        cropped.SetPixels(pixels);
        cropped.Apply();

        return cropped;
    }

    // �f�o�b�O�p�F�L�[����������ۑ�
    void Update()
    {
      //  if (Input.GetKeyDown(KeyCode.S))
      //  {
       //     SaveRenderTextureAsCroppedPNG();
       // }
    }
}
