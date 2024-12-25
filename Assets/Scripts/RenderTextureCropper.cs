using UnityEngine;
using System.IO;

public class RenderTextureCropper : MonoBehaviour
{
    public RenderTexture renderTexture; // 2592x2592のRenderTexture

    void SaveRenderTextureAsCroppedPNG()
    {
        if (renderTexture == null)
        {
            Debug.LogError("RenderTexture is not assigned.");
            return;
        }

        // RenderTextureをアクティブにする
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // RenderTextureの内容をTexture2Dにコピー
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // RenderTextureを元に戻す
        RenderTexture.active = currentRT;

        // 中央部分を切り抜く
        Texture2D croppedTexture = CropTexture(texture, 2592, 1944);

        // 日付時刻を利用してファイル名を設定
        string timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string savePath = Path.Combine(Application.persistentDataPath, $"CroppedImage_{timeStamp}.png");

        // PNGにエンコードして保存
        byte[] bytes = croppedTexture.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);

        Debug.Log($"Cropped image saved to: {savePath}");

        // メモリを解放
        Destroy(texture);
        Destroy(croppedTexture);
    }

    Texture2D CropTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        int startX = (source.width - targetWidth) / 2; // 横方向の開始位置
        int startY = (source.height - targetHeight) / 2; // 縦方向の開始位置

        Color[] pixels = source.GetPixels(startX, startY, targetWidth, targetHeight);
        Texture2D cropped = new Texture2D(targetWidth, targetHeight, source.format, false);
        cropped.SetPixels(pixels);
        cropped.Apply();

        return cropped;
    }

    // デバッグ用：キーを押したら保存
    void Update()
    {
      //  if (Input.GetKeyDown(KeyCode.S))
      //  {
       //     SaveRenderTextureAsCroppedPNG();
       // }
    }
}
