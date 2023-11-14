using System.Text;

namespace CreatingMinoAnswer {
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
        //このような形状のデータは、[ {"Blocks" : "{0, 0}, {0, 1}, {1, 0}, {2, 0}", "Height" : "3", "Width" : "2"}]となる。
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
        public static int[] AddBlock(int[] blocks, string direction) { 
            return new int[] { blocks[0] + directionMap[direction][0], blocks[1] + directionMap[direction][1] };
        }

        //ミノを出力する。(N >= 9ではスクロールしきれず全パターンを表示できない。)
        public static void PrintMino(List<Mino> minoList, List<int> printFlgs) {
            //選択されたブロック数のminoListだけに絞り込む。
            var printMinoList = minoList.Where(m => printFlgs.Contains(m.Blocks.Count())).ToList();

            //1つ前のminoのブロック数、Minoの総数を保持する。
            var bfrBlockCount = minoList.First().Blocks.Count();
            var minoCount = 0;

            var sbAllMinos = new StringBuilder();
            foreach(Mino mino in printMinoList) {
                //現在のminoのブロック数を保持する。
                var tmpBlockCount = mino.Blocks.Count();

                if(tmpBlockCount != bfrBlockCount) {
                    sbAllMinos.Append("ブロック数 " + bfrBlockCount + " の時 : " + minoCount + "個\n\n");
                    minoCount = 0;
                }
                //各ミノの縦×横の長さの、空文字が連続した文字列を生成する。
                var sbMino = new StringBuilder(new string('　', mino.Height * mino.Width));

                //各ブロックの座標から、何文字目を空文字から「■」に変換するかを求める。
                foreach(var block in mino.Blocks) {
                    var target = block[0] * mino.Width + block[1];
                    sbMino.Remove(target, 1).Insert(target, "■");
                }
                //各行末ごとに改行する。
                for(int i = mino.Height; i > 0; i--) {
                    var target = i * mino.Width;
                    sbMino.Insert(target, "\n");
                }
                sbAllMinos.Append(sbMino.Append("\n"));

                bfrBlockCount = tmpBlockCount;
                minoCount++;
            }
            sbAllMinos.Append("ブロック数 " + bfrBlockCount + " の時 : " + minoCount + "個\n");
            Console.WriteLine(sbAllMinos.ToString());
            Console.WriteLine();
        }
    }
}
