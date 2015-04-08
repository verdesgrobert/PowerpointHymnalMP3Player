using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibrary;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PowerpointHymnalMP3Player
{
    [Serializable]
    public enum AudioTypeEnum
    {
        NAudio, VLC
    }
    public partial class ThisAddIn
    {
        private IAudioPlayer audioPlayer;

        private string currentPath = "";
        List<int> currentTimes = new List<int>();
        private PowerPoint.Shape txtBox;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                this.Application.SlideShowNextSlide += Application_SlideShowNextSlide;
                Application.SlideShowBegin += Application_SlideShowBegin;
                this.Application.SlideShowEnd += Application_SlideShowEnd;
                Application.PresentationOpen += Application_PresentationOpen;
                Application.PresentationClose += Application_PresentationClose;


                CurrentApp = Application;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n" + exception.StackTrace);
            }
        }


        void audioPlayer_PositionChanged(double position, TimeSpan duration)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    int second = (int)audioPlayer.Position.TotalSeconds;
                    int nss = NextSlideSecond(second);
                    if (second > nss)
                        if (nss > 0)
                        {
                            currentTimes.RemoveAt(0);
                            CurrentApp.ActivePresentation.SlideShowWindow.View.Next();
                        }

                    txtBox.TextFrame.TextRange.Text = second + ":" + nss + ":" + string.Join("-", currentTimes);
                }
                catch (Exception e)
                {
                    txtBox.TextFrame.TextRange.Text = e.Message + ":" + e.StackTrace;
                }
            });
        }

        private int NextSlideSecond(int totSeconds)
        {
            int sec = -1;
            if (currentTimes.Any(el => el >= totSeconds))
                sec = currentTimes.First(el => el >= totSeconds);
            return sec;
        }
        public static PowerPoint.Application CurrentApp;
        void Application_PresentationOpen(PowerPoint.Presentation Pres)
        {
            if (Properties.Settings.Default.Mode == AudioTypeEnum.VLC)
                audioPlayer = new VLCPlayer();
            else
                audioPlayer = new NAudioPlayer();
            audioPlayer.Init();
            audioPlayer.PositionChanged += audioPlayer_PositionChanged;
        }

        void Application_PresentationClose(PowerPoint.Presentation Pres)
        {
            if (audioPlayer != null)
                audioPlayer.PositionChanged -= audioPlayer_PositionChanged;
        }
        void Application_SlideShowBegin(PowerPoint.SlideShowWindow Wn)
        {
            if (Wn.View.CurrentShowPosition == Properties.Settings.Default.SlideNumber)
            {
                Play(Wn.Presentation);
            }
        }

        private void Play(PowerPoint.Presentation Pres)
        {
            try
            {
                if (audioPlayer.IsPlaying) return;
                if (Properties.Settings.Default.Activated)
                {
                    audioPlayer.DisposeMediaIfAny();
                    currentPath = Pres.FullName.ToLower().Replace(".ppt", ".mp3");
                    try
                    {
                        StreamReader sr = new StreamReader(currentPath, Encoding.Default);
                        string firstLine = sr.ReadLine();
                        sr.Close();
                        var items = firstLine.Split(new string[] { "ita\0" }, StringSplitOptions.None);
                        var otherItems = items[1].Split('\0');
                        string boh = otherItems[0];

                        currentTimes = boh.Split('|').Select(el =>
                        {
                            int res;
                            return int.TryParse(el, out res) ? res : -1;

                        }).Where(el => el != -1).ToList();
                        txtBox = Pres.SlideShowWindow.View.Slide.Shapes.AddTextbox(
                            Office.MsoTextOrientation.msoTextOrientationHorizontal, 0, 0, 100, 20);
                        txtBox.TextFrame.TextRange.Text = string.Join("--", currentTimes);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    audioPlayer.SetSong(currentPath);
                    audioPlayer.Play();
                }
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message + "\n" + x.StackTrace);
            }
        }

        void Application_SlideShowEnd(PowerPoint.Presentation Pres)
        {
            audioPlayer.Stop();
            Pres.Saved = Office.MsoTriState.msoCTrue;
        }

        void Application_SlideShowNextSlide(PowerPoint.SlideShowWindow Wn)
        {
            if (Wn.View.CurrentShowPosition == Properties.Settings.Default.SlideNumber)
            {
                Play(Wn.Presentation);
            }
            else if (Wn.View.CurrentShowPosition <= Properties.Settings.Default.SlideNumber)
            {
                audioPlayer.Stop();
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            audioPlayer.Dispose();
        }

        #region Codice generato da VSTO

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }


}
