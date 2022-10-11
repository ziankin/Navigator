using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET.WindowsForms.Markers;
using System.Net.Http;
using System.Net.Http.Headers;
using GMap.NET;
using GMap.NET.WindowsForms.ToolTips;
using System.Drawing.Drawing2D;
using System.IO;
using Itinero;
using Itinero.Osm.Vehicles;
using Itinero.IO.Osm;
using GMap.NET.WindowsForms;

namespace Maps
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay("markers");
        private static readonly HttpClient client = new HttpClient();
        private object gmap;

        public Form1()
        {
            InitializeComponent();
        }

        
        private static async Task ProcessRepositories()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            var stringTask = client.GetStringAsync("http://maps.googleapis.com/maps/api/directions/xml?origin={0},&destination={1}&sensor=false&language=ru&mode={2}");

            var msg = await stringTask;
            Console.Write(msg);
        }
        static async Task Main(string[] args)
        {
            

            await ProcessRepositories();

        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            //показывать или скрывать красный крестик в центре
            gMapControl1.ShowCenter = false;

            gMapControl1.Bearing = 0;
            // Для перетаскивания ПКМ 
            gMapControl1.CanDragMap = true;

            //Тоже перетаскивание ЛКМ только
            gMapControl1.DragButton = MouseButtons.Left;

            gMapControl1.GrayScaleMode = true;

            //MarkersEnabled - Если параметр установлен в True,
            //любые маркеры, заданные вручную будет показаны.
            //Если нет, они не появятся.
            gMapControl1.MarkersEnabled = true;

            //Значение максимального приближения.
            gMapControl1.MaxZoom = 18;

            //Значение минимального приближения.
            gMapControl1.MinZoom = 2;

            //Центр приближения/удаления для
            //курсора мыши.
            gMapControl1.MouseWheelZoomType =
                GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            //Отказываемся от негативного режима.
            gMapControl1.NegativeMode = false;

            //Разрешаем полигоны.
            gMapControl1.PolygonsEnabled = true;

            //Разрешаем маршруты.
            gMapControl1.RoutesEnabled = true;

            //Скрываем внешнюю сетку карты
            //с заголовками.
            gMapControl1.ShowTileGridLines = false;

            //При загрузке карты будет использоваться 
            //2х кратное приближение.
            gMapControl1.Zoom = 12;

            gMapControl1.Overlays.Add(markersOverlay);

            //Указываем что будем использовать карты Google.
            gMapControl1.MapProvider =
                GMap.NET.MapProviders.GMapProviders.GoogleMap;
            GMap.NET.GMaps.Instance.Mode =
                GMap.NET.AccessMode.ServerOnly;
            gMapControl1.Position = new GMap.NET.PointLatLng(58.5966, 49.6601);// точка в центре карты при открытии (Киров)
            gMapControl1.MouseClick += new MouseEventHandler(gMapControl1_MouseClick);
            

        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                PointLatLng point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.green);
            }
        }
        // Добавление маркера по двойному клику ЛКМ по карте
        GMapOverlay PositionsForUser = new GMapOverlay("ПозицияпоЛКМ");

        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                gMapControl1.Overlays.Add(PositionsForUser);
                double lat = 0.0;
                double lng = 0.0;

                // Широта - latitude - lat - с севера на юг
                lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;
                // Долгота - longitude - lng - с запада на восток
                lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;

                textBox2.Text = lat.ToString();
                textBox1.Text = lng.ToString();

                // Добавляем метку на слой
                GMarkerGoogle MarkerWithMyPosition = new GMarkerGoogle(new PointLatLng(lng, lat), GMarkerGoogleType.blue_pushpin);
                MarkerWithMyPosition.ToolTip = new GMapRoundedToolTip(MarkerWithMyPosition);
                MarkerWithMyPosition.ToolTipText = "Метка пользователя";
                PositionsForUser.Markers.Add(MarkerWithMyPosition);
            }
        }
      
        private Stream stream;

        
        private void button3_Click(object sender, EventArgs e)
        {
            GMapOverlay RouteClick = new GMapOverlay("RouteClick");

            gMapControl1.Overlays.Add(RouteClick);

            var x1 = Convert.ToSingle(textBox3.Text);
            var y1 = Convert.ToSingle(textBox4.Text);

            var x2 = Convert.ToSingle(textBox5.Text);
            var y2 = Convert.ToSingle(textBox6.Text);
 
            var someList = new List<PointLatLng>();
            PointLatLng start = new PointLatLng (x1, y1);  
            PointLatLng end = new PointLatLng(x2, y2);
            someList.Add(start);
            someList.Add(end);
            foreach (var item in someList)
{
	Console.WriteLine(item);
}
            Console.WriteLine();
            
            GMapRoute r = new GMapRoute(someList, "My route");
             
            r.Stroke.Color = Color.Red;
            r.Stroke.Width = 4;
            r.Stroke.DashStyle = DashStyle.Solid;
            RouteClick.Routes.Add(r);
            GMapOverlay routeOverlay = new GMapOverlay("routeClick");
            routeOverlay.Routes.Add(r);
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = textBox1.Text;
            textBox4.Text = textBox2.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox5.Text = textBox1.Text;
            textBox6.Text = textBox2.Text;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PositionsForUser.Markers.Clear();
        }

    }
}
