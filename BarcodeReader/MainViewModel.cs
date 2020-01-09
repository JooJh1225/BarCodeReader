using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ZXing;
using System.Windows.Threading;
using System.Linq;

namespace BarcodeReader
{    
    class MainViewModel : INotifyPropertyChanged
    {
        public SerialPort serial = new SerialPort();
        public TextControl textControl = new TextControl();
        public BarcodeControl barcodeControl = new BarcodeControl();
        public DispatcherTimer timer = new DispatcherTimer();        
        
        
        
        private List<BarcodeDataModel> barcodeDatas = new List<BarcodeDataModel>();

        private bool threadBool;
        private BitmapImage barcodeImageSource;
        private string hidText;
        private int portIndex;
        private int baudrateIndex;
        private int databitsIndex;
        private int paritybitsIndex;
        private int stopbitsindex;
        private bool dropdownBool;
        private bool checkBool;
        private string[] port;
        private List<string> baudRateList = new List<string>()
        {
            "110",
            "300",
            "1200",
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600"
        };
        private List<int> dataBitsList = new List<int>()
        {
            5,
            6,
            7,
            8
        };
        private List<Parity> parityBitsList = new List<Parity>()
        {
            Parity.None,
            Parity.Even,
            Parity.Odd,
            Parity.Mark,
            Parity.Space
        };
        private List<StopBits> stopBitsList = new List<StopBits>()
        {
            StopBits.One,
            StopBits.Two,
            StopBits.OnePointFive
        };
        private string getPort;
        private string getBaudRate;
        private int getDataBits;
        private System.IO.Ports.Parity getparityBits;
        private System.IO.Ports.StopBits getStopBits;
        private string textBlockBarcodeData;
        private string textBlockProductName;
        private string productBarcodeData;
        private string receivedData;

