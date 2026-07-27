using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CsvHelper;
using System.Windows.Markup;

namespace EagleControl_Native2_CuFe
{

    internal class ToCsv
    {
        string[][] _alertTotalList = new string[74][];

        private static String folderPath = Environment.CurrentDirectory;


        public ToCsv()
        {
            _alertTotalList[0] = new string[] { "D7000", "0", "1", "PLC1_Normal_Fault0.0—停机报警：循环杯满溢" };
            _alertTotalList[1] = new string[] { "D7000", "1", "2", "PLC1_Normal_Fault0.1—停机报警：反应杯满溢" };
            _alertTotalList[2] = new string[] { "D7000", "2", "3", "PLC1_Normal_Fault0.2—停机报警：分析仪漏液" };
            _alertTotalList[3] = new string[] { "D7000", "3", "4", "PLC1_Normal_Fault0.3—停机报警：立即停止" };
            _alertTotalList[4] = new string[] { "D7000", "4", "5", "PLC1_Normal_Fault0.4—停机报警：反应杯液位Sensor异常" };
            _alertTotalList[5] = new string[] { "D7000", "5", "6", "PLC1_Normal_Fault0.5—停机报警：紧急停止" };
            _alertTotalList[6] = new string[] { "D7002", "0", "33", "PLC1_Normal_Warning0.0—报警：纯水桶满溢" };
            _alertTotalList[7] = new string[] { "D7002", "1", "34", "PLC1_Normal_Warning0.1—报警：部件X需更换" };
            _alertTotalList[8] = new string[] { "D7002", "2", "35", "PLC1_Normal_Warning0.2—报警：分析仪需保养" };
            _alertTotalList[9] = new string[] { "D7002", "3", "36", "PLC1_Normal_Warning0.3—报警：药水1需添加" };
            _alertTotalList[10] = new string[] { "D7002", "4", "37", "PLC1_Normal_Warning0.4—报警：药水2需添加" };
            _alertTotalList[11] = new string[] { "D7002", "5", "38", "PLC1_Normal_Warning0.5—报警：药水3需添加" };
            _alertTotalList[12] = new string[] { "D7002", "6", "39", "PLC1_Normal_Warning0.6—报警：药水4需添加" };
            _alertTotalList[13] = new string[] { "D7002", "7", "40", "PLC1_Normal_Warning0.7—报警：药水5需添加" };
            _alertTotalList[14] = new string[] { "D7002", "8", "41", "PLC1_Normal_Warning0.8—报警：药水6需添加" };
            _alertTotalList[15] = new string[] { "D7002", "9", "42", "PLC1_Normal_Warning0.9—报警：循环杯液位Sensor异常" };
            _alertTotalList[16] = new string[] { "D7002", "10", "43", "PLC1_Normal_Warning0.A—报警：反应杯液位Sensor异常" };
            _alertTotalList[17] = new string[] { "D7002", "11", "44", "PLC1_Normal_Warning0.B—报警：药水7需添加" };
            _alertTotalList[18] = new string[] { "D7003", "0", "49", "PLC1_Normal_Warning1.0—报警：注射泵需更换" };
            _alertTotalList[19] = new string[] { "D7003", "1", "50", "PLC1_Normal_Warning1.1—报警：电磁阀需要更换" };
            _alertTotalList[20] = new string[] { "D7003", "2", "51", "PLC1_Normal_Warning1.2—报警：mini2 需要更换" };
            _alertTotalList[21] = new string[] { "D7003", "3", "52", "PLC1_Normal_Warning1.3—报警：mini4 需要更换" };
            _alertTotalList[22] = new string[] { "D7003", "4", "53", "PLC1_Normal_Warning1.4—报警：UV sensor 需要更换" };
            _alertTotalList[23] = new string[] { "D7003", "5", "54", "PLC1_Normal_Warning1.5—报警：104 KA 需要更换" };
            _alertTotalList[24] = new string[] { "D7003", "6", "55", "PLC1_Normal_Warning1.6—报警：搅拌器需要更换" };
            _alertTotalList[25] = new string[] { "D7003", "7", "56", "PLC1_Normal_Warning1.7—报警：部件X需更换" };
            _alertTotalList[26] = new string[] { "D7003", "8", "57", "PLC1_Normal_Warning1.8—报警：部件X需更换" };
            _alertTotalList[27] = new string[] { "D7003", "9", "58", "PLC1_Normal_Warning1.9—报警：部件X需更换" };
            _alertTotalList[28] = new string[] { "D7004", "0", "201", "PLC1_A1_Fault0.0—停机报警：送样超时" };
            _alertTotalList[29] = new string[] { "D7004", "1", "202", "PLC1_A1_Fault0.1—停机报警：取样超时" };
            _alertTotalList[30] = new string[] { "D7012", "0", "401", "PLC1_B1_Fault0.0—停机报警：排废超时" };
            _alertTotalList[31] = new string[] { "D7012", "1", "402", "PLC1_B1_Fault0.1—停机报警：注射泵1通讯异常" };
            _alertTotalList[32] = new string[] { "D7012", "2", "403", "PLC1_B1_Fault0.2—停机报警：注射泵1饱管失败" };
            _alertTotalList[33] = new string[] { "D7016", "0", "501", "PLC1_B2_Fault0.0—停机报警：排废超时" };
            _alertTotalList[34] = new string[] { "D7016", "1", "502", "PLC1_B2_Fault0.1—停机报警：注射泵2通讯异常" };
            _alertTotalList[35] = new string[] { "D7016", "2", "503", "PLC1_B2_Fault0.2—停机报警：注射泵2饱管失败" };
            _alertTotalList[36] = new string[] { "D7020", "0", "601", "PLC1_B3_Fault0.0—停机报警：排废超时" };
            _alertTotalList[37] = new string[] { "D7020", "1", "602", "PLC1_B3_Fault0.1—停机报警：注射泵3通讯异常" };
            _alertTotalList[38] = new string[] { "D7020", "2", "603", "PLC1_B3_Fault0.2—停机报警：注射泵3饱管失败" };
            _alertTotalList[39] = new string[] { "D7024", "0", "701", "PLC1_C1_Fault0.0—停机报警：排废超时" };
            _alertTotalList[40] = new string[] { "D7024", "1", "702", "PLC1_C1_Fault0.1—停机报警：纯水超时" };
            _alertTotalList[41] = new string[] { "D7028", "0", "801", "PLC1_C2_Fault0.0—停机报警：排废超时" };
            _alertTotalList[42] = new string[] { "D7028", "1", "802", "PLC1_C2_Fault0.1—停机报警：纯水超时" };
            _alertTotalList[43] = new string[] { "D7032", "0", "901", "PLC1_C3_Fault0.0—停机报警：排废超时" };
            _alertTotalList[44] = new string[] { "D7032", "1", "902", "PLC1_C3_Fault0.1—停机报警：纯水超时" };
            _alertTotalList[45] = new string[] { "D7036", "0", "1001", "PLC1_C4_Fault0.0—停机报警：排废超时" };
            _alertTotalList[46] = new string[] { "D7040", "0", "1101", "PLC1_C5_Fault0.0—停机报警：排废超时" };
            _alertTotalList[47] = new string[] { "D7040", "1", "1102", "PLC1_C5_Fault0.1—停机报警：清洁液超时" };
            _alertTotalList[48] = new string[] { "D7040", "2", "1103", "PLC1_C5_Fault0.2—停机报警：排废超时" };
            _alertTotalList[49] = new string[] { "D7044", "0", "1201", "PLC1_C6_Fault0.0—停机报警：排废超时" };
            _alertTotalList[50] = new string[] { "D7048", "0", "1301", "PLC1_C7_Fault0.0—停机报警：排废超时" };
            _alertTotalList[51] = new string[] { "D7052", "0", "1401", "PLC1_D2_Fault0.0—停机报警：样液回退超时" };
            _alertTotalList[52] = new string[] { "D7064", "0", "1701", "PLC1_F1_Fault0.0—停机报警：排废超时" };
            _alertTotalList[53] = new string[] { "D7064", "1", "1702", "PLC1_F1_Fault0.1—停机报警：纯水超时" };
            _alertTotalList[54] = new string[] { "D7068", "0", "1801", "PLC1_S1_Fault0.0—停机报警：排废超时" };
            _alertTotalList[55] = new string[] { "D7068", "1", "1802", "PLC1_S1_Fault0.1—停机报警：pH4供液超时" };
            _alertTotalList[56] = new string[] { "D7068", "2", "1803", "PLC1_S1_Fault0.2—停机报警：排废超时" };
            _alertTotalList[57] = new string[] { "D7068", "3", "1804", "PLC1_S1_Fault0.3—停机报警：PH4供液超时" };
            _alertTotalList[58] = new string[] { "D7068", "4", "1805", "PLC1_S1_Fault0.4—停机报警：pH4校正异常" };
            _alertTotalList[59] = new string[] { "D7068", "5", "1806", "PLC1_S1_Fault0.5—停机报警：排废超时" };
            _alertTotalList[60] = new string[] { "D7068", "6", "1807", "PLC1_S1_Fault0.6—停机报警：pH7供液超时" };
            _alertTotalList[61] = new string[] { "D7068", "7", "1808", "PLC1_S1_Fault0.7—停机报警：排废超时" };
            _alertTotalList[62] = new string[] { "D7068", "8", "1809", "PLC1_S1_Fault0.8—停机报警：pH7供液超时" };
            _alertTotalList[63] = new string[] { "D7068", "9", "1810", "PLC1_S1_Fault0.9—停机报警：pH7校正异常" };
            _alertTotalList[64] = new string[] { "D7069", "0", "1817", "PLC1_S1_Fault1.0—停机报警：pH7供液超时" };
            _alertTotalList[65] = new string[] { "D7069", "1", "1818", "PLC1_S1_Fault1.1—停机报警：pH7校正异常" };
            _alertTotalList[66] = new string[] { "D7070", "0", "1833", "PLC1_S1_Warning0.0—报警:斜率过低建议更换电极" };
            _alertTotalList[67] = new string[] { "D7070", "1", "1834", "PLC1_S1_Warning0.1—报警:斜率过低建议更换电极" };
            _alertTotalList[68] = new string[] { "D7074", "0", "1933", "PLC1_Z1_Warning0.0—报警:纯水补给超时" };
            _alertTotalList[69] = new string[] { "D7076", "0", "2001", "PLC1_T1_Fault0.0—停机报警：纯水稀释超时" };
            _alertTotalList[70] = new string[] { "D7076", "15", "2016", "PLC1_T1_Fault0.F—停机报警：滴定超时" };
            _alertTotalList[71] = new string[] { "D7078", "0", "2033", "PLC1_T1_Warning0.0—报警:pH滴定初始值异常" };
            _alertTotalList[72] = new string[] { "D7078", "1", "2034", "PLC1_T1_Warning0.1—报警:ORP滴定初始值异常" };
            _alertTotalList[73] = new string[] { "D7080", "0", "2101", "PLC1_D3_Fault0.0—停机报警：注射泵1通讯异常" };

        }

