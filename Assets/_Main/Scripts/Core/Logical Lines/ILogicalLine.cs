using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue.LogicalLines
{
    public interface ILogicalLine
    {
        string keyword { get; }
        bool Matches(Dialogue_Line line);
        IEnumerator Execute(Dialogue_Line line);
    }
}