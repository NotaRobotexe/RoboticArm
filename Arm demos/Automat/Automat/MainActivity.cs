using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.Text;

namespace Automat
{
    [Activity(Label = "Automat", MainLauncher = true)]
    public class MainActivity : Activity
    {

        int g1 = 0;
        int g2 = 0;
        int t = 0;
        TextView z1,z2,tt;
        Socket socket;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button con = FindViewById<Button>(Resource.Id.con);
            Button gum1 = FindViewById<Button>(Resource.Id.gum1);
            Button gum2 = FindViewById<Button>(Resource.Id.gum2);
            Button tic = FindViewById<Button>(Resource.Id.tic);
            Button order = FindViewById<Button>(Resource.Id.order);
            z1 = FindViewById<TextView>(Resource.Id.textView1);
            tt = FindViewById<TextView>(Resource.Id.textView2);
            z2 = FindViewById<TextView>(Resource.Id.textView3);

            con.Click += Con_Click;
            gum1.Click += Gum1_Click;
            gum2.Click += Gum2_Click;
            tic.Click += Tic_Click;
            order.Click += Order_Click;
        }

        private void Order_Click(object sender, System.EventArgs e)
        {
            string or = g1.ToString() + "*" + t.ToString() + "*" + g2.ToString();
            SendData(or);
        }

        private void Tic_Click(object sender, System.EventArgs e)
        {
            t++;
            if (t>2){
                t = 0;
            }
            tt.Text = t.ToString();
        }

        private void Gum2_Click(object sender, System.EventArgs e)
        {
            g2++;
            if (g2 > 2)
            {
                g2 = 0;
            }
            z2.Text = g2.ToString();
        }

        private void Gum1_Click(object sender, System.EventArgs e)
        {
            g1++;
            if (g1 > 2)
            {
                g1 = 0;
            }
            z1.Text = g1.ToString();
        }

        private void Con_Click(object sender, System.EventArgs e)
        {
            EditText editText = FindViewById<EditText>(Resource.Id.editText1);
            string ip = editText.Text;
            InitCom(ip);
        }

        public int InitCom(string ip)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(System.Net.IPAddress.Parse(ip), 6979);
            }
            catch (System.Exception)
            {
                z1.Text = "er con";
            }
            return 1;
        }

        private void SendData(string s)
        {
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);
                socket.Send(buffer);
            }
            catch (System.Exception)
            {
                z1.Text = "er";
            }
        }
    }
}

