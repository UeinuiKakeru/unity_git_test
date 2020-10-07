using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonobitEngine;

public class NetworkControl : MonobitEngine.MonoBehaviour
{
    /** ルーム名. */
    private string roomName = "";

    /** プレイヤーキャラクタ. */
    private GameObject playerObject = null;

    /** ウィンドウ表示フラグ. */
    private bool bDisplayWindow = false;

    /** サーバ途中切断フラグ. */
    private static bool bDisconnect = false;

    /** サーバ接続失敗フラグ. */
    private bool bConnectFailed = false;

    /**
     * 途中切断コールバック.
     */
    public void OnDisconnectedFromServer()
    {
        Debug.Log("OnDisconnectedFromServer");
        if (bDisconnect == false)
        {
            bDisconnect = true;
            bDisplayWindow = true;
        }
        else
        {
            bDisconnect = false;
        }
    }

    /**
     * サーバ接続失敗コールバック.
     */
    public void OnConnectToServerFailed(object parameters)
    {
        Debug.Log("OnConnectToServerFailed : StatusCode = " + parameters);
        bConnectFailed = true;
        bDisplayWindow = true;
    }

    /**
     * ウィンドウ表示用メソッド.
     */
    void WindowControl(int windowId)
    {
        // GUIスタイル設定
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        GUIStyleState stylestate = new GUIStyleState();
        stylestate.textColor = Color.white;
        style.normal = stylestate;

        // 途中切断時の表示
        if (bDisconnect)
        {
            GUILayout.Label("途中切断しました。\n再接続しますか？", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("はい", GUILayout.Width(50)))
            {
                // もう一度接続処理を実行する
                MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
                bDisconnect = false;
                bDisplayWindow = false;
            }
            if (GUILayout.Button("いいえ", GUILayout.Width(50)))
            {
                // シーンをリロードし、初期化する
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#else
                Application.LoadLevel(Application.loadedLevelName);
#endif
                // オフラインモードで起動する
                MonobitNetwork.offline = true;
                bConnectFailed = false;
                bDisplayWindow = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }

        // 接続失敗時の表示
        if (bConnectFailed)
        {
            GUILayout.Label("接続に失敗しました。\n再接続しますか？", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("はい", GUILayout.Width(50)))
            {
                // もう一度接続処理を実行する
                MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
                bConnectFailed = false;
                bDisplayWindow = false;
            }
            if (GUILayout.Button("いいえ", GUILayout.Width(50)))
            {
                // オフラインモードで起動する
                MonobitNetwork.offline = true;
                bConnectFailed = false;
                bDisplayWindow = false;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // デフォルトロビーへの自動入室を許可する
        MonobitNetwork.autoJoinLobby = true;

        // MUNサーバに接続する
        MonobitNetwork.ConnectServer("Bearpocalypse_v1.0");
    }

    // Update is called once per frame
    void Update()
    {
        // MUNサーバに接続しており、かつルームに入室している場合
        if(MonobitNetwork.isConnect && MonobitNetwork.inRoom)
        {
            // プレイヤーキャラクタが未搭乗の場合に登場させる
            if ( playerObject == null)
            {
                playerObject = MonobitNetwork.Instantiate(
                                "Player",
                                Vector3.zero,
                                Quaternion.identity,
                                0);
            }
        }
    }

    // OnGUI is called for rendering and handring GUI events
    void OnGUI()
    {
        // サーバ接続状況に応じて、ウィンドウを表示する
        if (bDisplayWindow)
        {
            GUILayout.Window(0, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 40, 200, 80), WindowControl, "Caution");
        }

        // デフォルトのボタンと被らないように、段下げを行う。
        GUILayout.Space(24);

        // MUNサーバに接続している場合
        if(MonobitNetwork.isConnect)
        {
            // ボタン入力でサーバから切断＆シーンリセット
            if(GUILayout.Button("Disconnect", GUILayout.Width(150)))
            {
                // 正常動作のため、bDisconnect を true にして、GUIウィンドウ表示をキャンセルする
                bDisconnect = true;

                // サーバから切断する
                MonobitNetwork.DisconnectServer();

                // シーンをリロードする
#if UNITY_5_3_OR_NEWER || UNITY_5_3
                string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#else
                Application.LoadLevel(Application.loadedLevelName);
#endif
            }

            // ルームに入室している場合
            if (MonobitNetwork.inRoom)
            {
                // ボタン入力でルームから退室
                if(GUILayout.Button("Leave Room", GUILayout.Width(150)))
                {
                    MonobitNetwork.LeaveRoom();
                }
            }

            // ルームに入室していない場合
            if(!MonobitNetwork.inRoom)
            {
                GUILayout.BeginHorizontal();

                // ルーム名の入力
                GUILayout.Label("RoomName : ");
                roomName = GUILayout.TextField(roomName, GUILayout.Width(200));

                // ボタン入力でルーム作成
                if (GUILayout.Button("Create Room", GUILayout.Width(150)))
                {
                    MonobitNetwork.CreateRoom(roomName);
                }

                GUILayout.EndHorizontal();

                // 現在存在するルームからランダムに入室する
                if (GUILayout.Button("Join Random Room", GUILayout.Width(200)))
                {
                    MonobitNetwork.JoinRandomRoom();
                }

                // ルーム一覧から選択式で入室する
                foreach(RoomData room in MonobitNetwork.GetRoomData())
                {
                    string strRoomInfo =
                        string.Format("{0}({1}/{2})",
                                      room.name,
                                      room.playerCount,
                                      (room.maxPlayers == 0 ? "-" : room.maxPlayers.ToString()));

                    if(GUILayout.Button("Enter Room : "  + strRoomInfo))
                    {
                        MonobitNetwork.JoinRoom(room.name);
                    }
                }
            }
        }
    }
}