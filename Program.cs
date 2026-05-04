using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.AccessControl;
using System.Xml;
using csharpUNO.UNOBot;
namespace csharpUNO
{
    class CsharpUNO
    {
        static int funcId = 0, funcStart = 0, funcValue = 0, playerCount = 4, turn = 0, currentPlayer = 0, plusCombo = 1, blackCardType = 0, playerCharacter = 1;
        static bool publicOutput = false, allowBot = true;
        static List<List<int>> cardList = [], cardOut = [], playerCard = [];
        static List<int> playerTurn = [], rankList = [];
        static int[] lastCard = new int[] { 0, 0 };
        static int[] playerNotSayUNO = new int[playerCount];
        static void Main()
        {
            StartFunc(1, 1, -10);
            while (true)
            {
                if (funcId == 1)
                {
                    if (funcStart == 1)
                    {
                        Console.WriteLine("ConsoleUNO C# Version");
                        Console.WriteLine("<1>开始 <2>退出");
                        funcStart = 0;
                    }
                    funcValue = GetInput();
                    if (funcValue == 1)
                    {
                        Console.WriteLine("正在洗牌...");
                        CreateCards();
                        for (int i = 1; i <= playerCount * 2; i++)
                        {
                            playerCard.Add([]);
                            for (int j = 1; j <= 108; j++)
                            {
                                playerCard[i - 1].Add(0);  //不占内存，随便嗯造
                            }
                        }
                        GiveCards(5, playerCount);
                        int k = SizeRow(cardList[1]) - 1;
                        while (cardList[1][k] == 5)
                        {
                            k -= 1;
                        }
                        cardOut[0][0] = cardList[0][k];
                        cardOut[1][0] = cardList[1][k];
                        lastCard[0] = cardOut[0][0];
                        lastCard[1] = cardOut[1][0];
                        cardList[0][k] = 0;
                        cardList[1][k] = 0;
                        cardList[0].Remove(0);
                        cardList[1].Remove(0);
                        cardList[0].Add(0);
                        cardList[1].Add(0);
                        Console.WriteLine("第一张牌是：" + GetCardName(lastCard[0], lastCard[1]));
                        //printallcards();
                        Random Random = new Random();
                        turn = Random.Next(1, playerCount + 1);
                        for (int i = turn; i <= playerCount; i++) playerTurn.Add(i);
                        for (int i = 1; i < turn; i++) playerTurn.Add(i);
                        turn = 1;
                        currentPlayer = playerTurn[turn - 1];
                        for (int i = 1; i <= playerNotSayUNO.Length; i++) playerNotSayUNO[i - 1] = 0;
                        StartFunc(2, 0, -10);
                    }
                    if (funcValue == 2) break;
                }
                if (playerCard[currentPlayer * 2 - 2][0] == 0)
                {
                    Console.WriteLine(GetPlayerName(currentPlayer) + "获得胜利！");
                    rankList.Add(currentPlayer);
                    playerTurn.Remove(currentPlayer);
                    playerNotSayUNO[currentPlayer - 1] = 0;
                    if (turn == playerCount) turn = 1;
                    playerCount -= 1;
                    currentPlayer = playerTurn[turn - 1];
                    funcStart = 0;
                    if (playerCount == 1)
                    {
                        rankList.Add(currentPlayer);
                        StartFunc(7, 0, -10);
                    }
                }
                if (playerCard[currentPlayer * 2 - 2][0] != 0 && playerCard[currentPlayer * 2 - 2][1] == 0)
                {
                    playerNotSayUNO[currentPlayer - 1] += 1;
                }
                if (funcId == 2)
                {
                    if (funcStart == 1)
                    {
                        if (turn == playerCount) turn = 1; else turn++;
                        currentPlayer = playerTurn[turn - 1];
                        funcStart = 0;
                    }
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("现在是" + GetPlayerName(currentPlayer) + "的回合！");
                    StartFunc(3, 1, -10);
                }
                if (funcId == 3)
                {
                    if (funcStart == 1)//切换人机出牌
                    {
                        if (publicOutput || currentPlayer == playerCharacter)
                        {
                            Console.WriteLine("选择你的手牌：");
                            GetCardList(currentPlayer);
                            Console.WriteLine("<0>从牌组中抽" + plusCombo + "张");
                            Console.WriteLine(GetDispInfo());
                        }
                        //string test = "";
                        //for (int i = 1;i <= 4;i++) test += playerNotSayUNO[i - 1] + ",";
                        //Console.WriteLine(test);
                        funcStart = 0;
                    }
                    string? tempValue = "";
                    if (currentPlayer == playerCharacter) tempValue = Console.ReadLine();
                    else
                    {
                        if (allowBot) funcValue = Bot.GetBotOutput1(lastCard, playerCard, plusCombo, currentPlayer, 3, 1, ref tempValue, playerNotSayUNO);
                        else tempValue = Console.ReadLine();
                    }
                    if (int.TryParse(tempValue, out funcValue) == false)
                    {
                        StartFunc(3, 0, -9);
                        if (tempValue == "d") StartFunc(99, 1, -10);
                        if (tempValue == "u") StartFunc(8, 0, -10);
                        if (tempValue == "r") StartFunc(9, 1, -10);
                    }
                    if (funcValue >= 0 && funcValue <= SizeRow(playerCard[currentPlayer * 2 - 2]))
                    {
                        int currentType, currentColor;
                        if (funcValue == 0) StartFunc(4, 1, -10);
                        if (funcValue != -10)
                        {
                            currentType = playerCard[currentPlayer * 2 - 2][funcValue - 1];
                            currentColor = playerCard[currentPlayer * 2 - 1][funcValue - 1];
                            if (currentColor == 5)
                            {
                                if (CanBePlayed(currentType, currentColor, lastCard[0], lastCard[1], plusCombo) == 1)
                                {
                                    blackCardType = currentType;
                                    playerCard[currentPlayer * 2 - 2][funcValue - 1] = 0;
                                    playerCard[currentPlayer * 2 - 1][funcValue - 1] = 0;
                                    RemoveZero(currentPlayer);
                                    StartFunc(6, 1, -10);
                                }
                                else
                                {
                                    Console.WriteLine(GetErrorReport(3));
                                    funcValue = -10;
                                }
                            }
                        }
                        if (funcValue >= 1)
                        {
                            currentType = playerCard[currentPlayer * 2 - 2][funcValue - 1];
                            currentColor = playerCard[currentPlayer * 2 - 1][funcValue - 1]; //没与84 85重复
                            if (CanBePlayed(currentType, currentColor, lastCard[0], lastCard[1], plusCombo) == 1)
                            {
                                PlayCard(funcValue, currentPlayer);
                                StartFunc(5, 0, funcValue);
                            }
                            else if (plusCombo == 1) Console.WriteLine(GetErrorReport(2));
                            else Console.WriteLine(GetErrorReport(3));
                        }
                        else
                        {
                            if (funcValue >= 0)
                            {
                                Console.WriteLine("无效输入");
                                //funcValue = -10; 可能多余
                            }
                        }
                    }
                    else
                    {
                        if (funcValue != -10) Console.WriteLine(GetErrorReport(1));
                    }
                }
                if (funcId == 4)
                {
                    int latestType = 0, latestColor = 0;
                    int botState2 = 0;
                    if (funcStart == 1)
                    {
                        if (SizeRow(cardList[0]) < plusCombo) RefreshCardList();
                        for (int i = 1; i <= plusCombo; i++) AddCard(currentPlayer);
                        if (SizeRow(cardList[0]) == 0) RefreshCardList();
                        if (publicOutput || currentPlayer == playerCharacter)
                        {
                            string drawOutput = "你抽出的牌是：";
                            for (int i = SizeRow(playerCard[currentPlayer * 2 - 2]) - plusCombo; i <= SizeRow(playerCard[currentPlayer * 2 - 2]) - 1; i++)
                            {
                                drawOutput += GetCardName(playerCard[currentPlayer * 2 - 2][i], playerCard[currentPlayer * 2 - 1][i]) + " ";
                            }
                            Console.WriteLine(drawOutput);
                        }
                        else Console.WriteLine(GetPlayerName(currentPlayer) + "抽了" + plusCombo + "张牌！" + GetLeftCardNumber(currentPlayer));
                        latestType = playerCard[currentPlayer * 2 - 2][SizeRow(playerCard[currentPlayer * 2 - 2]) - 1];
                        latestColor = playerCard[currentPlayer * 2 - 1][SizeRow(playerCard[currentPlayer * 2 - 1]) - 1];
                        if (CanBePlayed(latestType, latestColor, lastCard[0], lastCard[1], plusCombo) == 1 && plusCombo == 1)
                        {
                            if (publicOutput || currentPlayer == playerCharacter)
                            {
                                if (playerCard[currentPlayer * 2 - 2][1] != 0 && playerCard[currentPlayer * 2 - 2][2] == 0)
                                {
                                    Console.WriteLine("<0>打出(并喊UNO) <1>继续");
                                }
                                else Console.WriteLine("<0>打出 <1>继续");
                            }
                            if (allowBot) botState2 = 1;
                        }
                        else
                        {
                            if (publicOutput || currentPlayer == playerCharacter)
                            {
                                Console.WriteLine("<1>继续");
                            }
                            if (allowBot) botState2 = 2;
                            plusCombo = 1;
                        }
                        funcStart = 0;
                    }
                    if (currentPlayer == playerCharacter) funcValue = GetInput();
                    else
                    {
                        if (allowBot) funcValue = Bot.GetBotOutput2(currentPlayer, 4, botState2, plusCombo);
                        else funcValue = GetInput();
                    }
                    if (funcValue == 0 && CanBePlayed(latestType, latestColor, lastCard[0], lastCard[1], plusCombo) == 1)
                    {
                        if (playerCard[currentPlayer * 2 - 2][1] != 0 && playerCard[currentPlayer * 2 - 2][2] == 0)
                        {
                            Console.WriteLine(GetPlayerName(currentPlayer) + "大喊：UNO！");
                            playerNotSayUNO[currentPlayer - 1] = -1;
                        }
                        if (latestColor != 5)
                        {
                            PlayCard(SizeRow(playerCard[currentPlayer * 2 - 2]), currentPlayer);
                            StartFunc(5, 0, funcValue);
                        }
                        else
                        {
                            blackCardType = latestType;
                            playerCard[currentPlayer * 2 - 2][SizeRow(playerCard[currentPlayer * 2 - 2]) - 1] = 0;
                            playerCard[currentPlayer * 2 - 1][SizeRow(playerCard[currentPlayer * 2 - 1]) - 1] = 0;
                            StartFunc(6, 1, -10);
                        }
                    }
                    else if (funcValue == 1) StartFunc(2, 1, -10);
                    else Console.WriteLine(GetErrorReport(4));
                }
                if (funcId == 5)
                {
                    switch (lastCard[0])
                    {
                        case 11:
                            {
                                playerTurn.Reverse();
                                if (playerCount >= 3) turn = playerCount + 1 - turn;
                                break;
                            }
                        case 12:
                            {
                                if (turn == playerCount) turn = 1; else turn++;
                                break;
                            }
                        case 13:
                            {
                                if (plusCombo == 1) plusCombo = 2; else plusCombo += 2;
                                break;
                            }
                    }
                    StartFunc(2, 1, -10);
                }
                if (funcId == 6)
                {
                    if (funcStart == 1)
                    {
                        if (publicOutput || currentPlayer == playerCharacter)
                        {
                            Console.WriteLine("选择颜色：<1>红 <2>绿 <3>蓝 <4>黄");
                        }
                        funcStart = 0;
                    }
                    string? blank = "";
                    if (currentPlayer == playerCharacter) funcValue = GetInput();
                    else
                    {
                        if (allowBot) funcValue = Bot.GetBotOutput1(lastCard, playerCard, plusCombo, currentPlayer, 6, 1, ref blank, playerNotSayUNO);
                        else funcValue = GetInput();
                    }
                    if (funcValue > 0 && funcValue <= 4)
                    {
                        playerCard[currentPlayer * 2 - 2][SizeRow(playerCard[currentPlayer * 2 - 2])] = blackCardType;
                        playerCard[currentPlayer * 2 - 1][SizeRow(playerCard[currentPlayer * 2 - 1])] = funcValue;
                        PlayCard(SizeRow(playerCard[currentPlayer * 2 - 2]), currentPlayer);
                        if (blackCardType == 14)
                        {
                            if (plusCombo == 1) plusCombo = 4; else plusCombo += 4;
                        }
                        StartFunc(2, 1, -1);
                    }
                    else Console.WriteLine(GetErrorReport(4));
                }
                if (funcId == 7)
                {
                    Console.WriteLine("游戏结束！");
                    Console.WriteLine("排行榜：");
                    for (int i = 1; i <= rankList.Count; i++) Console.WriteLine(i + "." + GetPlayerName(rankList[i - 1]));
                    return;
                }
                if (funcId == 8)
                {
                    Console.WriteLine(GetPlayerName(currentPlayer) + "大喊：UNO！");
                    playerNotSayUNO[currentPlayer - 1] = -1;
                    StartFunc(3, 0, -10);
                }
                if (funcId == 9)
                {
                    if (funcStart == 1)
                    {
                        if (publicOutput || currentPlayer == playerCharacter)
                        {
                            string output = "请选择玩家：";
                            for (int i = 1; i <= playerCount; i++)
                            {
                                output += "<" + playerTurn[i - 1] + ">" + GetPlayerName(playerTurn[i - 1]) + " ";
                            }
                            Console.WriteLine(output);
                            Console.WriteLine("<0>返回");
                        }
                        funcStart = 0;
                    }
                    string? blank = "";
                    if (currentPlayer == playerCharacter) funcValue = GetInput();
                    else
                    {
                        if (allowBot) funcValue = Bot.GetBotOutput1(lastCard, playerCard, plusCombo, currentPlayer, 9, 1, ref blank, playerNotSayUNO);
                        else funcValue = GetInput();
                    }
                    if (funcValue >= 1)
                    {
                        if (playerNotSayUNO[funcValue - 1] >= 1)
                        {
                            Console.WriteLine(GetPlayerName(currentPlayer) + "发现" + GetPlayerName(funcValue) + "没喊UNO，罚抽2张");
                            for (int i = 1; i <= 2; i++) AddCard(funcValue);
                            playerNotSayUNO[funcValue - 1] = 0;
                        }
                        else
                        {
                            Console.WriteLine(GetErrorReport(5));
                        }
                    }
                    else if (funcValue != 0) Console.WriteLine(GetErrorReport(4));
                    StartFunc(3, 0, -10);
                }
                if (funcId == 99)
                {
                    if (funcStart == 1)
                    {
                        Console.WriteLine("调试模式");
                        Console.WriteLine("<0>返回 <1>添加手牌 <2>切换玩家 <3>切换输出 <3>启用/禁用Bot");
                    }
                    funcValue = GetInput();
                    switch (funcValue)
                    {
                        case 0:
                            {
                                StartFunc(3, 1, -10);
                                break;
                            }
                        case 1:
                            {
                                Console.WriteLine("类型:");
                                playerCard[currentPlayer * 2 - 2][SizeRow(playerCard[currentPlayer * 2 - 2])] = GetInput();
                                Console.WriteLine("颜色:");
                                playerCard[currentPlayer * 2 - 1][SizeRow(playerCard[currentPlayer * 2 - 1])] = GetInput();
                                StartFunc(99, 1, -10);
                                break;
                            }
                        case 2:
                            {
                                turn = GetInput();
                                StartFunc(2, 0, -10);
                                break;
                            }
                        case 3:
                            {
                                publicOutput = !publicOutput;
                                break;
                            }
                        case 4:
                            {
                                allowBot = !allowBot;
                                break;
                            }
                    }
                }
            }
        }

