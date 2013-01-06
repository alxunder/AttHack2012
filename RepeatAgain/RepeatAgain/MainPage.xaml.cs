using ATT.WP8.Controls.Utils;
using ATT.WP8.SDK;
using ATT.WP8.SDK.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RepeatAgain.Resources;
using Windows.Phone.Speech.Synthesis;

namespace RepeatAgain
{
    public partial class MainPage : PhoneApplicationPage
    {
        private SoundRecorder _soundRecorder;
        private bool _isRecording;

        //PhoneNumberChooserTask _phoneNumberChooserTask;
        SpeechSynthesizer synthesizer;

        public MainPage()
        {
            InitializeComponent();

            //Init synthesizer
            this.synthesizer = new SpeechSynthesizer();

            statusProgress.Visibility = Visibility.Collapsed;
            btnRecord.Content = "Start";
            CreateSoundRecorder();

            Unloaded += (sender, args) => _soundRecorder.Dispose();

        }

        private async void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!_isRecording)
            {
                _isRecording = true;
                btnRecord.Content = "Stop";
                _soundRecorder.StartRecording();
            }
            else
            {

                //statusProgress.Visibility = Visibility.Visible;
                _isRecording = false;
                btnRecord.IsEnabled = false;
                _soundRecorder.StopRecording();

                try
                {
                    btnSpeech.AudioContent = await _soundRecorder.GetBytes();
                    btnSpeech.AudioContentName = _soundRecorder.FilePath;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnRecord.Content = "Start";
                btnRecord.IsEnabled = true;
            }
        }

        private void CreateSoundRecorder()
        {
            try
            {
                _soundRecorder = new SoundRecorder();
                _soundRecorder.RecodingTimerStoped += RecodingTimerStoped;
            }
            catch (MicrophoneNotSupportedException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RecodingTimerStoped(object sender, EventArgs eventArgs)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => btnRecord_Click(this, new RoutedEventArgs()));
        }

        public string clientId { get; set; }

        public string clientSecret { get; set; }

        public string uriString { get; set; }

        private void btnSpeech_Click(object sender, RoutedEventArgs e)
        {
            btnRecord.IsEnabled = false;
            statusProgress.Visibility = Visibility.Visible;
        }

        private void btnSpeech_MessageTranscripted(object sender, ATT.WP8.Controls.TranscriptedMessageEventArgs e)
        {
            statusProgress.Visibility = Visibility.Collapsed;
            txtSpeechOutput.Text = btnSpeech.TranscriptedText;
            btnRecord.IsEnabled = true;
        }

        private void btnSpeech_Error(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception != null)
            {
                MessageBox.Show(exception.Message);
            }

            btnRecord.IsEnabled = true;
            statusProgress.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //await synthesizer.SpeakTextAsync("i am saying it");
            await synthesizer.SpeakTextAsync(btnSpeech.TranscriptedText + " in bed");
        }
    }
}