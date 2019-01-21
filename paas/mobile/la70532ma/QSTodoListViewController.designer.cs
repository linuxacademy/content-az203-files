// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace la70532ma
{
    [Register ("QSTodoListViewController")]
    partial class QSTodoListViewController
    {
        [Outlet]
        UIKit.UITextField itemText { get; set; }

        [Action ("OnAdd:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnAdd (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (itemText != null) {
                itemText.Dispose ();
                itemText = null;
            }
        }
    }
}