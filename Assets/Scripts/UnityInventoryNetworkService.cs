using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UnityInventoryNetworkService : IInventoryNetworkService
{
    private readonly string url = "https://wadahub.manerai.com/api/inventory/status";
    
    public IEnumerator SendInventoryEvent(InventoryItem item, string action)
    {
        string json = JsonUtility.ToJson(new { itemID = item.ItemID, action = action });
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer kPERnYcWAY46xaSy8CEzanosAgsWM84Nx7SKM4QBSqPq6c7StWfGxzhxPfDh8MaP");
        
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error sending inventory event: " + request.error);
        }
    }
}
