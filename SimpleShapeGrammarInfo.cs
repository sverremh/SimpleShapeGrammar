using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ShapeGrammar
{
    public class SimpleShapeGrammarInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "SimpleShapeGrammar";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("620f08ca-4392-442c-bc0e-5fc52a754388");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
