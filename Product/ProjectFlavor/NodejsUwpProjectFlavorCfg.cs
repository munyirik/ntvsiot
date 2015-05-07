/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Debugger.Interop;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Globalization;

namespace Microsoft.NodejsUwp
{
    class NodejsUwpProjectFlavorCfg :
        IVsCfg,
        IVsProjectCfg,
        IVsProjectCfg2,
        IVsProjectFlavorCfg,
        IVsDebuggableProjectCfg,
        ISpecifyPropertyPages,
        IVsSpecifyProjectDesignerPages,
        IVsCfgBrowseObject,
        IVsDeployableProjectCfg,
        IVsProjectCfgDebugTargetSelection,
        IVsAppContainerProjectDeployCallback,
        IVsBuildPropertyStorage,
        IVsQueryDebuggableProjectCfg,
        IPersistXMLFragment
    {
        // The IVsCfg object of the base project.
        private IVsCfg _baseCfg;

        // The IVsProjectFlavorCfg object of the inner project subtype. 
        private IVsProjectFlavorCfg _innerCfg;

        private readonly object syncObject = new object();
        private EventSinkCollection deployCallbackCollection = new EventSinkCollection();
        private IVsAppContainerProjectDeployOperation deployOp;
        private IVsTask appContainerBootstrapperOperation;
        private IVsOutputWindowPane outputWindow;
        private IVsDebuggerDeployConnection connection;
        private string deployPackageMoniker;
        private string deployAppUserModelID;
        private IVsHierarchy project;
        private const string RemoteTarget = "Remote Machine";
        private static readonly Guid NativePortSupplier = new Guid("3b476d38-a401-11d2-aad4-00c04f990171"); // NativePortSupplier corespond to connection with No-Authentication mode
        private bool isDirty;
        private Dictionary<string, string> propertiesList = new Dictionary<string, string>();
        static Dictionary<IVsCfg, NodejsUwpProjectFlavorCfg> mapIVsCfgToNodejsUwpProjectFlavorCfg = new Dictionary<IVsCfg, NodejsUwpProjectFlavorCfg>();

        #region properties

        internal string DeployPackageMoniker
        {
            get { return this.deployPackageMoniker; }
        }

        internal string DeployAppUserModelID
        {
            get { return this.deployAppUserModelID; }
        }

        #endregion
        public NodejsUwpProjectFlavorCfg(NodejsUwpProjectFlavor baseProjectNode, IVsCfg baseConfiguration, IVsProjectFlavorCfg innerConfiguration)
        {
            _baseCfg = baseConfiguration;
            _innerCfg = innerConfiguration;
            project = baseProjectNode;
            mapIVsCfgToNodejsUwpProjectFlavorCfg.Add(baseConfiguration, this);
        }

        #region IVsCfg Members

        public int get_DisplayName(out string pbstrDisplayName)
        {
            int ret = _baseCfg.get_DisplayName(out pbstrDisplayName);

            return ret;
        }

        public int get_IsDebugOnly(out int pfIsDebugOnly)
        {
            return _baseCfg.get_IsDebugOnly(out pfIsDebugOnly);
        }

        public int get_IsReleaseOnly(out int pfIsReleaseOnly)
        {
            return _baseCfg.get_IsReleaseOnly(out pfIsReleaseOnly);
        }

        #endregion

        #region IVsProjectCfg Members

        public int EnumOutputs(out IVsEnumOutputs ppIVsEnumOutputs)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.EnumOutputs(out ppIVsEnumOutputs);
            }
            ppIVsEnumOutputs = null;
            return VSConstants.E_NOTIMPL;
        }

        public int OpenOutput(string szOutputCanonicalName, out IVsOutput ppIVsOutput)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.OpenOutput(szOutputCanonicalName, out ppIVsOutput);
            }
            ppIVsOutput = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_BuildableProjectCfg(out IVsBuildableProjectCfg ppIVsBuildableProjectCfg)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_BuildableProjectCfg(out ppIVsBuildableProjectCfg);
            }
            ppIVsBuildableProjectCfg = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_CanonicalName(out string pbstrCanonicalName)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_CanonicalName(out pbstrCanonicalName);
            }
            pbstrCanonicalName = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_IsPackaged(out int pfIsPackaged)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_IsPackaged(out pfIsPackaged);
            }
            pfIsPackaged = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_IsSpecifyingOutputSupported(out int pfIsSpecifyingOutputSupported)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_IsSpecifyingOutputSupported(out pfIsSpecifyingOutputSupported);
            }
            pfIsSpecifyingOutputSupported = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_Platform(out Guid pguidPlatform)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_Platform(out pguidPlatform);
            }
            pguidPlatform = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public int get_ProjectCfgProvider(out IVsProjectCfgProvider ppIVsProjectCfgProvider)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_ProjectCfgProvider(out ppIVsProjectCfgProvider);
            }
            ppIVsProjectCfgProvider = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_RootURL(out string pbstrRootURL)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_RootURL(out pbstrRootURL);
            }
            pbstrRootURL = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_TargetCodePage(out uint puiTargetCodePage)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_TargetCodePage(out puiTargetCodePage);
            }
            puiTargetCodePage = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_UpdateSequenceNumber(ULARGE_INTEGER[] puliUSN)
        {
            IVsProjectCfg projCfg = _innerCfg as IVsProjectCfg;
            if (projCfg != null)
            {
                return projCfg.get_UpdateSequenceNumber(puliUSN);
            }
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsProjectCfg2 Members

        public int OpenOutputGroup(string szCanonicalName, out IVsOutputGroup ppIVsOutputGroup)
        {
            IVsProjectCfg2 projCfg = _baseCfg as IVsProjectCfg2;
            if (projCfg != null)
            {
                return projCfg.OpenOutputGroup(szCanonicalName, out ppIVsOutputGroup);
            }
            ppIVsOutputGroup = null;
            return VSConstants.E_NOTIMPL;
        }

        public int OutputsRequireAppRoot(out int pfRequiresAppRoot)
        {
            IVsProjectCfg2 projCfg = _baseCfg as IVsProjectCfg2;
            if (projCfg != null)
            {
                return projCfg.OutputsRequireAppRoot(out pfRequiresAppRoot);
            }
            pfRequiresAppRoot = 1;
            return VSConstants.E_NOTIMPL;
        }

        public int get_CfgType(ref Guid iidCfg, out IntPtr ppCfg)
        {
            ppCfg = IntPtr.Zero;

            // See if this is an interface we support
            if (iidCfg == typeof(IVsDebuggableProjectCfg).GUID)
            {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsDebuggableProjectCfg));
            }
            else if (iidCfg == typeof(IVsDeployableProjectCfg).GUID)
            {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsDeployableProjectCfg));
            }
            else if (iidCfg == typeof(IVsQueryDebuggableProjectCfg).GUID)
            {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsQueryDebuggableProjectCfg));
            }
            else if (iidCfg == typeof(IVsBuildableProjectCfg).GUID)
            {
                IVsBuildableProjectCfg buildableConfig;
                this.get_BuildableProjectCfg(out buildableConfig);
                //
                //In some cases we've intentionally shutdown the build options
                //  If buildableConfig is null then don't try to get the BuildableProjectCfg interface
                //  
                if (null != buildableConfig)
                {
                    ppCfg = Marshal.GetComInterfaceForObject(buildableConfig, typeof(IVsBuildableProjectCfg));
                }
            }

            // If not supported
            if (ppCfg == IntPtr.Zero)
                return VSConstants.E_NOINTERFACE;

            return VSConstants.S_OK;
        }

        public int get_IsPrivate(out int pfPrivate)
        {
            IVsProjectCfg2 projCfg = _baseCfg as IVsProjectCfg2;
            if (projCfg != null)
            {
                return projCfg.get_IsPrivate(out pfPrivate);
            }
            pfPrivate = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_OutputGroups(uint celt, IVsOutputGroup[] rgpcfg, uint[] pcActual = null)
        {
            IVsProjectCfg2 projCfg = _baseCfg as IVsProjectCfg2;
            if (projCfg != null)
            {
                return projCfg.get_OutputGroups(celt, rgpcfg, pcActual);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int get_VirtualRoot(out string pbstrVRoot)
        {
            IVsProjectCfg2 projCfg = _baseCfg as IVsProjectCfg2;
            if (projCfg != null)
            {
                return projCfg.get_VirtualRoot(out pbstrVRoot);
            }
            pbstrVRoot = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsProjectFlavorCfg Members

        public int Close()
        {
            mapIVsCfgToNodejsUwpProjectFlavorCfg.Remove(_baseCfg);

            IVsProjectFlavorCfg cfg = _innerCfg as IVsProjectFlavorCfg;
            if (cfg != null)
            {
                return cfg.Close();
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsDebuggableProjectCfg Members

        public int DebugLaunch(uint flags)
        {
            VsDebugTargetInfo2[] targets;
            int hr = QueryDebugTargets(out targets);
            if (ErrorHandler.Failed(hr))
                return hr;

            flags |= (uint)__VSDBGLAUNCHFLAGS140.DBGLAUNCH_ContainsStartupTask;

            VsDebugTargetInfo4[] appPackageDebugTarget = new VsDebugTargetInfo4[1];
            int targetLength = (int)Marshal.SizeOf(typeof(VsDebugTargetInfo4));

            appPackageDebugTarget[0].AppPackageLaunchInfo.AppUserModelID = DeployAppUserModelID;
            appPackageDebugTarget[0].AppPackageLaunchInfo.PackageMoniker = DeployPackageMoniker;

            appPackageDebugTarget[0].dlo = (uint)_DEBUG_LAUNCH_OPERATION4.DLO_AppPackageDebug;
            appPackageDebugTarget[0].LaunchFlags = flags;
            appPackageDebugTarget[0].bstrRemoteMachine = targets[0].bstrRemoteMachine;
            appPackageDebugTarget[0].bstrExe = targets[0].bstrExe;
            appPackageDebugTarget[0].bstrArg = targets[0].bstrArg;
            appPackageDebugTarget[0].bstrCurDir = targets[0].bstrCurDir;
            appPackageDebugTarget[0].bstrEnv = targets[0].bstrEnv;
            appPackageDebugTarget[0].dwProcessId = targets[0].dwProcessId;
            appPackageDebugTarget[0].pStartupInfo = IntPtr.Zero; //?
            appPackageDebugTarget[0].guidLaunchDebugEngine = targets[0].guidLaunchDebugEngine;
            appPackageDebugTarget[0].dwDebugEngineCount = targets[0].dwDebugEngineCount;
            appPackageDebugTarget[0].pDebugEngines = targets[0].pDebugEngines;
            appPackageDebugTarget[0].guidPortSupplier = targets[0].guidPortSupplier;

            appPackageDebugTarget[0].bstrPortName = targets[0].bstrPortName;
            appPackageDebugTarget[0].bstrOptions = targets[0].bstrOptions;
            appPackageDebugTarget[0].fSendToOutputWindow = targets[0].fSendToOutputWindow;
            appPackageDebugTarget[0].pUnknown = targets[0].pUnknown;
            appPackageDebugTarget[0].guidProcessLanguage = targets[0].guidProcessLanguage;
            appPackageDebugTarget[0].project = this.project;

            // Pass the debug launch targets to the debugger
            System.IServiceProvider sp = this.project as System.IServiceProvider;
            IVsDebugger4 debugger4 = (IVsDebugger4)sp.GetService(typeof(SVsShellDebugger));
            VsDebugTargetProcessInfo[] results = new VsDebugTargetProcessInfo[1];

            debugger4.LaunchDebugTargets4(1, appPackageDebugTarget, results);

            return VSConstants.S_OK;
        }

        public int QueryDebugLaunch(uint grfLaunch, out int pfCanLaunch)
        {
            IVsDebuggableProjectCfg cfg = _baseCfg as IVsDebuggableProjectCfg;
            if (cfg != null)
            {
                return cfg.QueryDebugLaunch(grfLaunch, out pfCanLaunch);
            }
            pfCanLaunch = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region ISpecifyPropertyPages Members

        public void GetPages(CAUUID[] pPages)
        {
            var cfg = _baseCfg as ISpecifyPropertyPages;
            if (cfg != null)
            {
                cfg.GetPages(pPages);
            }
        }

        #endregion

        #region IVsSpecifyProjectDesignerPages Members

        public int GetProjectDesignerPages(CAUUID[] pPages)
        {
            var cfg = _baseCfg as IVsSpecifyProjectDesignerPages;
            if (cfg != null)
            {
                return cfg.GetProjectDesignerPages(pPages);
            }
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsCfgBrowseObject Members

        public int GetCfg(out IVsCfg ppCfg)
        {
            ppCfg = this;
            return VSConstants.S_OK;
        }

        public int GetProjectItem(out IVsHierarchy pHier, out uint pItemid)
        {
            var cfg = _baseCfg as IVsCfgBrowseObject;
            if (cfg != null)
            {
                return cfg.GetProjectItem(out pHier, out pItemid);
            }
            pHier = null;
            pItemid = 0;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsDeployableProjectCfg Members

        int IVsDeployableProjectCfg.AdviseDeployStatusCallback(IVsDeployStatusCallback pIVsDeployStatusCallback, out uint pdwCookie)
        {
            if (pIVsDeployStatusCallback == null)
            {
                pdwCookie = 0;
                return VSConstants.E_UNEXPECTED;
            }

            lock (syncObject)
            {
                pdwCookie = deployCallbackCollection.Add(pIVsDeployStatusCallback);
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.Commit(uint dwReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.QueryStartDeploy(uint dwOptions, int[] pfSupported, int[] pfReady)
        {
            if (pfSupported.Length > 0)
            {
                // Only Appx package producing appcontainer projects should support deployment
                pfSupported[0] = 1;
            }

            if (pfReady.Length > 0)
            {
                lock (syncObject)
                {
                    pfReady[0] = (deployOp == null && appContainerBootstrapperOperation == null) ? 1 : 0;
                }
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.QueryStatusDeploy(out int pfDeployDone)
        {
            lock (syncObject)
            {
                pfDeployDone = (deployOp == null && appContainerBootstrapperOperation == null) ? 1 : 0;
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.Rollback(uint dwReserved)
        {
            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.StartDeploy(IVsOutputWindowPane pIVsOutputWindowPane, uint dwOptions)
        {
            int continueOn = 1;

            outputWindow = pIVsOutputWindowPane;

            if (connection != null)
            {
                lock (syncObject)
                {
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
            }

            // Loop through deploy status callbacks
            foreach (IVsDeployStatusCallback callback in deployCallbackCollection)
            {
                if (ErrorHandler.Failed(callback.OnStartDeploy(ref continueOn)))
                {
                    continueOn = 0;
                }

                if (continueOn == 0)
                {
                    outputWindow = null;
                    return VSConstants.E_ABORT;
                }
            }

            try
            {
                VsDebugTargetInfo2[] targets;
                uint deployFlags = (uint)(_AppContainerDeployOptions.ACDO_NetworkLoopbackEnable | _AppContainerDeployOptions.ACDO_SetNetworkLoopback | _AppContainerDeployOptions.ACDO_ForceCleanLayout);
                string recipeFile = null;
                string layoutDir = null;
                EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
                EnvDTE.Project project = dte.Solution.Projects.Item(1);

                string uniqueName = project.UniqueName;
                IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                IVsHierarchy hierarchy;
                solution.GetProjectOfUniqueName(uniqueName, out hierarchy);
                string canonicalName;
                IVsBuildPropertyStorage bps = hierarchy as IVsBuildPropertyStorage;

                int hr = QueryDebugTargets(out targets);
                if (ErrorHandler.Failed(hr))
                {
                    NotifyEndDeploy(0);
                    return hr;
                }

                get_CanonicalName(out canonicalName);
                bps.GetPropertyValue("AppxPackageRecipe", canonicalName, (uint)_PersistStorageType.PST_PROJECT_FILE, out recipeFile);
                bps.GetPropertyValue("LayoutDir", canonicalName, (uint)_PersistStorageType.PST_PROJECT_FILE, out layoutDir);

                string projectUniqueName = null;
                IVsSolution vsSolution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (vsSolution != null)
                {
                    hr = vsSolution.GetUniqueNameOfProject((IVsHierarchy)this.project, out projectUniqueName);
                    
                }

                PrepareNodeStartupInfo();

                System.IServiceProvider sp = this.project as System.IServiceProvider;
                IVsAppContainerProjectDeploy deployHelper = (IVsAppContainerProjectDeploy)sp.GetService(typeof(SVsAppContainerProjectDeploy));
                if (String.IsNullOrEmpty(targets[0].bstrRemoteMachine))
                {
                    deployOp = deployHelper.StartDeployAsync(deployFlags, recipeFile, layoutDir, projectUniqueName, this);
                }
                else
                {
                    IVsDebuggerDeploy deploy = (IVsDebuggerDeploy)sp.GetService(typeof(SVsShellDebugger));
                    IVsDebuggerDeployConnection deployConnection;
                    bool useAuthentication = false; 
                    VsDebugRemoteAuthenticationMode am = useAuthentication ? VsDebugRemoteAuthenticationMode.VSAUTHMODE_WindowsAuthentication : VsDebugRemoteAuthenticationMode.VSAUTHMODE_None;
                    hr = deploy.ConnectToTargetComputer(targets[0].bstrRemoteMachine, am, out deployConnection);
                    if (ErrorHandler.Failed(hr))
                    {
                        NotifyEndDeploy(0);
                        return hr;
                    }

                    connection = deployConnection;

                    deployOp = deployHelper.StartRemoteDeployAsync(deployFlags, connection, recipeFile, projectUniqueName, this);
                }
            }
            catch (Exception)
            {
                if (connection != null)
                {
                    lock (syncObject)
                    {
                        if (connection != null)
                        {
                            connection.Dispose();
                            connection = null;
                        }
                    }
                }
                connection = null;

                NotifyEndDeploy(0);
                throw;
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.StopDeploy(int fSync)
        {
            IVsTask localAppContainerBootstrapperOperation = null;
            IVsAppContainerProjectDeployOperation localAppContainerDeployOperation = null;
            lock (syncObject)
            {
                localAppContainerBootstrapperOperation = appContainerBootstrapperOperation;
                localAppContainerDeployOperation = deployOp;
                appContainerBootstrapperOperation = null;
                deployOp = null;
            }

            if (localAppContainerBootstrapperOperation != null)
            {
                localAppContainerBootstrapperOperation.Cancel();
                if (fSync != 0)
                {
                    try
                    {
                        localAppContainerBootstrapperOperation.Wait();
                    }
                    catch
                    {
                    }
                }
            }

            if (localAppContainerDeployOperation != null)
            {
                localAppContainerDeployOperation.StopDeploy(fSync != 0);
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.UnadviseDeployStatusCallback(uint dwCookie)
        {
            lock (syncObject)
            {
                deployCallbackCollection.RemoveAt(dwCookie);
            }

            return VSConstants.S_OK;
        }

        int IVsDeployableProjectCfg.WaitDeploy(uint dwMilliseconds, int fTickWhenMessageQNotEmpty)
        {
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsProjectCfgDebugTargetSelection Members
        void IVsProjectCfgDebugTargetSelection.GetCurrentDebugTarget(out Guid pguidDebugTargetType, out uint pDebugTargetTypeId, out string pbstrCurrentDebugTarget)
        {
            IVsBuildPropertyStorage bps = this;

            pguidDebugTargetType = VSConstants.AppPackageDebugTargets.guidAppPackageDebugTargetCmdSet;
            pDebugTargetTypeId = VSConstants.AppPackageDebugTargets.cmdidAppPackage_RemoteMachine;
            pbstrCurrentDebugTarget = RemoteTarget;
        }

        Array IVsProjectCfgDebugTargetSelection.GetDebugTargetListOfType(Guid guidDebugTargetType, uint debugTargetTypeId)
        {
            string[] result = new string[1];
            if (guidDebugTargetType != VSConstants.AppPackageDebugTargets.guidAppPackageDebugTargetCmdSet)
            {
                return new string[0];
            }

            switch (debugTargetTypeId)
            {
                case VSConstants.AppPackageDebugTargets.cmdidAppPackage_RemoteMachine:
                    result[0] = RemoteTarget;
                    break;
                default:
                    return new string[0];
            }

            return result;
        }

        bool IVsProjectCfgDebugTargetSelection.HasDebugTargets(IVsDebugTargetSelectionService pDebugTargetSelectionService, out Array pbstrSupportedTargetCommandIDs)
        {
            pbstrSupportedTargetCommandIDs = new string[] {
                String.Join(":", VSConstants.AppPackageDebugTargets.guidAppPackageDebugTargetCmdSet, VSConstants.AppPackageDebugTargets.cmdidAppPackage_LocalMachine),
                String.Join(":", VSConstants.AppPackageDebugTargets.guidAppPackageDebugTargetCmdSet, VSConstants.AppPackageDebugTargets.cmdidAppPackage_RemoteMachine)
            };

            return true;
        }

        void IVsProjectCfgDebugTargetSelection.SetCurrentDebugTarget(Guid guidDebugTargetType, uint debugTargetTypeId, string bstrCurrentDebugTarget)
        {

        }

        #endregion

        #region IVsAppContainerProjectDeployCallback Members

        void IVsAppContainerProjectDeployCallback.OnEndDeploy(bool successful, string deployedPackageMoniker, string deployedAppUserModelID)
        {
            try
            {
                if (successful)
                {
                    deployPackageMoniker = deployedPackageMoniker;
                    deployAppUserModelID = deployedAppUserModelID;
                    NotifyEndDeploy(1);
                }
                else
                {
                    deployPackageMoniker = null;
                    deployAppUserModelID = null;
                    NotifyEndDeploy(0);
                }
            }
            finally
            {
                lock (syncObject)
                {
                    deployOp = null;

                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
            }
        }

        void IVsAppContainerProjectDeployCallback.OutputMessage(string message)
        {
            if (null != outputWindow)
            {
                outputWindow.OutputString(message);
            }
        }

        #endregion

        #region IVsBuildPropertyStorage Members

        int IVsBuildPropertyStorage.GetItemAttribute(uint item, string attributeName, out string attributeValue)
        {
            attributeValue = null;
            IVsBuildPropertyStorage cfg = _innerCfg as IVsBuildPropertyStorage;
            if (cfg != null)
            {
                return cfg.GetItemAttribute(item, attributeName, out attributeValue);
            }
            return VSConstants.S_OK;
        }

        int IVsBuildPropertyStorage.GetPropertyValue(string propertyName, string configName, uint storage, out string propertyValue)
        {
            propertyValue = null;
            IVsBuildPropertyStorage cfg = _innerCfg as IVsBuildPropertyStorage;
            if (cfg != null)
            {
                return cfg.GetPropertyValue(propertyName, configName, storage, out propertyValue);
            }
            return VSConstants.S_OK;
        }

        int IVsBuildPropertyStorage.RemoveProperty(string propertyName, string configName, uint storage)
        {
            return ((IVsBuildPropertyStorage)this).SetPropertyValue(propertyName, configName, storage, null);
        }

        int IVsBuildPropertyStorage.SetItemAttribute(uint item, string attributeName, string attributeValue)
        {
            IVsBuildPropertyStorage cfg = _innerCfg as IVsBuildPropertyStorage;
            if (cfg != null)
            {
                return cfg.SetItemAttribute(item, attributeName, attributeValue);
            }
            return VSConstants.S_OK;
        }

        int IVsBuildPropertyStorage.SetPropertyValue(string propertyName, string configName, uint storage, string propertyValue)
        {
            IVsBuildPropertyStorage cfg = _innerCfg as IVsBuildPropertyStorage;
            if (cfg != null)
            {
                return cfg.SetPropertyValue(propertyName, configName, storage, propertyValue);
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsQueryDebuggableProjectCfg Members
        int IVsQueryDebuggableProjectCfg.QueryDebugTargets(uint grfLaunch, uint cTargets, VsDebugTargetInfo2[] rgDebugTargetInfo, uint[] pcActual)
        {
            string debuggerMachineName;

            if (pcActual != null && pcActual.Length > 0)
            {
                pcActual[0] = 1;
            }

            grfLaunch |= (uint)__VSDBGLAUNCHFLAGS140.DBGLAUNCH_ContainsStartupTask;

            if (rgDebugTargetInfo != null && rgDebugTargetInfo.Length > 0)
            {
                IList<Guid> debugEngineGuids = new Guid[] { VSConstants.DebugEnginesGuids.Script_guid };

                rgDebugTargetInfo[0] = new VsDebugTargetInfo2();


                IVsBuildPropertyStorage bps = this;//.project as IVsBuildPropertyStorage;      
                rgDebugTargetInfo[0].bstrExe = "winuniversalnode.exe";
                propertiesList.TryGetValue(NodejsUwpConstants.DebuggerMachineName, out debuggerMachineName);
                rgDebugTargetInfo[0].bstrRemoteMachine = debuggerMachineName;
                rgDebugTargetInfo[0].guidLaunchDebugEngine = debugEngineGuids[0];
                rgDebugTargetInfo[0].guidPortSupplier = NativePortSupplier;
                rgDebugTargetInfo[0].fSendToOutputWindow = 1;
                rgDebugTargetInfo[0].dwDebugEngineCount = (uint)debugEngineGuids.Count;
                rgDebugTargetInfo[0].pDebugEngines = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Guid)) * debugEngineGuids.Count);

                for (var i = 0; i < debugEngineGuids.Count; i++)
                {
                    Marshal.StructureToPtr(debugEngineGuids[i],
                        IntPtr.Add(rgDebugTargetInfo[0].pDebugEngines, i * Marshal.SizeOf(typeof(Guid))),
                        false);
                }
            }

            return VSConstants.S_OK;
        }

        #endregion

        #region IPersistXMLFragment Members

        public int InitNew(ref Guid guidFlavor, uint storage)
        {
            //Return, if it is our guid.
            if (IsMyFlavorGuid(ref guidFlavor))
            {
                return VSConstants.S_OK;
            }

            //Forward the call to inner flavor(s).
            if (this._innerCfg != null
                && this._innerCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)this._innerCfg)
                    .InitNew(ref guidFlavor, storage);
            }

            return VSConstants.S_OK;
        }

        public int IsFragmentDirty(uint storage, out int pfDirty)
        {
            pfDirty = 0;
            switch (storage)
            {
                // Specifies storage file type to project file.
                case (uint)_PersistStorageType.PST_PROJECT_FILE:
                    if (isDirty)
                    {
                        pfDirty |= 1;
                    }
                    break;

                // Specifies storage file type to user file.
                case (uint)_PersistStorageType.PST_USER_FILE:
                    // Do not store anything in the user file.
                    break;
            }

            // Forward the call to inner flavor(s) 
            if (pfDirty == 0 && this._innerCfg != null
                && this._innerCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)this._innerCfg)
                    .IsFragmentDirty(storage, out pfDirty);
            }
            return VSConstants.S_OK;

        }

        public int Load(ref Guid guidFlavor, uint storage, string pszXMLFragment)
        {
            if (IsMyFlavorGuid(ref guidFlavor))
            {
                switch (storage)
                {
                    case (uint)_PersistStorageType.PST_PROJECT_FILE:
                        // Load our data from the XML fragment.
                        XmlDocument doc = new XmlDocument();
                        XmlNode node = doc.CreateElement(this.GetType().Name);
                        node.InnerXml = pszXMLFragment;
                        if (node == null || node.FirstChild == null)
                            break;

                        // Load all the properties
                        foreach (XmlNode child in node.FirstChild.ChildNodes)
                        {
                            propertiesList.Add(child.Name, child.InnerText);
                        }
                        break;
                    case (uint)_PersistStorageType.PST_USER_FILE:
                        // Do not store anything in the user file.
                        break;
                }
            }

            // Forward the call to inner flavor(s)
            if (this._innerCfg != null
                && this._innerCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)this._innerCfg)
                    .Load(ref guidFlavor, storage, pszXMLFragment);
            }

            return VSConstants.S_OK;

        }

        public int Save(ref Guid guidFlavor, uint storage, out string pbstrXMLFragment, int fClearDirty)
        {
            pbstrXMLFragment = null;

            if (IsMyFlavorGuid(ref guidFlavor))
            {
                switch (storage)
                {
                    case (uint)_PersistStorageType.PST_PROJECT_FILE:
                        // Create XML for our data (a string and a bool).
                        XmlDocument doc = new XmlDocument();
                        XmlNode root = doc.CreateElement(this.GetType().Name);

                        foreach (KeyValuePair<string, string> property in propertiesList)
                        {
                            XmlNode node = doc.CreateElement(property.Key);
                            node.AppendChild(doc.CreateTextNode(property.Value));
                            root.AppendChild(node);
                        }

                        doc.AppendChild(root);
                        // Get XML fragment representing our data
                        pbstrXMLFragment = doc.InnerXml;

                        if (fClearDirty != 0)
                            isDirty = false;
                        break;
                    case (uint)_PersistStorageType.PST_USER_FILE:
                        // Do not store anything in the user file.
                        break;
                }
            }

            // Forward the call to inner flavor(s)
            if (this._innerCfg != null
                && this._innerCfg is IPersistXMLFragment)
            {
                return ((IPersistXMLFragment)this._innerCfg)
                    .Save(ref guidFlavor, storage, out pbstrXMLFragment, fClearDirty);
            }

            return VSConstants.S_OK;
        }

        #endregion

        private bool IsMyFlavorGuid(ref Guid guidFlavor)
        {
            return guidFlavor.Equals(
                GuidList.guidNodejsUwpProjectFactory);
        }

        internal int QueryDebugTargets(out VsDebugTargetInfo2[] targets)
        {
            IntPtr queryDebuggableProjectCfgPtr = IntPtr.Zero;
            targets = null;

            Guid guid = typeof(IVsQueryDebuggableProjectCfg).GUID;
            int hr = get_CfgType(ref guid, out queryDebuggableProjectCfgPtr);
            if (ErrorHandler.Failed(hr))
                return hr;

            object queryDebuggableProjectCfgObject = Marshal.GetObjectForIUnknown(queryDebuggableProjectCfgPtr);
            if (queryDebuggableProjectCfgObject == null)
                return VSConstants.E_UNEXPECTED;

            IVsQueryDebuggableProjectCfg baseQueryDebugbableCfg = queryDebuggableProjectCfgObject as IVsQueryDebuggableProjectCfg;
            if (baseQueryDebugbableCfg == null)
                return VSConstants.E_UNEXPECTED;

            uint[] targetsCountOutput = new uint[1];
            hr = baseQueryDebugbableCfg.QueryDebugTargets(0, 0, null, targetsCountOutput);
            if (ErrorHandler.Failed(hr))
                return hr;
            uint numberOfDebugTargets = targetsCountOutput[0];

            targets = new VsDebugTargetInfo2[numberOfDebugTargets];
            hr = baseQueryDebugbableCfg.QueryDebugTargets(0, numberOfDebugTargets, targets, null);
            if (ErrorHandler.Failed(hr))
                return hr;

            if (string.IsNullOrEmpty(targets[0].bstrRemoteMachine))
            {
                MessageBox.Show(
                    "The project cannot be deployed or debugged because there is no remote machine specified in project settings.",
                    "Node Tools for Visual Studio", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return VSConstants.E_ABORT;
            }

            return hr;
        }

        private void NotifyEndDeploy(int success)
        {
            foreach (IVsDeployStatusCallback callback in deployCallbackCollection)
            {
                callback.OnEndDeploy(success);
            }

            outputWindow = null;

        }

        private void PrepareNodeStartupInfo()
        {
            string targetDir = null;
            string startupInfoFile = "startupinfo.xml";
            string startupFile = null;
            string nodeExeArguments;
            propertiesList.TryGetValue(NodejsUwpConstants.NodeExeArguments, out nodeExeArguments);
            string scriptArguments;
            propertiesList.TryGetValue(NodejsUwpConstants.ScriptArguments, out scriptArguments);
            string canonicalName = null;
            EnvDTE.DTE dte = (EnvDTE.DTE)Package.GetGlobalService(typeof(EnvDTE.DTE));
            EnvDTE.Project project = dte.Solution.Projects.Item(1);

            string uniqueName = project.UniqueName;
            IVsSolution solution = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            IVsHierarchy hierarchy;
            solution.GetProjectOfUniqueName(uniqueName, out hierarchy);
            IVsBuildPropertyStorage bps = hierarchy as IVsBuildPropertyStorage;

            get_CanonicalName(out canonicalName);
            bps.GetPropertyValue("TargetDir", canonicalName, (uint)_PersistStorageType.PST_PROJECT_FILE, out targetDir);

            string nodeStartupInfoFilePath = string.Format(CultureInfo.InvariantCulture, "{0}{1}", targetDir, startupInfoFile);

            // Get values to put into startupinfo.xml
            bps.GetPropertyValue("StartupFile", canonicalName, (uint)_PersistStorageType.PST_PROJECT_FILE, out startupFile);

            XDocument xdoc = new XDocument();

            XElement srcTree = new XElement("StartupInfo",
                new XElement("Script", startupFile),
                new XElement("NodeOptions", nodeExeArguments),
                new XElement("ScriptArgs", scriptArguments)
            );

            xdoc.Add(srcTree);
            
            // Make sure file is writable
            File.SetAttributes(nodeStartupInfoFilePath, FileAttributes.Normal);

            xdoc.Save(nodeStartupInfoFilePath);
        }

        public string this[string propertyName]
        {
            get
            {
                if (propertiesList.ContainsKey(propertyName))
                {
                    return propertiesList[propertyName];
                }
                return String.Empty;
            }
            set
            {
                // Don't do anything if there isn't any real change
                if (this[propertyName] == value)
                {
                    return;
                }

                isDirty = true;
                if (propertiesList.ContainsKey(propertyName))
                {
                    propertiesList.Remove(propertyName);
                }
                propertiesList.Add(propertyName, value);
            }
        }

        internal static NodejsUwpProjectFlavorCfg GetPropertyPageFromIVsCfg(IVsCfg configuration)
        {
            if (mapIVsCfgToNodejsUwpProjectFlavorCfg.ContainsKey(configuration))
            {
                return (NodejsUwpProjectFlavorCfg)mapIVsCfgToNodejsUwpProjectFlavorCfg[configuration];
            }
            else
            {
                throw new ArgumentOutOfRangeException("Cannot find configuration in mapIVsCfgToSpecializedCfg.");
            }
        }
    }
}
