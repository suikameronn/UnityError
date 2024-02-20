using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Unity;
using UnityEngine;

public class GamingClient : MonoBehaviour
{
    // プレイヤーの Transform (今回はメインカメラの Transform を指定)
    [SerializeField]
    Transform m_PlayerTransform;

    // 部屋に参加するときに使用するユーザ名 (何でも設定可)
    [SerializeField]
    string m_UserName;

    // 参加したい部屋のルーム名
    // (StreamingHub クライアント同士で交流したい場合は、
    // 各クライアントで同一の名前を設定する必要がある)
    [SerializeField]
    string m_RoomName;

    // StreamingHub サーバと通信を行うためのクライアント生成
    private GamingHubClient client = new GamingHubClient();

    private ChannelBase channel;

    async void Start()
    {
        channel = GrpcChannel.ForAddress("http://0.0.0.0:5001");

        // ゲーム起動時に設定した部屋名のルームに設定したユーザ名で入室する。
        await this.client.ConnectAsync(this.channel, this.m_RoomName, this.m_UserName);
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレームプレイヤーの位置(Vector3) と回転(Quaternion) を更新し、
        // ルームに入室している他ユーザ全員にブロードキャスト送信する
        client.MoveAsync(m_PlayerTransform.position, m_PlayerTransform.rotation);
    }

    async void OnDestroy()
    {
        // GameClient が破棄される際の StreamingHub クライアント及び gRPC チャネルの解放処理
        await this.client.LeaveAsync();
        await this.client.DisposeAsync();
        await this.channel.ShutdownAsync();
    }
}

