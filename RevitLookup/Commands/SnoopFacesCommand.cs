﻿/*
 * Created By WeiGan 2021.9.9
 * 
 */

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitLookupWpf.Helpers;
using RevitLookupWpf.View;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace RevitLookupWpf.Commands
{
    [RvtCommandInfo(Name = "Snoop Face", Image = "search.png")]
    [Transaction(TransactionMode.Manual)]
    public class SnoopFacesCommand : RvtCommandBase
    {
        public override Result SnoopClick(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                message = Resource.NoActiveDocument;
                return Result.Cancelled;
            }
            var lookupWindow = new LookupWindow(ProcessManager.GetActivateWindow());
            List<GeometryObject> geos = new List<GeometryObject>();
            TaskDialog.Show(Resource.AppName, "Select Ordered Faces,Press Esc To Finish", TaskDialogCommonButtons.Ok);
            while (true)
            {
                try
                {
                    var refElem = uidoc.Selection.PickObject(ObjectType.Face);
                    var geometryObject = uidoc.Document.GetElement(refElem).GetGeometryObjectFromReference(refElem);
                    geos.Add(geometryObject);
                }
                catch (Exception)
                {
                    //user press esc
                    break;
                }
            }
            if (geos.Count == 0) return Result.Cancelled;
            if (geos.Count == 1) lookupWindow.SetRvtInstance(geos.FirstOrDefault());
            else lookupWindow.SetRvtInstance(geos);
            lookupWindow.Show();

            return Result.Succeeded;
        }
    }
}
