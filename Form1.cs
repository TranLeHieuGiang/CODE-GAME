﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1: Form
    {
        List<PictureBox> pictureBoxes = new List<PictureBox>();
        List<Bitmap> images = new List<Bitmap>();
        List<string> locations = new List<string>();
        List<string> current_locations = new List<string>();
       
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        
        string winPosition;
        string currentPosition;

        Bitmap MainBitmap;

        public Form1()
        {
            InitializeComponent();
            LoadDefaultImage();
           Timer timer1 = new Timer();
            label3.Text = "00:00:00";
            timer1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //Tạo phần mở file lấy ảnh
        private void LoadDefaultImage()
        {
            // Kiểm tra xem liệu trò chơi có đang chơi giữa chừng không, nếu có thì xóa hết để bắt đầu lại
            if (pictureBoxes != null)
            {
                foreach (PictureBox pics in pictureBoxes.ToList())
                {
                    this.Controls.Remove(pics);
                }
                pictureBoxes.Clear();
                images.Clear();
                locations.Clear();
                current_locations.Clear();
                winPosition = string.Empty;
                currentPosition = string.Empty;
                label1.Text = string.Empty;
            }

            // Đặt đường dẫn hình ảnh mặc định
            string imagePath = @"C:\Users\Admin.DESKTOP-P3PG6TU\Downloads\logo ueh.png";
            if (System.IO.File.Exists(imagePath))
            {
                // Tạo Bitmap từ hình ảnh mặc định
                MainBitmap = new Bitmap(imagePath);
                CreateImagesBox();
                AddImages();
            }
        }

        //Tạo phần hình ảnh
        private void CreateImagesBox()
        {
            //định dạng hình ảnh được lấy vào
            for (int i=0;i<9;i++)
            {
                PictureBox temp_pic = new PictureBox() ;
                temp_pic.Size = new Size(130, 130);
                temp_pic.Tag = i.ToString();
                temp_pic.Click += OnPicClick;
                pictureBoxes.Add(temp_pic);
                locations.Add(temp_pic.Tag.ToString());
            }   
        }
        private void OnPicClick(object sender, EventArgs e)
        {
            //Bắt đầu tính thời gian
            if (label3.Text == "00:00:00")
                timer.Start();
            //Nếu là nút resume thì khi di chuyển khối thời gian vẫn tiếp tục chạy
            if (button1.Text == "Resume")
            {
                timer.Start();  // Tiếp tục chạy timer
                button1.Text = "Pause";  // Đổi nút về Pause
            }

            //Xử lý click
           PictureBox pictureBox = (PictureBox)sender;
           PictureBox emtyBox = pictureBoxes.Find(x => x.Tag != null && x.Tag.ToString() == "0");
            if (emtyBox == null)
                return;

           Point pic1 = new Point(pictureBox.Location.X, pictureBox.Location.Y);
           Point pic2 = new Point(emtyBox.Location.X, emtyBox.Location.Y);

            //Kiểm soát các khối khi di chuyển
            var index1 = this.Controls.IndexOf(pictureBox);
            var index2 = this.Controls.IndexOf(emtyBox);

            //Kiểm tra các điều kiện để di chuyển vị trí các khối
            if (pictureBox.Right == emtyBox.Left && pictureBox.Location.Y == emtyBox.Location.Y 
                || pictureBox.Left == emtyBox.Right && pictureBox.Location.Y == emtyBox.Location.Y
                || pictureBox.Top == emtyBox.Bottom && pictureBox.Location.X == emtyBox.Location.X
                || pictureBox.Bottom == emtyBox.Top && pictureBox.Location.X == emtyBox.Location.X)

            {
                //Di chuyển các khối
                pictureBox.Location = pic2;
                emtyBox.Location = pic1;

                this.Controls.SetChildIndex(pictureBox,index2);
                this.Controls.SetChildIndex(emtyBox,index1);
            }
            label6.Text = "";
            current_locations.Clear();
            CheckGame();

        }

        //Tạo phần cắt hình ảnh ra thành các cỡ khác nhau
        private void CropImages(Bitmap main_bitmap, int height, int width)
        {
            int x, y;
            x = 0;
            y = 0;
            for (int blocks = 0; blocks < 9; blocks++)
            {
                Bitmap cropped_image = new Bitmap (height, width);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        cropped_image.SetPixel(i, j, main_bitmap.GetPixel((i+x),(j+y)));
                    }    
                }
                images.Add(cropped_image);
                //Đảm bảo rằng các cạnh đều được chia 3 lần
                x += 130;
                if (x==390)
                {
                    x = 0;
                    y += 130;
                }    
            }
        }

        //Tạo phần thêm hình ảnh
        private void AddImages()
        {
            //Resize lại ảnh nếu kích cỡ không phù hợp
            Bitmap tempBitmap = new Bitmap(MainBitmap, new Size(390, 390));

            CropImages(tempBitmap,130,130);
            //Tạo vòng lặp và để trừa ra 1 ô trắng để có thể di chuyển hình
            for (int i=1; i< pictureBoxes.Count;i++)
            {
                pictureBoxes[i].BackgroundImage = (Image)images[i];
            }
            PlaceImagestoForm();

        }

        //Sắp xếp vị trí các khối
        private void PlaceImagestoForm()
        {
            //xáo vị trí của cách khối ảnh
            var shuffleImages = pictureBoxes.OrderBy(a => Guid.NewGuid()).ToList();
            pictureBoxes = shuffleImages;
            //vị trí của các khối trên màn hình
            int x = 175;
            int y = 25;
            for (int i=0; i< pictureBoxes.Count;i++)
            {
                //để vị trí ảnh theo bên trái
                pictureBoxes[i].BackColor = Color.Gray;
                if (i==3 || i==6)
                {
                    y += 130;
                    x = 175;
                }
                pictureBoxes[i].BorderStyle = BorderStyle.FixedSingle;
                //đưa về vị trí mới
                pictureBoxes[i].Location = new Point(x, y);

                this.Controls.Add(pictureBoxes[i]);
                x += 130;
                //tập hợp các vị trí của các khối
                winPosition += i.ToString();
            }    
        }

        //Kiểm tra game
        private void CheckGame()
        {
            current_locations.Clear(); //Đảm bảo danh sách vị trí hiện tại sạch

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    //scan để tìm tag của từng khối và cập nhật vị trí 
                    current_locations.Add(x.Tag.ToString());
                }
            }
            currentPosition = string.Join("", current_locations);
            label1.Text = winPosition;
            label6.Text = currentPosition;

            //Kiểm tra điều kiện để thắng
            if (winPosition == currentPosition)
            {
                label6.Text = "Match";
                timer.Stop();
                button1.Enabled = false;
                scoreOfGame();
                MessageBox.Show("Congratulation!!! Let's come to the next round");
            }
        }

        //Điểm số của game
        private void scoreOfGame()
        {
            int score = 0;
            int minutes = timer.Elapsed.Minutes; //Lấy số phút
            int seconds = timer.Elapsed.Seconds; //Lấy số giây
            if (minutes < 5 || (minutes == 5 && seconds == 0))    
                score = 100;
            else if ((minutes == 5 && seconds > 0) && ((minutes < 10) || (minutes == 10 && seconds == 0)))
                score = 70;
            else if ((minutes == 10 && seconds > 0) && (minutes < 15) || (minutes == 15 && seconds == 0))
                score = 30;
            else score = 0;

            label5.Text = score.ToString();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Ngừng cập nhật nếu time dừng
            if (!timer.IsRunning)
            {
                return;
            }    

            label3.Text = timer.Elapsed.ToString(@"hh\:mm\:ss");
            if (timer.Elapsed.ToString() != "00:00:00")
            {
                label3.Text = timer.Elapsed.ToString().Remove(8);
            } 
            if (timer.Elapsed.ToString() == "00:00:00")
            {
                button1.Enabled = false;
            }    
            else button1.Enabled = true;
            if (timer.Elapsed.Minutes.ToString() == "15")
            {
                timer.Reset();
                label3.Text = "00:00:00";
                button1.Enabled = false;
                MessageBox.Show("Sorry, Time is up!!! Please try again~~ ");
                PlaceImagestoForm();
            }    
        }

        //Bấm nút dừng lại 
        private void button1_Click(object sender, EventArgs e)
        {
            //Khi bấm dừng
            if (button1.Text == "Pause")
            {
                timer.Stop(); //Thời gian dừng lại
                button1.Text = "Resume"; //Thay đổi nút pause để bấm lại lần 2 thì thành resume
            }
            //Khi bấm dừng lần 2 => resume
            else if (button1.Text == "Resume")
            {
                timer.Start(); //Thời gian tiếp tục
                button1.Text = "Pause"; //Thay đổi để về nút pause
            }    
        }

        //Bấm nút restart để chơi lại
        private void restart_Click(object sender, EventArgs e)
        {
            if (restart.Text == "Restart")
            {
                timer.Restart(); //Restart lại thời gian
                label3.Text = "00:00:00";
                //Đặt lại hình ảnh
                LoadDefaultImage();
                PlaceImagestoForm();
                //Đặt lại trạng thái cho winposition
                winPosition = string.Empty; //xóa trạng thái cũ
                foreach (var picBox in pictureBoxes)
                {
                    winPosition += picBox.Tag.ToString(); //Gán lại winposition theo thứ tự
                }    
                //Đặt lại trạng thái cho nút Pause
                button1.Text = "Pause";
                button1.Enabled = true; //Đảm bảo cho nút được kích hoạt lại

                //Reset trạng thái game
                label6.Text = "";
                //Reset điểm số
                label5.Text = "0";
            }    
        }
    }
}

