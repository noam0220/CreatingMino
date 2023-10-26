using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
    public sealed class Util {

        //ミノを出力する。
        public static void PrintMino(List<MinoList> minoLists, List<int> printFlgs) {

            minoLists = minoLists.Where(ml => printFlgs.Contains(ml.CellCount)).ToList();

            //遅いアルゴリズムとして問題にしよう。
            //N=6 : 493ms
            foreach(MinoList minoList in minoLists) {
                foreach(var mino in minoList.MinoCollection) {
                    int[] tmpCell = new int[2];
                    Mino printMino = mino;
                    for(int row = 0; row < mino.Height; row++) {
                        for(int col = 0; col < mino.Width; col++) {
                            tmpCell[0] = row; tmpCell[1] = col;
                            if(printMino.Cells.Any(c => c.SequenceEqual(tmpCell))) {
                                Console.Write("■");
                            } else {
                                Console.Write("　");
                            }
                        }
                        Console.Write("\n");
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }

            //N=6 : 33ms
            //var sbAllMinos = new StringBuilder();
            //foreach(MinoList minoList in minoLists) {
            //    foreach(var mino in minoList.MinoCollection) {
            //        var sbMino = new StringBuilder(new string('　', mino.Height * mino.Width));
            //        foreach(var cell in mino.Cells) {
            //            var target = cell[0] * mino.Width + cell[1] + 1;
            //            sbMino.Remove(target - 1, 1).Insert(target - 1, "■");
            //        }
            //        for(int i = mino.Height; i > 0; i--) {
            //            var target = i * mino.Width;
            //            sbMino.Insert(target, "\n");
            //        }
            //        sbAllMinos.Append(sbMino.Insert(sbMino.Length - 1, '\n'));
            //    }
            //    sbAllMinos.Insert(sbAllMinos.Length - 1, '\n');
            //}
            //Console.WriteLine(sbAllMinos.ToString());
        }
    }
}