        public void CreateStatusExcel(string path)
        {

            DateTime currentTime = DateTime.Now;

            var encoding = new System.Text.UTF8Encoding(true);

            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // CSV 檔案路徑（建議加上 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 使用 CsvHelper 進行寫檔
                using (var writer = new StreamWriter(csvPath, false, encoding))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Index");
                    csv.WriteField("日期(Date)");
                    csv.WriteField("時間(Time)");
                    csv.WriteField("設備狀態(Status)");

                    csv.NextRecord();
                }

                // 紀錄成功訊息
                Log($"Success (CreateExcel) => {csvPath}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }


        }


        public String WriteStatus(string Path, string Status)
        {

            string result = "0";

            try
            {
                // CSV 完整路徑（建議保證帶有 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(Path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 若檔案不存在，先呼叫 CreateExcel( ) 建立表頭
                if (!File.Exists(csvPath))
                {
                    CreateStatusExcel(Path);
                }

                int currentCount = 1;

                currentCount = File.ReadLines(csvPath).Count();

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

                // 以 Append 方式打開檔案，寫入新的一列資料
                using (var stream = new FileStream(csvPath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(stream, utf8WithBom))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {

                    csv.WriteField(currentCount);
                    csv.WriteField($"{DateTime.Now:yyyy/MM/dd}");
                    csv.WriteField($"{DateTime.Now:HH:mm:ss}");
                    csv.WriteField(Status);

                    //// 呼叫 NextRecord() 換行，表示一列結束
                    csv.NextRecord();
                }

                result = "1";

                Log($"Success : (WriteStatus) => {csvPath},{currentCount},{DateTime.Now:yyyy/MM/dd},{DateTime.Now:HH:mm:ss},{Status}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }

            return result;
        }


        public void CreateErrorLogExcel(string path)
        {

            DateTime currentTime = DateTime.Now;

            var encoding = new System.Text.UTF8Encoding(true);

            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // CSV 檔案路徑（建議加上 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 使用 CsvHelper 進行寫檔
                using (var writer = new StreamWriter(csvPath, false, encoding))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Index");
                    csv.WriteField("日期(Date)");
                    csv.WriteField("時間(Time)");
                    csv.WriteField("警报序号");
                    csv.WriteField("描述");
                    csv.WriteField("触发 / 复位");
                    csv.NextRecord();
                }

                // 紀錄成功訊息
                Log($"Success (CreateErrorLogExcel) => {csvPath}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }
        }


        public String WriteErrorLog(string Path, List<List<string>> _onList, List<List<string>> _backToOffList)
        {

            //"[D7000, 2, ON, 2026/07/18 23:29:00, 1]"
            //"[D7000, 2, OFF, 2026/07/18 23:29:27]"

            string result = "0";

            try
            {
                List<List<string>> TotalList = new List<List<string>>();

                TotalList.AddRange(_onList);
                TotalList.AddRange(_backToOffList);

                TotalList = TotalList
                    .OrderBy(x => DateTime.Parse(x[3]))
                    .ToList();


                string csvPath = System.IO.Path.Combine(Path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                if (!File.Exists(csvPath))
                {
                    CreateErrorLogExcel(Path);
                }

                int currentCount = 1;

                currentCount = File.ReadLines(csvPath).Count();

                string[] parts = null;

                for (int i = 0; i < TotalList.Count; i++)
                {

                    for (int j = 0; j < _alertTotalList.Length; j++)
                    {

                        if (_alertTotalList[j][0] == TotalList[i][0] && _alertTotalList[j][1] == TotalList[i][1])
                        {

                            TotalList[i].Add(_alertTotalList[j][2]);
                            TotalList[i].Add(_alertTotalList[j][3]);

                            break;
                        }
                    }
                }

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

                // 以 Append 方式打開檔案，寫入新的一列資料
                using (var stream = new FileStream(csvPath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(stream, utf8WithBom))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    for (int i = 0; i < TotalList.Count; i++)
                    {

                        parts = TotalList[i][3].Split(' ');

                        csv.WriteField(currentCount + i);
                        csv.WriteField(parts[0]);
                        csv.WriteField(parts[1]);

                        if (TotalList[i][2] == "ON")
                        {

                            csv.WriteField(TotalList[i][5]);
                            csv.WriteField(TotalList[i][6]);
                            csv.WriteField("T");

                            Log($"Success : (WriteErrorLog) => {csvPath},{currentCount + i},{parts[0]},{parts[1]},{TotalList[i][5]},{TotalList[i][6]}, T");
                        }

                        else if (TotalList[i][2] == "OFF")
                        {
                            csv.WriteField(TotalList[i][4]);
                            csv.WriteField(TotalList[i][5]);
                            csv.WriteField("F");

                            Log($"Success : (WriteErrorLog) => {csvPath},{currentCount + i},{parts[0]},{parts[1]},{TotalList[i][4]},{TotalList[i][5]}, F");
                        }

                        csv.NextRecord();

                        
                    }
                }

                result = "1";
                
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }


            return result;
        }


        public void CreateAnalyseDataExcel(string path)
        {

            DateTime currentTime = DateTime.Now;

            var encoding = new System.Text.UTF8Encoding(true);

            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // CSV 檔案路徑（建議加上 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 使用 CsvHelper 進行寫檔
                using (var writer = new StreamWriter(csvPath, false, encoding))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Index");
                    csv.WriteField("日期(Date)");
                    csv.WriteField("時間(Time)");
                    csv.WriteField("結果");
                    csv.WriteField("濃度");
                    csv.WriteField("空白");
                    csv.NextRecord();
                }

                // 紀錄成功訊息
                Log($"Success (CreateAnalyseDataExcel) => {csvPath}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }
        }

        public String WriteAnalyseData(float[] _AnalyseData)
        {

            string result = "0";

            string Path = "";

            string FolderName = "";

            try
            {
                if (_AnalyseData[0] == 1) FolderName = "槽A";
                else if (_AnalyseData[0] == 2) FolderName = "槽B";
                else if (_AnalyseData[0] == 3) FolderName = "留A";
                else if (_AnalyseData[0] == 4) FolderName = "留B";

                if (_AnalyseData[1] == 1) FolderName += "_銅";
                else if (_AnalyseData[1] == 4) FolderName += "_鐵";

                Path = System.IO.Path.Combine(folderPath, "CsvData", FolderName);


                // CSV 完整路徑（建議保證帶有 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(Path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 若檔案不存在，先呼叫 CreateExcel( ) 建立表頭
                if (!File.Exists(csvPath))
                {
                    CreateAnalyseDataExcel(Path);
                }

                int currentCount = 1;

                currentCount = File.ReadLines(csvPath).Count();

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

                // 以 Append 方式打開檔案，寫入新的一列資料
                using (var stream = new FileStream(csvPath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(stream, utf8WithBom))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {

                    csv.WriteField(currentCount);
                    csv.WriteField($"{DateTime.Now:yyyy/MM/dd}");
                    csv.WriteField($"{DateTime.Now:HH:mm:ss}");
                    csv.WriteField(_AnalyseData[2]);
                    csv.WriteField(_AnalyseData[3]);
                    csv.WriteField(_AnalyseData[4]);

                    csv.NextRecord();
                }

                result = "1";

                Log($"Success : (WriteAnalyseData) => {csvPath},{currentCount},{DateTime.Now:yyyy/MM/dd},{DateTime.Now:HH:mm:ss},{_AnalyseData[2]},{_AnalyseData[3]},{_AnalyseData[4]}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }

            return result;
        }


        public void CreateCalibrationParameterExcel(string path)
        {

            DateTime currentTime = DateTime.Now;

            var encoding = new System.Text.UTF8Encoding(true);

            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // CSV 檔案路徑（建議加上 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 使用 CsvHelper 進行寫檔
                using (var writer = new StreamWriter(csvPath, false, encoding))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Index");
                    csv.WriteField("日期(Date)");
                    csv.WriteField("時間(Time)");

                    csv.WriteField("槽A_Cu2+_斜率");
                    csv.WriteField("槽A_Cu2+_偏移量");
                    csv.WriteField("槽A_Fe3+_斜率");
                    csv.WriteField("槽A_Fe3+_偏移量");

                    csv.WriteField("槽B_Cu2+_斜率");
                    csv.WriteField("槽B_Cu2+_偏移量");
                    csv.WriteField("槽B_Fe3+_斜率");
                    csv.WriteField("槽B_Fe3+_偏移量");

                    csv.WriteField("留样A_Cu2+_斜率");
                    csv.WriteField("留样A_Cu2+_偏移量");
                    csv.WriteField("留样A_Fe3+_斜率");
                    csv.WriteField("留样A_Fe3+_偏移量");

                    csv.WriteField("留样B_Cu2+_斜率");
                    csv.WriteField("留样B_Cu2+_偏移量");
                    csv.WriteField("留样B_Fe3+_斜率");
                    csv.WriteField("留样B_Fe3+_偏移量");
                    csv.NextRecord();
                }

                // 紀錄成功訊息
                Log($"Success (CreateCalibrationParameterExcel) => {csvPath}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }
        }

        public String WriteCalibrationParameter(string Path, float[] _AnalyseData)
        {

            string result = "0";

            try
            {
                // CSV 完整路徑（建議保證帶有 .csv 副檔名）
                string csvPath = System.IO.Path.Combine(Path, $"{DateTime.Now:yyyyMMdd}" + ".csv");

                // 若檔案不存在，先呼叫 CreateExcel( ) 建立表頭
                if (!File.Exists(csvPath))
                {
                    CreateCalibrationParameterExcel(Path);
                }

                int currentCount = 1;

                string data = "";

                currentCount = File.ReadLines(csvPath).Count();

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);

                // 以 Append 方式打開檔案，寫入新的一列資料
                using (var stream = new FileStream(csvPath, FileMode.Append, FileAccess.Write))
                using (var writer = new StreamWriter(stream, utf8WithBom))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {

                    csv.WriteField(currentCount);
                    csv.WriteField($"{DateTime.Now:yyyy/MM/dd}");
                    csv.WriteField($"{DateTime.Now:HH:mm:ss}");

                    

                    for (int i = 0; i < _AnalyseData.Length; i++)
                    {

                        csv.WriteField(_AnalyseData[i]);

                        data += "," + _AnalyseData[i].ToString();
                    }

                    csv.NextRecord();
                }
                result = "1";

                Log($"Success : (WriteCalibrationParameter) => {csvPath},{currentCount},{DateTime.Now:yyyy/MM/dd},{DateTime.Now:HH:mm:ss}, {data}");
            }
            catch (Exception ex)
            {
                Log($"{ex} \n{ex.StackTrace}\n");
            }

            return result;
        }


        private void Log(string text)
        {

            try
            {
                // 確保日誌資料夾存在
                if (!Directory.Exists(System.IO.Path.Combine(folderPath, "ToCsvLogs")))
                {

                    Directory.CreateDirectory(System.IO.Path.Combine(folderPath, "ToCsvLogs"));

                }

                // 生成以日期命名的日誌檔案名稱
                string logFileName = System.IO.Path.Combine(folderPath, "ToCsvLogs", $"{DateTime.Now:yyyy-MM-dd}.txt");

                // 將錯誤訊息追加到日誌檔案
                File.AppendAllText(logFileName, $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text}\n");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
        }
    }
}
