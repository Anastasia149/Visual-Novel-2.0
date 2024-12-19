using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisualNovel
{
    [System.Serializable]

    public class VN_ConversationDataCompressed
    {
        public string fileName;
        public int startIndex, endindex;
        public int progress;
    }
}