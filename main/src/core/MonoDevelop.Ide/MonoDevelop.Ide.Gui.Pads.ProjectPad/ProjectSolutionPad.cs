//
// ProjectSolutionPad.cs
//
// Author:
//   Lluis Sanchez Gual
//
// Copyright (C) 2005 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Resources;
using System.Collections.Generic;

using MonoDevelop.Projects;
using MonoDevelop.Core;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Components;
using System.Text;

namespace MonoDevelop.Ide.Gui.Pads.ProjectPad
{
	public class ProjectSolutionPad: SolutionPad
	{
		public ProjectSolutionPad ()
		{
			IdeApp.Workbench.ActiveDocumentChanged += new EventHandler (OnWindowChanged);
		}
		
		public override void Initialize (NodeBuilder[] builders, TreePadOption[] options, string contextMenuPath)
		{
			base.Initialize (builders, options, contextMenuPath);
			var aliases = new Dictionary<string,string> ();
			aliases.Add ("SystemFile", "MonoDevelop.Ide.Gui.Pads.ProjectPad.SystemFile");
			aliases.Add ("ProjectFolder", "MonoDevelop.Ide.Gui.Pads.ProjectPad.ProjectFolder");
			TreeView.ContextMenuTypeNameAliases = aliases;
			TreeView.EnableDragUriSource (n => {
				var pf = n as ProjectFile;
				if (pf != null) {
					return new Uri (pf.FilePath).AbsoluteUri;
				}
				return null;
			});
			TreeView.ShowSelectionPopupButton = true;
		}
		
		protected override void OnSelectionChanged (object sender, EventArgs args)
		{
			base.OnSelectionChanged (sender, args);
			ITreeNavigator nav = treeView.GetSelectedNode ();
			if (nav != null) {
				WorkspaceItem c = (WorkspaceItem) nav.GetParentDataItem (typeof(WorkspaceItem), true);
				IdeApp.ProjectOperations.CurrentSelectedWorkspaceItem = c;
				SolutionItem ce = (SolutionItem) nav.GetParentDataItem (typeof(SolutionItem), true);
				IdeApp.ProjectOperations.CurrentSelectedSolutionItem = ce;
				IdeApp.ProjectOperations.CurrentSelectedItem = nav.DataItem;
			}
		}
		
		protected override void OnCloseWorkspace (object sender, WorkspaceItemEventArgs e)
		{
			base.OnCloseWorkspace (sender, e);
			IdeApp.ProjectOperations.CurrentSelectedSolutionItem = null;
			IdeApp.ProjectOperations.CurrentSelectedWorkspaceItem = null;
		}
		
		void OnWindowChanged (object ob, EventArgs args)
		{
			DispatchService.GuiDispatch (new MessageHandler (SelectActiveFile));
		}
		
		void SelectActiveFile ()
		{
			Document doc = IdeApp.Workbench.ActiveDocument;
			if (doc != null && doc.Project != null) {
				string file = doc.FileName;
				if (file != null) {
					ProjectFile pf = doc.Project.Files.GetFile (file);
					if (pf != null) {
						ITreeNavigator nav = treeView.GetNodeAtObject (pf, true);
						if (nav != null) {
							nav.ExpandToNode ();
							nav.Selected = true;
						}
					}
				}
			}
		}
	}
}