        public bool ThreadBool { get => threadBool; set { threadBool = value; OnPropertyChangd(nameof(ThreadBool)); } }
        public BitmapImage BarcodeImageSource { get => barcodeImageSource; set { barcodeImageSource = value; OnPropertyChangd(nameof(barcodeImageSource)); } }
        public string HidText { get => hidText; set { hidText = value; OnPropertyChangd(nameof(hidText)); } }
        public int PortIndex { get => portIndex; set { portIndex = value; OnPropertyChangd(nameof(PortIndex)); } }
        public int BaudrateIndex { get => baudrateIndex; set { baudrateIndex = value; OnPropertyChangd(nameof(BaudrateIndex)); } }
        public int DatabitsIndex { get => databitsIndex; set { databitsIndex = value; OnPropertyChangd(nameof(DatabitsIndex)); } }
        public int ParitybitsIndex { get => paritybitsIndex; set { paritybitsIndex = value; OnPropertyChangd(nameof(ParitybitsIndex)); } }
        public int StopbitsIndex { get => stopbitsindex; set { stopbitsindex = value; OnPropertyChangd(nameof(StopbitsIndex)); } }
        public bool DropdownBool { get => dropdownBool; set { dropdownBool = value; OnPropertyChangd(nameof(DropdownBool)); } }
        public bool CheckBool { get => checkBool; set { checkBool = value; OnPropertyChangd(nameof(CheckBool)); } }
        public List<BarcodeDataModel> BarcodeDatas { get => barcodeDatas; set { barcodeDatas = value; OnPropertyChangd(nameof(BarcodeDatas)); } }
        public string[] Port
        {
            get { return port; }
            set
            {
                port = value;
                OnPropertyChangd(nameof(Port));
            }
        }
        public List<string> BaudRateList { get => baudRateList; set { baudRateList = value; OnPropertyChangd(nameof(BaudRateList)); } }
        public List<int> DataBitsList { get => dataBitsList; set { dataBitsList = value; OnPropertyChangd(nameof(DataBitsList)); } }
        public List<Parity> ParityBitsList { get => parityBitsList; set { parityBitsList = value; OnPropertyChangd(nameof(ParityBitsList)); } }
        public List<StopBits> StopBitsList { get => stopBitsList; set { stopBitsList = value; OnPropertyChangd(nameof(StopBitsList)); } }
        public string GetPort { get => getPort; set { getPort = value; OnPropertyChangd(nameof(GetPort)); } }
        public string GetBaudRate { get => getBaudRate; set { getBaudRate = value; OnPropertyChangd(nameof(GetBaudRate)); } }
        public int GetDataBits { get => getDataBits; set { getDataBits = value; OnPropertyChangd(nameof(GetDataBits)); } }
        public System.IO.Ports.Parity GetparityBits { get => getparityBits; set { getparityBits = value; OnPropertyChangd(nameof(GetparityBits)); } }
        public System.IO.Ports.StopBits GetStopBits { get => getStopBits; set { getStopBits = value; OnPropertyChangd(nameof(GetStopBits)); } }
        public string TextBlockBarcodeData { get => textBlockBarcodeData; set { textBlockBarcodeData = value; OnPropertyChangd(nameof(textBlockBarcodeData)); } }
        public string TextBlockProductName { get => textBlockProductName; set { textBlockProductName = value; OnPropertyChangd(nameof(TextBlockProductName)); } }   
        public string ProductBarcodeData { get => productBarcodeData; set { productBarcodeData = value; OnPropertyChangd(nameof(ProductBarcodeData)); } }
        public string ReceivedData
        {
            get { return receivedData; }
            set
            {
                receivedData = value;
                OnPropertyChangd(nameof(ReceivedData));
                DataSearch(value);
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;

        public Window mainWin;
        public Window barcodeWin;

        public MainViewModel()
        {
            Port = SerialPort.GetPortNames();
            ThreadBool = false;
            CheckBool = true;
            PortIndex = -1;
            BaudrateIndex = 5;
            DatabitsIndex = 3;
            ParitybitsIndex = 0;
            StopbitsIndex = 0;
            serial.NewLine = "\r";            
            serial.DataReceived += new SerialDataReceivedEventHandler(DataReceived);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                BarcodeDatas = textControl.TxtControl();
                MessageBox.Show("로딩 완료");
                ThreadBool = true;
            }));      
            timer.Interval = TimeSpan.FromSeconds(4);
            timer.Tick += new EventHandler(BarCodeReaderSet_timer_tick);            
            timer.Start();         
           

        }

        public ICommand RunBtnClick { protected get; set; }

        protected void OnPropertyChangd(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void OpenComPort(string port, int baudRate, int dataBits, System.IO.Ports.Parity parityBits, System.IO.Ports.StopBits stopBits)
        {
            serial.PortName = port;
            serial.BaudRate = baudRate;
            serial.DataBits = dataBits;
            serial.Parity = parityBits;
            serial.StopBits = stopBits;
            serial.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                ReceivedData = serial.ReadLine();
                HidText = "";
            }
            catch (TimeoutException)
            {
                ReceivedData = string.Empty;
            }
        }

