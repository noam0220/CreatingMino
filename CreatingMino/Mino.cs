using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CreatingMinoQuestion {
    public class Mino {
        //ミノの形状を表現するクラス。Blocksには各ブロックの位置をxy平面上の格子点に対応付け、その座標(y座標, x座標)が入る。
        //原点の座標は(0, 0)とし、xは正の方向に、yは負の方向に進む。
        //Heightにはその形状の縦の長さ、Widthには横の長さが入る。
        //
        // 　0  1  … x
        //0　■ ■
        //1　■ 　
        //2　■
        //:
        //y
        //このような形状のデータは、[ {"Blocks" : "{0, 0}, {0, 1}, {1, 0}, {2, 0}", "Height" : "3", "Width" : "2"} ]となる。
        public List<int[]> Blocks { get; }
        public int Height => CalcHeight();
        public int Width => CalcWidth();

        public Mino(List<int[]> blocks) {
            Blocks = blocks;
        }

        private int CalcHeight() {
            return Blocks.Max(y => y[0]) - Blocks.Min(y => y[0]) + 1;
        }

        private int CalcWidth() {
            return Blocks.Max(x => x[1]) - Blocks.Min(x => x[1]) + 1;
        }

        public int[] CalcSize() {
            return new int[] { Height, Width };
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
        //【問題2】処理を高速化しよう。
        // ヒント : sbMinoの初期化時に、何かを代入しよう。
        // 各Blockの値を利用して、Blockの個数だけ処理が走るようにしよう。
        public static void PrintMino(List<Mino> minoList) {

            var sbAllMinos = new StringBuilder();
            var tmpMino = new int[2];

            foreach(var mino in minoList) {

                //各ミノの縦×横の長さの、空文字が連続した文字列を生成する。
                var sbMino = new StringBuilder();

                //全座標においてmino.Blocksの中に含まれているかを探索し、あれば'■'、なければ'　'を代入する。
                //各行末ごとに改行する。
                for(var i = 0; i < mino.Height; i++) {
                    for (var j = 0; j < mino.Width + 1; j++){
                        var blockFlg = false;
                        if(j != mino.Width) {
                            (tmpMino[0], tmpMino[1]) = (i, j);
                            foreach(var block in mino.Blocks) {
                                if(block.SequenceEqual(tmpMino)) {
                                    sbMino.Append('■');
                                    blockFlg = true;
                                    break;
                                }
                            }
                            if(!blockFlg) { sbMino.Append('　'); }
                        } else {
                            sbMino.Append('\n');
                        }
                    }
                }
                sbAllMinos.Append(sbMino.Append("\n"));
            }
            sbAllMinos.Append("ブロック数 " + minoList.Last().Blocks.Count() + " の時 : " + minoList.Count() + "個\n");
            Console.WriteLine(sbAllMinos.ToString());
        }
    }
}
