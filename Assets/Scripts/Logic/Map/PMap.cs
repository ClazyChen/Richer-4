using System;
using System.Xml;
using System.Collections.Generic;

public class PMap : PObject, ICloneable {
    public int Length = 0;
    public int Width = 0;
    public int StartPointNumber = 0;
    public List<PBlock> BlockList;
    public List<PPortal> PortalList;
    private PMap() {
        BlockList = new List<PBlock>();
        PortalList = new List<PPortal>();
    }
    /// <summary>
    /// ���ݵ�ͼ�ļ���XML��������ͼ
    /// </summary>
    /// <param name="MapFileName"></param>
    public PMap(string MapFileName) {
        BlockList = new List<PBlock>();
        PortalList = new List<PPortal>();
        PXmlReader Reader = new PXmlReader(MapFileName);
        int BlockCount = 0, PortalCount = 0;
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;
        Reader.OpenNode("MAP");
        #region ɨ���ͼ������Ϣ
        Reader.Process((XmlNode MapNode) => {
            Name = Reader.GetString("NAME");
            Length = Reader.GetInt("LENGTH");
            Width = Reader.GetInt("WIDTH");
        });
        #endregion
        Reader.OpenNode("BLOCK");
        #region ��һ��ɨ�����и��ӣ������������
        Reader.Process((XmlNode BlockNode) => {
            PBlock TempBlock = new PBlock {
                X = Reader.GetInt("X"),
                Y = Reader.GetInt("Y"),
                Name = Reader.GetString("NAME"),
                Index = BlockCount++,
                Price = Reader.GetInt("PRICE"),
                StartPointIndex = Reader.GetInt("STARTPOINT") - 1,
                GetCardPass = Reader.GetInt("GETCARDPASS"),
                GetCardStop = Reader.GetInt("GETCARD"),
                GetMoneyPassPercent = Reader.GetInt("GETMONEYPASSPC"),
                GetMoneyPassSolid = Reader.GetInt("GETMONEYPASS"),
                GetMoneyStopPercent = Reader.GetInt("GETMONEYPC"),
                GetMoneyStopSolid = Reader.GetInt("GETMONEY")
            };
            TempBlock.CanPurchase = TempBlock.Price != 0;
            TempBlock.IsBusinessLand = TempBlock.CanPurchase && Reader.GetInt("BUSINESS") == 1;
            if (TempBlock.StartPointIndex + 1 > StartPointNumber) {
                StartPointNumber = TempBlock.StartPointIndex + 1;
            }
            minX = Math.Min(minX, TempBlock.X);
            maxX = Math.Max(maxX, TempBlock.X);
            minY = Math.Min(minY, TempBlock.Y);
            maxY = Math.Max(maxY, TempBlock.Y);
            BlockList.Add(TempBlock);
        });
        #endregion
        #region �ڶ���ɨ�����и��ӣ�������ת��ϵ
        BlockCount = 0;
        Reader.Process((XmlNode BlockNode) => {
            int BlockIndex = BlockCount++;
            PBlock CurrentBlock = BlockList[BlockIndex];
            #region ������һ��Ĺ�ϵ
            string NextBlockString = Reader.GetString("NEXT");
            if (NextBlockString.Equals(string.Empty)) {
                #region �ɰ���ת�ֶ�
                string[] NextNodeAttribute = { "UP", "DOWN", "LEFT", "RIGHT" };
                foreach (string Attribute in NextNodeAttribute) {
                    PBlock NextBlock = FindBlock(Reader.GetInt(Attribute) - 1);
                    if (NextBlock != null) {
                        CurrentBlock.NextBlockList.Add(NextBlock);
                    }
                }
                #endregion
            } else {
                #region �°���ת�ֶ�
                foreach (string NextBlockID in NextBlockString.Split('-')) {
                    try {
                        PBlock NextBlock = FindBlock(Convert.ToInt32(NextBlockID) - 1);
                        if (NextBlock != null) {
                            CurrentBlock.NextBlockList.Add(NextBlock);
                        }
                    } catch (Exception) {
                        // NEXT�ֶεĸ�ʽ���󣬲����뵽��ת��
                    }
                }
                #endregion
            }
            #endregion
            #region �����Ź�ϵ
            int PortalTargetNumber = Reader.GetInt("MULTIGOTO");
            if (PortalTargetNumber != 0) {
                #region �ɰ���ش������ֶ�
                for (int i = 1; i <= PortalTargetNumber; ++i) {
                    PBlock TargetBlock = FindBlock(Reader.GetInt("MULTIGOTO" + i.ToString()) - 1);
                    if (TargetBlock != null) {
                        CurrentBlock.PortalBlockList.Add(TargetBlock);
                    }
                }
                #endregion
            } else {
                #region �°���ش������ֶ�/�ɰ洫�����ֶ�
                string[] PortalTargets = Reader.GetString("GOTO").Split('-');
                foreach (string PortalTargetID in PortalTargets) {
                    try {
                        PBlock TargetBlock = FindBlock(Convert.ToInt32(PortalTargetID) - 1);
                        if (TargetBlock != null) {
                            CurrentBlock.PortalBlockList.Add(TargetBlock);
                        }
                    } catch (Exception) {
                        // GOTO�ֶεĸ�ʽ���󣬲����뵽��������ת��
                    }
                }
                #endregion
            }
            #endregion
        });
        #endregion
        Reader.CloseNode();
        Reader.OpenNode("BRIDGE");
        #region ɨ�����д�����
        Reader.Process((XmlNode PortalNode) => {
            PPortal TempPortal = new PPortal {
                X = Reader.GetInt("X"),
                Y = Reader.GetInt("Y"),
                Index = PortalCount++
            };
            switch (Reader.GetInt("TYPE")) {
                case 1: TempPortal.RotateAngle = 0; break;
                case 2: TempPortal.RotateAngle = 90; break;
                case 3: TempPortal.RotateAngle = 135; break;
                case 4: TempPortal.RotateAngle = 45; break;
                default: TempPortal.RotateAngle = Reader.GetInt("ANGLE"); break;
            }
            minX = Math.Min(minX, TempPortal.X);
            maxX = Math.Max(maxX, TempPortal.X);
            minY = Math.Min(minY, TempPortal.Y);
            maxY = Math.Max(maxY, TempPortal.Y);
            PortalList.Add(TempPortal);
        });
        #endregion
        #region ��һ������ֵ
        foreach (PBlock Block in BlockList) {
            Block.X -= minX;
            Block.Y -= minY;
        }
        foreach (PPortal Portal in PortalList) {
            Portal.X -= minX;
            Portal.Y -= minY;
        }
        #endregion
        #region �����ͼ�ĳ���
        Length = maxX - minX + 1;
        Width = maxY - minY + 1;
        #endregion
    }
    /// <summary>
    /// ��ȫ�ĸ����±���Ҹ���
    /// </summary>
    /// <param name="Index"></param>
    /// <returns></returns>
    public PBlock FindBlock(int Index) {
        if (Index < 0 || Index >= BlockList.Count) {
            return null;
        } else {
            return BlockList[Index];
        }
    }

