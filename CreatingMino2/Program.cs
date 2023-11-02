using System.Diagnostics;
using 

namespace CreatingMino2 {
    internal class Program {
        //セルがN個のテトリミノを全て求める。
        //縦が長くなるように出力する。
        //180°回転させたときに等しくなるミノ同士は同一のミノとみなし、どちらか一つのみを出力する。
        //　
        //　■　　■ ■　　■ ■　　■ 　　　　 ■　　■ ■　　　 ■
        //　■　　■ 　　　　 ■　　■ ■　　■ ■　　■ ■　　■ ■
        //　■　　■ 　　　　 ■　　■ 　　　■　　　　　　　　■ 
        //　■
        //　(出力例, N = 4)

        //各大きさのミノの結果を保持
        static MinoList minoList;

        //全ての結果を保持
        static List<MinoList> minoLists = new List<MinoList> { new MinoList(1, new List<Mino> {
            new Mino (new List<int[]>{ new int[] { 0, 0 } })
        }) };

        //印刷するかどうか決める。
        static List<int> printFlgs = new List<int> { 9 };
        //static List<int> printFlgs = new List<int> { 3, 4 };
        //static List<int> printFlgs = new List<int> { 1, 2, 3, 4, 5, 6 };

        //方向
        static string[] directionList = new string[] { "above", "right", "below", "left" };

        //今回の使用セル数
        const int N = 9;

        //回転して出力させる。
        const int degree = 0;

        static void Main(string[] args) {
            //変数名を考えてもらう。
            Stopwatch sw = Stopwatch.StartNew();

            CreateMino(minoLists.First());

            //回転させて出力させる
            //minoLists = Util.RoteteMino(minoLists, degree);

            Util.PrintMino(minoLists, printFlgs);

            sw.Stop();

            Console.WriteLine("経過時間: " + sw.ElapsedMilliseconds + " ms");

        }
        private static void CreateMino(MinoList oMinoList) {

            nextMinoList = new MinoList(oMinoList.CellCount + 1, new List<Mino> { });

            foreach(var oMino in oMinoList.MinoCollection) {
                var testCells = new List<int[]>();
                //ミスの例
                //var tmpCell = new int[2];
                //foreach(var cell in oMino.Cells) {
                //    tmpCell[0] = cell[0] + 1; tmpCell[1] = cell[1] + 1;
                //    testCells.Add(tmpCell);
                //}

                //セルの外側に、1マス分の空のセルを追加し、枠を作る。
                foreach(var cell in oMino.Cells) {
                    testCells.Add(new int[] { cell[0] + 1, cell[1] + 1 });
                }
                //AddMino()でtestCellsに対して操作すると反復処理が行えなくなるので、
                //中身のみコピーしておく
                var tmpCells = testCells.Select(c => c.ToArray()).ToList();

                //上→右→下→左の順に、各セルの1つ隣に新たなセルを追加できるか確認する。
                var tmpCell = new int[2];
                foreach(var cell in testCells) {
                    foreach(var direction in directionList) {
                        switch(direction) {
                            case "above": tmpCell[0] = cell[0] - 1; tmpCell[1] = cell[1]; break;
                            case "right": tmpCell[0] = cell[0]; tmpCell[1] = cell[1] + 1; break;
                            case "below": tmpCell[0] = cell[0] + 1; tmpCell[1] = cell[1]; break;
                            case "left": tmpCell[0] = cell[0]; tmpCell[1] = cell[1] - 1; break;
                        }
                        if(!testCells.Any(c => c.SequenceEqual(tmpCell))) {
                            tmpCells = AddMino(tmpCells, tmpCell);
                        }
                    }
                }
            }

            minoLists.Add(nextMinoList);

            if(nextMinoList.CellCount < N) { CreateMino(nextMinoList); }
        }

        private static List<int[]> AddMino(List<int[]> cells, int[] tCell) {
            cells.Add(tCell);
            cells = cells.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();

            var cellsWithNoSpace = new List<int[]>();

            int xMin = cells.Min(x => x[1]);
            int yMin = cells.Min(y => y[0]);

            foreach(var cell in cells) {
                cellsWithNoSpace.Add(new int[] { cell[0] - yMin, cell[1] - xMin });
            }

            int width = cellsWithNoSpace.Max(y => y[1]) - cellsWithNoSpace.Min(y => y[1]) + 1;
            int height = cellsWithNoSpace.Max(y => y[0]) - cellsWithNoSpace.Min(y => y[0]) + 1;

            if(height < width) {
                //縦より横の方が長い場合、図形を時計回りに90°回転させる
                var tmpCells = new List<int[]>();
                var tmpCellsWithNoSpace = new List<int[]>();
                foreach(var cell in cellsWithNoSpace) {
                    tmpCells.Add(new int[] { cell[1], height - cell[0] });
                }
                tmpCells = tmpCells.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();

                xMin = tmpCells.Min(x => x[1]);
                yMin = tmpCells.Min(y => y[0]);

                foreach(var cell in tmpCells) {
                    tmpCellsWithNoSpace.Add(new int[] { cell[0] - yMin, cell[1] - xMin });
                }

                cellsWithNoSpace = tmpCellsWithNoSpace;
                (height, width) = (width, height);
            }

            if(CheckNoDuplication(cellsWithNoSpace, height, width)) {
                nextMinoList.MinoCollection.Add(new Mino(cellsWithNoSpace));
            };
            cells.Remove(tCell);
            return cells;
        }

        private static bool CheckNoDuplication(List<int[]> cells, int height, int width) {
            foreach(var mino in nextMinoList.MinoCollection) {
                if(height == mino.Height && width == mino.Width) {
                    //同じかどうか比較
                    var rCells = cells.Select(c => c.ToArray()).ToList();
                    var tmpCells = new List<int[]>();
                    bool sameFlg = true;
                    for(int i = 0; i < 4; i++) {
                        if(i % 2 == 0) {
                            foreach(var rCell in rCells) {
                                tmpCells.Add(new int[] { rCell[1], height - 1 - rCell[0] });
                            }
                        } else {
                            foreach(var rCell in rCells) {
                                tmpCells.Add(new int[] { rCell[1], width - 1 - rCell[0] });
                            }
                        }
                        tmpCells = tmpCells.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();
                        rCells = tmpCells.Select(c => c.ToArray()).ToList();
                        sameFlg = true;
                        for(int j = 0; j < mino.Cells.Count; j++) {
                            if(!Enumerable.SequenceEqual(tmpCells[j], mino.Cells[j])) { sameFlg = false; }
                        }
                        if(sameFlg) { return false; }
                        tmpCells.Clear();
                    }
                    //for(int i = 0; i < mino.Cells.Count; i++) {
                    //    if(!Enumerable.SequenceEqual(cells[i], mino.Cells[i])) { sameFlg = false; }
                    //}
                    //if(sameFlg) { return false; }

                    //本当は、ここに90°回転させて同じかどうかも比較する必要あり

                    //180°回転させて、同じかどうか比較
                    //var tmpCells2 = new List<int[]>{ };
                    //foreach(var cell in cells) {
                    //    tmpCells2.Add(new int[] { height - cell[0] - 1, width - cell[1] - 1 });
                    //}

                    //tmpCells2 = tmpCells2.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();
                    //sameFlg = true;
                    //for(int i = 0; i < mino.Cells.Count; i++){
                    //    if(!Enumerable.SequenceEqual(tmpCells2[i], mino.Cells[i])) { sameFlg = false; }
                    //}
                    //if(sameFlg) { return false; }
                }
            }
            return true;
        }
    }
}
