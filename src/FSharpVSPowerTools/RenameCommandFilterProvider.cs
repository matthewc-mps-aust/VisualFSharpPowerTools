﻿using System.Diagnostics;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using FSharpVSPowerTools.ProjectSystem;

namespace FSharpVSPowerTools.Refactoring
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("F#")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class RenameCommandFilterProvider : IVsTextViewCreationListener
    {
#pragma warning disable 0649

        [Import]
        internal IVsEditorAdaptersFactoryService EditorFactory { get; set; }

#pragma warning restore 0649

        [Import]
        private VSLanguageService fsharpVsLanguageService = null;

        [Import(typeof(SVsServiceProvider))]
        private System.IServiceProvider  serviceProvider = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorFactory.GetWpfTextView(textViewAdapter);
            if (textView == null) return;

            GeneralOptionsPage generalOptions = (GeneralOptionsPage)(Package.GetGlobalService(typeof(GeneralOptionsPage)));
            if (!generalOptions.RenameRefactoringEnabled)
            {
                Debug.WriteLine("[Rename Refactoring] The feature is disabled in General option page.");
                return;
            }

            AddCommandFilter(textViewAdapter, new RenameCommandFilter(textView, fsharpVsLanguageService, serviceProvider));
        }

        private static void AddCommandFilter(IVsTextView viewAdapter, RenameCommandFilter commandFilter)
        {
            if (!commandFilter.IsAdded)
            {
                // Get the view adapter from the editor factory
                IOleCommandTarget next;
                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (hr == VSConstants.S_OK)
                {
                    commandFilter.IsAdded = true;
                    // You'll need the next target for Exec and QueryStatus
                    if (next != null) commandFilter.NextTarget = next;
                }
            }
        }

    }
}