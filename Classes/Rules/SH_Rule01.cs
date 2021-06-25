﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;


namespace SimpleShapeGrammar.Classes
{
    [Serializable]
    public class SH_Rule01 : SH_Rule
    {
        // --- properties ---
        public Vector3d TranslateStart { get; private set; }
        public Vector3d TranslateEnd { get; private set; }
        
        public State RuleState = State.alpha;

        // --- constructors ---
        public SH_Rule01()
        {
            // empty constructor
        }
        public SH_Rule01(Vector3d _translate_start, Vector3d _translate_end)
        {
            TranslateStart = _translate_start;
            TranslateEnd = _translate_end;
            
        }

        // --- methods ---
        public override void RuleOperation(SH_SimpleShape _ss)
        {
            // check if the state maches the simple shape state
            if (_ss.SimpleShapeState != RuleState)
            {
                return;
            }

            // take the 1st element
            SH_Element sh_elem = _ss.Elements[0];

            // apply the change
            #region NewMethod
            // new points
            //Point3d newStart = sh_elem.Nodes[0].Position + TranslateStart;
            //Point3d newEnd = sh_elem.Nodes[1].Position + TranslateEnd;            
            _ss.TranslateNode(TranslateStart, (int)sh_elem.Nodes[0].ID);
            _ss.TranslateNode(TranslateEnd, (int)sh_elem.Nodes[1].ID);

            #endregion

            #region Original Method

            /*
            Point3d currentStart = sh_line.Nodes[0].Position;
            Point3d currentEnd = sh_line.Nodes[1].Position;
            
            sh_line.Nodes[0].Position = currentStart + TranslateStart;
            sh_line.Nodes[1].Position = currentEnd + TranslateEnd;
            */

            #endregion
            // change the state
            _ss.SimpleShapeState = State.beta;

        }

    }
}
