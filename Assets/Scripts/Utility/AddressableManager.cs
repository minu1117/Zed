using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableManager : Singleton<AddressableManager>
{
    public async Task ApplyImage(string address, Image applyImage)
    {
        if (applyImage == null || address == null)
            return;

        if (applyImage.sprite.name == GetAddressName(address))
            return;

        var loadAsync = Addressables.LoadAssetAsync<Sprite>(address);
        loadAsync.Completed += handle => OnImageLoaded(handle, applyImage);

        await loadAsync.Task;
    }

    public async Task<Sprite> GetSprite(string address)
    {
        try
        {
            var sprite = await GetSpriteAsync(address);
            return sprite;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception : {ex.Message}");
            return null;
        }
    }

    private async Task<Sprite> GetSpriteAsync(string address)
    {
        try
        {
            AsyncOperationHandle<Sprite> loadAsync = Addressables.LoadAssetAsync<Sprite>(address);
            await loadAsync.Task;

            if (loadAsync.Status == AsyncOperationStatus.Succeeded)
            {
                return loadAsync.Result;
            }
            else
            {
                Debug.LogError($"Failed to load sprite at address {address}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception : {ex.Message}");
            return null;
        }
    }

    public async Task<Dictionary<string, Sprite>> LoadSpritesToDictionary(List<string> addresses)
    {
        var dict = new Dictionary<string, Sprite>();

        foreach (string address in addresses)
        {
            if (!dict.ContainsKey(address))
            {
                Sprite sprite = await GetSpriteAsync(address);
                dict.Add(address, sprite);
            }
        }

        return dict;
    }

    private void OnImageLoaded(AsyncOperationHandle<Sprite> obj, Image applyImage)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            applyImage.sprite = obj.Result;
        }
        else
        {
            Debug.LogError("Failed to load addressable image : private void OnImageLoaded");
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