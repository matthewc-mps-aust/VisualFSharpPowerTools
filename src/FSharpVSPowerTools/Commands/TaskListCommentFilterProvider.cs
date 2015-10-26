﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using FSharpVSPowerTools.TaskList;
using Microsoft.VisualStudio.Editor;
using FSharpVSPowerTools.ProjectSystem;
using Microsoft.VisualStudio.TextManager.Interop;

namespace FSharpVSPowerTools
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("F#")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class TaskListCommentFilterProvider : IVsTextViewCreationListener
    {
        internal IWpfTextView _textView;
        internal TaskListCommentFilter _taskCommentFilter;

        private readonly System.IServiceProvider _serviceProvider;
        private readonly IVsEditorAdaptersFactoryService _editorFactory;
        private readonly TaskListManager _taskListManager;

        [ImportingConstructor]
        public TaskListCommentFilterProvider(
            [Import(typeof(SVsServiceProvider))] System.IServiceProvider serviceProvider,
            IVsEditorAdaptersFactoryService editorFactory,
            TaskListManager taskListManager)
        {
            _serviceProvider = serviceProvider;
            _editorFactory = editorFactory;
            _taskListManager = taskListManager;
        }
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            _textView = _editorFactory.GetWpfTextView(textViewAdapter);
            if (_textView == null) return;

            var generalOptions = Setting.getGeneralOptions(_serviceProvider);
            if (generalOptions == null || !generalOptions.TaskListCommentsEnabled) return;

            _taskCommentFilter = new TaskListCommentFilter(_textView, _serviceProvider, _taskListManager);
        }
    }
}