using Newtonsoft.Json;
using RecivedDataContolRemoted.Properties;
using RecivedDataContolRemoted.Request;
using System.IO.Ports;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace RecivedDataContolRemoted
{
    public partial class RemoteTVForm : Form
    {
        static SerialPort serialPort;

        static List<ButtonItem> buttonItems = new List<ButtonItem>();

        static string mes = "Слушаем событие для кнопки: ";

        static ButtonTypesRemoteTV selectedButton;
        public RemoteTVForm()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            PortsComboBox.Items.AddRange(ports);

            // Инициализация SerialPort
            serialPort = new SerialPort();
            serialPort.BaudRate = 9600;
            serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private StringBuilder receivedData = new StringBuilder();

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine(); // Читаем строку данных из порта

            receivedData.Append(data); // Добавляем данные в буфер

            if (data.Equals("\r"))
            {
                return;
            }
            string bufferContent = receivedData.ToString();

            string pattern = @"\((\d+)\)";

            Regex regex = new Regex(pattern);
            Match match = regex.Match(data);

            // Если найдено соответствие
            if (match.Success)
            {
                // Получение числа из найденного соответствия
                string numberString = match.Groups[1].Value;
                int number = int.Parse(numberString);

                if (bufferContent.Contains("Raw:") && number > 20) //Regex.IsMatch(bufferContent, @"\d+\r")
                {
                    // Если содержит, то обрабатываем сообщение
                    ProcessReceivedData(receivedData.ToString());

                    // Очищаем буфер
                    receivedData.Clear();
                }
            }
            receivedData.Clear();

        }

        private void ProcessReceivedData(string data)
        {

            var input = data.Trim().TrimEnd(',');

            // Удаление вхождений "Raw(число):" или "Raw: (число):"
            input = input.Replace("Raw:", "").Trim();
            int startIndex = input.IndexOf('(');
            int endIndex = input.IndexOf(')');

            if (startIndex != -1 && endIndex != -1)
            {
                input = input.Remove(startIndex, endIndex - startIndex + 1).Trim();
            }

            var findButton = buttonItems.FirstOrDefault(x => x.Type == selectedButton);

            var buttonItem = new ButtonItem()
            {
                Data = input,
                Type = selectedButton,
            };

            if (findButton != null)
            {
                SendMessageToRichBox("Перезаписываем кнопку " + selectedButton.ToString() + "\n" + data);
                buttonItems.Remove(findButton);
                buttonItems.Add(buttonItem);
            }
            else
            {
                SendMessageToRichBox("Записываем данные на кнопку " + selectedButton.ToString() + "\n" + data);
                buttonItems.Add(buttonItem);
            }
        }
        private void SendMessageToRichBox(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action<string>(SendMessageToRichBox), message);
            }
            else
            {
                richTextBox1.Text += message + "\n";
            }
        }

        private void changeSelectedbutton(ButtonTypesRemoteTV type)
        {
            selectedButton = type;
            SendMessageToRichBox(mes + type.ToString());
        }

        // power
        private void button1_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.power);
        }


        private void ArrowUp_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.arrow_drop_up);
        }



        private void NotSoundButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.mute);
        }

        private void AddVolumeButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.volumeup);
        }



        private void AddChanelButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.channelup);
        }

        private void RemoveVolumeButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.volumedown);
        }

        private void RemoveChannelButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.channeldown);
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.menu);
        }

        private void SourceButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.source);
        }

        private void ArrowLeftButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.arrow_left);
        }

        private void ArrowRigthButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.arrow_right);
        }

        private void ArrowDownButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.arrow_drop_down);
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.exit);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.display);
        }


        private void PortsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedPort = PortsComboBox.SelectedItem.ToString();
            serialPort.PortName = selectedPort;

            // Открываем порт только после выбора COM-порта
            serialPort.Open();
        }

        private void SaveDataButton_Click(object sender, EventArgs e)
        {
            //string json = JsonConvert.SerializeObject(buttonItems);
            //SendMessageToRichBox(json);

           

            UpdateInterfaceToDeviceRequest request = new UpdateInterfaceToDeviceRequest();
            request.externalDeviceId = "1572fdd9-81b7-4af4-8d57-3f13730b1dd7";
            request.interfaces = buttonItems.Select(x => new DeviceIterfaceRequest()
            {
                name = x.Type.ToString(),
                data = x.Data
            }).ToList();


            string json = JsonConvert.SerializeObject(request);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

            using (var client = new HttpClient(handler))
            {
                var q = client.PostAsync(Resources.SmartHubIP + "api/Device/" + "UpdateInterfaceToDevice/", content).Result;
            }

        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            changeSelectedbutton(ButtonTypesRemoteTV.enter);
        }
    }

    public class ButtonItem
    {
        public ButtonTypesRemoteTV Type { get; set; }
        public string Data { get; set;}
    }

    public enum ButtonTypesRemoteTV
    {
        power,
        mute,
        volumeup,
        channelup,
        volumedown,
        channeldown,
        menu,
        source,
        arrow_drop_up,
        arrow_drop_down,
        arrow_left,
        arrow_right,
        exit,
        display,
        enter
    }
}