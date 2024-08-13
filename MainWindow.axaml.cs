using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace OtpApp
{
    public partial class MainWindow : Window
    {
        private readonly Random _random = new Random();
        private readonly EmailService _emailService = new EmailService();

        public MainWindow()
        {
            InitializeComponent();
            UpdateRecipientTextboxStates();
            UpdateSendButtonState();
        }

        private void UpdateRecipientTextboxStates()
        {
            bool sendByEmail = SendByEmail.IsChecked ?? false;
            bool sendByPhone = SendByPhone.IsChecked ?? false;

            RecipientEmails.IsEnabled = sendByEmail;
            PhoneNumbers.IsEnabled = sendByPhone;

            UpdateSendButtonState();
        }

        private void UpdateSendButtonState()
        {
            bool sendByEmail = SendByEmail.IsChecked ?? false;
            bool sendByPhone = SendByPhone.IsChecked ?? false;

            SendOtpButton.IsEnabled = sendByEmail || sendByPhone;
        }

        private async void OnSendOtpClick(object sender, RoutedEventArgs e)
        {
            string emailAddress = EmailAddress?.Text?.Trim();
            string password = Password?.Text?.Trim();
            string recipientEmailsText = RecipientEmails?.Text?.Trim();
            string phoneNumbersText = PhoneNumbers?.Text?.Trim();
            string otpContent = OtpContent?.Text?.Trim();

            // Null checks and default value
            bool sendByEmail = SendByEmail.IsChecked ?? false;
            bool sendByPhone = SendByPhone.IsChecked ?? false;

            if (!sendByEmail && !sendByPhone)
            {
                // Show error message if neither email nor phone is selected
                //await ShowErrorMessageAsync("Please select at least one option (email or phone).");
                return;
            }

            if (string.IsNullOrWhiteSpace(otpContent))
            {
                // Show error message if OTP content is missing
                //await ShowErrorMessageAsync("OTP content cannot be empty.");
                return;
            }

            string otp = GenerateOtp();
            string otpMessage = $"{otpContent}\n\nYour OTP code is {otp}";

            var tasks = Enumerable.Empty<Task>();

            if (sendByEmail)
            {
                var recipientEmails = recipientEmailsText?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(email => email.Trim()).ToList();
                if (recipientEmails != null)
                {
                    var emailTasks = recipientEmails.Select(email => _emailService.SendEmailAsync(emailAddress, password, email, "Your OTP Code", otpMessage));
                    tasks = tasks.Concat(emailTasks);
                }
            }

            if (sendByPhone)
            {
                var phoneNumbers = phoneNumbersText?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(phone => phone.Trim()).ToList();
                if (phoneNumbers != null)
                {
                    var smsTasks = phoneNumbers.Select(phone => SendSmsAsync(phone, otpMessage));
                    tasks = tasks.Concat(smsTasks);
                }
            }

            await Task.WhenAll(tasks);
        }

        private string GenerateOtp()
        {
            return _random.Next(100000, 999999).ToString();
        }

        private async Task SendSmsAsync(string phoneNumber, string message)
        {
            const string accountSid = "your_account_sid"; // Replace with your Twilio account SID
            const string authToken = "your_auth_token";  // Replace with your Twilio auth token

            TwilioClient.Init(accountSid, authToken);

            await MessageResource.CreateAsync(
                to: new PhoneNumber(phoneNumber),
                from: new PhoneNumber("your_twilio_phone_number"), // Replace with your Twilio phone number
                body: message);
        }

        //private async Task ShowErrorMessageAsync(string message)
        //{
        //    var dialog = new MessageBox("Error", message);
        //    await dialog.ShowDialog(this);
        //}

        private void OnSendByEmailChecked(object sender, RoutedEventArgs e)
        {
            UpdateRecipientTextboxStates();
        }

        private void OnSendByEmailUnchecked(object sender, RoutedEventArgs e)
        {
            UpdateRecipientTextboxStates();
        }

        private void OnSendByPhoneChecked(object sender, RoutedEventArgs e)
        {
            UpdateRecipientTextboxStates();
        }

        private void OnSendByPhoneUnchecked(object sender, RoutedEventArgs e)
        {
            UpdateRecipientTextboxStates();
        }
    }
}
