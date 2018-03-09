using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardboardVr
{
    public class SupportedVideoExtensions
    {
        private List<string> supVideoExtensions = new List<string> { ".mp4" };

        public bool isVideoFormatSupported(string extension)
        {
            return supVideoExtensions.Contains(extension);
        }
    }
}
