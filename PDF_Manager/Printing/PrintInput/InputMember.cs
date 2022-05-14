﻿using Newtonsoft.Json.Linq;
using PDF_Manager.Comon;
using PDF_Manager.Printing.Comon;
using PdfSharpCore.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDF_Manager.Printing
{
    public class Member
    {
        public string ni; // 節点番号
        public string nj;
        public string e;  // 材料番号
        public double cg; // コードアングル

    }


    internal class InputMember
    {
        public const string KEY = "member";

        private Dictionary<string, Member> members = new Dictionary<string, Member>();

        public InputMember(Dictionary<string, object> value)
        {
            if (!value.ContainsKey(KEY))
                return;

            // memberデータを取得する
            var target = JObject.FromObject(value[KEY]).ToObject<Dictionary<string, object>>();

            // データを抽出する
            for (var i = 0; i < target.Count; i++)
            {
                var key = target.ElementAt(i).Key;
                var item = JObject.FromObject(target.ElementAt(i).Value);

                var m = new Member();
                m.ni = dataManager.toString(item["ni"]);
                m.nj = dataManager.toString(item["nj"]);
                m.e = dataManager.toString(item["e"]);
                m.cg = dataManager.parseDouble(item["cg"]);
                this.members.Add(key, m);
            }
        }



        #region 印刷処理
        // タイトル
        private string title;
        // 2次元か3次元か
        private int dimension;
        // 項目タイトル
        private string[,] header_content;
        // ヘッダーのx方向の余白
        private double[] header_Xspacing;
        // ボディーのx方向の余白
        private double[] body_Xspacing;
        // ボディーの文字位置
        private XStringFormat[] body_align;
        // 節点情報
        private InputNode Node = null;
        // 材料情報
        private InputElement Element = null;

        /*
        /// <summary>
        /// 印刷前の初期化処理
        /// </summary>
        private void printInit(PdfDocument mc, PrintData data)
        {
            var X1 = printManager.H1PosX; //表題を印字するX位置  px ピクセル

            this.dimension = data.dimension;
            if (this.dimension == 3)
            {   // 3次元
                this.header_Xspacing = new double[] {
                    X1, X1 + 50, X1 + 100, X1 + 170, X1 + 220, X1 + 300, X1 + 400
                };
                this.body_Xspacing = Array.ConvertAll(this.header_Xspacing, (double x) => { return x + 15; });

                switch (data.language)
                {
                    case "en":
                        this.title = "Member Data";
                        this.header_content = new string[,] {
                            { "", "Node", "", "Distance", "Material No.", "Angle of Rotation" , "Name of Material"},
                            { "No", "Node-I", "Node-J", "(m)", "", "(°)" , ""}
                        };
                        break;

                    case "cn":
                        this.title = "构件";
                        this.header_content = new string[,] {
                            { "", "节点", "", "构件长", "材料编码", "转动角度" , "材料名称"},
                            { "No", "I端", "J端", "(m)", "", "(°)" , ""}
                        };
                        break;

                    default:
                        this.title = "部材データ";
                        this.header_content = new string[,] {
                            { "", "節点", "", "L", "材料番号", "コードアングル" , "材料名称"},
                            { "No", "I端", "J端", "(m)", "", "(°)" , ""}
                        };
                        break;
                }
                this.body_align = new XStringFormat[] {
                    XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomCenter, XStringFormats.BottomRight, XStringFormats.BottomLeft
                };

            }
            else
            {   // 2次元
                this.header_Xspacing = new double[] {
                    X1, X1 + 50, X1 + 100, X1 + 170, X1 + 220, X1 + 300
                };
                this.body_Xspacing = Array.ConvertAll(this.header_Xspacing, (double x) => { return x + 15; });

                switch (data.language)
                {
                    case "en":
                        this.title = "Member Data";
                        this.header_content = new string[,] {
                            { "", "Node", "", "Distance", "Material No.",  "Name of Material"},
                            { "No", "Node-I", "Node-J", "(m)", "" , ""}
                        };
                        break;

                    case "cn":
                        this.title = "构件";
                        this.header_content = new string[,] {
                            { "", "节点", "", "构件长", "材料编码" , "材料名称"},
                            { "No", "I端", "J端", "(m)", "" , ""}
                        };
                        break;

                    default:
                        this.title = "部材データ";
                        this.header_content = new string[,] {
                            { "", "節点", "", "L", "材料番号" , "材料名称"},
                            { "No", "I端", "J端", "(m)", "",  ""}
                        };
                        break;
                }
                this.body_align = new XStringFormat[] {
                    XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomRight, XStringFormats.BottomCenter, XStringFormats.BottomLeft
                };

            }

        }


        /// <summary>
        /// 1ページに入れるコンテンツを集計する
        /// </summary>
        /// <param name="target">印刷対象の配列</param>
        /// <param name="rows">行数</param>
        /// <returns>印刷する用の配列</returns>
        private List<string[]> getPageContents(Dictionary<string, Member> target)
        {
            int count = this.header_content.GetLength(1);

            // 行コンテンツを生成
            var table = new List<string[]>();

            for (var i = 0; i < target.Count; i++)
            {
                var lines = new string[count];

                string No = target.ElementAt(i).Key;
                Member item = target.ElementAt(i).Value;

                int j = 0;
                lines[j] = No;
                j++;
                lines[j] = printManager.toString(item.ni);
                j++;
                lines[j] = printManager.toString(item.nj);
                j++;
                lines[j] = printManager.toString(this.GetMemberLength(No), 3);
                j++;
                lines[j] = printManager.toString(item.e);
                j++;
                if (this.dimension == 3)
                {
                    lines[j] = printManager.toString(item.cg, 3);
                    j++;
                }
                lines[j] = printManager.toString(this.Element.GetElementName(item.e));
                j++;
                table.Add(lines);
            }
            return table;
        }


        /// <summary>
        /// 印刷する
        /// </summary>
        /// <param name="mc"></param>
        public void printPDF(PdfDocument mc, PrintData data)
        {
            // 部材長を取得できる状態にする
            this.Node = (InputNode)data.printDatas[InputNode.KEY];

            // 材料名称を取得できる状態にする
            this.Element = (InputElement)data.printDatas[InputElement.KEY];


            // タイトル などの初期化
            this.printInit(mc, data);

            // 印刷可能な行数
            var printRows = printManager.getPrintRowCount(mc, this.header_content);

            // 行コンテンツを生成
            var page = new List<List<string[]>>();

            // 1ページ目に入る行数
            int rows = printRows[0];

            // 集計開始
            var tmp1 = new Dictionary<string, Member>(this.members); // clone
            while (true)
            {
                // 1ページに納まる分のデータをコピー
                var tmp2 = new Dictionary<string, Member>();
                for (int i = 0; i < rows; i++)
                {
                    if (tmp1.Count <= 0)
                        break;
                    tmp2.Add(tmp1.First().Key, tmp1.First().Value);
                    tmp1.Remove(tmp1.First().Key);
                }

                if (tmp2.Count > 0)
                {
                    var table = this.getPageContents(tmp2);
                    page.Add(table);
                }
                else if (tmp1.Count <= 0)
                {
                    break;
                }
                else
                { // 印刷するものもない
                    mc.NewPage();
                }

                // 2ページ以降に入る行数
                rows = printRows[1];
            }

            // 表の印刷
            printManager.printContent(mc, page, new string[] { this.title },
                                      this.header_content, this.header_Xspacing,
                                      this.body_Xspacing, this.body_align);
 
        }
        */
        #endregion


        #region 他のモジュールのヘルパー関数

        /// <summary>
        /// 部材にの長さを取得する
        /// </summary>
        /// <param name="mc"></param>
        /// <param name="memberNo"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public double GetMemberLength(string memberNo)
        {
            var memb = this.GetMember(memberNo);

            if (memb == null)
                return double.NaN;

            if (memb.ni == null || memb.nj == null)
            {
                return double.NaN;
            }

            Vector3 iPos = this.Node.GetNodePos(memb.ni);
            Vector3 jPos = this.Node.GetNodePos(memb.nj);
            if (iPos == null || jPos == null)
            {
                return double.NaN;
            }

            double result = Math.Sqrt(Math.Pow(iPos.x - jPos.x, 2) + Math.Pow(iPos.y - jPos.y, 2) + Math.Pow(iPos.z - jPos.z, 2));

            return result;
        }

        /// <summary>
        /// 部材情報を取得する
        /// </summary>
        /// <param name="No">部材番号</param>
        /// <returns></returns>
        public Member GetMember(string No)
        {
            if (!this.members.ContainsKey(No))
            {
                return null;
            }
            return this.members[No];
        }
        

        /*
        public void printPDF(PdfDoc mc)
        {

            int bottomCell = mc.bottomCell;

            // 全行数の取得
            double count = (data.Count + ((data.Count / bottomCell) + 1) * 5) * mc.single_Yrow;
            //  改ページ判定
            mc.DataCountKeep(count);

            //　ヘッダー
            string[,] header_content3D = {
                { "", "節点", "", "L", "材料番号", "コードアングル" , "材料名称"},
                { "No", "I端", "J端", "(m)", "", "(°)" , ""}

            };

            string[,] header_content2D = {
                { "", "節点", "", "L", "材料番号", "" , "材料名称"},
                { "No", "I端", "J端", "(m)", "", "" , ""}            
            };

            switch (mc.language)
            {
                case "ja":
                    mc.PrintContent("部材データ", 0);
                    break;
                case "en":
                    mc.PrintContent("Member Data", 0);
                    header_content3D[0, 1] = "Node";
                    header_content3D[0, 3] = "Distance";
                    header_content3D[0, 4] = "Material No.";
                    header_content3D[0, 5] = "Angle of Rotation ";
                    header_content3D[0, 6] = "Name of Material";
                    header_content3D[1, 1] = "Node-I";
                    header_content3D[1, 2] = "Node-J";

                    header_content2D[0, 1] = "Node";
                    header_content2D[0, 3] = "Distance";
                    header_content2D[0, 4] = "Material No.";
                    header_content2D[0, 6] = "Name of Material";
                    header_content2D[1, 1] = "Node-I";
                    header_content2D[1, 2] = "Node-J";
                    break;
            }

        
            mc.CurrentRow(2);
            mc.CurrentColumn(0);

            // ヘッダーのx方向の余白
            int[,] header_Xspacing3D = {
                { 0, 75, 100, 145, 203, 280, 360 },
                { 10, 50, 100, 145, 203, 280, 360 } 
            };
            int[,] header_Xspacing2D = {
                { 10, 90, 120, 180, 255, 280, 351 },
                { 10, 60, 120, 180, 255, 280, 351 }
            };

            // ボディーのx方向の余白
            int[,] body_Xspacing3D = { { 17, 60, 110, 157, 208, 284, 341 } };
            int[,] body_Xspacing2D = { { 17, 70, 130, 192, 260, 284, 330 } };

            string[,] header_content = mc.dimension == 3 ? header_content3D : header_content2D;
            int[,] header_Xspacing = mc.dimension == 3 ? header_Xspacing3D : header_Xspacing2D;
            int[,] body_Xspacing = mc.dimension == 3 ? body_Xspacing3D : body_Xspacing2D;

            mc.Header(header_content, header_Xspacing);


            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Length; j++)
                {
                    mc.CurrentColumn(body_Xspacing[0, j]); //x方向移動
                    if (j == 6) // 材料名称のみ左詰め
                    {
                        mc.PrintContent(data[i][j], 1); // print
                    }
                    else
                    {
                        mc.PrintContent(data[i][j]); // print
                    }
                }
                if (!(i == data.Count - 1))
                {
                    mc.CurrentRow(1); // y方向移動
                }
            }
        }
        */
        #endregion
    }
}

