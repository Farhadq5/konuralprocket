using ClosedXML.Excel;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
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
        private SerialPort serialPort;
        public double x = 0.0f, y = 0.0f, z = 0.0f;
        bool by = false, bx = false, bz = false;
        private string datarecive;
        private float zoomFactor = 1.0f;
        Color renk1 = Color.White, renk2 = Color.Red;
        private readonly GMapOverlay markerOverlay = new GMapOverlay("marker");
        private readonly GMarkerGoogle currentPosationMarker = new GMarkerGoogle(new PointLatLng(40.839989, 31.155060), GMarkerGoogleType.blue_dot);

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeGMap();
            InitializeGMap2();

            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += backgroundWorker_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            // Set the interval in milliseconds (adjust as needed)
            serialtimer.Tick += DataTimer_Tick;
            serialtimer.Interval = 100;
            serialtimer.Start();
            //dataGridView1.Columns.Add("GPS1 Latitude", "GPS1 Latitude");
            //dataGridView1.Columns.Add("GPS1 Longitude", "GPS1 Longitude");

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
            string altitudeText = label47.Text;
            int altitudeValue = int.Parse(altitudeText);
            string speed = label44.Text;
            int speedvalue = int.Parse(speed);


            float step = 1.0f;
            float topla = step;
            float radius = 4.0f;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.2f, glControl1.Width / (float)glControl1.Height, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(25 * zoomFactor, 0, 0, 0, 0, 0, 0, 1, 0);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(z, 0.0, 1.0, 0.0);
            GL.Rotate(y, 0.0, 0.0, 1.0);



            DrawRocketComponents(step, topla, radius);

            DrawCoordinateAxes();

            glControl1.SwapBuffers();

            DrawSpeedIndicator(e.Graphics, speed, speedvalue);
            DrawAltitudeIndicator(e.Graphics, altitudeText, altitudeValue);

            GL.End();

        }



        private void DrawRocketComponents(float step, float topla, float radius)
        {
            silindir(step, topla, radius, 3, -8);
            koni(0.01f, 0.01f, radius, 3.0f, 3, 8);
            koni(0.01f, 0.01f, radius, 2.0f, -7.0f, -14.0f);
            silindir(0.01f, topla, 0.07f, 9, 3);
            silindir(0.01f, topla, 0.2f, 9, 9.3f);
            Pervane(9.0f, 7.0f, 0.3f, 0.3f);
            silindir(0.01f, topla, 0.2f, 7.3f, 7f);
            Pervane(7.0f, 7.0f, 0.3f, 0.3f);
        }

        private void DrawCoordinateAxes()
        {
            GL.Begin(BeginMode.Lines);

            // X-axis (red)
            GL.Color3(Color.Red);
            GL.Vertex3(-100, 0, 0);
            GL.Vertex3(100, 0, 0);

            // Y-axis (green)
            GL.Color3(Color.Green);
            GL.Vertex3(0, -100, 0);
            GL.Vertex3(0, 100, 0);

            // Z-axis (blue)
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, -100);
            GL.Vertex3(0, 0, 100);



            GL.End();

        }

        private void glControl1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // You can adjust the zoom sensitivity by changing the value in the next line
            float zoomSensitivity = 0.1f;

            // Update the zoom factor based on the mouse wheel delta
            zoomFactor += e.Delta * zoomSensitivity;

            // Ensure the zoom factor is within a reasonable range
            zoomFactor = Math.Max(zoomFactor, 0.1f);
            zoomFactor = Math.Min(zoomFactor, 10.0f);

            // Redraw the OpenGL control
            glControl1.Invalidate();
        }
        private void renk_ataması(float step)
        {
            if (step < 45)
                GL.Color3(renk2);
            else if (step < 90)
                GL.Color3(renk1);
            else if (step < 135)
                GL.Color3(renk2);
            else if (step < 180)
                GL.Color3(renk1);
            else if (step < 225)
                GL.Color3(renk2);
            else if (step < 270)
                GL.Color3(renk1);
            else if (step < 315)
                GL.Color3(renk2);
            else if (step < 360)
                GL.Color3(renk1);
        }
        private void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Quads);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180)//ALT KAPAK
            {
                renk_ataması(step);

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }
        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(BeginMode.Lines);//Y EKSEN CIZIM DAİRENİN
            while (step <= 360)
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();

            GL.Begin(BeginMode.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                renk_ataması(step);
                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }
        private void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            float radius = 10, angle = 45.0f;
            GL.Begin(BeginMode.Quads);

            GL.Color3(renk2);
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik, kalinlik);

            GL.Color3(renk2);
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, kalinlik);

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, -uzunluk);
            GL.Vertex3(-kalinlik, yukseklik + egiklik, 0.0);//+
            GL.Vertex3(kalinlik, yukseklik, 0.0);//-

            GL.Color3(renk1);
            GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, 0.0);
            GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);
            GL.End();

        }

        private void DrawAltitudeIndicator(Graphics g, string altitudeText, int altitudeValue)
        {
            // Define font and brushes for drawing the text
            Font largeFont = new Font("Arial", 12, FontStyle.Bold);
            Font smallFont = new Font("Arial", 9);
            Brush largeBrush = Brushes.White;
            Brush smallBrush = Brushes.Gray;

            // Define the coordinates for the altitude indicator
            int altitudeLineX1 = 20; // X-coordinate of the line start
            int altitudeLineY1 = 20; // Y-coordinate of the line start
            int altitudeLineY2 = glControl1.Height - 20; // Y-coordinate of the line end

            // Draw the background of the altitude indicator
            g.FillRectangle(Brushes.White, altitudeLineX1 - 2, altitudeLineY1, 2, altitudeLineY2 - altitudeLineY1);

            // Draw the altitude scale
            int scaleIncrement = altitudeValue <= 1000 ? 20 : 100; // Change the scale based on altitude
                                                                   //int maxAltitude = altitudeValue + 100; // Display 100 units above the current altitude
                                                                   //int minAltitude = altitudeValue - 100; // Display 100 units below the current altitude

            //for (int i = minAltitude; i <= maxAltitude; i += scaleIncrement)
            //{
            //    int y = altitudeLineY2 - i * (altitudeLineY2 - altitudeLineY1) / maxAltitude;

            //    if (i >= minAltitude && i <= maxAltitude)
            //    {
            //        if (i != altitudeValue) // Exclude the current altitude from moving
            //        {
            //            int x = altitudeLineX1 + 20; // X-coordinate for drawing text
            //            int textY = y - 7; // Y-coordinate for drawing text
            //            g.DrawString(i.ToString(), smallFont, smallBrush, x, textY); // Display other altitudes in a smaller font
            //        }
            //    }
            //}

            List<(int Altitude, float Y)> pitchLines = new List<(int, float)>();

            // Determine the total number of pitch lines to be drawn
            int totalPitchLines = 11; // We want exactly 11 lines to ensure the current altitude is at the center

            // Calculate the interval between each pitch line
            float pitchInterval = (float)(altitudeLineY2 - altitudeLineY1) / (totalPitchLines - 1);

            // Calculate the altitude value for the fifth line (middle line)
            int middleAltitude = altitudeValue;

            // Specify the vertical offset for moving the middle line (adjust as needed)
            float verticalOffset = 0; // Adjust the offset value to move the middle line up or down

            // Calculate the Y-coordinate for the fifth line with the vertical offset
            float middleLineY = altitudeLineY2 - (totalPitchLines / 2) * pitchInterval + verticalOffset;

            // Initialize the pitch lines with their initial altitudes and Y-coordinates
            for (int i = 0; i < totalPitchLines; i++)
            {
                int altitudeOffset = i - totalPitchLines / 2; // Offset from the middle line

                // Calculate altitude based on the rules provided
                int altitude;
                if (altitudeValue < 500)
                {
                    altitude = middleAltitude + altitudeOffset * 10;
                }
                else if (altitudeValue < 1000)
                {
                    altitude = middleAltitude + altitudeOffset * 50;
                }
                else
                {
                    altitude = middleAltitude + altitudeOffset * 100;
                }

                // For the middle line, set altitude to 0
                if (altitudeOffset == 0)
                {
                    altitude = 0;
                }

                // Adjust altitude value if altitude is greater than or equal to 3000
                if (altitude >= 3000)
                {
                    altitude = middleAltitude + altitudeOffset * 100;
                }

                float y = middleLineY - altitudeOffset * pitchInterval;
                pitchLines.Add((altitude, y));
            }

            // Draw the pitch indications and display their corresponding altitude values          
            foreach (var pitchLine in pitchLines)
            {
                Pen lineColor;
                if (pitchLine.Altitude == 0) // Check if it's the middle line
                {
                    lineColor = Pens.Lime; // Set color to bright green for the middle line
                }
                else
                {
                    lineColor = (pitchLine.Altitude >= 3000) ? Pens.Red : Pens.White; // Change line color to red for altitudes greater than or equal to 3000
                }

                g.DrawLine(lineColor, altitudeLineX1 - 10, pitchLine.Y, altitudeLineX1 + 10, pitchLine.Y);

                // Display altitude value for non-middle lines
                if (pitchLine.Altitude != 0)
                {
                    g.DrawString(pitchLine.Altitude.ToString(), smallFont, smallBrush, altitudeLineX1 + 20, pitchLine.Y - 7);
                }
            }

            // Draw the current altitude box
            int boxHeight = 25; // Adjust as needed
            int boxY = 105;
            int boxTopY = boxY - boxHeight / 2; // Calculate the top-left corner's y-coordinate of the box to center it vertically

            // Define the transparency level (alpha value)
            int transparency = 60; // Adjust the transparency level as needed (0 for fully transparent, 255 for fully opaque)

            // Create a transparent color
            Color transparentGray = Color.FromArgb(transparency, Color.Gray);

            //g.FillRectangle(Brushes.Gray, altitudeLineX1 + boxX, boxTopY, 60, boxHeight);

            // Draw the current altitude box with transparency
            g.FillRectangle(new SolidBrush(transparentGray), altitudeLineX1 + 15, boxTopY, 60, boxHeight);

            // Draw the altitude value inside the box
            int boxTextY = boxTopY + (boxHeight - largeFont.Height) / 2; // Y-coordinate for drawing text inside the box
            g.DrawString(altitudeValue.ToString(), largeFont, largeBrush, altitudeLineX1 + 20, boxTextY);

            GL.End();
        }

        private void DrawSpeedIndicator(Graphics g, string speedText, int speedValue)
        {
            // Define font and brush for drawing the text
            Font font = new Font("Arial", 9);
            Brush brush = Brushes.White;

            // Define the line coordinates for the altitude indicator
            int speedLineX1 = 280; // X-coordinate of the line start
            int speedLineX2 = 280; // X-coordinate of the line end
            int speedLineY1 = 20; // Y-coordinate of the line start
            int speedLineY2 = glControl1.Height - 20; // Y-coordinate of the line end

            // Draw the background of the altitude indicator
            g.FillRectangle(Brushes.White, speedLineX1 - 2, speedLineY1, 2, speedLineY2 - speedLineY1);

            // Draw the altitude scale
            for (int i = speedValue - 60; i <= speedValue + 60; i += 100)
            {
                int y = speedLineY2 - i * (speedLineY2 - speedLineY1) / 100;
                g.DrawLine(Pens.White, speedLineX1, y, speedLineX1 + 10, y);
                g.DrawString(i.ToString(), font, brush, speedLineX1 + 15, y - font.Height / 2);
            }

            // Draw horizon line
            int horizonY = speedLineY2 - 50 * (speedLineY2 - speedLineY1) / 100;
            g.DrawLine(Pens.White, speedLineX1 - 15, horizonY, speedLineX1 + 15, horizonY);

            // Draw pitch indications
            for (int i = -30; i <= 30; i += 10)
            {
                int pitchY = speedLineY2 - (50 + i) * (speedLineY2 - speedLineY1) / 100;
                g.DrawLine(Pens.White, speedLineX1 - 10, pitchY, speedLineX1 + 10, pitchY);
            }

            // Draw dynamic altitude pointer
            int pointerY = speedLineY2 - speedValue * (speedLineY2 - speedLineY1) / 100;
            g.FillPolygon(Brushes.White, new Point[] { new Point(speedLineX1 - 5, pointerY), new Point(speedLineX1 - 15, pointerY + 5), new Point(speedLineX1 - 15, pointerY - 5) });

            // Draw altitude text
            SizeF altitudeTextSize = g.MeasureString(speedText, font);
            g.DrawString(speedText, font, brush, speedLineX1 + 20, speedLineY2 - altitudeTextSize.Height - 5);
        }


        #endregion

        #region mapcontrol
        private void InitializeGMap()
        {
            // Set the map provider (you can choose other providers)
            mapcontrol.MapProvider = GMapProviders.GoogleMap;

            // Set the initial position and zoom level
            mapcontrol.Position = new PointLatLng(40.901233, 31.167545); // Default to center of the world
            mapcontrol.MinZoom = 1;
            mapcontrol.MaxZoom = 20;
            mapcontrol.Zoom = 12;

            // Add GMapControl to the form
            mapcontrol.Overlays.Add(markerOverlay);
        }

        private void ShowCurrantPosation(double lat, double lng)
        {
            // well set currant positions
            currentPosationMarker.Position = new PointLatLng(lat, lng);

            markerOverlay.Markers.Clear();
            markerOverlay.Markers.Add(currentPosationMarker);


            mapcontrol.Position = new PointLatLng(lat, lng);
            mapcontrol.DragButton = MouseButtons.Right;
        }
        private void UpdateMapPosition(double latitude, double longitude)
        {
            // Clear existing markers and add the new marker
            markerOverlay.Markers.Clear();
            GMarkerGoogle positionMarker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.arrow);
            markerOverlay.Markers.Add(positionMarker);
            // Set the map center to the new position
            mapcontrol.Position = new PointLatLng(latitude, longitude);
            mapcontrol.Zoom = 15;
        }

        #endregion

        #region mapcontrol 2
        private void InitializeGMap2()
        {
            // Set the map provider (you can choose other providers)
            mapcontrol2.MapProvider = GMapProviders.GoogleMap;

            // Set the initial position and zoom level
            mapcontrol2.Position = new PointLatLng(40.901233, 31.167545); // Default to center of the world
            mapcontrol2.MinZoom = 1;
            mapcontrol2.MaxZoom = 20;
            mapcontrol2.Zoom = 12;

            // Add GMapControl to the form
            mapcontrol2.Overlays.Add(markerOverlay);
        }

        private void ShowCurrantPosation2(double lat, double lng)
        {
            // well set currant positions
            currentPosationMarker.Position = new PointLatLng(lat, lng);

            markerOverlay.Markers.Clear();
            markerOverlay.Markers.Add(currentPosationMarker);


            mapcontrol2.Position = new PointLatLng(lat, lng);
            mapcontrol2.DragButton = MouseButtons.Right;
        }
        private void UpdateMapPosition2(double latitude, double longitude)
        {
            // Clear existing markers and add the new marker
            markerOverlay.Markers.Clear();
            GMarkerGoogle positionMarker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.arrow);
            markerOverlay.Markers.Add(positionMarker);
            // Set the map center to the new position
            mapcontrol.Position = new PointLatLng(latitude, longitude);
            mapcontrol.Zoom = 15;
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

            if (double.TryParse(X, out double xr))
            {
                x = xr;
            }

            if (double.TryParse(Y, out double yr))
            {
                y = yr;
            }

            if (double.TryParse(Z, out double zr))
            {
                z = zr;
            }

            // Trigger a repaint of the OpenGL control on a separate thread
            Task.Run(() => glControl1.Invalidate());
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

                    UpdateMapPosition(lat1, lng1);

                    // gps controll 2 for the second map update it when its necessary
                    label39.Text = lat1.ToString();
                    label7.Text = lng1.ToString();

                    UpdateMapPosition2(lat1, lng1);

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
            if (int.TryParse(label47.Text, out int altitudeValue))
            {
                // Increment the altitude value by 20
                altitudeValue += 25;

                // Update label47 with the new altitude value
                label47.Text = altitudeValue.ToString();

                // Trigger the paint event to redraw the altitude indicator with the updated altitude value
                glControl1.Invalidate();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (int.TryParse(label47.Text, out int altitudeValue))
            {
                // Increment the altitude value by 20
                altitudeValue -= 20;

                // Update label47 with the new altitude value
                label47.Text = altitudeValue.ToString();

                // Trigger the paint event to redraw the altitude indicator with the updated altitude value
                glControl1.Invalidate();
            }
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