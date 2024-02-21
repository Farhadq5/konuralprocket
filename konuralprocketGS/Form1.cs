using ClosedXML.Excel;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
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
        private float zoomFactor = 1.0f;
        private string datarecive;
        private readonly GMapOverlay markerOverlay = new GMapOverlay("marker");
        private readonly GMarkerGoogle currentPosationMarker = new GMarkerGoogle(new PointLatLng(40.839989, 31.155060), GMarkerGoogleType.blue_dot);

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeGMap();
            InitializeGMap2();

            //this code sends the Gmap option to the class
            map = new mapcontrol(mapcontrolrocket, mapcontrolstaite);


            //this code sends the glcontrol option to the class
            gyroGL = new GyroGL_Class(glControl1);



            //this code works in bakground for opening and closing the serialport
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            // Set the interval in milliseconds (adjust as needed)
            serialtimer.Tick += DataTimer_Tick;
            serialtimer.Interval = 100;
            serialtimer.Start();

            //this code gets the value from label the speed and altitude and sends it to class
            gyroGL.altitude = label47.Text;
            gyroGL.speed = label44.Text;

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            PopulateCOMPorts();
            PopulateBaudRates();
            timer1.Interval = 1;

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
        #region opengl
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            string altitude = label44.Text;
            int altitudeValue = int.Parse(altitude);
            string speed = label47.Text;
            int speedvalue = int.Parse(speed);

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
            gyroGL.DrawRocketComponents(1.0f, 1.0f, 4.0f);
            gyroGL.DrawCoordinateAxes();

            // Swap buffers
            glControl1.SwapBuffers();

            // Draw speed and altitude indicators
            gyroGL.DrawSpeedIndicator(e.Graphics, speed, speedvalue);
            gyroGL.DrawAltitudeIndicator(e.Graphics, altitude, altitudeValue);

            GL.End();
        }
        #endregion

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
        private void InitializeSerialPort()
        {
            serialPort = new SerialPort();
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string dataReceived = await ReadLineAsync(serialPort.BaseStream);
                this.Invoke(new Action(() => parsedata(dataReceived)));
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception in SerialPort_DataReceived: {ex.Message}");
            }
        }

        private async Task<string> ReadLineAsync(Stream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            char lastchar = '\0';

            while (true)
            {
                bytesRead += await stream.ReadAsync(buffer, bytesRead, 1);

                if (bytesRead > 0)
                {
                    char currentCahr = (char)buffer[bytesRead - 1];

                    if (currentCahr == '\n' && lastchar == '\r')
                    {
                        //return a newline character return the compleat line
                        return Encoding.ASCII.GetString(buffer, 0, bytesRead - 2);
                    }
                    lastchar = currentCahr;
                }
            }
        }


        private void parsedata(string data)
        {
            Console.WriteLine($"recived data {data}");
            richTextBox1.AppendText($"recived data {data}\n");
            // Split the data using the ',' delimiter
            string[] values = data.Split(',');

            if (values.Length >= 8) // Check if there are at least 5 elements in the array
            {
                updategyro(values[0], values[1], values[2]);
                updatebmp(values[3], values[4], values[5]);
                gps(values[6], values[7]);
            }

        }

        // gyro controll
        private void updategyro(string X, string Y, string Z)
        {
            label45.Text = X;
            label49.Text = Y;
            label50.Text = Z;
            label15.Text = X;
            label16.Text = Y;
            label17.Text = Z;

            //this code sends the X,Y,Z values to the gyroclass
            if (gyroGL.parsing_gyro(X, Y, Z) == 1)
            {
                gyroGL.X_value = x;
                gyroGL.Y_value = y;
                gyroGL.Z_value = z;
            }
            else
            {
                Console.WriteLine("there is something wrong in here ");
            }


            // Trigger a repaint of the OpenGL control on a separate thread
            Task.Run(() => glControl1.Invalidate());

            //this code put X,Y,Z values in selicted parts of the datagridview
            int rowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowIndex].Cells["Column18"].Value = X;
            dataGridView1.Rows[rowIndex].Cells["Column19"].Value = Y;
            dataGridView1.Rows[rowIndex].Cells["Column20"].Value = Z;
        }

        // tempreture and pressure controll
        private void updatebmp(string temp, string pressure, string altitude)
        {
            label43.Text = temp;
            label41.Text = pressure;
            label47.Text = altitude;


            int rowIndex = dataGridView1.Rows.Add();
            dataGridView1.Rows[rowIndex].Cells["Column13"].Value = temp;
            dataGridView1.Rows[rowIndex].Cells["Column6"].Value = pressure;
            dataGridView1.Rows[rowIndex].Cells["Column8"].Value = altitude;
            dataGridView1.Rows[rowIndex].Cells["Column10"].Value = altitude;

        }

        // gps controll
        private void gps(string lat, string lng)
        {
            double lat1, lng1;

            // Use double.TryParse to handle potential parsing errors
            if (double.TryParse(lat, out lat1) && double.TryParse(lng, out lng1))
            {
                // Check if both latitude and longitude are not zero
                if (lat1 != 0.0 || lng1 != 0.0)
                {
                    txtlat.Text = lat1.ToString();
                    txtlong.Text = lng1.ToString();

                    map.UpdateMapPosition(lat1, lng1);

                    // gps controll 2 for the second map update it when its necessary
                    label39.Text = lat1.ToString();
                    label7.Text = lng1.ToString();

                    map.UpdateMapPosition2(lat1, lng1);

                    int rowIndex = dataGridView1.Rows.Add();
                    dataGridView1.Rows[rowIndex].Cells["Column15"].Value = lat1;
                    dataGridView1.Rows[rowIndex].Cells["Column16"].Value = lng1;
                }
            }
        }

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

        //serialport connect and disconnect button
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox6.SelectedIndex >= 0)
            {
                if (!serialPort.IsOpen)
                {
                    // Set the selected COM port
                    serialPort.PortName = comboBox6.SelectedItem.ToString();

                    // Start background worker to open the serial port
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    DisconnectSerialPort();
                }
            }
            else
            {
                MessageBox.Show("Please select a COM port.");
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                serialPort.Open();
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = ex.Message;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is bool && (bool)e.Result)
            {
                MessageBox.Show($"Connected to {serialPort.PortName}");
                button3.BackColor = Color.Red;
                button3.Text = "Bağlantıyı kes";
            }
            else
            {
                MessageBox.Show($"Error opening serial port: {e.Result}");
            }
        }

        private void DisconnectSerialPort()
        {
            serialPort.Close();
            MessageBox.Show($"Disconnected from {serialPort.PortName}");
            button3.BackColor = Color.Green; ; // Change button back color to default
            button3.Text = "Bağlan"; // Change button text
        }


        private void button13_Click(object sender, EventArgs e)
        {
            if (int.TryParse(label44.Text, out int altitudeValue))
            {
                // Increment the altitude value by 20
                altitudeValue += 25;

                // Update label47 with the new altitude value
                label44.Text = altitudeValue.ToString();

                
            }

            if (int.TryParse(label47.Text, out int speed))
            {
                // Increment the altitude value by 20
                speed += 1;

                // Update label47 with the new altitude value
                label47.Text = speed.ToString();

               
            }
            // Trigger the paint event to redraw the altitude indicator with the updated altitude value
            glControl1.Invalidate();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (int.TryParse(label44.Text, out int altitudeValue))
            {
                // Increment the altitude value by 20
                altitudeValue -= 20;

                // Update label47 with the new altitude value
                label44.Text = altitudeValue.ToString();

                
            }

            if (int.TryParse(label47.Text, out int speed))
            {
                // Increment the altitude value by 20
                speed -= 20;

                // Update label47 with the new altitude value
                label47.Text = altitudeValue.ToString();

                
            }
            // Trigger the paint event to redraw the altitude indicator with the updated altitude value
            glControl1.Invalidate();
        }

        // this button get the values from datagridview to exel file
        private void button6_Click_1(object sender, EventArgs e)
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


        private void glControl1_Load(object sender, EventArgs e)
        {
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            //GL.Enable(EnableCap.DepthTest);
        }


        // this button get the values from datagridview to text file
        private void button15_Click_1(object sender, EventArgs e)
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
        private void temizle1()
        {
            label41.Text = "0";
            label42.Text = "0";
            label43.Text = "0";
            label44.Text = "0";
            label45.Text = "0";
            label46.Text = "0";
            label47.Text = "0";
            label48.Text = "0";
            label49.Text = "0";
            label50.Text = "0";
            label7.Text = "0";
            label39.Text = "0";
            label40.Text = "0";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i <= 3000; i += 20)
            {
                label44.Text = i.ToString();
            }
        }

    }
}