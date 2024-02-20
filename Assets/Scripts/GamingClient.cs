using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion;
using MagicOnion.Unity;
using UnityEngine;

public class GamingClient : MonoBehaviour
{
    // �v���C���[�� Transform (����̓��C���J������ Transform ���w��)
    [SerializeField]
    Transform m_PlayerTransform;

    // �����ɎQ������Ƃ��Ɏg�p���郆�[�U�� (���ł��ݒ��)
    [SerializeField]
    string m_UserName;

    // �Q�������������̃��[����
    // (StreamingHub �N���C�A���g���m�Ō𗬂������ꍇ�́A
    // �e�N���C�A���g�œ���̖��O��ݒ肷��K�v������)
    [SerializeField]
    string m_RoomName;

    // StreamingHub �T�[�o�ƒʐM���s�����߂̃N���C�A���g����
    private GamingHubClient client = new GamingHubClient();

    private ChannelBase channel;

    async void Start()
    {
        channel = GrpcChannel.ForAddress("http://0.0.0.0:5001");

        // �Q�[���N�����ɐݒ肵���������̃��[���ɐݒ肵�����[�U���œ�������B
        await this.client.ConnectAsync(this.channel, this.m_RoomName, this.m_UserName);
    }

    // Update is called once per frame
    void Update()
    {
        // ���t���[���v���C���[�̈ʒu(Vector3) �Ɖ�](Quaternion) ���X�V���A
        // ���[���ɓ������Ă��鑼���[�U�S���Ƀu���[�h�L���X�g���M����
        client.MoveAsync(m_PlayerTransform.position, m_PlayerTransform.rotation);
    }

    async void OnDestroy()
    {
        // GameClient ���j�������ۂ� StreamingHub �N���C�A���g�y�� gRPC �`���l���̉������
        await this.client.LeaveAsync();
        await this.client.DisposeAsync();
        await this.channel.ShutdownAsync();
    }
}

