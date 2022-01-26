using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Text_Recognizer
{
    public partial class Form1 : Form
    {
        private string filePuth = string.Empty;
        private string lang = string.Empty;
        private Image image;

        public Form1()
        {
            InitializeComponent();
        }

        private void відкритиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = openFileDialog1.ShowDialog();
            if (res == DialogResult.OK)
            {
                filePuth = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(filePuth);

            }
            else
            {
                MessageBox.Show("Зображення не вибрано.", "Необхідно вибрати зображення!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private static Bitmap CaptureScreen()
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int screenX = Screen.PrimaryScreen.Bounds.X;
            int screenY = Screen.PrimaryScreen.Bounds.Y;

            Size boundsSize = Screen.PrimaryScreen.Bounds.Size;
            //Инициализирует новый экземпляр класса System.Drawing.Bitmap
            //заданными значениями размера и формата.
            Bitmap picture = new Bitmap(
            //Ширина в пикселях нового изображения System.Drawing.Bitmap.
            screenWidth,
            //Высота в пикселях нового изображения System.Drawing.Bitmap.
            screenHeight,
            //Указываем, что форматом отводится 32 бита на пиксель:
            // по 8 бит на красный,зеленый и синий каналы, а также альфа-канал.
            System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //Создаем новый объект System.Drawing.Graphics из
            //рисунка picture, с новым объектом System.Drawing.Graphics
            //для указанного объекта.
            Graphics graphics = Graphics.FromImage(picture);
            //Выполняем передачу данных о цвете, соответствующих
            //прямоугольной области пикселей, блоками битов с экрана на
            //поверхность рисования объекта System.Drawing.Graphics.
            graphics.CopyFromScreen(
            //Координата X точки в верхнем левом углу исходного прямоугольника.
            screenX,
            //Координата Y точки в верхнем левом углу исходного прямоугольника.
            screenY,
            //Координата X точки в верхнем левом углу конечного прямоугольника.
            0,
            //Координата Y точки в верхнем левом углу конечного прямоугольника.
            0,
            //Размер передаваемой области.
            boundsSize,
            //Область источника копируется прямо в область назначения.
            CopyPixelOperation.SourceCopy);
            //Возвращаем полученное изображение
            return picture;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(filePuth) || String.IsNullOrWhiteSpace(filePuth))
                {
                    throw new Exception("Зображення не вибрано ");
                }

                else if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("Мова не вибрана ");
                }
                else
                {
                    //Tesseract tesseract = new Tesseract(@"C:\train", lang, OcrEngineMode.TesseractLstmCombined);// Змінено в звязку з зміною розміщення папки з моднлями
                    Tesseract tesseract = new Tesseract(@"../net5.0-windows/train", lang, OcrEngineMode.TesseractLstmCombined);

                    tesseract.SetImage(new Image<Bgr, byte>(filePuth));
                    tesseract.Recognize();
                    richTextBox1.Text = tesseract.GetUTF8Text();
                    tesseract.Dispose();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }












        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
            {
                lang = "ukr";
            }
            else if (toolStripComboBox1.SelectedIndex == 1)
            {
                lang = "rus";
            }
            else if (toolStripComboBox1.SelectedIndex == 2)
            {
                lang = "eng";
            }
            else if (toolStripComboBox1.SelectedIndex == 3)
            {
                lang = "deu";
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //Преобразуємо кольорове в чорно біле// кнопка Ч/Б

            if (pictureBox1.Image != null) // якщо зображення в pictureBox1 є
            {
                // створюємо Bitmap з зображенням,яке знаходиться в pictureBox1
                Bitmap input = new Bitmap(pictureBox1.Image);
                // створюємо Bitmap для черно-білого зображення
                Bitmap output = new Bitmap(input.Width, input.Height);
                // перебираемо в циклах всі пікселі вихідного зображення
                for (int j = 0; j < input.Height; j++)
                    for (int i = 0; i < input.Width; i++)
                    {
                        // отримуєм (i, j) піксель
                        UInt32 pixel = (UInt32)(input.GetPixel(i, j).ToArgb());
                        // отримуєм компоненти кольорів пікселя
                        float R = (float)((pixel & 0x00FF0000) >> 16); // червоний
                        float G = (float)((pixel & 0x0000FF00) >> 8); // зелений
                        float B = (float)(pixel & 0x000000FF); // синій
                                                               // робимо кольор черно-білим (віддтінки сірого) - знаходимо среднє арифметичне
                        R = G = B = (R + G + B) / 3.0f;
                        // складаємо новий піксель по частинам (по каналам)
                        UInt32 newPixel = 0xFF000000 | ((UInt32)R << 16) | ((UInt32)G << 8) | ((UInt32)B);
                        // добавляємо его в Bitmap нового зображення
                        output.SetPixel(i, j, Color.FromArgb((int)newPixel));
                    }
                // виводимо черно-білый Bitmap в pictureBox1
                pictureBox1.Image = output;
            }


        }

        private void toolStripButton3_Click(object sender, EventArgs e) //Копіюємо текст в буфер обміну
        {
            try
            {
                Clipboard.SetText(richTextBox1.Text);
                //Якщо в буфері обміну є текст
                if (Clipboard.ContainsText() == true)
                {
                    toolStripLabel2.Text = "Текст скопійован у буфер обміну";
                    toolStripLabel2.BackColor = Color.Green;

                }
                else
                {
                    //Выводим сообщение о том, что в буфере обмена нет текста
                    toolStripLabel2.Text = "Буфер обміну порожній";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

           
           

        }

        private void toolStripButton4_Click(object sender, EventArgs e)//Захват екрану, розпізнавання тексту та копіювання в буфер обміну
        {
            try
            {
                pictureBox1.Image = CaptureScreen();
                image = pictureBox1.Image;
                //image.Save(@"D:\Институт\7семестр\СистемиШтучногоІнтелекту\Пример\Screen\temp.jpeg");//Запис в тимчасовий файл
                image.Save(@"../net5.0-windows/train\temp.jpeg");//Запис в тимчасовий файл
                if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("Мова не вибрана ");
                }
                //Tesseract tesseract = new Tesseract(@"C:\train", lang, OcrEngineMode.TesseractLstmCombined);//розпізнавання тексту "eng"// Змінено в звязку з зміною розміщення папки з моделями
                Tesseract tesseract = new Tesseract(@"../net5.0-windows/train", lang, OcrEngineMode.TesseractLstmCombined);//розпізнавання тексту "eng"

                //Tesseract tesseract = new Tesseract(@"C:\train", "eng", OcrEngineMode.TesseractLstmCombined);//розпізнавання тексту
                // tesseract.SetImage(new Image<Bgr, byte>(@"D:\Институт\7семестр\СистемиШтучногоІнтелекту\Пример\Screen\temp.jpeg"));
                tesseract.SetImage(new Image<Bgr, byte>(@"../net5.0-windows/train\temp.jpeg"));
                tesseract.Recognize();
                richTextBox1.Text = tesseract.GetUTF8Text();
                tesseract.Dispose();
                Clipboard.SetText(richTextBox1.Text);
                //Якщо в буфері обміну є текст
                if (Clipboard.ContainsText() == true)
                {
                    toolStripLabel2.Text = "Текст скопійован у буфер обміну";


                }
                else
                {
                    //Выводим сообщение о том, что в буфере обмена нет текста
                    toolStripLabel2.Text = "Буфер обміну порожній";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Щось пішло не так!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }






        }

        private void toolStripButton5_Click(object sender, EventArgs e)//Зберегти зображення в файл
        {
            if (pictureBox1.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.JPG)|*.JPG";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        image = pictureBox1.Image;
                        image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                filePuth = @"../net5.0-windows/train\temp.jpeg";//шлях до тимчасового файлу;
                pictureBox1.Image = CaptureScreen();
                image = pictureBox1.Image;
                image.Save(filePuth);//Запис в тимчасовий файл
                if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("Мова не вибрана ");
                }
                //Tesseract tesseract = new Tesseract(@"C:\train", lang, OcrEngineMode.TesseractLstmCombined);// Змінено в звязку з зміною розміщення папки з моделями
                Tesseract tesseract = new Tesseract(@"../net5.0-windows/train", lang, OcrEngineMode.TesseractLstmCombined);//розпізнавання тексту 

                tesseract.SetImage(new Image<Bgr, byte>(filePuth));
                tesseract.Recognize();
                richTextBox1.Text = tesseract.GetUTF8Text();
                tesseract.Dispose();
                Clipboard.SetText(richTextBox1.Text);
                //Якщо в буфері обміну є текст
                if (Clipboard.ContainsText() == true)
                {
                    toolStripLabel2.Text = "Текст скопійован у буфер обміну";

                }
                else
                {
                    //Выводим сообщение о том, что в буфере обмена нет текста
                    toolStripLabel2.Text = "Буфер обміну порожній";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Щось пішло не так!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {

            this.Width = 1000;
            this.Height = 70;

        }

       
    }
       
}
