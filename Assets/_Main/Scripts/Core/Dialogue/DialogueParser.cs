using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Testing;
using System;


namespace Dialogue
{
    public class DialogueParser
    {

        // a word of any length so long as it is not proceeded by a white space
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";   // \w identifies word character  - \[, \] identifies start and ending brackets for commands- * allows for any character, any time - [*\s] removes whitespace

        public static Dialogue_Line Parse(string rawLine)
        {
            //Debug.Log($"Parsing line- '{rawLine}'");

            (string speaker, string dialogue, string commands) = RipContent(rawLine);

            //Debug.Log($"Speaker = '{speaker}'\nDialogue = '{dialogue}'\nCommand = '{commands}'");


            // We have to inject tags and variables into the speaker and dialogue separately because there are intial checks that have
            // to be performed
            // but commands need no checks, so we can inject the variables in them now
            commands = TagManager.Inject(commands);

            return new Dialogue_Line(rawLine, speaker, dialogue, commands);
        } 

        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands = "";    
            
            int dialogueStart = -1;
            int dialogueEnd = -1;
            bool isEscaped = false;   

         // allows parser to know there dialogue ends and if there are quotations in dialogue
            for ( int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];
                if (current == '\\')
                    isEscaped = !isEscaped;        
                else if (current == '"' && !isEscaped)
                {
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                else
                    isEscaped = false;
            }

            // Starts dialogue after the quotation mark, ends before the last quotation
            //Debug.Log(rawLine.Substring(dialogueStart + 1, (dialogueEnd - dialogueStart) - 1));


            // Identify Command Pattern
            Regex commandRegex = new Regex(commandRegexPattern);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach (Match match in matches)
            {
                if (match.Index < dialogueStart || match.Index > dialogueEnd)
                {
                    commandStart = match.Index;
                    break;
                }
            }

            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
                return ("", "", rawLine.Trim());

            // If we get here- either have a dialogue or a multi-word argument in a command. Differentiate command and dialogue.
            if (dialogueStart != -1 && dialogueEnd != -1 && commandStart == -1 || commandStart > dialogueEnd)
            {
                //valid dialogue
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart -1).Replace("\\\"", "\"");  // +1 bypasses quotation mark     replaces \\ with one quote
                if (commandStart != -1)
                    commands = rawLine.Substring(commandStart).Trim();
            }
            // command
            else if (commandStart != -1 && dialogueStart > commandStart)
                commands = rawLine;
            //speaker
            else
                dialogue = rawLine;

            return (speaker, dialogue, commands);
        }
    }
}