using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int seed = 0;
        int getpoint = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            while (true)
            {
                monte();
                System.Console.ReadLine();
            }
        }
        private void monte()
        {//原始モンテカルロ
            Point[] GH = new Point[100];
            int[,] BG = new int[20, 10];
            int score = 0; ;
            Random r = new Random(Environment.TickCount + seed);　//シード値が被らないようにする
            seed++;
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 20; i++)
                {//盤面の作成
                    BG[i, j] = r.Next(3);
                    System.Console.Write(BG[i, j]);
                }
                System.Console.WriteLine();
            }
            do
            {
                int[,] copy = new int[20, 10];
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {//盤面行列の作成
                        copy[i, j] = BG[i, j];
                    }
                }
                GH = Ghs(copy);
                if (GH.Length != 0)
                {
                    double maxsyoritu = -1;
                    Point Nextt = new Point(-1, -1);
                    int result = 0;
                    for (int i = 0; i < GH.Length; i++)
                    {
                        int X = GH[i].X;
                        int Y = GH[i].Y;
                        result = 0;
                        for (int j = 0; j < 10; j++)
                        {
                            result += playout(X, Y, BG);
                            for (int n = 0; n < 20; n++)
                            {
                                for (int m = 0; m < 10; m++)
                                {//盤面行列の作成
                                    BG[n, m] = copy[n, m];
                                }
                            }
                        }
                        double Syoritu = 100 * (result / 10.0);  //分母は試行回数　 単位は%
                        if (maxsyoritu < Syoritu)
                        {
                            Nextt.X = GH[i].X;
                            Nextt.Y = GH[i].Y;
                            maxsyoritu = Syoritu;
                        }
                    }
                    System.Console.WriteLine("勝率最大は(" + Nextt.X + "," + Nextt.Y + ")");
                    BG = play(Nextt, BG);
                    score += getpoint * getpoint;
                    System.Console.WriteLine();
                    for (int j = 0; j < 10; j++)
                    {
                        for (int i = 0; i < 20; i++)
                        {//盤面行列の作成
                            System.Console.Write(BG[i, j]);
                        }
                        System.Console.WriteLine();
                    }
                }
            } while (GH.Length != 0);
            System.Console.WriteLine("最終的な得点：" + score);
        }

        private int playout(int x, int y, int[,] BG)
        {//int は仮　数値で結果を返す場合　０は勝ち　１は負け
            Point kesu = new Point(x, y);
            int rns = 0;
            Point[] GH = new Point[100]; //盤面が１０×２０ならペアの最大値は１００
            Random r = new Random(Environment.TickCount+seed);  //シード値が被らないようにする
            seed++;
            do
            {
                BG = play(kesu, BG); //ＢＧからｘ、ｙで消す　必ず合法手を入れること
                GH = Ghs(BG);//合法手を返す 実装済
                if (GH.Length != 0)
                {
                    rns = r.Next(GH.Length);
                    kesu = GH[rns];    //   次に消す手をランダムに決定する
                }
            } while (GH.Length != 0);
            return SyoHi(BG);　//実装済
        }

        private Point[] Ghs(int[,] BG)
        {
            Point[] GH = new Point[100];
            for (int i = 0; i < GH.Length - 1; i++)
            {
                GH[i] = new Point(-1, -1);  //最初は-1を入れる
            }
            int[,] copy = new int[BG.GetLength(0), BG.GetLength(1)]; //Ghsを実行するとBGまで変更されてしまうので
            for (int i = 0; i < copy.GetLength(0); i++)
            {
                for (int j = 0; j < copy.GetLength(1); j++)
                {//盤面行列の作成
                    copy[i, j] = BG[i, j];
                }
            }
            int counta = 0;
            for (int i = 0; i < copy.GetLength(0); i++)
            {
                for (int j = 0; j < copy.GetLength(1); j++)
                {   //盤面全体が探索の対象
                    Search(ref counta, i, j, BG[i, j], ref copy, ref GH);
                    if (GH[counta].X != -1)  //値が入っていたら次に行く
                    {
                        counta++;
                    }
                }
            }
            Array.Resize(ref GH, counta);
            return GH;
        }

        private void Search(ref int counta, int i, int j, int colorNo, ref int[,] BG, ref Point[] GH)
        {
            if (BG[i, j] != 9) //9とは黒のこと
            {
                colorNo = BG[i, j];
                BG[i, j] = 9;
                if ((GH[counta].X == -1) &&(( (i != 0) && (colorNo == BG[i - 1, j]) ) ||( (j != 0) && (colorNo == BG[i, j - 1]) ) ||( (i != BG.GetLength(0) - 1) && (colorNo == BG[i + 1, j]) ) ||( (j != BG.GetLength(1) - 1) && (colorNo == BG[i, j + 1]) )))//何も代入されていない場合
                {
                    GH[counta] = new Point(i, j);
                }
                if ((i != 0) && (colorNo == BG[i - 1, j]))
                {
                    Search(ref counta, i - 1, j, colorNo, ref BG, ref GH);
                }
                if ((j != 0) && (colorNo == BG[i, j - 1]))
                {
                    Search(ref counta, i, j - 1, colorNo, ref BG, ref GH);
                }
                if ((i != BG.GetLength(0) - 1) && (colorNo == BG[i + 1, j]))
                {
                    Search(ref counta, i + 1, j, colorNo, ref BG, ref GH);
                }
                if ((j != BG.GetLength(1) - 1) && (colorNo == BG[i, j + 1]))
                {
                    Search(ref counta, i, j + 1, colorNo, ref BG, ref GH);
                }
            }
        }

        private int SyoHi(int[,] BG)
        {//プレイアウトのreturnに合わせた値を返すようにする
            int counta = 0;
            for (int i = 0; i < BG.GetLength(0); i++)
            {
                for (int j = 0; j < BG.GetLength(1); j++)
                {
                    if (BG[i, j] != 9)
                    {
                        counta++;
                    }
                }
            }
            if (counta < 1) //１は適当
            {
                return 1;  //１で勝利
            }
            else
            {
                return 0;  //０で敗北
            }
        }

        private int[,] play(Point P, int[,] BG)
        {
            //必ず合法手が入る
            getpoint = 0;
            Boolean[,] chack = new Boolean[BG.GetLength(0), BG.GetLength(1)];
            Search2(P.X, P.Y, BG[P.X, P.Y], ref BG, ref chack);
            //穴あきの状態なので整理する
            for (int g = 0; g < BG.GetLength(1); g++)
            {//10回繰り返す
                for (int i = 0; i < BG.GetLength(0); i++)
                {
                    for (int j = BG.GetLength(1) - 1; j > 0; j--)
                    {//下から上に
                        if (BG[i, j] == 9)
                        {//穴を見つけた場合
                            Sort1(i, j, ref BG);
                        }
                    }
                }
            }
            for (int i = BG.GetLength(0) - 1; i > -1; i--)
            {
                if (BG[i, BG.GetLength(1) - 1] == 9)
                {//一番下のブロックが無い場合
                    for (int j = 0; j < BG.GetLength(1); j++)
                    {
                        Sort2(i, j, ref BG);
                    }
                }
            }
            return BG;
        }

        private void Sort1(int i, int j, ref int[,] BG)
        {//縦方向のソート
            var tmp = BG[i, j - 1];
            BG[i, j - 1] = BG[i, j];
            BG[i, j] = tmp;
        }

        private void Sort2(int i, int j, ref int[,] BG)
        {//横方向のソート
            if (i < BG.GetLength(0) - 1)
            {
                var tmp = BG[i, j];

                BG[i, j] = BG[i + 1, j];

                BG[i + 1, j] = tmp;

                Sort2(i + 1, j,ref BG);
            }
        }

        private void Search2(int i, int j, int c, ref int[,] BG, ref Boolean[,] chack)
        {//選択した合法手で盤面を消す
            if (chack[i, j] != true)
            {
                chack[i, j] = true;
                BG[i, j] = 9;
                getpoint++; //消した数を数える
                if ((i != 0) && (BG[i - 1, j] == c))
                {
                    Search2(i - 1, j, c, ref BG, ref chack);
                }
                if ((j != 0) && (BG[i, j - 1] == c))
                {
                    Search2(i, j - 1, c, ref BG, ref chack);
                }
                if ((i != BG.GetLength(0) - 1) && (BG[i + 1, j] == c))
                {
                    Search2(i + 1, j, c, ref BG, ref chack);
                }
                if ((j != BG.GetLength(1) - 1) && (BG[i, j + 1] == c))
                {
                    Search2(i, j + 1, c, ref BG, ref chack);
                }
            }
        }
    }
}

