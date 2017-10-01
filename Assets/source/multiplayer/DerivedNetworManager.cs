using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class DerivedNetworManager : NetworkManager
{
    private static DerivedNetworManager m_instance;
    private string m_ipInfo = "";
   
    public delegate void OnConnect(bool value);

    public OnConnect m_onServerConnect = null;
    public OnConnect m_onClientConnect = null;
    public OnConnect m_onStartHost = null;

    public delegate void OnDisconnect();
    public OnDisconnect m_onServerDisconnect = null;
    public OnDisconnect m_onClientDisconnect = null;
    public OnDisconnect m_onStopHost = null;

    public delegate void onServerAddPlayer(NetworkConnection conn);
    public onServerAddPlayer m_onServerAddPlayer = null;

    public static DerivedNetworManager getInstance() {
        return m_instance;
    }

    void Awake() {
        m_instance = this;
    }

	// Use this for initialization
	void Start () {
        m_ipInfo = Network.player.ipAddress;
        m_ipInfo = m_ipInfo + " : " + networkPort;
    }

    public void setConnectionDelegates(OnConnect serverConnect, OnConnect clientConnect, OnConnect startHost, onServerAddPlayer serverAddPlayer)
    {
        m_onServerConnect = serverConnect;
        m_onClientConnect = clientConnect;
        m_onStartHost = startHost;
        m_onServerAddPlayer = serverAddPlayer;
    }

    public void setDisconnectionDelegates(OnDisconnect serverDisconnect, OnDisconnect clientDisconnect, OnDisconnect stopHost)
    {
        m_onServerDisconnect = serverDisconnect;
        m_onClientDisconnect = clientDisconnect;
        m_onStopHost = stopHost;
    }

	// Update is called once per frame
	void Update () {
	
	}

    private void OnGUI()
    {
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(m_ipInfo));
        GUI.Label(new Rect(Screen.width - size.x, Screen.height - size.y - 10, size.x, size.y), m_ipInfo);
    }

    /// <summary>
    /// Calls : 
    /// if(host)
    ///     LAN Host
    ///     Client connect
    ///     Server connect
    /// 
    /// if(client)
    ///     OnClientConnect
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        CustomDebug.Log("On Client Connect");
        if(m_onClientConnect != null)
            m_onClientConnect(conn.isConnected); // delegate
    }

    /// <summary>
    /// if(client)
    ///     If host stops then onClientDisconnect gets called
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        CustomDebug.Log("On Client Disconnect");
        if(m_onClientDisconnect != null)
            m_onClientDisconnect(); //delegate
    }

    /// <summary>
    /// if(host)
    ///  - on client connect => calls host's onServerConnect
    ///  
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        CustomDebug.Log("On Server Connect");
        if(m_onServerConnect != null)
            m_onServerConnect(conn.isConnected); // delegate
    }

    /// <summary>
    /// if(host)
    ///  - If client disconnects=> calls host's OnServerDIsconnect
    /// </summary>
    /// <param name="conn"></param>
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        CustomDebug.Log("On Server Disconnect");
        if(m_onServerDisconnect != null)
            m_onServerDisconnect(); // delegate
    }

    /// <summary>
    /// it gets called only if LAN has started
    /// </summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        CustomDebug.Log("On Start Host");
        if(m_onStartHost != null)
            m_onStartHost(true);
    }
    
    /// <summary>
    /// On stop host
    /// </summary>
    public override void OnStopHost()
    {
        base.OnStopHost();
        CustomDebug.Log("On Stop Host");
        if(m_onStopHost != null)
            m_onStopHost();
    }

    public void startHost()
    {
        StartHost();
    }

    public void startClient(string ipAddress, int port)
    {
        networkAddress = ipAddress;
        networkPort = port;
        StartClient();
    }

    public void stopHost()
    {
        StopHost();
        //StopClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        CustomDebug.Log("On Server Add New Player : " + conn.connectionId);
        if (m_onServerAddPlayer != null)
            m_onServerAddPlayer(conn);
    }
}