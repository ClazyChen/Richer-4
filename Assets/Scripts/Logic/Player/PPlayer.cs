public class PPlayer: PObject {
    public class Config {
        public static int DefaultMoney = 30000;
    }

    public int Index;
    public int Money = 0;
    public PSex Sex = PSex.NoSex;
    public bool IsAlive = true;
    public int TeamIndex;
    public PBlock Position;

    public PTagManager Tags;

    /// <summary>
    /// ��ҵ�IP��ַ��ֻ�з��������Ի�ȡ��ȱʡΪ�մ�
    /// </summary>
    public string IPAddress {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.Room.GetPlayer(Index).IPAddress;
            } else {
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// �Ƿ�Ϊ��ʵ��ң�ֻ�з��������Ի�ȡ��ȱʡΪfalse
    /// </summary>
    public bool IsUser {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.Room.GetPlayer(Index).PlayerType.Equals(PPlayerType.Player);
            } else {
                return false;
            }
        }
    }
    public bool IsAI {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return !IsUser;
            } else {
                return false;
            }
        }
    }

    /// <summary>
    /// �ƶ��������棬���������ɻ�ȡ
    /// </summary>
    public int AiCardExpectation {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PAiCardExpectation.Expect(PNetworkManager.NetworkServer.Game, this);
            } else {
                return 0;
            }
        }
    }

    public PPlayerCardArea Area {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.NetworkServer.Game.CardManager.PlayerAreaList[Index];
            } else {
                return null;
            }
        }
    }

    public bool HasHouse {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Server)) {
                return PNetworkManager.Game.Map.BlockList.Exists((PBlock Block ) => Equals(Block.Lord) && Block.HouseNumber > 0);
            } else {
                return false;
            }
        }
    }

    public bool BackFace {
        get {
            return Tags.ExistTag(PTag.BackFaceTag.Name);
        }
    }

    public bool OutOfGame {
        get {
            return Tags.ExistTag(PTag.OutOfGameTag.Name);
        }
    }

    public bool NoLadder {
        get {
            return Tags.ExistTag(PTag.NoLadderTag.Name);
        }
    }

    /// <summary>
    /// �ͻ���ר�ã����ڸ�����������
    /// </summary>
    public int HandCardNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã����ڸ����������
    /// </summary>
    public int NormalLandNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã����ڸ�����ҵ�õ�����
    /// </summary>
    public int BusinessLandNumber = 0;
    /// <summary>
    /// �ͻ���ר�ã��������ñ����
    /// </summary>
    public string MarkString = "|";

    /// <summary>
    /// ��������ר�ã��������ñ����string
    /// </summary>
    /// <returns></returns>
    public string GetMarkString() {
        return Tags.TagString;
    }


    private string _EquipCards = string.Empty;
    public int EquipCardNumber {
        get {
            if (PNetworkManager.CurrentHostType.Equals(PHostType.Client)) {
                return _EquipCards.Length;
            } else {
                return Area.EquipmentCardArea.CardNumber;
            }
        }
    }

}