        static void StartFunc(int id, int firstStart, int value)
        {
            funcId = id;
            funcStart = firstStart;
            funcValue = value;
        }
        static void CreateCards()
        {
            int i, j, k;
            cardList.Add([]);
            cardList.Add([]);
            for (i = 1; i <= 8; i++)
            {
                for (j = 1; j <= 9; j++)
                {
                    cardList[0].Add(j);
                }
            }
            for (i = 1; i <= 4; i++)
            {
                cardList[0].Add(10);
            }
            for (i = 11; i <= 13; i++)
            {
                for (j = 1; j <= 8; j++)
                {
                    cardList[0].Add(i);
                }
            }
            for (i = 14; i <= 15; i++)
            {
                for (j = 1; j <= 4; j++)
                {
                    cardList[0].Add(i);
                }
            }
            for (i = 1; i <= 4; i++)
            {
                for (j = 1; j <= 18; j++)
                {
                    cardList[1].Add(i);
                }
            }
            for (i = 1; i <= 4; i++)
            {
                cardList[1].Add(i);
            }
            for (i = 1; i <= 3; i++)
            {
                for (j = 1; j <= 4; j++)
                {
                    for (k = 1; k <= 2; k++)
                    {
                        cardList[1].Add(j);
                    }
                }
            }
            for (i = 1; i <= 8; i++)
            {
                cardList[1].Add(5);
            }
            cardOut.Add([]);
            cardOut.Add([]);
            for (i = 0; i <= 107; i++)
            {
                cardOut[0].Add(cardList[0][i]);
                cardOut[1].Add(cardList[1][i]);
            }
            for (i = 0; i <= 107; i++)
            {
                cardList[0][i] = 0;
                cardList[1][i] = 0;
            }
            Random Random = new Random();
            i = 0;
            while (i <= 107)
            {
                j = Random.Next(0, 108);
                if (cardList[0][j] == 0)
                {
                    cardList[0][j] = cardOut[0][i];
                    cardList[1][j] = cardOut[1][i];
                    cardOut[0][i] = 0;
                    cardOut[1][i] = 0;
                    i++;
                }
            }
        }
        public static int SizeRow(List<int> srList)//返回元素个数
        {
            int i = 0;
            while (true)
            {
                if (i == srList.Count) break;
                if (srList[i] == 0) break;
                i++;
            }
            return i;
        }
        static void AddCard(int player)
        {
            if (SizeRow(cardList[0]) != 0)//错误检测：SizeRow为0
            {
                playerCard[player * 2 - 2][SizeRow(playerCard[player * 2 - 2])] = cardList[0][SizeRow(cardList[0]) - 1];
                playerCard[player * 2 - 1][SizeRow(playerCard[player * 2 - 1])] = cardList[1][SizeRow(cardList[1]) - 1];
                cardList[0][SizeRow(cardList[0]) - 1] = 0;
                cardList[1][SizeRow(cardList[1]) - 1] = 0;
            }
        }
        static void GiveCards(int cardCount, int player)
        {
            int i, j;
            for (i = 1; i <= player; i++)
            {
                for (j = 1; j <= cardCount; j++)
                {
                    AddCard(i);
                }
            }
        }
        static string GetCardName(int type, int color)
        {
            string output = "";
            string[] colorName = { "红", "绿", "蓝", "黄", "黑" }, typeName = { "0", "↺", "⊘", "+2", "+4", "◑" };
            output += colorName[color - 1];
            if (type <= 9)
            {
                output += type;
            }
            else
            {
                output += typeName[type - 10];
            }
            return output;
        }
        static string GetPlayerName(int player)
        {
            string[] playerName = { "Atara", "Hikari", "Selantia", "Etis" };
            return playerName[player - 1];
        }
        static void GetCardList(int player)
        {
            string output = "";
            int cardCount = SizeRow(playerCard[player * 2 - 2]), loopCount = cardCount / 7;
            for (int i = 1; i <= loopCount; i++)
            {
                for (int j = 1; j <= 7; j++) output += "<" + ((i - 1) * 7 + j) + ">" + GetCardName(playerCard[player * 2 - 2][(i - 1) * 7 + j - 1], playerCard[player * 2 - 1][(i - 1) * 7 + j - 1]) + " ";
                Console.WriteLine(output);
                output = "";
            }
            if (loopCount * 7 != cardCount)
            {
                for (int i = 1; i <= cardCount - loopCount * 7; i++)
                {
                    output += "<" + (loopCount * 7 + i) + ">" + GetCardName(playerCard[player * 2 - 2][loopCount * 7 + i - 1], playerCard[player * 2 - 1][loopCount * 7 + i - 1]) + " ";
                }
                Console.WriteLine(output);
            }
        }
        static void RemoveZero(int player)
        {
            playerCard[player * 2 - 2].Remove(0);
            playerCard[player * 2 - 1].Remove(0);
            playerCard[player * 2 - 2].Add(0);
            playerCard[player * 2 - 1].Add(0);
        }
        static void PlayCard(int number, int player)
        {
            number--;
            string cardName = GetCardName(playerCard[player * 2 - 2][number], playerCard[player * 2 - 1][number]);
            lastCard[0] = playerCard[player * 2 - 2][number];
            lastCard[1] = playerCard[player * 2 - 1][number];
            cardOut[0][SizeRow(cardOut[0])] = lastCard[0];
            cardOut[1][SizeRow(cardOut[1])] = lastCard[1];
            playerCard[player * 2 - 2][number] = 0;
            playerCard[player * 2 - 1][number] = 0;
            RemoveZero(player);
            Console.WriteLine(GetPlayerName(player) + "打出了" + cardName + "！" + GetLeftCardNumber(player));
        }
        public static int CanBePlayed(int currentType, int currentColor, int lastType, int lastColor, int combo)
        {
            if (combo != 1)
            {
                if ((currentType == lastType && lastType == 13)
                 || (currentType == lastType && lastType == 14)) return 1;
                else return 0;
            }
            else
            {
                if (currentColor == 5) return 1;
                else
                {
                    if (currentColor == lastColor || currentType == lastType) return 1;
                    else return 0;
                }
            }
        }
        static int GetInput()
        {
            string? input;
            int output;
        retryInput:
            input = Console.ReadLine();
            if (int.TryParse(input, out output)) return output;
            else
            {
                //Console.WriteLine("无效输入");
                goto retryInput;
            }
        }
        static string GetDispInfo()
        {
            string output = "上一张牌：";
            output += GetCardName(lastCard[0], lastCard[1]) + " | 玩家回合：";
            for (int i = turn; i <= playerCount; i++)
            {
                output += GetPlayerName(playerTurn[i - 1]) + "->";
            }
            for (int i = 1; i <= turn - 1; i++)
            {
                output += GetPlayerName(playerTurn[i - 1]) + "->";
            }
            output += "... | <u>喊UNO! <r>举报未喊UNO <d>调试模式";
            return output;
        }
        static string GetErrorReport(int errorType)
        {
            Thread.Sleep(500);
            switch (errorType)
            {
                case 1:
                    {
                        return "无效输入：没有这张牌！";
                    }
                case 2:
                    {
                        return "无效输入：颜色或花色不匹配！";
                    }
                case 3:
                    {
                        return "无效输入：请先抽牌！";
                    }
                case 4:
                    {
                        return "无效输入：选项不存在！";
                    }
                case 5:
                    {
                        return "无效输入：不属实！";
                    }
                default:
                    {
                        return "无效输入";
                    }
            }
        }
        static void RefreshCardList()
        {
            Console.WriteLine("牌抽完了！正在重新洗牌...");
            Random Random = new Random();
            int cardOutCount = SizeRow(cardOut[0]);
            int cardListCount = SizeRow(cardList[0]);
            for (int k = 1; k <= cardOutCount; k++)
            {
                if (cardOut[0][k - 1] == 14 || cardOut[0][k - 1] == 15) cardOut[1][k - 1] = 5;
            }
            int i = 1, j = 0;
            while (i <= cardOutCount)
            {
                j = Random.Next(cardListCount + 1, cardOutCount + cardListCount + 1);
                if (cardList[0][j - 1] == 0)
                {
                    cardList[0][j - 1] = cardOut[0][i - 1];
                    cardList[1][j - 1] = cardOut[1][i - 1];
                    cardOut[0][i - 1] = 0;
                    cardOut[1][i - 1] = 0;
                    i++;
                }
            }
        }
        static string GetLeftCardNumber(int player)
        {
            string output = "(剩";
            output += SizeRow(playerCard[player * 2 - 2]) + "张牌)";
            return output;
        }
        static void printallcards()
        {
            for (int i = 0; i <= 107; i++)
            {
                //Console.WriteLine(cardList[0][i] + "," + cardList[1][i]);
            }
            for (int i = 0; i <= 107; i++)
            {
                //Console.WriteLine(cardOut[0][i] + "," + cardOut[1][i]);
            }
            for (int i = 1; i <= playerCount; i++)
            {
                for (int j = 0; j <= SizeRow(playerCard[i * 2 - 2]) - 1; j++)
                {
                    Console.Write(playerCard[i * 2 - 2][j] + ",");
                }
                Console.Write("\n");
                for (int j = 0; j <= SizeRow(playerCard[i * 2 - 1]) - 1; j++)
                {
                    Console.Write(playerCard[i * 2 - 1][j] + ",");
                }
                Console.Write("\n");
            }
        }
    }
}