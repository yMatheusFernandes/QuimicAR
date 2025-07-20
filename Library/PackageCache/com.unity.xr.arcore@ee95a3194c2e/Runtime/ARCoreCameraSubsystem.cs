using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using Unity.Collections;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// The ARCore implementation of the
    /// [`XRCameraSubsystem`](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem).
    /// Do not create this directly. Use the
    /// [`SubsystemManager`](xref:UnityEngine.SubsystemManager)
    /// instead.
    /// </summary>
    [Preserve]
    public sealed class ARCoreCameraSubsystem : XRCameraSubsystem
    {
        /// <summary>
        /// The identifying name for the camera-providing implementation.
        /// </summary>
        /// <value>
        /// The identifying name for the camera-providing implementation.
        /// </value>
        const string k_SubsystemId = "ARCore-Camera";

        /// <summary>
        /// The name of the shader for rendering the camera texture before opaques render.
        /// </summary>
        /// <value>
        /// The name of the shader for rendering the camera texture.
        /// </value>
        const string k_BeforeOpaquesBackgroundShaderName = "Unlit/ARCoreBackground";

        /// <summary>
        /// The name of the shader for rendering the camera texture after opaques have rendered.
        /// </summary>
        /// <value>
        /// The name of the shader for rendering the camera texture.
        /// </value>
        const string k_AfterOpaquesBackgroundShaderName = "Unlit/ARCoreBackground/AfterOpaques";

        enum CameraConfigurationResult
        {
            /// <summary>
            /// Setting the camera configuration was successful.
            /// </summary>
            Success = 0,

            /// <summary>
            /// The given camera configuration was not valid to be set by the provider.
            /// </summary>
            InvalidCameraConfiguration = 1,

            /// <summary>
            /// The provider session was invalid.
            /// </summary>
            InvalidSession = 2,

            /// <summary>
            /// An error occurred because the user did not dispose of all <c>XRCpuImage</c> and did not allow all
            /// asynchronous conversion jobs to complete before changing the camera configuration.
            /// </summary>
            ErrorImagesNotDisposed = 3,
        }

        /// <summary>
        /// The name for the background shader based on the current render pipeline.
        /// </summary>
        /// <value>
        /// The name for the background shader based on the current render pipeline. Or, <c>null</c> if the current
        /// render pipeline is incompatible with the set of shaders.
        /// </value>
        /// <remarks>
        /// The value for the <c>GraphicsSettings.currentRenderPipeline</c> is not expected to change within the lifetime
        /// of the application.
        /// </remarks>
        [Obsolete("'backgroundShaderName' is obsolete, use 'backgroundShaderNames' instead. (2022/04/04)")]
        public static string backgroundShaderName => k_BeforeOpaquesBackgroundShaderName;

        /// <summary>
        /// The names for the background shaders based on the current render pipeline.
        /// </summary>
        /// <value>
        /// The names for the background shaders based on the current render pipeline. Or, <c>null</c> if the current
        /// render pipeline is incompatible with the set of shaders.
        /// </value>
        /// <remarks>
        /// The value for the <c>GraphicsSettings.currentRenderPipeline</c> is not expected to change within the lifetime
        /// of the application.
        ///
        /// There are two shaders in the Google ARCore Provider Package. One is used for rendering
        /// before opaques and one is used for rendering after opaques.
        ///
        /// In order:
        /// 1. Before Opaque Shader Name
        /// 2. After Opaque Shader Name
        /// </remarks>
        public static readonly string[] backgroundShaderNames =
        {
            k_BeforeOpaquesBackgroundShaderName,
            k_AfterOpaquesBackgroundShaderName
        };

        /// <summary>
        /// Creates and registers the camera subsystem descriptor to advertise a providing implementation for camera
        /// functionality.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            if (!Api.platformAndroid || !Api.loaderPresent)
                return;

            var cameraSubsystemCinfo = new XRCameraSubsystemDescriptor.Cinfo
            {
                id = k_SubsystemId,
                providerType = typeof(ARCoreCameraSubsystem.ARCoreProvider),
                subsystemTypeOverride = typeof(ARCoreCameraSubsystem),
                supportsAverageBrightness = true,
                supportsAverageColorTemperature = false,
                supportsColorCorrection = true,
                supportsDisplayMatrix = true,
                supportsProjectionMatrix = true,
                supportsTimestamp = true,
                supportsCameraConfigurations = true,
                supportsCameraImage = true,
                supportsAverageIntensityInLumens = false,
                supportsFocusModes = true,
                supportsFaceTrackingAmbientIntensityLightEstimation = true,
                supportsFaceTrackingHDRLightEstimation = false,
                supportsWorldTrackingAmbientIntensityLightEstimation = true,
                supportsWorldTrackingHDRLightEstimation = true,
                supportsCameraGrain = false,
                supportsExifData = true,
                supportsCameraTorchMode = true,
                // uses delegate because support query need ARSession and cannot be determined on load
                supportsImageStabilizationDelegate = NativeApi.UnityARCore_Camera_GetImageStabilizationSupported,
            };

            XRCameraSubsystemDescriptor.Register(cameraSubsystemCinfo);
        }

        /// <summary>
        /// Invoked from native code just before this subsystem calls
        /// [ArSession_getSupportedCameraConfigsWithFilter](https://developers.google.com/ar/reference/c/group/ar-session#arsession_getsupportedcameraconfigswithfilter).
        /// </summary>
        /// <remarks>
        /// This allows you to customize the
        /// [ArCameraConfigFilter](https://developers.google.com/ar/reference/c/group/ar-camera-config-filter)
        /// passed to
        /// [ArSession_getSupportedCameraConfigsWithFilter](https://developers.google.com/ar/reference/c/group/ar-session#arsession_getsupportedcameraconfigswithfilter).
        ///
        /// Do not destroy the <see cref="ARCoreBeforeGetCameraConfigurationEventArgs.filter"/> object in this
        /// callback. Doing so is undefined behavior and may crash.
        ///
        /// The filter provided by <see cref="ARCoreBeforeGetCameraConfigurationEventArgs.filter"/> is only guaranteed
        /// to exist for the duration of this callback. Accessing it from outside this callback is undefined behavior.
        /// </remarks>
        /// <value>An Action delegate that you can use to modify the camera config filter.</value>
        public event Action<ARCoreBeforeGetCameraConfigurationEventArgs> beforeGetCameraConfiguration
        {
            add => ((ARCoreProvider)provider).beforeGetCameraConfiguration += value;
            remove => ((ARCoreProvider)provider).beforeGetCameraConfiguration -= value;
        }

        class ARCoreProvider : Provider
        {
            /// <summary>
            /// The shader property name for the main texture of the camera video frame.
            /// </summary>
            const string k_MainTexPropertyName = "_MainTex";

            /// <summary>
            /// The name of the camera permission for Android.
            /// </summary>
            const string k_CameraPermissionName = "android.permission.CAMERA";

            /// <summary>
            /// The shader property name identifier for the main texture of the camera video frame.
            /// </summary>
            static readonly int k_MainTexPropertyNameId = Shader.PropertyToID(k_MainTexPropertyName);

            /// <summary>
            /// The shader keyword for enabling image stabilization mode when rendering the camera background.
            /// </summary>
            const string k_ImageStabilizationEnabledMaterialKeyword = "ARCORE_IMAGE_STABILIZATION_ENABLED";

            /// <summary>
            /// The shader keywords for enabling image stabilization rendering.
            /// </summary>
            static readonly List<string> k_StabilizationEnabledKeywordList = new() { k_ImageStabilizationEnabledMaterialKeyword };
            static readonly ReadOnlyList<string> k_StabilizationEnabledKeywordListReadOnly = new(k_StabilizationEnabledKeywordList);

            static readonly XRShaderKeywords k_StabilizationEnabledKeywords =
                new(k_StabilizationEnabledKeywordListReadOnly, null);

            static readonly XRShaderKeywords k_StabilizationDisabledKeywords =
                new(null, k_StabilizationEnabledKeywordListReadOnly);

            Material GetOrCreateCameraMaterial()
            {
                switch (currentBackgroundRenderingMode)
                {
                    case XRCameraBackgroundRenderingMode.BeforeOpaques:
                        return m_BeforeOpaqueCameraMaterial ??= CreateCameraMaterial(k_BeforeOpaquesBackgroundShaderName);

                    case XRCameraBackgroundRenderingMode.AfterOpaques:
                        return m_AfterOpaqueCameraMaterial ??= CreateCameraMaterial(k_AfterOpaquesBackgroundShaderName);

                    default:
                        Debug.LogError($"Unable to create material for unknown background rendering mode {currentBackgroundRenderingMode}.");
                        return null;
                }
            }

            /// <remarks>
            /// This subsystem will lazily create the camera materials depending on the <see cref="currentBackgroundRenderingMode"/>.
            /// Once created, the materials exist for the lifespan of this subsystem.
            /// </remarks>
            public override Material cameraMaterial => GetOrCreateCameraMaterial();

            Material m_BeforeOpaqueCameraMaterial;
            Material m_AfterOpaqueCameraMaterial;

            /// <summary>
            /// Determine whether camera permission has been granted.
            /// </summary>
            /// <returns>
            /// <see langword="true"/> if camera permission has been granted for this app. Otherwise, <see langword="false"/>.
            /// </returns>
            public override bool permissionGranted => ARCorePermissionManager.IsPermissionGranted(k_CameraPermissionName);

            public override bool invertCulling => NativeApi.UnityARCore_Camera_ShouldInvertCulling();

            public override XRCpuImage.Api cpuImageApi => ARCoreCpuImageApi.instance;

            /// <summary>
            /// Describes the subsystem's current (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode).
            /// </summary>
            /// <remarks>
            /// If the <see cref="requestedBackgroundRenderingMode"/> is set to (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode.Any)
            /// then this subsystem will default to (xref: UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode.BeforeOpaques).
            /// </remarks>
            public override XRCameraBackgroundRenderingMode currentBackgroundRenderingMode
            {
                get
                {
                    switch (m_RequestedCameraRenderingMode)
                    {
                        case XRSupportedCameraBackgroundRenderingMode.Any:
                        case XRSupportedCameraBackgroundRenderingMode.BeforeOpaques:
                            return XRCameraBackgroundRenderingMode.BeforeOpaques;

                        case XRSupportedCameraBackgroundRenderingMode.AfterOpaques:
                            return XRCameraBackgroundRenderingMode.AfterOpaques;

                        case XRSupportedCameraBackgroundRenderingMode.None:
                        default:
                            return XRCameraBackgroundRenderingMode.None;
                    }
                }
            }

            public override XRSupportedCameraBackgroundRenderingMode requestedBackgroundRenderingMode
            {
                get => m_RequestedCameraRenderingMode;
                set => m_RequestedCameraRenderingMode = value;
            }

            /// <summary>
            /// Attempts to get the platform specific rendering parameters for rendering the camera background texture.
            /// </summary>
            /// <param name="cameraBackgroundRenderingParameters">
            /// The platform specific rendering parameters for rendering the camera background texture.
            /// </param>
            /// <returns>
            /// <see langword="true"/> if the platform provides specialized rendering parameters or <see langword="false"/> otherwise.
            /// </returns>
            /// <remarks>
            /// ARCore provides a specialized rendering path for rendering the camera background texture when <see cref="imageStabilizationEnabled"/>
            /// is <see langword="true"/>. In this case, ARCore will render the camera background texture to a mesh that is provided
            /// by ARCore with specialized UVW coordinates and perspective correction.
            /// </remarks>
            public override bool TryGetRenderingParameters(out XRCameraBackgroundRenderingParams cameraBackgroundRenderingParameters)
            {
                if (!imageStabilizationEnabled || ARCoreImageStabilizationUtils.ImageStabilizationMesh == null)
                {
                    cameraBackgroundRenderingParameters = default;
                    return false;
                }

                var zTranslation = currentBackgroundRenderingMode == XRCameraBackgroundRenderingMode.AfterOpaques ? 1f : 0f;
                cameraBackgroundRenderingParameters = new XRCameraBackgroundRenderingParams(
                    ARCoreImageStabilizationUtils.ImageStabilizationMesh,
                    // Place the mesh at the proper distance from the camera for a given render mode.
                    Matrix4x4.Translate(new Vector3(0f, 0f, zTranslation)),
                    Matrix4x4.identity,
                    ARCoreImageStabilizationUtils.k_OrthographicProjectionGlesNdc);
                return true;
            }

            XRSupportedCameraBackgroundRenderingMode m_RequestedCameraRenderingMode = XRSupportedCameraBackgroundRenderingMode.Any;

            public override XRSupportedCameraBackgroundRenderingMode supportedBackgroundRenderingMode
                => XRSupportedCameraBackgroundRenderingMode.Any;

            static Action<IntPtr, ArSession, ArCameraConfigFilter> s_OnBeforeGetCameraConfigurationDelegate = OnBeforeGetCameraConfiguration;

            GCHandle m_GCHandle;

            /// <summary>
            /// Construct the camera functionality provider for ARCore.
            /// </summary>
            public ARCoreProvider()
            {
                NativeApi.UnityARCore_Camera_Construct(k_MainTexPropertyNameId);
                m_GCHandle = GCHandle.Alloc(this);
            }

            protected override bool TryInitialize()
            {
                NativeApi.UnityARCore_Camera_SetMeshUpdateFunction(
                    ARCoreImageStabilizationUtils.UpdateBackgroundGeometry);
                return base.TryInitialize();
            }

            /// <summary>
            /// Get the camera facing direction.
            /// </summary>
            public override Feature currentCamera => NativeApi.GetCurrentFacingDirection();

            /// <summary>
            /// Get the currently active camera or set the requested camera
            /// </summary>
            public override Feature requestedCamera
            {
                get => Api.GetRequestedFeatures();
                set
                {
                    Api.SetFeatureRequested(Feature.AnyCamera, false);
                    Api.SetFeatureRequested(value, true);
                }
            }

            public override void Start() => NativeApi.UnityARCore_Camera_Start();
            public override void Stop() => NativeApi.UnityARCore_Camera_Stop();

            public override void Destroy()
            {
                NativeApi.UnityARCore_Camera_Destruct();
                if (m_GCHandle.IsAllocated)
                {
                    m_GCHandle.Free();
                }
                m_GCHandle = default;
            }

            public override bool TryGetFrame(XRCameraParams cameraParams, out XRCameraFrame cameraFrame)
                => NativeApi.UnityARCore_Camera_TryGetFrame(cameraParams, out cameraFrame);

            public override bool autoFocusRequested
            {
                get => Api.GetRequestedFeatures().All(Feature.AutoFocus);
                set => Api.SetFeatureRequested(Feature.AutoFocus, value);
            }

            public override bool autoFocusEnabled => NativeApi.GetAutoFocusEnabled();

            public override bool imageStabilizationRequested
            {
                get => Api.GetRequestedFeatures().All(Feature.ImageStabilization);
                set => Api.SetFeatureRequested(Feature.ImageStabilization, value);
            }

            public override bool imageStabilizationEnabled => NativeApi.UnityARCore_Camera_GetImageStabilizationEnabled();

            public override XRCameraTorchMode requestedCameraTorchMode
            {
                get => Api.GetRequestedFeatures().All(Feature.CameraTorch) ? XRCameraTorchMode.On : XRCameraTorchMode.Off;
                set => Api.SetFeatureRequested(Feature.CameraTorch, (value == XRCameraTorchMode.On ? true : false));
            }

            public override XRCameraTorchMode currentCameraTorchMode
            {
                get {
                    return NativeApi.UnityARCore_Camera_GetCameraTorchMode();
                }
            }

            public override bool DoesCurrentCameraSupportTorch()
                => NativeApi.UnityARCore_Camera_GetSupportsCameraTorchMode() == Supported.Supported;

            /// <summary>
            /// Called on the render thread by background rendering code immediately before the background
            /// is rendered.
            /// For ARCore, this is required in order to submit the GL commands for waiting on the fence
            /// created on the main thread after calling ArPresto_Update().
            /// </summary>
            /// <param name="id">Platform-specific data.</param>
            public override void OnBeforeBackgroundRender(int id)
            {
                NativeApi.UnityARCore_Camera_GetFenceWaitHandler(id);
            }

            [Obsolete]
            public override void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                if (imageStabilizationEnabled)
                {
                    enabledKeywords = k_StabilizationEnabledKeywordList;
                    disabledKeywords = null;
                }
                else
                {
                    enabledKeywords = null;
                    disabledKeywords = k_StabilizationEnabledKeywordList;
                }
            }

            [Obsolete]
            public override ShaderKeywords GetShaderKeywords()
            {
                return imageStabilizationEnabled
                    ? new ShaderKeywords(k_StabilizationEnabledKeywordList.AsReadOnly(), null)
                    : new ShaderKeywords(null, k_StabilizationEnabledKeywordList.AsReadOnly());
            }

            public override XRShaderKeywords GetShaderKeywords2()
            {
                return imageStabilizationEnabled ? k_StabilizationEnabledKeywords : k_StabilizationDisabledKeywords;
            }

            public override Feature requestedLightEstimation
            {
                get => Api.GetRequestedFeatures();
                set
                {
                    Api.SetFeatureRequested(Feature.AnyLightEstimation, false);
                    Api.SetFeatureRequested(value, true);
                }
            }

            public override Feature currentLightEstimation => NativeApi.GetCurrentLightEstimation();

            public override bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
                => NativeApi.UnityARCore_Camera_TryGetIntrinsics(out cameraIntrinsics);

            public override NativeArray<XRCameraConfiguration> GetConfigurations(
                XRCameraConfiguration defaultCameraConfiguration,
                Allocator allocator)
            {
                IntPtr configurations = NativeApi.UnityARCore_Camera_AcquireConfigurations(
                    out int configurationsCount, out int configurationSize);
                try
                {
                    unsafe
                    {
                        return NativeCopyUtility.PtrToNativeArrayWithDefault(
                            defaultCameraConfiguration,
                            (void*)configurations,
                            configurationSize,
                            configurationsCount,
                            allocator);
                    }
                }
                finally
                {
                    NativeApi.UnityARCore_Camera_ReleaseConfigurations(configurations);
                }
            }

            /// <summary>
            /// The current camera configuration.
            /// </summary>
            /// <value>
            /// The current camera configuration if it exists. Otherwise, <c>null</c>.
            /// </value>
            /// <exception cref="System.ArgumentException">Thrown when setting the current configuration if the given
            /// configuration is not a valid, supported camera configuration.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown when setting the current configuration if the
            /// implementation is unable to set the current camera configuration for various reasons such as:
            /// <list type="bullet">
            /// <item><description>ARCore session is invalid</description></item>
            /// <item><description>Captured <c>XRCpuImage</c> have not been disposed</description></item>
            /// </list>
            /// </exception>
            /// <seealso cref="TryAcquireLatestCpuImage"/>
            public override XRCameraConfiguration? currentConfiguration
            {
                get
                {
                    if (TryGetCurrentConfiguration(out XRCameraConfiguration cameraConfiguration))
                    {
                        return cameraConfiguration;
                    }

                    return null;
                }
                set
                {
                    // Assert that the camera configuration is not null.
                    // The XRCameraSubsystem should have already checked this.
                    Debug.Assert(value != null, "Cannot set the current camera configuration to null");

                    switch (NativeApi.UnityARCore_Camera_TrySetCurrentConfiguration((XRCameraConfiguration)value))
                    {
                        case CameraConfigurationResult.Success:
                            break;
                        case CameraConfigurationResult.InvalidCameraConfiguration:
                            throw new ArgumentException(
                                "Camera configuration does not exist in the available configurations", nameof(value));
                        case CameraConfigurationResult.InvalidSession:
                            throw new InvalidOperationException(
                                "Cannot set camera configuration because the ARCore session is not valid");
                        case CameraConfigurationResult.ErrorImagesNotDisposed:
                            throw new InvalidOperationException(
                                "Cannot set camera configuration because you have not disposed of all XRCpuImage" +
                                " and allowed all asynchronous conversion jobs to complete");
                        default:
                            throw new InvalidOperationException("Cannot set camera configuration for ARCore");
                    }
                }
            }

            public override unsafe NativeArray<XRTextureDescriptor> GetTextureDescriptors(
                XRTextureDescriptor defaultDescriptor,
                Allocator allocator)
            {
                int length, elementSize;
                var textureDescriptors = NativeApi.UnityARCore_Camera_AcquireTextureDescriptors(
                    out length, out elementSize);

                try
                {
                    return NativeCopyUtility.PtrToNativeArrayWithDefault(
                        defaultDescriptor,
                        textureDescriptors, elementSize, length, allocator);
                }
                finally
                {
                    NativeApi.UnityARCore_Camera_ReleaseTextureDescriptors(textureDescriptors);
                }
            }

            public override bool TryAcquireLatestCpuImage(out XRCpuImage.Cinfo cameraImageCinfo)
                => ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.Camera, out cameraImageCinfo);

            event Action<ARCoreBeforeGetCameraConfigurationEventArgs> m_BeforeGetCameraConfiguration;

            internal event Action<ARCoreBeforeGetCameraConfigurationEventArgs> beforeGetCameraConfiguration
            {
                add
                {
                    m_BeforeGetCameraConfiguration += value;
                    NativeApi.SetOnBeforeGetCameraConfigurationCallback(s_OnBeforeGetCameraConfigurationDelegate, (IntPtr)m_GCHandle);
                }
                remove
                {
                    m_BeforeGetCameraConfiguration -= value;
                    if (m_BeforeGetCameraConfiguration == null)
                    {
                        NativeApi.SetOnBeforeGetCameraConfigurationCallback(null, IntPtr.Zero);
                    }
                }
            }

            [MonoPInvokeCallback(typeof(Action<IntPtr, ArSession, ArCameraConfigFilter>))]
            static void OnBeforeGetCameraConfiguration(IntPtr providerHandle, ArSession session, ArCameraConfigFilter filter)
            {
                if (GCHandle.FromIntPtr(providerHandle).Target is ARCoreProvider provider)
                {
                    provider.m_BeforeGetCameraConfiguration?.Invoke(new ARCoreBeforeGetCameraConfigurationEventArgs
                    {
                        session = session,
                        filter = filter
                    });
                }
            }
        }

        internal static bool TryGetCurrentConfiguration(out XRCameraConfiguration configuration)
            => NativeApi.UnityARCore_Camera_TryGetCurrentConfiguration(out configuration);

        static class NativeApi
        {
            [DllImport(Constants.k_LibraryName, EntryPoint = "UnityARCore_Camera_SetOnBeforeGetCameraConfigurationCallback")]
            public static extern void SetOnBeforeGetCameraConfigurationCallback(
                Action<IntPtr, ArSession, ArCameraConfigFilter> OnBeforeGetCameraConfigFilterDelegate, IntPtr providerHandle);

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_Construct(int mainTexPropertyNameId);

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_Destruct();

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_Start();

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_Stop();

            [DllImport(Constants.k_LibraryName)]
            public static extern bool UnityARCore_Camera_TryGetFrame(
                XRCameraParams cameraParams, out XRCameraFrame cameraFrame);

            [DllImport(Constants.k_LibraryName, EntryPoint = "UnityARCore_Camera_GetAutoFocusEnabled")]
            public static extern bool GetAutoFocusEnabled();

            [DllImport(Constants.k_LibraryName)]
            public static extern XRCameraTorchMode UnityARCore_Camera_GetCameraTorchMode();

            [DllImport(Constants.k_LibraryName)]
            public static extern Supported UnityARCore_Camera_GetSupportsCameraTorchMode();

            [DllImport(Constants.k_LibraryName)]
            public static extern bool UnityARCore_Camera_GetImageStabilizationEnabled();

            [DllImport(Constants.k_LibraryName)]
            public static extern Supported UnityARCore_Camera_GetImageStabilizationSupported();

            [DllImport(Constants.k_LibraryName, EntryPoint = "UnityARCore_Camera_GetCurrentLightEstimation")]
            public static extern Feature GetCurrentLightEstimation();

            [DllImport(Constants.k_LibraryName)]
            public static extern bool UnityARCore_Camera_TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics);

            [DllImport(Constants.k_LibraryName)]
            public static extern IntPtr UnityARCore_Camera_AcquireConfigurations(
                out int configurationsCount, out int configurationSize);

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_ReleaseConfigurations(IntPtr configurations);

            [DllImport(Constants.k_LibraryName)]
            public static extern bool UnityARCore_Camera_TryGetCurrentConfiguration(
                out XRCameraConfiguration cameraConfiguration);

            [DllImport(Constants.k_LibraryName)]
            public static extern CameraConfigurationResult UnityARCore_Camera_TrySetCurrentConfiguration(
                XRCameraConfiguration cameraConfiguration);

            [DllImport(Constants.k_LibraryName)]
            public static extern unsafe void* UnityARCore_Camera_AcquireTextureDescriptors(
                out int length, out int elementSize);

            [DllImport(Constants.k_LibraryName)]
            public static extern unsafe void UnityARCore_Camera_ReleaseTextureDescriptors(
                void* descriptors);

            [DllImport(Constants.k_LibraryName)]
            public static extern bool UnityARCore_Camera_ShouldInvertCulling();

            [DllImport(Constants.k_LibraryName, EntryPoint = "UnityARCore_Camera_GetCurrentFacingDirection")]
            public static extern Feature GetCurrentFacingDirection();

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_GetFenceWaitHandler(int unused);

            [DllImport(Constants.k_LibraryName)]
            public static extern void UnityARCore_Camera_SetMeshUpdateFunction(
                Action<IntPtr, int, IntPtr, int> updateMeshDelegate);
        }
    }
}
