using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsPresentation;
using System.Windows.Forms;

namespace konuralprocketGS
{
    public class mapcontrol
    {

        public GMap.NET.WindowsForms.GMapControl mapControl1;
        public GMap.NET.WindowsForms.GMapControl mapControl2;
        public mapcontrol(GMap.NET.WindowsForms.GMapControl Map_control1, GMap.NET.WindowsForms.GMapControl Map_control2)
        {
            mapControl1 = Map_control1;
            mapControl2 = Map_control2;
        }

        private readonly GMapOverlay markerOverlay = new GMapOverlay("marker");
        private readonly GMarkerGoogle currentPosationMarker = new GMarkerGoogle(new PointLatLng(40.839989, 31.155060), GMarkerGoogleType.blue_dot);

        public void ShowCurrantPosation(double lat, double lng)
        {
            // well set currant positions
            currentPosationMarker.Position = new PointLatLng(lat, lng);

            markerOverlay.Markers.Clear();
            markerOverlay.Markers.Add(currentPosationMarker);


            mapControl1.Position = new PointLatLng(lat, lng);
            mapControl1.DragButton = MouseButtons.Right;
        }
        public void UpdateMapPosition(double latitude, double longitude)
        {
            // Clear existing markers and add the new marker
            markerOverlay.Markers.Clear();
            GMarkerGoogle positionMarker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.arrow);
            markerOverlay.Markers.Add(positionMarker);
            // Set the map center to the new position
            mapControl1.Position = new PointLatLng(latitude, longitude);
            mapControl1.Zoom = 15;
        }




        public void ShowCurrantPosation2(double lat, double lng)
        {
            // well set currant positions
            currentPosationMarker.Position = new PointLatLng(lat, lng);

            markerOverlay.Markers.Clear();
            markerOverlay.Markers.Add(currentPosationMarker);


            mapControl2.Position = new PointLatLng(lat, lng);
            mapControl2.DragButton = MouseButtons.Right;
        }
        public void UpdateMapPosition2(double latitude, double longitude)
        {
            // Clear existing markers and add the new marker
            markerOverlay.Markers.Clear();
            GMarkerGoogle positionMarker = new GMarkerGoogle(new PointLatLng(latitude, longitude), GMarkerGoogleType.arrow);
            markerOverlay.Markers.Add(positionMarker);
            // Set the map center to the new position
            mapControl2.Position = new PointLatLng(latitude, longitude);
            mapControl2.Zoom = 15;
        }
    }
}
