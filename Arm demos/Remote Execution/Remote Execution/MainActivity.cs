using Android.App;
using Android.Widget;
using Android.OS;
using System.Net.Sockets;
using System.Text;

namespace Remote_Execution
{
    [Activity(Label = "Remote_Execution", MainLauncher = true)]
    public class MainActivity : Activity
    {
        TextView tx;
        private Socket socket;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Button bc = FindViewById<Button>(Resource.Id.con);
            Button e0u = FindViewById<Button>(Resource.Id.el0u);
            Button e1u = FindViewById<Button>(Resource.Id.el1u);
            Button e2u = FindViewById<Button>(Resource.Id.el2u);
            Button e0d = FindViewById<Button>(Resource.Id.el0d);
            Button e1d = FindViewById<Button>(Resource.Id.el1d);
            Button e2d = FindViewById<Button>(Resource.Id.el2d);
            Button b1 = FindViewById<Button>(Resource.Id.b1);
            Button b2 = FindViewById<Button>(Resource.Id.b2);
            Button go = FindViewById<Button>(Resource.Id.go);
            Button gc = FindViewById<Button>(Resource.Id.gc);
            Button gr = FindViewById<Button>(Resource.Id.gr);
            Button gr1 = FindViewById<Button>(Resource.Id.gr2);
            e0d.Click += E0d_Click;
            e0u.Click += E0u_Click;
            e1d.Click += E1d_Click;
            e1u.Click += E1u_Click;
            e2d.Click += E2d_Click;
            e2u.Click += E2u_Click;
            b1.Click += B1_Click;
            b2.Click += B2_Click;
            go.Click += Go_Click;
            gc.Click += Gc_Click;
            gr.Click += Gr_Click;
            gr1.Click += Gr1_Click;
            bc.Click += Bc_Click;

            tx = FindViewById<TextView>(Resource.Id.textView);
        }

        private void Gr1_Click(object sender, System.EventArgs e)
        {
            SendData("gt");
            tx.Text = "gt";
        }

        private void Gr_Click(object sender, System.EventArgs e)
        {
            SendData("gr");
            tx.Text = "gr";
        }

        private void Gc_Click(object sender, System.EventArgs e)
        {
            SendData("gc");
            tx.Text = "gc";
        }

        private void Go_Click(object sender, System.EventArgs e)
        {
            SendData("go");
            tx.Text = "go";
        }

        private void B2_Click(object sender, System.EventArgs e)
        {
            SendData("b2");
            tx.Text = "b2";
        }

        private void B1_Click(object sender, System.EventArgs e)
        {
            SendData("b1");
            tx.Text = "b1";
        }

        private void E2u_Click(object sender, System.EventArgs e)
        {
            SendData("2u");
            tx.Text = "2u";
        }

        private void E2d_Click(object sender, System.EventArgs e)
        {
            SendData("2d");
            tx.Text = "2d";
        }

        private void E1u_Click(object sender, System.EventArgs e)
        {
            SendData("1u");
            tx.Text = "1u";
        }

        private void E1d_Click(object sender, System.EventArgs e)
        {
            SendData("1d");
            tx.Text = "1d";
        }

        private void E0u_Click(object sender, System.EventArgs e)
        {
            tx.Text = "0u";
            SendData("0u");
        }

        private void E0d_Click(object sender, System.EventArgs e)
        {
            tx.Text = "0d";
            SendData("0d");
        }

        private void Bc_Click(object sender, System.EventArgs e)
        {
            EditText editText = FindViewById<EditText>(Resource.Id.editText1);
            string ip = editText.Text;
            tx.Text = ip;
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
                tx.Text = "er con";
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
                tx.Text = "er";
            }
        }
    }
}

