using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CreatingMinoQuestion {
    internal class Program {
        //ブロックがN個のミノの全ての形状と、その個数を求める。
        //縦が長くなるように出力する。
        //回転させたときに等しくなるミノ同士は同一のミノとみなし、どちらか一つのみを出力する。
        //
        //　■　　■ ■　　■ ■　　■ 　　　　 ■　　■ ■　　■
        //　■　　■ 　　　　 ■　　■ ■　　■ ■　　■ ■　　■ ■
        //　■　　■ 　　　　 ■　　■ 　　　■　　　　　　　　　 ■ 
        //　■
        //
        //　ブロック数 4 の時 : 7個
        //　(出力例, N = 4)

        //ブロック数3のミノ。
        static List<Mino> thridMinoList = new List<Mino> { 
            new Mino(new List<int[]> { new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 2, 0 } }),
            new Mino(new List<int[]> { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 } })
        };

        //結果を保持する。
        static List<Mino> minoList = new List<Mino> { };

        //今回の使用ブロック数。
        const int N = 4;

        static void Main(string[] args) {

            var sw = Stopwatch.StartNew();

            CreateMino(thridMinoList);

            Mino.PrintMino(minoList);

            sw.Stop();

            Console.WriteLine("経過時間: " + sw.ElapsedMilliseconds + " ms");

        }

        //ブロック数がi個の全てのミノの形状と、その個数を求める。
        //【問題3】処理を汎用化しよう。(Nに4以外の自然数も入れられるようにする。)
        // ヒント : まず、N = 1のときのミノの組合わせを考え、
        // thridMinoListのように事前に定義しておこう。
        // そして、ブロック数が(i - 1)個のときのCreateMino()で生成されるMinoのリストを引数として再利用して、
        // ブロック数がi個のときのCreateMino()を実行し…を繰り返し、必要な回数分CreateMino()を呼び出すようにしよう。
        // (再帰的に実装できるとより美しいですね。)
        private static void CreateMino(List<Mino> minoList) {

            foreach(var mino in minoList) {
                var testBlocks = new List<int[]>();

                //ブロックの外側に1マス分の空のブロックを追加し、枠を作る。
                foreach(var block in mino.Blocks) {
                    testBlocks.Add(new int[] { block[0] + 1, block[1] + 1 });
                }

                //AddMino()をtestBlocksに対して行うと反復処理が行えなくなるので、
                //"testBlocks"の中身を"tmpBlocks"にコピーしておく。
                var tmpBlocks = testBlocks.Select(b => b.ToArray()).ToList();
                var newBlock = new int[2];
                var settable = true;

                //上→右→下→左の順に、ブロックの隣に新たなブロックを置けるか確認する。
                //【問題2】重複を取り除こう。
                // ヒント : 同じ処理が4回繰り返されていると、コードの修正に時間がかかってしまいます。
                // foreach文を使って、できるだけ同様の記述は1つにまとめましょう。
                // Mino.cs(Minoクラス)にあるGetNewBlock()と、Mino.directionMapを使うと楽かも…？
                foreach(var tBlock in testBlocks) {

                    //tBlockの1つ上、右、下、左隣の座標を求める。
                    //newBlockに、既にブロックが置かれていなければ、その形状をminoListに追加する。
                    newBlock[0] = tBlock[0] - 1; newBlock[1] = tBlock[1];
                    foreach(var block in testBlocks) {
                        if(block[0] == newBlock[0] && block[1] == newBlock[1]) {
                            settable = false;
                        }
                    }
                    if(settable == true) { AddMino(tmpBlocks, newBlock); tmpBlocks.Remove(newBlock); }
                    settable = true;
                    newBlock[0] = tBlock[0]; newBlock[1] = tBlock[1] + 1;
                    foreach(var block in testBlocks) {
                        if(block[0] == newBlock[0] && block[1] == newBlock[1]) {
                            settable = false;
                        }
                    }
                    if(settable == true) { AddMino(tmpBlocks, newBlock); tmpBlocks.Remove(newBlock); }
                    settable = true;
                    newBlock[0] = tBlock[0] + 1; newBlock[1] = tBlock[1];
                    foreach(var block in testBlocks) {
                        if(block[0] == newBlock[0] && block[1] == newBlock[1]) {
                            settable = false;
                        }
                    }
                    if(settable == true) { AddMino(tmpBlocks, newBlock); tmpBlocks.Remove(newBlock); }
                    settable = true;
                    newBlock[0] = tBlock[0]; newBlock[1] = tBlock[1] - 1;
                    foreach(var block in testBlocks) {
                        if(block[0] == newBlock[0] && block[1] == newBlock[1]) {
                            settable = false;
                        }
                    }
                    if(settable == true) { AddMino(tmpBlocks, newBlock); tmpBlocks.Remove(newBlock); }
                    settable = true;
                }
            }
        }

        //適切な形状であれば、ミノを生成しリストに追加する。
        private static void AddMino(List<int[]> blocks, int[] nBlock) {
            blocks.Add(nBlock);
            blocks = blocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();

            //xMin(各座標の中で最小のx座標の値)を算出し、左端に何マスの隙間があるか計算する。 yMin(最小のy座標の値)も同様。
            var xMin = blocks.Min(x => x[1]);
            var yMin = blocks.Min(y => y[0]);

            var blocksWithNoSpace = new List<int[]>();

            foreach(var block in blocks) {
                blocksWithNoSpace.Add(new int[] { block[0] - yMin, block[1] - xMin });
            }

            var height = blocksWithNoSpace.Max(y => y[0]) - blocksWithNoSpace.Min(y => y[0]) + 1;
            var width = blocksWithNoSpace.Max(x => x[1]) - blocksWithNoSpace.Min(x => x[1]) + 1;

            //縦より横の長さの方が長い場合
            if(height < width) {
                //形状を時計回りに90°回転させて、長さを(縦)≥(横)にする。
                var tmpBlocks = new List<int[]>();
                var tmpBlocksWithNoSpace = new List<int[]>();
                foreach(var block in blocksWithNoSpace) {
                    tmpBlocks.Add(new int[] { block[1], height - block[0] });
                }
                tmpBlocks = tmpBlocks.OrderBy(x => x[0]).ThenBy(y => y[1]).ToList();

                xMin = tmpBlocks.Min(x => x[1]);
                yMin = tmpBlocks.Min(y => y[0]);

                foreach(var block in tmpBlocks) {
                    tmpBlocksWithNoSpace.Add(new int[] { block[0] - yMin, block[1] - xMin });
                }

                blocksWithNoSpace = tmpBlocksWithNoSpace;
                (height, width) = (width, height);
            }

            //既に生成されているミノと比較して、重複がない場合のみミノを生成し、リストに追加する。
            if(CheckNoDuplication(blocksWithNoSpace, height, width)) {
                minoList.Add(new Mino(blocksWithNoSpace));
            };
        }

        //重複しているミノの生成を防ぐ。
        //与えられた形状に重複が無かった場合、trueを返す。
        private static bool CheckNoDuplication(List<int[]> blocks, int height, int width) {
            bool sameFlg;
            foreach(var mino in minoList) {
                //縦と横の長さが同じ場合のみ重複をチェックする。
                if(height == mino.Height && width == mino.Width) {
                    //同じかどうか比較
                    var rBlocks = blocks.Select(b => b.ToArray()).ToList();
                    var tmpBlocks = new List<int[]>();
                    for(var i = 0; i < 4; i++) {
                        var differ = i % 2 == 0 ? height : width;
                        //tmpBlocksに、rBlocksを90°回転させた座標を代入して、チェックを行う。
                        foreach(var rBlock in rBlocks) {
                            tmpBlocks.Add(new int[] { rBlock[1], differ - 1 - rBlock[0] });
                        }
                        tmpBlocks = tmpBlocks.OrderBy(y => y[0]).ThenBy(x => x[1]).ToList();
                        rBlocks = tmpBlocks.Select(b => b.ToArray()).ToList();
                        sameFlg = true;
                        //全ての座標が一致していた場合、重複していると判定される。
                        for(var j = 0; j < mino.Blocks.Count; j++) {
                            if(!Enumerable.SequenceEqual(tmpBlocks[j], mino.Blocks[j])) { sameFlg = false; }
                        }
                        if(sameFlg) { return false; }
                        tmpBlocks.Clear();
                    }
                }
            }
            return true;
        }
    }
}
