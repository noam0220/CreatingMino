using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CreatingMino {
    internal class Program {
        //セルがN個のテトリミノを全て求める。
        //縦が長くなるように出力する。
        //180°回転させたときに等しくなるミノ同士は同一のミノとみなし、どちらか一つのみを出力する。
        //　
        //　■　■ ■　■ ■　■ 　　　 ■　■ ■　■ 
        //　■　■ 　　　 ■　■ ■　■ ■　■ ■　■ ■
        //　■　■ 　　　 ■　■ 　　■　　　　　　■ 
        //　■、　　、　　、　　、　　、　　、
        //　(出力例)

        //今回の使用セル数
        const int N = 4;

        //
        //　■　■■
        //　■　■
        //　■、
        //
        //元となるミノ、縦横サイズ(旧)
        static List<(bool[,], int[])> mino3List = new List<(bool[,], int[])> {
            (new bool[N - 1, N - 1] { { true, false, false }, { true, false, false }, { true, false, false } }, new int[] { 3, 1 }),
            (new bool[N - 1, N - 1] { { false, false, false }, { true, true, false }, { false, true, false } }, new int[] { 2, 2 }),
        };

        //元となるミノ、縦横サイズ
        static MinoList oMinoList = new MinoList(3, new List<Mino> {
            new Mino(new List<int[]> { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 2, 0 } }),
            new Mino(new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 } })
        });

        //結果を保持(旧)
        static List<(bool[,], int[])> mino4List = new List<(bool[,], int[])>();

        //結果を保持
        static MinoList minoList = new MinoList(4, new List<Mino> { });

        //重複が無いか確かめる場所(旧)
        static bool[,] testField = new bool[N + 1, N + 1];

        //サイズの初期値(旧)
        static int[] size = new int[2] { 1, 1 };

        static void Main(string[] args) {

            foreach(var oMino in oMinoList.MinoCollection) {
                var testCells = new List<int[]>();
                var tmpCells  = new List<int[]>();
                //あとで、奴に理由を聞く
                //var tmpCell = new int[2];
                //foreach(var cell in oMino.Cells) {
                //    tmpCell[0] = cell[0] + 1; tmpCell[1] = cell[1] + 1;
                //    testCells.Add(tmpCell);
                //}
                foreach(var cell in oMino.Cells) {
                    testCells.Add(new int[] { cell[0] + 1, cell[1] + 1 });
                    tmpCells .Add(new int[] { cell[0] + 1, cell[1] + 1 });
                }

                var tmpCell = new int[2];
                var directionList = new string[] { "above", "right", "below", "left" };

                foreach(var cell in testCells) {
                    foreach(var direction in directionList) {
                        switch(direction) {
                            case "above": tmpCell[0] = cell[0] - 1; tmpCell[1] = cell[1]; break;
                            case "right": tmpCell[0] = cell[0]; tmpCell[1] = cell[1] + 1; break;
                            case "below": tmpCell[0] = cell[0] + 1; tmpCell[1] = cell[1]; break;
                            case "left" : tmpCell[0] = cell[0]; tmpCell[1] = cell[1] - 1; break;
                        }
                        if(!testCells.Any(c => c.SequenceEqual(tmpCell))) {
                            tmpCells = AddMino(tmpCells, tmpCell);
                        }
                    }
                }
            }

            foreach(var mino in minoList.MinoCollection) {
                //いずれ、セルの順番をイイ感じにするメソッドを追加しないと、ヤバいはず
                //もっといいのを、探そう
                //将来的に、横一列に出力させる？
                int[] tmpCell = new int[2];
                Mino printMino = mino;
                for(int row = 0; row < mino.Height; row++) {
                    for(int col = 0; col < mino.Width; col++) {
                        tmpCell[0] = row; tmpCell[1] = col;
                        if(printMino.Cells.Any(c => c.SequenceEqual(tmpCell))) {
                            Console.Write("■");
                            printMino.Cells.Remove(tmpCell);
                        } else {
                            Console.Write("　");
                        }
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }

            //旧
            foreach(var mino3 in mino3List) {
                for(int row = 0; row < mino3.Item1.GetLength(0); row++) {
                    for(int col = 0; col < mino3.Item1.GetLength(1); col++) {
                        testField[row + 1, col + 1] = mino3.Item1[row, col];
                    }
                }

                for(int row = 1; row < testField.GetLength(0) - 1; row++) {
                    for(int col = 1; col < testField.GetLength(1) - 1; col++) {
                        if(testField[row, col]) {
                            //上に増やす
                            if(!testField[row - 1, col]) {
                                testField[row - 1, col] = true;
                                CheckDuplication(testField, size);
                                testField[row - 1, col] = false;
                            }
                            //右に増やす
                            if(!testField[row, col + 1]) {
                                testField[row, col + 1] = true;
                                CheckDuplication(testField, size);
                                testField[row, col + 1] = false;
                            }
                            //下に増やす
                            if(!testField[row + 1, col]) {
                                testField[row + 1, col] = true;
                                CheckDuplication(testField, size);
                                testField[row + 1, col] = false;
                            }
                            //左に増やす
                            if(!testField[row, col - 1]) {
                                testField[row, col - 1] = true;
                                CheckDuplication(testField, size);
                                testField[row, col - 1] = false;
                            }
                        }
                    }
                }
                Array.Clear(testField, 0, testField.Length);
                size[0] = 1; size[1] = 1;
            }

            //旧
            foreach(var mino4 in mino4List) {
                for(int row = 0; row < mino4.Item1.GetLength(0); row++) {
                    for(int col = 0; col < mino4.Item1.GetLength(1); col++) {
                        if(mino4.Item1[row, col]) {
                            //Console.Write("■");
                        } else {
                            //Console.Write("　");
                        }
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
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

            if(CheckDuplication2(cellsWithNoSpace, height, width)) {
                minoList.MinoCollection.Add(new Mino(cellsWithNoSpace));
            };  
            cells.Remove(tCell);
            return cells;
        }

        private static bool CheckDuplication2(List<int[]> cells, int height, int width) {
            foreach(var mino in minoList.MinoCollection) {
                if(height == mino.Height && width == mino.Width) {
                    //同じかどうか比較
                    bool sameFlg = true;
                    for(int i = 0; i < mino.Cells.Count; i++) {
                        if(!Enumerable.SequenceEqual(cells[i], mino.Cells[i])) { sameFlg = false; }
                    }
                    if(sameFlg) { return false; }
                    if(Enumerable.SequenceEqual(cells, mino.Cells)) { return false; }
                    //180°回転させて、同じかどうか比較
                    var tmpCells = new List<int[]>{ };
                    //ミスあり
                    foreach(var cell in cells) {
                        tmpCells.Add(new int[] { height - cell[0] - 1, width - cell[1] - 1 });
                    }

                    tmpCells = tmpCells.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();
                    sameFlg = true;
                    for(int i = 0; i < mino.Cells.Count; i++){
                        if(!Enumerable.SequenceEqual(tmpCells[i], mino.Cells[i])) { sameFlg = false; }
                    }
                    if(sameFlg) { return false; }
                }
            } 
            return true;
        }

        //旧
        private static void CheckDuplication(bool[,] field, int[] size) {
            List<int[]> cellList = new List<int[]>();

            //縦、横の長さを計測
            for(int row = 0; row < field.GetLength(0); row++) {
                for(int col = 0; col < field.GetLength(1); col++) {
                    if(field[row, col]) {
                        cellList.Add(new int[] { row, col });
                    }
                }
            }
            int xMin = cellList.Min(y => y[1]);
            int yMin = cellList.Min(x => x[0]);

            size[0] = cellList.Max(x => x[0]) - yMin + 1;
            size[1] = cellList.Max(x => x[1]) - xMin + 1;

            if(size[1] > size[0]) {
                bool[,] tmpfield = new bool[N + 1, N + 1];
                cellList.Clear();

                for(int i = 0; i < N + 1; i++) {
                    for(int j = 0; j < N + 1; j++) {
                        tmpfield[i, j] = field[j, N - i];
                        if(tmpfield[i, j]) { cellList.Add(new int[] { i, j }); }
                    }
                }
                field = tmpfield;

                (size[0], size[1]) = (size[1], size[0]);
                xMin = cellList.Min(y => y[1]);
                yMin = cellList.Min(x => x[0]);
            }
            bool[,] testfield2 = new bool[size[0], size[1]];

            foreach(var cell in cellList) {
                testfield2[cell[0] - yMin, cell[1] - xMin] = true;
            }

            foreach(var mino4 in mino4List) {
                //サイズが同じなら、比較開始
                if(size[0] == mino4.Item2[0] && size[1] == mino4.Item2[1]) {
                    //同じかどうか比較
                    bool isSame = true;
                    for(int row = 0; row < testfield2.GetLength(0); row++) {
                        for(int col = 0; col < testfield2.GetLength(1); col++) {
                            if(testfield2[row, col] != mino4.Item1[row, col]) {
                                isSame = false;
                            }
                        }
                    }
                    if(isSame) { return; }
                    //180°回転させて、同じかどうか比較
                    isSame = true;
                    for(int row = 0; row < testfield2.GetLength(0); row++) {
                        for(int col = 0; col < testfield2.GetLength(1); col++) {
                            if(testfield2[row, col] != mino4.Item1[mino4.Item2[0] - (1 + row), mino4.Item2[1] - (1 + col)]) {
                                isSame = false;
                            }
                        }
                    }
                    if(isSame) { return; }
                }
            }
            mino4List.Add((testfield2, new int[] { size[0], size[1] }));
        }
    }
}