        private int DataSearch(string data)
        {          
            int x = 0;
            int i = 0;
            TextBlockBarcodeData = "";
            TextBlockProductName = "";
            foreach (var items in BarcodeDatas)
            {
                if ((BarcodeDatas[i].BarcodeData) == data)
                {
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        TextBlockBarcodeData = BarcodeDatas[i].BarcodeData;
                        TextBlockProductName = BarcodeDatas[i].ProductName;
                    }));
                    x++;
                }
                i++;                
            }            
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate
            {
                BarcodeImageSource = barcodeControl.GenBarcode(TextBlockBarcodeData);
            }));
            if (x < 1)
            {
                if(data == "")
                {
                    TextBlockProductName = null;
                    TextBlockBarcodeData = null;
                }
                else
                {
                    TextBlockBarcodeData = "일치하는 바코드가 없습니다";
                    BarcodeImageSource = null;
                    OnPropertyChangd(nameof(TextBlockBarcodeData));
                    OnPropertyChangd(nameof(BarcodeImageSource));                    
                }
                
            }
            return x;
            
        }

        private void MainWindow_timer_tick(object sender, EventArgs e)
        {
            if(CheckBool == true)
            {
                string[] subPort = SerialPort.GetPortNames();
                if (!Port.SequenceEqual(subPort))
                {
                    Port = SerialPort.GetPortNames();
                }
                try
                {
                    serial.Close();
                    serial.Open();
                }
                catch
                {
                    MessageBox.Show("포트와의 연결이 끊어졌습니다");
                    CmdClosemainWindow.Execute(null);
                }
            }            
        }
        private void BarCodeReaderSet_timer_tick(object sender, EventArgs e)
        {
            string[] subPort = SerialPort.GetPortNames();
            if (!Port.SequenceEqual(subPort))
            {
                Port = SerialPort.GetPortNames();
                GetPort = null;
                OnPropertyChangd(nameof(CmdOpenMainWindow.CanExecute));
            }
        }

        private DelegateCommand keyStroke;

        public DelegateCommand KeyStroke => keyStroke ?? (keyStroke = new DelegateCommand()
        {
            
            ExecuteAct = (p) =>
            {
                DataSearch(HidText);                            
            }
        });

        
        private DelegateCommand cmdOpenMainWindow;

        public DelegateCommand CmdOpenMainWindow => cmdOpenMainWindow ?? (cmdOpenMainWindow = new DelegateCommand()
        {
            CanExecuteFunc = (p) =>
            {
                if (CheckBool == false)
                {
                    DropdownBool = false;
                    PortIndex = -1;
                    BaudrateIndex = -1;
                    DatabitsIndex = -1;
                    ParitybitsIndex = -1;
                    StopbitsIndex = -1;
                    return true;
                }
                else
                {
                    
                    BaudrateIndex = 5;
                    DatabitsIndex = 3;
                    ParitybitsIndex = 0;
                    StopbitsIndex = 0;
                    bool a = false;
                    DropdownBool = true;
                    if (GetPort != null && GetBaudRate != null)
                    {
                        a = true;
                    }
                    return a;
                }
            },
            ExecuteAct = (p) =>
            {
                if (CheckBool == true)
                {
                    OpenComPort(GetPort, Convert.ToInt32(getBaudRate), GetDataBits, GetparityBits, GetStopBits);
                }
                mainWin = new MainWindow();
                mainWin.DataContext = this;
                mainWin.Show();
                barcodeWin?.Close();
                barcodeWin = null;                
                timer.Tick -= new EventHandler(BarCodeReaderSet_timer_tick);
                timer.Tick += new EventHandler(MainWindow_timer_tick);               


            }
        });

     

        private DelegateCommand cmdClosemainWindow;

        public DelegateCommand CmdClosemainWindow => cmdClosemainWindow ?? (cmdClosemainWindow = new DelegateCommand()
        {

            ExecuteAct = (p) =>
            {
                serial.Close();
                barcodeWin = new BarcodeReaderSet();
                barcodeWin.DataContext = this;
                barcodeWin.Show();
                mainWin?.Close();
                mainWin = null;
                TextBlockProductName = null;
                TextBlockBarcodeData = null;
                barcodeImageSource = null;
                HidText = null;
                timer.Tick -= new EventHandler(MainWindow_timer_tick);
                timer.Tick += new EventHandler(BarCodeReaderSet_timer_tick);
                string[] subPort = SerialPort.GetPortNames();
                if (!Port.SequenceEqual(subPort))
                {
                    Port = SerialPort.GetPortNames();
                }

            }
        });

       

        private DelegateCommand cmdShutDown;

        public DelegateCommand CmdShutDown => cmdShutDown ?? (cmdShutDown = new DelegateCommand()
        {
            ExecuteAct = (p) =>
            {
                App.Current.Shutdown();
            }
        });
       
    }
}