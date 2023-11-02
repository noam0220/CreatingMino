using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMinoAnswer {
    public class MinoList {
        //各ブロック数のミノのパターンを保持するクラス。
        //BlockCountにはブロック数が、MinoCollectionには、Minoのデータが入る。
        public int BlockCount { get; }
        public List<Mino> MinoCollection { get; }

        public MinoList(int blockCount, List<Mino> minoCollection) {
            BlockCount = blockCount;
            MinoCollection = minoCollection;
        }

        //ミノを出力する。(N >= 9ではスクロールしきれず全パターンを表示できない。)
        public static void PrintMino(List<MinoList> minoLists, List<int> printFlgs) {

            //選択されたブロック数のminoListだけに絞り込む。
            minoLists = minoLists.Where(ml => printFlgs.Contains(ml.BlockCount)).ToList();

            var sbAllMinos = new StringBuilder();
            foreach(MinoList minoList in minoLists) {
                foreach(var mino in minoList.MinoCollection) {
                    //各ミノの縦×横の長さの長さの、空文字が連続した文字列を生成する。
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
                    sbAllMinos.Append(sbMino.Insert(sbMino.Length - 1, '\n'));
                }
                sbAllMinos.Insert(sbAllMinos.Length - 1, "\nブロック数 " + minoList.BlockCount + " の時 : " + minoList.MinoCollection.Count() + "個\n");
            }
            Console.WriteLine(sbAllMinos.ToString());
            Console.WriteLine();
        }
    }
}
