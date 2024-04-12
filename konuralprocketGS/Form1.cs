using ClosedXML.Excel;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace konuralprocketGS
{
    public partial class Form1 : Form
    {
        //this tefine the glcontrol class as gyro
        private GyroGL_Class gyroGL;

        private mapcontrol map;


        // veriables here to get value from gyro and oomfactor for glcontrol and marker for map
        private SerialPort serialPort;
        public double x = 0.0f, y = 0.0f, z = 0.0f;
        private float zoomFactor = 2f;
        private string datarecive;
        private readonly GMapOverlay markerOverlay = new GMapOverlay("marker");
        private readonly GMarkerGoogle currentPosationMarker = new GMarkerGoogle(new PointLatLng(40.839989, 31.155060), GMarkerGoogleType.blue_dot);

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            Initializesatalite_satSerialPort();
            InitializeGMap();
            InitializeGMap2();

            //this code sends the Gmap option to the class
            map = new mapcontrol(mapcontrolrocket, mapcontrolstaite);


            //this code sends the glcontrol option to the class
            gyroGL = new GyroGL_Class(glControl1);


            // Set the interval in milliseconds (adjust as needed)
            serialtimer.Tick += DataTimer_Tick;
            serialtimer.Interval = 200;
            serialtimer.Start();

            //this code gets the value from label the speed and altitude and sends it to class
            gyroGL.altitude = label47.Text;
            gyroGL.speed = label44.Text;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
          
            PopulateCOMPorts();
            PopulateBaudRates();
        }



        private void DataTimer_Tick(object sender, EventArgs e)
        {
            // This event will be triggered every 100 milliseconds (adjust as needed)
            // Perform your data receiving and updating here
            if (serialPort.IsOpen)
            {

                if (serialPort.BytesToRead > 0)
                {
                    datarecive = serialPort.ReadLine();
                    // parsedata(datarecive);
                }
            }
        }

        #region rocket serialport

        #region mapcontrol
        private void InitializeGMap()
        {
            // Set the map provider (you can choose other providers)
            mapcontrolrocket.MapProvider = GMapProviders.GoogleMap;

            // Set the initial position and zoom level
            mapcontrolrocket.Position = new PointLatLng(40.901233, 31.167545); // Default to center of the world
            mapcontrolrocket.MinZoom = 1;
            mapcontrolrocket.MaxZoom = 20;
            mapcontrolrocket.Zoom = 12;

            // Add GMapControl to the form
            mapcontrolrocket.Overlays.Add(markerOverlay);
        }
        #endregion

        #region old parsing and puting in label
        //private void InitializeSerialPort()
        //{
        //    serialPort = new SerialPort();
        //    serialPort.DataReceived += SerialPort_DataReceived;
        //}

        //private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        string dataReceived = await ReadLineAsync(serialPort.BaseStream);
        //        this.Invoke(new Action(() => parsedata(dataReceived)));

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception details
        //        Console.WriteLine($"Exception in SerialPort_DataReceived: {ex.Message}");
        //    }
        //}

        //private async Task<string> ReadLineAsync(Stream stream)
        //{
        //    byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
        //    StringBuilder line = new StringBuilder();


        //    try
        //    {
        //        while (true)
        //        {
        //            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

        //            if (bytesRead == 0)
        //            {
        //                // Disconnection detected
        //                return null; // Or throw an exception if disconnection should be treated as an error

        //            }

        //            string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
        //            line.Append(data);

        //            int newlineIndex;
        //            while ((newlineIndex = line.ToString().IndexOf('\n')) >= 0)
        //            {
        //                string lineStr = line.ToString(0, newlineIndex);
        //                line.Remove(0, newlineIndex + 1); // Remove the processed line including the newline character
        //                return lineStr;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions, e.g., log the error
        //        Console.WriteLine($"Error in ReadLineAsync: {ex.Message}");
        //        return null;
        //    }

        //}

        //private void parsedata(string data)
        //{
        //    Console.WriteLine($"recived data {data}");
        //    richTextBox1.AppendText($"recived data {data}\n");
        //    // Split the data using the ',' delimiter
        //    string[] values = data.Split(',');
        //    try
        //    {
        //        if (values != null && values.Length >= 4) // Check if the array is not null and has at least 6 elements
        //        {
        //            updategyro(values[0], values[1], values[2]);
        //            updatebmp(values[3], values[4]);
        //            // gps(values[6], values[7]);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("there is a NULL or your incress your aray size");
        //    }

        //}

        //// gyro controll
        //private void updategyro(string X, string Y, string Z)
        //{
        //    label45.Text = X;
        //    label49.Text = Y;
        //    label50.Text = Z;
        //    //label44.Text = hiz;

        //    //label15.Text = X;
        //    //label16.Text = Y;
        //    //label17.Text = Z;


        //    double.TryParse(X, out double a);
        //    x = a;
        //    double.TryParse(Y, out double b);
        //    y = b;
        //    double.TryParse(Z, out double c);
        //    z = c;



        //    // Trigger a repaint of the OpenGL control on a separate thread
        //    glControl1.Invalidate();

        //    //this code put X,Y,Z values in selicted parts of the datagridview
        //    int rowIndex = dataGridView1.Rows.Add();
        //    dataGridView1.Rows[rowIndex].Cells["Column18"].Value = X;
        //    dataGridView1.Rows[rowIndex].Cells["Column19"].Value = Y;
        //    dataGridView1.Rows[rowIndex].Cells["Column20"].Value = Z;
        //}

        //// tempreture and pressure controll
        //private void updatebmp(string tempreture, string pressure)
        //{
        //    label43.Text = tempreture;
        //    label41.Text = pressure;
        //    //label47.Text = altitude;

        //    glControl1.Invalidate();
        //    int rowIndex = dataGridView1.Rows.Add();
        //    dataGridView1.Rows[rowIndex].Cells["Column13"].Value = tempreture;
        //    dataGridView1.Rows[rowIndex].Cells["Column6"].Value = pressure;
        //    //dataGridView1.Rows[rowIndex].Cells["Column8"].Value = altitude;
        //    //dataGridView1.Rows[rowIndex].Cells["Column10"].Value = altitude;

        //}

        //// gps controll
        //private void gps(string lat, string lng)
        //{
        //    double lat1, lng1;

        //    // Use double.TryParse to handle potential parsing errors
        //    if (double.TryParse(lat, out lat1) && double.TryParse(lng, out lng1))
        //    {
        //        // Check if both latitude and longitude are not zero
        //        if (lat1 != 0.0 || lng1 != 0.0)
        //        {
        //            // txtlat.Text = lat1.ToString();
        //            //txtlong.Text = lng1.ToString();

        //            map.UpdateMapPosition(lat1, lng1);

        //            // gps controll 2 for the second map update it when its necessary
        //            label39.Text = lat1.ToString();
        //            label7.Text = lng1.ToString();

        //            map.UpdateMapPosition2(lat1, lng1);

        //            int rowIndex = dataGridView1.Rows.Add();
        //            dataGridView1.Rows[rowIndex].Cells["Column15"].Value = lat1;
        //            dataGridView1.Rows[rowIndex].Cells["Column16"].Value = lng1;
        //        }
        //    }
        //}
        #endregion

        #region new implementation
        private void InitializeSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;

            // Configure and start the timer
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 150; // Set the interval to 150 milliseconds
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }


        private Thread connectionThread;
        //serialport connect and disconnect button
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex >= 0)
            {
                if (connectionThread == null || !connectionThread.IsAlive)
                {
                    connectionThread = new Thread(ConnectOrDisconnect);
                    connectionThread.Start();
                }
            }
            else
            {
                MessageBox.Show("Please select a COM port.");
            }
        }

        private void ConnectOrDisconnect()
        {
            if (!serialPort.IsOpen)
            {
                ConnectSerialPort();
                if (this.InvokeRequired)
                {
                    label55.Invoke((MethodInvoker)(() => label55.Font = new Font(label55.Font.FontFamily, 10)));
                    label55.Invoke((MethodInvoker)(() => label55.ForeColor = Color.Green));
                    label55.Invoke((MethodInvoker)(() => label55.Text = "roket Connect"));
                }
            }
            else
            {
                DisconnectSerialPort();
                if (this.InvokeRequired)
                {
                    label55.Invoke((MethodInvoker)(() => label55.Font = new Font(label55.Font.FontFamily, 10)));
                    label55.Invoke((MethodInvoker)(() => label55.ForeColor = Color.Red));
                    label55.Invoke((MethodInvoker)(() => label55.Text = "roket Disconnect"));
                }
            }
        }
        private void ConnectSerialPort()
        {
            if (!serialPort.IsOpen)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // Set the selected COM port
                    serialPort.PortName = comboBox6.SelectedItem.ToString();
                    serialPort.Open();
                    MessageBox.Show($"Connected to {serialPort.PortName}");
                    button3.BackColor = Color.Red;
                    button3.Text = "Bağlantıyı kes";
                });
            }
            else
                throw new Exception("error port denied connection");
        }
        private void DisconnectSerialPort()
        {
            if (serialPort.IsOpen)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    serialPort.Close();
                    MessageBox.Show($"Disconnected from {serialPort.PortName}");
                    button3.BackColor = Color.Green;
                    button3.Text = "Bağlan";
                });
            }
        }

        // Define variables to store received data
        private string gyroX = 0.ToString();
        private string gyroY = 0.ToString();
        private string gyroZ = 0.ToString();
        private string temperature = 0.ToString();
        private string pressure = 0.ToString();

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string dataReceived = await ReadLineAsync(serialPort.BaseStream);
                ParseData(dataReceived);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception in SerialPort_DataReceived: {ex.Message}");
            }
        }

        private async Task<string> ReadLineAsync(Stream stream)
        {
            byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
            StringBuilder line = new StringBuilder();


            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        // Disconnection detected
                        return null; // Or throw an exception if disconnection should be treated as an error

                    }

                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    line.Append(data);

                    int newlineIndex;
                    while ((newlineIndex = line.ToString().IndexOf('\n')) >= 0)
                    {
                        string lineStr = line.ToString(0, newlineIndex);
                        line.Remove(0, newlineIndex + 1); // Remove the processed line including the newline character
                        return lineStr;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                Console.WriteLine($"Error in ReadLineAsync: {ex.Message}");
                return null;
            }

        }

        // Timer elapsed event handler
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Update labels with received data
            UpdateLabels();
        }

        // Method to parse received data
        private void ParseData(string data)
        {
            try
            {
                string[] values = data.Split(',');
                if (values.Length <= 5)
                {
                    gyroX = values[0];
                    gyroY = values[1];
                    gyroZ = values[2];
                    temperature = values[3];
                    pressure = values[4];
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void UpdateLabels()
        {
            if (this.InvokeRequired)
            {
                // Update gyro labels
                label45.Invoke((MethodInvoker)(() => label45.Text = gyroX));
                label49.Invoke((MethodInvoker)(() => label49.Text = gyroY));
                label50.Invoke((MethodInvoker)(() => label50.Text = gyroZ));

                // Update temperature and pressure labels
                label43.Invoke((MethodInvoker)(() => label43.Text = temperature));
                label41.Invoke((MethodInvoker)(() => label41.Text = pressure));
            }
        }

        #endregion

        #endregion

        #region Satalite serialpoer

        #region opengl
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            string altitudeText = label47.Text;
            double altitudeValue = 0;
            double.TryParse(altitudeText, out altitudeValue);
            int altitudevalue = (int)Math.Round(altitudeValue);

            string speedtext = label44.Text;
            double speedValue = 0;
            double.TryParse(speedtext, out speedValue);
            int speedvalue = (int)Math.Round(speedValue);

            // Clear the buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Set up perspective and look-at matrices
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.2f, glControl1.Width / (float)glControl1.Height, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25 * zoomFactor, 0, 0, 0, 0, 0, 0, 1, 0);

            // Load matrices
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Draw background colors
            gyroGL.DrawBackground();

            // Rotate based on gyro values
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);

            // Draw rocket components and coordinate axes
            gyroGL.DrawRocketComponents(0.01f, 0.01f, 3.1f);
            // gyroGL.DrawCoordinateAxes();

            // Swap buffers
            glControl1.SwapBuffers();

            // Draw speed and altitude indicators
            gyroGL.DrawSpeedIndicator(e.Graphics, speedtext, speedvalue);
            gyroGL.DrawAltitudeIndicator(e.Graphics, altitudeText, altitudevalue);


            GL.End();
        }
        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            // You can adjust the zoom sensitivity by changing the value in the next line
            float zoomSensitivity = 0.01f;

            // Update the zoom factor based on the mouse wheel delta
            zoomFactor += e.Delta * zoomSensitivity;

            // Ensure the zoom factor is within a reasonable range
            zoomFactor = Math.Max(zoomFactor, 0.8f);
            zoomFactor = Math.Min(zoomFactor, 3.0f);

            // Redraw the OpenGL control
            glControl1.Invalidate();
        }

        #endregion

        #region mapcontrol 2
        private void InitializeGMap2()
        {
            // Set the map provider (you can choose other providers)
            mapcontrolstaite.MapProvider = GMapProviders.GoogleMap;

            // Set the initial position and zoom level
            mapcontrolstaite.Position = new PointLatLng(40.901233, 31.167545); // Default to center of the world
            mapcontrolstaite.MinZoom = 1;
            mapcontrolstaite.MaxZoom = 20;
            mapcontrolstaite.Zoom = 12;

            // Add GMapControl to the form
            mapcontrolstaite.Overlays.Add(markerOverlay);
        }
        #endregion

        private Thread sat_connectionThread;
        //serialport connect and disconnect button
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex >= 0)
            {
                if (sat_connectionThread == null || !sat_connectionThread.IsAlive)
                {
                    connectionThread = new Thread(ConnectOrDisconnect_sat);
                    connectionThread.Start();
                }
            }
            else
            {
                MessageBox.Show("Please select a COM port.");
            }
        }

        private void ConnectOrDisconnect_sat()
        {
            if (!satserialport.IsOpen)
            {
                ConnectSerialPort_sat();
                if (this.InvokeRequired)
                {
                    label19.Invoke((MethodInvoker)(() => label19.Font = new Font(label19.Font.FontFamily, 10)));
                    label19.Invoke((MethodInvoker)(() => label19.ForeColor = Color.Green));
                    label19.Invoke((MethodInvoker)(() => label19.Text = " Uydu Connect"));  
                }
            }
            else
            {
                DisconnectSerialPort_sat();
                if (this.InvokeRequired)
                {
                    label19.Invoke((MethodInvoker)(() => label19.Font = new Font(label19.Font.FontFamily, 10)));
                    label19.Invoke((MethodInvoker)(() => label19.ForeColor = Color.Red));
                    label19.Invoke((MethodInvoker)(() => label19.Text = " Uydu Disconnect"));  
                }
            }
        }
        private void ConnectSerialPort_sat()
        {
            if (!satserialport.IsOpen)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // Set the selected COM port
                    satserialport.PortName = comboBox6.SelectedItem.ToString();
                    satserialport.Open();
                    MessageBox.Show($"Connected to {satserialport.PortName}");
                    button4.BackColor = Color.Red;
                    button4.Text = "Yuk Bağlantıyı kes";
                });
            }
            else
                throw new Exception("error port denied connection");
        }
        private void DisconnectSerialPort_sat()
        {
            if (satserialport.IsOpen)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    satserialport.Close();
                    MessageBox.Show($"Disconnected from {satserialport.PortName}");
                    button4.BackColor = Color.Green;
                    button4.Text = "Yuk Bağlan";
                });
            }
        }

        private void Initializesatalite_satSerialPort()
        {
            satserialport = new SerialPort();
            satserialport.DataReceived += satserialport_DataReceived;

            // Configure and start the timer
            sat_timer = new System.Timers.Timer();
            sat_timer.Interval = 200; // Set the interval to 200 milliseconds
            sat_timer.Elapsed += sat_timer_Elapsed;
            sat_timer.Start();
        }

        private async void satserialport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string sat_dataReceived = await sat_ReadLineAsync(satserialport.BaseStream); // Use satserialport instead of serialPort
                sat_ParseData(sat_dataReceived);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception in SerialPort_DataReceived: {ex.Message}");
            }
        }

        private async Task<string> sat_ReadLineAsync(Stream stream)
        {
            byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
            StringBuilder line = new StringBuilder();


            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                    {
                        // Disconnection detected
                        return null; // Or throw an exception if disconnection should be treated as an error

                    }

                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    line.Append(data);

                    int newlineIndex;
                    while ((newlineIndex = line.ToString().IndexOf('\n')) >= 0)
                    {
                        string lineStr = line.ToString(0, newlineIndex);
                        line.Remove(0, newlineIndex + 1); // Remove the processed line including the newline character
                        return lineStr;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error
                Console.WriteLine($"Error in ReadLineAsync: {ex.Message}");
                return null;
            }

        }

        // Define variables to store received satalite data
        private string sat_gyroX = 0.ToString();
        private string sat_gyroY = 0.ToString();
        private string sat_gyroZ = 0.ToString();
        private string sat_temperature = 0.ToString();
        private string sat_pressure = 0.ToString();
        private System.Timers.Timer sat_timer;

        private void sat_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Update labels with received satalite data
            sat_updatelabel();
        }

        private void sat_ParseData(string sat_dataReceived)
        {
            try
            {
                string[] sat_values = sat_dataReceived.Split(",");

                if (sat_values.Length <= 5)
                {
                    sat_gyroX = sat_values[0];
                    sat_gyroY = sat_values[1];
                    sat_gyroZ = sat_values[2];
                    sat_temperature = sat_values[3];
                    sat_pressure = sat_values[4];
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void sat_updatelabel()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    // Update gyro labels
                    label15.Invoke((MethodInvoker)(() => label15.Text = sat_gyroX));
                    label16.Invoke((MethodInvoker)(() => label16.Text = sat_gyroY));
                    label4.Invoke((MethodInvoker)(() => label4.Text = sat_gyroZ));

                    // Update temperature and pressure labels
                    label28.Invoke((MethodInvoker)(() => label28.Text = sat_temperature));
                    label33.Invoke((MethodInvoker)(() => label33.Text = sat_pressure));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(MessageBox.Show("there is error while invoking the labels" + ex));
            }
        }

        #endregion


        private void PopulateCOMPorts()
        {
            comboBox6.Items.Clear();

            try
            {
                // Get available COM ports and add them to the ComboBox
                string[] ports = SerialPort.GetPortNames();
                comboBox6.Items.AddRange(ports);

                // Select the first port by default if available
                if (ports.Length > 0)
                    comboBox6.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting COM ports: {ex.Message}");
            }
        }

        private void PopulateBaudRates()
        {
            // Add commonly used baud rates to the Baud Rate ComboBox
            comboBox7.Items.AddRange(new string[] { "9600", "115200", "19200" }); // Add more if needed

            // Select the first baud rate by default
            comboBox7.SelectedIndex = 0;
        }

        //private void sendCommand(char command)
        //{
        //    try
        //    {

        //        serialPort.Write($"{command}");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error sending command to Arduino: {ex.Message}");
        //    }
        //}

        //this button closes the serialport
        private void mapcontrol_Load(object sender, EventArgs e)
        {

        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }


        private void button13_Click(object sender, EventArgs e)
        {
            temizle1();
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }




        private void glControl1_Load(object sender, EventArgs e)
        {
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            //GL.Enable(EnableCap.DepthTest);
        }


        // this button get the values from datagridview to text file
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                // Choose a location to save the text file
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "text Files|*.txt";
                saveFileDialog.Title = "Save Data as text  File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Open the file stream for writing
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the header
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            sw.Write(column.HeaderText + "\t");
                        }
                        sw.WriteLine();

                        // Write each row
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                sw.Write(cell.Value + "\t");
                            }
                            sw.WriteLine();
                        }
                    }

                    MessageBox.Show("Data exported to " + saveFileDialog.FileName, "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        // this button get the values from datagridview to exel file
        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                // Choose a location to save the Excel file
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save Data as Excel File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Create a new Excel workbook
                    var workbook = new XLWorkbook();

                    // Add a worksheet to the workbook
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    // Write the header
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                    }

                    // Write each row
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            worksheet.Cell(i + 2, j + 1).Value = dataGridView1.Rows[i].Cells[j].Value.ToString();
                        }
                    }

                    // Save the workbook to the specified file
                    workbook.SaveAs(saveFileDialog.FileName);

                    MessageBox.Show("Data exported to " + saveFileDialog.FileName, "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button23_Click(object sender, EventArgs e) => PopulateCOMPorts();

        private void button5_Click_1(object sender, EventArgs e)
        {
            sat_gyroX = 0.ToString();
            sat_gyroY = 0.ToString();
            sat_gyroZ = 0.ToString();
            sat_temperature = 0.ToString();
            sat_pressure = 0.ToString();
        }

        private void temizle1()
        {
            gyroX = 0.ToString();
            gyroY = 0.ToString();
            gyroZ = 0.ToString();
            temperature = 0.ToString();
            pressure = 0.ToString();
        }

        private void show_portaktivity()
        {
            //if (!satserialport.IsOpen)
            //{
              
            //    label12.ForeColor = Color.Red;
            //    label12.Text = "Disconnected";
            //}
            //else if(satserialport.IsOpen)
            //{
            //    label12.ForeColor = Color.Green;
            //    label12.Text = "Connected";
            //}
        }


    }
}