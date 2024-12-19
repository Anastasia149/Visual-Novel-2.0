using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dialogue
{
    public class DL_SpeakerData
    {
        public string rawData { get; private set; } = string.Empty;
        public string name, castName;

        // name that will display in the dialogue box to show who is speaking
        public string displayName => isCastingName ? castName : name;

        public Vector2 castPosition;

        public List<(int layer, string expression)> CastExpressions { get; set; }

        public bool isCastingName => castName != string.Empty;
        public bool isCastingPosition = false;
        public bool isCastingExpressions => CastExpressions.Count > 0;

        public bool makeCharacterEnter = false;

        private const string NameCastID = " as ";
        private const string PositionCastID = " at ";
        private const string ExpressionCastID = " [";
        private const char AxisDelimiter = ':';                 // separates position
        private const char ExpressionLayerDelimiter = ':';      // separates expressions from one another
        private const char ExpressionLayerJoiner = ',';         // joins expressions

        private const string EnterID = "enter ";


        // looks for "enter" before speaker name
        private string ProcessKeywords (string rawSpeaker)
        {
            if (rawSpeaker.StartsWith(EnterID))
            {
                rawSpeaker = rawSpeaker.Substring(EnterID.Length);
                makeCharacterEnter = true;
            }

            return rawSpeaker;
        }
        public DL_SpeakerData(string rawSpeaker)
        {
            rawData = rawSpeaker;
            rawSpeaker = ProcessKeywords(rawSpeaker);

            string pattern = @$"{NameCastID}|{PositionCastID}|{ExpressionCastID.Insert(ExpressionCastID.Length - 1, @"\")}";
            MatchCollection matches = Regex.Matches(rawSpeaker, pattern);

            // populate this data to avoid null references to values
            castName = "";
            castPosition = Vector2.zero;
            CastExpressions = new List<(int layer, string expression)>();


            // if no matches, the entire line is the speaker name
            if (matches.Count == 0)
            {
                name = rawSpeaker;
                return;
            }

            // otherwise, isolate speakername from casting data
            int index = matches[0].Index;

            name = rawSpeaker.Substring(0, index);

            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];

                int startIndex = 0, endIndex = 0;

                if (match.Value == NameCastID)
                {
                    startIndex = match.Index + NameCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    castName = rawSpeaker.Substring(startIndex, endIndex - startIndex);
                }

                else if (match.Value == PositionCastID)
                {
                    isCastingPosition = true;

                    startIndex = match.Index + PositionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castPos = rawSpeaker.Substring(startIndex, endIndex - startIndex);

                    string[] axis = castPos.Split(AxisDelimiter, System.StringSplitOptions.RemoveEmptyEntries);

                    float.TryParse(axis[0], out castPosition.x);

                    if (axis.Length > 1)
                        float.TryParse(axis[1], out castPosition.y);
                }

                else if (match.Value == ExpressionCastID)
                {
                    startIndex = match.Index + ExpressionCastID.Length;
                    endIndex = i < matches.Count - 1 ? matches[i + 1].Index : rawSpeaker.Length;
                    string castExp = rawSpeaker.Substring(startIndex, endIndex - (startIndex + 1));

                    CastExpressions = castExp.Split(ExpressionLayerJoiner)
                    .Select(x =>
                    {
                        var parts = x.Trim().Split(ExpressionLayerDelimiter);

                        if (parts.Length == 2)
                            return (int.Parse(parts[0]), parts[1]);
                        else
                            return (0, parts[0]);
                    }).ToList();
                }
            }
        }
    }
}