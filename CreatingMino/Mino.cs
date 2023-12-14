using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreatingMinoQuestion {
    public class Mino {
        //ミノの形状を表現するクラス。Blocksには各ブロックの位置をxy平面上の格子点に対応付け、その座標(y座標, x座標)が入る。
        //原点の座標は(0, 0)とし、xは正の方向に、yは負の方向に進む。
        //Heightにはその形状の高さ、Widthには長さが入る。
        //
        // 　0  1  … x
        //0　■ ■
        //1　■ 　
        //2　■
        //:
        //y
        //このような形状のデータは、[ {"Blocks" : "{0, 0}, {0, 1}, {1, 0}, {2, 0}", "Height" : "3", "Width" : "2"} ]となる。
        
        //ブロックの座標
        public List<int[]> Blocks { get; }
        //ミノの高さ
        public int Height => CalcHeight();
        //ミノの長さ
        public int Width => CalcWidth();

        public Mino(List<int[]> blocks) {
            Blocks = blocks;
        }

        //ミノの高さを求める。
        private int CalcHeight() {
            return Blocks.Max(y => y[0]) - Blocks.Min(y => y[0]) + 1;
        }

        //ミノの長さを求める。
        private int CalcWidth() {
            return Blocks.Max(x => x[1]) - Blocks.Min(x => x[1]) + 1;
        }

        //ブロックを追加する方向
        public static Dictionary<String, int[]> directionMap = new Dictionary<String, int[]>() {
            {"above", new int[]{ -1,  0 } },
            {"right", new int[]{  0,  1 } },
            {"below", new int[]{  1,  0 } },
            {"left" , new int[]{  0, -1 } }
        };

        //ブロックを1つ追加する。
        public static int[] GetNewBlock(int[] blocks, string direction) {
            return new int[] { blocks[0] + directionMap[direction][0], blocks[1] + directionMap[direction][1] };
        }

        //ミノを出力する。(N ≥ 9ではスクロールしきれず全パターンを表示できない。)
        //【問題1】処理を高速化しよう。
        // ヒント : (mino.Height * mino.Width)回探索していると時間がかかるので、
        // できればfor文が重なっている構造からは脱却したいですね。
        // mino.Blocksの値を利用してtargetに代入する値を求めると、探索の必要が無くなるかも…？
        public static void PrintMino(List<Mino> minoList) {

            var sbAllMinos = new StringBuilder();

            foreach(var mino in minoList) {

                //各ミノの縦×横の長さの、空文字が連続した文字列を生成する。
                var sbMino = new StringBuilder(new string('　', mino.Height * mino.Width));

                //(i, j)という座標がmino.Blocksの中に存在するかを探索し、あれば'■'を挿入する。
                var target = 0;
                for(var i = 0; i < mino.Height; i++) {
                    for (var j = 0; j < mino.Width; j++) {
                        target = i * mino.Width + j;
                        foreach(var block in mino.Blocks) {
                            if(block[0] == i && block[1] == j) {
                                sbMino.Remove(target, 1).Insert(target, '■');
                                break;
                            }
                        }
                    }
                }
                //各行末ごとに改行する。
                for(var k = mino.Height; k > 0; k--) {
                    target = k * mino.Width;
                    sbMino.Insert(target, "\n");
                }
                sbAllMinos.Append(sbMino.Append("\n"));
            }
            sbAllMinos.Append("ブロック数 " + minoList.Last().Blocks.Count() + " の時 : " + minoList.Count() + "個\n");
            Console.WriteLine(sbAllMinos.ToString());
        }
    }
}
