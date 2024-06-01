using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableManager : Singleton<AddressableManager>
{
    public void ApplyImage(string address, Image applyImage)
    {
        if (applyImage == null)
            return;

        if (applyImage.sprite.name == GetAddressName(address))
            return;

        var loadAsync = Addressables.LoadAssetAsync<Sprite>(address);
        loadAsync.Completed += handle => OnImageLoaded(handle, applyImage);
    }

    private void OnImageLoaded(AsyncOperationHandle<Sprite> obj, Image applyImage)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            // 로드된 스프라이트를 UI 이미지에 적용
            applyImage.sprite = obj.Result;
        }
        else
        {
            Debug.LogError("Failed to load addressable image");
        }
    }

    private string GetAddressName(string address)
    {
        int lastSlashIndex = address.LastIndexOf('/');
        if (lastSlashIndex >= 0 && lastSlashIndex < address.Length - 1)
        {
            return address.Substring(lastSlashIndex + 1);
        }

        return address;
    }
}
