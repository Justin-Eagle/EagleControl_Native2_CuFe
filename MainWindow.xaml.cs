using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using ActUtlTypeLib;
using static System.Net.Mime.MediaTypeNames;

namespace EagleControl_Native2_CuFe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static String folderPath = Environment.CurrentDirectory;

        private int StationNum_Value = 0;

        CancellationTokenSource cancellationTokenSource = null;

        private ActUtlType plc = null;

        private bool NextHeartBeat = false;

        ToCsv tocsv = new ToCsv();

        string StatusCsvPath = System.IO.Path.Combine(folderPath, "CsvData", "StatusCsv");
        string ErrorLogCsvPath = System.IO.Path.Combine(folderPath, "CsvData", "ErrorLogCsv");

        string CalibrationParameterCsvPath = System.IO.Path.Combine(folderPath, "CsvData", "CalibrationParameterCsv");
    

        private HashSet<(string Device, int Bit)> _offIndex = new HashSet<(string Device, int Bit)>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Monitor1.Text = "";
                Monitor2.Text = "";
                Monitor3.Text = "";
                Monitor4.Text = "";
                Monitor5.Text = "";
                Monitor6.Text = "";

                MonitorShow(6, "監控啟動");


                StationNum_Value = int.Parse(StationNum.Text);

                plc = new ActUtlType();
                plc.ActLogicalStationNumber = StationNum_Value;

                HeartBeatBtn1.Background = Brushes.Black;
                HeartBeatBtn2.Background = Brushes.Black;


                cancellationTokenSource = new CancellationTokenSource();

                var task1 = Task.Run(() => Go(cancellationTokenSource.Token));

                GoButton.Background = Brushes.Gray;
                GoButton.IsEnabled = false;

                StopButton.Background = Brushes.Red;
                StopButton.IsEnabled = true;

                StationNum.IsEnabled = false;
            }

            catch (Exception ex) {

                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
               
            }
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonitorShow(6, "監控關閉");

                GoButton.Background = Brushes.Green;
                GoButton.IsEnabled = true;

                StopButton.Background = Brushes.Gray;
                StopButton.IsEnabled = false;

                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;

                StationNum.IsEnabled = true;

                _offIndex.Clear();

                //plc = null;
            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");

            }
        }

        private void Go(CancellationToken cancellationToken)
        {

            DateTime _nextHeartbeat = DateTime.Now;
            DateTime _nextStatus = DateTime.Now;
            DateTime _nextAlertData = DateTime.Now;
            DateTime _nextAnalyseData = DateTime.Now;
            DateTime _nextCalibrationParameter = DateTime.Now;
            DateTime _nextWriteAlertData = DateTime.Now;

            bool _runnung;
            bool _waiting;
            bool _stopping;
            bool _brokening;
            bool _alerting;

            string temV = "";

            NextHeartBeat = ReadHeartBeat();

            int WriteAlready = 0;
            bool AlertWriteAlready = false;

            bool ScanAlertStatus = false;

            string content = "";

            bool _analyseDataIsAlreadyWrite = false;

            bool _startAnalyseIsAlreadyWrite = false;

            bool _isAnalyseDataOutSpec = false;

            int HeartBeatDownCount = 1;

            int _onListAlreadyWriteCount = 0;

            int _backToOffListAlreadyWriteCount = 0;

            short[] _rawData = new short[1];

            string[] _deviceList = new string[] {
                        "D7000", "D7002", "D7003", "D7004", "D7012", "D7016", "D7020", "D7024", "D7028", "D7032" , "D7036", "D7040", "D7044", "D7048", "D7052", "D7056", "D7062", "D7064", "D7068", "D7069", "D7070", "D7074", "D7076", "D7078", "D7080"
                    };

            int[][] _checkBitPosition = new int[25][];
            _checkBitPosition[0] = new int[] {0, 1, 2, 3, 4, 5}; // D7000
            _checkBitPosition[1] = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}; // D7002
            _checkBitPosition[2] = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}; // D7003
            _checkBitPosition[3] = new int[] {0, 1}; // D7004
            _checkBitPosition[4] = new int[] {0, 1, 2}; // D7012
            _checkBitPosition[5] = new int[] {0, 1, 2}; // D7016
            _checkBitPosition[6] = new int[] {0, 1, 2}; // D7020
            _checkBitPosition[7] = new int[] {0, 1}; // D7024
            _checkBitPosition[8] = new int[] {0, 1}; // D7028
            _checkBitPosition[9] = new int[] {0, 1}; // D7032
            _checkBitPosition[10] = new int[] {0}; // D7036
            _checkBitPosition[11] = new int[] {0, 1, 2}; // D7040
            _checkBitPosition[12] = new int[] {0}; // D7044
            _checkBitPosition[13] = new int[] {0}; // D7048
            _checkBitPosition[14] = new int[] {0};// D7052
            _checkBitPosition[15] = new int[] {0};// D7056
            _checkBitPosition[16] = new int[] {0};// D7062
            _checkBitPosition[17] = new int[] {0, 1}; // D7064
            _checkBitPosition[18] = new int[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}; // D7068
            _checkBitPosition[19] = new int[] {0, 1}; // D7069
            _checkBitPosition[20] = new int[] {0, 1}; // D7070
            _checkBitPosition[21] = new int[] {0}; // D7074
            _checkBitPosition[22] = new int[] {0, 15}; // D7076
            _checkBitPosition[23] = new int[] {0, 1, 2}; // D7078
            _checkBitPosition[24] = new int[] {0}; // D7080


            for (int i = 0; i < _deviceList.Length; i++)
            {
                content += _deviceList[i] + "\n";
            }

            short[] _nowAlertData = new short[_deviceList.Length];

            List<List<string>> _onList = new List<List<string>>(); 
            List<List<string>> _nowList = new List<List<string>>();
            List<List<string>> _backToOffList = new List<List<string>>();

            var row = new List<string>();

            short[] _calibrationParameter = new short[32];

            short[] _nowCalibrationParameter = new short[32];

            string Catch_1 = "D3692\n"; //判斷機台狀態
            string Catch_2 = content; //判斷錯誤的 deviceList
            string Catch_3 = "D3510\n"; //判斷濃度
            string Catch_4 = "D3511\nD3512\nD3520\nD3521\nD3522\nD3523\nD7732\nD7733\n"; //判斷濃度(加3)
            string Catch_5 = "D3530\nD3531\nD3532\nD3533\nD3538\nD3539\nD3540\nD3541\nD3554\nD3555\nD356\nD357\nD3562\nD3563\nD3564\nD3565\nD3578\nD3579\nD3580\nD3581\nD3586\nD3587\nD3588\nD3589\nD3602\nD3603\nD3604\nD3605\nD3610\nD3611\nD3612\nD3613\n"; //判斷參數調整

            short[] TotolContent = Read(Catch_1 + Catch_2 + Catch_3 + Catch_4 + Catch_5, 1 + 25 + 1 + 8 + 32);

            Array.Copy(TotolContent, 35, _calibrationParameter, 0, 32);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {

                    TotolContent = Read(Catch_1+ Catch_2+ Catch_3+ Catch_4+ Catch_5, 1+25+1+8+32);

                    temV = "";

                    foreach (var item in TotolContent) {
                        temV += item + ", ";
                    }

                    Log("TotolContentLog", $"{temV}");

                    // 0-1
                    // 1-26
                    // 26-27
                    // 27-35
                    // 35-63

                    //判斷機台狀態
                    if (DateTime.Now >= _nextStatus)
                    {
                        Array.Copy(TotolContent, 0, _rawData, 0, 1);

                        _runnung = ReadSingleBitFromD(_rawData[0], 0);
                        _waiting = ReadSingleBitFromD(_rawData[0], 1);
                        _stopping = ReadSingleBitFromD(_rawData[0], 2);
                        _brokening = ReadSingleBitFromD(_rawData[0], 3);
                        _alerting = ReadSingleBitFromD(_rawData[0], 4);

                        Log("StatusLog", $"_runnung:{_runnung} ; _waiting:{_waiting} ; _stopping:{_stopping} ; _brokening:{_brokening} ; _alerting:{_alerting} ");

                        if (_runnung)
                        {
                            if (WriteAlready != 1)
                            {

                                string result = tocsv.WriteStatus(StatusCsvPath, "分析中");

                                MonitorShow(1, "分析中");

                                if (result == "1") {
                                    MonitorShow(6, "分析儀狀態更新(分析中)，輸出至CSV (StatusCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(分析中)，輸出至CSV (StatusCsv) 失敗");
                                }

                                WriteAlready = 1;


                                string[] contentArray = new string[] { "Working" , "0", "分析中"};
                                result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), contentArray);

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(分析中)，輸出至 Show CSV (EventCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(分析中)，輸出至 Show CSV (EventCsv) 失敗");
                                }

                            }
                        }

                        else if (_waiting)
                        {
                            if (WriteAlready != 2)
                            {
                                string result = tocsv.WriteStatus(StatusCsvPath, "等待中");
                                MonitorShow(1, "等待中");

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(等待中)，輸出至CSV (StatusCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(等待中)，輸出至CSV (StatusCsv) 失敗");
                                }

                                WriteAlready = 2;


                                string[] contentArray = new string[] { "Standby", "99", "等待中" };
                                result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), contentArray);

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(等待中)，輸出至 Show CSV (EventCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(等待中)，輸出至 Show CSV (EventCsv) 失敗");
                                }
                            }
                        }

                        else if (_stopping)
                        {
                            if (WriteAlready != 3)
                            {
                                string result = tocsv.WriteStatus(StatusCsvPath, "停止中");
                                MonitorShow(1, "停止中");

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停止中)，輸出至CSV (StatusCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停止中)，輸出至CSV (StatusCsv) 失敗");
                                }

                                WriteAlready = 3;


                                string[] contentArray = new string[] { "Quit", "100", "停止中" };
                                result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), contentArray);

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停止中)，輸出至 Show CSV (EventCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停止中)，輸出至 Show CSV (EventCsv) 失敗");
                                }
                            }

                        }

                        else if (_brokening)
                        {
                            if (WriteAlready != 4)
                            {
                                string result = tocsv.WriteStatus(StatusCsvPath, "停機故障");
                                MonitorShow(1, "停機故障");

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停機故障)，輸出至CSV (StatusCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析儀狀態更新(停機故障)，輸出至CSV (StatusCsv) 失敗");
                                }

                                WriteAlready = 4;
                            }
                        }

                        if (_alerting && !AlertWriteAlready)
                        {
                            string result = tocsv.WriteStatus(StatusCsvPath, "警告");
                            MonitorShow(1, "警告");

                            if (result == "1")
                            {
                                MonitorShow(6, "分析儀狀態更新(警告)，輸出至CSV (StatusCsv) 完成");
                            }

                            else if (result == "0")
                            {
                                MonitorShow(6, "分析儀狀態更新(警告)，輸出至CSV (StatusCsv) 失敗");
                            }

                            AlertWriteAlready = true;
                        }
                        else if (!_alerting)
                        {
                            AlertWriteAlready = false;
                        }


                        if (_brokening || _alerting)
                        {
                            ScanAlertStatus = true;

                        }
                        else
                        {
                            ScanAlertStatus = false;
                        }

                        _nextStatus = DateTime.Now.AddSeconds(1);
                    }


                    if (ScanAlertStatus)
                    {

                        //一直抓數值最新狀態
                        _nowList.Clear();
                       
                        Array.Copy(TotolContent, 1, _nowAlertData, 0, 25);

                        string LogString = "";

                        for (int i = 0; i < _deviceList.Length; i++)
                        {
                            bool[] _deviceBitValue = ReadMultiBitFromD(_nowAlertData[i], _checkBitPosition[i]);

                            _onList = CollectOnBit(_deviceBitValue, _deviceList, _checkBitPosition[i], _onList, i); //保持彙總有異常的        

                            for (int j = 0; j < _deviceBitValue.Length; j++)
                            {

                                row.Add(_deviceList[i]);
                                row.Add(_checkBitPosition[i][j].ToString());
                                if (_deviceBitValue[j] == true)
                                {
                                    row.Add("ON");

                                    if (_deviceList[i] == "D7003" && _checkBitPosition[i][j].ToString() == "10")
                                    {
                                        _isAnalyseDataOutSpec = true;
                                    }
                                }
                                else if (_deviceBitValue[j] == false)
                                {
                                    row.Add("OFF");
                                }

                                _nowList.Add(new List<string>(row));

                                row.Clear();

                                LogString += $"{_deviceList[i]}.{_checkBitPosition[i][j]}:{_deviceBitValue[j]} ; ";
                            }
                        }

                        Log("AlertStatusLog" , LogString);

                        _backToOffList = CollectBackToOffBit(_onList, _nowList, _backToOffList, _deviceList);

                        _nextAlertData = DateTime.Now.AddSeconds(1);
                    }

                    if (_onList.Count != 0 && DateTime.Now >= _nextWriteAlertData)
                    {
                        Log("AlertPeriodWriteCountLog", $"_onList write : {_onListAlreadyWriteCount}/{_onList.Count} ; _backToOffList write : {_backToOffListAlreadyWriteCount}/{_backToOffList.Count}");

                        if (_onList.Count > _onListAlreadyWriteCount || _backToOffList.Count > _backToOffListAlreadyWriteCount) {

                            string result = tocsv.WriteErrorLog(ErrorLogCsvPath, _onList.GetRange(_onListAlreadyWriteCount, _onList.Count - _onListAlreadyWriteCount), _backToOffList.GetRange(_backToOffListAlreadyWriteCount, _backToOffList.Count - _backToOffListAlreadyWriteCount));

                           
                            if (result == "1")
                            {
                                MonitorShow(6, "警報紀錄輸出至CSV (ErrorLogCsv) 完成");
                            }

                            else if (result == "0")
                            {
                                MonitorShow(6, "警報紀錄輸出至CSV (ErrorLogCsv) 失敗");
                            }



                            List<List<string>> TotalList = new List<List<string>>();

                            TotalList.AddRange(_onList.GetRange(_onListAlreadyWriteCount, _onList.Count - _onListAlreadyWriteCount));
                            TotalList.AddRange(_backToOffList.GetRange(_backToOffListAlreadyWriteCount, _backToOffList.Count - _backToOffListAlreadyWriteCount));

                            TotalList = TotalList
                                .OrderBy(x => DateTime.Parse(x[3]))
                                .ToList();

                            result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), TotalList);

                            if (result == "1")
                            {
                                MonitorShow(6, "警報紀錄輸出至 EventCsv 完成");
                            }

                            else if (result == "0")
                            {
                                MonitorShow(6, "警報紀錄輸出至 EventCsv 失敗");
                            }



                            _onListAlreadyWriteCount = _onList.Count;
                            _backToOffListAlreadyWriteCount = _backToOffList.Count;
                        }

                        if (!ScanAlertStatus) {

                            _onList.Clear();
                            _nowList.Clear();
                            _backToOffList.Clear();
                            Array.Clear(_nowAlertData, 0, _nowAlertData.Length);

                            _offIndex.Clear();

                            _onListAlreadyWriteCount = 0;
                            _backToOffListAlreadyWriteCount = 0;
                        }

                        _nextWriteAlertData = DateTime.Now.AddSeconds(3);
                    }


                    if (DateTime.Now >= _nextAnalyseData)
                    {
                        Array.Copy(TotolContent, 26, _rawData, 0, 1);

                        if (_rawData[0] == 2)
                        {
                            if (_analyseDataIsAlreadyWrite == false)
                            {

                                short[] _AnalyseRowData = new short[8];

                                Array.Copy(TotolContent, 27, _AnalyseRowData, 0, 8);

                                float[] _AnalyseData = new float[] { _AnalyseRowData[0], _AnalyseRowData[1], Calculate32bit(_AnalyseRowData[2], _AnalyseRowData[3]), Calculate32bit(_AnalyseRowData[4], _AnalyseRowData[5]), Calculate32bit(_AnalyseRowData[6], _AnalyseRowData[7]) };

                                string result = tocsv.WriteAnalyseData(_AnalyseData);

                                MonitorShow(3, $"槽位:{_AnalyseData[0]} ; 成分:{_AnalyseData[1]} ; 結果:{_AnalyseData[2]} ; 濃度:{_AnalyseData[3]} ; 空白:{_AnalyseData[4]}");

                                if (result == "1")
                                {
                                    MonitorShow(6, "分析項目結束，濃度資訊輸出至CSV完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "分析項目結束，濃度資訊輸出至CSV失敗");
                                }

                                _analyseDataIsAlreadyWrite = true;

                                string[] toShowDataContent = new string[2];
                                if (_AnalyseData[1] == 1)
                                {
                                    toShowDataContent[0] = _AnalyseData[3].ToString();
                                    toShowDataContent[1] = "";
                                }
                                else if (_AnalyseData[1] == 4)
                                {
                                    toShowDataContent[1] = _AnalyseData[3].ToString();
                                    toShowDataContent[0] = "";
                                }

                                result = tocsv.WriteShowDataCsv(System.IO.Path.Combine(folderPath, "ShowData", "Data"), toShowDataContent);

                                if (result == "1")
                                {
                                    MonitorShow(6, "(ShowData) 濃度資訊輸出至CSV完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "(ShowData) 濃度資訊輸出至CSV失敗");
                                }



                                if (!_isAnalyseDataOutSpec)
                                {

                                    string[] toShowEventContent = new string[3];
                                    toShowEventContent[0] = "Error reset";

                                    if (_AnalyseData[1] == 1)
                                    {

                                        toShowEventContent[1] = "102";

                                        toShowEventContent[2] = "Cu2+ 監控濃度在規格內";
                                    }

                                    else if (_AnalyseData[1] == 4)
                                    {

                                        toShowEventContent[1] = "105";

                                        toShowEventContent[2] = "Fe3+監控濃度在規格內";
                                    }

                                    result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), toShowEventContent);

                                    if (result == "1")
                                    {
                                        MonitorShow(6, "(ShowData) 濃度規格判斷輸出至CSV完成");
                                    }

                                    else if (result == "0")
                                    {
                                        MonitorShow(6, "(ShowData)  濃度規格判斷輸出至CSV失敗");
                                    }

                                }

                            }
                        }

                        else
                        {
                            _analyseDataIsAlreadyWrite = false;
                        }



                        if (_rawData[0] == 1)
                        {
                            if (_startAnalyseIsAlreadyWrite == false)
                            {

                                short[] _AnalyseRowData = new short[8];

                                Array.Copy(TotolContent, 27, _AnalyseRowData, 0, 8);

                                float[] _AnalyseData = new float[] { _AnalyseRowData[0], _AnalyseRowData[1], Calculate32bit(_AnalyseRowData[2], _AnalyseRowData[3]), Calculate32bit(_AnalyseRowData[4], _AnalyseRowData[5]), Calculate32bit(_AnalyseRowData[6], _AnalyseRowData[7]) };

                                MonitorShow(3, $"槽位:{_AnalyseData[0]} ; 成分:{_AnalyseData[1]} 檢測開始");


                                _startAnalyseIsAlreadyWrite = true;


                                string[] toShowEventContent = new string[3];
                                toShowEventContent[0] = "Analyze start";

                                if (_AnalyseData[1] == 1)
                                {
                                    toShowEventContent[1] = "101";
                                    toShowEventContent[2] = "Cu2+检测开始";
                                }
                                else if (_AnalyseData[1] == 4)
                                {
                                    toShowEventContent[1] = "104";
                                    toShowEventContent[2] = "Fe3+检测开始";
                                }

                                string result = tocsv.WriteShowEventCsv(System.IO.Path.Combine(folderPath, "ShowData", "Event"), toShowEventContent);

                                if (result == "1")
                                {
                                    MonitorShow(6, "偵測到開始檢測，輸出至 Show CSV (EventCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "偵測到開始檢測，輸出至 Show CSV (EventCsv) 失敗");
                                }
                            }
                        }

                        else
                        {
                            _startAnalyseIsAlreadyWrite = false;
                        }

                        _nextAlertData = DateTime.Now.AddSeconds(1);

                    }



                    if (DateTime.Now >= _nextCalibrationParameter)
                    {

                        Array.Copy(TotolContent, 35, _nowCalibrationParameter, 0, 32);

                        for (int i = 0; i < 32; i++)
                        {

                            if (_nowCalibrationParameter[i] != _calibrationParameter[i])
                            {

                                float[] _nowCalibrationParameterValue = new float[] {

                                        Calculate32bit(_nowCalibrationParameter[0], _nowCalibrationParameter[1]),
                                        Calculate32bit(_nowCalibrationParameter[2], _nowCalibrationParameter[3]),
                                        Calculate32bit(_nowCalibrationParameter[4], _nowCalibrationParameter[5]),
                                        Calculate32bit(_nowCalibrationParameter[6], _nowCalibrationParameter[7]),
                                        Calculate32bit(_nowCalibrationParameter[8], _nowCalibrationParameter[9]),
                                        Calculate32bit(_nowCalibrationParameter[10], _nowCalibrationParameter[11]),
                                        Calculate32bit(_nowCalibrationParameter[12], _nowCalibrationParameter[13]),
                                        Calculate32bit(_nowCalibrationParameter[14], _nowCalibrationParameter[15]),
                                        Calculate32bit(_nowCalibrationParameter[16], _nowCalibrationParameter[17]),
                                        Calculate32bit(_nowCalibrationParameter[18], _nowCalibrationParameter[19]),
                                        Calculate32bit(_nowCalibrationParameter[20], _nowCalibrationParameter[21]),
                                        Calculate32bit(_nowCalibrationParameter[22], _nowCalibrationParameter[23]),
                                        Calculate32bit(_nowCalibrationParameter[24], _nowCalibrationParameter[25]),
                                        Calculate32bit(_nowCalibrationParameter[26], _nowCalibrationParameter[27]),
                                        Calculate32bit(_nowCalibrationParameter[28], _nowCalibrationParameter[29]),
                                        Calculate32bit(_nowCalibrationParameter[30], _nowCalibrationParameter[31])
                                    };

                                string result = tocsv.WriteCalibrationParameter(CalibrationParameterCsvPath, _nowCalibrationParameterValue);

                                _calibrationParameter = (short[])_nowCalibrationParameter.Clone();

                                MonitorShow(5, "校正參數更新");

                                if (result == "1")
                                {
                                    MonitorShow(6, "校正參數更新，輸出至CSV (CalibrationParameterCsv) 完成");
                                }

                                else if (result == "0")
                                {
                                    MonitorShow(6, "校正參數更新，輸出至CSV (CalibrationParameterCsv) 失敗");
                                }

                                break;
                            }
                        }

                        _nextCalibrationParameter = DateTime.Now.AddSeconds(5);

                    }


                    //判斷心跳
                    if (DateTime.Now >= _nextHeartbeat)
                    {
                        bool m8 = ReadHeartBeat();

                        Log("HeartBeatLog", $"M8點位心跳 : {m8}");

                        if (m8 == NextHeartBeat)
                        {
                            HeartBeatDownCount = 1;

                            NextHeartBeat = !m8;

                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                if (HeartBeatBtn1.Background == Brushes.Green)
                                {
                                    HeartBeatBtn1.Background = Brushes.Black;
                                    HeartBeatBtn2.Background = Brushes.Green;
                                }

                                else if (HeartBeatBtn1.Background == Brushes.Black)
                                {
                                    HeartBeatBtn1.Background = Brushes.Green;
                                    HeartBeatBtn2.Background = Brushes.Black;
                                }

                                else if (HeartBeatBtn1.Background == Brushes.Red && HeartBeatBtn2.Background == Brushes.Red) {

                                    HeartBeatBtn1.Background = Brushes.Green;
                                    HeartBeatBtn2.Background = Brushes.Black;
                                }
                            });
                        }

                        else
                        {
                           
                            Log("HeartBeatLog", $"HeartBeatDownCount計數值 : {HeartBeatDownCount}/10");

                            HeartBeatDownCount++;

                            if (HeartBeatDownCount > 10)
                            {
                                HeartBeatDownCount = 1;

                                MonitorShow(6, "心跳交握出現異常!!");

                                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                                {
                                    HeartBeatBtn1.Background = Brushes.Red;
                                    HeartBeatBtn2.Background = Brushes.Red;
                                });
                            }         
                        }

                        _nextHeartbeat = DateTime.Now.AddSeconds(1.25);
                    }

                }

                catch (Exception ex)
                {
                    MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                    Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");

                }

                Thread.Sleep(1000);
            }
        }

        
        private List<List<string>> CollectOnBit(bool[] DeviceBitValue , string[] _deviceList, int[] _checkBitPosition, List<List<string>> _onList,  int i)
        {
            try
            {

                var row = new List<string>();

                for (int j = 0; j < DeviceBitValue.Length; j++)
                {
                    if (DeviceBitValue[j] == true)
                    {
                        var key = (_deviceList[i], _checkBitPosition[j]);

                        if (_offIndex.Add(key))   // Add 成功代表以前沒有
                        {

                            row.Add(_deviceList[i]);
                            row.Add(_checkBitPosition[j].ToString());
                            row.Add("ON");
                            row.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                            row.Add("0");

                            //_onList.Add(row);

                            _onList.Add(new List<string>(row));
                            row.Clear();

                            MonitorShow(2, $"{_deviceList[i]}.{_checkBitPosition[j].ToString()} 異常問題'觸發'");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");

            }

            return _onList;
        }

        private List<List<string>> CollectBackToOffBit(List<List<string>> _onList, List<List<string>> _nowList, List<List<string>> _backToOffList, string[] _deviceList)
        {
            try
            {

                var row = new List<string>();

                for (int i = 0; i < _onList.Count; i++)
                {

                    for (int j = 0; j < _nowList.Count; j++)
                    {

                        if (_onList[i][4] == "0" && _onList[i][0] == _nowList[j][0] && _onList[i][1] == _nowList[j][1])
                        {

                            if (_nowList[j][2] == "OFF")
                            {

                                _onList[i][4] = "1"; //

                                row.Add(_onList[i][0]);
                                row.Add(_onList[i][1].ToString());
                                row.Add("OFF");
                                row.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                
                                _backToOffList.Add(new List<string>(row));
                                row.Clear();

                                MonitorShow(2, $"{_onList[i][0]}.{_onList[i][1].ToString()} 異常問題'復歸'");
                            }
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");

            }

            return _backToOffList;
        }

        private bool ReadHeartBeat() {

            int response = -1;

            try
            {
                plc.Open();
                plc.GetDevice("M8", out response);
            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
            }

            finally
            {
                plc.Close();
            }

            bool m8 = response == 1;

            return m8;
        }

        private short[] Read( string deviceListString , int count)
        {

            int response = -1;

            short[] rawData = new short[count];

            try
            {
                plc.Open();
                plc.ReadDeviceRandom2(deviceListString, count, out rawData[0]);

            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
            }

            finally
            {
                plc.Close();
            }

            return rawData;
        }

        public bool ReadSingleBitFromD(short value, int bit)
        {
            try
            {
                return ((value >> bit) & 1) == 1;
            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
                return false ;

            }
        }

        public bool [] ReadMultiBitFromD(short value, int [] bit)
        {
            bool[] bitValue = null;

            try
            {
                bitValue = new bool[bit.Length];

                for (int i = 0; i < bit.Length; i++)
                {

                    bitValue[i] = ((value >> bit[i]) & 1) == 1;
                }

            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
            }

            return bitValue;
        }

        public float Calculate32bit(short value1, short value2)
        {
            float value = -0.1f;

            try
            {
                byte[] bytes = new byte[4];
                Buffer.BlockCopy(new short[] { value1, value2 }, 0, bytes, 0, 4);

                value = BitConverter.ToSingle(bytes, 0);

            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
            }

            return value;
        }


        private void MonitorShow(int WhichLog, string text)
        {
            try
            {

                if (WhichLog == 1)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Monitor1.Text += $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text} \n";

                    });

                    Log("Monitor1", text);

                }

                if (WhichLog == 2)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Monitor2.Text += $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text} \n";

                    });

                    Log("Monitor2", text);

                }

                if (WhichLog == 3)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Monitor3.Text += $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text} \n";

                    });

                    Log("Monitor3", text);

                }

                if (WhichLog == 5)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Monitor5.Text += $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text} \n";

                    });

                    Log("Monitor5", text);

                }


                if (WhichLog == 6)
                {

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        Monitor6.Text += $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text} \n";

                    });

                    Log("Monitor6", text);

                }
            }

            catch (Exception ex)
            {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Log("ExceptionLog", $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void Log(string Path, string text){
            
        try
        {
            // 確保日誌資料夾存在
            if (!Directory.Exists(System.IO.Path.Combine(folderPath, "Logs", Path)))
            {   

             Directory.CreateDirectory(System.IO.Path.Combine(folderPath, "Logs", Path));

            }

            // 生成以日期命名的日誌檔案名稱
            string logFileName = System.IO.Path.Combine(folderPath , "Logs" , Path, $"{DateTime.Now:yyyy-MM-dd}.txt");

            // 將錯誤訊息追加到日誌檔案
            File.AppendAllText(logFileName, $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {text}\n");
        }
        catch (Exception ex)
        {
                MonitorShow(6, $"{ex.Message}\n{ex.StackTrace}");

                Console.WriteLine($"Failed to write to log file: {ex.Message}");
        }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Monitor1.Text = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Monitor2.Text = "";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Monitor3.Text = "";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Monitor5.Text = "";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Monitor6.Text = "";
        }
    }
}
