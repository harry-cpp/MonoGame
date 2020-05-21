// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoGame.Tools.Pipeline
{
    public enum AskResult
    {
        Yes,
        No,
        Cancel
    }

    public interface IView
    {
        void Attach(IController controller);

        void Invoke(Action action);

        AskResult AskSaveOrCancel();

        bool AskSaveName(ref string filePath, string title);

        bool AskOpenProject(out string projectFilePath);

        bool AskImportProject(out string projectFilePath);

        void ShowError(string title, string message);

        void ShowMessage(string message);

        void UpdateRecentList(List<string> recentList);

        void InitSolutionExplorer(PipelineProject project);
    }
}
