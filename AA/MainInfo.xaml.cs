using AA.DataSet;
using Microsoft.Reporting.WinForms;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using wpfAA;

namespace AA
{
    public partial class MainInfo : Window
    {
        bool isView1Ready = false;

        struct DataSQL
        {
            public int num;
            public string location;
            public string id;
            
            public DataSQL(int num, string location, string id)
            {
                this.num = num;
                this.location = location;
                this.id = id;
            }
        }

        List<DataSQL> stationInfo = new List<DataSQL>();

        public MainInfo(string login)
        {
            InitializeComponent();
            tabControl1.SelectionChanged += OnTabItemSelectionChanged1;
            if(login == "useraa")
            {
                tabGraph.Visibility = Visibility.Collapsed;
                tabView.Visibility = Visibility.Collapsed;
                isView1Ready = true;
            }
            else
            {
                FillView();
            }
        }

        private void OnTabItemSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.TabControl tabControl = sender as System.Windows.Controls.TabControl;
            var selectedTab = tabControl.SelectedItem as TabItem;

            if (selectedTab != null && isView1Ready)
            {
                FillTable1(selectedTab.Header.ToString());
            }
        }

        private void OnTabItemSelectionChanged2(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.TabControl tabControl = sender as System.Windows.Controls.TabControl;
            var selectedTab = tabControl.SelectedItem as TabItem;

            if (selectedTab != null)
            {
                FillTable2(selectedTab.Header.ToString());
            }
        }

