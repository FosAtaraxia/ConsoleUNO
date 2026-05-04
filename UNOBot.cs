using System.Collections;
using System.Drawing;
using csharpUNO;

namespace csharpUNO.UNOBot
{
    public class Bot
    {
        public static int GetBotOutput1(int[] lastCard, List<List<int>> playerCard, int combo, int player, int state1, int state2, ref string? strOutput, int[] playerNotSayUNO)
        {
            Random Random = new Random();
            Thread.Sleep(Random.Next(300, 2000));
            switch (state1)
            {
                case 3:
                    {
                        bool isAnyoneNotSayUNO = false;
                        for (int i = 1; i <= playerNotSayUNO.Length; i++)
                        {
                            if (playerNotSayUNO[i - 1] == 1 && i != player)
                            {
                                isAnyoneNotSayUNO = true;
                                break;
                            }
                        }
                        if (isAnyoneNotSayUNO)
                        {
                            int isReportUNO = Random.Next(0, 100);
                            if (isReportUNO <= 70)
                            {
                                strOutput = "r";
                                return 0;
                            }
                        }
                        List<int> tempCardListIndex = [];
                        for (int i = 1; i <= CsharpUNO.SizeRow(playerCard[player * 2 - 2]); i++)
                        {
                            if (CsharpUNO.CanBePlayed(playerCard[player * 2 - 2][i - 1], playerCard[player * 2 - 1][i - 1], lastCard[0], lastCard[1], combo) == 1)
                            {
                                tempCardListIndex.Add(i - 1);
                            }
                        }
                        if (playerCard[player * 2 - 2][1] != 0 && playerCard[player * 2 - 2][2] == 0 && playerNotSayUNO[player - 1] != -1 && combo == 1 && tempCardListIndex.Count != 0)
                        {
                            int isSelfSayUNO = Random.Next(0, 100);
                            if (isSelfSayUNO <= 70)
                            {
                                strOutput = "u";
                                return 0;
                            }
                        }
                        if (tempCardListIndex.Count != 0)
                        {
                            int outputIndex = Random.Next(0, tempCardListIndex.Count);
                            strOutput = Convert.ToString(tempCardListIndex[outputIndex] + 1);
                        }
                        else
                        {
                            strOutput = "0";
                        }
                        return 0;
                    }
                case 6:
                    {
                        List<int> tempColorList = [0, 0, 0, 0, 0];
                        int color = 0;
                        for (int i = 1; i <= CsharpUNO.SizeRow(playerCard[player * 2 - 1]); i++)
                        {
                            color = playerCard[player * 2 - 1][i - 1];
                            if (color >= 1 && color <= 4)
                            {
                                tempColorList[color] += 1;
                            }
                        }
                        tempColorList[0] = tempColorList[2];
                        int whichIsBiggest = 0;
                        for (int i = 1; i <= 4; i++)
                        {
                            if (tempColorList[i - 1] < tempColorList[i]) whichIsBiggest = i;
                        }
                        if (whichIsBiggest != 0) return whichIsBiggest;
                        else return Random.Next(1, 5);
                    }
                case 9:
                    {
                        for (int i = 1; i <= playerNotSayUNO.Length; i++)
                        {
                            if (playerNotSayUNO[i - 1] == 1 && i != player)
                            {
                                return i;
                            }
                        }
                        return 0;
                    }
                default: return -10;
            }
        }
        public static int GetBotOutput2(int player, int state1, int state2, int combo)
        {
            switch (state1)
            {
                case 4:
                    {
                        switch (state2)
                        {
                            case 1: return 0;
                            case 2: return 1;
                            default: return -10;
                        }
                    }
                default: return -10;
            }
        }
    }
}