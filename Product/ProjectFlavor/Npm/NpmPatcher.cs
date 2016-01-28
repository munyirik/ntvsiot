using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Microsoft.NodejsUwp
{
    /// <summary>
    /// Provides basic logic for applying UWP patches to npm packages.
    /// </summary>
    public class NpmPatcher
    {
        /// <summary>
        /// Path to Visual Studio project.
        /// </summary>
        private string ProjectPath { get; set; }

        /// <summary>
        /// Path of downloaded zip file with patch.
        /// </summary>
        private string ZipFilePath { get; set; }

        /// <summary>
        /// Visual Studio output pane to display messages.
        /// </summary>
        private IVsOutputWindowPane NpmOutputPane { get; set; }

        /// <summary>
        /// Name of the npm package.
        /// </summary>
        private string PackageName { get; set; }

        /// <summary>
        /// Maps the files in ZipFilePath to their destinations in the npm package to be patched.
        /// </summary>
        private Dictionary<string, string> PatchMap { get; set; }

        /// <summary>
        /// Uri of zip file with patch
        /// </summary>
        private Uri PatchUri { get; set; }

        /// <summary>
        /// Main (and only) method called by patcher classes.
        /// </summary>
        /// <param name="uri">Uri of zip file with patch</param>
        /// <param name="destPath">Patch download destination</param>
        /// <param name="pane">Visual Studio output pane to display messages</param>
        /// <param name="packageName">Name of the npm package</param>
        /// <param name="patchMap">Maps the files in in patch to their destinations</param>
        internal void UpdatePackage(Uri uri, string destPath, IVsOutputWindowPane pane, string packageName, Dictionary<string, string> patchMap)
        {
            ProjectPath = string.Format(CultureInfo.CurrentCulture, "{0}\\", destPath);

            // Check if package is already installed
            if (!Directory.Exists(string.Format(CultureInfo.CurrentCulture, "{0}node_modules\\{1}", ProjectPath, packageName)))
            {
                return;
            }

            NpmOutputPane = pane;
            PackageName = packageName;
            PatchMap = patchMap;
            PatchUri = uri;

            DownloadPatch();
        }


        /// <summary>
        /// Initiates patch download.
        /// </summary>
        private void DownloadPatch()
        {
            WebClient client = new WebClient();
            ZipFilePath = string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", ProjectPath, PackageName, "patch.zip");

            NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Downloading {0} UWP patch from {1}",
                PackageName, PatchUri.ToString()), NpmOutputPane);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadPatchCompleted);
            client.DownloadFileAsync(PatchUri, ZipFilePath);
        }

        /// <summary>
        /// Extracts and applies patch after download completes.
        /// </summary>
        private void DownloadPatchCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Download failed: {0}",
                    e.Error.Message), NpmOutputPane);
                return;
            }

            NpmHandler.PrintOutput("Download complete.", NpmOutputPane);
            ExtractPatch();
            ApplyPatch();
        }


        /// <summary>
        /// Extracts patch zip file.
        /// </summary>
        private void ExtractPatch()
        {
            NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Extracting {0} UWP patch.", 
                PackageName), NpmOutputPane);

            string ZipFileExtractPath = string.Format(CultureInfo.CurrentCulture, "{0}{1}patch", ProjectPath, PackageName);

            try
            {
                // Clean existing extracted path
                if (Directory.Exists(ZipFileExtractPath))
                {
                    Directory.Delete(ZipFileExtractPath, true);
                }
                ZipFile.ExtractToDirectory(ZipFilePath, ZipFileExtractPath);
            }
            catch (Exception ex)
            {
                NpmHandler.PrintOutput(ex.Message, NpmOutputPane);
                return;
            }
            NpmHandler.PrintOutput("Extraction complete.", NpmOutputPane);
        }

        /// <summary>
        /// Applies patch to npm package.
        /// </summary>
        private void ApplyPatch()
        {
            foreach(KeyValuePair<string, string> pair in PatchMap)
            {
                FileInfo src = new FileInfo(string.Format("{0}{1}patch\\{2}", ProjectPath, PackageName, pair.Key));
                FileInfo dest = new FileInfo(string.Format("{0}{1}", ProjectPath, pair.Value));

                if (!File.Exists(src.FullName))
                {
                    NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Error. {0} does not exist",
                        src.FullName), NpmOutputPane);
                    continue;
                }
         
                try
                {
                    if (!Directory.Exists(dest.DirectoryName))
                    {
                        Directory.CreateDirectory(dest.DirectoryName);
                    }
                    NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "Copying {0} to {1}",
                        src.FullName, dest.FullName), NpmOutputPane);
                    File.Copy(src.FullName, dest.FullName, true);
                }
                catch (Exception ex)
                {
                    NpmHandler.PrintOutput(ex.Message, NpmOutputPane);
                }
            }
            NpmHandler.PrintOutput(string.Format(CultureInfo.CurrentCulture, "{0} UWP patch applied!",
                PackageName), NpmOutputPane);
        }
    }
}
