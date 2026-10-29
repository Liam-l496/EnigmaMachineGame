using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlugBoardPlugSocketScript : MonoBehaviour
{

    [Header("Text")]
    [SerializeField] TextMeshPro textObject;
    [SerializeField] char plugBoardText;
    [SerializeField] Transform plugInPosition;
    [SerializeField] Transform plugOutPosition;
    public PlugBoardPlugScript plugObject;


    private Outline socketOutline;


    // Start is called before the first frame update
    void Start()
    {
        if (plugBoardText == '\0')
        {
            plugBoardText = ' ';
        }
        textObject.text = plugBoardText + "";
        socketOutline = gameObject.AddComponent<Outline>();
        socketOutline.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        PlugBoardPlugScript plug = collision.gameObject.GetComponent<PlugBoardPlugScript>();
        if (plug && !plugObject)
        {
            // Collided with a plug and no plug is in yet
            Debug.Log($"[{Time.time:F2}s] Collided with plug socket");
            SetPlug(plug);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        PlugBoardPlugScript plug = collision.gameObject.GetComponent<PlugBoardPlugScript>();
        if (plug && plug == plugObject)
        {
            Debug.Log($"[{Time.time:F2}s] Not collided with plug socket");
            RemovePlug();
        }
    }

    public void SetPlug(PlugBoardPlugScript plug)
    {
        if (plugObject)
        {
            return;
        }
        Debug.Log($"[{Time.time:F2}s] Setting with plug socket");
        EnableOutline();
        plug.StartAnimation(plugInPosition);
        plug.SetPlugBoard(this, plugBoardText);
        plugObject = plug;
        return;
    }

    public void RemovePlug()
    {
        if (!plugObject)
        {
            return;
        }
        DisableOutline();
        plugObject.StartAnimation(plugOutPosition);
        plugObject.SetPlugBoard(null, ' ');
        plugObject = null;
    }

    public void EnableOutline()
    {
        socketOutline.enabled = true;
    }

    public void DisableOutline()
    {
        socketOutline.enabled = false;
    }
}