        async private void FillTable1(string tabName)
        {
            LoadingGrid.Visibility = Visibility.Visible;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataTable dt = new DataTable();

            await Task.Run(() =>
            {
                switch (tabName)
                {
                    case "Станції":
                        cmd = new NpgsqlCommand("SELECT * FROM StationInfo", DB.GetConnection());
                        break;
                    case "Блок повідомлень":
                        cmd = new NpgsqlCommand("SELECT * FROM mqtt_message_unit", DB.GetConnection());
                        break;
                    case "Сервер":
                        cmd = new NpgsqlCommand("SELECT * FROM mqtt_server", DB.GetConnection());
                        break;
                    case "Оптимальні значення":
                        cmd = new NpgsqlCommand("SELECT * FROM OptimalValues", DB.GetConnection());
                        break;
                    case "Результат":
                        cmd = new NpgsqlCommand("SELECT * FROM ResultInfo", DB.GetConnection());
                        break;
                }

                da = new NpgsqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
            });
            switch (tabName)
            {
                case "Станції":
                    dgStation.ItemsSource = dt.DefaultView;
                    break;
                case "Блок повідомлень":
                    dgMqttMessageUnit.ItemsSource = dt.DefaultView;
                    break;
                case "Сервер":
                    dgMqttServer.ItemsSource = dt.DefaultView;
                    break;
                case "Оптимальні значення":
                    dgOptimalValue.ItemsSource = dt.DefaultView;
                    break;
                case "Результат":
                    dgResult.ItemsSource = dt.AsEnumerable().Take(100).CopyToDataTable().DefaultView;
                    break;
            }
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        private void View2_Click(object sender, RoutedEventArgs e)
        {
            FillView2();
        }

        private void View71_Click(object sender, RoutedEventArgs e)
        {
            FillView71();
        }

        private void View72_Click(object sender, RoutedEventArgs e)
        {
            FillView72();
        }

        private void View73_Click(object sender, RoutedEventArgs e)
        {
            FillView73();
        }

        private void View74_Click(object sender, RoutedEventArgs e)
        {
            FillView74();
        }

        async private void FillView()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet2 dataSet2 = new DataSet2();

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand("SELECT * FROM stationmeasurements", DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet2);
            });
            reportViewer61.ProcessingMode = ProcessingMode.Local;
            reportViewer61.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report2.rdlc";
            reportViewer61.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet2.Tables[1]));
            reportViewer61.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer61.RefreshReport();

            FillcbStations();
        }

        async private void FillcbStations()
        {
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand("SELECT city, id_station, name_ FROM stations", DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                da.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    stationInfo.Add(new DataSQL(i, (string)(dataTable.Rows[i]["city"] + ", " + dataTable.Rows[i]["name_"]), (string)dataTable.Rows[i]["id_station"]));
                }
            });

            foreach (var entry in stationInfo)
            {
                cbStations.Items.Add(entry.location);
                cbStations73.Items.Add(entry.location);
                cbStations74.Items.Add(entry.location);
            }

            isView1Ready = true;
        }

        async private void FillView2()
        {
            string cbStationStr = cbStations.SelectedItem.ToString();
            foreach (DataSQL data in stationInfo)
            {
                if(data.location == cbStationStr)
                {
                    cbStationStr = data.id; 
                    break;
                }  
            }

            DateTime dtStartDate = Convert.ToDateTime(StartDate.SelectedDate);
            string strStartDate = dtStartDate.ToString("dd.MM.yyyy HH:mm:ss");
            DateTime dtEndDate = Convert.ToDateTime(EndDate.SelectedDate);
            string strEndDate = dtEndDate.ToString("dd.MM.yyyy HH:mm:ss");
            gifForView2.Visibility = Visibility.Visible;
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet1 dataSet1 = new DataSet1();

            string strSql = "SELECT mu.title AS \"Name_of_units\", min(re.value_) AS \"MIN\", avg(re.value_)::numeric(10, 3) AS \"AVG\", max(re.value_) AS \"MAX\", mu.unit AS \"Units\" FROM result_ re JOIN measured_unit mu ON re.id_measured_unit = mu.id_measured_unit WHERE re.id_station = '" + cbStationStr + "' AND re.date_time <= '" + strEndDate + "' AND re.date_time >= '" + strStartDate + "'GROUP BY re.id_station, re.id_measured_unit, mu.title, mu.unit;";

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand(strSql, DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet1);
            });

            reportViewer62.ProcessingMode = ProcessingMode.Local;
            reportViewer62.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report1.rdlc";
            reportViewer62.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet1.Tables[1]));
            reportViewer62.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer62.RefreshReport();
            gifForView2.Visibility = Visibility.Collapsed;
        }

        async private void FillView71()
        {
            DateTime dtStartDate = Convert.ToDateTime(StartDate71.SelectedDate);
            string strStartDate = dtStartDate.ToString("dd.MM.yyyy HH:mm:ss");
            DateTime dtEndDate = Convert.ToDateTime(EndDate71.SelectedDate);
            string strEndDate = dtEndDate.ToString("dd.MM.yyyy HH:mm:ss");
            gifForView71.Visibility = Visibility.Visible;
            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet71 dataSet = new DataSet71();

            string strSql = "SELECT stations.city AS city, measured_unit.title AS title, MAX(result_.value_) AS valuemax FROM result_ JOIN stations ON result_.id_station = stations.id_station JOIN measured_unit ON result_.id_measured_unit = measured_unit.id_measured_unit WHERE measured_unit.title IN('PM2.5', 'PM10') AND result_.date_time >= '" + dtStartDate + "' AND result_.date_time <= '" + strEndDate + "' GROUP BY stations.city, measured_unit.title;";
            //string strSql = "SELECT stations.city AS city, measured_unit.title AS title, MAX(result_.value_) AS valuemax FROM result_ JOIN stations ON result_.id_station = stations.id_station JOIN measured_unit ON result_.id_measured_unit = measured_unit.id_measured_unit WHERE measured_unit.title IN('PM2.5', 'PM10') AND result_.date_time >= '11.11.2011 00:00:00' AND result_.date_time <= '11.11.2023 00:00:00' GROUP BY stations.city, measured_unit.title;";

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand(strSql, DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet);
            });

            reportViewer71.ProcessingMode = ProcessingMode.Local;
            reportViewer71.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report71.rdlc";
            reportViewer71.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
            reportViewer71.SetDisplayMode(DisplayMode.PrintLayout);

            string reportTitle = "Максимальні значення шкідливих частинок PM2.5, PM10 в розріз областей з " + strStartDate.Substring(0, 10) + " до " + strEndDate.Substring(0, 10); ;
            ReportParameter reportParameter = new ReportParameter("ReportParameter1", reportTitle);
            reportViewer71.LocalReport.SetParameters(new ReportParameter[] { reportParameter });

            reportViewer71.RefreshReport();
            gifForView71.Visibility = Visibility.Collapsed;
        }

        async private void FillView72()
        {
            DateTime dtStartDate = Convert.ToDateTime(StartDate72.SelectedDate);
            string strStartDate = dtStartDate.ToString("dd.MM.yyyy HH:mm:ss");
            DateTime dtEndDate = Convert.ToDateTime(EndDate72.SelectedDate);
            string strEndDate = dtEndDate.ToString("dd.MM.yyyy HH:mm:ss");
            gifForView72.Visibility = Visibility.Visible;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet72 dataSet = new DataSet72();

            string connectionStr = "WITH dailyaverages AS (SELECT st_1.id_station,date_trunc('day'::text, re.date_time) AS measurementdate, avg(re.value_) AS dailyaverage FROM stations st_1 JOIN result_ re ON st_1.id_station = re.id_station JOIN measured_unit mu ON re.id_measured_unit = mu.id_measured_unit JOIN optimal_value ov ON mu.id_measured_unit = ov.id_measured_unit WHERE mu.title::text = 'PM2.5'::text AND re.value_ > ov.upper_border AND re.date_time >= '" + strStartDate + "' AND re.date_time <= '" + strEndDate + "' GROUP BY st_1.id_station, (date_trunc('day'::text, re.date_time)) HAVING count(*) > 0) SELECT st.city AS station_city, st.name_ AS station_name, count(*) AS number_of_exceedances FROM stations st JOIN dailyaverages da ON st.id_station = da.id_station WHERE da.dailyaverage > (( SELECT avg(dailyaverages.dailyaverage) AS avg FROM dailyaverages)) GROUP BY st.city, st.name_ HAVING count(*) > 0;";

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand(connectionStr, DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet);
            });
            reportViewer72.ProcessingMode = ProcessingMode.Local;
            reportViewer72.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report72.rdlc";
            reportViewer72.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
            reportViewer72.SetDisplayMode(DisplayMode.PrintLayout);

            string reportTitle = "Кількість разів фіксувань перевищень кількості твердих частинок PM2.5 на станціях з "+ strStartDate.Substring(0, 10) + " до " + strEndDate.Substring(0, 10); ;
            ReportParameter reportParameter = new ReportParameter("ReportParameter1", reportTitle);
            reportViewer72.LocalReport.SetParameters(new ReportParameter[] { reportParameter });

            reportViewer72.RefreshReport();
            gifForView72.Visibility = Visibility.Collapsed;
        }

        async private void FillView73()
        {
            string cbStationStr = cbStations73.SelectedItem.ToString();
            foreach (DataSQL data in stationInfo)
            {
                if (data.location == cbStationStr)
                {
                    cbStationStr = data.id;
                    break;
                }
            }

            DateTime dtStartDate = Convert.ToDateTime(StartDate73.SelectedDate);
            string strStartDate = dtStartDate.ToString("dd.MM.yyyy HH:mm:ss");
            DateTime dtEndDate = Convert.ToDateTime(EndDate73.SelectedDate);
            string strEndDate = dtEndDate.ToString("dd.MM.yyyy HH:mm:ss");
            gifForView73.Visibility = Visibility.Visible;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet73 dataSet = new DataSet73();

            //string connectionStr = " SELECT ca.designation AS category, count(*) AS measurement_count FROM result_ re JOIN optimal_value ov ON re.id_measured_unit = ov.id_measured_unit JOIN category ca ON ov.id_category = ca.id_category WHERE re.value_ < ov.upper_border AND re.value_ > ov.bottom_border AND re.id_measured_unit = '12' AND re.date_time >= '" + strStartDate + "' AND re.date_time <= '" + strEndDate + "' GROUP BY ca.designation;";
            string connectionStr = " SELECT ca.designation AS category, count(*) AS measurement_count FROM result_ re JOIN optimal_value ov ON re.id_measured_unit = ov.id_measured_unit JOIN category ca ON ov.id_category = ca.id_category WHERE re.value_ < ov.upper_border AND re.value_ > ov.bottom_border AND re.id_measured_unit = '13' AND re.date_time >= '" + strStartDate + "' AND re.date_time <= '" + strEndDate + "' GROUP BY ca.designation;";

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand(connectionStr, DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet);
            });
            reportViewer73.ProcessingMode = ProcessingMode.Local;
            reportViewer73.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report73.rdlc";
            reportViewer73.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
            reportViewer73.SetDisplayMode(DisplayMode.PrintLayout);

            string reportTitle = "Кількість вимірювань, що належать до категорії оптимальних значень для діоксиду сірки на  " + cbStations73.SelectedItem.ToString()  + " з " + strStartDate.Substring(0, 10) + " до " + strEndDate.Substring(0, 10); ;
            ReportParameter reportParameter = new ReportParameter("ReportParameter1", reportTitle);
            reportViewer73.LocalReport.SetParameters(new ReportParameter[] { reportParameter });

            reportViewer73.RefreshReport();
            gifForView73.Visibility = Visibility.Collapsed;
        }

        async private void FillView74()
        {
            string cbStationStr = cbStations74.SelectedItem.ToString();
            foreach (DataSQL data in stationInfo)
            {
                if (data.location == cbStationStr)
                {
                    cbStationStr = data.id;
                    break;
                }
            }

            DateTime dtStartDate = Convert.ToDateTime(StartDate74.SelectedDate);
            string strStartDate = dtStartDate.ToString("dd.MM.yyyy HH:mm:ss");
            DateTime dtEndDate = Convert.ToDateTime(EndDate74.SelectedDate);
            string strEndDate = dtEndDate.ToString("dd.MM.yyyy HH:mm:ss");
            gifForView74.Visibility = Visibility.Visible;

            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();
            DataSet74 dataSet = new DataSet74();

            string connectionStr = " SELECT ca.designation AS category, count(*) AS measurement_count FROM result_ re JOIN optimal_value ov ON re.id_measured_unit = ov.id_measured_unit JOIN category ca ON ov.id_category = ca.id_category WHERE re.value_ < ov.upper_border AND re.value_ > ov.bottom_border AND re.id_measured_unit = '12' AND re.date_time >= '" + strStartDate + "' AND re.date_time <= '" + strEndDate + "' GROUP BY ca.designation;";

            await Task.Run(() =>
            {
                cmd = new NpgsqlCommand(connectionStr, DB.GetConnection());
                da = new NpgsqlDataAdapter(cmd);
                da.Fill(dataSet);
            });
            reportViewer74.ProcessingMode = ProcessingMode.Local;
            reportViewer74.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report74.rdlc";
            reportViewer74.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
            reportViewer74.SetDisplayMode(DisplayMode.PrintLayout);

            string reportTitle = "Кількість вимірювань, що належать до категорії оптимальних значень для чадного газу на  " + cbStations73.SelectedItem.ToString() + " з " + strStartDate.Substring(0, 10) + " до " + strEndDate.Substring(0, 10); ;
            ReportParameter reportParameter = new ReportParameter("ReportParameter1", reportTitle);
            reportViewer74.LocalReport.SetParameters(new ReportParameter[] { reportParameter });

            reportViewer74.RefreshReport();
            gifForView74.Visibility = Visibility.Collapsed;
        }

        async public void FillTable2(string tabName)
        {
            LoadingGrid.Visibility = Visibility.Visible;

            DataTable dataTable = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();
            NpgsqlCommand cmd = new NpgsqlCommand();

            await Task.Run(() =>
            {
                if (tabName == "Звіт 1")
                {
                    DataSet1 dataSet = new DataSet1();
                    cmd = new NpgsqlCommand("SELECT * FROM resultofmeasurments", DB.GetConnection());
                    da = new NpgsqlDataAdapter(cmd);
                    da.Fill(dataSet);
                    reportViewer61.ProcessingMode = ProcessingMode.Local;
                    reportViewer61.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report1.rdlc";
                    reportViewer61.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
                    reportViewer61.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer61.RefreshReport();
                }
                else if (tabName == "Звіт 2")
                {
                    DataSet2 dataSet = new DataSet2();
                    cmd = new NpgsqlCommand("SELECT * FROM stationmeasurements", DB.GetConnection());
                    da = new NpgsqlDataAdapter(cmd);
                    da.Fill(dataSet);
                    reportViewer62.ProcessingMode = ProcessingMode.Local;
                    reportViewer62.LocalReport.ReportPath = "F:\\Nubip\\Sem_3.1\\DB\\AA\\AA\\Resources\\Report2.rdlc";
                    reportViewer62.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[1]));
                    reportViewer62.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer62.RefreshReport();


                }
            });

            LoadingGrid.Visibility = Visibility.Collapsed;
        }
    }
}
