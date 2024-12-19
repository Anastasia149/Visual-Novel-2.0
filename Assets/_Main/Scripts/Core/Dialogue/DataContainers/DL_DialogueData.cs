using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dialogue
{
    public class DL_DialogueData
    {
        public string rawData { get; private set; } = string.Empty;
        public List<Dialogue_Segment> segments;
        private const string segmentIdentifierPattern = @"\{[a]\}|\{w[a]\s\d*\.?\d*\}";

        public DL_DialogueData(string rawDialogue)
        {
            this.rawData = rawDialogue;
            segments = RipSegments(rawDialogue);
        }

        public List<Dialogue_Segment> RipSegments(string rawDialogue)
        {
            List<Dialogue_Segment> segments = new List<Dialogue_Segment>();
            MatchCollection matches = Regex.Matches(rawDialogue, segmentIdentifierPattern);

            int lastIndex = 0;
            // find the forst or only segment in the file
            Dialogue_Segment segment = new Dialogue_Segment();
            segment.dialogue = matches.Count == 0 ? rawDialogue : rawDialogue.Substring(0, matches[0].Index);
            segment.startSignal = Dialogue_Segment.StartSignal.NONE;
            segment.signalDelay = 0;

            segments.Add(segment);

            if (matches.Count == 0)
                return segments;
            else
                lastIndex = matches[0].Index;

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                segment = new Dialogue_Segment();


                // get the start signal for the segment
                string signalMatch = match.Value; //{A}
                signalMatch = signalMatch.Substring(1, match.Length - 2);
                string[] signalSplit = signalMatch.Split(' ');

                segment.startSignal = (Dialogue_Segment.StartSignal)Enum.Parse(typeof(Dialogue_Segment.StartSignal), signalSplit[0].ToUpper());

                // get signal delay
                if (signalSplit.Length > 1)
                    float.TryParse(signalSplit[1], out segment.signalDelay);

                // get the dialogue for the segment
                int nextIndex = i + 1 < matches.Count ? matches[i + 1].Index : rawDialogue.Length;
                segment.dialogue = rawDialogue.Substring(lastIndex + match.Length, nextIndex - (lastIndex + match.Length));
                lastIndex = nextIndex;

                segments.Add(segment);
            }

            return segments;
        }

        public struct Dialogue_Segment
        {
            public string dialogue;
            public StartSignal startSignal;
            public float signalDelay;

            public enum StartSignal { NONE, A, WA }

            public bool appendText => (startSignal == StartSignal.A || startSignal == StartSignal.WA);
        }
    }
}