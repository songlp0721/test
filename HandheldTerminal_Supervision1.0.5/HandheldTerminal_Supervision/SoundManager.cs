using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Media;

namespace HandheldTerminal_Supervision
{
    public static class SoundManager
    {
        private const string filePath0 = @"\windows\Barcodebeep.wav";
        private const string filePath1 = @"\windows\asterisk.wav";
        public static bool PlaySound(int witchone)
        {
            string path = "";
            try
            {
                switch (witchone)
                {
                    case 0:
                        path = filePath0;
                        break;
                    case 1:
                        path = filePath1;
                        break;
                    default:
                        return false;
                }

                if (System.IO.File.Exists(path))
                {
                    using (SoundPlayer player = new SoundPlayer(path))
                    {
                        player.Play();
                    }
                    return true;
                }
                return false;

            }
            catch (System.Exception)
            {
                return false;
            }


        }
    }
}