    public List<PBlock> FindBlock(PPlayer Lord) {
        return BlockList.FindAll((PBlock Block) => Lord.Equals(Block.Lord));
    }

    public PBlock FindBlockByCoordinate(int x, int y) {
        return BlockList.Find((PBlock Block) => Block.X == x && Block.Y == y);
    }
    public object Clone() {
        #region ����һ����ͼ
        PMap Copy = new PMap() {
            Name = Name,
            Length = Length,
            Width = Width,
            StartPointNumber = StartPointNumber
        };
        #region ���Ƹ��ӣ���������ת��ϵ��
        foreach (PBlock Block in BlockList) {
            Copy.BlockList.Add(new PBlock() {
                Name = Block.Name,
                Index = Block.Index,
                X = Block.X,
                Y = Block.Y,
                StartPointIndex = Block.StartPointIndex,
                GetMoneyPassSolid = Block.GetMoneyPassSolid,
                GetMoneyStopSolid = Block.GetMoneyStopSolid,
                GetMoneyPassPercent = Block.GetMoneyPassPercent,
                GetMoneyStopPercent = Block.GetMoneyStopPercent,
                GetCardPass = Block.GetCardPass,
                GetCardStop = Block.GetCardStop,
                Price = Block.Price,
                CanPurchase = Block.CanPurchase,
                IsBusinessLand = Block.IsBusinessLand,
                BusinessType = Block.BusinessType,
                Lord = Block.Lord,
                HouseNumber = Block.HouseNumber
            });
        }
        #endregion
        #region ���Ƹ��ӵ���ת��ϵ
        foreach (PBlock Block in BlockList) {
            Copy.FindBlock(Block.Index).NextBlockList.AddRange(Block.NextBlockList.ConvertAll(
                (PBlock NextBlock) => Copy.FindBlock(NextBlock.Index)));
            Copy.FindBlock(Block.Index).PortalBlockList.AddRange(Block.PortalBlockList.ConvertAll(
                (PBlock NextBlock) => Copy.FindBlock(NextBlock.Index)));
        }
        #endregion
        #region ���ƴ�����
        foreach (PPortal Portal in PortalList) {
            Copy.PortalList.Add(new PPortal() {
                Index = Portal.Index,
                RotateAngle = Portal.RotateAngle,
                X = Portal.X,
                Y = Portal.Y
            });
        }
        #endregion
        #endregion
        return Copy;
    }

    public PBlock NextStepBlock(PBlock StartPoint, int StepCount) {
        PBlock Answer = StartPoint;
        for (int i =0; i < StepCount; ++ i) {
            Answer = Answer.NextBlock;
        }
        return Answer;
    }
}