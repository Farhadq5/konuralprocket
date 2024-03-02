using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;

namespace konuralprocketGS
{
    public class GyroGL_Class
    {

        private GLControl glControl1;

        public GyroGL_Class(GLControl glControl)
        {
            glControl1 = glControl;
        }

       
        public double x = 0.0f, y = 0.0f, z = 0.0f;
       // bool by = false, bx = false, bz = false;
        private float zoomFactor = 1.0f;
        Color renk1 = Color.White, renk2 = Color.Red;

        public string speed { get; set; }

        public string altitude { get; set; }


        public void DrawRocketComponents(float step, float topla, float radius)
        {
            silindir(step, topla, radius, 3f, -8);

            koni(step, topla, radius , 0.5f, 3, 10.5f);
            koni(step, step, radius, 2.8f, -7.0f, -12.0f);
           
                 
            Pervane(-13.0f, 6.0f, 0.1f, 5.3f);


        }

        public void silindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Quads);//Y EKSEN CIZIM DAİRENİN
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
            GL.Begin(PrimitiveType.Lines);
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
        public void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Lines);//Y EKSEN CIZIM DAİRENİN
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

            GL.Begin(PrimitiveType.Lines);
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
        public void Pervane(float yukseklik, float uzunluk, float kalinlik, float egiklik)
        {
            GL.Begin(PrimitiveType.Quads);

            // Front face of the fin (right side)
            GL.Color3(renk2);
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(0, yukseklik, kalinlik);

            // Back face of the fin (right side)
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik, -kalinlik);
            GL.Vertex3(0, yukseklik + egiklik, kalinlik);

            // Top face of the fin with curvature
            GL.Color3(renk1);
            for (float t = 0; t <= 1; t += 0.1f)
            {
                // Calculate control points for Bézier curve
                float controlX1 = kalinlik * (1 - t);
                float controlY1 = yukseklik;
                float controlZ1 = (float)(kalinlik * Math.Sin(Math.PI * t));

                float controlX2 = -kalinlik * (1 - t);
                float controlY2 = yukseklik + egiklik;
                float controlZ2 = (float)(-kalinlik * Math.Sin(Math.PI * t));

                // Calculate points on the Bézier curve
                float bezierX = (1 - t) * controlX1 + t * controlX2;
                float bezierY = (1 - t) * controlY1 + t * controlY2;
                float bezierZ = (1 - t) * controlZ1 + t * controlZ2;

                GL.Vertex3(bezierX, bezierY, bezierZ);

                // If not the last iteration, calculate the next point on the curve
                if (t < 1)
                {
                    float nextBezierX = (1 - (t + 0.1f)) * controlX1 + (t + 0.1f) * controlX2;
                    float nextBezierY = (1 - (t + 0.1f)) * controlY1 + (t + 0.1f) * controlY2;
                    float nextBezierZ = (1 - (t + 0.1f)) * controlZ1 + (t + 0.1f) * controlZ2;

                    GL.Vertex3(nextBezierX, nextBezierY, nextBezierZ);
                }
            }

            // Bottom face of the fin
            GL.Vertex3(kalinlik, yukseklik + egiklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, +uzunluk);
            GL.Vertex3(-kalinlik, yukseklik, 0.0);
            GL.Vertex3(kalinlik, yukseklik + egiklik, 0.0);

            // Right face of the fin
            GL.Vertex3(uzunluk, yukseklik, kalinlik);
            GL.Vertex3(uzunluk, yukseklik + egiklik, -kalinlik);
            GL.Vertex3(-uzunluk, yukseklik + egiklik, kalinlik);
            GL.Vertex3(-uzunluk, yukseklik, -kalinlik);

            GL.End();
        }

        //public void DrawCoordinateAxes()
        //{
        //    GL.Begin(PrimitiveType.Lines);

        //    // X-axis (red)
        //    GL.Color3(Color.Red);
        //    GL.Vertex3(-100, 0, 0);
        //    GL.Vertex3(100, 0, 0);

        //    // Y-axis (green)
        //    GL.Color3(Color.Green);
        //    GL.Vertex3(0, -100, 0);
        //    GL.Vertex3(0, 100, 0);

        //    // Z-axis (blue)
        //    GL.Color3(Color.Blue);
        //    GL.Vertex3(0, 0, -100);
        //    GL.Vertex3(0, 0, 100);



        //    GL.End();

        //}

        public void DrawBackground()
        {
            // Set the clear color to sky blue for the top half
            GL.ClearColor(Color.SkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Set the clear color to saddle brown for the bottom half
            GL.ClearColor(Color.SaddleBrown);
            GL.Scissor(0, 0, glControl1.Width, glControl1.Height / 2);
            GL.Enable(EnableCap.ScissorTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.ScissorTest);
        }
       
        public void renk_ataması(float step)
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

        // Define font and brush for drawing the text
        Font largeFont = new Font("Arial", 12, FontStyle.Bold);
        Font font = new Font("Arial", 9);
        Brush largeBrush = Brushes.White;
        Brush smallBrush = Brushes.Gray;

        const int transparency = 150;
        int totalLines = 11;

        public void DrawAltitudeIndicator(Graphics g, string altitudeText, int altitudeValue)
        {
            // Define font and brushes for drawing the text
            Font largeFont = new Font("Arial", 12, FontStyle.Bold);
            Font smallFont = new Font("Arial", 9);
            Brush largeBrush = Brushes.White;
            Brush smallBrush = Brushes.Gray;

            // Define the coordinates for the altitude indicator
            int altitudeLineX1 = 16; // X-coordinate of the line start
            int altitudeLineY1 = 10; // Y-coordinate of the line start
            int altitudeLineY2 = glControl1.Height - 10; // Y-coordinate of the line end

            //int transparency2 = 150; // Adjust the transparency level as needed (0 for fully transparent, 255 for fully opaque)

            // Create a transparent color for horozontal line indecators
            Color transparenthorozontal= Color.FromArgb(transparency, Color.Black);         
    
            // Draw the background of the altitude indicator
            g.FillRectangle(new SolidBrush(transparenthorozontal), altitudeLineX1 -15, altitudeLineY1,30, altitudeLineY2 + 8 - altitudeLineY1);

            // Draw the altitude scale
            int scaleIncrement = altitudeValue <= 1000 ? 20 : 100; // Change the scale based on altitude


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
            int boxY = (altitudeLineY1 + altitudeLineY2)/2;
            int boxTopY = boxY - boxHeight / 2; // Calculate the top-left corner's y-coordinate of the box to center it vertically

            // Define the transparency level (alpha value)
            //int transparency = 110; // Adjust the transparency level as needed (0 for fully transparent, 255 for fully opaque)

            // Create a transparent color
            Color transparent = Color.FromArgb(transparency, Color.Black);

            //g.FillRectangle(Brushes.Gray, altitudeLineX1 + boxX, boxTopY, 60, boxHeight);

            // Draw the current altitude box with transparency
            g.FillRectangle(new SolidBrush(transparent), altitudeLineX1 + 15, boxTopY, 60, boxHeight);

            // Draw the altitude value inside the box
            int boxTextY = boxTopY + (boxHeight - largeFont.Height) / 2; // Y-coordinate for drawing text inside the box
            g.DrawString(altitudeValue.ToString(), largeFont, largeBrush, altitudeLineX1 + 20, boxTextY);

            GL.End();
        }

        #region speedIndecator

        int scaleIncrement = 30;    
        public void Drawspeedhorozontal(Graphics g)
        {
            // Define the line coordinates for the speed indicator
            int speedLineX1 = 336; // X-coordinate of the line start
            int speedLineY1 = 10; // Y-coordinate of the line start

            int speedLineY2 = glControl1.Height - 10; // Y-coordinate of the line end  
            
            float pitchInterval = (float)(speedLineY2 - speedLineY1) / (totalLines - 1);          

            // Create a transparent color for horozontal line indecators
            Color transparenthorozontal = Color.FromArgb(transparency, Color.Black);

            // Draw the background of the altitude indicator
            g.FillRectangle(new SolidBrush(transparenthorozontal), speedLineX1 - 15, speedLineY1, 30, speedLineY2 + 8 - speedLineY1);

            // Draw the speed scale and text
            for (int i = 0; i < totalLines; i++)
            {
                int pitchY = speedLineY2 - (int)(i * pitchInterval);
                g.DrawLine(Pens.White, speedLineX1 - 10, pitchY, speedLineX1 + 10, pitchY);

                // Draw the value of each horizontal line
                g.DrawString((i * scaleIncrement).ToString(), font, Brushes.Gray, speedLineX1 - 43, pitchY - font.Height / 2);
            }
        }

        public void DrawSpeedIndicator(Graphics g, string speedText, int speedValue)
        {
            // Define the line coordinates for the speed indicator
            int speedLineX1 = 336; // X-coordinate of the line start
            int speedLineY1 = 10; // Y-coordinate of the line start

            int speedLineY2 = glControl1.Height - 10; // Y-coordinate of the line end  

            //int transparency = 150; // Adjust the transparency level as needed (0 for fully transparent, 255 for fully opaque)

            // Calculate the interval between each pitch line
            float pitchInterval = (float)(speedLineY2 - speedLineY1) / (totalLines - 1);

            // Calculate the middle of the speed indicator
            int middleY = (speedLineY1 + speedLineY2) / 2;

            // Draw horizontal lines and their values
            Drawspeedhorozontal(g);

            // Define a scaling factor for pointer movement
            float pointerScale = 0.330f; // Adjust as needed for desired speed

            // Calculate the pointer position
            int pointerY = (int)(speedLineY2 - speedValue * (speedLineY2 - speedLineY1) / 100 * pointerScale);

            // Check if the pointer is within the range of the horizontal lines
            if (pointerY < speedLineY1)
            {
                pointerY = speedLineY1; // Set pointer to the position of the first horizontal line
            }
            else if (pointerY > speedLineY2)
            {
                pointerY = speedLineY2; // Set pointer to the position of the last horizontal line
            }

            // Draw the speed indicator line and pointer
            if (pointerY <= speedLineY2 && pointerY >= speedLineY1)
            {
                if (speedValue >= 200)
                {
                    g.FillPolygon(Brushes.Red, new Point[] { new Point(speedLineX1 - 9, pointerY), new Point(speedLineX1 - 15, pointerY + 5), new Point(speedLineX1 - 15, pointerY - 5) });
                }
                else
                {
                    g.FillPolygon(Brushes.Green, new Point[] { new Point(speedLineX1 - 9, pointerY), new Point(speedLineX1 - 15, pointerY + 5), new Point(speedLineX1 - 15, pointerY - 5) });
                }
            }

            // Draw the speed text
            SizeF speedTextSize = g.MeasureString(speedText, font);
            g.DrawString(speedText, font, smallBrush, speedLineX1 - speedTextSize.Width - 20, middleY - speedTextSize.Height / 2);

            // Draw the current speed box
            int boxHeight = 25; // Adjust as needed
            int boxY = (speedLineY1 + speedLineY2) / 2; // Set the Y-coordinate to the middle of the speed indicator
            int boxTopY = boxY - boxHeight / 2; // Calculate the top-left corner's y-coordinate of the box to center it vertically

            // Create a transparent color for horizontal line indicators
            Color Transparentbox = Color.FromArgb(transparency, Color.Black);

            // Draw the current speed box with transparency
            g.FillRectangle(new SolidBrush(Transparentbox), speedLineX1 - 75, boxTopY, 60, boxHeight);

            // Draw the initial speed value inside the box
            int boxTextY = boxTopY + (boxHeight - largeFont.Height) / 2; // Y-coordinate for drawing text inside the box
            g.DrawString(speedValue.ToString(), largeFont, largeBrush, speedLineX1 - 70, boxTextY);

        }

        #endregion
    }
}
