﻿/*
 * Created By WeiGan 2021.9.9
 * 
 */

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitLookupWpf.Helpers;
using RevitLookupWpf.View;

namespace RevitLookupWpf.Commands
{
    [Transaction(TransactionMode.Manual)]
    [RvtCommandInfo(Name = "Snoop\nApplication", Image = "search.png")]
    public class SnoopUIApplicationCommand : RvtCommandBase
    {
        public override Result SnoopClick(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var lookupWindow = new LookupWindow(ProcessManager.GetActivateWindow());
                lookupWindow.SetRvtInstance(commandData.Application);
                lookupWindow.Show();
            }
            catch (Exception)
            {
                throw;
            }

            return Result.Succeeded;
        }
    }
}
