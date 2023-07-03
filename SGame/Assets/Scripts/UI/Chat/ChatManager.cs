using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.IO.Compression;
using Unity.Collections;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private TMP_InputField textInput;
    [SerializeField] private TextMeshProUGUI textDisplay;
    
    // Start subscribes to input field events
    void Start()
    {
        textInput.onSelect.AddListener((string value) =>
        {
            OnTextSelected();
        });
        textInput.onDeselect.AddListener((string value) =>
        {
            OnTextDeSelected();
        });
        textInput.onSubmit.AddListener((string value) =>
        {
            OnMessageSent();
        });
    }
    //Update checks for if the text input can be selected when enter is pressed
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Return) && !textInput.isFocused && PlayerHandler.instance.KeyBlockers.Count == 0)
        {
            textInput.Select();
        }
    }
    /// <summary>
    /// Method that adds a blocker to the player when the text input is selected
    /// </summary>
    private void OnTextSelected()
    {
        PlayerHandler.instance.KeyBlockers.Add(this.gameObject);
    }
    /// <summary>
    /// Method that removes this blocker when the text input is deselected
    /// </summary>
    private void OnTextDeSelected()
    {
        PlayerHandler.instance.KeyBlockers.Remove(this.gameObject);
    }
    /// <summary>
    /// Method that calls the ServerRPC to send the message and removes this blocker when the message is submitted
    /// </summary>
    private void OnMessageSent()
    {
        Debug.Log("Message was submitted");
        string message = textInput.text;
        using var sentValue = message.ToBrotliAsync();
        if (sentValue.Result.Original.Size > 511)
        {
            message = "Message was longer than 512 bytes";
        }
        else
        {
            message = sentValue.Result.Result.Value;
        }
        FixedString512Bytes fixedMessage = new FixedString512Bytes(message);
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("We served");
            LogChatMessage(textInput.text);
            SendMessageClientRpc(fixedMessage);
        }
        else
        {
            SendClientsMessageServerRpc(fixedMessage);
        }
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("We hosted");
        }
        textInput.text = "";
    } 
    /// <summary>
    /// Server RPC that calls the SendMessageClientRpc
    /// </summary>
    /// <param name="message">The message to send</param>
    [ServerRpc(RequireOwnership = false)]
    private void SendClientsMessageServerRpc(FixedString512Bytes message)
    {
        SendMessageClientRpc(message);
    }

    /// <summary>
    /// Client RPC that unpacks the sent message and logs it
    /// </summary>
    /// <param name="message">The sent message</param>
    [ClientRpc]
    private void SendMessageClientRpc(FixedString512Bytes message)
    {
        string recievedMessage = message.ToString();
        using var clearMessage = recievedMessage.FromBrotliAsync();
        recievedMessage = clearMessage.Result;
        LogChatMessage(recievedMessage);
    }
    
    /// <summary>
    /// Method that logs a message to the chat
    /// </summary>
    /// <param name="message">The message to log</param>
    public void LogChatMessage(string message)
    {
        textDisplay.text += $"\n {message}";
    }
}
